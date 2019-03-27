using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Validations
{
    public class AddOrderValidation: OrderValidation
    {
        public AddOrderValidation()
        {
            ValidateContent();
        }
    }
}
