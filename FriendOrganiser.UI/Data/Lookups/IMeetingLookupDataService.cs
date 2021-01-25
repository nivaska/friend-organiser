using FriendOrganiser.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganiser.UI.Data.Lookups
{
    public interface IMeetingLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetMeetingLookupAsync();
    }
}