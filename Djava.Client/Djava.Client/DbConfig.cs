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
            return "Host=192.168.1.48;Port=5432;Database=djava_restaurant;Username=st52-03;Password=5203;Encoding=UTF8";
        }
    }
}
