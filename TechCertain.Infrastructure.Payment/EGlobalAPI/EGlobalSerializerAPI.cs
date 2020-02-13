using System;
using System.IO;
using System.Xml.Serialization;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Infrastructure.Payment.EGlobalAPI.BaseClasses;

namespace TechCertain.Infrastructure.Payment.EGlobalAPI
{
    public class EGlobalSerializerAPI 
    {
        protected EGlobalPolicy EGlobalPolicy;
        protected EGlobalPolicyAPI EGlobalPolicyAPI;
        protected EGlobalAPI EGlobalAPI;
        protected string gv_strXML;
        protected string gv_strPaymentType;

        /****************************************
        * TODO - MUST BE OVERRIDDEN IN BASE CLASS
        ****************************************/
        protected virtual string Folder
        {
            get;
            private set;
        }

        /// <summary>
        /// Serializes the policy into an XML file, sends it to EGlobal, and stores a local copy
        /// </summary>
        /// <param name="objPolicy">Object policy.</param>
        public string SerializePolicy(ClientProgramme programme, User CurrentUser, IUnitOfWork _unitOfWork, Guid transactionreferenceid, string paymentType, bool reversetran, bool canceltran, 
            EGlobalSubmission originaleglobalsubmission)
        {
            string xml = "Failed to Serialize programme ";
            EGlobalAPI = new EGlobalAPI();
            try
            {
                foreach (Package package in programme.BaseProgramme.Packages)
                {
                    EGlobalPolicy = GetEGlobalXML(package, programme, CurrentUser);
                    if (reversetran && originaleglobalsubmission != null)
                    {
                        EGlobalPolicyAPI.CreateReversePolicyInvoice(originaleglobalsubmission);
                    } else if (canceltran) {
                        EGlobalPolicyAPI.CreateCancelPolicyInvoice(package, programme);
                    } else
                    {
                        EGlobalPolicyAPI.CreatePolicyInvoice();
                    }
                    
                    if (EGlobalPolicy != null)
                    {
                        EGlobalPolicy.PaymentType = paymentType; //Credit, Hunter, Invoice
                        xml = EGlobalPolicy.Serialize();
                        //removed for testing
                        //SaveXml(xml, EGlobalPolicy.FTPFolder);

                        //Save the request transaction
                        using (var uow = _unitOfWork.BeginUnitOfWork())
                        {
                            EGlobalSubmission eGlobalSubmission = new EGlobalSubmission(CurrentUser);
                            eGlobalSubmission.TransactionReferenceID = transactionreferenceid;
                            eGlobalSubmission.SubmissionRequestXML = xml;
                            eGlobalSubmission.EGlobalSubmissionPackage = package;
                            eGlobalSubmission.EGlobalPaymentType = paymentType;
                            eGlobalSubmission.EGlobalSubmissionClientProgramme = programme;
                            programme.ClientAgreementEGlobalSubmissions.Add(eGlobalSubmission);
                            //save eglobal submission term
                            EGlobalPolicyAPI.SaveTransactionTerms(eGlobalSubmission, _unitOfWork);

                            uow.Commit();

                        }

                    }
                    else
                        throw new ArgumentNullException(nameof(EGlobalPolicy));
                }
            }
            catch (Exception ex)
            {
                xml += ex.InnerException + " "+ ex.StackTrace;
            }

            return xml;
        }

        public string DeSerializeResponse(string byteResponse, ClientProgramme programme, User CurrentUser, IUnitOfWork _unitOfWork, EGlobalSubmission eglobalsubmission)
        {
            string xml = "Failed to Deserialize programme";            
            try
            {
                xml = EGlobalAPI.ProcessAsyncResult(byteResponse, programme, CurrentUser, _unitOfWork, eglobalsubmission);
            }
            catch (Exception ex)
            {
                xml += ex.InnerException + " " + ex.StackTrace;
            }

            

            return xml;
        }

        public void ReversePolicy(Package package, ClientProgramme programme, User CurrentUser)
        {
            EGlobalPolicy = GetEGlobalXML(package, programme, CurrentUser);


            Folder = EGlobalPolicy.FTPFolder;

            var strFileName = GetPolicyFileLocation(EGlobalPolicy.FTPFolder);

            var doc = new System.Xml.XmlDocument();
            doc.Load(strFileName);

            XmlSerializer serializer = new XmlSerializer(typeof(EGlobalPolicy));
            EGlobalPolicyAPI = (EGlobalPolicyAPI)serializer.Deserialize(new StringReader(doc.InnerXml));

            // do cancel stuff here
            EGlobalPolicyAPI.CreateReversePolicyInvoice(EGlobalPolicy.PolicyRisks);
            //EGlobalPolicy.SaveTransaction();

            string xml = EGlobalPolicy.Serialize();

            doc.LoadXml(gv_strXML);
        }

        public void UpdatePolicy(Package package, ClientProgramme programme, User CurrentUser)
        {
            EGlobalPolicy = GetEGlobalXML(package, programme, CurrentUser);

            Folder = EGlobalPolicy.FTPFolder;

            EGlobalPolicy.PaymentType = "3";
            EGlobalPolicyAPI.CreatePolicyInvoice();

            //            gv_objXML.SaveTransaction();
            //
            string xml = EGlobalPolicy.Serialize();
            //TC_Shared.LogEvent(TC_Shared.EventType.Information, "EGlobal XML Invoice", xml);
            //
            //            SaveXml(xml, gv_objXML.FTPFolder);
            //            SendEGlobalPolicy(xml, gv_objPolicy);

            if (EGlobalPolicyAPI != null)
            {
            }

        }

        public void CancelPolicy(Package package, ClientProgramme programme, User CurrentUser)
        {
            EGlobalPolicy = GetEGlobalXML(package, programme, CurrentUser);

            EGlobalPolicyAPI.CreateCancelPolicyInvoice(package, programme);
            EGlobalPolicyAPI.SaveTransaction();

            string xml = EGlobalPolicy.Serialize();
            SaveXml(xml, EGlobalPolicy.FTPFolder);
        }

        public EGlobalPolicy LoadLastXML(ClientProgramme clientProgramme)
        {
            throw new Exception("unimplemented method - LoadLastXML");
            /*gv_objPolicy = quote;
            gv_objXML = GetEGlobalXML(quote);

            Folder = gv_objXML.FTPFolder;

            var strFileName = GetPolicyFileLocation(gv_objXML.FTPFolder);

            var doc = new System.Xml.XmlDocument();
            try
            {
                doc.Load(strFileName);

                XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Base_EGlobalPolicy));
                Base_EGlobalPolicy policy = (Base_EGlobalPolicy)serializer.Deserialize(new StringReader(doc.InnerXml));

                // call private set on public property
                typeof(Base_EGlobalPolicy).GetProperty("TCPolicy").SetValue(policy, quote, null);
                typeof(Base_EGlobalPolicy).GetMethod("Setup", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(policy, null);

                List<EGlobalPolicyRiskConfig> prConfig = (List<EGlobalPolicyRiskConfig>)
                    typeof(Base_EGlobalPolicy).GetField("gv_objPolicyRisksConfigs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(policy);

                foreach (var pr in policy.PolicyRisks)
                {
                    EGlobalPolicyRiskConfig prc = prConfig.SingleOrDefault(c => c.SubCover == pr.SubCoverString
                                                  && c.RiskCode == pr.RiskCode && c.MergeWithCOBID == Guid.Empty);
                    pr.TCClassOfBusiness = (prc != null) ? prc.ClassOfBusinessID : Guid.Empty;
                }

                //typeof(Base_EGlobalPolicy).GetConstructor(new[] { typeof(TCQuote) }).Invoke(new object[] { quote });

                return policy;
            }
            catch (FileNotFoundException)
            {
                //TC_Shared.LogEvent(TC_Shared.EventType.Bug, "Could not find file " + strFileName);
                return null;
            }*/
        }

        /****************************************
        * TODO - MUST BE OVERRIDDEN IN BASE CLASS
        ****************************************/
        /// <summary>
        /// Gets the completed EGLobal XML object for the current policy
        /// </summary>
        /// <returns>The EGlobal XML.</returns>
        public virtual EGlobalPolicy GetEGlobalXML(Package package, ClientProgramme programme, User CurrentUser)
        {
            EGlobalPolicy = new EGlobalPolicy(package, programme);
            EGlobalPolicyAPI = new EGlobalPolicyAPI(EGlobalPolicy, CurrentUser);
            EGlobalPolicyAPI.Setup();
            return EGlobalPolicy;
        }

        protected void SaveXml(string xml, string folder)
        {
            try
            {
                var strFileName = GetPolicyFileLocation(folder);
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);

                if (System.IO.File.Exists(strFileName))
                    System.IO.File.Delete(strFileName);

                doc.Save(strFileName);

                // attempt to set permissions
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to save Xml file " + ex.GetBaseException().ToString());
            }
        }

        string GetPolicyFileLocation(string folder)
        {
            string strFileName = "";
            try
            {
                //Check/create the directory for the scheme
                var strSchemePath = Path.Combine("C:\tmp", folder);
                if (!Directory.Exists(strSchemePath))
                {
                    Directory.CreateDirectory(strSchemePath);
                }
                //Check/create the directory for the day
                var strDayPath = Path.Combine(strSchemePath, (DateTime.Now).ToString("yyyy-MM-dd"));//((DateTime)EGlobalPolicy.ClientProgramme.LastModifiedOn).ToString("yyyy-MM-dd"));
                if (!Directory.Exists(strDayPath))
                {
                    Directory.CreateDirectory(strDayPath);
                }
                //Save the file into the directory for today
                strFileName = Path.Combine(strDayPath, Folder + "_Policy_" + EGlobalPolicy.EGlobalPolicyId.ToString() + ".xml");
            }
            catch (IOException e)
            {
                throw new Exception("Cannot get XML file location " + e.GetBaseException().ToString());
            }
            return strFileName;
        }

    }
}

