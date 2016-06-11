using System;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace lenomV1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Signper : Page
    {
        private Geolocator locator;
        public Signper()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            locator = new Geolocator();
            locator.DesiredAccuracy = PositionAccuracy.High;
            locator.MovementThreshold = 20;
            locator.StatusChanged += geolocator_StatusChanged;

        }

        private async void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            string status = "";
            switch (args.Status)
            {
                case PositionStatus.Disabled: status = "GPS Disabled"; break;
                case PositionStatus.Initializing: status = "Initializing"; break;
                case PositionStatus.Ready:
                    status = "GPS is ready";
                    try
                    {
                        var position = await locator.GetGeopositionAsync();
                        BasicGeoposition myLocation;
                        myLocation = new BasicGeoposition
                        {
                            Longitude = position.Coordinate.Longitude,
                            Latitude = position.Coordinate.Latitude
                        };

                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            MainPage.MyLongitude = position.Coordinate.Longitude.ToString();
                            MainPage.MyLatitude = position.Coordinate.Latitude.ToString();
                            

                              
                        });
                        Geopoint pointToReverseGeocode = new Geopoint(myLocation);
                        MapLocationFinderResult result = await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            MainPage.MyWilaya = result.Locations[0].Address.Region;
                            MainPage.MyCountry = result.Locations[0].Address.Country;
                            MainPage.MyCommune = result.Locations[0].Address.Town;
                            submit.IsEnabled = true;

                           /* golo.Text = "Latitude " + MainPage.MyLatitude +
                                 "Longitude " + MainPage.MyLongitude +
                                 "Country " + MainPage.MyCountry +
                                 "Commune " + MainPage.MyCommune +
                                 "Wilaya  " + MainPage.MyWilaya;*/
                        });

                    }
                    catch (Exception exx)
                    {
                        var dialoger = new MessageDialog("Something went wrong, try again!");
                        await dialoger.ShowAsync();
                    }
                    break;
                case PositionStatus.NotAvailable:
                case PositionStatus.NotInitialized:
                case PositionStatus.NoData:
                    break;
            }
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { header.Text = status; });
            
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            locator.StatusChanged -= geolocator_StatusChanged;
            locator = null;
        }
        private void Main_Click(object sender, RoutedEventArgs e) { Frame.Navigate(typeof(Login)); }

        private async void submit_Click(object sender, RoutedEventArgs e)
        {
            if (username.Text == "" || password.Text == "" || phone.Text == "" || Address.Text == "" || description.Text == "")
            {
                var dialog = new MessageDialog("Fill all the fields");
                await dialog.ShowAsync();
            }
            else
            {
                string content = ((ComboBoxItem)comboBoxChoice.SelectedItem).Content.ToString();
                try
                {
                    Random random = new Random();
                    Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
                    string response = await httpClient.GetStringAsync(
                       new Uri(MainPage.ServerLink + "signup.php?&username=" + username.Text +
                                                   "&password=" + password.Text +
                                                   "&phone=" + phone.Text +
                                                   "&address=" + Address.Text +
                                                   "&description=" + description.Text +
                                                   "&country=" + MainPage.MyCountry +
                                                   "&wilaya=" + MainPage.MyWilaya +
                                                   "&commune=" + MainPage.MyCommune+
                                                   "&longitude=" + MainPage.MyLongitude +
                                                   "&latitude=" + MainPage.MyLatitude +
                                                   "&type=" + content +
                                                   "&random=" + random.Next().ToString())
                       );
                    comboBoxChoice.IsEnabled = false;
                    comboBoxChoice.Opacity = 0;
                    username.IsEnabled = false;
                    username.Opacity = 0;
                    password.IsEnabled = false;
                    password.Opacity = 0;
                    phone.IsEnabled = false;
                    phone.Opacity = 0;
                    Address.IsEnabled = false;
                    Address.Opacity = 0;
                    description.IsEnabled = false;
                    description.Opacity = 0;
                    submit.IsEnabled = false;
                    submit.Opacity = 0;
                    combotext.Opacity = 0;
                    combotext.Text = "";
                    descriptiontextblock.Opacity = 0;
                    descriptiontextblock.Text = "";
                    result.Opacity = 1;
                }
                catch (Exception ex)
                {
                    var dialoger = new MessageDialog("Something went wrong, try again!");
                    await dialoger.ShowAsync();
                }
            }
        }
    }
}
