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

using Microsoft.EntityFrameworkCore;

namespace Nailpod.Data.Services
{
    partial class DataServiceBase
    {
        public async Task<Product> GetProductAsync(string id)
        {
            return await _dataSource.Products.Where(r => r.product_id == id).FirstOrDefaultAsync();
        }

        public async Task<IList<Product>> GetProductsAsync(int skip, int take, DataRequest<Product> request)
        {
            IQueryable<Product> items = GetProducts(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<IList<Product>> GetProductKeysAsync(int skip, int take, DataRequest<Product> request)
        {
            IQueryable<Product> items = GetProducts(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Product
                {
                    product_id = r.product_id,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        private IQueryable<Product> GetProducts(DataRequest<Product> request)
        {
            IQueryable<Product> items = _dataSource.Products;

            // Query
            //if (!String.IsNullOrEmpty(request.Query))
            //{
            //    items = items.Where(r => r.search_terms.Contains(request.Query.ToLower()));
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

        public async Task<int> GetProductsCountAsync(DataRequest<Product> request)
        {
            IQueryable<Product> items = _dataSource.Products;

            // Query
            //if (!String.IsNullOrEmpty(request.Query))
            //{
            //    items = items.Where(r => r.search_terms.Contains(request.Query.ToLower()));
            //}

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            return await items.CountAsync();
        }

        public async Task<int> UpdateProductAsync(Product product)
        {
            if (!String.IsNullOrEmpty(product.product_id))
            {
                _dataSource.Entry(product).State = EntityState.Modified;
            }
            else
            {
                product.product_id = UIDGenerator.Next(6).ToString();
                product.reg_dt = DateTime.UtcNow;
                _dataSource.Entry(product).State = EntityState.Added;
            }
            product.upd_dt = DateTime.UtcNow;
            //product.SearchTerms = product.BuildSearchTerms();
            return await _dataSource.SaveChangesAsync();
        }

        public async Task<int> DeleteProductsAsync(params Product[] products)
        {
            _dataSource.Products.RemoveRange(products);
            return await _dataSource.SaveChangesAsync();
        }
    }
}
