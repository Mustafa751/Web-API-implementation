using MyPosTask.Application.Person.Commands.DeletePerson;

namespace Microsoft.Extensions.DependencyInjection.Person.Commands.DeletePerson;

public class DeletePersonCommandValidator : AbstractValidator<DeletePersonCommand>
{
    public DeletePersonCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}
