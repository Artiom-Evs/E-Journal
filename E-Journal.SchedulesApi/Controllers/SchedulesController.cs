using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Journal.SchedulesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        [HttpGet("/{name}/", Name = nameof(Group))]
        public IResult Group(string name)
        {
            return Results.Ok($"Requested group: {name}");
        }
    }
}
