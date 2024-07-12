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

        public UserDto ValidateAdmin(string username, string password)
        {
            _logger.LogInformation($"{nameof(ValidateAdmin)}: username = {username}");
            var user =
                FindEntity<User>(u => u.Username == username, isTracking: true)
                ?? throw new UserFriendlyException(ErrorCode.UsernameOrPasswordIncorrect);

            if (user.UserType != UserTypes.SHOP && user.UserType != UserTypes.SUPER_ADMIN)
            {
                throw new UserFriendlyException(ErrorCode.UserLoginUserTypeInvalid);
            }
            else if (!PasswordHasher.VerifyPassword(password, user.Password))
            {
                HandleIncorrectPassword(user.Id);
            }
            else if (user.Status == UserStatus.DEACTIVE)
            {
                throw new UserFriendlyException(ErrorCode.UserIsDeactive);
            }
            return _mapper.Map<UserDto>(user);
        }

        public UserDto ValidateAppUser(string username, string password)
        {
            _logger.LogInformation($"{nameof(ValidateAppUser)}: username = {username}");
            var user =
                FindEntity<User>(u => u.Username == username, isTracking: true)
                ?? throw new UserFriendlyException(ErrorCode.UsernameOrPasswordIncorrect);

            if (!PasswordHasher.VerifyPassword(password, user.Password))
            {
                HandleIncorrectPassword(user.Id);
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
            return _mapper.Map<UserDto>(user);
        }

        private void HandleIncorrectPassword(int userId)
        {
            var user = _dbContext.Users.Find(userId);
            if (user is not null)
            {
                if (
                    user.DateTimeLoginFailCount < DateTimeUtils.GetDate()
                    && user.LoginFailCount != 0
                )
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
        }

        public virtual async Task Create(CreateUserDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");
            input.Password = PasswordHasher.HashPassword(input.Password);
            if (_dbContext.Users.Any(u => u.Username == input.Username && !u.Deleted))
            {
                throw new UserFriendlyException(ErrorCode.UsernameHasBeenUsed);
            }
            var transaction = _dbContext.Database.BeginTransaction();
            var user = _mapper.Map<User>(input);
            user.UserType = UserTypes.SHOP;
            user.UserRoles.AddRange(
                input.UserRoles.Select(x => new UserRole { RoleId = x.RoleId })
            );
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            transaction.Commit();
        }

        public UserDto FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");
            var user =
                _dbContext.Users.Find(id)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            if (user.Status == UserStatus.DEACTIVE)
            {
                throw new UserFriendlyException(ErrorCode.UserIsDeactive);
            }
            return _mapper.Map<UserDto>(user);
        }

        public UserDto GetById(int id)
        {
            _logger.LogInformation($"{nameof(GetById)}: id = {id}");
            return GetIQueryableResult<UserDto, User>(
                        selector: x =>
                            new()
                            {
                                Id = x.Id,
                                AvatarImageUri = x.AvatarImageUri,
                                Username = x.Username,
                                UserType = x.UserType,
                                Status = x.Status,
                            },
                        predicate: x => x.Id == id
                    )
                    .AsSplitQuery()
                    .FirstOrDefault() ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
        }

        public UserDto FindByUser()
        {
            var userId = _httpContext.GetCurrentUserId();
            _logger.LogInformation($"{nameof(FindByUser)}: userId = {userId}");
            return GetIQueryableResult<UserDto, User>(
                        selector: x =>
                            new()
                            {
                                Id = x.Id,
                                AvatarImageUri = x.AvatarImageUri,
                                Username = x.Username,
                                UserType = x.UserType,
                                Status = x.Status,
                                RoleNames = x.UserRoles.Select(x => x.Role.Name),
                                RoleIds = x.UserRoles.Select(x => x.RoleId),
                            },
                        predicate: x => x.Id == userId,
                        include: x => x.Include(x => x.UserRoles).ThenInclude(x => x.Role)
                    )
                    .AsSplitQuery()
                    .FirstOrDefault() ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
        }

        public PagingResult<UserDto> FindAll(FilterUserDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");
            var result = new PagingResult<UserDto>();
            var users = GetIQueryableResult<UserDto, User>(
                selector: x =>
                    new()
                    {
                        Id = x.Id,
                        AvatarImageUri = x.AvatarImageUri,
                        Username = x.Username,
                        UserType = x.UserType,
                        Status = x.Status,
                        RoleNames = x.UserRoles.Select(x => x.Role.Name),
                        RoleIds = x.UserRoles.Select(x => x.RoleId),
                    },
                predicate: x =>
                    !x.Deleted
                    && x.UserType == UserTypes.SHOP
                    && (string.IsNullOrEmpty(input.Username) || x.Username.Contains(input.Username))
                    && (
                        string.IsNullOrEmpty(input.FullName)
                        || (
                            !string.IsNullOrEmpty(x.FullName) && x.FullName.Contains(input.FullName)
                        )
                    )
                    && (!input.Status.HasValue || x.Status == input.Status),
                include: x => x.Include(x => x.UserRoles).ThenInclude(x => x.Role)
            );

            // đếm tổng trước khi phân trang
            result.TotalItems = users.Count();
            users = users.OrderDynamic(input.Sort);
            if (input.PageSize != -1)
            {
                users = users.Skip(input.GetSkip()).Take(input.PageSize);
            }
            result.Items = users;
            return result;
        }

        public void Update(UpdateUserDto input)
        {
            var userId = _httpContext.GetCurrentUserId();
            _logger.LogInformation(
                $"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}, userId = {userId}"
            );

            var user =
                FindEntity<User>(
                    x => x.Id == input.Id,
                    isTracking: true,
                    include: x => x.Include(x => x.UserRoles).ThenInclude(x => x.Role)
                ) ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            user.FullName = input.FullName;

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

        public void Login(int id)
        {
            _logger.LogInformation($"{nameof(Login)}: id = {id}");
            var user =
                _dbContext.Set<User>().Find(id)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            user.LastLogin = DateTimeUtils.GetDate();
            var contextInfo = _httpContext.HttpContext;

            // Lấy thông tin trình duyệt
            if (contextInfo is not null)
            {
                var header = contextInfo.Request.Headers;
                string? operatingSystem = string.Empty;
                string? browser = string.Empty;
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
                if (browserInfoCheck && browserInfoString is not null)
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
            var user =
                FindEntity<User>(o => o.Id == userId)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

            return new()
            {
                OperatingSystem = user.OperatingSystem,
                Browser = user.Browser,
                LastLogin = user.LastLogin,
                AvatarImageUri = user.AvatarImageUri
            };
        }

        public async Task UpdateAvatar(string s3Key)
        {
            var userId = _httpContext.GetCurrentUserId();
            _logger.LogInformation($"{nameof(UpdateAvatar)}: userId = {userId}, s3Key = {s3Key}");
            var user =
                FindEntity<User>(o => o.Id == userId)
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

        public void UpdateStatus(string userName, int userStatus)
        {
            var user =
                FindEntity<User>(o => o.Username == userName)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            user.Status = userStatus;
            _dbContext.SaveChanges();
        }

        public void UpdateRole(UpdateRoleDto input)
        {
            _logger.LogInformation(
                $"{nameof(UpdateRole)}: input = {JsonSerializer.Serialize(input)}"
            );
            //Danh sách role hiện tại được gán cho user
            List<UserRole> currentUserRoles =
            [
                .. _dbContext
                .UserRoles.Where(e => e.UserId == input.UserId)
            ];
            UpdateItems(
                currentUserRoles,
                input.UserRoles,
                (x, y) => x.Id == y.RoleId,
                (x, y) =>
                {
                    x.Deleted = true;
                }
            );
            currentUserRoles.AddRange(
                input.UserRoles.Select(x => new UserRole
                {
                    UserId = input.UserId,
                    RoleId = x.RoleId
                })
            );
            _dbContext.SaveChanges();
        }
    }
}
