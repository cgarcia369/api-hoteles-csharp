using Hoteles.Contracs;
using Hoteles.Data.Context;
using Hoteles.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hoteles.Repository
{
    public class UnitOfWork : IUnitOFWork
    { 

        private readonly DatabaseContext _context;
        public IRepository<Country> _countries;
        public IRepository<Hotel> _hotels;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        public IRepository<Country> Countries => _countries ??= new Repository<Country>(_context);
        public IRepository<Hotel> Hotels => _hotels ??= new Repository<Hotel>(_context);

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}


