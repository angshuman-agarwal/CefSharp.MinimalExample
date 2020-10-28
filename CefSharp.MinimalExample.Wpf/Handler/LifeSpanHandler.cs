using CefSharp.Wpf;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace CefSharp.MinimalExample.Wpf.Handler
{
    public class LifeSpanHandler : ILifeSpanHandler
    {
        private DevToolsStateHack dsh;
        private RoutedCommand ShowDevToolsCommand = new RoutedCommand();
        public LifeSpanHandler(DevToolsStateHack dsh)
        {
            this.dsh = dsh;
        }

        bool ILifeSpanHandler.OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

            ChromiumWebBrowser popupChromiumWebBrowser = null;

            var windowX = (windowInfo.X == int.MinValue) ? double.NaN : windowInfo.X;
            var windowY = (windowInfo.Y == int.MinValue) ? double.NaN : windowInfo.Y;
            var windowWidth = (windowInfo.Width == int.MinValue) ? double.NaN : windowInfo.Width;
            var windowHeight = (windowInfo.Height == int.MinValue) ? double.NaN : windowInfo.Height;

            chromiumWebBrowser.Dispatcher.Invoke(() =>
            {
                var owner = Window.GetWindow(chromiumWebBrowser);
                popupChromiumWebBrowser = new ChromiumWebBrowser();

                popupChromiumWebBrowser.SetAsPopup();
                popupChromiumWebBrowser.LifeSpanHandler = this;

                var popup = new Window
                {
                    Left = windowX,
                    Top = windowY,
                    Width = windowWidth,
                    Height = windowHeight,
                    Content = popupChromiumWebBrowser,
                    Owner = owner,
                    Title = targetFrameName
                };

                ShowDevToolsCommand.InputGestures.Add(new KeyGesture(Key.F12, ModifierKeys.None));
                popup.CommandBindings.Add(new CommandBinding(ShowDevToolsCommand, HandleDevTools));


                var windowInteropHelper = new WindowInteropHelper(popup);
                var handle = windowInteropHelper.EnsureHandle();
                windowInfo.SetAsWindowless(handle);

                popup.Closed += (o, e) =>
                {
                    var w = o as Window;
                    if (w != null && w.Content is IWebBrowser)
                    {
                        (w.Content as IWebBrowser).Dispose();
                        w.Content = null;
                    }
                };
            });

            newBrowser = popupChromiumWebBrowser;

            return false;
        }

        private void HandleDevTools(object sender, ExecutedRoutedEventArgs e)
        {
            // dirty hack to control "DevTools was launched state"
            // Cannot get DevTools instance, manual hack - https://www.magpcss.org/ceforum/viewtopic.php?f=6&t=12119
            dsh.ShowDevTools = true;
            ((sender as Window).Content as ChromiumWebBrowser).ShowDevTools();
        }

        void ILifeSpanHandler.OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {
            if (!browser.IsDisposed && browser.IsPopup)
            {
                // Cannot get DevTools instance, manual hack - https://www.magpcss.org/ceforum/viewtopic.php?f=6&t=12119
                if (!dsh.ShowDevTools)
                {
                    var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

                    chromiumWebBrowser.Dispatcher.Invoke(() =>
                    {
                        var owner = Window.GetWindow(chromiumWebBrowser);

                        if (owner != null && owner.Content == browserControl)
                        {
                            owner.Show();
                        }
                    });
                }

                // reset the state
                dsh.ShowDevTools = false;
            }
        }

        bool ILifeSpanHandler.DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            return false;
        }

        void ILifeSpanHandler.OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {
            if (!browser.IsDisposed && browser.IsPopup)
            {
                if (!browser.MainFrame.Url.Equals("devtools://devtools/devtools_app.html"))
                {
                    var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

                    chromiumWebBrowser.Dispatcher.Invoke(() =>
                    {
                        var owner = Window.GetWindow(chromiumWebBrowser);

                        if (owner != null && owner.Content == browserControl)
                        {
                            owner.Close();
                        }
                    });
                }
            }
        }
    }
}

