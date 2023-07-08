using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
        CancellationTokenSource cts = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew(); // more precise than DateTime.Now()
            //RunDownloadSync();
            var results = DemoMehods.RunDownloadParallelSync();
            PrintResults(results);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            resultsWindow.Text += $"Total execution time: { elapsedMs }";
        }

        private async void executeASync_Click(object sender, RoutedEventArgs e)  // Event - void
        {
            // Before watch, create an instant event.
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;// Create an event (wire up to the ReportProgress event)

            var watch = System.Diagnostics.Stopwatch.StartNew(); // more precise than DateTime.Now()

            //await RunDownloadAsync();
            try
            {
                var results = await DemoMehods.RunDownloadAsync(progress, cts.Token); // pass in cts.Token
                PrintResults(results);
            }
            catch (OperationCanceledException)
            {
                resultsWindow.Text += $"The async download was cancelled. { Environment.NewLine}";
            }

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
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;

            var watch = System.Diagnostics.Stopwatch.StartNew(); // more precise than DateTime.Now()
            //await RunDownloadAsync();
            var results = await DemoMehods.RunDownloadParallelASyncV2(progress); // able to control our UI
            PrintResults(results);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            resultsWindow.Text += $"Total execution time: {elapsedMs}";
        }

        private void cancelOperation_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }

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
