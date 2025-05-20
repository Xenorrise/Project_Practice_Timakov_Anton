namespace HRDatabase.Core.Interfaces;
public interface IRepository<T> 
    where T : IEntity
{
    IEnumerable<T> GetAll();
    T GetById(Guid id);
    void Add(T item);
    void Update(T item);
    void Delete(Guid id);
}