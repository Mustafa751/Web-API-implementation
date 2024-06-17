using Microsoft.Extensions.DependencyInjection.Person.Queries;

namespace MyPosTask.Application.Common.Exceptions;

public class ConcurrencyException : Exception
{
    public ConcurrencyException(string message, PersonDto currentPerson) : base(message)
    {
        CurrentPerson = currentPerson;
    }

    public PersonDto CurrentPerson { get; }
}
