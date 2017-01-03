using Hangfire.Dashboard;

namespace Server.Hangfire
{
    public class AllowAnyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}