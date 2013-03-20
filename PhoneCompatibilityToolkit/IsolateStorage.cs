
using System.Collections.Generic;
using System.Xml;
namespace System.IO.IsolatedStorage
{
    // Summary:
    //     Provides a System.Collections.Generic.Dictionary<TKey,TValue> that stores
    //     key-value pairs in isolated storage.
    //
    // Exceptions:
    //   System.ArgumentNullException:
    //     key is null. This exception is thrown when you attempt to reference an instance
    //     of the class by using an indexer and the variable you pass in for the key
    //     value is null.
    public class IsolatedStorageSettings : Dictionary<string, object>
    {
        private bool initialized;

        private IsolatedStorageSettings()
        {
            initialized = false;
            Load();
        }

        private async void Load()
        {
            try
            {
                var data = await global::Windows.Storage.ApplicationData.Current.RoamingFolder.OpenStreamForReadAsync("isosettings.xml");
                var xr = XmlReader.Create(data);
            }
            catch (FileNotFoundException e)
            {
                // 
            }
        }

        // Summary:
        //     Gets an instance of System.IO.IsolatedStorage.IsolatedStorageSettings that
        //     contains the contents of the application's System.IO.IsolatedStorage.IsolatedStorageFile,
        //     scoped at the application level, or creates a new instance of System.IO.IsolatedStorage.IsolatedStorageSettings
        //     if one does not exist.
        //
        // Returns:
        //     An System.IO.IsolatedStorage.IsolatedStorageSettings object that contains
        //     the contents of the application's System.IO.IsolatedStorage.IsolatedStorageFile,
        //     scoped at the application level. If an instance does not already exist, a
        //     new instance is created.
        public static IsolatedStorageSettings ApplicationSettings { get { return new IsolatedStorageSettings(); } }
        //
        // Summary:
        //     Saves data written to the current System.IO.IsolatedStorage.IsolatedStorageSettings
        //     object.
        //
        // Exceptions:
        //   System.IO.IsolatedStorage.IsolatedStorageException:
        //     The System.IO.IsolatedStorage.IsolatedStorageFile does not have enough available
        //     free space.
        public void Save()
        {
        }
    }
}