using System.Text.RegularExpressions;
using System.Web;

namespace ShopDev.Utils.Contracts
{
        public static class ContractDataUtils
        {
                public static Dictionary<string, string> GetParams(string uri)
                {
                        var matches = Regex.Matches(uri, @"[\?&](([^&=]+)=([^&=#]*))", RegexOptions.Compiled);
                        return matches
                            .Cast<Match>()
                            .ToDictionary(
                                m => Uri.UnescapeDataString(m.Groups[2].Value),
                                m => Uri.UnescapeDataString(m.Groups[3].Value)
                            );
                }

                public static string GetFullPathFile(string folder, string fileName, string filePath)
                {
                        var fullPath = Path.Combine(filePath, folder, fileName);
                        return fullPath;
                }

                public static string FindAndReplace(string docText, List<ReplaceTextDto> replateTextDtos)
                {
                        string docTextReplate = docText;
                        foreach (var text in replateTextDtos)
                        {
                                var replaceTextXml = text.ReplaceText;
                                if (text.ReplaceText != null)
                                {
                                        replaceTextXml = HttpUtility.HtmlEncode(text.ReplaceText);
                                }
                                docTextReplate = docTextReplate.Replace(text.FindText!, replaceTextXml);
                        }
                        return docTextReplate;
                }
        }
}
