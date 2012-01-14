using System.Collections.Generic;
using System.Data.Entity;
using Cocktail;
using IdeaBlade.EntityModel;

namespace DomainModel
{
    [DataSourceKeyName("TempHireEntities")]
    internal class TempHireDbContext : DbContext
    {
        public TempHireDbContext(string connection = null)
            : base(connection)
        {
            Database.SetInitializer(new TempHireDbInitializer());
        }

        public DbSet<Resource> Resources { get; set; }
        public DbSet<AddressType> AddressTypes { get; set; }
        public DbSet<PhoneNumberType> PhoneNumberTypes { get; set; }
        public DbSet<RateType> RateTypes { get; set; }
        public DbSet<State> States { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<EntityAspect>();
        }
    }

    internal class TempHireDbInitializer : DropCreateDatabaseIfModelChanges<TempHireDbContext>
    {
        protected override void Seed(TempHireDbContext context)
        {
            var addressTypes = new List<AddressType>
                                   {
                                       new AddressType
                                           {
                                               Id = CombGuid.NewGuid(),
                                               Name = "Main",
                                               DisplayName = "Main Address",
                                               Default = true
                                           },
                                       new AddressType
                                           {Id = CombGuid.NewGuid(), Name = "Home", DisplayName = "Home Address"},
                                       new AddressType
                                           {Id = CombGuid.NewGuid(), Name = "Work", DisplayName = "Work Address"}
                                   };
            addressTypes.ForEach(e => context.AddressTypes.Add(e));

            var phoneTypes = new List<PhoneNumberType>
                                 {
                                     new PhoneNumberType {Id = CombGuid.NewGuid(), Name = "Home", Default = true},
                                     new PhoneNumberType {Id = CombGuid.NewGuid(), Name = "Mobile"}
                                 };
            phoneTypes.ForEach(e => context.PhoneNumberTypes.Add(e));

            var rateTypes = new List<RateType>
                                {
                                    new RateType
                                        {
                                            Id = CombGuid.NewGuid(),
                                            Name = "hourly",
                                            DisplayName = "Per Hour",
                                            Sequence = 0
                                        },
                                    new RateType
                                        {Id = CombGuid.NewGuid(), Name = "daily", DisplayName = "Per Day", Sequence = 1},
                                    new RateType
                                        {
                                            Id = CombGuid.NewGuid(),
                                            Name = "weekly",
                                            DisplayName = "Per Week",
                                            Sequence = 2
                                        },
                                    new RateType
                                        {
                                            Id = CombGuid.NewGuid(),
                                            Name = "monthly",
                                            DisplayName = "Per Month",
                                            Sequence = 3
                                        }
                                };
            rateTypes.ForEach(e => context.RateTypes.Add(e));

            var states = new List<State>
                             {
                                 new State {Id = CombGuid.NewGuid(), ShortName = "AL", Name = "Alabama"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "AK", Name = "Alaska"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "AZ", Name = "Arizona"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "AR", Name = "Arkansas"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "CA", Name = "California"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "CO", Name = "Colorado"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "CT", Name = "Connecticut"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "DE", Name = "Delaware"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "DC", Name = "District of Columbia"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "FL", Name = "Florida"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "GA", Name = "Georgia"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "HI", Name = "Hawaii"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "ID", Name = "Idaho"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "IL", Name = "Illinois"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "IN", Name = "Indiana"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "IA", Name = "Iowa"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "KS", Name = "Kansas"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "KY", Name = "Kentucky"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "LA", Name = "Louisiana"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "ME", Name = "Maine"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "MD", Name = "Maryland"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "MA", Name = "Massachusetts"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "MI", Name = "Michigan"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "MN", Name = "Minnesota"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "MS", Name = "Mississippi"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "MO", Name = "Missouri"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "MT", Name = "Montana"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "NE", Name = "Nebraska"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "NV", Name = "Nevada"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "NH", Name = "New Hampshire"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "NJ", Name = "New Jersey"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "NM", Name = "New Mexico"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "NY", Name = "New York"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "NC", Name = "North Carolina"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "ND", Name = "North Dakota"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "OH", Name = "Ohio"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "OK", Name = "Oklahoma"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "OR", Name = "Oregan"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "PA", Name = "Pennsylvania"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "RI", Name = "Rhode Island"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "SC", Name = "South Carolina"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "SD", Name = "South Dakota"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "TN", Name = "Tennessee"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "TX", Name = "Texas"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "UT", Name = "Utah"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "VT", Name = "Vermont"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "VA", Name = "Virginia"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "WA", Name = "Washington"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "WV", Name = "West Virginia"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "WI", Name = "Wisconsin"},
                                 new State {Id = CombGuid.NewGuid(), ShortName = "WY", Name = "Wyoming"},
                             };
            states.ForEach(e => context.States.Add(e));
        }
    }
}