using MF.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebHost.Infrastructure
{
    public class WebHostDbContext : MFDbContext<Guid>
    {
        public WebHostDbContext(List<string> nameSpaces, string nameOrConnectionString)
            : base(nameSpaces, nameOrConnectionString)
        {
        }
    }
}