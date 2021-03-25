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
        public async Task<Place> GetPlaceAsync(long id)
        {
            return await _dataSource.Places.Where(r => r.place_id == id)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync();
        }

        public async Task<IList<Place>> GetPlacesAsync(int skip, int take, DataRequest<Place> request)
        {
            IQueryable<Place> items = GetPlaces(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Place
                {
                    place_id = r.place_id,
                    customer_id = r.customer_id,
                    addr = r.addr,
                    addr2 = r.addr2,
                    city = r.city,
                    region = r.region,
                    country_cd = r.country_cd,
                    postalcode = r.postalcode,
                    place_tel_no = r.place_tel_no,
                    reg_dt = r.reg_dt,
                    upd_dt = r.upd_dt,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<IList<Place>> GetPlaceKeysAsync(int skip, int take, DataRequest<Place> request)
        {
            IQueryable<Place> items = GetPlaces(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Place
                {
                    place_id = r.place_id,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        private IQueryable<Place> GetPlaces(DataRequest<Place> request)
        {
            IQueryable<Place> items = _dataSource.Places;

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

        public async Task<int> GetPlacesCountAsync(DataRequest<Place> request)
        {
            IQueryable<Place> items = _dataSource.Places;

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

        public async Task<int> UpdatePlaceAsync(Place place)
        {
            if (place.place_id > 0)
            {
                _dataSource.Entry(place).State = EntityState.Modified;
            }
            else
            {
                place.place_id = UIDGenerator.Next();
                place.reg_dt = DateTime.UtcNow;
                _dataSource.Entry(place).State = EntityState.Added;
            }
            place.upd_dt = DateTime.UtcNow;
            place.searchterms = place.BuildSearchTerms();
            int res = await _dataSource.SaveChangesAsync();
            return res;
        }

        public async Task<int> DeletePlacesAsync(params Place[] places)
        {
            _dataSource.Places.RemoveRange(places);
            return await _dataSource.SaveChangesAsync();
        }
    }
}
