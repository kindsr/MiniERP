#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Nailpod.Models;
using Nailpod.ViewModels;

namespace Nailpod.Views
{
    public sealed partial class PlacesCard : UserControl
    {
        public PlacesCard()
        {
            InitializeComponent();
        }

        #region ViewModel
        public PlaceDetailsViewModel ViewModel
        {
            get { return (PlaceDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(PlaceDetailsViewModel), typeof(PlacesCard), new PropertyMetadata(null));
        #endregion

        #region Item
        public PlaceModel Item
        {
            get { return (PlaceModel)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(PlaceModel), typeof(PlacesCard), new PropertyMetadata(null));
        #endregion
    }
}
