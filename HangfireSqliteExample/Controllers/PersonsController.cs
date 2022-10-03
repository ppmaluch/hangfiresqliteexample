using Hangfire;
using HangfireSqliteExample.Data;
using HangfireSqliteExample.Model;
using HangfireSqliteExample.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HangfireSqliteExample.Controllers
{
    [ApiController]
    [Route("api/persons")]
    public class PersonsController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public PersonsController(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        [HttpPost("create")]
        public ActionResult Create(string personName)
        {

            _backgroundJobClient
                .Enqueue<IPersonRepository>(repo => repo.AddPerson(personName));

            return Ok();
        }

        [HttpPost("schedule")]
        public ActionResult Schedule(string personName)
        {
            _backgroundJobClient
                .Schedule(() => Console.WriteLine($"The name is {personName}"),
                    TimeSpan.FromMinutes(5));
            return Ok();
        }
    }
}
