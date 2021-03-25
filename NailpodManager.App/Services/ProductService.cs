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
    public class ProductService : IProductService
    {
        public ProductService(IDataServiceFactory dataServiceFactory, ILogService logService)
        {
            DataServiceFactory = dataServiceFactory;
            LogService = logService;
        }

        public IDataServiceFactory DataServiceFactory { get; }
        public ILogService LogService { get; }

        public async Task<ProductModel> GetProductAsync(string id)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetProductAsync(dataService, id);
            }
        }
        static private async Task<ProductModel> GetProductAsync(IDataService dataService, string id)
        {
            var item = await dataService.GetProductAsync(id);
            if (item != null)
            {
                return await CreateProductModelAsync(item, includeAllFields: true);
            }
            return null;
        }

        public async Task<IList<ProductModel>> GetProductsAsync(DataRequest<Product> request)
        {
            var collection = new ProductCollection(this, LogService);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<ProductModel>> GetProductsAsync(int skip, int take, DataRequest<Product> request)
        {
            var models = new List<ProductModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetProductsAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreateProductModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetProductsCountAsync(DataRequest<Product> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetProductsCountAsync(request);
            }
        }

        public async Task<int> UpdateProductAsync(ProductModel model)
        {
            string id = model.ProductID;
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var product = !String.IsNullOrEmpty(id) ? await dataService.GetProductAsync(model.ProductID) : new Product();
                if (product != null)
                {
                    UpdateProductFromModel(product, model);
                    await dataService.UpdateProductAsync(product);
                    model.Merge(await GetProductAsync(dataService, product.product_id));
                }
                return 0;
            }
        }

        public async Task<int> DeleteProductAsync(ProductModel model)
        {
            var product = new Product { product_id = model.ProductID };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeleteProductsAsync(product);
            }
        }

        public async Task<int> DeleteProductRangeAsync(int index, int length, DataRequest<Product> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetProductKeysAsync(index, length, request);
                return await dataService.DeleteProductsAsync(items.ToArray());
            }
        }

        static public async Task<ProductModel> CreateProductModelAsync(Product source, bool includeAllFields)
        {
            var model = new ProductModel()
            {
                ProductID = source.product_id,
                CategoryID = source.category_id,
                Name = source.name,
                Description = source.desc,
                Size = source.size,
                Color = source.color,
                ListPrice = source.list_price,
                DealerPrice = source.dealer_price,
                TaxType = source.tax_type,
                Discount = source.discount,
                DiscountStartDate = source.discount_begin_dt,
                DiscountEndDate = source.discount_end_dt,
                StockUnits = source.stock_unit,
                SafetyStockLevel = source.safety_stock_level,
                CreatedOn = source.reg_dt,
                LastModifiedOn = source.upd_dt,
                Thumbnail = source.thumbnail,
                ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.thumbnail)
            };

            if (includeAllFields)
            {
                model.Picture = source.picture;
                model.PictureSource = await BitmapTools.LoadBitmapAsync(source.picture);
            }
            return model;
        }

        private void UpdateProductFromModel(Product target, ProductModel source)
        {
            target.category_id = source.CategoryID;
            target.name = source.Name;
            target.desc = source.Description;
            target.size = source.Size;
            target.color = source.Color;
            target.list_price = source.ListPrice;
            target.dealer_price = source.DealerPrice;
            target.tax_type = source.TaxType;
            target.discount = source.Discount;
            target.discount_begin_dt = source.DiscountStartDate;
            target.discount_end_dt = source.DiscountEndDate;
            target.stock_unit = source.StockUnits;
            target.safety_stock_level = source.SafetyStockLevel;
            target.reg_dt = source.CreatedOn;
            target.upd_dt = source.LastModifiedOn;
            target.picture = source.Picture;
            target.thumbnail = source.Thumbnail;
        }
    }
}
