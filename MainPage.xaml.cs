using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution.Foreground;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MicrosoftTimerCheck
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public int count = 0;
        public StorageFile sampleFile { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            sampleFile = await storageFolder.GetFileAsync("sample.txt");
            timerCountValue.Text = sampleFile.Path;

            await ExtendedExecutionCall();
        }

        public async Task ExtendedExecutionCall()
        {

            // Create an extended execution session
            var newSession = new ExtendedExecutionSession();
            newSession.Reason = ExtendedExecutionReason.Unspecified;
            newSession.Description = "Long Running Processing";
            newSession.Revoked += (sender, args) =>
            {
            };
            ExtendedExecutionResult result = await newSession.RequestExtensionAsync();

            if (result == ExtendedExecutionResult.Denied)
            {
                // Extended execution was denied by the user
                // Handle this case accordingly
            }
            else
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(20 * 1000);
                timer.Tick += Timer_Tick;
                timer.Start();
                // Extended execution was granted; you can perform your timer-related background tasks here
            }
        }


        private async void Timer_Tick(object sender, object e)
        {
            string logstr = " " + count.ToString() + " log: " + DateTime.Now.ToString() + "\r\n";

            await Windows.Storage.FileIO.AppendTextAsync(sampleFile, logstr);
            count++;
        }

        private void NewSession_Revoked(object sender, ExtendedExecutionForegroundRevokedEventArgs args)
        {

        }
    }

}
