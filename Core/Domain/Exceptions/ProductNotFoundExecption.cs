using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ProductNotFoundExecption(int id) :
        NotFoundExecption($"Product with Id:{id} Not Found !")
    {}
}
