using lenomV1.Common;
using Newtonsoft.Json;
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
    public sealed partial class activeservice : Page
    {
//        StreamSocket sock = new StreamSocket();
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        DispatcherTimer mytimer; //this is a thread that will be executed each several seconds   
        int i=0;

        public activeservice()
        {
            this.InitializeComponent();
        
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            //this is the initialization of the myTimer
            mytimer = new DispatcherTimer();
            mytimer.Interval = new TimeSpan(0, 0, 0, 10,0);//this means execute each 10 seconds
            mytimer.Tick += new EventHandler<Object>(myTime_Tick);
        }
        private async void myTime_Tick(object sender, object e)
        {
            Random random = new Random();//generating a random number
            try
            {
                gui.Content = "looking...";
                //this httpClient is to check whether there is a job or not
                Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
                string response = await httpClient.GetStringAsync(
                   new Uri(MainPage.ServerLink + "fixdetect.php?categorie="+Selector.passCategorie+"&random=" + random.Next().ToString())
                   );
                //the part is to convert the response from the json to a list of the type myClassBinder 
                myClassBinder root = JsonConvert.DeserializeObject<myClassBinder>(response);
                //this is to get the tel lat and long from the list
                MainPage.DestinationPhone= root.item[0].tel;
                MainPage.DestinationLatitude = root.item[0].latitude;
                MainPage.DestinationLongitude = root.item[0].longitude;
                gui.Content = "Take the job";
                gui.Content = MainPage.DestinationPhone;
                gui.IsEnabled = true;
                mytimer.Stop();
            }
            catch (Exception ex)
            {
                gui.Content = "wrong";
            }

        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper{ get { return this.navigationHelper; } }
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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e){ }
        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e){ }
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
        protected override void OnNavigatedTo(NavigationEventArgs e){ this.navigationHelper.OnNavigatedTo(e);}
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //this.navigationHelper.OnNavigatedFrom(e);
            //this is to stop the myTime before leaving the page
            mytimer.Stop();
        }
        #endregion
        private void Main_Click(object sender, RoutedEventArgs e){ Frame.Navigate(typeof(MainPage));}

        //this button is to activate the service 
        private async void activate_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();//generate a random number
            try
            {
                Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();//this is the httpClient to modify the state of the employee in the data base
                string response = await httpClient.GetStringAsync(
                   new Uri(MainPage.ServerLink + "activeservice.php?phone="+MainPage.MyPhone+"&side=do&random=" + random.Next().ToString())
                   );
                myClassBinder root = JsonConvert.DeserializeObject<myClassBinder>(response);
                Selector.passCategorie = root.item[0].categorie;
                gui.Content = Selector.passCategorie;
                mytimer.Start();//starting looking for jobs
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("Something went wrong");
                await dialog.ShowAsync();
          }
          subtitle.Text = "Waite for requests"; 
        }

        //this button is to stop the service
        private async void stop_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random(); //generating a random number
            try
            {

                Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient(); //httpClient to stop the job requests recivement
                string response = await httpClient.GetStringAsync(
                   new Uri(MainPage.ServerLink + "activeservice.php?phone=" + MainPage.MyPhone + "&random=" + random.Next().ToString())
                   );
                var dialog = new MessageDialog(response);
                await dialog.ShowAsync();
                mytimer.Stop();//stop the looking for jobs
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("Something went wrong");
                await dialog.ShowAsync();
            }
            subtitle.Text = "You are in stop mode";
        }
        private void gui_Click(object sender, RoutedEventArgs e) { Frame.Navigate(typeof(GuideChoices));  }
    }
}
