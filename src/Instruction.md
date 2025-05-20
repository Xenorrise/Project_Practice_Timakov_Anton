# Полное руководство: создание собственной библиотеки классов HR Database на C# для начинающих

## Что мы создадим?

Вы построите систему управления HR-данными, где можно хранить сотрудников, отделы, зарплаты и отпуска. Вы реализуете:

* Многоуровневую архитектуру (разделение ответственности),
* Инъекцию зависимостей (Dependency Injection),
* Два типа хранилищ: **In-Memory** (в памяти) и **файловое (JSON)**.

---

## Часть 1. Настройка проекта в Visual Studio

### Шаг 1. Создайте Solution

1. Откройте **Visual Studio**.
2. Нажмите **Create a new project**.
3. Выберите **Class Library (.NET Standard)** или **Class Library (.NET 6/7)** – это будет для доменного слоя.
4. Назовите проект `HRMS.Core`, нажмите **Next** и затем **Create**.

Теперь добавим ещё два проекта:

* `HRMS.Infrastructure` – реализация хранилищ.
* `HRMS.App` – консольное приложение или интерфейс для тестирования.

Добавление второго проекта:

1. Кликните правой кнопкой мыши по Solution (`HRDatabase`) → **Add** → **New Project**.
2. Выберите `Class Library`, назовите `HRMS.Infrastructure`.
3. Повторите для `Console App` – назовите `HRMS.App`.

### Шаг 2. Настройка связей проектов

1. Кликните правой кнопкой на `HRMS.App` → **Add → Project Reference**.
2. Отметьте `HRMS.Core` и `HRMS.Infrastructure`.
3. То же самое для `HRMS.Infrastructure` → укажите зависимость от `HRMS.Core`.

---

## Часть 2. Создание доменных сущностей (HRMS.Core)

### Шаг 3. Добавьте сущности

В `HRMS.Core`, создайте папку `Entities`:

* Правый клик на проект → **Add → New Folder** → `Entities`.

Теперь добавим класс `Employee.cs`:

```csharp
using System;

namespace HRMS.Core.Entities
{
    public class Employee : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid SalaryId { get; set; }
        public DateTime DateOfHire { get; set; }
    }
}
```

Повторите для:

* `Department.cs`
* `Salary.cs`
* `Leave.cs`

Создайте также интерфейс `IEntity.cs`:

```csharp
namespace HRMS.Core.Entities
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
```

### Шаг 4. Добавьте интерфейсы репозиториев

Создайте папку `Interfaces` и добавьте:

```csharp
public interface IRepository<T> where T : IEntity
{
    IEnumerable<T> GetAll();
    T GetById(Guid id);
    void Add(T item);
    void Update(T item);
    void Delete(Guid id);
}
```

Создайте специфичные интерфейсы, например:

```csharp
public interface IEmployeeRepository : IRepository<Employee> { }
```

---

## Часть 3. Реализация хранилищ (HRMS.Infrastructure)

### Шаг 5. Реализация In-Memory хранилища

В `HRMS.Infrastructure` создайте папку `Repositories/Memory`.

Пример: `InMemoryEmployeeRepository.cs`:

```csharp
using HRMS.Core.Entities;
using HRMS.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace HRMS.Infrastructure.Repositories.Memory
{
    public class InMemoryEmployeeRepository : IEmployeeRepository
    {
        private readonly Dictionary<Guid, Employee> _employees = new();

        public IEnumerable<Employee> GetAll() => _employees.Values;
        public Employee GetById(Guid id) => _employees.TryGetValue(id, out var e) ? e : null;
        public void Add(Employee e) => _employees[e.Id] = e;
        public void Update(Employee e) => _employees[e.Id] = e;
        public void Delete(Guid id) => _employees.Remove(id);
    }
}
```

Аналогично создайте `InMemoryDepartmentRepository`, `InMemorySalaryRepository` и т.д.

---

## Часть 4. Реализация файлового хранилища (JSON)

### Шаг 6. Добавьте `Repositories/File` папку

Добавьте `JsonFileEmployeeRepository.cs`:

```csharp
using HRMS.Core.Entities;
using HRMS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HRMS.Infrastructure.Repositories.File
{
    public class JsonFileEmployeeRepository : IEmployeeRepository
    {
        private readonly string _folderPath;

        public JsonFileEmployeeRepository(string folderPath)
        {
            _folderPath = Path.Combine(folderPath, "Employees");
            Directory.CreateDirectory(_folderPath);
        }

        public IEnumerable<Employee> GetAll()
        {
            foreach (var file in Directory.GetFiles(_folderPath, "*.json"))
            {
                var json = File.ReadAllText(file);
                yield return JsonSerializer.Deserialize<Employee>(json);
            }
        }

        public Employee GetById(Guid id)
        {
            var path = Path.Combine(_folderPath, $"{id}.json");
            if (!File.Exists(path)) return null;

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Employee>(json);
        }

        public void Add(Employee e)
        {
            var json = JsonSerializer.Serialize(e);
            File.WriteAllText(Path.Combine(_folderPath, $"{e.Id}.json"), json);
        }

        public void Update(Employee e) => Add(e);
        public void Delete(Guid id)
        {
            var path = Path.Combine(_folderPath, $"{id}.json");
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
```

Аналогично реализуйте для `Department`, `Salary`, `Leave`.

---

## Часть 5. Консольное приложение и DI (HRMS.App)

### Шаг 7. Установите пакет для DI

В `HRMS.App`, откройте **NuGet Package Manager** и установите:

```
Microsoft.Extensions.DependencyInjection
```

### Шаг 8. Пример запуска

В `Program.cs`:

```csharp
using HRMS.Core.Entities;
using HRMS.Core.Interfaces;
using HRMS.Infrastructure.Repositories.File;
using HRMS.Infrastructure.Repositories.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;

class Program
{
    static void Main()
    {
        var services = new ServiceCollection();

        // Поменяй путь на свой
        var dataFolder = "C:\\HRData";

        // Выбор хранилища:
        services.AddSingleton<IEmployeeRepository>(new JsonFileEmployeeRepository(dataFolder));
        // или: services.AddSingleton<IEmployeeRepository, InMemoryEmployeeRepository>();

        var provider = services.BuildServiceProvider();
        var repo = provider.GetRequiredService<IEmployeeRepository>();

        var emp = new Employee { FirstName = "Антон", LastName = "Иванов", DepartmentId = Guid.NewGuid() };
        repo.Add(emp);

        Console.WriteLine("Сотрудники:");
        foreach (var e in repo.GetAll())
        {
            Console.WriteLine($"{e.FirstName} {e.LastName} ({e.Id})");
        }
    }
}
```

---

## Финальная структура проектов

```
HRDatabase.sln
├── HRMS.Core
│   ├── Entities/
│   │   └── Employee.cs, Department.cs, ...
│   ├── Interfaces/
│       └── IRepository.cs, IEmployeeRepository.cs
├── HRMS.Infrastructure
│   ├── Repositories/
│   │   ├── Memory/
│   │   └── File/
├── HRMS.App
│   └── Program.cs
```

---

