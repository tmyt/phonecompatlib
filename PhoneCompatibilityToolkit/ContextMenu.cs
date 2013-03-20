using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Microsoft.Phone.Controls
{
    public class ContextMenuService
    {
        public static ContextMenu GetContextMenu(DependencyObject obj)
        {
            return (ContextMenu)obj.GetValue(ContextMenuProperty);
        }

        public static void SetContextMenu(DependencyObject obj, ContextMenu value)
        {
            var oldValue = GetContextMenu(obj);
            obj.SetValue(ContextMenuProperty, value);
            if (oldValue == null)
            {
                (obj as UIElement).RightTapped += PopupMenu_RightTapped;
            }
            if (value == null)
            {
                (obj as UIElement).RightTapped -= PopupMenu_RightTapped;
            }
        }

        static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        static async void PopupMenu_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            var menu = (ContextMenu)(sender as DependencyObject).GetValue(ContextMenuProperty);
            if (menu == null) return;
            e.Handled = true;
            await menu.get().ShowForSelectionAsync(GetElementRect((FrameworkElement)sender));
        }

        // Using a DependencyProperty as the backing store for ContextMenu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextMenuProperty =
            DependencyProperty.RegisterAttached("ContextMenu", typeof(ContextMenu), typeof(ContextMenuService), new PropertyMetadata(null));


    }

    [ContentProperty(Name = "Commands")]
    public class ContextMenu : FrameworkElement
    {
        public ContextMenu()
        {
            Commands = new List<MenuItemBase>();
        }

        public List<MenuItemBase> Commands
        {
            get { return (List<MenuItemBase>)GetValue(CommandsProperty); }
            set { SetValue(CommandsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Commands.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandsProperty =
            DependencyProperty.Register("Commands", typeof(List<MenuItemBase>), typeof(ContextMenuService), new PropertyMetadata(null));

        public Windows.UI.Popups.PopupMenu get()
        {
            var menu = new Windows.UI.Popups.PopupMenu();
            foreach (var cmd in Commands)
            {
                if (cmd is Separator)
                {
                    menu.Commands.Add(new UICommandSeparator());
                }
                else
                {
                    var item = cmd as MenuItem;
                    menu.Commands.Add(new UICommand(item.Header, param =>
                    {
                        if (item.Command != null)
                        {
                            item.Command.Execute(param);
                        }
                        else
                        {
                            item.OnClick();
                        }
                    }, item.CommandParameter));
                }
            }
            return menu;
        }
    }

    public abstract class MenuItemBase : FrameworkElement
    {
    }

    public class MenuItem : MenuItemBase
    {
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(MenuItem), new PropertyMetadata(null));




        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(MenuItem), new PropertyMetadata(null));



        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(MenuItem), new PropertyMetadata(null));


        public event RoutedEventHandler Click;

        internal void OnClick()
        {
            var h = Click;
            if (h != null)
            {
                h(this, new RoutedEventArgs());
            }
        }

    }

    public class Separator : MenuItemBase
    {
    }
}