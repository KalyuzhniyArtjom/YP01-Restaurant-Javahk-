using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Djava.Client
{
    public static class DbConfig
    {
        public static string GetConnectionString()
        {
            return "Host=localhost;Port=5432;Database=djava_restaurant;Username=postgres;Password=1991;Encoding=UTF8";
        }
    }
}
