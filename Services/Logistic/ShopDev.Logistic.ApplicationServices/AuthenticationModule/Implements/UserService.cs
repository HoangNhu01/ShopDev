using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase.Common;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.RoleDto;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto;
using ShopDev.Authentication.ApplicationServices.Common;
using ShopDev.Authentication.Domain.Users;
using ShopDev.Constants.ErrorCodes;
using ShopDev.Constants.SysVar;
using ShopDev.Constants.Users;
using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.InfrastructureBase.Files;
using ShopDev.S3Bucket;
using ShopDev.Utils.DataUtils;
using ShopDev.Utils.Linq;
using ShopDev.Utils.Security;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Implements
{
    public class UserService : AuthenticationServiceBase, IUserService
    {
        private readonly IManagerFile _s3ManagerFile;

        public UserService(
            ILogger<UserService> logger,
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContext,
            IS3ManagerFile s3ManagerFile
        )
            : base(logger, httpContext)
        {
            _s3ManagerFile = s3ManagerFile;
        }

        public User ValidateAdminUser(string username, string password)
        {
            _logger.LogInformation($"{nameof(ValidateAdminUser)}: username = {username}");
            var user =
                FindEntities<User>(u => u.Username == username)
                ?? throw new UserFriendlyException(ErrorCode.UsernameOrPasswordIncorrect);
            if (
                new int[] { UserStatus.TEMP, UserStatus.TEMP_OTP, UserStatus.LOCK }.Contains(
                    user.Status
                )
            )
            {
                throw new UserFriendlyException(ErrorCode.UserNotFound);
            }
            else if (!new int[] { UserTypes.ADMIN, UserTypes.SUPER_ADMIN }.Contains(user.UserType))
            {
                throw new UserFriendlyException(ErrorCode.UserLoginUserTypeInvalid);
            }
            else if (!PasswordHasher.VerifyPassword(password, user.Password))
            {
                HandleIncorrectPassword(user);
            }
            else if (user.Status == UserStatus.DEACTIVE)
            {
                throw new UserFriendlyException(ErrorCode.UserIsDeactive);
            }
            return user;
        }

        public User ValidateAppUser(string username, string password)
        {
            _logger.LogInformation($"{nameof(ValidateAppUser)}: username = {username}");
            var user =
                _dbContext.Users.FirstOrDefault(u => u.Username == username && !u.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            
            if (new int[] { UserStatus.TEMP, UserStatus.TEMP_OTP }.Contains(user.Status))
            {
                throw new UserFriendlyException(ErrorCode.UserNotFound);
            }
            else if (!PasswordHasher.VerifyPassword(password, user.Password))
            {
                HandleIncorrectPassword(user);
            }
            else if (user.Status == UserStatus.DEACTIVE)
            {
                throw new UserFriendlyException(ErrorCode.UserIsDeactive);
            }
            else if (user.Status == UserStatus.LOCK)
            {
                throw new UserFriendlyException(ErrorCode.UserIsLock);
            }
            user.LoginFailCount = 0;
            user.DateTimeLoginFailCount = new DateTime(0, DateTimeKind.Local);
            // Nếu là lần đầu đăng nhập đối với Tài khoản được tạo trên CMS
            if (user.IsFirstTime)
            {
                user.IsFirstTime = false;
                _dbContext.SaveChanges();
                // Trả về param Token
                user.IsFirstTime = true;
            }
            return user;
        }

        private void HandleIncorrectPassword(User user)
        {
            if (user.DateTimeLoginFailCount < DateTimeUtils.GetDate() && user.LoginFailCount != 0)
            {
                user.LoginFailCount = 0;
                user.DateTimeLoginFailCount = new DateTime(0, DateTimeKind.Local);
            }
            user.LoginFailCount++;
            var loginMaxTurn = GetLimitedInputTurn(VarNames.LOGINMAXTURN);
            user.DateTimeLoginFailCount = DateTimeUtils.GetDate().AddMinutes(15);
            if (user.LoginFailCount >= loginMaxTurn)
            {
                user.Status = UserStatus.DEACTIVE;
                user.LoginFailCount = 0;
                user.DateTimeLoginFailCount = new DateTime(0, DateTimeKind.Local);
            }
            _dbContext.SaveChanges();
            if (user.Status == UserStatus.DEACTIVE)
            {
                throw new UserFriendlyException(ErrorCode.UserIsDeactive);
            }
            if (user.LoginFailCount == 1)
            {
                throw new UserFriendlyException(ErrorCode.UsernameOrPasswordIncorrect);
            }
            throw new UserFriendlyException(
                ErrorCode.AppPasswordIncorrect,
                loginMaxTurn.ToString(),
                (loginMaxTurn - user.LoginFailCount).ToString()
            );
        }

        public virtual void CreateUser(CreateUserDto input)
        {
            _logger.LogInformation(
                $"{nameof(CreateUser)}: input = {JsonSerializer.Serialize(input)}"
            );
            input.Password = PasswordHasher.HashPassword(input.Password);
            var user = _mapper.Map<User>(input);
            if (_dbContext.Users.Any(u => u.Username == input.Username && !u.Deleted))
            {
                throw new UserFriendlyException(ErrorCode.UsernameHasBeenUsed);
            }
            var transaction = _dbContext.Database.BeginTransaction();
            if (input.Status == null)
            {
                user.Status = UserStatus.ACTIVE;
            }
            user.UserType = UserTypes.ADMIN;
            var result = _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            //Thêm role
            if (input.RoleIds != null)
            {
                foreach (var item in input.RoleIds.Distinct())
                {
                    var role =
                        _dbContext.Roles.FirstOrDefault(e => e.Id == item)
                        ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);
                    _dbContext.UserRoles.Add(
                        new UserRole { UserId = result.Entity.Id, RoleId = item }
                    );
                }
                _dbContext.SaveChanges();
            }
            transaction.Commit();
        }

        public User FindUserAuthorizatonById(int id)
        {
            _logger.LogInformation($"{nameof(FindUserAuthorizatonById)}: id = {id}");
            var user =
                _dbContext.Users.FirstOrDefault(u =>
                    u.Id == id && u.Status == UserStatus.ACTIVE && !u.Deleted
                ) ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            if (
                new int[] { UserStatus.TEMP, UserStatus.TEMP_OTP, UserStatus.LOCK }.Contains(
                    user.Status
                )
            )
            {
                throw new UserFriendlyException(ErrorCode.UserNotFound);
            }
            else if (user.Status == UserStatus.DEACTIVE)
            {
                throw new UserFriendlyException(ErrorCode.UserIsDeactive);
            }
            return user;
        }

        public UserDto GetById(int id)
        {
            _logger.LogInformation($"{nameof(GetById)}: id = {id}");

            var user = (
                from u in _dbContext.Users
                where u.Id == id && !u.Deleted
                select new UserDto
                {
                    Id = u.Id,
                    AvatarImageUri = u.AvatarImageUri,
                    Username = u.Username,
                    UserType = u.UserType,
                }
            ).FirstOrDefault();

            return user ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
        }

        public UserDto FindByUser()
        {
            var userId = _httpContext.GetCurrentUserId();
            _logger.LogInformation($"{nameof(FindByUser)}: userId = {userId}");

            var user = (
                from u in _dbContext.Users
                where u.Id == userId && !u.Deleted && u.Status == UserStatus.ACTIVE
                select new UserDto
                {
                    Id = u.Id,
                    AvatarImageUri = u.AvatarImageUri,
                    Username = u.Username,
                    UserType = u.UserType,
                    IsPasswordTemp = u.IsPasswordTemp,
                    RoleNames = (
                        from userRole in _dbContext.UserRoles
                        join role in _dbContext.Roles on userRole.RoleId equals role.Id
                        where userRole.UserId == userId && !userRole.Deleted && !role.Deleted
                        select role.Name
                    ).AsEnumerable(),
                }
            ).FirstOrDefault();

            return user ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
        }

        public PagingResult<UserDto> FindAll(FilterUserDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");
            var result = new PagingResult<UserDto>();

            var users =
                from user in _dbContext.Users
                let roleIds = _dbContext
                    .UserRoles.Where(e => e.UserId == user.Id && !e.Deleted)
                    .Select(e => e.RoleId)
                    .AsEnumerable()
                where
                    !user.Deleted
                    && user.UserType == UserTypes.ADMIN
                    && (input.Username == null || user.Username.Contains(input.Username))
                    && (
                        input.FullName == null
                        || (user.FullName != null && user.FullName.Contains(input.FullName))
                    )
                    && (input.Status == null || user.Status == input.Status)
                select new UserDto
                {
                    Id = user.Id,
                    AvatarImageUri = user.AvatarImageUri,
                    Username = user.Username,
                    UserType = user.UserType,
                    Status = user.Status,
                    RoleNames = _dbContext
                        .Roles.Where(r => roleIds.Contains(r.Id))
                        .Select(r => r.Name)
                        .AsEnumerable(),
                    RoleIds = roleIds,
                };
            // đếm tổng trước khi phân trang
            result.TotalItems = users.Count();
            users = users.OrderDynamic(input.Sort);

            if (input.PageSize != -1)
            {
                users = users.Skip(input.GetSkip()).Take(input.PageSize);
            }
            result.Items = _mapper.Map<IEnumerable<UserDto>>(users);
            return result;
        }

        public void Update(UpdateUserDto input)
        {
            var userId = _httpContext.GetCurrentUserId();
            _logger.LogInformation(
                $"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}, userId = {userId}"
            );

            var user =
                _dbContext.Users.FirstOrDefault(u =>
                    u.Id == input.Id && u.Status == UserStatus.ACTIVE && !u.Deleted
                ) ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            user.FullName = input.FullName;

            //Thêm role
            if (input.RoleIds != null)
            {
                foreach (var item in input.RoleIds)
                {
                    if (_dbContext.Roles.FirstOrDefault(e => e.Id == item) == null)
                        throw new UserFriendlyException(ErrorCode.RoleNotFound);
                }
            }

            var inputUserRole = input.RoleIds ?? [];
            //Danh sách role hiện tại được gán cho user
            var currentUserRole = _dbContext
                .UserRoles.Where(e => e.UserId == input.Id)
                .Select(e => e.RoleId)
                .ToList();

            //Xóa những role gán với user
            var removeUserRole = currentUserRole.Except(inputUserRole).ToList();
            foreach (var item in removeUserRole)
            {
                var userRole = _dbContext.UserRoles.FirstOrDefault(e =>
                    e.UserId == input.Id && e.RoleId == item
                );
                if (userRole != null)
                {
                    userRole.Deleted = true;
                }
            }

            //Thêm những role trong input chưa  có trong db
            var insertUserRole = inputUserRole.Except(currentUserRole).ToList();
            foreach (var item in insertUserRole)
            {
                _dbContext.UserRoles.Add(new UserRole { UserId = input.Id, RoleId = item });
            }

            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var userId = _httpContext.GetCurrentUserId();
            _logger.LogInformation($"{nameof(Delete)}: id = {id}, userId = {userId}");

            var user =
                _dbContext.Users.FirstOrDefault(u => u.Id == id && !u.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            user.Deleted = true;
            _dbContext.SaveChanges();
        }

        public void ChangeStatus(int id)
        {
            var userId = _httpContext.GetCurrentUserId();
            _logger.LogInformation($"{nameof(ChangeStatus)}: id = {id}, userId = {userId}");
            var user =
                _dbContext.Users.FirstOrDefault(u => u.Id == id && !u.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

            if (user.Status == UserStatus.ACTIVE)
            {
                user.Status = UserStatus.DEACTIVE;
            }
            else
            {
                user.Status = UserStatus.ACTIVE;
            }
            _dbContext.SaveChanges();
        }

        public void SetPassword(SetPasswordUserDto input)
        {
            _logger.LogInformation(
                $"{nameof(SetPassword)}: input = {JsonSerializer.Serialize(input)}"
            );

            var user =
                _dbContext.Users.FirstOrDefault(e => e.Id == input.Id && !e.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

            user.Password = PasswordHasher.HashPassword(input.Password);
            user.IsPasswordTemp = input.IsPasswordTemp;
            _dbContext.SaveChanges();
        }

        public void ChangePassword(ChangePasswordDto input)
        {
            var userId = _httpContext.GetCurrentUserId();
            _logger.LogInformation(
                $"{nameof(ChangePassword)}: input = {JsonSerializer.Serialize(input)}, userId = {userId}"
            );
            var user =
                _dbContext.Users.FirstOrDefault(e => e.Id == userId && !e.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

            if (
                !user.IsPasswordTemp
                && !PasswordHasher.VerifyPassword(input.OldPassword!, user.Password)
            )
            {
                throw new UserFriendlyException(ErrorCode.UserOldPasswordIncorrect);
            }

            user.Password = PasswordHasher.HashPassword(input.NewPassword!);
            user.IsPasswordTemp = false;
            _dbContext.SaveChanges();
        }

        public void UpdateUserFullname(UpdateFullNameUserDto input)
        {
            var userId = _httpContext.GetCurrentUserId();
            _logger.LogInformation(
                $"{nameof(UpdateUserFullname)}: id = {input.Id}, userId = {userId}"
            );
            var user =
                _dbContext.Users.FirstOrDefault(u => u.Id == input.Id && !u.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            user.FullName = input.FullName;
            _dbContext.SaveChanges();
        }

        public void Login(int id)
        {
            _logger.LogInformation($"{nameof(Login)}: id = {id}");
            var user =
                _dbContext.Users.FirstOrDefault(u => u.Id == id && !u.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            user.LastLogin = DateTimeUtils.GetDate();
            var contextInfo = _httpContext.HttpContext;

            // Lấy thông tin trình duyệt
            if (contextInfo != null)
            {
                var header = contextInfo.Request.Headers;
                string? operatingSystem = "";
                string? browser = "";
                var platFormCheck = header.ContainsKey("Sec-Ch-Ua-Platform");
                if (platFormCheck)
                {
                    var operatingSystemInfo = contextInfo
                        .Request.Headers["Sec-Ch-Ua-Platform"]
                        .ToString();
                    operatingSystem =
                        operatingSystemInfo.Length > 0
                            ? operatingSystemInfo.Replace("\"", "")
                            : null;
                }
                var browserInfoCheck = header.ContainsKey("Sec-Ch-Ua");
                var browserInfo = contextInfo.Request.Headers["Sec-Ch-Ua"].ToString();
                var browserInfoString =
                    browserInfo.Length > 0 ? browserInfo.Replace("\"", "").Split(',') : null;
                if (browserInfoCheck && browserInfoString != null)
                {
                    browser = browserInfoString.Length > 0 ? browserInfoString[0].Trim() : null;
                }
                user.OperatingSystem = operatingSystem;
                user.Browser = browser;
            }
            _dbContext.SaveChanges();
        }

        public PrivacyInfoDto GetPrivacyInfo()
        {
            var userId = _httpContext.GetCurrentUserId();
            _logger.LogInformation($"{nameof(GetPrivacyInfo)}: userId = {userId}");
            var userFind =
                _dbContext.Users.FirstOrDefault(o => o.Id == userId)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

            var result = new PrivacyInfoDto()
            {
                OperatingSystem = userFind.OperatingSystem,
                Browser = userFind.Browser,
                LastLogin = userFind.LastLogin,
                AvatarImageUri = userFind.AvatarImageUri
            };
            return result;
        }

        public async Task UpdateAvatar(string s3Key)
        {
            var userId = _httpContext.GetCurrentUserId();
            _logger.LogInformation($"{nameof(UpdateAvatar)}: userId = {userId}, s3Key = {s3Key}");
            var user =
                _dbContext.Users.FirstOrDefault(u => u.Id == userId && !u.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            //var returnValueS3Key = await _s3ManagerFile.MoveAsync((s3Key, MediaTypes.Image));
            //var image = returnValueS3Key?.Images?.Find(c => c.S3KeyOld == s3Key);
            //user.AvatarImageUri = image?.Uri;
            //user.S3Key = image?.S3Key;
            _dbContext.SaveChanges();
            await Task.CompletedTask;
        }

        public int GetLimitedInputTurn(string varName)
        {
            var sysVar =
                _dbContext.SysVars.FirstOrDefault(o =>
                    o.GrName == GrNames.AUTHMAXTURN && o.VarName == varName
                ) ?? throw new UserFriendlyException(ErrorCode.SysVarsIsNotConfig);
            return int.Parse(sysVar.VarValue);
        }

        public void UpdateUserStatus(string userName, int userStatus)
        {
            var user =
                _dbContext.Users.FirstOrDefault(u => u.Username == userName && !u.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            user.Status = userStatus;
            _dbContext.SaveChanges();
        }

        public void UpdateUserStatus(int userId, int userStatus)
        {
            var user =
                _dbContext.Users.Find(userId)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            user.Status = userStatus;
            _dbContext.SaveChanges();
        }

        public void UpdateRole(UpdateRoleDto input)
        {
            _logger.LogInformation(
                $"{nameof(UpdateRole)}: input = {JsonSerializer.Serialize(input)}"
            );

            var inputUserRole = input.RoleIds ?? [];
            //Danh sách role hiện tại được gán cho user
            var currentUserRole = _dbContext
                .UserRoles.Where(e => e.UserId == input.UserId && !e.Deleted)
                .Select(e => e.RoleId)
                .ToList();

            //Xóa những role gán với user
            var removeUserRole = currentUserRole.Except(inputUserRole).ToList();
            foreach (var item in removeUserRole)
            {
                _dbContext
                    .UserRoles.Where(e =>
                        e.UserId == input.UserId && e.RoleId == item && !e.Deleted
                    )
                    .ToList()
                    .ForEach(o => o.Deleted = true);
            }

            //Thêm những role trong input chưa  có trong db
            var insertUserRole = inputUserRole.Except(currentUserRole).ToList();
            foreach (var item in insertUserRole)
            {
                _dbContext.UserRoles.Add(new UserRole { UserId = input.UserId, RoleId = item });
            }

            _dbContext.SaveChanges();
        }
    }
}
