namespace HRDatabase;

using Microsoft.Extensions.DependencyInjection;

public class DBControl {
    private IEmployeeRepository _empRepoInMemory;
    private JsonFileEmployeeRepository _empRepoJson;
    private IDepartmentRepository _depRepoInMemory;
    private JsonFileDepartmentRepository _depRepoJson;

    public DBControl()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IEmployeeRepository, InMemoryEmployeeRepository>();
        services.AddSingleton<IDepartmentRepository, InMemoryDepartmentRepository>();
        services.AddSingleton<IEmployeeRepository, JsonFileEmployeeRepository>();
        services.AddSingleton<IDepartmentRepository, JsonFileDepartmentRepository>();
        //var provider = services.BuildServiceProvider();

        string dBPath = $"C:/Users/{Environment.UserName}/Documents/HRDB/";
        _empRepoInMemory = new InMemoryEmployeeRepository();
        _depRepoInMemory = new InMemoryDepartmentRepository();
        _empRepoJson = new JsonFileEmployeeRepository(dBPath);
        _depRepoJson = new JsonFileDepartmentRepository(dBPath);
    }
    public void AddDepartment(string name, bool isInMemory)
    {
        var dep = new Department { Name = name };
        if (isInMemory)
            _depRepoInMemory.Add(dep);
        else
            _depRepoJson.Add(dep);
    }

    public void ChangeFolder(string newFolder)
    {
        _depRepoJson.ChangeFolder(newFolder);
        _empRepoJson.ChangeFolder(newFolder);
    }

    public void UpdateDepartment(Guid id, string name, Guid departId, bool isInMemory)
    {
        Department existing;
        if (isInMemory)
            existing = _depRepoInMemory.GetById(id) ?? throw new Exception("Employee not found.");
        else
            existing = _depRepoJson.GetById(id) ?? throw new Exception("Employee not found.");

        existing.Name = name;

        if (isInMemory)
            _depRepoInMemory.Update(existing);
        else
            _depRepoJson.Update(existing);
    }

    public void DeleteDepartment(Guid id, bool isInMemory)
    {
        foreach (Employee emp in ReadEmployee(isInMemory).Where(emp => emp.DepartmentId == id))
        {
            UpdateEmployee(emp.Id, emp.FirstName, emp.LastName, emp.Salary, Guid.Empty, isInMemory);
        }
        if (isInMemory)
            _depRepoInMemory.Delete(id);
        else
            _depRepoJson.Delete(id);
    }

    public IEnumerable<Department> ReadDepatment(bool isInMemory)
    {
        if (isInMemory)
            return _depRepoInMemory.GetAll().Reverse();
        else
            return _depRepoJson.GetAll().Reverse();
    }

    public void AddEmployee(string firstName, string lastName, decimal salary, Guid departId, bool isInMemory)
    {
        if (isInMemory)
        {
            var emp = new Employee { FirstName = firstName, LastName = lastName, DepartmentId = departId, DateOfHire = DateTime.Today, Salary = salary, FullName = $"{firstName} {lastName}\t{salary}\t{_depRepoInMemory.GetById(departId).Name}\t\t\t{DateTime.Now.ToShortDateString()}" };
            _depRepoInMemory.GetById(departId).EmployeesCount++;
            _empRepoInMemory.Add(emp);
        }
        else
        {
            var emp = new Employee { FirstName = firstName, LastName = lastName, DepartmentId = departId, DateOfHire = DateTime.Today, Salary = salary, FullName = $"{firstName} {lastName}\t{salary}\t{_depRepoJson.GetById(departId).Name}\t\t\t{DateTime.Now.ToShortDateString()}" };
            _depRepoJson.GetById(departId).EmployeesCount++;
            _depRepoJson.Update(_depRepoJson.GetById(departId));
            _empRepoJson.Add(emp);
        }
    }
    public void UpdateEmployee(Guid id, string firstName, string lastName, decimal salary, Guid departId, bool isInMemory)
    {
        Employee existing;
        if (isInMemory)
            existing = _empRepoInMemory.GetById(id) ?? throw new Exception("Employee not found.");
        else
            existing = _empRepoJson.GetById(id) ?? throw new Exception("Employee not found.");
        
        existing.FirstName = firstName;
        existing.LastName = lastName;
        existing.Salary = salary;
        Guid prevDepartId = existing.DepartmentId;
        existing.DepartmentId = departId;

        if (prevDepartId != departId)
        {
            if (isInMemory)
            {
                _depRepoInMemory.GetById(departId).EmployeesCount++;
                _depRepoInMemory.GetById(prevDepartId).EmployeesCount--;
            }
            else
            {
                _depRepoJson.GetById(departId).EmployeesCount++;
                _depRepoJson.GetById(prevDepartId).EmployeesCount--;
                _depRepoJson.Update(_depRepoJson.GetById(departId));
                _depRepoJson.Update(_depRepoJson.GetById(prevDepartId));
            }
        }

        if (isInMemory)
            existing.FullName = $"{firstName} {lastName}\t{salary}\t{_depRepoInMemory.GetById(departId).Name}\t\t\t{DateTime.Now.ToShortDateString()}";
        else
            existing.FullName = $"{firstName} {lastName}\t{salary}\t{_depRepoJson.GetById(departId).Name}\t\t\t{DateTime.Now.ToShortDateString()}";
        
        if (isInMemory)
            _empRepoInMemory.Update(existing);
        else
            _empRepoJson.Update(existing);
    }

    public void DeleteEmployee(Guid id, bool isInMemory)
    {
        if (isInMemory)
        {
            _depRepoInMemory.GetById(_empRepoInMemory.GetById(id).DepartmentId).EmployeesCount--;
            _empRepoInMemory.Delete(id);
        }
        else
        {
            _depRepoJson.GetById(_empRepoJson.GetById(id).DepartmentId).EmployeesCount--;
            _empRepoJson.Delete(id);
            _depRepoJson.Update(_depRepoJson.GetById(_empRepoJson.GetById(id).DepartmentId));
        }
    }

    public IEnumerable<Employee> ReadEmployee(bool isInMemory)
    {
        if (isInMemory)
            return _empRepoInMemory.GetAll().Reverse();
        else
            return _empRepoJson.GetAll().Reverse();
    }
}