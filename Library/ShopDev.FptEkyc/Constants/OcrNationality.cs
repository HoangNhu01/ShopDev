namespace CR.FptEkyc.Constants
{
    public static class OcrNationality
    {
        public const string VietNam = "Việt Nam";

        public static string ConvertStandard(string input)
        {
            if (input != "N/A")
                return input;
            return VietNam;
        }
    }
}
