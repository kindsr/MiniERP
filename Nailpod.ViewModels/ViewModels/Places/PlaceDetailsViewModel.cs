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
    #region PlaceDetailsArgs
    public class PlaceDetailsArgs
    {
        static public PlaceDetailsArgs CreateDefault() => new PlaceDetailsArgs();

        public long CustomerID { get; set; }
        public long PlaceID { get; set; }

        public bool IsNew => PlaceID <= 0;
    }
    #endregion

    public class PlaceDetailsViewModel : GenericDetailsViewModel<PlaceModel>
    {
        public PlaceDetailsViewModel(IPlaceService placeService, IFilePickerService filePickerService, ICommonServices commonServices) : base(commonServices)
        {
            PlaceService = placeService;
            FilePickerService = filePickerService;
        }

        public IPlaceService PlaceService { get; }
        public IFilePickerService FilePickerService { get; }

        override public string Title => (Item?.IsNew ?? true) ? "New Place" : TitleEdit;
        public string TitleNew => Item?.Customer == null ? "New Place" : $"New Place, {Item?.Customer?.FullName}";
        public string TitleEdit => Item == null ? "Place" : $"{Item.FullAddress}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public bool CanEditCustomer => Item?.CustomerID <= 0;

        public ICommand CustomerSelectedCommand => new RelayCommand<CustomerModel>(CustomerSelected);
        private void CustomerSelected(CustomerModel customer)
        {
            EditableItem.CustomerID = customer.CustomerID;
            EditableItem.Customer = customer;

            EditableItem.NotifyChanges();
        }

        public PlaceDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(PlaceDetailsArgs args)
        {
            ViewModelArgs = args ?? PlaceDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = new PlaceModel();
                Item = await PlaceService.CreateNewPlaceAsync(ViewModelArgs.CustomerID);
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await PlaceService.GetPlaceAsync(ViewModelArgs.PlaceID);
                    Item = item ?? new PlaceModel { PlaceID = ViewModelArgs.PlaceID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    LogException("Place", "Load", ex);
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.CustomerID = Item?.CustomerID ?? 0;
            ViewModelArgs.PlaceID = Item?.PlaceID ?? 0;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<PlaceDetailsViewModel, PlaceModel>(this, OnDetailsMessage);
            MessageService.Subscribe<PlaceListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public PlaceDetailsArgs CreateArgs()
        {
            return new PlaceDetailsArgs
            {
                CustomerID = Item?.CustomerID ?? 0,
                PlaceID = Item?.PlaceID ?? 0
            };
        }

        //private object _newPictureSource = null;
        //public object NewPictureSource
        //{
        //    get => _newPictureSource;
        //    set => Set(ref _newPictureSource, value);
        //}

        //public override void BeginEdit()
        //{
        //    NewPictureSource = null;
        //    base.BeginEdit();
        //}

        //public ICommand EditPictureCommand => new RelayCommand(OnEditPicture);
        //private async void OnEditPicture()
        //{
        //    NewPictureSource = null;
        //    var result = await FilePickerService.OpenImagePickerAsync();
        //    if (result != null)
        //    {
        //        EditableItem.Picture = result.ImageBytes;
        //        EditableItem.PictureSource = result.ImageSource;
        //        EditableItem.Thumbnail = result.ImageBytes;
        //        EditableItem.ThumbnailSource = result.ImageSource;
        //        NewPictureSource = result.ImageSource;
        //    }
        //    else
        //    {
        //        NewPictureSource = null;
        //    }
        //}

        protected override async Task<bool> SaveItemAsync(PlaceModel model)
        {
            try
            {
                StartStatusMessage("Saving place...");
                await Task.Delay(100);
                await PlaceService.UpdatePlaceAsync(model);
                EndStatusMessage("Place saved");
                LogInformation("Place", "Save", "Place saved successfully", $"Place {model.PlaceID} '{model.FullAddress}' was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Place: {ex.Message}");
                LogException("Place", "Save", ex);
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(PlaceModel model)
        {
            try
            {
                StartStatusMessage("Deleting place...");
                await Task.Delay(100);
                await PlaceService.DeletePlaceAsync(model);
                EndStatusMessage("Place deleted");
                LogWarning("Place", "Delete", "Place deleted", $"Place {model.PlaceID} '{model.FullAddress}' was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Place: {ex.Message}");
                LogException("Place", "Delete", ex);
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current place?", "Ok", "Cancel");
        }

        override protected IEnumerable<IValidationConstraint<PlaceModel>> GetValidationConstraints(PlaceModel model)
        {
            yield return new RequiredConstraint<PlaceModel>("Customer ID", m => m.CustomerID);
            yield return new RequiredConstraint<PlaceModel>("Address", m => m.Address);
            yield return new RequiredConstraint<PlaceModel>("City", m => m.City);
            yield return new RequiredConstraint<PlaceModel>("Region", m => m.Region);
            yield return new RequiredConstraint<PlaceModel>("Postal Code", m => m.PostalCode);
            yield return new RequiredConstraint<PlaceModel>("Country", m => m.CountryCode);
        }

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(PlaceDetailsViewModel sender, string message, PlaceModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.PlaceID == current?.PlaceID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await ContextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await PlaceService.GetPlaceAsync(current.PlaceID);
                                    item = item ?? new PlaceModel { PlaceID = current.PlaceID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    NotifyPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        StatusMessage("WARNING: This place has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogException("Place", "Handle Changes", ex);
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

        private async void OnListMessage(PlaceListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<PlaceModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.PlaceID == current.PlaceID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await PlaceService.GetPlaceAsync(current.PlaceID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException("Place", "Handle Ranges Deleted", ex);
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
                StatusMessage("WARNING: This place has been deleted externally");
            });
        }
    }
}
