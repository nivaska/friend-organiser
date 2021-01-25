using FriendOrganiser.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganiser.UI.Data.Repositories
{

    public interface IFriendRepository : IGenericRepository<Friend>
    {

        void RemovePhoneNumber(FriendPhoneNumber model);
        Task<bool> HasMeetingsAsync(int friendId);
    }
}