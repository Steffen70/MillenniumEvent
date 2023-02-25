using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Data.Repositories;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork
    {
        internal readonly DataContext Context;
        internal readonly IMapper Mapper;
        public UnitOfWork(DataContext context, IMapper mapper)
        {
            Mapper = mapper;
            Context = context;
        }

        private readonly Dictionary<Type, BaseRepository> _repositories = new();
        public TRepo GetRepo<TRepo>() where TRepo : BaseRepository, new()
        {
            var repoType = typeof(TRepo);

            if (!_repositories.ContainsKey(repoType))
                _repositories.Add(typeof(TRepo), BaseRepository.CreateRepo<TRepo>(Context, this, Mapper));

            if (_repositories[repoType] is not TRepo repo)
                throw new Exception($"Failed to instantiate repository: \"{typeof(TRepo)}\"");

            return repo;
        }

        public async Task<bool> Complete()
        => await Context.SaveChangesAsync() > 0;

        public bool HasChanges()
        => Context.ChangeTracker.HasChanges();

        public async Task MigrateAsync()
        => await Context.Database.MigrateAsync();
    }
}