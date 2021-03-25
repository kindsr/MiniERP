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
    public class OrderService : IOrderService
    {
        public OrderService(IDataServiceFactory dataServiceFactory, ILogService logService)
        {
            DataServiceFactory = dataServiceFactory;
            LogService = logService;
        }

        public IDataServiceFactory DataServiceFactory { get; }
        public ILogService LogService { get; }

        public async Task<OrderModel> GetOrderAsync(long id)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetOrderAsync(dataService, id);
            }
        }
        static private async Task<OrderModel> GetOrderAsync(IDataService dataService, long id)
        {
            var item = await dataService.GetOrderAsync(id);
            if (item != null)
            {
                return await CreateOrderModelAsync(item, includeAllFields: true);
            }
            return null;
        }

        public async Task<IList<OrderModel>> GetOrdersAsync(DataRequest<Order> request)
        {
            var collection = new OrderCollection(this, LogService);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<OrderModel>> GetOrdersAsync(int skip, int take, DataRequest<Order> request)
        {
            var models = new List<OrderModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetOrdersAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreateOrderModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetOrdersCountAsync(request);
            }
        }

        public async Task<OrderModel> CreateNewOrderAsync(long placeID)
        {
            var model = new OrderModel
            {
                PlaceID = placeID,
                OrderDate = DateTime.UtcNow,
                Status = 0
            };
            if (placeID > 0)
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var parent = await dataService.GetPlaceAsync(placeID);
                    if (parent != null)
                    {
                        model.PlaceID = placeID;
                        model.ShipAddress = parent.addr;
                        model.ShipCity = parent.city;
                        model.ShipRegion = parent.region;
                        model.ShipCountryCode = parent.country_cd;
                        model.ShipPostalCode = parent.postalcode;
                        model.Place = await PlaceService.CreatePlaceModelAsync(parent, includeAllFields: true);
                    }
                }
            }
            return model;
        }

        public async Task<int> UpdateOrderAsync(OrderModel model)
        {
            long id = model.OrderID;
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var order = id > 0 ? await dataService.GetOrderAsync(model.OrderID) : new Order();
                if (order != null)
                {
                    UpdateOrderFromModel(order, model);
                    await dataService.UpdateOrderAsync(order);
                    model.Merge(await GetOrderAsync(dataService, order.order_id));
                }
                return 0;
            }
        }

        public async Task<int> DeleteOrderAsync(OrderModel model)
        {
            var order = new Order { order_id = model.OrderID };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeleteOrdersAsync(order);
            }
        }

        public async Task<int> DeleteOrderRangeAsync(int index, int length, DataRequest<Order> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetOrderKeysAsync(index, length, request);
                return await dataService.DeleteOrdersAsync(items.ToArray());
            }
        }

        static public async Task<OrderModel> CreateOrderModelAsync(Order source, bool includeAllFields)
        {
            var model = new OrderModel()
            {
                OrderID = source.order_id,
                PlaceID = source.place_id,
                OrderDate = source.order_dt,
                ShippedDate = source.shipped_dt,
                DeliveredDate = source.delivered_dt,
                Status = source.order_status,
                PaymentType = source.payment_type,
                TrackingNumber = source.tracking_no,
                ShipVia = source.shipper_id,
                ShipAddress = source.shipped_addr,
                ShipCity = source.shipped_city,
                ShipRegion = source.shipped_region,
                ShipCountryCode = source.shipped_country_cd,
                ShipPostalCode = source.shipped_postalcode,
                ShipPhone = source.shipped_tel_no,
            };
            if (source.Place != null)
            {
                model.Place = await PlaceService.CreatePlaceModelAsync(source.Place, includeAllFields);
            }
            return model;
        }

        private void UpdateOrderFromModel(Order target, OrderModel source)
        {
            target.place_id = source.PlaceID;
            target.order_dt = source.OrderDate;
            target.shipped_dt = source.ShippedDate;
            target.delivered_dt = source.DeliveredDate;
            target.order_status = source.Status;
            target.payment_type = source.PaymentType;
            target.tracking_no = source.TrackingNumber;
            target.shipper_id = source.ShipVia;
            target.shipped_addr = source.ShipAddress;
            target.shipped_city = source.ShipCity;
            target.shipped_region = source.ShipRegion;
            target.shipped_country_cd = source.ShipCountryCode;
            target.shipped_postalcode = source.ShipPostalCode;
            target.shipped_tel_no = source.ShipPhone;
        }
    }
}
