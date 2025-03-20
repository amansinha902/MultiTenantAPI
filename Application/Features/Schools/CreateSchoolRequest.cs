using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schools
{
    public class CreateSchoolRequest
    {
        public string Name { get; set; }
        public DateTime EstablishedDate { get; set; }
    }
}
