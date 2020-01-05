using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;

namespace TechCertain.Services.Impl
{
    public class ImportService : IImportService
    {
        public Task ImportAOEService()
        {
            var userFileName = "C:\\tmp\\NZACS Individuals Demo.csv";
            var currentUser = await CurrentUser();
            StreamReader reader;
            User user;
            Organisation organisation;
            ClaimNotification claimNotification;
            BusinessContract businessContract;
            bool readFirstLine = false;
            string line;
            using (reader = new StreamReader(userFileName))
            {
                while (!reader.EndOfStream)
                {
                    if (!readFirstLine)
                    {
                        line = reader.ReadLine();
                        readFirstLine = true;
                    }
                    line = reader.ReadLine();
                    string[] parts = line.Split(',');

                    if (parts[0] == "f")
                    {
                        organisation = await _organisationService.GetOrganisationByEmail(parts[4]);

                        if (organisation == null)
                        {
                            organisation = new Organisation(currentUser, Guid.NewGuid());
                            await _organisationService.CreateNewOrganisation(organisation);
                        }

                        user = new User(currentUser, Guid.NewGuid(), parts[8]);
                        user.FirstName = parts[2];
                        user.LastName = parts[3];
                        user.Email = parts[4];
                        user.Address = parts[5];
                        user.MembershipNo = parts[7];

                        if (!user.Organisations.Contains(organisation))
                            user.Organisations.Add(organisation);

                        user.SetPrimaryOrganisation(organisation);
                        await _userRepository.AddAsync(user);

                    }

                }
            }
            readFirstLine = false;
            var principalsFileName = "C:\\tmp\\NZACS Principals Data Demo.csv";
            using (reader = new StreamReader(principalsFileName))
            {
                while (!reader.EndOfStream)
                {
                    if (!readFirstLine)
                    {
                        line = reader.ReadLine();
                        readFirstLine = true;
                    }
                    line = reader.ReadLine();
                    string[] parts = line.Split(',');

                    user = await _userService.GetUserByMembershipNo(parts[1]);
                    organisation = new Organisation(currentUser, Guid.Parse(parts[0]), parts[2]);
                    organisation.NZIAmembership = parts[0];
                    organisation.Email = parts[5];
                    if (!string.IsNullOrEmpty(parts[6]))
                    {
                        organisation.Qualifications = parts[6];
                    }
                    if (!string.IsNullOrEmpty(parts[7]))
                    {
                        if (parts[7] == "1")
                        {
                            organisation.IsNZIAmember = true;
                        }
                        else
                            organisation.IsNZIAmember = false;
                    }
                    if (!string.IsNullOrEmpty(parts[8]))
                    {
                        if (parts[8] == "1")
                        {
                            organisation.IsADNZmember = true;
                        }
                        else
                            organisation.IsADNZmember = false;
                    }
                    //clarify correct field
                    if (!string.IsNullOrEmpty(parts[9]))
                    {
                        if (parts[9] == "1")
                        {
                            organisation.IsOtherdirectorship = true;
                        }
                        else
                            organisation.IsOtherdirectorship = false;
                    }

                    await _organisationService.CreateNewOrganisation(organisation);

                }
            }

            readFirstLine = false;
            var claimFileName = "C:\\tmp\\NZACS Claims Data Demo.csv";
            using (reader = new StreamReader(claimFileName))
            {
                while (!reader.EndOfStream)
                {
                    if (!readFirstLine)
                    {
                        line = reader.ReadLine();
                        readFirstLine = true;
                    }
                    line = reader.ReadLine();
                    string[] parts = line.Split(',');
                    claimNotification = new ClaimNotification(currentUser);
                    claimNotification.ClaimTitle = parts[1];
                    claimNotification.ClaimReference = parts[2];
                    claimNotification.ClaimNotifiedDate = DateTime.Parse(parts[3]);
                    claimNotification.Claimant = parts[4];
                    claimNotification.ClaimStatus = parts[5];

                    //figure out how to save
                }
            }

            readFirstLine = false;
            var contractFileName = "C:\\tmp\\NZACS Contractors Principals Demo.csv";
            using (reader = new StreamReader(contractFileName))
            {
                while (!reader.EndOfStream)
                {
                    if (!readFirstLine)
                    {
                        line = reader.ReadLine();
                        readFirstLine = true;
                    }
                    line = reader.ReadLine();
                    string[] parts = line.Split(',');
                    businessContract = new BusinessContract(currentUser);
                    businessContract.ContractTitle = parts[1];
                    businessContract.Year = parts[0];
                    businessContract.ConstructionValue = parts[2];
                    businessContract.Fees = parts[3];

                    //figure out how to save
                }
            }
        }
    }
}

