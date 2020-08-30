using Microsoft.EntityFrameworkCore;
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
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDbContext db;

        public TrailRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public bool CreateTrail(Trail trail)
        {
            db.Trails.Add(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            db.Trails.Remove(trail);
            return Save();
        }

        public Trail GetTrail(int id)
        {
            return db.Trails.Include(n => n.NationalPark).FirstOrDefault(t => t.Id == id);
        }

        public ICollection<Trail> GetTrails()
        {
            return db.Trails.Include(n => n.NationalPark).OrderBy(t => t.Name).ToList();
        }

        public bool TrailExists(string name)
        {
            bool value = db.Trails.Any(a => a.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool TrailExists(int id)
        {
            bool value = db.Trails.Any(t => t.Id == id);
            return value;
        }

        public bool Save()
        {
           return db.SaveChanges() >= 0;
        }

        public bool UpdateTrail(Trail trail)
        {
            db.Trails.Update(trail);

            return Save();
        }

        public ICollection<Trail> GetTrailsInNationalPark(int npId)
        {
            return db.Trails.Include(t => t.NationalPark).Where(n => n.NationalParkId == npId).ToList();
        }
    }
}
