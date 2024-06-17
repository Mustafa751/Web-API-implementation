using Microsoft.Extensions.DependencyInjection.Person.Queries;
using MyPosTask.Application.Person.Commands.CreatePerson;
using MyPosTask.Application.Person.Commands.DeletePerson;
using MyPosTask.Application.Person.Commands.UpdatePerson;
using MyPosTask.Web.Endpoints.Mappings;

namespace MyPosTask.Web.Endpoints;

public class People : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetPeopleWithPagination)
            .MapPost(CreatePerson)
            .MapPut(UpdatePerson, "{id}")
            .MapDelete(DeletePerson, "{id}");
    }

    public async Task<IResult> GetPeopleWithPagination(ISender sender, [AsParameters] GetPeopleWithPaginationQuery query)
    {
        var result = await sender.Send(query);
        var response = PersonResponseMapper.MapToResponse(result);
        return Results.Ok(response);
    }


    public Task<int> CreatePerson(ISender sender, CreatePersonCommand command)
    {
        return sender.Send(command);
    }

    public async Task<IResult> UpdatePerson(ISender sender, int id, UpdatePersonCommand command)
    {
        
        //command.Id = id; I did think about setting it like this, but I think the user should ensure they are the same
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.Accepted();
    }

    public async Task<IResult> DeletePerson(ISender sender, int id)
    {
        await sender.Send(new DeletePersonCommand { Id = id });
        return Results.NoContent();
    }
}
