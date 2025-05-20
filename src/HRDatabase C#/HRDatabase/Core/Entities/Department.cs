using HRDatabase.Core.Interfaces;

public class Department : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public int EmployeesCount { get; set; }
    public string ExtendedName => $"{Name}\t\t{EmployeesCount}";

}