using System.Text.RegularExpressions;
using ShopDev.Utils.Validation;

namespace ShopDev.Utils.Security
{
    /// <summary>
    /// Sinh các mã code cho toàn hệ thống
    /// </summary>
    public class GenerateCodes
    {
        /// <summary>
        /// Mã xác thực người dùng
        /// </summary>
        public static string VerificationCode()
        {
            Random generator = new Random();
            string code = generator.Next(0, 1000000).ToString("D6");
            return code;
        }

        public static string ResetPasswordToken()
        {
            Random random = new();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(
                Enumerable.Repeat(chars, 30).Select(s => s[random.Next(s.Length)]).ToArray()
            );
        }

        public static string GetRandomPassword()
        {
            Random random = new();
            const string chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_-+=<>?";
            var plainPassword = "";

            var reg = new Regex(
                RegexPatterns.Password8Characters1Uppercase1Lowercase1Number1Special
            );

            do
            {
                plainPassword = new string(
                    Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray()
                );
            } while (!reg.IsMatch(plainPassword));

            return plainPassword;
        }

        public static string GetRandomPIN()
        {
            Random random = new();
            const string chars = "0123456789";
            var pin = new string(
                Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray()
            );

            return pin;
        }
    }
}
