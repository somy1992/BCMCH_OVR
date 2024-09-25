using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCMCHOVR.Models
{

    public class ApiActionModal<T>
    {
        public ApiActionModal()
        {
            Status = true;
        }
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string ErrorType { get; set; }
        public string TKey { get; set; }    
    }

    public abstract class BCViewModelBase 
    {
        public string UserName { get; set; }
        public string UserRole { get; set; }

    }
}
