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
    #region MachineDetailsArgs
    public class MachineDetailsArgs
    {
        static public MachineDetailsArgs CreateDefault() => new MachineDetailsArgs();

        public long MachineID { get; set; }

        public bool IsNew => MachineID <= 0;
    }
    #endregion

    public class MachineDetailsViewModel : GenericDetailsViewModel<MachineModel>
    {
        public MachineDetailsViewModel(IMachineService machineService, IFilePickerService filePickerService, ICommonServices commonServices) : base(commonServices)
        {
            MachineService = machineService;
            FilePickerService = filePickerService;
        }

        public IMachineService MachineService { get; }
        public IFilePickerService FilePickerService { get; }

        override public string Title => (Item?.IsNew ?? true) ? "New Machine" : TitleEdit;
        public string TitleEdit => Item == null ? "Machine" : $"{Item.Name}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public MachineDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(MachineDetailsArgs args)
        {
            ViewModelArgs = args ?? MachineDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = new MachineModel();
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await MachineService.GetMachineAsync(ViewModelArgs.MachineID);
                    Item = item ?? new MachineModel { MachineID = ViewModelArgs.MachineID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    LogException("Machine", "Load", ex);
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.MachineID = Item?.MachineID;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<MachineDetailsViewModel, ProductModel>(this, OnDetailsMessage);
            MessageService.Subscribe<MachineListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public MachineDetailsArgs CreateArgs()
        {
            return new MachineDetailsArgs
            {
                MachineID = Item?.MachineID
            };
        }

        private object _newPictureSource = null;
        public object NewPictureSource
        {
            get => _newPictureSource;
            set => Set(ref _newPictureSource, value);
        }

        public override void BeginEdit()
        {
            NewPictureSource = null;
            base.BeginEdit();
        }

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

        //protected override async Task<bool> SaveItemAsync(MachineModel model)
        //{
        //    try
        //    {
        //        StartStatusMessage("Saving machine...");
        //        await Task.Delay(100);
        //        await MachineService.UpdateMachineAsync(model);
        //        EndStatusMessage("Machine saved");
        //        LogInformation("Machine", "Save", "Machine saved successfully", $"Product {model.MachineID} was saved successfully.");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        StatusError($"Error saving Machine: {ex.Message}");
        //        LogException("Machine", "Save", ex);
        //        return false;
        //    }
        //}

        //protected override async Task<bool> DeleteItemAsync(ProductModel model)
        //{
        //    try
        //    {
        //        StartStatusMessage("Deleting product...");
        //        await Task.Delay(100);
        //        await ProductService.DeleteProductAsync(model);
        //        EndStatusMessage("Product deleted");
        //        LogWarning("Product", "Delete", "Product deleted", $"Product {model.ProductID} '{model.Name}' was deleted.");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        StatusError($"Error deleting Product: {ex.Message}");
        //        LogException("Product", "Delete", ex);
        //        return false;
        //    }
        //}

        //protected override async Task<bool> ConfirmDeleteAsync()
        //{
        //    return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current product?", "Ok", "Cancel");
        //}

        //override protected IEnumerable<IValidationConstraint<ProductModel>> GetValidationConstraints(ProductModel model)
        //{
        //    yield return new RequiredConstraint<ProductModel>("Name", m => m.Name);
        //    yield return new RequiredGreaterThanZeroConstraint<ProductModel>("Category", m => m.CategoryID);
        //}

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(MachineDetailsViewModel sender, string message, MachineModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.MachineID == current?.MachineID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await ContextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await MachineService.GetMachineAsync(current.MachineID);
                                    item = item ?? new MachineModel { MachineID = current.MachineID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    NotifyPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        StatusMessage("WARNING: This machine has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogException("Machine", "Handle Changes", ex);
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

        private async void OnListMessage(MachineListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<MachineModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.MachineID == current.MachineID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await MachineService.GetMachineAsync(current.MachineID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException("Machine", "Handle Ranges Deleted", ex);
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
                StatusMessage("WARNING: This machine has been deleted externally");
            });
        }
    }
}
