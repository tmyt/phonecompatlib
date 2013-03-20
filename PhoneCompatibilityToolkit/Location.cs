using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace System.Device.Location
{
    public enum GeoPositionAccuracy
    {
        Default = 0,
        High = 1,
    }

    public enum GeoPositionPermission
    {
        Unknown = 0,
        Granted = 1,
        Denied = 2,
    }

    public enum GeoPositionStatus
    {
        Disabled = 0,
        Ready = 1,
        Initializing = 2,
        NoData = 3,
    }

    public class GeoPositionChangedEventArgs<T> : EventArgs
    {
        public GeoPositionChangedEventArgs(GeoPosition<T> position) { Position = position; }

        public GeoPosition<T> Position { get; private set; }
    }

    public class GeoPositionStatusChangedEventArgs : EventArgs
    {
        public GeoPositionStatusChangedEventArgs(GeoPositionStatus status) { Status = status; }

        public GeoPositionStatus Status { get; private set; }
    }

    public class GeoCoordinateWatcher : IDisposable, INotifyPropertyChanged
    {
        #region private
        private double GetValue(double? v)
        {
            if (!v.HasValue) return double.NaN;
            return v.Value;
        }
        #endregion

        Geolocator locator_;

        public GeoCoordinateWatcher() { }
        public GeoCoordinateWatcher(GeoPositionAccuracy desiredAccuracy) { DesiredAccuracy = desiredAccuracy; }

        public GeoPositionAccuracy DesiredAccuracy { get; private set; }
        public double MovementThreshold { get; set; }
        public GeoPositionPermission Permission { get; private set; }
        public GeoPosition<GeoCoordinate> Position { get; private set; }
        public GeoPositionStatus Status { get; private set; }

        [SecuritySafeCritical]
        public void Dispose() { }
        [SecuritySafeCritical]
        protected virtual void Dispose(bool disposing) { }

        [SecuritySafeCritical]
        public void Start()
        {
            locator_ = new Geolocator();
            locator_.DesiredAccuracy = PositionAccuracy.Default;
            locator_.PositionChanged += locator__PositionChanged;
            locator_.StatusChanged += locator__StatusChanged;
        }

        [SecuritySafeCritical]
        public void Start(bool suppressPermissionPrompt) { Start(); }
        [SecuritySafeCritical]
        public void Stop() { locator_ = null; }
        [SecuritySafeCritical]
        public bool TryStart(bool suppressPermissionPrompt, TimeSpan timeout) { Start(); return true; }

        #region Events
        public event EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>> PositionChanged;
        public event EventHandler<GeoPositionStatusChangedEventArgs> StatusChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPositionChanged(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Position = new GeoPosition<GeoCoordinate>(e.Position.Timestamp, e.Position.Location);
            var h = PositionChanged;
            if (h != null)
            {
                h(this, e);
            }
        }
        protected void OnPositionStatusChanged(GeoPositionStatusChangedEventArgs e)
        {
            Status = e.Status;
            var h = StatusChanged;
            if (h != null)
            {
                h(this, e);
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            var h = PropertyChanged;
            if (h != null)
            {
                h(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnPositionChanged(Geoposition pos)
        {
            OnPositionChanged(new GeoPositionChangedEventArgs<GeoCoordinate>(new GeoPosition<GeoCoordinate>()
            {
                Location = new GeoCoordinate()
                {
                    Altitude = GetValue(pos.Coordinate.Altitude),
                    Course = GetValue(pos.Coordinate.Heading),
                    HorizontalAccuracy = pos.Coordinate.Accuracy,
                    Latitude = pos.Coordinate.Latitude,
                    Longitude = pos.Coordinate.Longitude,
                    Speed = GetValue(pos.Coordinate.Speed),
                    VerticalAccuracy = GetValue(pos.Coordinate.AltitudeAccuracy)
                },
                Timestamp = pos.Coordinate.Timestamp
            }));
        }
        #endregion

        #region Handlers
        void locator__PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            OnPositionChanged(args.Position);
        }

        async void locator__StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            GeoPositionStatus s;
            switch (args.Status)
            {
                default:
                case PositionStatus.Disabled:
                    s = GeoPositionStatus.Disabled;
                    break;
                case PositionStatus.Initializing:
                    s = GeoPositionStatus.Initializing;
                    break;
                case PositionStatus.NoData:
                case PositionStatus.NotAvailable:
                case PositionStatus.NotInitialized:
                    s = GeoPositionStatus.NoData;
                    break;
                case PositionStatus.Ready:
                    s = GeoPositionStatus.Ready;
                    break;
            }
            OnPositionStatusChanged(new GeoPositionStatusChangedEventArgs(s));
            if (s == GeoPositionStatus.Ready && s != Status)
            {
                var pos = await locator_.GetGeopositionAsync();
                OnPositionChanged(pos);
            }
        }
        #endregion
    }

    public class GeoPosition<T>
    {
        public GeoPosition() { }
        public GeoPosition(DateTimeOffset timestamp, T position)
        {
            Location = position;
            Timestamp = timestamp;
        }

        public T Location { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    public class GeoCoordinate : IEquatable<GeoCoordinate>
    {
        public static readonly GeoCoordinate Unknown;

        public GeoCoordinate() { }
        public GeoCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
        public GeoCoordinate(double latitude, double longitude, double altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }
        public GeoCoordinate(double latitude, double longitude, double altitude, double horizontalAccuracy, double verticalAccuracy, double speed, double course)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            HorizontalAccuracy = horizontalAccuracy;
            VerticalAccuracy = verticalAccuracy;
            Speed = speed;
            Course = course;
        }

        public static bool operator !=(GeoCoordinate left, GeoCoordinate right) { return false; }
        public static bool operator ==(GeoCoordinate left, GeoCoordinate right) { return false; }

        public double Altitude { get; set; }
        public double Course { get; set; }
        public double HorizontalAccuracy { get; set; }
        public bool IsUnknown { get; private set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public double VerticalAccuracy { get; set; }

        public bool Equals(GeoCoordinate other) { return false; }
        public override bool Equals(object obj) { return false; }
        public double GetDistanceTo(GeoCoordinate other) { return 0; }
    }
}
