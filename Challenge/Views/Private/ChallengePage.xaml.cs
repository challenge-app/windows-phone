using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using ChallengeApp.Models;
using ChallengeApp.Controllers;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;


namespace ChallengeApp.Views.Private
{
    public partial class ChallengePage : PhoneApplicationPage
    {
        PhotoChooserTask photoChooserTask;

        TransferUtility _transferUtility;
        System.IO.Stream fileStream;
        string BaseURL = String.Format("https://{0}.s3{1}.amazonaws.com/", App.BUCKET, String.IsNullOrEmpty(App.AWSRegion.SystemName) ? "" : String.Format("-{0}", App.AWSRegion.SystemName));
        string FilePath;

        public Challenge ChallengeObject
        {
            get { return (Challenge)GetValue(challengeObject); }
            set { SetValue(challengeObject, value); }
        }

        // Using a DependencyProperty as the backing store.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty challengeObject = DependencyProperty.Register("ChallengeObject", typeof(Challenge), typeof(PhoneApplicationPage), new PropertyMetadata(null));

        public ChallengePage()
        {
            InitializeComponent();
            loadConfiguration();

            ChallengeObject = App.ChallengeObject;
            Debug.WriteLine(ChallengeObject.url);
            FilePath = String.Format("picture/{0}.jpg", ChallengeObject.id);

            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //string id = NavigationContext.QueryString["id"];
        }
        private void loadConfiguration()
        {
            if (string.IsNullOrEmpty(App.AWSAccessKey))
            {
                MessageBox.Show("AWSAccessKey is not set");
                return;
            }

            if (string.IsNullOrEmpty(App.AWSSecretKey))
            {
                MessageBox.Show("AWSSecretKey is not set");
                return;
            }

            this._transferUtility = new TransferUtility(App.AWSAccessKey, App.AWSSecretKey, App.AWSRegion);
        }

        private void uploadFile()
        {
            try
            {
                SystemTray.ProgressIndicator = new ProgressIndicator();
                SystemTray.ProgressIndicator.IsIndeterminate = true;
                SystemTray.ProgressIndicator.IsVisible = true;

                TransferUtilityUploadRequest request = new TransferUtilityUploadRequest()
                {
                    BucketName = App.BUCKET,
                    Key = FilePath,
                    CannedACL = S3CannedACL.PublicRead,
                    InputStream = this.fileStream,
                    
                };

                request.UploadProgressEvent += this.uploadFileProgressCallback;

                this._transferUtility.UploadAsync(request);
                if (SystemTray.ProgressIndicator != null) SystemTray.ProgressIndicator.IsVisible = false;

                Debug.WriteLine("URL: " + BaseURL + FilePath);
                ChallengeController.Instance.AcceptChallenge(ChallengeObject.id, BaseURL + FilePath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                //updateIsEnabled(this._ctlUploadFile, true);
            }
        }

        // This gets called as bytes are written to the request stream.  The sender is the TransferUtilityUploadRequest
        // that was used to start the upload. The UploadProgressArgs contains the total bytes to be transferred and how many bytes 
        // have already been transferred.
        private void uploadFileProgressCallback(object sender, UploadProgressArgs e)
        {
            Debug.WriteLine(String.Format("Uploaded {0} / {1}", e.TransferredBytes.ToString(), e.TotalBytes.ToString()));

            if (e.PercentDone == 100) Debug.WriteLine("Completed file upload!");
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                this.fileStream = new MemoryStream();

                WriteableBitmap wb = new WriteableBitmap(640, 640);
                wb.SetSource(e.ChosenPhoto);

                int w = wb.PixelWidth;
                int h = wb.PixelHeight;
                float p;

                if (w > 640 || h > 640)
                {
                    if (w < h)  p = (float)w / 640;
                    else        p = (float)h / 640;

                    // new dimensions
                    w = (int)(w / p);
                    h = (int)(h / p);

                    // resize
                    wb = wb.Resize(w, h, WriteableBitmapExtensions.Interpolation.Bilinear);

                    // crop
                    int x = Math.Max(0, (w - 640) / 2);
                    int y = Math.Max(0, (h - 640) / 2);
                    wb = wb.Crop(x, y, Math.Min(w, 640), Math.Min(h, 640));
                }

                // render and save stream
                wb.SaveJpeg(this.fileStream, 640, 640, 0, 90);

                //Code to display the photo on the page in an image control named myImage.
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(this.fileStream);
                //TMPIMG.Source = bmp;

                this.uploadFile();
            }
        }

        private void SendChallenge_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new Uri(App.VIEW_CAMERA, UriKind.Relative));
            photoChooserTask.Show();
        }

        private void ChallengeClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var challenge = (sender as FrameworkElement).DataContext as Challenge;
            if (challenge == null || string.IsNullOrEmpty(challenge.id)) return;

            ChallengeObject = challenge;
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(string.Format(App.VIEW_CHALLENGE, challenge.id), UriKind.Relative));
        }

        private void UserProfileClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var user = (sender as FrameworkElement).DataContext as User;
            if (user == null || string.IsNullOrEmpty(user.id)) return;

            App.UserProfileObject = user;
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(string.Format(App.VIEW_PROFILE, user.id), UriKind.Relative));
        }
    }
}