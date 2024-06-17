namespace MyPosTask.Application.Person.Commands.DeletePerson;

public class DeletePersonCommand : IRequest
{
    public int Id { get; set; }
}
