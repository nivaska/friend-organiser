using FriendOrganiser.Model;
using FriendOrganiser.UI.Data;
using FriendOrganiser.UI.Data.Lookups;
using FriendOrganiser.UI.Data.Repositories;
using FriendOrganiser.UI.Events;
using FriendOrganiser.UI.View.Services;
using FriendOrganiser.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Data.Entity.Infrastructure;

namespace FriendOrganiser.UI.ViewModel
{
    public class FriendDetailViewModel : DetailViewModelBase, IFriendDetailViewModel
    {
        private IFriendRepository _friendRepository;
        private FriendPhoneNumberWrapper _selectedPhoneNumber;
        private IProgrammingLangaugeLookupDataService _programmingLangaugeLookupDataService;
        private FriendWrapper _friend;

        public FriendDetailViewModel(IFriendRepository friendRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IProgrammingLangaugeLookupDataService programmingLangaugeLookupDataService)
            : base(eventAggregator, messageDialogService)
        {
            _friendRepository = friendRepository;
            _programmingLangaugeLookupDataService = programmingLangaugeLookupDataService;

            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);
            ProgrammingLanguages = new ObservableCollection<LookupItem>();
            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();

        }

        public FriendWrapper Friend
        {
            get { return _friend; }
            private set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddPhoneNumberCommand { get; set; }
        public ICommand RemovePhoneNumberCommand { get; set; }

        public ObservableCollection<LookupItem> ProgrammingLanguages { get; }
        public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; }

        public FriendPhoneNumberWrapper SelectedPhoneNumber
        {
            get { return _selectedPhoneNumber; }
            set
            {
                _selectedPhoneNumber = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
            }
        }

        public override async Task LoadAsync(int friendId)
        {
            var friend = friendId > 0 ?
                         await _friendRepository.GetByIdAsync(friendId) :
                         CreateNewFriend();

            Id = friendId;

            InitializeFriend(friend);
            InitializeFriendPhoneNumbers(friend.PhoneNumbers);
            await LoadProgrammingLanguagesAsync();
        }

        private void InitializeFriendPhoneNumbers(ICollection<FriendPhoneNumber> phoneNumbers)
        {
            foreach (var wrapper in PhoneNumbers)
            {
                wrapper.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            }

            PhoneNumbers.Clear();

            foreach (var friendPhoneNumber in phoneNumbers)
            {
                var wrapper = new FriendPhoneNumberWrapper(friendPhoneNumber);
                PhoneNumbers.Add(wrapper);
                wrapper.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            }
        }

        private void FriendPhoneNumberWrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (!HasChanges)
            {
                HasChanges = _friendRepository.HasChanges();
            }
            if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        protected override bool OnSaveCanExecute()
        {
            return Friend != null && !Friend.HasErrors
                && PhoneNumbers.All(pn => !pn.HasErrors)
                && HasChanges;
        }

        protected override async void OnSaveExecute()
        {
            try
            {
                await _friendRepository.SaveAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var dbValues = ex.Entries.Single().GetDatabaseValues();
                if (dbValues == null)
                {
                    MessageDialogService.ShowInfoDialog("The entity has been deleted by another user.");
                    RaiseDetailDeletedEvent(Id);
                    return;
                }

                var result = MessageDialogService.ShowOkCancelDialog("The entity has been changed by someone else.\n" +
                    "Click OK to saye your changes anyway, click Cancel to reload entity from DB.", "Warning");

                if (result == MessageDialogResult.OK)
                {
                    // update the original values with db values
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    await _friendRepository.SaveAsync();
                }
                else
                {
                    await ex.Entries.Single().ReloadAsync();
                    await LoadAsync(Friend.Id);
                }
            }

            HasChanges = _friendRepository.HasChanges();
            Id = Friend.Id;
            RaiseDetailSavedEvent(Friend.Id, $"{Friend.FirstName} {Friend.LastName}");
        }

        private bool OnRemovePhoneNumberCanExecute()
        {
            return SelectedPhoneNumber != null;
        }

        private void OnRemovePhoneNumberExecute()
        {
            SelectedPhoneNumber.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            _friendRepository.RemovePhoneNumber(SelectedPhoneNumber.Model);
            PhoneNumbers.Remove(SelectedPhoneNumber);
            SelectedPhoneNumber = null;
            HasChanges = _friendRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddPhoneNumberExecute()
        {
            var newNumber = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
            newNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            PhoneNumbers.Add(newNumber);
            Friend.Model.PhoneNumbers.Add(newNumber.Model);
            newNumber.Number = ""; // to trigger validation
        }

        protected override async void OnDeleteExecute()
        {
            if (await _friendRepository.HasMeetingsAsync(Friend.Id))
            {
                MessageDialogService.ShowInfoDialog($"{Friend.FirstName} {Friend.LastName} can'be deleted. He is part of atleast one meeting");
                return;
            }

            var result = MessageDialogService.ShowOkCancelDialog($"Do you want to delete the friend {Friend.Model.FirstName}",
                "Warning");

            if (result == MessageDialogResult.OK)
            {
                _friendRepository.Remove(Friend.Model);
                await _friendRepository.SaveAsync();
                RaiseDetailDeletedEvent(Friend.Id);
            }
        }

        private Friend CreateNewFriend()
        {
            var friend = new Friend();
            _friendRepository.Add(friend);
            return friend;
        }

        private void InitializeFriend(Friend friend)
        {
            Friend = new FriendWrapper(friend);
            Friend.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _friendRepository.HasChanges();
                }

                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }

                if (e.PropertyName == nameof(Friend.FirstName) || e.PropertyName == nameof(Friend.LastName))
                {
                    SetTitle();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            if (Friend.Id == 0)
            {
                // trick to trigger validation
                Friend.FirstName = "";
            }
            SetTitle();
        }

        private void SetTitle()
        {
            Title = $"{Friend.FirstName} {Friend.LastName}";
        }

        private async Task LoadProgrammingLanguagesAsync()
        {
            ProgrammingLanguages.Clear();
            ProgrammingLanguages.Add(new NullLookupItem() { DisplayMember = "-" });
            var lookup = await _programmingLangaugeLookupDataService.GetProgrammingLanguageLookupAsync();
            foreach (var lookupItem in lookup)
            {
                ProgrammingLanguages.Add(lookupItem);
            }
        }

    }
}
