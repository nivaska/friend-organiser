namespace FriendOrganiser.DataAccess.Migrations
{
    using FriendOrganiser.Model;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FriendOrganiser.DataAccess.FriendOrganiserDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FriendOrganiser.DataAccess.FriendOrganiserDBContext context)
        {
            context.Friends.AddOrUpdate(f => f.FirstName,
                new Friend() { FirstName = "Nivas", LastName = "Anand" },
                new Friend() { FirstName = "Dwight", LastName = "Schrute" },
                new Friend() { FirstName = "Jim", LastName = "Harper" });

            context.ProgrammingLanguages.AddOrUpdate(f => f.Name,
                new ProgrammingLanguage() { Name = "C#" },
                new ProgrammingLanguage() { Name = "TypeScript" },
                new ProgrammingLanguage() { Name = "JavaScript" },
                new ProgrammingLanguage() { Name = "C++" },
                new ProgrammingLanguage() { Name = "Java" });

            context.SaveChanges();

            context.FriendPhoneNumbers.AddOrUpdate(
                pn => pn.Number, new FriendPhoneNumber { Number = "+49 176 121212", FriendId = context.Friends.First().Id }
                );

            context.Meetings.AddOrUpdate(
                m => m.Title,
                new Meeting
                {
                    Title = "Watching Football",
                    DateFrom = new DateTime(2020, 12, 11),
                    DateTo = new DateTime(2020, 12, 11),
                    Friends = new List<Friend>
                    {
                        context.Friends.Single(f => f.FirstName == "Nivas"),
                        context.Friends.Single(f => f.FirstName == "Jim")
                    }
                });
        }
    }
}
