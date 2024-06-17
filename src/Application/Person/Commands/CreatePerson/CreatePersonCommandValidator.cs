using MyPosTask.Application.Common.Interfaces;

namespace Microsoft.Extensions.DependencyInjection.People.Commands;

public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    private readonly IApplicationDbContext _context;

    public CreatePersonCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .MustAsync(BeUniqueName).WithMessage("Name must be unique.");

        RuleForEach(v => v.Addresses).SetValidator(new AddressDtoValidator());
    }

    public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return await _context.People
            .AllAsync(p => p.Name != name, cancellationToken);
    }

    public class AddressDtoValidator : AbstractValidator<CreatePersonCommand.AddressDto>
    {
        public AddressDtoValidator()
        {
            RuleFor(v => v.Address1)
                .NotEmpty().WithMessage("Address is required.");

            RuleForEach(v => v.PhoneNumbers).SetValidator(new PhoneNumberDtoValidator());
        }
    }

    public class PhoneNumberDtoValidator : AbstractValidator<CreatePersonCommand.PhoneNumberDto>
    {
        public PhoneNumberDtoValidator()
        {
            RuleFor(v => v.PhoneNumber1)
                .NotEmpty().WithMessage("Phone number is required.");
        }
    }
}
