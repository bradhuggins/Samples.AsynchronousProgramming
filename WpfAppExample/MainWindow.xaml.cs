using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private List<string> UrlList
        {
            get
            {
                List<string> output = new List<string>();
                output.Add("https://www.isnetworld.com");
                output.Add("https://www.microsoft.com");
                output.Add("https://www.google.com");
                output.Add("https://www.cnn.com");
                output.Add("https://www.stackoverflow.com");
                output.Add("https://www.msn.com");
                output.Add("https://www.yahoo.com");
                return output;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            resultsWindow.Text = string.Empty;

            RunDownloadSync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execution time: { elapsedMs }{ Environment.NewLine }";
        }

        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            resultsWindow.Text = string.Empty;

            await RunDownloadParallelAsync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execution time: { elapsedMs }{ Environment.NewLine }";
        }

        private async Task RunDownloadAsync()
        {
            foreach (string site in UrlList)
            {
                WebsiteDataModel results = await Task.Run(() => DownloadWebsite(site));
                ReportWebsiteInfo(results);
            }
        }

        private async Task RunDownloadParallelAsync()
        {
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            foreach (string site in UrlList)
            {
                tasks.Add(DownloadWebsiteAsync(site));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var item in results)
            {
                ReportWebsiteInfo(item);
            }
        }

        private void RunDownloadSync()
        {
            foreach (string site in UrlList)
            {
                WebsiteDataModel results = DownloadWebsite(site);
                ReportWebsiteInfo(results);
            }
        }

        private WebsiteDataModel DownloadWebsite(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();
            output.WebsiteUrl = websiteURL;
            output.WebsiteData = client.DownloadString(websiteURL);
            return output;
        }

        private async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            //WebClient client = new WebClient();
            output.WebsiteUrl = websiteURL;
            //output.WebsiteData = await client.DownloadStringTaskAsync(websiteURL);
            output.WebsiteData = await _httpClient.GetStringAsync(websiteURL);
            return output;
        }

        private void ReportWebsiteInfo(WebsiteDataModel data)
        {
            resultsWindow.Text += $"{ data.WebsiteUrl } downloaded: { data.WebsiteData.Length } characters long.{ Environment.NewLine }";
        }

    }
}