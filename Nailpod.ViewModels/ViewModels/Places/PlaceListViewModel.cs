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
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Nailpod.Data;
using Nailpod.Models;
using Nailpod.Services;

namespace Nailpod.ViewModels
{
    #region PlaceListArgs
    public class PlaceListArgs
    {
        static public PlaceListArgs CreateEmpty() => new PlaceListArgs { IsEmpty = true };

        public PlaceListArgs()
        {
            OrderBy = r => r.place_id;
        }

        public bool IsEmpty { get; set; }
        public long CustomerID { get; set; }
        public string Query { get; set; }

        public Expression<Func<Place, object>> OrderBy { get; set; }
        public Expression<Func<Place, object>> OrderByDesc { get; set; }
    }
    #endregion

    public class PlaceListViewModel : GenericListViewModel<PlaceModel>
    {
        public PlaceListViewModel(IPlaceService placeService, ICommonServices commonServices) : base(commonServices)
        {
            PlaceService = placeService;
        }

        public IPlaceService PlaceService { get; }

        public PlaceListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(PlaceListArgs args, bool silent = false)
        {
            ViewModelArgs = args ?? PlaceListArgs.CreateEmpty();
            Query = ViewModelArgs.Query;

            if (silent)
            {
                await RefreshAsync();
            }
            else
            {
                StartStatusMessage("Loading places...");
                if (await RefreshAsync())
                {
                    EndStatusMessage("Places loaded");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<PlaceListViewModel>(this, OnMessage);
            MessageService.Subscribe<PlaceDetailsViewModel>(this, OnMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public PlaceListArgs CreateArgs()
        {
            return new PlaceListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc,
                CustomerID = ViewModelArgs.CustomerID
            };
        }

        public async Task<bool> RefreshAsync()
        {
            bool isOk = true;

            Items = null;
            ItemsCount = 0;
            SelectedItem = null;

            try
            {
                Items = await GetItemsAsync();
            }
            catch (Exception ex)
            {
                Items = new List<PlaceModel>();
                StatusError($"Error loading Places: {ex.Message}");
                LogException("Places", "Refresh", ex);
                isOk = false;
            }

            ItemsCount = Items.Count;
            if (!IsMultipleSelection)
            {
                SelectedItem = Items.FirstOrDefault();
            }
            NotifyPropertyChanged(nameof(Title));

            return isOk;
        }

        private async Task<IList<PlaceModel>> GetItemsAsync()
        {
            if (!ViewModelArgs.IsEmpty)
            {
                DataRequest<Place> request = BuildDataRequest();
                return await PlaceService.GetPlacesAsync(request);
            }
            return new List<PlaceModel>();
        }

        public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
        private async void OnOpenInNewView()
        {
            if (SelectedItem != null)
            {
                await NavigationService.CreateNewViewAsync<PlaceDetailsViewModel>(new PlaceDetailsArgs { PlaceID = SelectedItem.PlaceID });
            }
        }

        protected override async void OnNew()
        {
            if (IsMainView)
            {
                await NavigationService.CreateNewViewAsync<PlaceDetailsViewModel>(new PlaceDetailsArgs { CustomerID = ViewModelArgs.CustomerID });
            }
            else
            {
                NavigationService.Navigate<PlaceDetailsViewModel>(new PlaceDetailsArgs { CustomerID = ViewModelArgs.CustomerID });
            }

            StatusReady();
        }

        protected override async void OnRefresh()
        {
            StartStatusMessage("Loading places...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Places loaded");
            }
        }

        protected override async void OnDeleteSelection()
        {
            StatusReady();
            if (await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected places?", "Ok", "Cancel"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} places...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} places...");
                        await DeleteItemsAsync(SelectedItems);
                        MessageService.Send(this, "ItemsDeleted", SelectedItems);
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Places: {ex.Message}");
                    LogException("Places", "Delete", ex);
                    count = 0;
                }
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    EndStatusMessage($"{count} places deleted");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<PlaceModel> models)
        {
            foreach (var model in models)
            {
                await PlaceService.DeletePlaceAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Place> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await PlaceService.DeletePlaceRangeAsync(range.Index, range.Length, request);
            }
        }

        private DataRequest<Place> BuildDataRequest()
        {
            var request = new DataRequest<Place>()
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
            if (ViewModelArgs.CustomerID > 0)
            {
                request.Where = (r) => r.customer_id == ViewModelArgs.CustomerID;
            }
            return request;
        }

        private async void OnMessage(ViewModelBase sender, string message, object args)
        {
            switch (message)
            {
                case "NewItemSaved":
                case "ItemDeleted":
                case "ItemsDeleted":
                case "ItemRangesDeleted":
                    await ContextService.RunAsync(async () =>
                    {
                        await RefreshAsync();
                    });
                    break;
            }
        }
    }
}
