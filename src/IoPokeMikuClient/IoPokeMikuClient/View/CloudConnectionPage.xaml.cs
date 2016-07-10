﻿using IoPokeMikuClient.ViewModel;
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

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace IoPokeMikuClient.View
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class CloudConnectionPage : Page
    {
        public CloudConnectionPage()
        {
            this.InitializeComponent();
            DataContext = ViewModelLocator.Instance.CloudConnection;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // do something with param if neccessary
            //var parameter = e.Parameter as string;
            base.OnNavigatedTo(e);
        }
    }
}
