using FriendOrganiser.UI.Events;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganiser.UI.ViewModel
{
    public class NavigationItemViewModel:ViewModelBase
    {
        private string _displayMember;
        private IEventAggregator _eventAggregator;
        private string _detailViewModelName;

        public NavigationItemViewModel(int id, string displayMember,
            string detailViewModelName,
            IEventAggregator eventAggregator)
        {
            Id = id;
            _displayMember = displayMember;
            _eventAggregator = eventAggregator;
            _detailViewModelName = detailViewModelName;
            OpenDetailViewCommand = new DelegateCommand(OnOpenDetailViewExecute);
        }
     
        public int Id { get; }
        
        public string DisplayMember
        {
            get { return _displayMember; }
            set { 
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenDetailViewCommand { get;  }

        private void OnOpenDetailViewExecute()
        {
            _eventAggregator.GetEvent<OpenDetailEvent>()
                         .Publish(new OpenDetailEventArgs() 
                         { Id = Id, ViewModelName = _detailViewModelName });
        }
    }
}