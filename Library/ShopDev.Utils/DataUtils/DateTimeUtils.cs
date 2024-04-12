namespace ShopDev.Utils.DataUtils
{
    public class DateTimeUtils
    {
        public static DateTime? FromDateStrDD_MM_YYYY_ToDate(string? input)
        {
            if (input == null)
                return null;
            try
            {
                return DateTime.ParseExact(
                    input,
                    "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture
                );
            }
            catch
            {
                try
                {
                    return DateTime.ParseExact(
                        input,
                        "yyyy",
                        System.Globalization.CultureInfo.InvariantCulture
                    );
                }
                catch (Exception)
                {
                    try
                    {
                        return DateTime.ParseExact(
                            input,
                            "dd/MM/yyy",
                            System.Globalization.CultureInfo.InvariantCulture
                        );
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }

        public static DateTime? FromDateStrDD_MM_YY_ToDate(string input)
        {
            try
            {
                return DateTime.ParseExact(
                    input,
                    "dd/MM/yyy",
                    System.Globalization.CultureInfo.InvariantCulture
                );
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? FromStrToDate(string input)
        {
            try
            {
                return Convert.ToDateTime(input);
            }
            catch (Exception)
            {
                try
                {
                    return DateTime.ParseExact(
                        input,
                        "dd/MM/yyy",
                        System.Globalization.CultureInfo.InvariantCulture
                    );
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static DateTime GetDate()
        {
            return DateTime.UtcNow.AddHours(7);
        }
    }
}
