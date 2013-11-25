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
        public LabeledMapLocation Start { get; set; }

        public LabeledMapLocation End { get; set; }

        public async void Show()
        {
            if (Start == null && End == null)
            {
                throw new InvalidOperationException();
            }

            var start = default(string);
            if (Start.Location != null)
            {
                start = string.Format("pos.{0}_{1}", Start.Location.Latitude, Start.Location.Longitude);
            }
            else
            {
                start = string.Format("adr.{0}", Uri.EscapeDataString(Start.Label));
            }

            var end = default(string);
            if (End.Location != null)
            {
                end = string.Format("pos.{0}_{1}", End.Location.Latitude, End.Location.Longitude);
            }
            else
            {
                end = string.Format("adr.{0}", Uri.EscapeDataString(End.Label));
            }

            var url = string.Format("bingmaps:?rtp={0}~{1}", start, end);
            await Launcher.LaunchUriAsync(new Uri(url));
        }
    }

    public class LabeledMapLocation
    {
        private string p;
        private System.Device.Location.GeoCoordinate geoCoordinate;

        /// <summary>
        /// Gets or sets the text label that identifies the associated geographic location.
        /// </summary>
        public string Label 
        { 
            set { p = value; } 
            get { return p; } 
        }
        
        /// <summary>
        /// Gets or sets the geographic coordinate associated with a labeled map location.
        /// </summary>
        public System.Device.Location.GeoCoordinate Location
        { 
            set { geoCoordinate = value; } 
            get { return geoCoordinate; } 
        }

        public LabeledMapLocation(string p, System.Device.Location.GeoCoordinate geoCoordinate)
        {
            this.p = p;
            this.geoCoordinate = geoCoordinate;
        }
    }
}
