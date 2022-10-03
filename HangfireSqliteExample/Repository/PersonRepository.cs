using HangfireSqliteExample.Data;
using HangfireSqliteExample.Model;

namespace HangfireSqliteExample.Repository
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonRepository> _logger;

        public PersonRepository(ApplicationDbContext context, ILogger<PersonRepository> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task AddPerson(string personName)
        {
            _logger.LogInformation($"adding person {personName} from background service");
            var person = new Person { Name = personName };
            _context.Add(person);
            await Task.Delay(5000);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"added person {personName} from background service");
        }
    }
}
