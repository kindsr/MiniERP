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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel;

using Nailpod.Data;
using Nailpod.Models;
using Nailpod.Services;

namespace Nailpod.Controls
{
    public sealed partial class PlaceSuggestBox : UserControl
    {
        public PlaceSuggestBox()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                PlaceService = ServiceLocator.Current.GetService<IPlaceService>();
            }
            InitializeComponent();
        }

        private IPlaceService PlaceService { get; }

        #region Items
        public IList<PlaceModel> Items
        {
            get { return (IList<PlaceModel>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(IList<PlaceModel>), typeof(PlaceSuggestBox), new PropertyMetadata(null));
        #endregion

        #region DisplayText
        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof(DisplayText), typeof(string), typeof(PlaceSuggestBox), new PropertyMetadata(null));
        #endregion

        #region IsReadOnly*
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PlaceSuggestBox;
            control.suggestBox.Mode = ((bool)e.NewValue == true) ? FormEditMode.ReadOnly : FormEditMode.Auto;
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(PlaceSuggestBox), new PropertyMetadata(false, IsReadOnlyChanged));
        #endregion

        #region PlaceSelectedCommand
        public ICommand PlaceSelectedCommand
        {
            get { return (ICommand)GetValue(PlaceSelectedCommandProperty); }
            set { SetValue(PlaceSelectedCommandProperty, value); }
        }

        public static readonly DependencyProperty PlaceSelectedCommandProperty = DependencyProperty.Register(nameof(PlaceSelectedCommandProperty), typeof(ICommand), typeof(PlaceSuggestBox), new PropertyMetadata(null));
        #endregion

        private async void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (args.CheckCurrent())
                {
                    Items = String.IsNullOrEmpty(sender.Text) ? null : await GetItems(sender.Text);
                }
            }
        }

        private async Task<IList<PlaceModel>> GetItems(string query)
        {
            var request = new DataRequest<Place>()
            {
                Query = query,
                OrderBy = r => r.place_id
            };
            return await PlaceService.GetPlacesAsync(0, 20, request);
        }

        private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            PlaceSelectedCommand?.TryExecute(args.SelectedItem);
        }
    }
}
