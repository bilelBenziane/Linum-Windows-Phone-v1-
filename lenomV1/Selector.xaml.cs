using lenomV1.Common;
using Newtonsoft.Json;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace lenomV1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Selector : Page
    {
        public static string passCategorie = "";
        public static string passImg = "";
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public Selector()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper{get { return this.navigationHelper; }}

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel{get { return this.defaultViewModel; }}

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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e){}

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e){    }

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
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //this part is to fill the listbox
            header.Text = MainPage.SelectorHeader;
            Random random = new Random();//generate a random number
            try
            {
                //this is the httpClient 
                Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
                string  response = await httpClient.GetStringAsync(
                   new Uri(MainPage.ServerLink +"index.php?format="+MainPage.FromPageParam+"&random="+random.Next().ToString())
                   );
                   //convert the json format to the list for the listbox
                   myClassBinder root = JsonConvert.DeserializeObject<myClassBinder>(response);
                  progress.Opacity = 0;
                 //binding the listbox to the list
                  listSelector1.ItemsSource = root.item;
           }
            catch (Exception ex)
            {
                progress.Opacity = 0;
                FadeImageStoryboard.Begin();
                imgmoveStoryBoard.Begin();
                blockmoveStoryBoard.Begin();
            }
 
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {         // this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        //this is to handel the selection from the listbox based on the gategorie
        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Selector.passImg= ((sender as StackPanel).FindName("gategorieImg") as Image).Source.ToString();
            Selector.passCategorie = ((sender as StackPanel).FindName("gategorieTextBlock") as TextBlock).Text;


            if (MainPage.FromPageParam == "askhelp" || MainPage.FromPageParam == "reparation" || MainPage.FromPageParam == "compagnship")
            {
                Frame.Navigate(typeof(SuperSelector));
            }
            else if (MainPage.FromPageParam == "help")
            {
               Selector.passImg = ((sender as StackPanel).FindName("img") as TextBlock).Text;
               Frame.Navigate(typeof(AskHelp));
            }
            else if (MainPage.FromPageParam == "urgent")
            {
                Frame.Navigate(typeof(urgent_finder));
            }
            else if (MainPage.FromPageParam == "donation")
            {
                MainPage.DestinationLatitude = ((sender as StackPanel).FindName("latitudeTextBlock") as TextBlock).Text;
                MainPage.DestinationLongitude = ((sender as StackPanel).FindName("longitudeTextBlock") as TextBlock).Text;
                MainPage.DestinationPhone = ((sender as StackPanel).FindName("telTextBlock") as TextBlock).Text;
                Frame.Navigate(typeof(GuideChoices));
            }
        }
        //main click handler
        private void Main_Click(object sender, RoutedEventArgs e) {Frame.Navigate(typeof(MainPage));}
    }
}
