using CR.EntitiesBase.Base;

namespace CR.FptEkyc.Constants
{
    public class FptEkycErrorCode : IErrorCode
    {
        public const int PersonalCustomerOCRIDTypeInvalid = 10000;
        public const int PersonalCustomerOCRDifference = 10001;
        public const int PersonalCustomerOCRBackId = 10002;
        public const int PersonalCustomerOCRPassport = 10003;
        public const int PersonalCustomerFaceRecognitionNoFaceDetected = 10004;
        public const int PersonalCustomerFaceMatch = 10005;
        public const int PersonalCustomerAgeInvalid = 10006;
        public const int PersonalCustomerIdExpired = 10007;
        public const int PersonalCustomerOCRBackIdNotEmpty = 10008;
        public const int PersonalCustomerOCRFrontId = 10009;
        public const int PersonalCustomerFaceRecognitionNotMatch = 10010;
        public const int PersonalCustomerHttpError = 10011;
        public const int OCRNotPermission = 10012;
        public const int OCRErrorFromFPTPoc = 10013;
    }
}
