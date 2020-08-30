using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class NationalParkRepository : INationalParkRepository
    {
        private readonly ApplicationDbContext db;

        public NationalParkRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public bool CreateNationalPark(NationalPark nationalPark)
        {
            db.NationalParks.Add(nationalPark);
            return Save();
        }

        public bool DeleteNationalPark(NationalPark nationalPark)
        {
            db.NationalParks.Remove(nationalPark);
            return Save();
        }

        public NationalPark GetNationalPark(int id)
        {
            return db.NationalParks.FirstOrDefault(n => n.Id == id);
        }

        public ICollection<NationalPark> GetNationalParks()
        {
            return db.NationalParks.OrderBy(n => n.Name).ToList();
        }

        public bool NationalParkExists(string name)
        {
            bool value = db.NationalParks.Any(a => a.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool NationalParkExists(int id)
        {
            bool value = db.NationalParks.Any(a => a.Id == id);
            return value;
        }

        public bool Save()
        {
           return db.SaveChanges() >= 0;
        }

        public bool UpdateNationalPark(NationalPark nationalPark)
        {
            db.NationalParks.Update(nationalPark);

            return Save();
        }
    }
}
