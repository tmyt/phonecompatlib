using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Microsoft.Phone.Controls
{
    public class TiltEffect
    {


        public static bool GetIsTiltEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsTiltEnabledProperty);
        }

        public static void SetIsTiltEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsTiltEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsTiltEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTiltEnabledProperty =
            DependencyProperty.RegisterAttached("IsTiltEnabled", typeof(bool), typeof(TiltEffect), new PropertyMetadata(false));


    }
}
