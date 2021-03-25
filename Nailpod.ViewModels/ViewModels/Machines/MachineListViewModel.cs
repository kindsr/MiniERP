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
    #region MachineListArgs
    public class MachineListArgs
    {
        static public MachineListArgs CreateEmpty() => new MachineListArgs { IsEmpty = true };

        public MachineListArgs()
        {
            OrderBy = r => r.Name;
        }

        public bool IsEmpty { get; set; }

        public string Query { get; set; }

        public Expression<Func<Product, object>> OrderBy { get; set; }
        public Expression<Func<Product, object>> OrderByDesc { get; set; }
    }
    #endregion

    public class MachineListViewModel : GenericListViewModel<MachineModel>
    {
        public MachineListViewModel(IMachineService machineService, ICommonServices commonServices) : base(commonServices)
        {
            MachineService = machineService;
        }

        public IMachineService MachineService { get; }

        public MachineListArgs ViewModelArgs { get; private set; }

        public ICommand ItemInvokedCommand => new RelayCommand<MachineModel>(ItemInvoked);
        private async void ItemInvoked(MachineModel model)
        {
            await NavigationService.CreateNewViewAsync<MachineDetailsViewModel>(new MachineDetailsArgs { MachineId = model.MachineId });
        }

        public async Task LoadAsync(MachineListArgs args)
        {
            ViewModelArgs = args ?? MachineListArgs.CreateEmpty();
            Query = ViewModelArgs.Query;

            StartStatusMessage("Loading machines...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Machines loaded");
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<MachineListViewModel>(this, OnMessage);
            MessageService.Subscribe<MachineDetailsViewModel>(this, OnMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public ProductListArgs CreateArgs()
        {
            return new ProductListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
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
                Items = new List<MachineModel>();
                StatusError($"Error loading Products: {ex.Message}");
                LogException("Products", "Refresh", ex);
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

        private async Task<IList<MachineModel>> GetItemsAsync()
        {
            if (!ViewModelArgs.IsEmpty)
            {
                DataRequest<MachineModel> request = BuildDataRequest();
                return await MachineService.GetMachinesAsync(request);
            }
            return new List<MachineModel>();
        }

        protected override async void OnNew()
        {

            if (IsMainView)
            {
                await NavigationService.CreateNewViewAsync<MachineDetailsViewModel>(new MachineDetailsArgs());
            }
            else
            {
                NavigationService.Navigate<MachineDetailsViewModel>(new MachineDetailsArgs());
            }

            StatusReady();
        }

        protected override async void OnRefresh()
        {
            StartStatusMessage("Loading machines...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Machines loaded");
            }
        }

        //protected override async void OnDeleteSelection()
        //{
        //    StatusReady();
        //    if (await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected machines?", "Ok", "Cancel"))
        //    {
        //        int count = 0;
        //        try
        //        {
        //            if (SelectedIndexRanges != null)
        //            {
        //                count = SelectedIndexRanges.Sum(r => r.Length);
        //                StartStatusMessage($"Deleting {count} machines...");
        //                await DeleteRangesAsync(SelectedIndexRanges);
        //                MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
        //            }
        //            else if (SelectedItems != null)
        //            {
        //                count = SelectedItems.Count();
        //                StartStatusMessage($"Deleting {count} products...");
        //                await DeleteItemsAsync(SelectedItems);
        //                MessageService.Send(this, "ItemsDeleted", SelectedItems);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            StatusError($"Error deleting {count} Products: {ex.Message}");
        //            LogException("Products", "Delete", ex);
        //            count = 0;
        //        }
        //        await RefreshAsync();
        //        SelectedIndexRanges = null;
        //        SelectedItems = null;
        //        if (count > 0)
        //        {
        //            EndStatusMessage($"{count} products deleted");
        //        }
        //    }
        //}

        //private async Task DeleteItemsAsync(IEnumerable<MachineModel> models)
        //{
        //    foreach (var model in models)
        //    {
        //        await MachineService.DeleteProductAsync(model);
        //    }
        //}

        //private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        //{
        //    DataRequest<Product> request = BuildDataRequest();
        //    foreach (var range in ranges)
        //    {
        //        await ProductService.DeleteProductRangeAsync(range.Index, range.Length, request);
        //    }
        //}

        private DataRequest<MachineModel> BuildDataRequest()
        {
            return new DataRequest<MachineModel>()
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
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
