using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenseNet.Web.Mvc.Mem.Admin.Models
{
    public class ContainerEnvironment
    {
        public string Name { get; set; }
        public string Authority { get; set; }
    }
    public class SenseNetEnvironment
    {
        public ContainerEnvironment Container { get; set; }
    }
}
