using System.Reflection;
using System.Xml.Linq;

namespace CR.ApplicationBase.Localization
{
    public class LocalizationBase : ILocalization
    {
        public Dictionary<string, Dictionary<string, string>> Dictionary = new();
        public const string DicNameDefault = LocalizationNames.Vietnamese;
        protected readonly IHttpContextAccessor _httpContext;
        public LocalizationBase(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        public void LoadDictionary(string nameSpace)
        {
            string rootNameSpace = nameSpace ?? throw new ArgumentNullException(nameof(nameSpace));
            var assembly = Assembly.GetCallingAssembly();
            foreach (
                var resourceName in assembly.GetManifestResourceNames().Where(r => r.StartsWith(rootNameSpace))
            )
            {
                using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
                XElement element = XElement.Load(stream);
                var dicName = element.FirstAttribute!.Value;

                var textElements = element.Element("Texts")?.Elements("Text") ?? Enumerable.Empty<XElement>();
                var dicValues = textElements.ToDictionary(
        x => x.Attribute("Name")!.Value,
        x => x.Attribute("value")?.Value ?? x.Value
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
                _httpContext.HttpContext?.Items[LocalizationQuery.QueryName]?.ToString()
                ?? DicNameDefault;
            return Localize(localizationName, keyName);
        }

        public string Localize(string keyName, string[]? listParam)
        {
            string localizationName =
                _httpContext.HttpContext?.Items[LocalizationQuery.QueryName]?.ToString()
                ?? DicNameDefault;
            return string.Format(Localize(localizationName, keyName), listParam ?? []);
        }
    }
}