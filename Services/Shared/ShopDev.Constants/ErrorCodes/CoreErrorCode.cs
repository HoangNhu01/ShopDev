namespace ShopDev.Constants.ErrorCodes
{
    public class CoreErrorCode : ErrorCode
    {
        protected CoreErrorCode()
            : base() { }

        //public const int <Tên Error> = <giá trị>;
        public const int CorePersonalCustomerNotFound = 4000;
        public const int CoreCustomerIdentificationNotFound = 4001;
        public const int CorePersonalCustomerTempStatusInvalid = 4002;
        public const int CoreApproveStatusInitInvalid = 4003;
        public const int CoreBankNotFound = 4004;

        public const int CoreSaleTempNotFound = 4005;
        public const int CoreSaleParentNotFound = 4006;
        public const int CoreSaleTempIsInRequest = 4007;

        public const int CoreCustomerCannotNull = 4008;
        public const int CoreDepartmentNotFound = 4009;
        public const int CorePersonalCustomerExistRequestUpdate = 4010;
        public const int CorePersonalCustomerTempNotFound = 4011;
        public const int CoreBankAccountTempNotFound = 4012;
        public const int CoreBankAccountNotFound = 4013;
        public const int CoreCustomerContactAddressTempNotFound = 4014;
        public const int CoreCustomerContactAddressNotFound = 4015;
        public const int CoreCustomerIdentificationTempNotFound = 4016;

        public const int CoreDepartmentNotFoundToRequest = 4017;
        public const int CoreEmployeeCodeIsExists = 4018;
        public const int CoreSaleTempApproveRequestNotFound = 4019;
        public const int CoreSaleTempNotFoundToRequest = 4020;

        public const int CoreApproveNotFound = 4021;
        public const int CoreCollabContractTemplateNotFound = 4022;

        public const int CorePersonalPhoneHasBeenUsed = 4023;
        public const int CorePersonalEmailHasBeenUsed = 4024;
        public const int CorePersonalHasBeenRequestUpdate = 4025;
        public const int CoreBankAccountIsDefaultCannotDelete = 4026;
        public const int CoreSaleNotFound = 4027;
        public const int CoreSaleIsSameType = 4028;
        public const int CoreChooseTheWrongTypeSale = 4029;
        public const int CoreDontHaveAnySalesInDepartment = 4030;
        public const int CoreSaleManagerNotFound = 4031;
        public const int CoreDepartmentCantCreate = 4032;
        public const int CoreSaleIsInADepartment = 4033;
        public const int CoreSaleCollabContractError = 4034;
        public const int CoreSaleTempCannotUpdate = 4035;

        public const int CoreBusinessCustomerNotFound = 4036;
        public const int CoreBusinessCustomerTempStatusInvalid = 4037;
        public const int CoreBusinessCustomerExistRequestUpdate = 4038;
        public const int CoreIdentificationIsDefaultCannotDelete = 4039;
        public const int CoreCustomerContactAddressIsDefaultCannotDelete = 4040;
        public const int CoreCustomerIdentifcationIsUse = 4041;
        public const int CoreCustomerIdentifcationIsUseInTemp = 4042;
        public const int CoreBusinessCustomerLicenseNotFound = 4043;

        public const int CoreSalerIsParentSale = 4044;
        public const int CoreSaleParentCannotNull = 4045;
        public const int CorePersonalApproveDataTypeInvalid = 4046;

        public const int CoreBusinessBankAccountNotFound = 4047;
        public const int CoreErrorContractUpload = 4048;
        public const int CorePersonalCustomerReferralCodeNotFound = 4049;
        public const int CorePersonalCustomerReferralCodeExist = 4050;
        public const int CoreHistoryUpdateCustomerInfoNotFound = 4051;
        public const int CoreCustomerNotFound = 4052;
        public const int CoreErrorCollabContractFileWordUpload = 4053;
        public const int CoreErrorCollabContractFilePdfUpload = 4054;
        public const int CoreErrorCollabContractFileSignatureUpload = 4055;
        public const int CorePersonalCustomerTempPhoneCannotEmpty = 4056;
        public const int CorePersonalCustomerTempCannotUpdate = 4057;
        public const int CoreCollabContractLinkNotFound = 4058;
        public const int CoreSaleCollabContractNotFound = 4059;
        public const int CoreSaleCollabContractFileWordTempNotFound = 4060;
        public const int CoreCustomerBankAccountIsUse = 4061;
        public const int CoreCustomerBankAccountIsUseInTemp = 4062;
        public const int CoreDepartmentCannotDeleteBecauseHaveChild = 4063;
        public const int CoreDepartmentCannotDeleteBecauseHaveSaler = 4064;
        public const int CorePersonalCustomerUpdateReferralCodeIsMe = 4065;

        public const int CoreDepartmentNameIsExist = 4066;
        public const int CoreSaleTempIsExists = 4067;
        public const int CoreSaleIsExists = 4068;
        public const int CoreStampImageNotFound = 4069;
        public const int CoreStampImageCanNotDelete = 4070;
        public const int CoreFileSaleCollabContractSignatureNotFound = 4071;
        public const int CoreSaleAppIsExisted = 4072;
        public const int CoreSaleNotPermission = 4073;
        public const int CorePersonalCustomerLivenessPhotosInvalid = 4075;

        public const int PinCodeIsNotCorrect = 4076;
        public const int CoreCustomerIsRequestChange = 4077;
        public const int CoreCollabContractIsUsing = 4078;
        public const int CoreEmployeeCodeIsExistsInTemp = 4079;
        public const int CoreCustomerIdentifcationTempCannotAddMutiple = 4080;
        public const int CoreCustomerContactTempCannotAddMutiple = 4081;
        public const int CoreCustomerBankTempCannotAddMutiple = 4082;
        public const int CoreLogoBankNotFound = 4083;
        public const int CoreCannotReadLogoExtension = 4084;

        public const int CoreBusinessCodeExist = 4085;
        public const int CoreBusinessPhoneExist = 4086;
        public const int CoreBusinessMobilePhoneExist = 4087;
        public const int CoreBusinessEmailExist = 4088;

        public const int CoreBusinessCodeTempExist = 4089;
        public const int CoreBusinessPhoneTempExist = 4090;
        public const int CoreBusinessMobilePhoneTempExist = 4091;
        public const int CoreBusinessEmailTempExist = 4092;
        public const int CoreBusinessCustomerLicenseExist = 4093;
        public const int CoreCustomerSaleCannotAddMutiple = 4094;
        public const int CoreCustomerSaleTempNotFound = 4095;
        public const int CoreCustomerSaleNotFound = 4096;
        public const int CoreCustomerItemInfoInTempHasUpdate = 4097;
        public const int CoreBankNotFoundPVcomBank = 4098;
        public const int CoreCustomerRegisterUserNotVefifyOtp = 4099;
        public const int CoreCustomerSaleIsExist = 4100;
        public const int CoreSaleStatusDeactive = 4101;
        public const int CoreSaleManagerNotInDepartment = 4102;
        public const int CorePersonalCustomerAddCustomerSaleIsMe = 4103;
        public const int CoreCustomerInfoUpdateNotDataChange = 4104;
        public const int CoreReferralCodeNotFound = 4105;
    }
}
