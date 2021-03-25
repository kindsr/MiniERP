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
using System.Threading.Tasks;

using Nailpod.Models;
using Nailpod.Services;

namespace Nailpod.ViewModels
{
    public class PlacesViewModel : ViewModelBase
    {
        public PlacesViewModel(IPlaceService placeService, IOrderService orderService, IFilePickerService filePickerService, ICommonServices commonServices) : base(commonServices)
        {
            PlaceService = placeService;

            PlaceList = new PlaceListViewModel(PlaceService, commonServices);
            PlaceDetails = new PlaceDetailsViewModel(PlaceService, filePickerService, commonServices);
            PlaceOrders = new OrderListViewModel(orderService, commonServices);
        }

        public IPlaceService PlaceService { get; }

        public PlaceListViewModel PlaceList { get; set; }
        public PlaceDetailsViewModel PlaceDetails { get; set; }
        public OrderListViewModel PlaceOrders { get; set; }

        public async Task LoadAsync(PlaceListArgs args)
        {
            await PlaceList.LoadAsync(args);
        }
        public void Unload()
        {
            PlaceDetails.CancelEdit();
            PlaceList.Unload();
        }

        public void Subscribe()
        {
            MessageService.Subscribe<PlaceListViewModel>(this, (Action<PlaceListViewModel, string, object>)this.OnMessage);
            PlaceList.Subscribe();
            PlaceDetails.Subscribe();
            PlaceOrders.Subscribe();
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            PlaceList.Unsubscribe();
            PlaceDetails.Unsubscribe();
            PlaceOrders.Unsubscribe();
        }

        private async void OnMessage(PlaceListViewModel viewModel, string message, object args)
        {
            if (viewModel == PlaceList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async void OnItemSelected()
        {
            if (PlaceDetails.IsEditMode)
            {
                StatusReady();
                PlaceDetails.CancelEdit();
            }
            PlaceOrders.IsMultipleSelection = false;
            var selected = PlaceList.SelectedItem;
            if (!PlaceList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                    await PopulateOrders(selected);
                }
            }
            PlaceDetails.Item = selected;
        }

        private async Task PopulateDetails(PlaceModel selected)
        {
            try
            {
                var model = await PlaceService.GetPlaceAsync(selected.PlaceID);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                LogException("Places", "Load Details", ex);
            }
        }

        private async Task PopulateOrders(PlaceModel selectedItem)
        {
            try
            {
                if (selectedItem != null)
                {
                    await PlaceOrders.LoadAsync(new OrderListArgs { PlaceID = selectedItem.PlaceID }, silent: true);
                }
            }
            catch (Exception ex)
            {
                LogException("Places", "Load Orders", ex);
            }
        }
    }
}
