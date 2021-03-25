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
using System.Collections.Generic;
using System.Threading.Tasks;

using Nailpod.Data;
using Nailpod.Models;
using Nailpod.Services;

namespace Nailpod.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        public DashboardViewModel(ICustomerService customerService, IOrderService orderService, IProductService productService, ICommonServices commonServices) : base(commonServices)
        {
            CustomerService = customerService;
            OrderService = orderService;
            ProductService = productService;
        }

        public DashboardViewModel(ICustomerService customerService, IOrderService orderService, IProductService productService, IPlaceService placeService, ICommonServices commonServices) : base(commonServices)
        {
            CustomerService = customerService;
            OrderService = orderService;
            ProductService = productService;
            PlaceService = placeService;
        }

        public ICustomerService CustomerService { get; }
        public IOrderService OrderService { get; }
        public IProductService ProductService { get; }
        public IPlaceService PlaceService { get; }

        private IList<CustomerModel> _customers = null;
        public IList<CustomerModel> Customers
        {
            get => _customers;
            set => Set(ref _customers, value);
        }

        private IList<ProductModel> _products = null;
        public IList<ProductModel> Products
        {
            get => _products;
            set => Set(ref _products, value);
        }

        private IList<OrderModel> _orders = null;
        public IList<OrderModel> Orders
        {
            get => _orders;
            set => Set(ref _orders, value);
        }

        private IList<PlaceModel> _places = null;
        public IList<PlaceModel> Places
        {
            get => _places;
            set => Set(ref _places, value);
        }

        public async Task LoadAsync()
        {
            StartStatusMessage("Loading dashboard...");
            await LoadCustomersAsync();
            await LoadPlacesAsync();
            await LoadOrdersAsync();
            await LoadProductsAsync();
            EndStatusMessage("Dashboard loaded");
        }
        public void Unload()
        {
            Customers = null;
            Products = null;
            Orders = null;
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var request = new DataRequest<Customer>
                {
                    OrderByDesc = r => r.reg_dt
                };
                Customers = await CustomerService.GetCustomersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                LogException("Dashboard", "Load Customers", ex);
            }
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                var request = new DataRequest<Order>
                {
                    OrderByDesc = r => r.order_dt
                };
                Orders = await OrderService.GetOrdersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                LogException("Dashboard", "Load Orders", ex);
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var request = new DataRequest<Product>
                {
                    OrderByDesc = r => r.reg_dt
                };
                Products = await ProductService.GetProductsAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                LogException("Dashboard", "Load Products", ex);
            }
        }

        private async Task LoadPlacesAsync()
        {
            try
            {
                var request = new DataRequest<Place>
                {
                    OrderByDesc = r => r.reg_dt
                };
                Places = await PlaceService.GetPlacesAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                LogException("Dashboard", "Load Places", ex);
            }
        }

        public void ItemSelected(string item)
        {
            switch (item)
            {
                case "Customers":
                    NavigationService.Navigate<CustomersViewModel>(new CustomerListArgs { OrderByDesc = r => r.reg_dt });
                    break;
                case "Orders":
                    NavigationService.Navigate<OrdersViewModel>(new OrderListArgs { OrderByDesc = r => r.order_dt });
                    break;
                case "Products":
                    NavigationService.Navigate<ProductsViewModel>(new ProductListArgs { OrderByDesc = r => r.list_price });
                    break;
                case "Places":
                    NavigationService.Navigate<PlacesViewModel>(new PlaceListArgs { OrderByDesc = r => r.reg_dt });
                    break;
                default:
                    break;
            }
        }
    }
}
