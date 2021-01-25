using Autofac;
using FriendOrganiser.DataAccess;
using FriendOrganiser.UI.Data;
using FriendOrganiser.UI.Data.Lookups;
using FriendOrganiser.UI.Data.Repositories;
using FriendOrganiser.UI.View.Services;
using FriendOrganiser.UI.ViewModel;
using Prism.Events;

namespace FriendOrganiser.UI.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            
            builder.RegisterType<FriendOrganiserDBContext>().AsSelf();

            builder.RegisterType<MainWindow>().AsSelf();

            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();

            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();
            builder.RegisterType<FriendDetailViewModel>().As<IFriendDetailViewModel>();
            builder.RegisterType<MeetingDetailViewModel>().As<IMeetingDetailViewModel>();

            builder.RegisterType<FriendRepository>().As<IFriendRepository>();
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
            builder.RegisterType<MeetingRepository>().As<IMeetingRepository>();

            return builder.Build();
        }
    }
}
