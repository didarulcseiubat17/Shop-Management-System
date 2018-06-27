using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApplicationV.Validators
{
    static class ValidationHelpers
    {
        public static bool BeAValidDate(DateTime date)
        {
            return !date.Equals(default(DateTime));
        }

        public static bool BeAValidBirthDate(DateTime date)
        {
            var x = (int)Math.Floor((DateTime.Now - date).TotalDays / 365.25D);
            return (x > 10 && x < 100);
        }
    }
}
