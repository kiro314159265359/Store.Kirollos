using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class DeliveryMethodNotFoundExecption(int id) :
        NotFoundExecption($"Delivery Method with Id:{id} Not Found !!")
    {
    }
}
