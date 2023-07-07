using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebAsyncDemoUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew(); // more precise than DateTime.Now()
            //RunDownloadSync();
            var results = DemoMehods.RunDownloadSync();
            PrintResults(results);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            resultsWindow.Text += $"Total execution time: { elapsedMs }";
        }

        private async void executeASync_Click(object sender, RoutedEventArgs e)  // Event - void
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;// Create an event
            var watch = System.Diagnostics.Stopwatch.StartNew(); // more precise than DateTime.Now()

            //await RunDownloadAsync();
            var results = await DemoMehods.RunDownloadAsync(progress);
            PrintResults(results);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            resultsWindow.Text += $"Total execution time: {elapsedMs}";
        }

        private void ReportProgress(object? sender, ProgressReportModel e)
        {
            dashboardProgress.Value = e.PercentageComplete;
            PrintResults(e.SitesDownloaded);
        }

        private async void executeParalleASync_Click(object sender, RoutedEventArgs e)  // Event - void
        {
            var watch = System.Diagnostics.Stopwatch.StartNew(); // more precise than DateTime.Now()
            //await RunDownloadAsync();
            var results = await DemoMehods.RunDownloadParalleAsync();
            PrintResults(results);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            resultsWindow.Text += $"Total execution time: {elapsedMs}";
        }

        // cancelOperation()

        private void PrintResults(List<WebsiteDataModel> results)
        {
            resultsWindow.Text = "";
            foreach (var item in results)
            {
                resultsWindow.Text += $"{item.WebsiteUrl} downloaded: {item.WebsiteData.Length} characters long.{Environment.NewLine}";
            }
        }
       
    }
}
