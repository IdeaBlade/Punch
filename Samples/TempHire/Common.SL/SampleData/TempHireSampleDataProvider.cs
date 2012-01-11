using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Cocktail;
using DomainModel;

namespace Common.SampleData
{
    [Export(typeof(ISampleDataProvider<TempHireEntities>))]
    public class TempHireSampleDataProvider : ISampleDataProvider<TempHireEntities>
    {
        #region ISampleDataProvider<TempHireEntities> Members

        public void AddSampleData(TempHireEntities manager)
        {
            GenerateSampleData(manager);
        }

        #endregion

        public static Guid CreateGuid(int a)
        {
            return new Guid(a, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        private void GenerateSampleData(TempHireEntities manager)
        {
            var addressTypes = new List<AddressType>
                                   {
                                       new AddressType
                                           {
                                               Id = CreateGuid(1),
                                               Name = "Main",
                                               DisplayName = "Main Address",
                                               Default = true
                                           },
                                       new AddressType {Id = CreateGuid(2), Name = "Home", DisplayName = "Home Address"},
                                       new AddressType {Id = CreateGuid(3), Name = "Work", DisplayName = "Work Address"}
                                   };
            manager.AttachEntities(addressTypes);

            var phoneTypes = new List<PhoneNumberType>
                                 {
                                     new PhoneNumberType {Id = CreateGuid(1), Name = "Home", Default = true},
                                     new PhoneNumberType {Id = CreateGuid(2), Name = "Mobile"}
                                 };
            manager.AttachEntities(phoneTypes);

            var rateTypes = new List<RateType>
                                {
                                    new RateType
                                        {Id = CreateGuid(1), Name = "hourly", DisplayName = "Per Hour", Sequence = 0},
                                    new RateType
                                        {Id = CreateGuid(2), Name = "daily", DisplayName = "Per Day", Sequence = 1},
                                    new RateType
                                        {Id = CreateGuid(3), Name = "weekly", DisplayName = "Per Week", Sequence = 2},
                                    new RateType
                                        {
                                            Id = CreateGuid(4),
                                            Name = "monthly",
                                            DisplayName = "Per Month",
                                            Sequence = 3
                                        }
                                };
            manager.AttachEntities(rateTypes);

            var states = new List<State>
                             {
                                 new State {Id = CreateGuid(1), ShortName = "AL", Name = "Alabama"},
                                 new State {Id = CreateGuid(2), ShortName = "AK", Name = "Alaska"},
                                 new State {Id = CreateGuid(3), ShortName = "AZ", Name = "Arizona"},
                                 new State {Id = CreateGuid(4), ShortName = "AR", Name = "Arkansas"},
                                 new State {Id = CreateGuid(5), ShortName = "CA", Name = "California"},
                                 new State {Id = CreateGuid(6), ShortName = "CO", Name = "Colorado"},
                                 new State {Id = CreateGuid(7), ShortName = "CT", Name = "Connecticut"},
                                 new State {Id = CreateGuid(8), ShortName = "DE", Name = "Delaware"},
                                 new State {Id = CreateGuid(9), ShortName = "DC", Name = "District of Columbia"},
                                 new State {Id = CreateGuid(10), ShortName = "FL", Name = "Florida"},
                                 new State {Id = CreateGuid(11), ShortName = "GA", Name = "Georgia"},
                                 new State {Id = CreateGuid(12), ShortName = "HI", Name = "Hawaii"},
                                 new State {Id = CreateGuid(13), ShortName = "ID", Name = "Idaho"},
                                 new State {Id = CreateGuid(14), ShortName = "IL", Name = "Illinois"},
                                 new State {Id = CreateGuid(15), ShortName = "IN", Name = "Indiana"},
                                 new State {Id = CreateGuid(16), ShortName = "IA", Name = "Iowa"},
                                 new State {Id = CreateGuid(17), ShortName = "KS", Name = "Kansas"},
                                 new State {Id = CreateGuid(18), ShortName = "KY", Name = "Kentucky"},
                                 new State {Id = CreateGuid(19), ShortName = "LA", Name = "Louisiana"},
                                 new State {Id = CreateGuid(20), ShortName = "ME", Name = "Maine"},
                                 new State {Id = CreateGuid(21), ShortName = "MD", Name = "Maryland"},
                                 new State {Id = CreateGuid(22), ShortName = "MA", Name = "Massachusetts"},
                                 new State {Id = CreateGuid(23), ShortName = "MI", Name = "Michigan"},
                                 new State {Id = CreateGuid(24), ShortName = "MN", Name = "Minnesota"},
                                 new State {Id = CreateGuid(25), ShortName = "MS", Name = "Mississippi"},
                                 new State {Id = CreateGuid(26), ShortName = "MO", Name = "Missouri"},
                                 new State {Id = CreateGuid(27), ShortName = "MT", Name = "Montana"},
                                 new State {Id = CreateGuid(28), ShortName = "NE", Name = "Nebraska"},
                                 new State {Id = CreateGuid(29), ShortName = "NV", Name = "Nevada"},
                                 new State {Id = CreateGuid(30), ShortName = "NH", Name = "New Hampshire"},
                                 new State {Id = CreateGuid(31), ShortName = "NJ", Name = "New Jersey"},
                                 new State {Id = CreateGuid(32), ShortName = "NM", Name = "New Mexico"},
                                 new State {Id = CreateGuid(33), ShortName = "NY", Name = "New York"},
                                 new State {Id = CreateGuid(34), ShortName = "NC", Name = "North Carolina"},
                                 new State {Id = CreateGuid(35), ShortName = "ND", Name = "North Dakota"},
                                 new State {Id = CreateGuid(36), ShortName = "OH", Name = "Ohio"},
                                 new State {Id = CreateGuid(37), ShortName = "OK", Name = "Oklahoma"},
                                 new State {Id = CreateGuid(38), ShortName = "OR", Name = "Oregan"},
                                 new State {Id = CreateGuid(39), ShortName = "PA", Name = "Pennsylvania"},
                                 new State {Id = CreateGuid(40), ShortName = "RI", Name = "Rhode Island"},
                                 new State {Id = CreateGuid(41), ShortName = "SC", Name = "South Carolina"},
                                 new State {Id = CreateGuid(42), ShortName = "SD", Name = "South Dakota"},
                                 new State {Id = CreateGuid(43), ShortName = "TN", Name = "Tennessee"},
                                 new State {Id = CreateGuid(44), ShortName = "TX", Name = "Texas"},
                                 new State {Id = CreateGuid(45), ShortName = "UT", Name = "Utah"},
                                 new State {Id = CreateGuid(46), ShortName = "VT", Name = "Vermont"},
                                 new State {Id = CreateGuid(47), ShortName = "VA", Name = "Virginia"},
                                 new State {Id = CreateGuid(48), ShortName = "WA", Name = "Washington"},
                                 new State {Id = CreateGuid(49), ShortName = "WV", Name = "West Virginia"},
                                 new State {Id = CreateGuid(50), ShortName = "WI", Name = "Wisconsin"},
                                 new State {Id = CreateGuid(51), ShortName = "WY", Name = "Wyoming"},
                             };
            manager.AttachEntities(states);

            for (int i = 1; i < 10; i++)
            {
                Guid primaryAddressId = CreateGuid(i);
                Guid primarePhoneId = CreateGuid(i);
                var resource = new Resource
                                   {
                                       Id = CreateGuid(i),
                                       FirstName = i % 2 == 0 ? "John" : "Jane",
                                       LastName = "Doe " + i,
                                       MiddleName = "M.",
                                       Summary =
                                           "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor " +
                                           "incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud " +
                                           "exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute " +
                                           "irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat " +
                                           "nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa " +
                                           "qui officia deserunt mollit anim id est laborum.",
                                   };
                manager.AttachEntity(resource);

                var addresses = new List<Address>
                                    {
                                        new Address
                                            {
                                                Id = primaryAddressId,
                                                AddressTypeId = CreateGuid(2),
                                                Address1 = i%10 + "123 Home Street",
                                                Address2 = "",
                                                City = "Hometown " + i,
                                                StateId = CreateGuid(9),
                                                Zipcode = "11111-0000",
                                                ResourceId = resource.Id,
                                                Primary = true
                                            },
                                        new Address
                                            {
                                                Id = CreateGuid(i + 10),
                                                AddressTypeId = CreateGuid(3),
                                                Address1 = i%10 + "123 Work Street",
                                                Address2 = "",
                                                City = "Worktown " + i,
                                                StateId = CreateGuid(9),
                                                Zipcode = "22222-0000",
                                                ResourceId = resource.Id
                                            }
                                    };
                manager.AttachEntities(addresses);

                var phoneNumbers = new List<PhoneNumber>
                                       {
                                           new PhoneNumber
                                               {
                                                   Id = primarePhoneId,
                                                   PhoneNumberTypeId = CreateGuid(1),
                                                   AreaCode = "555",
                                                   Number = i%10 + "110000",
                                                   ResourceId = resource.Id,
                                                   Primary = true
                                               },
                                           new PhoneNumber
                                               {
                                                   Id = CreateGuid(i + 10),
                                                   PhoneNumberTypeId = CreateGuid(2),
                                                   AreaCode = "555",
                                                   Number = i%10 + "220000",
                                                   ResourceId = resource.Id
                                               }
                                       };
                manager.AttachEntities(phoneNumbers);

                var rates = new List<Rate>
                                {
                                    new Rate
                                        {
                                            Id = CreateGuid(i + 10),
                                            RateTypeId = CreateGuid(1),
                                            Amount = 250,
                                            ResourceId = resource.Id
                                        },
                                    new Rate
                                        {
                                            Id = CreateGuid(i + 20),
                                            RateTypeId = CreateGuid(2),
                                            Amount = 2000,
                                            ResourceId = resource.Id
                                        },
                                    new Rate
                                        {
                                            Id = CreateGuid(i + 30),
                                            RateTypeId = CreateGuid(3),
                                            Amount = 10000,
                                            ResourceId = resource.Id
                                        },
                                    new Rate
                                        {
                                            Id = CreateGuid(i + 40),
                                            RateTypeId = CreateGuid(4),
                                            Amount = 40000,
                                            ResourceId = resource.Id
                                        }
                                };
                manager.AttachEntities(rates);

                var workExperienceItems = new List<WorkExperienceItem>
                                              {
                                                  new WorkExperienceItem
                                                      {
                                                          Id = CreateGuid(i + 10),
                                                          ResourceId = resource.Id,
                                                          From = new DateTime(2000, 1, 1),
                                                          To = new DateTime(2001, 12, 31),
                                                          Company = "Acme",
                                                          Location = "Acmetown, CA",
                                                          PositionTitle = "CEO",
                                                          Description =
                                                              "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor " +
                                                              "incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud " +
                                                              "exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute " +
                                                              "irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat " +
                                                              "nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa " +
                                                              "qui officia deserunt mollit anim id est laborum.",
                                                      },
                                                  new WorkExperienceItem
                                                      {
                                                          Id = CreateGuid(i + 20),
                                                          ResourceId = resource.Id,
                                                          From = new DateTime(2001, 1, 1),
                                                          To = new DateTime(2002, 12, 31),
                                                          Company = "Acme Holdings",
                                                          Location = "Acmetown, CA",
                                                          PositionTitle = "Vice President, Sales & Marketing",
                                                          Description =
                                                              "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor " +
                                                              "incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud " +
                                                              "exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute " +
                                                              "irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat " +
                                                              "nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa " +
                                                              "qui officia deserunt mollit anim id est laborum.",
                                                      }
                                              };
                manager.AttachEntities(workExperienceItems);

                var skills = new List<Skill>
                                 {
                                     new Skill {Id = CreateGuid(i + 10), Description = "Java", ResourceId = resource.Id},
                                     new Skill
                                         {
                                             Id = CreateGuid(i + 20),
                                             Description = "Microsoft Windows",
                                             ResourceId = resource.Id
                                         },
                                     new Skill {Id = CreateGuid(i + 30), Description = "C#", ResourceId = resource.Id},
                                     new Skill
                                         {
                                             Id = CreateGuid(i + 40),
                                             Description = "Vendor Management",
                                             ResourceId = resource.Id
                                         }
                                 };
                manager.AttachEntities(skills);
            }
        }
    }
}