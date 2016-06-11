using lenomV1.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Calls;
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
    public sealed partial class GuideChoices : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public static bool remember = true;

        public GuideChoices()
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
        public ObservableDictionary DefaultViewModel{ get { return this.defaultViewModel; }}

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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e){  }

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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // this.navigationHelper.OnNavigatedTo(e);
            MainPage.remember_help_askhelp = true;
            asker.Text = "Ask " + MainPage.DestinationName + " via a phone call on : " + MainPage.DestinationPhone;
        }
    //this is the command handler for asking the helpers whether they are sure before going to the map page to delete the help request
        public async void CommandHandlers(IUICommand commandLabel)
        {
            var Actions = commandLabel.Label;
            switch (Actions)
            {
                case "Yes":
                    Random random = new Random();
                    try
                    {
                        Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
                        string response = await httpClient.GetStringAsync(
                           new Uri(MainPage.ServerLink + "askhelp.php?TargetId=" + MainPage.DestinationId +
                                                         "&MyCategorie=" + Selector.passCategorie +
                                                         "&random=" + random.Next().ToString())
                                                         );
                        Frame.Navigate(typeof(Map));
                    }
                    catch (Exception ex)
                    {
                        MainPage.remember_help_askhelp = true;
                        var messageDialog1 = new MessageDialog("An error happend, try again!");
                        await messageDialog1.ShowAsync();
                    }
                    
                 break;
                case "No": MainPage.remember_help_askhelp = true;  break;
            }
        }
        //________________________________________________________________________________________________________________________________
        public async void CommandHandlers1(IUICommand commandLabel)
        {
            var Actions = commandLabel.Label;
            switch (Actions)
            {
                case "Yes":
                    Random random = new Random();
                    try
                    {
                        Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
                        string response = await httpClient.GetStringAsync(
                           new Uri(MainPage.ServerLink + "delete_fixed.php?phono=" + MainPage.DestinationPhone +
                                                         "&random=" + random.Next().ToString())
                                                         );
                     Frame.Navigate(typeof(Map));
                    }
                    catch (Exception ex)
                    {
                        MainPage.remember_help_askhelp = true;
                        var messageDialog1 = new MessageDialog("An error happend, try again!");
                        await messageDialog1.ShowAsync();
                    }

                    break;
                case "No": MainPage.remember_help_askhelp = true; break;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e){ }

        #endregion

        private void Main_Click(object sender, RoutedEventArgs e){Frame.Navigate(typeof(MainPage));}//the app bar button handler to go to main menu

        private void CallButton_Click(object sender, RoutedEventArgs e)// this is to make the phone call
        { PhoneCallManager.ShowPhoneCallUI(MainPage.DestinationPhone, MainPage.DestinationName); }

        //This is the map click handler for all the map user  
        private async void MapButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainPage.FromPageParam == "askhelp" && MainPage.remember_help_askhelp==true)
            {
                MainPage.remember_help_askhelp = false;
                var messageDialog = new MessageDialog("The client will be erased from  the DB?");
                messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(CommandHandlers)));
                messageDialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(CommandHandlers)));
                await messageDialog.ShowAsync();
            }
            else if (MainPage.FromPageParam == "urgent" && MainPage.remember_help_askhelp == true)
            {
                MainPage.remember_help_askhelp = false;
                var messageDialog1 = new MessageDialog("The client will be erased from  the DB?");
                messageDialog1.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(CommandHandlers1)));
                messageDialog1.Commands.Add(new UICommand("No", new UICommandInvokedHandler(CommandHandlers1)));
                await messageDialog1.ShowAsync();
            }
            else    {Frame.Navigate(typeof(Map));}
        }
    }
}
