using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace API.Helpers.Filtration
{
    public class FilteredList<TList> : FilteredList<TList, FiltrationHeader>
    {
        public static async Task<FilteredList<TList>> CreateAsync(
            IQueryable<TList> source, FiltrationParams @params, IMapper mapper)
            => mapper.Map<FilteredList<TList>>(await CreateAsync<FiltrationParams>(source, @params, mapper));

        public static async Task<FilteredList<TList>> CreateAndMapInMemoryAsync<TEntity>(
            IQueryable<TEntity> source, FiltrationParams @params, IMapper mapper)
            => mapper.Map<FilteredList<TList>>(await CreateAndMapInMemoryAsync<FiltrationParams, TEntity>(source, @params, mapper));
    }

    public class FilteredList<TList, THeader> where THeader : FiltrationHeader
    {
        public THeader Header { get; set; }
        public List<TList> Result { get; set; } = new List<TList>();

        public FilteredList() { }

        public FilteredList(IEnumerable<TList> items, THeader header)
        {
            Header = header;

            Result.AddRange(items);
        }

        public static async Task<FilteredList<TList, THeader>> CreateAsync<TParams>(
            IQueryable<TList> source, TParams @params, IMapper mapper)
            where TParams : FiltrationParams
        {
            var (items, header) = await FetchDataAsync<TParams>(source, @params, mapper);

            return new FilteredList<TList, THeader>(items, header);
        }

        public static async Task<FilteredList<TList, THeader>> CreateAndMapInMemoryAsync<TParams, TEntity>(
            IQueryable<TEntity> source, TParams @params, IMapper mapper)
            where TParams : FiltrationParams
        {
            var (items, header) = await FetchDataAsync<TEntity, TParams>(source, @params, mapper);

            var dtos = mapper.Map<IEnumerable<TList>>(items);

            return new FilteredList<TList, THeader>(dtos, header);
        }

        protected static async Task<Tuple<IEnumerable<TList>, THeader>> FetchDataAsync<TParams>(
            IQueryable<TList> source, TParams @params, IMapper mapper)
            where TParams : FiltrationParams
            => await FetchDataAsync<TList, TParams>(source, @params, mapper);

        protected static async Task<Tuple<IEnumerable<T>, THeader>> FetchDataAsync<T, TParams>(
            IQueryable<T> source, TParams @params, IMapper mapper)
            where TParams : FiltrationParams
        {
            var header = mapper.Map<THeader>(@params);

            header.TotalItems = await source.CountAsync();

            var items = await source
                .Skip((header.CurrentPage - 1) * header.PageSize)
                .Take(header.PageSize)
                .ToListAsync();

            header.TotalPages = (int)Math.Ceiling(header.TotalItems / (double)header.PageSize);

            return new Tuple<IEnumerable<T>, THeader>(items, header);
        }
    }
}