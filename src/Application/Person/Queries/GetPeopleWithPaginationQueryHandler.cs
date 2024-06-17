using MyPosTask.Application.Common.Interfaces;
using MyPosTask.Application.Common.Mappings;
using MyPosTask.Application.Common.Models;
using MyPosTask.Domain.Enums;

namespace Microsoft.Extensions.DependencyInjection.Person.Queries;

public class GetPeopleWithPaginationQueryHandler : IRequestHandler<GetPeopleWithPaginationQuery, PaginatedList<PersonDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPeopleWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<PersonDto>> Handle(GetPeopleWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.People
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Addresses)
            .ThenInclude(a => a.PhoneNumbers)
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

        return new PaginatedList<PersonDto>(personDtos, totalCount, request.PageNumber, request.PageSize);
    }
}
