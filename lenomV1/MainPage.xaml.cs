using System;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=391641

namespace lenomV1
{

    public sealed partial class MainPage : Page
    {
        private Geolocator locator;
      
        public static string MyPhone = "0698300131";
        public static string MyName  = "Benziane Bilel";
        public static string MyAddress = "El mouhgoune";
        public static string MyLatitude = "3.144";
        public static string MyLongitude = "1205";
        public static string MyWilaya = "Boumerdess";//region
        public static string MyCommune = "Cordo";//town
        public static string MyCountry = "Algeria";

        public static string DestinationName = "client";
        public static string DestinationCommune = "";
        public static string DestinationWilaya = "";
        public static string DestinationLongitude = "3.46221182495356";
        public static string DestinationLatitude = "36.7481081932783";
        public static string DestinationPhone = "0698300131";
        public static string DestinationId = "";

        public static bool remember_help_askhelp = true;

        public static string SelectorHeader = "";

        public static string ServerLink = "http://linum2016.azurewebsites.net/";
        public static string FromPageParam = ""; //pass info to know the page
        private object _rootPage;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //this is to set up the gps 
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            locator = new Geolocator();
            locator.DesiredAccuracy = PositionAccuracy.High;
            locator.MovementThreshold = 20;
            locator.StatusChanged += geolocator_StatusChanged;
        }

        //this part is to handel the different states of the GPS
        private async void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            string status="";

             switch(args.Status)
             {
                 case PositionStatus.Disabled:
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
                    {
                          status = "GPS Disabled";
                          gpsoff.Opacity = 1;
                          refresher.Opacity = 1;
                          refresher.IsEnabled = true;
                    });
                    
                    break;
                 case PositionStatus.Initializing:  
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        status = "Initializing";
                        gpsoff.Opacity = 0;
                        refresher.Opacity = 0;
                        refresher.IsEnabled = false;
                    });
                    break;
                   //when gps is ready 
                 case PositionStatus.Ready:         status = "GPS is ready";
                    //finding the longitude and latitude
                    var  position = await locator.GetGeopositionAsync();
                     MainPage.MyLongitude = position.Coordinate.Longitude.ToString();
                     MainPage.MyLatitude = position.Coordinate.Latitude.ToString();
                     BasicGeoposition myLocation = new BasicGeoposition
                     {
                         Longitude = position.Coordinate.Longitude,
                         Latitude = position.Coordinate.Latitude
                     };
                    //finding the exact place the town, region
                     Geopoint pointToReverseGeocode = new Geopoint(myLocation);
                     MapLocationFinderResult result = await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);
                     await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                     {                                           
                     MainPage.MyWilaya = result.Locations[0].Address.Region;
                     MainPage.MyCountry = result.Locations[0].Address.Country;
                     MainPage.MyCommune = result.Locations[0].Address.Town;
                     header.Text = "Latitude " + MainPage.MyLatitude +
                                   "Longitude " + MainPage.MyLongitude +
                                   "Country " + MainPage.MyCountry +
                                   "Commune " + MainPage.MyCommune +
                                   "Wilaya  " + MainPage.MyWilaya;
             //this place is to hide the progress ring and display the main menu button
             main_ring.IsActive = false;
             Compagnship.IsEnabled = true;
             Compagnship.Opacity = 1;    
             urgent.IsEnabled = true;
             urgent.Opacity = 1;
             reparation.IsEnabled = true;
             reparation.Opacity = 1;
             volunteer.IsEnabled = true;
             volunteer.Opacity = 1;
             main_textblock.Text = "";
                  });
                     break;
                 case PositionStatus.NotAvailable:
                 case PositionStatus.NotInitialized:
                 case PositionStatus.NoData:
                     break;
             }
            //this is the dispatcher is to modify the GUI thread values from a different thread
             await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {header.Text = status;});
        }

        //__________________________________________________________________________________________________________________________________________
        //the compagnship button click handeler to go to the corresponding page
        private void compagnship_Click(object sender, RoutedEventArgs e)
        {
            MainPage.FromPageParam = "compagnship";
            SelectorHeader = "COMPAGNSHIP";
            Frame.Navigate(typeof(Selector));
        }
        //the Reparation button click handeler to go to the corresponding page
        private void reparation_Click(object sender, RoutedEventArgs e)
        {
            MainPage.FromPageParam = "reparation";
            SelectorHeader = "REPARATION";
            Frame.Navigate(typeof(Selector));
        }
        //the Volunteer button click handeler to go to the corresponding page
        private void volunteer_Click(object sender, RoutedEventArgs e){Frame.Navigate(typeof(Volunteer));}
        //the Urgent button click handeler to go to the corresponding page
        private void urgent_Click(object sender, RoutedEventArgs e)
        {
            MainPage.FromPageParam = "urgent";
            Frame.Navigate(typeof(urgent));
        }

        //this is stop the gps status event before leaving the page 
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            locator.StatusChanged -= geolocator_StatusChanged;
            locator = null;
        }

        //this is  the main click handler to go to the login page
        private void Main_Click(object sender, RoutedEventArgs e){  Frame.Navigate(typeof(Login)); }

        //this button is to refresh the main menu after activating the GPS sensor
        private void refresher_Click(object sender, RoutedEventArgs e) {  Frame.Navigate(typeof(MainPage));}
        //______________________________________________________________________________________________________________________________________________
    }
}
