using FriendOrganiser.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganiser.UI.Data.Lookups
{
    public interface IFriendLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetFriendLookupAsync();
    }
}