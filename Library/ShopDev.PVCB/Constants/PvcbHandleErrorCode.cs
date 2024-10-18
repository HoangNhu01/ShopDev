namespace CR.PVCB.Constants
{
    public static class PvcbHandleErrorCode
    {
        //HttpCode = 201
        public const string WaitForConfirmation = "01";

        //Code 04,Không có HttpStatuscode
        public const string TransactionPendingPayment = "04";
        public const string TheTransactionHasBeenRefunded = "06";
        public const string BackendResponseSuccess = "08";

        //HttpCode = 400
        public const string NoDataFound = "101";
        public const string TransactionIdNotFound = "102";
        public const string OriginalPaymentNeverReceived = "103";

        public const string InnerLockAccMessageCode = "113";
        public const string InnerRegisterVAMessageCode = "134";
        public const string InnerDetailVAMessageCode = "136";
        public const string InnerTransactionMessageCode = "135";
        public const string InnerUnlockAccMessageCode = "121";
        public const string InnerCloseAccMessageCode = "133";

        public const string JSONRequestInvalid = "028";
        public const string AmountExceededDailyTransactionLimit = "301";
        public const string TransactionAmountExceedsTheAllowedLimit = "302";
        public const string TransactionAmountBelowAllowable = "303";
        public const string TheAmountstringheAccountIsNotEnoughToMakeTheTransaction = "304";
        public const string InvalidTransactionAmount = "305";
        public const string CardNumberOrAccountNotFound = "401";
        public const string CardNumberOrAccountNotCorrect = "402";
        public const string CardOrAccountStatusNotValid = "403";
        public const string CardOrccountTypeNotValid = "404";
        public const string NameOnCardOrAccountNotCorrect = "405";
        public const string CardOrAccountNotActivated = "406";
        public const string CardOrAccountHasBeenBlocked = "407";
        public const string InsufficientFunds = "409";
        public const string BeneficiaryBankIsNotValidOrHasNotJoinedTheService = "410";
        public const string ExpiredBeneficiaryCard = "411";
        public const string TransactionFailedBecauseDestinationCardIsInLostStatus = "412";
        public const string CardNumberOrAccountHasExpired = "413";
        public const string CardNumberIsInLostStatus = "414";
        public const string TransactionAmountExceedsTheAllowedLimitForTheDay = "415";
        public const string NumberOfTransactionsRxceedingTheAllowedLimitInADay = "416";
        public const string TimeoutFromBeneficiaryBank = "417";
        public const string NoTransactionProcessingStatusMessagesFromNapas = "418";
        public const string ThisTransactionDoesNotSupportRefund = "501";
        public const string NotEligibleToUseTheService = "502";
        public const string TransactionIsDuplicate = "503";
        public const string ClientIDInvalid = "600";

        //Không có HttpStatus
        public const string MACInvalid = "990";

        //HttpStatus = 500
        public const string UnableToConnectToNapasSystem = "991";
        public const string UnableToParseMessageFromNapas = "992";
        public const string UnableToConnectToT24System = "993";
        public const string UnableToConnectToERSSystem = "994";

        //Không có HttpStatus
        public const string GenerateMACFailure = "995";

        //HttpStatus = 504
        public const string Timeout = "996";

        //HttpStatus = 500
        public const string UnableToConnectToFESystem = "997";
        public const string UnableToGetMessageFromNapasSystem = "68";
        public const string SystemHasAnErrorPleaseContactBankForMoreDetails = "999";

        //HttpStatus = 501
        public const string TransactionsRejectedByInternalSystems = "027";
        public const string TransactionsRejectedByExternalSystems = "029";

        //HttpStatus = 400
        public const string DataCreditorAccountSourceNumberMustNotBeNull = "0001";
        public const string DataCreditorAccountSourceTypeMustNotBeNull = "0002";
        public const string RiskBINIdMustNotBeNull = "0003";
        public const string DataCreditorAccountSourceTypeBeneficiaryAccountTypeACCAccountOrPANCard =
            "0004";
        public const string DataTransIdMustNotBeNull = "0005";
        public const string DataDateTimeMustNotBeNull = "0006";
        public const string DataCreditorAccountSourceNumberMustNotBeNull2 = "0007";
        public const string DataCreditorAccountSourceTypeMustNotBeNull2 = "0008";
        public const string DataInstructedAmountAmountMustNotBeNull = "0009";
        public const string DataInstructedAmountAmountIsNotNumber = "0010";
        public const string RiskBINIdMustNotBeNull2 = "0011";
        public const string DataTransIdAlreadyExists = "0012";
        public const string DataDateTimeIsNotInTheCorrectFormat = "0013";
        public const string DataCreditorAccountSourceTypeBeneficiaryAccountTypeACCAccountPANCard =
            "0014";
        public const string DataInstructedAmountAmountLimitPerTime = "0015";
        public const string RiskTransDescNeedsToBeLessThan210Characters = "0016";
        public const string RiskMustNotBeNull = "0017";
        public const string RiskTransDescMustNotBeNull = "0018";
        public const string DataCreditorAccountSourceNameMustNotBeNull = "0019";
        public const string RiskTransDescDoNotUseSpecialCharacters = "0020";
        public const string DataTransIdCannotBeANumber = "0021";
        public const string DataTransIdWrongFormat = "0022";
        public const string DataCreditorAccountSourceNumberCannotBeANumber = "0023";
        public const string DataCreditorAccountSourceNumberWrongFormat = "024";
        public const string RiskTransDescCannotBeANumber = "0025";
        public const string DataCreditorAccountSourceNameCannotBeANumber = "026";
        public const string SystemErrorPleaseTrAagainLater = "500";
        public const string SystemError = "400";
        #region Success
        public const string SuccessCode = "200";
        #endregion
    }
}
