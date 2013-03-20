using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace System.Windows
{
    public class MessageBox
    {
        public static async void Show(string text)
        {
            await (new MessageDialog(text)).ShowAsync();
        }

        public static async void Show(string text, string title)
        {
            await (new MessageDialog(text, title)).ShowAsync();
        }
    }
}
