using System.Data.Common;
using System.Text.Json;

public class JsonFileDepartmentRepository : IDepartmentRepository
{
    private string _folder;
    private readonly Dictionary<Guid, Department> _cache = new();
    private Department _safeDep = new();
    public JsonFileDepartmentRepository(string dataFolder)
    {
        ChangeFolder(dataFolder);
    }

    public void ChangeFolder(string dataFolder)
    {
        _cache.Clear();
        if (!Path.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);
        else if (new DirectoryInfo(dataFolder).Name == "HRDB")
            _folder = Path.Combine(dataFolder, "Departments");
        else
            _folder = Path.Combine(dataFolder, "HRDB/Departments");
        Directory.CreateDirectory(_folder);
        foreach (var file in Directory.GetFiles(_folder, "*.json"))
        {
            var content = File.ReadAllText(file);
            var dep = JsonSerializer.Deserialize<Department>(content);
            if (dep != null) _cache[dep.Id] = dep;
        }
    }
    
    public IEnumerable<Department> GetAll() => _cache.Values;

    public Department GetById(Guid id) { if(id != Guid.Empty) return _cache.TryGetValue(id, out var e) ? e : null; return _safeDep; }

    public void Add(Department dep)
    {
        _cache[dep.Id] = dep;
        SaveToFile(dep);
    }

    public void Update(Department dep)
    {
        if (dep != _safeDep)
        {
            _cache[dep.Id] = dep;
            SaveToFile(dep);
        }
    }

    public void Delete(Guid id)
    {
        if (_cache.Remove(id))
            File.Delete(Path.Combine(_folder, id + ".json"));
    }

    private void SaveToFile(Department dep)
    {
        var filePath = Path.Combine(_folder, dep.Id + ".json");
        var json = JsonSerializer.Serialize(dep);
        File.WriteAllText(filePath, json);
    }
}