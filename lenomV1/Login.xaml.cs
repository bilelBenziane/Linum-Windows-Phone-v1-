using lenomV1.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.System;
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
    public sealed partial class Login : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
       
        public Login()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper { get { return this.navigationHelper; }}
        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel{ get { return this.defaultViewModel; }  }
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
            //this part is to recover the username and password if they exist
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if(localSettings.Values.ContainsKey("phone") && localSettings.Values.ContainsKey("password"))
            {
                user_name.Text = localSettings.Values["phone"].ToString();
                password.Text = localSettings.Values["password"].ToString();
            }
        }
        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e){        }
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
        protected override void OnNavigatedTo(NavigationEventArgs e){this.navigationHelper.OnNavigatedTo(e);}
        protected override void OnNavigatedFrom(NavigationEventArgs e){this.navigationHelper.OnNavigatedFrom(e);}

        #endregion
        private void sign_up_Click(object sender, RoutedEventArgs e){Frame.Navigate(typeof(Signper));}// this butto is to go to the signup page

        private async void submitt_Click(object sender, RoutedEventArgs e)
        {
            //this part is to handel all the possibilities for the login 
            if(password.Text=="" || user_name.Text=="")
            {
                var dialog_empty = new MessageDialog("Enter username and password");
                await dialog_empty.ShowAsync();
            }
            else
            {
                try
                {
                    buttom_text.Text = "Looking.....";
                    Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
                    string response = await httpClient.GetStringAsync(
                                      new Uri(MainPage.ServerLink + "login.php?username=" + user_name.Text + "&random=" + (new Random()).Next().ToString())
                                        );
                    myClassBinder root = JsonConvert.DeserializeObject<myClassBinder>(response);
                    string til = root.item[0].password;
                    MainPage.MyPhone = root.item[0].tel;
                    //var dialogx = new MessageDialog(til);
                    //await dialogx.ShowAsync();
                    if (password.Text == til)
                    {
                        //if the password is true then save it and the user name, then go to the main menu
                        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                        localSettings.Values["phone"] = user_name.Text;
                        localSettings.Values["password"] = password.Text;
                        Frame.Navigate(typeof(MainPage));

                    }
                    else if (til == "")
                    {
                        var dialog_wrong_user = new MessageDialog("Wrong user name");
                        await dialog_wrong_user.ShowAsync();
                    }
                    else
                    {
                        var dialog_wrong_pass = new MessageDialog("Wrong password");
                        await dialog_wrong_pass.ShowAsync();
                    }

                }
                catch (Exception ex)
                {
                    buttom_text.Text = "Follow us on";
                    var dialog_catch = new MessageDialog("Something Went wrong, check your internet connection!");
                    await dialog_catch.ShowAsync();
                }
            }

        }

        //this button is to handle the animations after pushing the login  
        private void login_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(MainPage));
            login.Opacity = 0;
            sign_up.Opacity = 0;
            sign_up.IsEnabled = false;
            login.IsEnabled = false;
            move_box_pass_StoryBoard.Begin();
            up_text_StoryBoard.Begin();
            move_box_userStoryBoard.Begin();
            move_buttonStoryBoard.Begin();
            login_StoryBoard.Begin();
            user_name.Opacity = 1;
            user_name.IsEnabled = true;
            password.Opacity = 1;
            password.IsEnabled = true;
            submitt.Opacity = 1;
            submitt.IsEnabled = true;
        }

        //this button is to open the twitter official page
        private async void Image_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        { await Launcher.LaunchUriAsync(new Uri("https://twitter.com/linum_connect"));}

        //this button is to open the facebook official page
        private async void Image_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        { await Launcher.LaunchUriAsync(new Uri("https://web.facebook.com/linumalg/"));}
    }
}
