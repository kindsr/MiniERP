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
        public async Task<Customer> GetCustomerAsync(long id)
        {
            return await _dataSource.Customers.Where(r => r.customer_id == id).FirstOrDefaultAsync();
        }

        public async Task<IList<Customer>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request)
        {
            IQueryable<Customer> items = GetCustomers(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Customer
                {
                    customer_id = r.customer_id,
                    firstname = r.firstname,
                    lastname = r.lastname,
                    gender = r.gender,
                    pe_corp_gb = r.pe_corp_gb,
                    identify_no = r.identify_no,
                    phone_no = r.phone_no,
                    email = r.email,
                    country_cd = r.country_cd,
                    birthdate = r.birthdate,
                    remark = r.remark,
                    del_yn = r.del_yn,
                    reg_dt = r.reg_dt,
                    upd_dt = r.upd_dt,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<IList<Customer>> GetCustomerKeysAsync(int skip, int take, DataRequest<Customer> request)
        {
            IQueryable<Customer> items = GetCustomers(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Customer
                {
                    customer_id = r.customer_id,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        private IQueryable<Customer> GetCustomers(DataRequest<Customer> request)
        {
            IQueryable<Customer> items = _dataSource.Customers;

            // Query
            if (!String.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.searchterms.Contains(request.Query.ToLower()));
            }

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

        public async Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            IQueryable<Customer> items = _dataSource.Customers;

            // Query
            if (!String.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.searchterms.Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            return await items.CountAsync();
        }

        public async Task<int> UpdateCustomerAsync(Customer customer)
        {
            if (customer.customer_id > 0)
            {
                _dataSource.Entry(customer).State = EntityState.Modified;
            }
            else
            {
                customer.customer_id = UIDGenerator.Next();
                customer.reg_dt = DateTime.UtcNow;
                _dataSource.Entry(customer).State = EntityState.Added;
            }
            customer.upd_dt = DateTime.UtcNow;
            customer.searchterms = customer.BuildSearchTerms();
            int res = await _dataSource.SaveChangesAsync();
            return res;
        }

        public async Task<int> DeleteCustomersAsync(params Customer[] customers)
        {
            _dataSource.Customers.RemoveRange(customers);
            return await _dataSource.SaveChangesAsync();
        }
    }
}
