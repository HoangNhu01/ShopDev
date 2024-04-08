using System.Reflection;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;

namespace ShopDev.ApplicationBase.Localization
{
    public abstract class LocalizationBase : ILocalization
    {
        protected Dictionary<string, Dictionary<string, string>> Dictionary = [];
        public const string DicNameDefault = LocalizationNames.Vietnamese;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected LocalizationBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected void LoadDictionary(string nameSpace)
        {
            string rootNameSpace = nameSpace ?? throw new ArgumentNullException(nameof(nameSpace));
            var assembly = Assembly.GetCallingAssembly();
            foreach (
                var resourceName in assembly
                    .GetManifestResourceNames()
                    .Where(r => r.StartsWith(rootNameSpace))
            )
            {
                using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
                XElement element = XElement.Load(stream);
                var dicName = element.FirstAttribute!.Value;

                var dicValues = element
                    .Elements("texts")
                    .Elements("text")
                    .ToDictionary(
                        e => e.Attribute("name")!.Value,
                        e => e.Attribute("value")?.Value ?? e.Value
                    );

                if (!Dictionary.ContainsKey(dicName))
                {
                    Dictionary[dicName] = dicValues;
                }
                else
                {
                    foreach (var item in dicValues)
                    {
                        Dictionary[dicName][item.Key] = item.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Lấy nội dung từ tên từ điển và tên key
        /// </summary>
        /// <param name="dicName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        private string Localize(string dicName, string keyName)
        {
            try
            {
                return Dictionary[dicName][keyName];
            }
            catch
            {
                return $"{dicName}:{keyName}";
            }
        }

        public string Localize(string keyName)
        {
            string localizationName =
                _httpContextAccessor.HttpContext?.Items[LocalizationQuery.QueryName]?.ToString()
                ?? DicNameDefault;
            return Localize(localizationName, keyName);
        }

        public string Localize(string dicName, string[]? listParam)
        {
            string localizationName =
                _httpContextAccessor.HttpContext?.Items[LocalizationQuery.QueryName]?.ToString()
                ?? DicNameDefault;
            return string.Format(Localize(localizationName, dicName), listParam ?? []);
        }
    }
}
