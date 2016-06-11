using lenomV1.Common;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace lenomV1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AskHelp : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public AskHelp()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper{ get { return this.navigationHelper; }}

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel { get { return this.defaultViewModel; }}

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e) {  }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e){}

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e){  }
        protected override void OnNavigatedFrom(NavigationEventArgs e){ }

        #endregion

        private void Main_Click(object sender, RoutedEventArgs e){ Frame.Navigate(typeof(MainPage));}//to go to the main

        //help submission click event handler
        private async void Submit_Click(object sender, RoutedEventArgs e) 
        {
            Random random = new Random();//generate a random number
            try
            {
                Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();//httpCleint to send the help request
                string response = await httpClient.GetStringAsync(
                   new Uri(MainPage.ServerLink + "askhelp.php?MyName=" + MainPage.MyName +
                                                 "&MyPhone=" + MainPage.MyPhone +
                                                 "&MyCategorie=" + Selector.passCategorie +
                                                 "&MyAddress=" + MainPage.MyAddress +
                                                 "&MyLongitude=" + MainPage.MyLatitude +
                                                 "&MyLatitude=" + MainPage.MyLongitude +
                                                 "&MyWilaya=" + MainPage.MyWilaya +
                                                 "&MyCommune=" + MainPage.MyCommune +
                                                 "&MyImg=" + Selector.passImg +
                                                 "&random=" + random.Next().ToString())
                   );
                var dialog = new MessageDialog(response);
                await dialog.ShowAsync();
                notif_TextBlock.Text = "Request Successfully Sent, Wait For Being Called";
                Submit.Opacity = 0;
                Return_To_Main.Opacity = 0;
            }
            catch (Exception ex)
            {
                header.Text = "";
                header.Text = "Server Might be Down!";
            }

        }
    }
}
