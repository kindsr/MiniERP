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
using System.Windows.Input;

using Nailpod.Services;

namespace Nailpod.ViewModels
{
    #region SettingsArgs
    public class SettingsArgs
    {
        static public SettingsArgs CreateDefault() => new SettingsArgs();
    }
    #endregion

    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(ISettingsService settingsService, ICommonServices commonServices) : base(commonServices)
        {
            SettingsService = settingsService;
        }

        public ISettingsService SettingsService { get; }

        public string Version => $"v{SettingsService.Version}";

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        private bool _isLocalProvider;
        public bool IsLocalProvider
        {
            get { return _isLocalProvider; }
            set { if (Set(ref _isLocalProvider, value)) UpdateProvider(); }
        }

        private bool _isSqlProvider;
        public bool IsSqlProvider
        {
            get => _isSqlProvider;
            set => Set(ref _isSqlProvider, value);
        }

        private bool _isMariaProvider;
        public bool IsMariaProvider
        {
            get => _isMariaProvider;
            set => Set(ref _isMariaProvider, value);
        }

        private bool _isHttpProvider;
        public bool IsHttpProvider
        {
            get { return _isHttpProvider; }
            set { if (Set(ref _isHttpProvider, value)) UpdateProvider(); }
        }

        private string _sqlConnectionString = null;
        public string SqlConnectionString
        {
            get => _sqlConnectionString;
            set => Set(ref _sqlConnectionString, value);
        }

        private string _mariaConnectionString = null;
        public string MariaConnectionString
        {
            get => _mariaConnectionString;
            set => Set(ref _mariaConnectionString, value);
        }

        public bool IsRandomErrorsEnabled
        {
            get { return SettingsService.IsRandomErrorsEnabled; }
            set { SettingsService.IsRandomErrorsEnabled = value; }
        }

        public ICommand ResetLocalDataCommand => new RelayCommand(OnResetLocalData);
        public ICommand ValidateSqlConnectionCommand => new RelayCommand(OnValidateSqlConnection);
        public ICommand ValidateMariaConnectionCommand => new RelayCommand(OnValidateMariaConnection);
        public ICommand CreateDatabaseCommand => new RelayCommand(OnCreateDatabase);
        public ICommand SaveChangesCommand => new RelayCommand(OnSaveChanges);

        public SettingsArgs ViewModelArgs { get; private set; }

        public Task LoadAsync(SettingsArgs args)
        {
            ViewModelArgs = args ?? SettingsArgs.CreateDefault();

            StatusReady();

            IsLocalProvider = SettingsService.DataProvider == DataProviderType.SQLite;

            SqlConnectionString = SettingsService.SQLServerConnectionString;
            IsSqlProvider = SettingsService.DataProvider == DataProviderType.SQLServer;

            MariaConnectionString = SettingsService.MariaConnectionString;
            IsMariaProvider = SettingsService.DataProvider == DataProviderType.Maria;

            IsHttpProvider = SettingsService.DataProvider == DataProviderType.WebAPI;

            return Task.CompletedTask;
        }

        private void UpdateProvider()
        {
            if (IsLocalProvider && !IsSqlProvider && !IsMariaProvider && !IsHttpProvider)
            {
                SettingsService.DataProvider = DataProviderType.SQLite;
            }
            else if (!IsLocalProvider && !IsSqlProvider && !IsMariaProvider && IsHttpProvider)
            {
                SettingsService.DataProvider = DataProviderType.WebAPI;
            }

        }

        private async void OnResetLocalData()
        {
            IsBusy = true;
            StatusMessage("Waiting database reset...");
            var result = await SettingsService.ResetLocalDataProviderAsync();
            IsBusy = false;
            if (result.IsOk)
            {
                StatusReady();
            }
            else
            {
                StatusMessage(result.Message);
            }
        }

        private async void OnValidateSqlConnection()
        {
            await ValidateSqlConnectionAsync();
        }

        private async void OnValidateMariaConnection()
        {
            await ValidateMariaConnectionAsync();
        }

        private async Task<bool> ValidateSqlConnectionAsync()
        {
            StatusReady();
            IsBusy = true;
            StatusMessage("Validating connection string...");
            var result = await SettingsService.ValidateConnectionAsync(SqlConnectionString, DataProviderType.SQLServer);
            IsBusy = false;
            if (result.IsOk)
            {
                StatusMessage(result.Message);
                return true;
            }
            else
            {
                StatusMessage(result.Message);
                return false;
            }
        }

        private async Task<bool> ValidateMariaConnectionAsync()
        {
            StatusReady();
            IsBusy = true;
            StatusMessage("Validating connection string...");
            var result = await SettingsService.ValidateConnectionAsync(MariaConnectionString, DataProviderType.Maria);
            IsBusy = false;
            if (result.IsOk)
            {
                StatusMessage(result.Message);
                return true;
            }
            else
            {
                StatusMessage(result.Message);
                return false;
            }
        }

        private async void OnCreateDatabase()
        {
            StatusReady();
            DisableAllViews("Waiting for the database to be created...");
            var result = await SettingsService.CreateDabaseAsync(SqlConnectionString);
            EnableOtherViews();
            EnableThisView("");
            await Task.Delay(100);
            if (result.IsOk)
            {
                StatusMessage(result.Message);
            }
            else
            {
                StatusError("Error creating database");
            }
        }

        private async void OnSaveChanges()
        {
            if (IsSqlProvider)
            {
                if (await ValidateSqlConnectionAsync())
                {
                    SettingsService.SQLServerConnectionString = SqlConnectionString;
                    SettingsService.DataProvider = DataProviderType.SQLServer;
                }
            }
            else if (IsMariaProvider)
            {
                if (await ValidateMariaConnectionAsync())
                {
                    SettingsService.MariaConnectionString = MariaConnectionString;
                    SettingsService.DataProvider = DataProviderType.Maria;
                }
            }
            else if (IsHttpProvider)
            {
                SettingsService.DataProvider = DataProviderType.WebAPI;
            }
            else
            {
                SettingsService.DataProvider = DataProviderType.SQLite;
            }
        }
    }
}
