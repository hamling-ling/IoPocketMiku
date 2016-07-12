using IoPokeMikuClient.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace IoPokeMikuClient.View
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            DataContext = ViewModelLocator.Instance.Main;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // do something with param if neccessary
            //var parameter = e.Parameter as string;
            base.OnNavigatedTo(e);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainPageViewModel;
            vm.NoteOffCommand.Execute(null);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            if(combobox == null)
            {
                return;
            }
            if(combobox.SelectedItem == null)
            {
                return;
            }

            var vm = DataContext as MainPageViewModel;
            var cbItem = combobox.SelectedItem as ComboBoxItem;
            string itemString = cbItem.Content as string;
            vm.SelectPlayerCommand.Execute(itemString);
        }
    }
}
