using lenomV1.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace lenomV1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SuperSelector : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public SuperSelector()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

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
            Random random = new Random();
            try
            {
                header.Text = MainPage.SelectorHeader; 
                Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
                string response = await httpClient.GetStringAsync(
                new Uri(MainPage.ServerLink+"index.php?format="+MainPage.FromPageParam+"&passCategorie="+Selector.passCategorie+"&random=" + 
                random.Next().ToString())
               );
               myClassBinder root = JsonConvert.DeserializeObject<myClassBinder>(response);
                progress.Opacity = 0;
                listSelector1.ItemsSource = root.item;
               if (MainPage.FromPageParam == "help") MainPage.remember_help_askhelp = true;
            }
            catch(Exception ex)
            {
                FadeImageStoryboard.Begin();
                imgmoveStoryBoard.Begin();
                blockmoveStoryBoard.Begin();
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
          //this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainPage.DestinationName= ((sender as StackPanel).FindName("gategorieTextBlock") as TextBlock).Text;
            MainPage.DestinationWilaya= ((sender as StackPanel).FindName("wilayaTextBlock") as TextBlock).Text;
            MainPage.DestinationCommune = ((sender as StackPanel).FindName("communeTextBlock") as TextBlock).Text;
            MainPage.DestinationLatitude = ((sender as StackPanel).FindName("latitudeTextBlock") as TextBlock).Text;
            MainPage.DestinationLongitude = ((sender as StackPanel).FindName("longitudeTextBlock") as TextBlock).Text;
            MainPage.DestinationPhone = ((sender as StackPanel).FindName("telTextBlock") as TextBlock).Text;
            MainPage.DestinationId = ((sender as StackPanel).FindName("idTextBlock") as TextBlock).Text;

            if (MainPage.FromPageParam == "compagnship" || MainPage.FromPageParam == "reparation" || MainPage.FromPageParam == "help")
            {
                Frame.Navigate(typeof(GuideChoices));
            }
            else if (MainPage.FromPageParam == "askhelp")
            {
                Frame.Navigate(typeof(GuideChoices));
            }

        }
    }
}
