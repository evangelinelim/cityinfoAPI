using CityInfo.API.Contexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public class CityRepository : ICityInfoRepository
    {
        public CityInfoContext _context;

        public CityRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddPointOfInterest(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(pointOfInterest);
        }
        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
        public bool CityExists(int cityId)
        {
            return _context.Cities.Any(c => c.Id == cityId);
        }

        public IEnumerable<City> GetCities()
        {
            return _context.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int cityId, bool includePoI)
        {
            if (includePoI)
            {

                return _context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefault();
            }
            return _context.Cities.Where(c => c.Id == cityId).FirstOrDefault();

        }


        public PointOfInterest GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            return _context.PointsOfInterest.Where(c => c.CityId == cityId && c.Id == pointOfInterestId).FirstOrDefault();
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterests()
        {
            return _context.PointsOfInterest.OrderBy(p => p.Name).ToList();
        }

        public void UpdatePointOfInterest(int cityId, PointOfInterest pointOfInterest)
        {
        }

        public void RemovePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }
    }
}
