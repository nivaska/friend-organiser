﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganiser.UI.Events
{
    public class AfterDetailSavedEvent: PubSubEvent<AfterDetailSavedEventArgs>
    {
    }

    public class AfterDetailSavedEventArgs
    {

        public int Id { get; set; }
        public string DisplayMember { get; set; }
        public string ViewModelName { get; set; }

    }
}
