using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniApp
{
    class Course : TableEntity 
    {
        public Course() { }
        public Course(string partKey,string Field1) 
        {
        this.PartitionKey = partKey; 
        this.RowKey = Field1;
        
        }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
    }
}
