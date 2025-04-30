using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class OrderNotFoundExecption(Guid id) :
       NotFoundExecption($"Order with Id:{id} Not Found !!")
    { }
}
