using System.Threading.Tasks;

namespace FriendOrganiser.UI.Data.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        Task SaveAsync();
        bool HasChanges();
        void Add(T friend);
        void Remove(T model);
    }
}