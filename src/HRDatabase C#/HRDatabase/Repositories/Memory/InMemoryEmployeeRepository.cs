public class InMemoryEmployeeRepository : IEmployeeRepository
{
    private readonly Dictionary<Guid, Employee> _employees = new();

    public IEnumerable<Employee> GetAll() => _employees.Values;

    public Employee GetById(Guid id) => _employees.TryGetValue(id, out var emp) ? emp : null;

    public void Add(Employee emp) => _employees[emp.Id] = emp;

    public void Update(Employee emp) => _employees[emp.Id] = emp;

    public void Delete(Guid id) => _employees.Remove(id);
}