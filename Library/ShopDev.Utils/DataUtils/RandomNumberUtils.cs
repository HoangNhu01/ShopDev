using System.Text;

namespace ShopDev.Utils.DataUtils
{
    public static class RandomNumberUtils
    {
        public static string RandomNumber(int length)
        {
            Random random = new Random();
            const string chars = "0123456789";
            return new string(
                Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()
            );
        }

        public static string RandomCharNumber(int length, string prefix = "")
        {
            Random random = new Random();
            const string chars = "0123456789ABCDEGIKLMNOPGRSTUVXY";
            return prefix
                + new string(
                    Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()
                );
        }

        public static string GenerateRandomHexString(int length)
        {
            Random random = new Random();
            StringBuilder hexString = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int randomNumber = random.Next(0, 16); // generates a random number between 0 and 15
                hexString.Append(randomNumber.ToString("X")); // converts the number to its hexadecimal representation and appends it
            }

            return hexString.ToString();
        }
    }
}
