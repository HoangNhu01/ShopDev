namespace CR.FptEkyc.Constants
{
    public static class RecognitionUtils
    {
        public static string GetValueStandard(string input)
        {
            if (input != "N/A")
                return input;
            return "";
        }
    }
}
