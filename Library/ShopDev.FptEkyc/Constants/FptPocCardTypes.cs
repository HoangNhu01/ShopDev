namespace CR.FptEkyc.Constants
{
    /// <summary>
    /// Loại giấy tờ FptPoc
    /// </summary>
    public static class FptPocCardTypes
    {
        public const string CCCD = "cccd";
        public const string CCCD_CHIP = "cccdchip";
        public const string CMND = "cmtnd";
        public const string PASSPORT = "hc";

        public static string GetPocType(string type)
        {
            return type switch
            {
                CardTypesInput.CMND => CMND,
                CardTypesInput.CCCD_CHIP => CCCD_CHIP,
                CardTypesInput.CCCD => CCCD,
                CardTypesInput.PASSPORT => PASSPORT,
                _ => "",
            };
        }
    }
}
