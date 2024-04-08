using CR.EntitiesBase.Base;

namespace CR.PVCB.Constants
{
    public class PvcbErrorCode : IErrorCode
    {
        //HttpCode = 201
        public const int WaitForConfirmation = 14000;

        //Code 04,Không có HttpStatuscode
        public const int TransactionPendingPayment = 14001;
        public const int TheTransactionHasBeenRefunded = 14002;
        public const int BackendResponseSuccess = 14003;

        //HttpCode = 400
        public const int NoDataFound = 14004;
        public const int TransactionIdNotFound = 14005;
        public const int OriginalPaymentNeverReceived = 14006;
        public const int JSONRequestInvalid = 14007;
        public const int AmountExceededDailyTransactionLimit = 14008;
        public const int TransactionAmountExceedsTheAllowedLimit = 14009;
        public const int TransactionAmountBelowAllowable = 14010;
        public const int TheAmountInTheAccountIsNotEnoughToMakeTheTransaction = 14011;
        public const int InvalidTransactionAmount = 14012;
        public const int CardNumberOrAccountNotFound = 14013;
        public const int CardNumberOrAccountNotCorrect = 14014;
        public const int CardOrAccountStatusNotValid = 14015;
        public const int CardOrccountTypeNotValid = 14016;
        public const int NameOnCardOrAccountNotCorrect = 14017;
        public const int CardOrAccountNotActivated = 14018;
        public const int CardOrAccountHasBeenBlocked = 14019;
        public const int InsufficientFunds = 14020;
        public const int BeneficiaryBankIsNotValidOrHasNotJoinedTheService = 14021;
        public const int ExpiredBeneficiaryCard = 14022;
        public const int TransactionFailedBecauseDestinationCardIsInLostStatus = 14023;
        public const int CardNumberOrAccountHasExpired = 14024;
        public const int CardNumberIsInLostStatus = 14025;
        public const int TransactionAmountExceedsTheAllowedLimitForTheDay = 14026;
        public const int NumberOfTransactionsRxceedingTheAllowedLimitInADay = 14027;
        public const int TimeoutFromBeneficiaryBank = 14028;
        public const int NoTransactionProcessingStatusMessagesFromNapas = 14029;
        public const int ThisTransactionDoesNotSupportRefund = 14030;
        public const int NotEligibleToUseTheService = 14031;
        public const int TransactionIsDuplicate = 14032;
        public const int ClientIDInvalid = 14033;

        //Không có HttpStatus
        public const int MACInvalid = 14034;

        //HttpStatus = 500
        public const int UnableToConnectToNapasSystem = 14035;
        public const int UnableToParseMessageFromNapas = 14036;
        public const int UnableToConnectToT24System = 14037;
        public const int UnableToConnectToERSSystem = 14038;

        //Không có HttpStatus
        public const int GenerateMACFailure = 14039;

        //HttpStatus = 504
        public const int Timeout = 14040;

        //HttpStatus = 500
        public const int UnableToConnectToFESystem = 14041;
        public const int UnableToGetMessageFromNapasSystem = 14042;
        public const int SystemHasAnErrorPleaseContactBankForMoreDetails = 14043;

        //HttpStatus = 501
        public const int TransactionsRejectedByInternalSystems = 14044;
        public const int TransactionsRejectedByExternalSystems = 14045;

        //HttpStatus = 400
        public const int DataCreditorAccountSourceNumberMustNotBeNull = 14046;
        public const int DataCreditorAccountSourceTypeMustNotBeNull = 14047;
        public const int RiskBINIdMustNotBeNull = 14048;
        public const int DataCreditorAccountSourceTypeBeneficiaryAccountTypeACCAccountOrPANCard =
            14049;
        public const int DataTransIdMustNotBeNull = 14050;
        public const int DataDateTimeMustNotBeNull = 14051;
        public const int DataCreditorAccountSourceNumberMustNotBeNull2 = 14052;
        public const int DataCreditorAccountSourceTypeMustNotBeNull2 = 14053;
        public const int DataInstructedAmountAmountMustNotBeNull = 14054;
        public const int DataInstructedAmountAmountIsNotNumber = 14055;
        public const int RiskBINIdMustNotBeNull2 = 14056;
        public const int DataTransIdAlreadyExists = 14057;
        public const int DataDateTimeIsNotInTheCorrectFormat = 14058;
        public const int DataCreditorAccountSourceTypeBeneficiaryAccountTypeACCAccountPANCard =
            14059;
        public const int DataInstructedAmountAmountLimitPerTime = 14060;
        public const int RiskTransDescNeedsToBeLessThan210Characters = 14061;
        public const int RiskMustNotBeNull = 14062;
        public const int RiskTransDescMustNotBeNull = 14063;
        public const int DataCreditorAccountSourceNameMustNotBeNull = 14064;
        public const int RiskTransDescDoNotUseSpecialCharacters = 14065;
        public const int DataTransIdCannotBeANumber = 14066;
        public const int DataTransIdWrongFormat = 14067;
        public const int DataCreditorAccountSourceNumberCannotBeANumber = 14068;
        public const int DataCreditorAccountSourceNumberWrongFormat = 14069;
        public const int RiskTransDescCannotBeANumber = 14070;
        public const int DataCreditorAccountSourceNameCannotBeANumber = 14068;
        public const int SystemErrorPleaseTrAagainLater = 14069;
        public const int SystemError = 14070;

        //Lỗi default
        public const int PvcbBadRequest = 14071;
        public const int PvcbCannotConnectToServer = 14072;
        public const int PvcbNotImplemented = 14073;
        public const int PvcbCreated = 14074;
        public const int PvcbGatewayTimeout = 14075;
        public const int PvcbError = 14076;

        //Lỗi repsonse
        public const int PvcbResponseDataNull = 14077;
        public const int PvcbResponseCredittorAccountNull = 14078;
        public const int PvcbResponseSourceNameNull = 14079;

        public const int EkycErrorHttpRequest = 14080;
        public const int EkycErrorGetInformationUnauthorized = 14081;
        public const int EkycErrorGetInformationNotFound = 14082;

        #region virtual acc
        //Lỗi repsonse
        public const int GetInforNotFound = 14083;
        public const int GetVirtualAccNotFound = 14084;
        public const int GetTransactionVirtualAccNotFound = 14085;
        public const int VAIsClosed = 14086;
        public const int LiveRecordNotChange = 14087;
        public const int NotAllowClose = 14088;
        #endregion
        public const int AnErrorOccurred = 14089;
    }
}
