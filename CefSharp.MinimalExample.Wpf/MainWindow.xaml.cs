using CefSharp.MinimalExample.Wpf.Handler;
using System;
using System.Windows;
using System.Windows.Input;

namespace CefSharp.MinimalExample.Wpf
{
    public partial class MainWindow : Window
    {
        public static readonly RoutedCommand ShowDevToolsCommand = new RoutedCommand();
        private DevToolsStateHack _dsh;

        public MainWindow()
        {
            InitializeComponent();
            _dsh = new DevToolsStateHack();
            ShowDevToolsCommand.InputGestures.Add(new KeyGesture(Key.F12, ModifierKeys.None));
            CommandBindings.Add(new CommandBinding(ShowDevToolsCommand, HandleDevTools));
            this.Browser.LifeSpanHandler = new LifeSpanHandler(_dsh);
        }

        private void HandleDevTools(object sender, ExecutedRoutedEventArgs e)
        {
            // lifespanhandler needs it to detect if it was popup or dev tools
            _dsh.ShowDevTools = true;
            Browser.ShowDevTools();
        }
    }

    // Cannot get DevTools instance in OnAfterCreated, manual hack as per the hint - https://www.magpcss.org/ceforum/viewtopic.php?f=6&t=12119
    public class DevToolsStateHack
    {
        /// <summary>
        /// Was DevTools shown
        /// </summary>
        public bool ShowDevTools { get; set; }
    }
}
