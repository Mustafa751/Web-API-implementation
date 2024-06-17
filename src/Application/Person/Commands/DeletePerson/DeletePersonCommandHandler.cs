using Microsoft.Extensions.DependencyInjection.Person.Queries;
using MyPosTask.Application.Common.Exceptions;
using MyPosTask.Application.Common.Interfaces;
using MyPosTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyPosTask.Application.Person.Commands.DeletePerson
{
    public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DeletePersonCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _context.People
                .Include(p => p.PersonAddresses)
                    .ThenInclude(pa => pa.Address)
                .Include(p => p.PersonAddresses)
                    .ThenInclude(pa => pa.PhoneNumbers)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (person == null)
            {
                throw new NotFoundException(nameof(Person), request.Id.ToString());
            }

            var originalRowVersion = _context.Entry(person).Property("RowVersion").CurrentValue as byte[];

            _context.People.Remove(person);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                var currentPerson = await _context.People
                    .AsNoTracking()
                    .Include(p => p.PersonAddresses)
                        .ThenInclude(pa => pa.Address)
                    .Include(p => p.PersonAddresses)
                        .ThenInclude(pa => pa.PhoneNumbers)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                if (currentPerson == null)
                {
                    throw new NotFoundException(nameof(Person), request.Id.ToString());
                }

                var personDto = _mapper.Map<PersonDto>(currentPerson);

                throw new ConcurrencyException<PersonDto>("The record you attempted to delete was modified by another user after you got the original value.", personDto);
            }
        }
    }
}
