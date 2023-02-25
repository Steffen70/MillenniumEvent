using AutoMapper;

namespace API.Data.Repositories
{
    public abstract class BaseRepository
    {
        protected DataContext Context;
        protected UnitOfWork UnitOfWork;
        protected IMapper Mapper;

        public static TRepo CreateRepo<TRepo>(DataContext context, UnitOfWork unitOfWork, IMapper mapper)
            where TRepo : BaseRepository, new()
        {
            var repo = new TRepo
            {
                Mapper = mapper,
                UnitOfWork = unitOfWork,
                Context = context
            };

            return repo;
        }
    }
}