namespace Company.Models.Models
{
    public interface IDepartment
    {
        string departmentLocation { get; set; }
        string departmentName { get; set; }
        int departmentNo { get; set; }
    }
}