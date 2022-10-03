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

        //we inject Background manager interface to handle jobs
        public PersonsController(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        [HttpPost("create")]
        public ActionResult Create(string personName)
        {
            //this job will be executed on background right away
            _backgroundJobClient
                .Enqueue<IPersonRepository>(repo => repo.AddPerson(personName));

            return Ok();
        }

        [HttpPost("schedule")]
        public ActionResult Schedule(string personName)
        {
            //this is a scheduled job, it will be executed pass a delay time
            _backgroundJobClient
                .Schedule(() => Console.WriteLine($"The name is {personName}"),
                    TimeSpan.FromMinutes(5));
            return Ok();
        }
    }
}
