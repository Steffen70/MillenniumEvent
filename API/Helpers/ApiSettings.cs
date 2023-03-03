using Microsoft.AspNetCore.Hosting;

namespace API.Helpers
{
    public class ApiSettings
    {
        public string ConnectionString { get; set; }
        public string TokenKey { get; set; }
        public string AdminEmail { get; set; }
        public string AdminPassword { get; set; }
        public string DefaultPassword { get; set; }

        public string FlyerImageName { get; set; }
        public string TempFolder { get; set; }
        public EmailConfiguration EmailConfiguration { get; set; }
    }
}