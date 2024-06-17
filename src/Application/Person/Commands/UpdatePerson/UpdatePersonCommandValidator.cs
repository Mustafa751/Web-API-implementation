using FluentValidation;
using MyPosTask.Application.Common.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection.People.Commands.UpdatePerson
{
    public class UpdatePersonCommandValidator : AbstractValidator<UpdatePersonCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdatePersonCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
                .MustAsync(BeUniqueName).WithMessage("Name must be unique.");

            RuleForEach(v => v.Addresses).SetValidator(new AddressDtoValidator());
        }

        public async Task<bool> BeUniqueName(UpdatePersonCommand command, string name,
            CancellationToken cancellationToken)
        {
            var personId = GetPersonIdFromContext(); // Fetch the person ID from the context

            return await _context.People
                .Where(p => p.Id != personId)
                .AllAsync(p => p.Name != name, cancellationToken);
        }

        private int GetPersonIdFromContext()
        {
            // This method should fetch the person ID from the current context (e.g., route parameters, authenticated user)
            // Placeholder implementation
            return 1; // Replace with actual implementation
        }
    }

    public class AddressDtoValidator : AbstractValidator<UpdatePersonCommand.AddressDto>
    {
        public AddressDtoValidator()
        {
            RuleFor(v => v.Address1)
                .NotEmpty().WithMessage("Address is required.");

            RuleForEach(v => v.PhoneNumbers).SetValidator(new PhoneNumberDtoValidator());
        }
    }

    public class PhoneNumberDtoValidator : AbstractValidator<UpdatePersonCommand.PhoneNumberDto>
    {
        public PhoneNumberDtoValidator()
        {
            RuleFor(v => v.PhoneNumber1)
                .NotEmpty().WithMessage("Phone number is required.");
        }
    }
}
