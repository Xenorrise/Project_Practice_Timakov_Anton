using HRDatabase.Core.Interfaces;
public class Employee : IEntity 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public decimal Salary { get; set; }
    public DateTime DateOfHire { get; set; }
    public string FullName { get; set; } = string.Empty;
}