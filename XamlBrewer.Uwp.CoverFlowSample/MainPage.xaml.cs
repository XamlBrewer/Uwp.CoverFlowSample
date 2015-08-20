using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace XamlBrewer.Uwp.CoverFlowSample
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CoverFlow.ItemsSource = await GetUrls();
            Slider.Maximum = CoverFlow.Items.Count() - 1;
        }

        private async static Task<IEnumerable<string>> GetUrls()
        {
            string xml;

            using (var client = new HttpClient())
            {
                xml = await client.GetStringAsync("http://www.apple.com/trailers/home/xml/current.xml");
            }

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            return doc.DocumentElement.GetElementsByTagName("xlarge").Cast<XmlNode>().Select(node => node.InnerText).ToList();
        }
    }
}
