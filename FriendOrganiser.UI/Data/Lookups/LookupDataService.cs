﻿using FriendOrganiser.DataAccess;
using FriendOrganiser.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganiser.UI.Data.Lookups
{
    public class LookupDataService : IFriendLookupDataService, IProgrammingLangaugeLookupDataService, IMeetingLookupDataService
    {
        private Func<FriendOrganiserDBContext> _contextCreator;

        public LookupDataService(Func<FriendOrganiserDBContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public async Task<IEnumerable<LookupItem>> GetFriendLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Friends.AsNoTracking()
                    .Select(f => new LookupItem { Id = f.Id, DisplayMember = f.FirstName })
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<LookupItem>> GetProgrammingLanguageLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.ProgrammingLanguages.AsNoTracking()
                    .Select(f => new LookupItem { Id = f.Id, DisplayMember = f.Name })
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<LookupItem>> GetMeetingLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                var items = await ctx.Meetings.AsNoTracking()
                    .Select(f => new LookupItem { Id = f.Id, DisplayMember = f.Title })
                    .ToListAsync();

                return items;
            }
        }
    }
}
