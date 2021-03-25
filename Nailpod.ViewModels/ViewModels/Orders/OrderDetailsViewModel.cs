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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Nailpod.Models;
using Nailpod.Services;

namespace Nailpod.ViewModels
{
    #region OrderDetailsArgs
    public class OrderDetailsArgs
    {
        static public OrderDetailsArgs CreateDefault() => new OrderDetailsArgs { PlaceID = 0 };

        public long PlaceID { get; set; }
        public long OrderID { get; set; }

        public bool IsNew => OrderID <= 0;
    }
    #endregion

    public class OrderDetailsViewModel : GenericDetailsViewModel<OrderModel>
    {
        public OrderDetailsViewModel(IOrderService orderService, ICommonServices commonServices) : base(commonServices)
        {
            OrderService = orderService;
        }

        public IOrderService OrderService { get; }

        override public string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
        public string TitleNew => Item?.Place == null ? "New Order" : $"New Order, {Item?.Place}";
        public string TitleEdit => Item == null ? "Order" : $"Order #{Item?.OrderID}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public bool CanEditPlace => Item?.PlaceID <= 0;

        public ICommand PlaceSelectedCommand => new RelayCommand<PlaceModel>(PlaceSelected);
        private void PlaceSelected(PlaceModel place)
        {
            EditableItem.PlaceID = place.PlaceID;
            EditableItem.ShipAddress = place.Address;
            EditableItem.ShipCity = place.City;
            EditableItem.ShipRegion = place.Region;
            EditableItem.ShipCountryCode = place.CountryCode;
            EditableItem.ShipPostalCode = place.PostalCode;
            EditableItem.Place = place;

            EditableItem.NotifyChanges();
        }

        public OrderDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(OrderDetailsArgs args)
        {
            ViewModelArgs = args ?? OrderDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = await OrderService.CreateNewOrderAsync(ViewModelArgs.PlaceID);
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await OrderService.GetOrderAsync(ViewModelArgs.OrderID);
                    Item = item ?? new OrderModel { OrderID = ViewModelArgs.OrderID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    LogException("Order", "Load", ex);
                }
            }
            NotifyPropertyChanged(nameof(ItemIsNew));
        }
        public void Unload()
        {
            ViewModelArgs.PlaceID = Item?.PlaceID ?? 0;
            ViewModelArgs.OrderID = Item?.OrderID ?? 0;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<OrderDetailsViewModel, OrderModel>(this, OnDetailsMessage);
            MessageService.Subscribe<OrderListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public OrderDetailsArgs CreateArgs()
        {
            return new OrderDetailsArgs
            {
                PlaceID = Item?.PlaceID ?? 0,
                OrderID = Item?.OrderID ?? 0
            };
        }

        protected override async Task<bool> SaveItemAsync(OrderModel model)
        {
            try
            {
                StartStatusMessage("Saving order...");
                await Task.Delay(100);
                await OrderService.UpdateOrderAsync(model);
                EndStatusMessage("Order saved");
                LogInformation("Order", "Save", "Order saved successfully", $"Order #{model.OrderID} was saved successfully.");
                NotifyPropertyChanged(nameof(CanEditPlace));
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Order: {ex.Message}");
                LogException("Order", "Save", ex);
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(OrderModel model)
        {
            try
            {
                StartStatusMessage("Deleting order...");
                await Task.Delay(100);
                await OrderService.DeleteOrderAsync(model);
                EndStatusMessage("Order deleted");
                LogWarning("Order", "Delete", "Order deleted", $"Order #{model.OrderID} was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Order: {ex.Message}");
                LogException("Order", "Delete", ex);
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current order?", "Ok", "Cancel");
        }

        override protected IEnumerable<IValidationConstraint<OrderModel>> GetValidationConstraints(OrderModel model)
        {
            yield return new RequiredGreaterThanZeroConstraint<OrderModel>("Place", m => m.PlaceID);
            if (model.Status > 0)
            {
                yield return new RequiredConstraint<OrderModel>("Payment Type", m => m.PaymentType);
                yield return new RequiredGreaterThanZeroConstraint<OrderModel>("Payment Type", m => m.PaymentType);
                if (model.Status > 1)
                {
                    yield return new RequiredConstraint<OrderModel>("Shipper", m => m.ShipVia);
                    yield return new RequiredGreaterThanZeroConstraint<OrderModel>("Shipper", m => m.ShipVia);
                }
            }
        }

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(OrderDetailsViewModel sender, string message, OrderModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.OrderID == current?.OrderID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await ContextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await OrderService.GetOrderAsync(current.OrderID);
                                    item = item ?? new OrderModel { OrderID = current.OrderID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    NotifyPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        StatusMessage("WARNING: This order has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogException("Order", "Handle Changes", ex);
                                }
                            });
                            break;
                        case "ItemDeleted":
                            await OnItemDeletedExternally();
                            break;
                    }
                }
            }
        }

        private async void OnListMessage(OrderListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<OrderModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.OrderID == current.OrderID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await OrderService.GetOrderAsync(current.OrderID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException("Order", "Handle Ranges Deleted", ex);
                        }
                        break;
                }
            }
        }

        private async Task OnItemDeletedExternally()
        {
            await ContextService.RunAsync(() =>
            {
                CancelEdit();
                IsEnabled = false;
                StatusMessage("WARNING: This order has been deleted externally");
            });
        }
    }
}
