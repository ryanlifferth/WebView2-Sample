using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using WebView2_Sample.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using HtmlAgilityPack;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WebView2_Sample
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// Info from this site:
    ///     - https://learn.microsoft.com/en-us/microsoft-edge/webview2/get-started/winui
    ///     - https://stackoverflow.com/questions/74818270/c-sharp-webview2-is-there-a-way-to-enable-an-element-picker-without-the-dev-to
    ///     - https://learn.microsoft.com/en-us/microsoft-edge/webview2/samples/webview2_sample_uwp (https://github.com/MicrosoftEdge/WebView2Samples/tree/main/SampleApps/WebView2_WinUI3_Sample)
    ///     - 
    ///     
    ///     TODO: Work from this one
    ///     - https://weblog.west-wind.com/posts/2021/Jan/26/Chromium-WebView2-Control-and-NET-to-JavaScript-Interop-Part-2
    ///     - https://stackoverflow.com/questions/73048329/webview2-get-clicked-element-details
    ///     
    ///     THIS IS THE ONE...
    ///     DON'T NEED THE NUGET, THIS IS BUILT IN NOW
    ///     - https://www.nuget.org/packages/Microsoft.Web.WebView2.DevToolsProtocolExtension
    ///     - https://learn.microsoft.com/en-us/microsoft-edge/webview2/how-to/chromium-devtools-protocol
    ///     - https://chromedevtools.github.io/devtools-protocol
    ///     
    ///     THIS ONE IS INTERESTING TOO
    ///     - https://github.com/ChromiumDotNet/WebView2.DevTools.Dom
    ///     
    ///     THIS IS THE FINAL ANSWER
    ///     - This, but without the nuget -> https://learn.microsoft.com/en-us/microsoft-edge/webview2/how-to/chromium-devtools-protocol#use-devtoolsprotocolhelper
    ///     - https://chromedevtools.github.io/devtools-protocol/tot/Overlay/#method-setInspectMode
    ///     - https://github.com/MicrosoftEdge/WebView2Feedback/discussions/3846
    ///     
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            //AddressBar.Text = "https://www.microsoft.com";
            AddressBar.Text = "https://byucougars.com/sports/womens-soccer/roster/season/2023/staff/jennifer-rockwood";
            //MyWebView.NavigationStarting += EnsureHttps;

            MyWebView.CoreWebView2Initialized += MyWebView_CoreWebView2Initialized;
            MyWebView.NavigationCompleted += MyWebView_NavigationCompleted;
            MyWebView.WebMessageReceived += WebView_WebMessageReceived;


            MyWebView.Source = new Uri(AddressBar.Text);
            StatusUpdate("Ready");
            SetTitle();
        }

        private async void MyWebView_InspectNodeMessage(CoreWebView2 sender, CoreWebView2DevToolsProtocolEventReceivedEventArgs e)
        {
            var ryan = e.ParameterObjectAsJson;
            //var jsonObject = JsonSerializer.Deserialize<JsonObject>(e.WebMessageAsJson);
            var json = JsonConvert.DeserializeObject<NodeMessageResponse>(e.ParameterObjectAsJson);

            var aimee = await MyWebView.CoreWebView2.CallDevToolsProtocolMethodAsync("DOM.getOuterHTML", "{\"backendNodeId\": " + json.BackendNodeId + "}");

            if (!string.IsNullOrEmpty(aimee))
            {
                var html = JsonConvert.DeserializeObject<DomOuterHTML>(aimee);
                var element = html.OuterHTML;

                /*
                 * var doc = new HtmlDocument();
                   doc.LoadHtml(html);
                 */
                var doc = new HtmlDocument();
                doc.LoadHtml(element);

                var innerText = doc.DocumentNode.InnerText;
                StatusRightUpdate(innerText);
            }

            var jacob = "";
        }


        private void MyWebView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            if (args.Exception != null)
            {
                StatusUpdate($"Error initializing WebView2: {args.Exception.Message}");
            }
            else
            {
                SetTitle(sender);
            }
        }

        private async void MyWebView_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            StatusUpdate("Navigation complete");

            // Update the address bar with the full URL that was navigated to.
            AddressBar.Text = sender.Source.ToString();

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var di = new DirectoryInfo(path);
            var currentDir = di.Parent.Parent.Parent.Parent.Parent.Parent.FullName;

            string script = File.ReadAllText($"{currentDir}\\Mouse.js");
            await MyWebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script);
            
            await MyWebView.CoreWebView2.CallDevToolsProtocolMethodAsync("Emulation.setGeolocationOverride", "{}");

            //MyWebView.CoreWebView2.OpenDevToolsWindow();
            //var ryan = MyWebView.CoreWebView2.GetDevToolsProtocolEventReceiver("mousedown");
            //ryan.DevToolsProtocolEventReceived += new Windows.Foundation.TypedEventHandler<CoreWebView2, CoreWebView2DevToolsProtocolEventReceivedEventArgs>(this.MyWebView_DevToolsProtocolEventReceived);

            //await MyWebView.ExecuteScriptAsync("document.getElementById('DropDownList').selectedIndex");
        }

        private void MyWebView_DevToolsProtocolEventReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2DevToolsProtocolEventReceivedEventArgs e)
        {
            Debug.WriteLine("event");
        }

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var jsonObject = JsonConvert.DeserializeObject<JsonObject>(e.WebMessageAsJson);
            //var jsonObject = JsonSerializer.Deserialize<MouseDown>(e.WebMessageAsJson);
            var ryan = jsonObject.ElemId;
        }

        private void StatusUpdate(string message)
        {
            StatusBar.Text = message;
            Debug.WriteLine(message);
        }

        private void StatusRightUpdate(string message)
        {
            StatusBarRight.Text = message;
            Debug.WriteLine(message);
        }

        private void SetTitle(WebView2 webView2 = null)
        {
            var packageDisplayName = Windows.ApplicationModel.Package.Current.DisplayName;
            var webView2Version = (webView2 != null) ? " - " + GetWebView2Version(webView2) : string.Empty;
            Title = $"{packageDisplayName}{webView2Version}";
        }

        private string GetWebView2Version(WebView2 webView2)
        {
            var runtimeVersion = webView2.CoreWebView2.Environment.BrowserVersionString;

            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions();
            var targetVersionMajorAndRest = options.TargetCompatibleBrowserVersion;
            var versionList = targetVersionMajorAndRest.Split('.');
            if (versionList.Length != 4)
            {
                return "Invalid SDK build version";
            }
            var sdkVersion = versionList[2] + "." + versionList[3];

            return $"{runtimeVersion}; {sdkVersion}";
        }

        private bool TryCreateUri(String potentialUri, out Uri result)
        {
            StatusUpdate("TryCreateUri");

            Uri uri;
            if ((Uri.TryCreate(potentialUri, UriKind.Absolute, out uri) || Uri.TryCreate("http://" + potentialUri, UriKind.Absolute, out uri)) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                result = uri;
                return true;
            }
            else
            {
                StatusUpdate("Unable to configure URI");
                result = null;
                return false;
            }
        }

        private void TryNavigate()
        {
            StatusUpdate("TryNavigate");

            Uri destinationUri;
            if (TryCreateUri(AddressBar.Text, out destinationUri))
            {
                MyWebView.Source = destinationUri;
            }
            else
            {
                StatusUpdate("URI couldn't be figured out use it as a bing search term");

                String bingString = "https://www.bing.com/search?q=" + Uri.EscapeUriString(AddressBar.Text);
                if (TryCreateUri(bingString, out destinationUri))
                {
                    AddressBar.Text = destinationUri.AbsoluteUri;
                    MyWebView.Source = destinationUri;
                }
                else
                {
                    StatusUpdate("URI couldn't be configured as bing search term, giving up");
                }
            }
        }

        private void Go_OnClick(object sender, RoutedEventArgs e)
        {
            StatusUpdate("Go_OnClick: " + AddressBar.Text);

            TryNavigate();
        }

        private async void Inspect_OnClick(object sender, RoutedEventArgs e)
        {
            StatusUpdate("Inspect_OnClick");

            if (Inspect.Content == "Inspect")
            {

                //var json = "{\"mode\":\"searchForNode\",\"highlightConfig\":{\"showInfo\":true,\"contentColor\":{\"r\": 155, \"g\": 11, \"b\": 239, \"a\": 0.7}}}";
                var folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var json = File.ReadAllText($@"{folder}\Assets\Data\highlightConfig.json");

                await MyWebView.CoreWebView2.CallDevToolsProtocolMethodAsync("DOM.enable", "{}");
                await MyWebView.CoreWebView2.CallDevToolsProtocolMethodAsync("Overlay.enable", "{}");
                await MyWebView.CoreWebView2.CallDevToolsProtocolMethodAsync("Overlay.setInspectMode", json);
                Inspect.Content = "Inspect [ON]";

                var myReceiver = MyWebView.CoreWebView2.GetDevToolsProtocolEventReceiver("Overlay.inspectNodeRequested");
                myReceiver.DevToolsProtocolEventReceived += MyWebView_InspectNodeMessage;
            }
            else
            {
                await MyWebView.CoreWebView2.CallDevToolsProtocolMethodAsync("Overlay.disable", "{}");
                Inspect.Content = "Inspect";
            }

        }

        private void AddressBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                StatusUpdate("AddressBar_KeyDown [Enter]: " + AddressBar.Text);

                e.Handled = true;
                TryNavigate();
            }
        }

        //private void EnsureHttps(object sender, CoreWebView2NavigationStartingEventArgs args)
        //{
        //    string uri = args.Uri;
        //    if (string.IsNullOrEmpty(uri))
        //    {
        //        return;
        //    }

        //    if (!uri.StartsWith("https://"))
        //    {
        //        args.Cancel = true;
        //        MyWebView.CoreWebView2.ExecuteScriptAsync($"alert('{uri} is not safe, try an https link')");
        //    }
        //    else
        //    {
        //        AddressBar.Text = uri;
        //    }
        //}
                
        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Uri targetUri = new Uri(AddressBar.Text);
                MyWebView.Source = targetUri;
            }
            catch (FormatException ex)
            {
                // Incorrect address entered.
            }
        }
    }
}
