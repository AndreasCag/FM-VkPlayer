using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK;
using VK.WindowsPhone.SDK.API;
using VK.WindowsPhone.SDK.API.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// The WebView Application template is documented at http://go.microsoft.com/fwlink/?LinkID=391641

namespace VkPlayer
{

    public sealed partial class MainPage : Page
    {
       
        // TODO: Replace with your URL here.


        private static readonly Uri HomeUri = new Uri("ms-appx-web:///Html/index.html", UriKind.Absolute);

        private List<String> _scope = new List<string>() { VKScope.AUDIO };
        public TextBox txt;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            VKSDK.Initialize("5650916");
            VKSDK.Authorize(_scope, false, false);
            Player.CurrentStateChanged += Player_CurrentStateChanged;
            Player.BufferingProgressChanged += Player_BufferingProgressChanged;
           // Player.DownloadProgressChanged += Player_DownloadProgressChanged;
            
        }

        private void Player_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            if (MediaElementState.Playing == Player.CurrentState)
                ProgressPos.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void ProgressPos_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Point pnt = e.GetPosition(sender as UIElement);
            double ddt = pnt.X / ProgressPos.ActualWidth;
            Player.Position = new TimeSpan(0,0,(int)(Player.NaturalDuration.TimeSpan.TotalSeconds * ddt));
        }


        private void Player_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (MediaElementState.Paused == Player.CurrentState)
            {
                if (Player.NaturalDuration.TimeSpan != Player.Position)
                {
                    Grid_Start.Visibility = Visibility.Visible;
                    Grid_Stop.Visibility = Visibility.Collapsed;
                    
                }
                else
                {
                    Grid_Tapped(Grid_Next, null);
                }
            }
            else if (MediaElementState.Playing == Player.CurrentState)
            {
                Grid_Start.Visibility = Visibility.Collapsed;
                Grid_Stop.Visibility = Visibility.Visible;
            }
        }
        

        private void textRequest_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text != "")
            {
                VKRequest.Dispatch<VKList<NewVKAudio>>(new VKRequestParameters(
                    "audio.search",
                    "q", (sender as TextBox).Text
                    ),
                    (result) =>
                    {
                        audioView.ItemsSource = result.Data.items;
                    }

                    );
            }
        }

        private static string GetTrueUrl(string url)
        {
            return url.Substring(0, url.IndexOf('?'));
        }
        
        private void PlayUrl(string url)
        {
            
            MyProgress.Visibility = Visibility.Visible;
            Player.Source = new Uri(GetTrueUrl(url));
            Player.Play();
            audioView.ScrollIntoView(audioView.SelectedItem);
        }

        private void audioView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (audioView.SelectedIndex >= 0 && audioView.SelectedIndex < audioView.Items.Count)
            {
                if (new Uri(GetTrueUrl((audioView.SelectedItem as NewVKAudio).url)) != Player.Source)
                    PlayUrl((audioView.SelectedItem as NewVKAudio).url);
                else if (Player.CurrentState == MediaElementState.Playing)
                    Grid_Stop_Tapped(Grid_Stop, null);
                else if (Player.CurrentState == MediaElementState.Paused)
                    Grid_Start_Tapped(Grid_Start, null);
            }
        }
        
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (audioView.Items != null)
            {
                bool k=false;
                if ((sender as Grid).Name == "Grid_Next")
                    k = true;

                if ((audioView.SelectedIndex == (k?(audioView.Items.Count - 1):0)))
                    audioView.SelectedIndex = k?0: (audioView.Items.Count - 1);
                else
                    audioView.SelectedIndex+=k?1:-1;

                PlayUrl((audioView.SelectedItem as NewVKAudio).url);

            }
        }

        private void Grid_Start_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Player.Source!=null)
            {
                Player.Play();
            }
        }

        private void Grid_Stop_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.Pause();
            }
        }

    }
}
