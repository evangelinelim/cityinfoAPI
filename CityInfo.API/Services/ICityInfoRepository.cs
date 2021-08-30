using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        IEnumerable<City> GetCities();

        City GetCity(int cityId, bool includePoI);
        IEnumerable<PointOfInterest> GetPointsOfInterests();

        PointOfInterest GetPointOfInterest(int cityId, int pointOfInterestId);

        bool CityExists(int cityId);

        void AddPointOfInterest(int cityId, PointOfInterest pointOfInterest);
        bool Save();

        void UpdatePointOfInterest(int cityId, PointOfInterest pointOfInterest);
        void RemovePointOfInterest(PointOfInterest pointOfInterest);
    }
}
