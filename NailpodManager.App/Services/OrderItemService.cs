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
    public class OrderItemService : IOrderItemService
    {
        public OrderItemService(IDataServiceFactory dataServiceFactory)
        {
            DataServiceFactory = dataServiceFactory;
        }

        public IDataServiceFactory DataServiceFactory { get; }

        public async Task<OrderItemModel> GetOrderItemAsync(long orderID, int lineID)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetOrderItemAsync(dataService, orderID, lineID);
            }
        }
        static private async Task<OrderItemModel> GetOrderItemAsync(IDataService dataService, long orderID, int lineID)
        {
            var item = await dataService.GetOrderItemAsync(orderID, lineID);
            if (item != null)
            {
                return await CreateOrderItemModelAsync(item, includeAllFields: true);
            }
            return null;
        }

        public Task<IList<OrderItemModel>> GetOrderItemsAsync(DataRequest<OrderItem> request)
        {
            // OrderItems are not virtualized
            return GetOrderItemsAsync(0, 100, request);
        }

        public async Task<IList<OrderItemModel>> GetOrderItemsAsync(int skip, int take, DataRequest<OrderItem> request)
        {
            var models = new List<OrderItemModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetOrderItemsAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreateOrderItemModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetOrderItemsCountAsync(DataRequest<OrderItem> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetOrderItemsCountAsync(request);
            }
        }

        public async Task<int> UpdateOrderItemAsync(OrderItemModel model)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var orderItem = model.OrderLine > 0 ? await dataService.GetOrderItemAsync(model.OrderID, model.OrderLine) : new OrderItem();
                if (orderItem != null)
                {
                    UpdateOrderItemFromModel(orderItem, model);
                    await dataService.UpdateOrderItemAsync(orderItem);
                    model.Merge(await GetOrderItemAsync(dataService, orderItem.order_id, orderItem.seq));
                }
                return 0;
            }
        }

        public async Task<int> DeleteOrderItemAsync(OrderItemModel model)
        {
            var orderItem = new OrderItem { order_id = model.OrderID, seq = model.OrderLine };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeleteOrderItemsAsync(orderItem);
            }
        }

        public async Task<int> DeleteOrderItemRangeAsync(int index, int length, DataRequest<OrderItem> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetOrderItemKeysAsync(index, length, request);
                return await dataService.DeleteOrderItemsAsync(items.ToArray());
            }
        }

        static public async Task<OrderItemModel> CreateOrderItemModelAsync(OrderItem source, bool includeAllFields)
        {
            var model = new OrderItemModel()
            {
                OrderID = source.order_id,
                OrderLine = source.seq,
                ProductID = source.product_id,
                Quantity = source.qty,
                UnitPrice = source.unitprice,
                Discount = source.discount,
                TaxType = source.tax_type,
                Product = await ProductService.CreateProductModelAsync(source.Product, includeAllFields)
            };
            return model;
        }

        private void UpdateOrderItemFromModel(OrderItem target, OrderItemModel source)
        {
            target.order_id = source.OrderID;
            target.seq = source.OrderLine;
            target.product_id = source.ProductID;
            target.qty = source.Quantity;
            target.unitprice = source.UnitPrice;
            target.discount = source.Discount;
            target.tax_type = source.TaxType;
        }
    }
}
