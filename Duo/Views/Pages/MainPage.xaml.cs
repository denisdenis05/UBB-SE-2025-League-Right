using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Duo.ViewModels;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Duo.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel _viewModel;

        public MainPage()
        {
            this.InitializeComponent();
            _viewModel = new MainPageViewModel();
            // Subscribe to events
            _viewModel.NavigationRequested += OnNavigationRequested;
            try
            {
                //contentFrame.Navigate(typeof(AdminMainPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Initial navigation failed: {ex.Message}");
            }
        }
        

        private void NavigationView_SelectionChanged(object sender, NavigationViewSelectionChangedEventArgs args)
        {
            _viewModel.HandleNavigationSelectionChanged(args);
        }

        private void OnNavigationRequested(object sender, Type pageType)
        {
            try
            {
                contentFrame.Navigate(pageType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation failed: {ex.Message}");
            }
        }
    }

}
