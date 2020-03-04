//using System;
//using System.Collections.Generic;
//using System.Web.Mvc;
//using DealEngine.Domain.Entities;

//namespace DealEngine.Web.UI.ViewModels
//{
//    public class OperatorViewModel : BaseViewModel
//    {
//        public Guid AnswerSheetId { get; set; }

//        public Guid OperatorId { get; set; }

//        public string OperatorFirstName { get; set; }

//        public string OperatorLastName { get; set; }

//        public string OperatorDocumentType { get; set; }

//        public string OperatorDocumentNumber { get; set; }

//        public string OperatorYearsOfExp { get; set; }

//        public string OperatorNotes { get; set; }

//        public Guid BoatOperation { get; set; }

//        public Guid VehicleOperation { get; set; }

//        public string OperatorDocumentEffectiveDate { get; set; }

//        public string OperatorDocumentExpiryDate { get; set; }

//        public Operator ToEntity(User creatingUser)
//        {
//            Operator operato = new Operator(creatingUser, OperatorFirstName, OperatorLastName);
//            UpdateEntity(operato);
//            return operato;
//        }

//        public Operator UpdateEntity(Operator operato)
//        {
//            operato.OperatorFirstName = OperatorFirstName;
//            operato.OperatorLastName = OperatorLastName;
//            operato.OperatorDocumentType = OperatorDocumentType;
//            operato.OperatorDocumentNumber = OperatorDocumentNumber;
//            operato.OperatorYearsOfExp = OperatorYearsOfExp;
//            operato.OperatorNotes = OperatorNotes;
//            if (!string.IsNullOrEmpty(OperatorDocumentEffectiveDate))
//            {
//                operato.OperatorDocumentEffectiveDate = DateTime.Parse(OperatorDocumentEffectiveDate, System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ"));
//            }
//            else
//            {
//                operato.OperatorDocumentEffectiveDate = DateTime.MinValue;
//            }
//            if (!string.IsNullOrEmpty(OperatorDocumentExpiryDate))
//            {
//                operato.OperatorDocumentExpiryDate = DateTime.Parse(OperatorDocumentExpiryDate, System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ"));
//            }
//            else
//            {
//                operato.OperatorDocumentExpiryDate = DateTime.MinValue;
//            }
//            return operato;
//        }

//        public static OperatorViewModel FromEntity(Operator operato)
//        {
//            OperatorViewModel model = new OperatorViewModel
//            {
//                OperatorId = operato.Id,
//                OperatorFirstName = operato.OperatorFirstName,
//                OperatorLastName = operato.OperatorLastName,
//                OperatorDocumentType = operato.OperatorDocumentType,
//                OperatorDocumentNumber = operato.OperatorDocumentNumber,
//                OperatorYearsOfExp = operato.OperatorYearsOfExp,
//                OperatorNotes = operato.OperatorNotes,
//                OperatorDocumentEffectiveDate = (operato.OperatorDocumentEffectiveDate > DateTime.MinValue) ? operato.OperatorDocumentEffectiveDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "",
//                OperatorDocumentExpiryDate = (operato.OperatorDocumentExpiryDate > DateTime.MinValue) ? operato.OperatorDocumentExpiryDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "",
//            };
//            return model;
//        }
//    }

//}