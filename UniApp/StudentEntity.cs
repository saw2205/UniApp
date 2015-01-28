using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniApp
{
    class StudentEntity : TableEntity
    {
        public StudentEntity(string UserName, string Password)
        {
            this.PartitionKey = UserName;
            this.RowKey = Password;
            
        }

        public StudentEntity() { }

        public string PassKey { get; set; }
        public string StudentName { get; set; }
    }
}
