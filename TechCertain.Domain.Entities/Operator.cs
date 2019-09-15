//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TechCertain.Domain.Entities.Abstracts;

//namespace TechCertain.Domain.Entities
//{
//    public class Operator : EntityBase, IAggregateRoot
//    {
//        protected Operator() : base(null) { }

//        public Operator(User createdBy, string operatorFirstName, string operatorLastName)
//            : base(createdBy)
//        {
//            OperatorFirstName = operatorFirstName;
//            OperatorLastName = operatorLastName;
//        }

//        public virtual Operator OriginalOperator
//        {
//            get;
//            protected set;
//        }

//        public virtual ClientInformationSheet ClientInformationSheet
//        {
//            get;
//            set;
//        }

//        public virtual string OperatorFirstName
//        {
//            get;
//            set;
//        }

//        public virtual string OperatorLastName
//        {
//            get;
//            set;
//        }

//        public virtual string OperatorDocumentType
//        {
//            get;
//            set;
//        }

//        public virtual string OperatorDocumentNumber
//        {
//            get;
//            set;
//        }

//        public virtual string OperatorYearsOfExp
//        {
//            get;
//            set;
//        }

//        public virtual string OperatorNotes
//        {
//            get;
//            set;
//        }

//        public virtual Boat BoatOperation
//        {
//            get;
//            set;
//        }

//        public virtual Vehicle VehicleOperation
//        {
//            get;
//            set;
//        }
        
//        public virtual bool Removed
//        {
//            get;
//            set;
//        }

//        public virtual DateTime OperatorDocumentEffectiveDate
//        {
//            get;
//            set;
//        }

//        public virtual DateTime OperatorDocumentExpiryDate
//        {
//            get;
//            set;
//        }
       

//        public virtual Operator CloneForNewSheet(ClientInformationSheet newSheet)
//        {
//            if (ClientInformationSheet == newSheet)
//                throw new Exception("Cannot clone operator for original information");

//            Operator newOperator = new Operator(newSheet.CreatedBy, OperatorFirstName, OperatorLastName);
//            newOperator.OperatorDocumentType = OperatorDocumentType;
//            newOperator.OperatorDocumentNumber = OperatorDocumentNumber;
//            newOperator.OperatorYearsOfExp = OperatorYearsOfExp;
//            newOperator.OperatorNotes = OperatorNotes;
//            newOperator.BoatOperation = newSheet.Boats.FirstOrDefault(ob => ob.OriginalBoat.Id == Id);
//            newOperator.VehicleOperation = newSheet.Vehicles.FirstOrDefault(ov => ov.OriginalVehicle.Id == Id);
//            if (OperatorDocumentEffectiveDate > DateTime.MinValue)
//                newOperator.OperatorDocumentEffectiveDate = OperatorDocumentEffectiveDate;
//            if (OperatorDocumentExpiryDate > DateTime.MinValue)
//                newOperator.OperatorDocumentExpiryDate = OperatorDocumentExpiryDate;

//            newOperator.OriginalOperator = this;
//            return newOperator;
//        }
//    }
//}

