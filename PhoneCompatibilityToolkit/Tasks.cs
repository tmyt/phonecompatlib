using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace Microsoft.Phone.Tasks
{
    public class WebBrowserTask
    {
        public Uri Uri { get; set; }

        [Obsolete("Please use Uri property")]
        public string URL { get { return Uri.OriginalString; } set { Uri = new Uri(value, UriKind.RelativeOrAbsolute); } }

        public async void Show()
        {
            await Launcher.LaunchUriAsync(Uri);
        }
    }

    public class BingMapsDirectionsTask
    {
        internal LabeledMapLocation Start { get; set; }

        internal LabeledMapLocation End { get; set; }

        internal void Show()
        {
            throw new NotImplementedException();
        }
    }

    class LabeledMapLocation
    {
        private string p;
        private System.Device.Location.GeoCoordinate geoCoordinate;

        public LabeledMapLocation(string p, System.Device.Location.GeoCoordinate geoCoordinate)
        {
            this.p = p;
            this.geoCoordinate = geoCoordinate;
        }
    }
}
