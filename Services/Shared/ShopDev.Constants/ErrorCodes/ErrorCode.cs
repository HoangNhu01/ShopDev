using ShopDev.EntitiesBase.Base;

namespace ShopDev.Constants.ErrorCodes
{
    public class ErrorCode : IErrorCode
    {
        protected ErrorCode() { }

        public const int System = 1;
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int NotFound = 404;
        public const int InternalServerError = 500;

        // Authentication 1xxx
        public const int UsernameOrPasswordIncorrect = 1000;
        public const int UserNotFound = 1001;
        public const int RoleNotFound = 1002;
        public const int UserIsDeactive = 1003;
        public const int InvalidUserType = 1004;
        public const int UserOldPasswordIncorrect = 1005;
        public const int UserNotHavePermission = 1006;
        public const int UsernameHasBeenUsed = 1007;
        public const int UserStatusIsInvalid = 1008;
        public const int UserIsVerify = 1009;
        public const int UserIsRequestVerify = 1010;
        public const int UserEmailIsExist = 1011;
        public const int UserIsRegistered = 1011;
        public const int UserCustomerNotFound = 1012;
        public const int OptCodeNotValid = 1013;
        public const int OptCodeIsExpired = 1014;
        public const int UserCustomerIsVerifyOcr = 1015;
        public const int UserCustomerTypeIsNotPersonal = 1016;
        public const int UserRegisterExistPersonalPhone = 1017;
        public const int UserRegisterExistPersonalEmail = 1018;
        public const int UserIsVerifyCannotDetele = 1019;
        public const int RoleNameExist = 1020;
        public const int UserInvalidCannotRegisterPassword = 1021;
        public const int UserRegisterReferralCodeInvalid = 1022;
        public const int AppPasswordIncorrect = 1023;
        public const int AppConfirmForgotPasswordInvalid = 1024;
        public const int UserLoginUserTypeInvalid = 1025;
        public const int UserChangePasswordFirstTimeInvalid = 1026;
        public const int UserIsLock = 1027;

        // Image && File 2xxx
        public const int FileNotFound = 2000;
        public const int FileMaxLength = 2001;
        public const int FileExtention = 2002;

        public const int SysVarsIsNotConfig = 2003;
    }
}
