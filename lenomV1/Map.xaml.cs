using lenomV1.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
    public sealed partial class Map : Page
    {
        private Geolocator locator;//the Geolocator instance to find the location
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public Map()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper {get { return this.navigationHelper; }}

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel{  get { return this.defaultViewModel; }}

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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e) { }

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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {//MapManager.ShowDownloadedMapsUI(); for download map
            //the map token from the devoloper account            
            MyMap.MapServiceToken = "_y9lVdW-3ewMrx0C6bGnZQ";
            //Geolocator initialization
            locator = new Geolocator();
            locator.DesiredAccuracy = PositionAccuracy.High;
            locator.MovementThreshold = 20;
            Geoposition position = null;
            try{   position= await locator.GetGeopositionAsync();}
            catch {}

            //await MyMap.TrySetViewAsync(position.Coordinate.Point, 18D);
            //centering the map to the current position
            MyMap.Center = position.Coordinate.Point;
            MyMap.ZoomLevel = 15;

            //adding icon to the current position
            MapIcon mapIcon = new MapIcon();
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(
              new Uri("ms-appx:///Assets/inside_project_img/map_pin.png"));
            mapIcon.NormalizedAnchorPoint = new Point(0.5,1);
            mapIcon.Location = position.Coordinate.Point;
            mapIcon.Title = "You are here";
            MyMap.MapElements.Add(mapIcon);

            //adding icon to the distination 
            BasicGeoposition destinationPosition = new BasicGeoposition();
            destinationPosition.Longitude = Convert.ToDouble(MainPage.DestinationLongitude);
            destinationPosition.Latitude = Convert.ToDouble(MainPage.DestinationLatitude);
            MapIcon mapIcon1 = new MapIcon();
            mapIcon1.Image = RandomAccessStreamReference.CreateFromUri(
              new Uri("ms-appx:///Assets/inside_project_img/map_pin.png"));
            mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1);
            mapIcon1.Location =new Geopoint(destinationPosition);
            mapIcon.Title = "You are here";
            MyMap.MapElements.Add(mapIcon1);

            //adding a circle to the current position
            Windows.UI.Xaml.Shapes.Ellipse fence = new Windows.UI.Xaml.Shapes.Ellipse();
            fence.Width = 100;
            fence.Height = 100;
            fence.Fill= new SolidColorBrush(Colors.Blue);
            fence.Stroke = new SolidColorBrush(Colors.Blue);
            fence.Opacity = 0.3;
            fence.StrokeThickness = 2;
            MapControl.SetLocation(fence, position.Coordinate.Point);
            MapControl.SetNormalizedAnchorPoint(fence, new Point(0.5, 0.5));
            MyMap.Children.Add(fence);

            // This part is for the route  .
            BasicGeoposition startLocation = new BasicGeoposition();
            startLocation.Latitude = position.Coordinate.Latitude;
            startLocation.Longitude = position.Coordinate.Longitude;
            Geopoint startPoint = new Geopoint(startLocation);
            // End at Central Park.
            BasicGeoposition endLocation = new BasicGeoposition();
            endLocation.Latitude = Convert.ToDouble(MainPage.DestinationLatitude);
            endLocation.Longitude = Convert.ToDouble(MainPage.DestinationLongitude);
            Geopoint endPoint = new Geopoint(endLocation);
            // Get the route between the points.
            MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(startPoint, endPoint,MapRouteOptimization.Time,MapRouteRestrictions.None);
            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                // Use the route to initialize a MapRouteView.
                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = Colors.Blue;
                viewOfRoute.OutlineColor = Colors.Blue;
                // Add the new MapRouteView to the Routes collection
                // of the MapControl.
                MyMap.Routes.Add(viewOfRoute);
                // Fit the MapControl to the route.
                await MyMap.TrySetViewBoundsAsync(routeResult.Route.BoundingBox,null,Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);

            }

            //locator.PositionChanged += locator_PositionChanged;
            // locator.StatusChanged += locator_StatusChanged;
            // this.navigationHelper.OnNavigatedTo(e);
            //after finding the route  we disable the progress ring 
            progress.Opacity = 0;
            appBar.IsEnabled = true;
            subtitle.Text = "Follow the road";
            progress.Opacity = 0;
            MyMap.Opacity = 1;
        }

       /* private void locator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            string status = "";
            switch(args.Status)
            {
                case PositionStatus.Disabled:
                    status = "GPS Disabled";
                    break;
                case PositionStatus.Initializing:
                    status = "GPS Initializing";
                    break;

                case PositionStatus.Ready:
                    status = "GPS Ready";
                    break;
                    
            }
             
            //throw new NotImplementedException();
        }*/


      /*  private void locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
           // throw new NotImplementedException();
        }*/

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //locator.PositionChanged -= locator_PositionChanged;
            //locator.StatusChanged -= locator_StatusChanged;
             locator = null;
            // this.navigationHelper.OnNavigatedFrom(e);
        }
        #endregion
        //the main click handler
        private void Main_Click(object sender, RoutedEventArgs e){Frame.Navigate(typeof(MainPage));}

        //the zoom Out handler
        private void Zoom_Out_Click(object sender, RoutedEventArgs e)
        {
            if (MyMap.ZoomLevel == 0) return;
            else MyMap.ZoomLevel--;
        }

        //the zoom out handler
        private void Zoom_In_Click(object sender, RoutedEventArgs e)
        {
            BasicGeoposition destinationPosition1 = new BasicGeoposition();
            destinationPosition1.Longitude = Convert.ToDouble(MainPage.MyLongitude);
            destinationPosition1.Latitude = Convert.ToDouble(MainPage.MyLatitude);

            if (MyMap.ZoomLevel == 20) return;
            else MyMap.ZoomLevel++;
            MyMap.Center = new Geopoint(destinationPosition1);
        }

        //this is the map loaded event
        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
           // Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "ApplicationID";
           // Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "AuthenticationToken";

        }
    }
}
