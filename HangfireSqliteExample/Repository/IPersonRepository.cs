namespace HangfireSqliteExample.Repository
{
    public interface IPersonRepository
    {
        public Task AddPerson(string personName);
    }
}
