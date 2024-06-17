using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Person.Queries;
using MyPosTask.Application.Common.Interfaces;
using MyPosTask.Application.Common.Models;

namespace MyPosTask.Application.Person.Queries;

public class GetPeopleWithPaginationQueryHandler : IRequestHandler<GetPeopleWithPaginationQuery, PaginatedList<PersonDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public GetPeopleWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IMemoryCache cache)
    {
        _context = context;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<PaginatedList<PersonDto>> Handle(GetPeopleWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"GetPeopleWithPagination_{request.PageNumber}_{request.PageSize}_{request.Filter}";
        
        if (!_cache.TryGetValue(cacheKey, out PaginatedList<PersonDto>? cachedResult) || cachedResult == null)
        {
            var query = _context.People
                .AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.PersonAddresses)
                    .ThenInclude(pa => pa.Address)
                .Include(p => p.PersonAddresses)
                    .ThenInclude(pn => pn.PhoneNumbers)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                query = query.Where(p => p.Name.Contains(request.Filter));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(p => p.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var personDtos = _mapper.Map<List<PersonDto>>(items);

            cachedResult = new PaginatedList<PersonDto>(personDtos, totalCount, request.PageNumber, request.PageSize);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            };

            _cache.Set(cacheKey, cachedResult, cacheEntryOptions);
        }

        return cachedResult;
    }
}
