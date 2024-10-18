namespace CR.FptEkyc.Constants
{
    public static class OcrGenders
    {
        public const string MALE = "NAM";
        public const string MALE2 = "NAM/M";

        public const string FEMALE = "NỮ";
        public const string FEMALE2 = "NỮ/F";

        public const string NA = "N/A";

        public static string ConvertStandard(string input)
        {
            if (input == MALE || input == MALE2)
                return MALE;
            else if (input == FEMALE || input == FEMALE2)
                return FEMALE;
            return MALE;
        }
    }
}
