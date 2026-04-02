

using System.Reflection;
using System.Xml.Linq;

namespace CR.ApplicationBase.Localization
{
    public class LocalizationBase : ILocalization
    {
        protected Dictionary<string, string> Dictionary = new();
        public const string DicNameDefault = LocalizationName.Vietnamese;

        protected void LoadDictionary(string nameSpace)
        {
            // nameSpace sẽ là tên của file chứa các key và value ngôn ngữ khác nhau, ví dụ: "CR.ApplicationBase.Localization.Vi" hoặc "CR.ApplicationBase.Localization.En"
            string rootNameSpace = nameSpace ?? throw new ArgumentNullException(nameof(nameSpace));
            // assembly sẽ là assembly chứa file đó, ví dụ: assembly của project CR.ApplicationBase
            var assembly = Assembly.GetExecutingAssembly();

            foreach (
                var resourceName in assembly
                .GetManifestResourceNames()
                .Where(x => x.StartsWith(rootNameSpace))
            )
            {
                // Ý nghĩa:
                // Lấy file embedded trong DLL
                // resourceName kiểu như:
                // CR.Localization.vi.xml
                using Stream stream = assembly.GetManifestResourceStream(resourceName)!;

                // Convert stream → XML object
                XElement element = XElement.Load(stream);
                //Lấy attribute đầu tiên của root:
                //<localization name="vi">
                //  Result:
                //  = "vi"
                var dicName = element.FirstAttribute!.Value;

                var dicValues = element
                .Elements("text")
                .Elements("texts")
                .ToDictionary(
                    e => e.Attribute("name")!.Value,
                    e => e.Attribute("value").Value ?? e.Value
                );
                if (!Dictionary.ContainsKey(dicName))
                {
                    Dictionary[dicName] = dicValues;
                }
                else
                {
                    // nếu đã tồn tại rồi thì sẽ ghi đè lên các key đã tồn tại, còn các key mới thì sẽ thêm vào
                    foreach (var item in dicValues)
                    {
                        Dictionary[dicName][item.Key] = item.Value;
                    }
                }
            }

        }


        public string Localize(string keyName)
        {
            throw new NotImplementedException();
        }

        public string Localize(string keyName, string[]? listParams)
        {
            throw new NotImplementedException();
        }
    }
}