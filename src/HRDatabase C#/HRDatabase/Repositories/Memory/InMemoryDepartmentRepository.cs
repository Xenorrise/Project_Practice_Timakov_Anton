public class InMemoryDepartmentRepository : IDepartmentRepository
{
    private readonly Dictionary<Guid, Department> _departs = new();

    private Department _safeDep = new();

    public IEnumerable<Department> GetAll() => _departs.Values;

    public Department GetById(Guid id) { if(id != Guid.Empty) return _departs.TryGetValue(id, out var e) ? e : null; return _safeDep; }

    public void Add(Department emp) => _departs[emp.Id] = emp;

    public void Update(Department emp) => _departs[emp.Id] = emp;

    public void Delete(Guid id) => _departs.Remove(id);
}