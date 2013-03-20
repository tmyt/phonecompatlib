using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace System.Windows.Navigation
{
    public static class NavigationService
    {
        private static void InvokeEvent<T>(EventHandler handler, T arg)
            where T : EventArgs
        {
            var h = handler;
            if (h == null) return;
            h(RootFrame, arg);
        }

        private static async void NavigateAsyncImpl(ResourceCandidate res)
        {
            try
            {
                string xaml = "";
                var file = await res.GetValueAsFileAsync();
                using (StreamReader sr = new StreamReader(await file.OpenStreamForReadAsync()))
                {
                    xaml = sr.ReadToEnd();
                }
                XDocument xd = XDocument.Parse(xaml);
                XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/winfx/2006/xaml");
                var className = xd.Root.Attribute(ns + "Class").Value;
                var targetType = Type.GetType(className);
                RootFrame.Navigated += RootFrame_Navigated;
                RootFrame.Navigate(targetType);
            }
            catch (Exception e)
            {
                InvokeEvent(NavigationFailed, new EventArgs());
                throw e;
            }
        }

        static void RootFrame_Navigated(object sender, global::Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            RootFrame.Navigated -= RootFrame_Navigated;
            if (!(RootFrame.Content is Page)) return;

            // Create Top AppBar
            var appBar = new AppBar();
            var grid = new Grid();
            var backBtn = new Button() { Style = (Style)Application.Current.Resources["BackButtonStyle"] };
            backBtn.Click += (_, __) => GoBack();

            grid.Children.Add(backBtn);
            appBar.Content = grid;

            (RootFrame.Content as Page).TopAppBar = appBar;
        }

        private static void NavigateImpl(Uri uri)
        {
            try
            {
                InvokeEvent(Navigating, new EventArgs());
                if (uri.IsAbsoluteUri || !uri.OriginalString.StartsWith("/"))
                {
                    throw new InvalidOperationException("Uri MUST be starts with /");
                }
                var map = ResourceManager.Current.MainResourceMap;
                var res = map["Files" + uri.OriginalString].ResolveAll().First();
                NavigateAsyncImpl(res);
            }
            catch (Exception e)
            {
                InvokeEvent(NavigationFailed, new EventArgs());
                throw e;
            }
        }

        private static Frame RootFrame { get { return (Frame)Window.Current.Content; } }

        public static IEnumerable<JournalEntry> BackStack { get { throw new NotImplementedException(); } }
        public static bool CanGoBack { get { return RootFrame.CanGoBack; } }
        public static bool CanGoForward { get { return RootFrame.CanGoForward; } }
        public static Uri CurrentSource { get { throw new NotImplementedException(); } internal set { Navigate(value); } }
        public static Uri Source { get { return CurrentSource; } set { CurrentSource = value; } }

        public static void GoBack() { RootFrame.GoBack(); }
        public static void GoForward() { RootFrame.GoForward(); }
        public static bool Navigate(Uri source) { NavigateImpl(source); return true; }
        public static JournalEntry RemoveBackEntry() { throw new NotImplementedException(); }
        public static void StopLoading() { }

        /* 互換性のための実相 */
        public static event EventHandler FragmentNavigation;
        public static event EventHandler JournalEntryRemoved;
        public static event EventHandler Navigated;
        public static event EventHandler Navigating;
        public static event EventHandler NavigationFailed;
        public static event EventHandler NavigationStopped;
    }

    public sealed class JournalEntry : DependencyObject
    {
        public Uri Source { get; internal set; }
    }
}