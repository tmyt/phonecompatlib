using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace Microsoft.Phone.Shell
{
    public class ApplicationBarStateChangedEventArgs : EventArgs
    {
        public bool IsMenuVisible { get; set; }
    }

    public enum ApplicationBarMode
    {
        Default,
        Minimized
    }

    public class ApplicationBarIconButton : INotifyPropertyChanged
    {
        public string IconUri { get; set; }
        public bool IsEnabled { get; set; }
        public string Text { get; set; }

        public event EventHandler Click;

        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnClick()
        {
            var h = Click;
            if (h != null) h(this, EventArgs.Empty);
        }
    }

    public class ApplicationBarMenuItem : INotifyPropertyChanged
    {
        public bool IsEnabled { get; set; }
        public string Text { get; set; }

        public event EventHandler Click;

        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnClick()
        {
            var h = Click;
            if (h != null) h(this, EventArgs.Empty);
        }
    }

    [ContentProperty(Name = "Buttons")]
    public class ApplicationBar : INotifyPropertyChanged
    {
        private AppBar appBar_;
        private StackPanel rightItems_;
        private StackPanel leftItems_;
        private Button menuButton_;

        private bool initialized_;

        public ApplicationBar()
        {
            Buttons = new ObservableCollection<ApplicationBarIconButton>();
            MenuItems = new ObservableCollection<ApplicationBarMenuItem>();

            BackgroundColor = Color.FromArgb(255, 0, 0, 0);
            Opacity = 1.0;
        }

        public ObservableCollection<ApplicationBarIconButton> Buttons { get; set; }
        public ObservableCollection<ApplicationBarMenuItem> MenuItems { get; set; }

        public Color BackgroundColor { get; set; }
        public double DefaultSize { get; set; }
        public Color ForegroundColor { get; set; }
        public bool IsMenuEnabled { get; set; }
        public bool IsVisible { get; set; }
        public double MinSize { get; set; }
        public ApplicationBarMode Mode { get; set; }
        public double Opacity { get; set; }

        public event EventHandler<ApplicationBarStateChangedEventArgs> StateChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Initialize(Page rootPage)
        {
            // Instansiate AppBar
            var appBarGrid = new Grid();
            appBarGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            appBarGrid.ColumnDefinitions.Add(new ColumnDefinition());
            appBarGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });

            // Instansiate StackPanel
            leftItems_ = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left };
            rightItems_ = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };

            // Insert StackPanels to Grid
            appBarGrid.Children.Add(leftItems_);
            appBarGrid.Children.Add(rightItems_);
            Grid.SetColumn(rightItems_, 1);

            // Create MenuItems Popup Button
            menuButton_ = MakeAppBarButton(null, "");
            menuButton_.Content = "…";
            menuButton_.Click += ShowContextMenuClick;
            appBarGrid.Children.Add(menuButton_);
            Grid.SetColumn(menuButton_, 2);

            // Set Grid to AppBar
            appBar_ = new AppBar();
            appBar_.Content = appBarGrid;

            appBar_.Background = new SolidColorBrush(BackgroundColor);
            appBar_.Opacity = Opacity;

            // Set AppBar
            rootPage.BottomAppBar = appBar_;

            // Attach Events
            PropertyChanged += ApplicationBarPropertyChanged;
            Buttons.CollectionChanged += ApplicationBarButtonsCollectionChanged;
            MenuItems.CollectionChanged += ApplicationBarMenuItemsCollectionChanged;
            Buttons.All(_ => { _.PropertyChanged += ButtonPropertyChanged; return true; });
            MenuItems.All(_ => { _.PropertyChanged += MenuItemPropertyChanged; return true; });

            // Create MenuButtons
            foreach (var button in Buttons)
            {
                var b = MakeAppBarButton(button.IconUri, button.Text);
                b.Click += (s, e) =>
                {
                    button.OnClick();
                };
                rightItems_.Children.Add(b);
            }

            // Hide Three dots button
            UpdateContextMenuButtonVisibility();

            // Set flag
            initialized_ = true;
        }

        public void Uninitialize()
        {
            initialized_ = false;

            // Remove handlers
            PropertyChanged -= ApplicationBarPropertyChanged;
            Buttons.CollectionChanged -= ApplicationBarButtonsCollectionChanged;
            MenuItems.CollectionChanged -= ApplicationBarMenuItemsCollectionChanged;
            Buttons.All(_ => { _.PropertyChanged -= ButtonPropertyChanged; return true; });
            MenuItems.All(_ => { _.PropertyChanged -= MenuItemPropertyChanged; return true; });
        }

        private void UpdateContextMenuButtonVisibility()
        {
            menuButton_.Visibility = MenuItems.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        void ApplicationBarPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        void ApplicationBarButtonsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }

        void ApplicationBarMenuItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }
            UpdateContextMenuButtonVisibility();
        }

        void ButtonPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        void MenuItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        async void ShowContextMenuClick(object sender, RoutedEventArgs e)
        {
            var popup = new PopupMenu();
            foreach (var menu in MenuItems)
            {
                var cmd = new UICommand(menu.Text, new UICommandInvokedHandler(_ =>
                {
                    menu.OnClick();
                }));
                popup.Commands.Add(cmd);
            }
            var trans = menuButton_.TransformToVisual(Window.Current.Content);
            await popup.ShowForSelectionAsync(
                new Rect(trans.TransformPoint(new Point(0, 0)), 
                    trans.TransformPoint(new Point(menuButton_.ActualWidth, menuButton_.ActualHeight))),
                Placement.Above);
        }

        Button MakeAppBarButton(string IconUri, string Text)
        {
            var button = new Button() { Style = (Style)Application.Current.Resources["AppBarButtonStyle"] };
            AutomationProperties.SetName(button, Text);
            if (!string.IsNullOrEmpty(IconUri))
            {
                var uri = new Uri(IconUri, UriKind.RelativeOrAbsolute);
                if (IconUri.StartsWith("/")) uri = new Uri("ms-appx://" + IconUri, UriKind.RelativeOrAbsolute);
                var icon = new Image();
                var bitmap = new BitmapImage(uri);
                icon.Source = bitmap;
                button.Content = icon;
            }
            return button;
        }
    }

    public class AppbarHost
    {
        public static ApplicationBar GetApplicationBar(DependencyObject obj)
        {
            return (ApplicationBar)obj.GetValue(ApplicationBarProperty);
        }

        public static void SetApplicationBar(DependencyObject obj, ApplicationBar value)
        {
            if (!(obj is Page)) { throw new InvalidOperationException("AppbarHost is must owned by Page"); } // TODO: メッセージ怪しい
            var rootPage = obj as Page;

            // Check old ApplicationBar
            if (GetApplicationBar(obj) != null)
            {
                // Detatch Events
                var oldAppBar = GetApplicationBar(obj);
                oldAppBar.Uninitialize();
            }

            // Initialize ApplicationBar
            value.Initialize(rootPage);

            // Set attached property
            obj.SetValue(ApplicationBarProperty, value);
        }

        // Using a DependencyProperty as the backing store for ApplicationBar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ApplicationBarProperty =
            DependencyProperty.RegisterAttached("ApplicationBar", typeof(ApplicationBar), typeof(AppbarHost), new PropertyMetadata(null));
    }
}

