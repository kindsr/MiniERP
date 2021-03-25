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

using Nailpod.Data;
using Nailpod.Data.Services;
using Nailpod.Models;

namespace Nailpod.Services
{
    public class PlaceService : IPlaceService
    {
        public PlaceService(IDataServiceFactory dataServiceFactory, ILogService logService)
        {
            DataServiceFactory = dataServiceFactory;
            LogService = logService;
        }

        public IDataServiceFactory DataServiceFactory { get; }
        public ILogService LogService { get; }

        public async Task<PlaceModel> GetPlaceAsync(long id)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetPlaceAsync(dataService, id);
            }
        }
        static private async Task<PlaceModel> GetPlaceAsync(IDataService dataService, long id)
        {
            var item = await dataService.GetPlaceAsync(id);
            if (item != null)
            {
                return await CreatePlaceModelAsync(item, includeAllFields: true);
            }
            return null;
        }

        public async Task<IList<PlaceModel>> GetPlacesAsync(DataRequest<Place> request)
        {
            var collection = new PlaceCollection(this, LogService);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<PlaceModel>> GetPlacesAsync(int skip, int take, DataRequest<Place> request)
        {
            var models = new List<PlaceModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetPlacesAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreatePlaceModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetPlacesCountAsync(DataRequest<Place> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetPlacesCountAsync(request);
            }
        }

        public async Task<PlaceModel> CreateNewPlaceAsync(long customerID)
        {
            var model = new PlaceModel
            {
                CustomerID = customerID,
                CreatedOn = DateTime.UtcNow,
                //Status = 0
            };
            if (customerID > 0)
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var parent = await dataService.GetCustomerAsync(customerID);
                    if (parent != null)
                    {
                        model.CustomerID = customerID;
                        model.Customer = await CustomerService.CreateCustomerModelAsync(parent, includeAllFields: true);
                    }
                }
            }
            return model;
        }

        public async Task<int> UpdatePlaceAsync(PlaceModel model)
        {
            long id = model.PlaceID;
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var place = id > 0 ? await dataService.GetPlaceAsync(model.PlaceID) : new Place();
                if (place != null)
                {
                    UpdatePlaceFromModel(place, model);
                    await dataService.UpdatePlaceAsync(place);
                    model.Merge(await GetPlaceAsync(dataService, place.place_id));
                }
                return 0;
            }
        }

        public async Task<int> DeletePlaceAsync(PlaceModel model)
        {
            var place = new Place { place_id = model.PlaceID };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeletePlacesAsync(place);
            }
        }

        public async Task<int> DeletePlaceRangeAsync(int index, int length, DataRequest<Place> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetPlaceKeysAsync(index, length, request);
                return await dataService.DeletePlacesAsync(items.ToArray());
            }
        }

        static public async Task<PlaceModel> CreatePlaceModelAsync(Place source, bool includeAllFields = false)
        {
            var model = new PlaceModel()
            {
                PlaceID = source.place_id,
                CustomerID = source.customer_id,
                Address = source.addr,
                Address2 = source.addr2,
                City = source.city,
                Region = source.region,
                CountryCode = source.country_cd,
                PostalCode = source.postalcode,
                Phone = source.place_tel_no,
                InstallDate = source.install_dt,
                DelYn = source.del_yn,
                CreatedOn = source.reg_dt,
                LastModifiedOn = source.upd_dt
            };
            if (source.Customer != null)
            {
                model.Customer = await CustomerService.CreateCustomerModelAsync(source.Customer, includeAllFields);
            }
            return model;
        }

        private void UpdatePlaceFromModel(Place target, PlaceModel source)
        {
            target.place_id = source.PlaceID;
            target.customer_id = source.CustomerID;
            target.addr = source.Address;
            target.addr2 = source.Address2;
            target.city = source.City;
            target.region = source.Region;
            target.country_cd = source.CountryCode;
            target.postalcode = source.PostalCode;
            target.place_tel_no = source.Phone;
            target.install_dt = source.InstallDate;
            target.del_yn = source.DelYn;
            target.reg_dt = source.CreatedOn;
            target.upd_dt = source.LastModifiedOn;
        }
    }
}
