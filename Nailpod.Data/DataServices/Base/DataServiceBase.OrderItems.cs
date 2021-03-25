﻿#region copyright
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

using Microsoft.EntityFrameworkCore;

namespace Nailpod.Data.Services
{
    partial class DataServiceBase
    {
        public async Task<OrderItem> GetOrderItemAsync(long orderID, int orderLine)
        {
            return await _dataSource.OrderItems
                .Where(r => r.order_id == orderID && r.seq == orderLine)
                .Include(r => r.Product)
                .FirstOrDefaultAsync();
        }

        public async Task<IList<OrderItem>> GetOrderItemsAsync(int skip, int take, DataRequest<OrderItem> request)
        {
            IQueryable<OrderItem> items = GetOrderItems(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Include(r => r.Product)
                .AsNoTracking().ToListAsync();

            return records;
        }

        public async Task<IList<OrderItem>> GetOrderItemKeysAsync(int skip, int take, DataRequest<OrderItem> request)
        {
            IQueryable<OrderItem> items = GetOrderItems(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new OrderItem
                {
                    order_id = r.order_id,
                    seq = r.seq
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        private IQueryable<OrderItem> GetOrderItems(DataRequest<OrderItem> request)
        {
            IQueryable<OrderItem> items = _dataSource.OrderItems;

            // Query
            // TODO: Not supported
            //if (!String.IsNullOrEmpty(request.Query))
            //{
            //    items = items.Where(r => r.SearchTerms.Contains(request.Query.ToLower()));
            //}

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            // Order By
            if (request.OrderBy != null)
            {
                items = items.OrderBy(request.OrderBy);
            }
            if (request.OrderByDesc != null)
            {
                items = items.OrderByDescending(request.OrderByDesc);
            }

            return items;
        }

        public async Task<int> GetOrderItemsCountAsync(DataRequest<OrderItem> request)
        {
            IQueryable<OrderItem> items = _dataSource.OrderItems;

            // Query
            // TODO: Not supported
            //if (!String.IsNullOrEmpty(request.Query))
            //{
            //    items = items.Where(r => r.SearchTerms.Contains(request.Query.ToLower()));
            //}

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            return await items.CountAsync();
        }

        public async Task<int> UpdateOrderItemAsync(OrderItem orderItem)
        {
            if (orderItem.seq > 0)
            {
                _dataSource.Entry(orderItem).State = EntityState.Modified;
            }
            else
            {
                orderItem.seq = _dataSource.OrderItems.Where(r => r.order_id == orderItem.order_id).Select(r => r.seq).DefaultIfEmpty(0).Max() + 1;
                // TODO: 
                //orderItem.CreateOn = DateTime.UtcNow;
                _dataSource.Entry(orderItem).State = EntityState.Added;
            }
            // TODO: 
            //orderItem.LastModifiedOn = DateTime.UtcNow;
            //orderItem.SearchTerms = orderItem.BuildSearchTerms();
            return await _dataSource.SaveChangesAsync();
        }

        public async Task<int> DeleteOrderItemsAsync(params OrderItem[] orderItems)
        {
            _dataSource.OrderItems.RemoveRange(orderItems);
            return await _dataSource.SaveChangesAsync();
        }
    }
}
