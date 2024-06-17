using Microsoft.Extensions.DependencyInjection.Person.Queries;

namespace MyPosTask.Application.Common.Exceptions;

public class ConcurrencyException<T> : Exception
{
    public ConcurrencyException(string message, T currentPerson) : base(message)
    {
        CurrentPerson = currentPerson;
    }

    public T CurrentPerson { get; }
}
