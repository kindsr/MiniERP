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

namespace Nailpod.Services
{
    public class PlaceCollection : VirtualCollection<PlaceModel>
    {
        private DataRequest<Place> _dataRequest = null;

        public PlaceCollection(IPlaceService placeService, ILogService logService) : base(logService)
        {
            PlaceService = placeService;
        }

        public IPlaceService PlaceService { get; }

        private PlaceModel _defaultItem = PlaceModel.CreateEmpty();
        protected override PlaceModel DefaultItem => _defaultItem;

        public async Task LoadAsync(DataRequest<Place> dataRequest)
        {
            try
            {
                _dataRequest = dataRequest;
                Count = await PlaceService.GetPlacesCountAsync(_dataRequest);
                Ranges[0] = await PlaceService.GetPlacesAsync(0, RangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                Count = 0;
                throw ex;
            }
        }

        protected override async Task<IList<PlaceModel>> FetchDataAsync(int rangeIndex, int rangeSize)
        {
            try
            {
                return await PlaceService.GetPlacesAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                LogException("PlaceCollection", "Fetch", ex);
            }
            return null;
        }
    }
}
