using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Foundation.Collections;

namespace System.IO.IsolatedStorage
{
    public class IsolatedStorageSettings
    {
        public static IPropertySet ApplicationSettings { get { return ApplicationData.Current.LocalSettings.Values; } }
    }
}
