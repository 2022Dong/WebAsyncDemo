using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WebAsyncDemoUI
{
    public static class DemoMehods
    {       
        public static List<string> PrepData() // A list of string
        {
            List<string> output = new List<string>();

            //resultsWindow.Text = "";

            output.Add("https://www.yahoo.com");
            output.Add("https://www.google.com");
            output.Add("https://en.wikipedia.org/wiki/.NET_Framework");
            output.Add("https://en.wikipedia.org/wiki/.NET");
            //output.Add("https://www.microsoft.com");
            //output.Add("https://www.cnn.com");
            //output.Add("https://www.codeproject.com");
            //output.Add("https://www.stackoverflow.com");

            return output;
        }

        public static List<WebsiteDataModel> RunDownloadSync()
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();

            foreach (string site in websites)
            {
                WebsiteDataModel results = DownloadWebsite(site);
                output.Add(results);
            }
            return output;
        }

        public static List<WebsiteDataModel> RunDownloadParallelSync()
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();

            // Paralled Libray. Parallel.ForEach - locks everything up until it's done. when run the UI be locked up, still is sync.
            // DB might use it, but is not good for tasks
            Parallel.ForEach<string>(websites, (site) =>  
            {
                WebsiteDataModel results = DownloadWebsite(site);
                output.Add(results);
            });
            return output;
        }

        public static async Task<List<WebsiteDataModel>> RunDownloadParallelASyncV2(IProgress<ProgressReportModel> progress)
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();
            ProgressReportModel report = new ProgressReportModel(); // variable

            await Task.Run(() =>
            {
                Parallel.ForEach<string>(websites, (site) =>
                {
                    WebsiteDataModel results = DownloadWebsite(site);
                    output.Add(results);

                    report.SitesDownloaded = output;
                    report.PercentageComplete = (output.Count * 100) / websites.Count;
                    progress.Report(report);
                });
            });

            return output;
        }

        // async method: xxxAsync()
        public static async Task<List<WebsiteDataModel>> RunDownloadAsync(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken) // No void use Task instead. But only one exception - used in Event
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();
            ProgressReportModel report = new ProgressReportModel();

            foreach (string site in websites)
            {
                WebsiteDataModel results = await DownloadWebsiteAsync(site);
                output.Add(results);

                cancellationToken.ThrowIfCancellationRequested(); // throw an exception here, catch the exception in the MainWindow btn click event ->

                report.SitesDownloaded = output;
                report.PercentageComplete = (output.Count * 100) / websites.Count;
                progress.Report(report);
            }
            return output;
        }

        public static async Task<List<WebsiteDataModel>> RunDownloadParalleAsync() // download everything, then executed at a time. => More FASTER!
        {
            List<string> websites = PrepData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            foreach (string site in websites)
            {
                //tasks.Add(Task.Run(() => DownloadWebsite(site)));
                tasks.Add(DownloadWebsiteAsync(site));  // calling async method NO need Task.Run()
            }

            var results = await Task.WhenAll(tasks);

            return new List<WebsiteDataModel>(results);
        }

        public static WebsiteDataModel DownloadWebsite(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteURL;
            output.WebsiteData = client.DownloadString(websiteURL);

            return output;
        }

        public static async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteURL) // Change to async method
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteURL;
            output.WebsiteData = await client.DownloadStringTaskAsync(websiteURL);

            return output;
        }
    }
}