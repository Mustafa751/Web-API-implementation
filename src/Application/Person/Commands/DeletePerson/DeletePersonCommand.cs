namespace Microsoft.Extensions.DependencyInjection.Person.Commands.DeletePerson;

public class DeletePersonCommand : IRequest
{
    public int Id { get; set; }
}
