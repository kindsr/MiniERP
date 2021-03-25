
using System;
using System.Threading.Tasks;

using Nailpod.Models;
using Nailpod.Services;

namespace Nailpod.ViewModels
{
    public class MachinesViewModel : ViewModelBase
    {
        public MachinesViewModel(IMachineService machineService, IOrderService orderService, IFilePickerService filePickerService, ICommonServices commonServices) : base(commonServices)
        {
            MachineService = machineService;

            MachineList = new MachineListViewModel(MachineService, commonServices);
            MachineDetails = new MachineDetailsViewModel(MachineService, filePickerService, commonServices);
        }

        public IMachineService MachineService { get; }

        public MachineListViewModel MachineList { get; set; }
        public MachineDetailsViewModel MachineDetails { get; set; }

        public async Task LoadAsync(MachineListArgs args)
        {
            await MachineList.LoadAsync(args);
        }
        public void Unload()
        {
            MachineDetails.CancelEdit();
            MachineList.Unload();
        }

        public void Subscribe()
        {
            MessageService.Subscribe<MachineListViewModel>(this, OnMessage);
            MachineList.Subscribe();
            MachineDetails.Subscribe();
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            MachineList.Unsubscribe();
            MachineDetails.Unsubscribe();
        }

        private async void OnMessage(MachineListViewModel viewModel, string message, object args)
        {
            if (viewModel == MachineList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async void OnItemSelected()
        {
            if (MachineDetails.IsEditMode)
            {
                StatusReady();
                MachineDetails.CancelEdit();
            }
            var selected = MachineList.SelectedItem;
            if (!MachineList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                }
            }
            MachineDetails.Item = selected;
        }

        private async Task PopulateDetails(MachineModel selected)
        {
            try
            {
                var model = await MachineService.GetMachineAsync(selected.MachineId);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                LogException("Products", "Load Details", ex);
            }
        }
    }
}