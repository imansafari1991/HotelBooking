using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Constants
{
    public class PriceExceptionMessages
    {
        public const string Price_Cant_Be_Negative = "Price cannot be negative";
        public const string Price_Cant_Be_Zero = "Price cannot be zero";
        public const string Price_Cant_Be_More_Than_TenThousand = "Price cannot be more than ten thousand";

    }
}
