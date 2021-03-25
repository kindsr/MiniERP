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
    public class CustomerService : ICustomerService
    {
        public CustomerService(IDataServiceFactory dataServiceFactory, ILogService logService)
        {
            DataServiceFactory = dataServiceFactory;
            LogService = logService;
        }

        public IDataServiceFactory DataServiceFactory { get; }
        public ILogService LogService { get; }

        public async Task<CustomerModel> GetCustomerAsync(long id)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetCustomerAsync(dataService, id);
            }
        }
        static private async Task<CustomerModel> GetCustomerAsync(IDataService dataService, long id)
        {
            var item = await dataService.GetCustomerAsync(id);
            if (item != null)
            {
                return await CreateCustomerModelAsync(item, includeAllFields: true);
            }
            return null;
        }

        public async Task<IList<CustomerModel>> GetCustomersAsync(DataRequest<Customer> request)
        {
            var collection = new CustomerCollection(this, LogService);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<CustomerModel>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request)
        {
            var models = new List<CustomerModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetCustomersAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreateCustomerModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetCustomersCountAsync(request);
            }
        }

        public async Task<int> UpdateCustomerAsync(CustomerModel model)
        {
            long id = model.CustomerID;
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var customer = id > 0 ? await dataService.GetCustomerAsync(model.CustomerID) : new Customer();
                if (customer != null)
                {
                    UpdateCustomerFromModel(customer, model);
                    await dataService.UpdateCustomerAsync(customer);
                    model.Merge(await GetCustomerAsync(dataService, customer.customer_id));
                }
                return 0;
            }
        }

        public async Task<int> DeleteCustomerAsync(CustomerModel model)
        {
            var customer = new Customer { customer_id = model.CustomerID };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeleteCustomersAsync(customer);
            }
        }

        public async Task<int> DeleteCustomerRangeAsync(int index, int length, DataRequest<Customer> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetCustomerKeysAsync(index, length, request);
                return await dataService.DeleteCustomersAsync(items.ToArray());
            }
        }

        static public async Task<CustomerModel> CreateCustomerModelAsync(Customer source, bool includeAllFields)
        {
            var model = new CustomerModel()
            {
                CustomerID = source.customer_id,
                //Title = source.Title,
                FirstName = source.firstname,
                //MiddleName = source.MiddleName,
                LastName = source.lastname,
                //Suffix = source.Suffix,
                Gender = source.gender,
                EmailAddress = source.email,
                //AddressLine1 = source.AddressLine1,
                //AddressLine2 = source.AddressLine2,
                //City = source.City,
                //Region = source.Region,
                //CountryCode = source.country_cd,
                //PostalCode = source.PostalCode,
                Phone = source.phone_no,
                CreatedOn = source.reg_dt,
                LastModifiedOn = source.upd_dt,
                Thumbnail = source.thumbnail,
                ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.thumbnail)
            };
            if (includeAllFields)
            {
                model.BirthDate = source.birthdate;
                //model.Education = source.Education;
                //model.Occupation = source.Occupation;
                //model.YearlyIncome = source.YearlyIncome;
                //model.MaritalStatus = source.MaritalStatus;
                //model.TotalChildren = source.TotalChildren;
                //model.ChildrenAtHome = source.ChildrenAtHome;
                //model.IsHouseOwner = source.IsHouseOwner;
                //model.NumberCarsOwned = source.NumberCarsOwned;
                model.Picture = source.picture;
                model.PictureSource = await BitmapTools.LoadBitmapAsync(source.picture);
            }
            return model;
        }

        private void UpdateCustomerFromModel(Customer target, CustomerModel source)
        {
            //target.Title = source.Title;
            target.firstname = source.FirstName;
            //target.MiddleName = source.MiddleName;
            target.lastname = source.LastName;
            //target.Suffix = source.Suffix;
            target.gender = source.Gender;
            target.email = source.EmailAddress;
            //target.AddressLine1 = source.AddressLine1;
            //target.AddressLine2 = source.AddressLine2;
            //target.City = source.City;
            //target.Region = source.Region;
            //target.country_cd = source.CountryCode;
            //target.PostalCode = source.PostalCode;
            target.phone_no = source.Phone;
            target.birthdate = source.BirthDate;
            //target.Education = source.Education;
            //target.Occupation = source.Occupation;
            //target.YearlyIncome = source.YearlyIncome;
            //target.MaritalStatus = source.MaritalStatus;
            //target.TotalChildren = source.TotalChildren;
            //target.ChildrenAtHome = source.ChildrenAtHome;
            //target.IsHouseOwner = source.IsHouseOwner;
            //target.NumberCarsOwned = source.NumberCarsOwned;
            target.reg_dt = source.CreatedOn;
            target.upd_dt = source.LastModifiedOn;
            target.picture = source.Picture;
            target.thumbnail = source.Thumbnail;
        }
    }
}
