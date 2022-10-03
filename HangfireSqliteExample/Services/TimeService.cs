using System.Globalization;

namespace HangfireSqliteExample.Services
{
    public class TimeService : ITimeService
    {
        private readonly ILogger<TimeService> _logger;

        public TimeService(ILogger<TimeService> logger)
        {
            _logger = logger;
        }

        public void PrintTime()
        {
            _logger.LogInformation(DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }
    }
}
