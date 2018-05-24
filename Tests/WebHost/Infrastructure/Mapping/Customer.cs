using MF.Core;
using System;

namespace WebHost.Infrastructure.Mapping
{
    public class Customer:BaseEntity<Guid>
    {



        public string NickName { get; set; }


        public string PhoneNum { get; set; }
    }
}