using System;

namespace Company.Models.Models
{
    public interface IEmployee
    {
        Department department { get; set; }
        int EdepartmentNo { get; set; }
        string employeeName { get; set; }
        int employeeNo { get; set; }
        DateTime lastModifyDate { get; set; }
        int Salary { get; set; }
    }
}