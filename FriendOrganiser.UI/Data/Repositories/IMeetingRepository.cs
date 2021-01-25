using FriendOrganiser.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganiser.UI.Data.Repositories
{
    public interface IMeetingRepository:IGenericRepository<Meeting>
    {
        Task<List<Friend>> GetAllFriendsAsync();
        Task ReloadFriendAsync(int id);
    }
}