//using DealEngine.Domain.Entities;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using System;
//using System.Collections.Generic;

//namespace DealEngine.WebUI.Models
//{
//    public class MarinaInterestedPartiesViewModel : BaseViewModel
//    {
//        public MarinaInterestedPartiesViewModel() //ClientInformationSheet clientInformationSheet, User OrgUser, User CurrentUser
//        {
//            OrganisationViewModel = new OrganisationViewModel(); //clientInformationSheet, OrgUser
//        }

//        //public IList<WaterLocation> WaterLocations { get; set; }

//        //public IList<Organisation> FinancialinterestedParties { get; set; }

//        public OrganisationViewModel OrganisationViewModel { get; set; }
//        public IEnumerable<OrganisationViewModel> MarinaLocations { get; set; }


//        // public IEnumerable<OrganisationViewModel> MarinaLocations { get; set; }

//        /*public MarinaInterestedPartiesViewModel(Domain.Entities.Programme programme)
//        {
//            //GenerateClientsOptions(programme);
//        }*/

//        /*private void GenerateClientsOptions(Domain.Entities.Programme programme)
//        {
//            Owners = new List<SelectListItem>();
//            Domain.Entities.Organisation org = null;
//            try
//            {
//                foreach (var owner in programme.ClientProgrammes)
//                {
//                    if (owner.Owner != null)
//                    {
//                        Owners.Add(
//                            new SelectListItem()
//                            {
//                                Text = owner.Owner.Name,
//                                Value = owner.Owner.Id.ToString()
//                            });
//                    }

//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//        }*/

//        /*public IList<SelectListItem> Owners { get; set; }*/

//    }
//}
