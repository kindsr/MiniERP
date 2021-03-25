using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Nailpod.Data;
using Nailpod.Models;

namespace Nailpod.Services
{
    public interface IPlaceService
    {
        Task<PlaceModel> GetPlaceAsync(long id);
        Task<IList<PlaceModel>> GetPlacesAsync(DataRequest<Place> request);
        Task<IList<PlaceModel>> GetPlacesAsync(int skip, int take, DataRequest<Place> request);
        Task<int> GetPlacesCountAsync(DataRequest<Place> request);

        Task<PlaceModel> CreateNewPlaceAsync(long customerID);
        Task<int> UpdatePlaceAsync(PlaceModel model);

        Task<int> DeletePlaceAsync(PlaceModel model);
        Task<int> DeletePlaceRangeAsync(int index, int length, DataRequest<Place> request);
    }
}
