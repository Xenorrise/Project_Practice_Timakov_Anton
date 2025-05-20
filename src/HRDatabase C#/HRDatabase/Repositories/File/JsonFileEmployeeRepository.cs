using System.Text.Json;

public class JsonFileEmployeeRepository : IEmployeeRepository
{
    private string _folder;
    private readonly Dictionary<Guid, Employee> _cache = new();
    public JsonFileEmployeeRepository(string dataFolder)
    {
        ChangeFolder(dataFolder);
    }

    public void ChangeFolder(string dataFolder)
    {
        _cache.Clear();
        if (!Path.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);
        else if (new DirectoryInfo(dataFolder).Name == "HRDB")
            _folder = Path.Combine(dataFolder, "Employees");
        else
            _folder = Path.Combine(dataFolder, "HRDB/Employees");
        Directory.CreateDirectory(_folder);
        foreach (var file in Directory.GetFiles(_folder, "*.json"))
        {
            var content = File.ReadAllText(file);
            var emp = JsonSerializer.Deserialize<Employee>(content);
            if (emp != null) _cache[emp.Id] = emp;
        }
    }

    public IEnumerable<Employee> GetAll() => _cache.Values;

    public Employee GetById(Guid id) => _cache.TryGetValue(id, out var e) ? e : null;

    public void Add(Employee emp)
    {
        _cache[emp.Id] = emp;
        SaveToFile(emp);
    }

    public void Update(Employee emp)
    {
        _cache[emp.Id] = emp;
        SaveToFile(emp);
    }

    public void Delete(Guid id)
    {
        if (_cache.Remove(id))
            File.Delete(Path.Combine(_folder, id + ".json"));
    }

    private void SaveToFile(Employee emp)
    {
        var filePath = Path.Combine(_folder, emp.Id + ".json");
        var json = JsonSerializer.Serialize(emp);
        File.WriteAllText(filePath, json);
    }
}