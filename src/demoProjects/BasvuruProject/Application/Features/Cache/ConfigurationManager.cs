using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Cache
{
    static  class ConfigurationManager
    {
        public static IConfiguration AppSetting
        {
            get;
        }
        static ConfigurationManager()
        {
            AppSetting = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            

        }
    }
}
