using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace TechCertain.Infrastructure.Payment.EGlobalAPI
{
    public class EGlobalAPI
    {

        #region API fields
      
        /// <summary>
        /// Processes the Async result.
        /// </summary>
        /// <param name="result">The Async result.</param>
        public void ProcessAsyncResult(string res)
        {
            // adjust the response
            int start = res.IndexOf("billingdesktop/\">") + 17;
            int to = res.IndexOf("</ns3:createInvoiceResponse>", start);
            var subString = res.Substring(start, to - start);
            // decode string to Base64
            byte[] byteStream = Convert.FromBase64String(subString);
            ASyncInvoice = Encoding.UTF8.GetString(byteStream, 0, byteStream.Length);
            EGlobalXmlResponse xo = GetResponseClass(ASyncInvoice);
                        
            //// process it
            ProcessResponse(xo);

            // indicate we have received a response
            ASyncInvoiceRecieved = true;
        }

        public bool ASyncInvoiceRecieved
        {
            get;
            set;
        }

        public string ASyncInvoice
        {
            get;
            set;
        }

        /// <summary>
        /// Deserializes the returned XML into a C# object
        /// </summary>
        /// <returns>The equivalent C# class</returns>
        /// <param name="xml">The response xml</param>
        public EGlobalXmlResponse GetResponseClass(string xml)
        {
            XmlSerializer xs = new XmlSerializer(typeof(EGlobalXmlResponse));
            StringReader sr = new StringReader(xml);
            EGlobalXmlResponse xo = (EGlobalXmlResponse)xs.Deserialize(sr);
            return xo;
        }

        #endregion

        private void ProcessResponse(EGlobalXmlResponse xo)
        {            
        
            //try
            //{
            //    // process the response
            //    string key = "";
            //    if (xo.Update != null)
            //        key = xo.Update.UpdExtSysKey;
            //    else if (xo.Error != null)
            //    {
            //        key = xo.Error.ExtSysKey;
            //        // check for API failure/crash errors
            //        //TC_Shared.LogEvent(TC_Shared.EventType.API_Bug, "ELink API billing failure", xo.XmlSerializeToString());
            //    }
            //    string[] sysKeys = key.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //    key = sysKeys[1].Split(new char[] { '-' }, StringSplitOptions.None)[1];
            //    Product product = new Product();
            //    IList<Product> subProducts;

            //    // save the response
            //    Guid responseID;
            //    SaveInvoiceData(xo, product.Id, out responseID);
            //    // determine the original transaction
            //    Guid invoiceID = Base_EGlobalPolicy.GetInvoiceID(sysKeys[1], TC_Shared.CNullInt(sysKeys[2], 0));
            //    Base_EGlobalPolicy.SetResponseID(responseID, invoiceID);

            //    // store the invoice number if it is an update
            //    if (xo.Update != null)
            //    {
            //        product = new Product(Base_EGlobalPolicy.GetQuoteID(sysKeys[1], TC_Shared.CNullInt(sysKeys[2], 0)));

            //        product.SetExternalInvoiceNumber(xo.Update.InvoiceNo.ToString());

            //        //Set quote status to bound and allow it gets updated laster
            //        product.SetWorkflowID(10);

            //        // also check to see if it is a cancel
            //        if (xo.Update.UpdTranCode == "C" && product.Product.UsesInsuranceSystemAPI)
            //        {
            //            TCInsuranceSystemPolicyCancel objInsuranceSystemPolicyCancel = TCInsuranceSystemPolicyCancel.GetCancelPolicy(product.Id);
            //            TCInsuranceSystem objInsuranceSystem = TCInsuranceSystem.GetInsuranceSystem(product.SchemeProject.Product.InsuranceSystemID);
            //            objInsuranceSystem.SubmitConfirmPolicyCancelation(objInsuranceSystemPolicyCancel);

            //            quote.SchemeProject.LogSchemeProjectHistory(quote.BrokerUser.ID, "Comfirm a Policy Cancellation.", quote.Proposal.CompanyName);
            //            quote.SetWorkflowID(25);
            //        }

            //        // if guidewire, send bind request
            //        if (product.Product.UsesInsuranceSystemAPI && (xo.Update.UpdTranCode == "N" || xo.Update.UpdTranCode == "E" || xo.Update.UpdTranCode == "R"))
            //        {
            //            try
            //            {
            //                if (!product.ExternalPolicy)
            //                {
            //                    product.SendExternalBindQuoteRequest();

            //                    product.SetWorkflowID((int)TCQuote.TCWorkflowStatus.Invoiced_Pending_Bind);

            //                    new Thread(new ThreadStart(delegate ()
            //                    {
            //                        Thread.Sleep(new TimeSpan(0, 3, 0));
            //                        TCQuote objTQuote = new TCQuote(quote.ID);

            //                        if (!objTQuote.ExternalPolicy)
            //                        {
            //                            //Send Emails
            //                            objTQuote.SendExternalBindFailedToAgency();
            //                            objTQuote.SendExternalBindFailedToBroker();
            //                        }
            //                    })).Start();
            //                }

            //            }
            //            catch (Exception ex)
            //            {
            //                TC_Shared.LogEvent(TC_Shared.EventType.API_Bug, "Something went wrong trying to bind a policy, try binding it manually.");
            //            }
            //        }

            //        // determine the paymemnt type
            //        string type = quote.Proposal.GetPropData("PaymentMethod");
            //        // if HPF
            //        if (type == "2")
            //        {
            //            HunterPremiumFunding.HunterPremium hpf = new HunterPremiumFunding.HunterPremium(quote.ProductID);
            //            hpf.LoadHunterRecord(quote.ID);
            //            hpf.SetInvoiceNumber(quote.ExternalInvoiceNumber);
            //            hpf.SaveHunterRecord();
            //        }

            //        // produce policy documents
            //        if (!quote.Product.UsesInsuranceSystemAPI)
            //        {
            //            quote.DeleteFilesNotInBoundCOBs(quote.Proposal.ContactUser);
            //            //Render policy documents
            //            try
            //            {
            //                quote.RenderPolicyDocuments(quote.Proposal.ContactUser);
            //            }
            //            catch (Exception ex)
            //            {
            //                TC_Shared.LogEvent(TC_Shared.EventType.Bug, String.Format("Error while rendering documents for quote reference {0}", quote.ReferenceID.ToString()), ex.GetBaseException().ToString());
            //            }

            //            //Remove policy documents for custom products
            //            quote.RemoveDocuments();

            //            //Send the Policy documents
            //            if (quote.SchemeProject.MailTemplate_PolicyDocumentsCoveringText.IsValid)
            //            {
            //                if (!quote.SchemeProject.SendPolicyDocuments(quote.Proposal.ContactUser, quote))
            //                {
            //                    TC_Shared.LogEvent(TC_Shared.EventType.Bug, "Unable to send documents for quote: " + quote.ReferenceID.ToString());
            //                }
            //            }
            //            else
            //            {
            //                TC_Shared.LogEvent(TC_Shared.EventType.Warning, "Policy Documents Email not configured - automatic issue on acceptance failed", quote.ID.ToString());
            //            }

            //            subProducts = quote.GetSecondaryQuotes();

            //            foreach (TCQuote q in subProducts)
            //            {
            //                if (q != null)
            //                {
            //                    q.SetExternalInvoiceNumber(xo.Update.InvoiceNo.ToString());

            //                    q.RenderPolicyDocuments(q.Proposal.ContactUser);

            //                    if (q.SchemeProject.MailTemplate_PolicyDocumentsCoveringText.IsValid)
            //                    {
            //                        if (!q.SchemeProject.SendPolicyDocuments(q.Proposal.ContactUser, q))
            //                        {
            //                            TC_Shared.LogEvent(TC_Shared.EventType.Bug, "Unable to send documents for quote: " + q.ReferenceID.ToString());
            //                        }
            //                    }
            //                    else
            //                    {
            //                        TC_Shared.LogEvent(TC_Shared.EventType.Warning, "Policy Documents Email not configured - automatic issue on acceptance failed", q.ID.ToString());
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // Eglobal Invoice Failed 
            //        // Unbund quote
            //        // Send notification to client

            //        if (quote.Policy && string.IsNullOrEmpty(quote.ExternalInvoiceNumber))
            //        {
            //            quote.UnBindQuote(quote.BrokerUser);

            //            //                        try
            //            //                        {
            //            //                            TCMail objmail = new TCMail();
            //            //                    
            //            //                            objmail.ToAddress = quote.BrokerUser.Email;
            //            //
            //            //                            objmail.FromName = "Proposalonline";
            //            //                            objmail.ReplyToAddress = "support@techcertain.com";
            //            //                    
            //            //                            objmail.Subject = "EGlobal Invoice Failed for " + quote.ReferenceID;
            //            //                    
            //            //                            string strBody = "Dear " + quote.BrokerUser.FirstName + "," + 
            //            //                                Constants.vbNewLine + 
            //            //                                Constants.vbNewLine + 
            //            //                                "EGlobal Invoice Failed for " + quote.ReferenceID;
            //            //                    
            //            //                            objmail.Body = strBody;
            //            //                    
            //            //                            objmail.Send();
            //            //                        } catch (Exception ex)
            //            //                        {
            //            //                            TC_Shared.LogEvent(TC_Shared.EventType.Bug, "There was an error trying to send out an email.", "EGlobal Invoice Failed for " + quote.ReferenceID);
            //            //                        }
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    TC_Shared.LogEvent(TC_Shared.EventType.API_Bug, "Error in EGlobal API response", ex.GetBaseException().ToString());
            //}
        }

        /*static EGlobalXmlResponse ProcessXMLResponseObject(NpgsqlDataReader dr, EGlobalXmlResponse xo)
        {
            bool boolError = TC_Shared.CNullStr(dr["responsetype"]) == "error";
            if (boolError && (xo == null || xo.Update == null))
            {
                xo.Text = TC_Shared.CNullStr(dr["message"]);
                EGlobalResponseError objError = new EGlobalResponseError();
                objError.ExtSysKey = TC_Shared.CNullStr(dr["extsyskey"]);
                objError.ExtSysRef = TC_Shared.CNullStr(dr["extsysref"]);
                objError.Key = TC_Shared.CNullStr(dr["key"]);
                objError.TranCode = TC_Shared.CNullStr(dr["trancode"]);
                objError.Code = TC_Shared.CNullStr(dr["code"]);
                objError.Desc = TC_Shared.CNullStr(dr["description"]);
                objError.ExtSysInput = TC_Shared.CNullStr(dr["extsysinput"]);
                xo.Error = objError;
            }
            else if (!boolError)
            {
                EGlobalResponseUpdate objUpdate = new EGlobalResponseUpdate();
                objUpdate.UpdExtSysKey = TC_Shared.CNullStr(dr["extsyskey"]);
                objUpdate.UpdExtSysRef = TC_Shared.CNullStr(dr["extsysref"]);
                objUpdate.UpdKey = TC_Shared.CNullStr(dr["key"]);
                objUpdate.UpdTranCode = TC_Shared.CNullStr(dr["trancode"]);
                objUpdate.UpdCode = TC_Shared.CNullStr(dr["code"]);
                objUpdate.UpdDesc = TC_Shared.CNullStr(dr["description"]);
                objUpdate.UpdExtSysInput = TC_Shared.CNullStr(dr["extsysinput"]);
                objUpdate.Company = TC_Shared.CNullStr(dr["company"]);
                objUpdate.Branch = TC_Shared.CNullStr(dr["branch"]);
                objUpdate.ClientNo = TC_Shared.CNullInt(dr["clientnumber"], 0);
                objUpdate.CoverNo = TC_Shared.CNullInt(dr["covernumber"], 0);
                objUpdate.VersionNo = TC_Shared.CNullInt(dr["versionnumber"], 0);
                objUpdate.Tranident = TC_Shared.CNullStr(dr["tranident"]);
                objUpdate.InvoiceNo = TC_Shared.CNullInt(dr["invoicenumber"], 0);
                xo.Update = objUpdate;
            }
            return xo;
        }

        #region Save/Load

        private EGlobalXmlResponse GetInvoiceDataByAttribute(Guid ID, string attribute, bool latest)
        {
            EGlobalXmlResponse xo = new EGlobalXmlResponse();

            string strCmd = "";
            strCmd = @"SELECT * FROM tbleglobalinvoiceresponse WHERE " + attribute + " = @Id";
            if (latest)
                strCmd += " order by datecreated desc";

            using (NpgsqlConnection sqlConnection1 = TC_Shared.GetSqlConnection())
            {
                NpgsqlCommand sqlcmd = new NpgsqlCommand(strCmd, sqlConnection1);
                sqlcmd.Parameters.Add("@Id", NpgsqlTypes.NpgsqlDbType.Uuid).Value = ID;

                try
                {
                    sqlConnection1.Open();

                    using (var dr = sqlcmd.ExecuteReader())
                    {
                        if (latest && dr.Read())
                        {
                            xo = ProcessXMLResponseObject(dr, xo);
                        }
                        else
                        {
                            while (dr.Read())
                                xo = ProcessXMLResponseObject(dr, xo);
                        }
                    }
                }
                catch (Exception e)
                {
                    TC_Shared.LogEvent(TC_Shared.EventType.Bug, "Failed to load EGlobal response record with " + attribute +
                    ": " + ID.ToString(), e.ToString());
                }
                finally
                {
                    sqlConnection1.Close();
                }
            }
            return xo;
        }

        public EGlobalXmlResponse GetResponseInvoiceData(Guid responseID)
        {
            return GetInvoiceDataByAttribute(responseID, "responseID", false);
        }

        public EGlobalXmlResponse LoadInvoiceData(Guid quoteID)
        {
            return GetInvoiceDataByAttribute(quoteID, "quoteID", false);
        }

        public EGlobalXmlResponse GetLatestInvoiceData(Guid quoteID)
        {
            return GetInvoiceDataByAttribute(quoteID, "quoteID", true);
        }

        public bool SaveInvoiceData(EGlobalXmlResponse xo, Guid quoteID, out Guid responseID)
        {
            bool success = false;
            responseID = Guid.Empty;
            if (xo == null)
                return success;
            using (NpgsqlConnection sqlConnection1 = TC_Shared.GetSqlConnection())
            {
                bool objError = xo.Error != null;

                string strSqlCmd = @"
INSERT INTO tbleglobalinvoiceresponse (responseid, quoteid, responsetype, datecreated, extsyskey, extsysref, key, trancode, code,
	description, extsysinput";
                if (!objError)
                    strSqlCmd += @", company, branch, clientnumber, covernumber, versionnumber, tranident, invoicenumber)";
                else
                    strSqlCmd += @", message)";
                strSqlCmd += @"VALUES (@ResponseID, @QuoteID, @ResponseType, @DateCreated, @ExtSysKey, @ExtSysRef, @Key, 
@TranCode, @Code, @Description, @ExtSysInput";
                if (!objError)
                    strSqlCmd += @", @Company, @Branch, @ClientNumber, @CoverNumber, @VersionNumber, @Tranident, @InvoiceNumber);";
                else
                    strSqlCmd += @", @Message);";

                NpgsqlCommand sqlcmd = new NpgsqlCommand(strSqlCmd, sqlConnection1);
                responseID = Guid.NewGuid();
                sqlcmd.Parameters.Add("@ResponseID", NpgsqlDbType.Uuid).Value = responseID;
                sqlcmd.Parameters.Add("@QuoteID", NpgsqlDbType.Uuid).Value = quoteID;
                sqlcmd.Parameters.Add("@DateCreated", NpgsqlDbType.Timestamp).Value = DateTime.Now;
                if (objError)
                {
                    // set error object paramaters
                    sqlcmd.Parameters.Add("@ResponseType", NpgsqlDbType.Varchar).Value = "error";
                    sqlcmd.Parameters.Add("@ExtSysKey", NpgsqlDbType.Varchar).Value = xo.Error.ExtSysKey;
                    sqlcmd.Parameters.Add("@ExtSysRef", NpgsqlDbType.Varchar).Value = xo.Error.ExtSysRef;
                    sqlcmd.Parameters.Add("@Key", NpgsqlDbType.Varchar).Value = xo.Error.Key;
                    sqlcmd.Parameters.Add("@TranCode", NpgsqlDbType.Varchar).Value = xo.Error.TranCode;
                    sqlcmd.Parameters.Add("@Code", NpgsqlDbType.Varchar).Value = xo.Error.Code;
                    sqlcmd.Parameters.Add("@Description", NpgsqlDbType.Varchar).Value = xo.Error.Desc;
                    sqlcmd.Parameters.Add("@ExtSysInput", NpgsqlDbType.Varchar).Value = xo.Error.ExtSysInput;
                    sqlcmd.Parameters.Add("@Message", NpgsqlDbType.Varchar).Value = xo.Text;
                }
                else
                {
                    // set update object parameters
                    sqlcmd.Parameters.Add("@ResponseType", NpgsqlDbType.Varchar).Value = "update";
                    sqlcmd.Parameters.Add("@ExtSysKey", NpgsqlDbType.Varchar).Value = xo.Update.UpdExtSysKey;
                    sqlcmd.Parameters.Add("@ExtSysRef", NpgsqlDbType.Varchar).Value = xo.Update.UpdExtSysRef;
                    sqlcmd.Parameters.Add("@Key", NpgsqlDbType.Varchar).Value = xo.Update.UpdKey;
                    sqlcmd.Parameters.Add("@TranCode", NpgsqlDbType.Varchar).Value = xo.Update.UpdTranCode;
                    sqlcmd.Parameters.Add("@Code", NpgsqlDbType.Varchar).Value = xo.Update.UpdCode;
                    sqlcmd.Parameters.Add("@Description", NpgsqlDbType.Varchar).Value = xo.Update.UpdDesc;
                    sqlcmd.Parameters.Add("@ExtSysInput", NpgsqlDbType.Varchar).Value = xo.Update.UpdExtSysInput;
                    sqlcmd.Parameters.Add("@Company", NpgsqlDbType.Varchar).Value = xo.Update.Company;
                    sqlcmd.Parameters.Add("@Branch", NpgsqlDbType.Varchar).Value = xo.Update.Branch;
                    sqlcmd.Parameters.Add("@ClientNumber", NpgsqlDbType.Integer).Value = xo.Update.ClientNo;
                    sqlcmd.Parameters.Add("@CoverNumber", NpgsqlDbType.Integer).Value = xo.Update.CoverNo;
                    sqlcmd.Parameters.Add("@VersionNumber", NpgsqlDbType.Integer).Value = xo.Update.VersionNo;
                    sqlcmd.Parameters.Add("@Tranident", NpgsqlDbType.Varchar).Value = xo.Update.Tranident;
                    sqlcmd.Parameters.Add("@InvoiceNumber", NpgsqlDbType.Integer).Value = xo.Update.InvoiceNo;
                }

                try
                {
                    sqlConnection1.Open();
                    sqlcmd.ExecuteNonQuery();
                    success = true;
                }
                catch (Exception e)
                {
                    TC_Shared.LogEvent(TC_Shared.EventType.Bug, "Failed to save EGlobal response record for quote: " + quoteID.ToString(), e.ToString());
                }
                finally
                {
                    sqlConnection1.Close();
                }
            }
            return success;
        }

        #endregion

        #region Testing

        public string GetSampleXml()
        {
            return GetSampleXml("test.xml");
        }

        public string GetSampleXml(string filename)
        {
            string file;
            string loc = "./Components/TCShared/EGlobal/EBIX/Testing/" + filename;
            //FileStream fs = new FileStream(loc, FileMode.Open);
            //BinaryReader reader = new BinaryReader(fs);
            //StreamReader reader = new StreamReader(loc, false);
            //file = reader.ReadString();
            file = File.ReadAllText(loc);
            return file;
        }

        //		public string UpdateInvoiceXML (string xml)
        //		{
        //			return service.updateInvoice (xml);
        //		}

        #endregion*/

        #region XML Serialization Objects

        [XmlRoot("XmlOutput")]
        public class EGlobalXmlResponse
        {
            [XmlAttribute("Version")]
            public string Version
            { get; set; }

            [XmlElement("Error")]
            public EGlobalResponseError Error
            { get; set; }

            [XmlElement("Update")]
            public EGlobalResponseUpdate Update
            { get; set; }

            [XmlText]
            public string Text
            { get; set; }

        }

        [XmlRoot("Error")]
        public class EGlobalResponseError
        {
            [XmlElement("ExtSysKey")]
            public string ExtSysKey
            { get; set; }

            [XmlElement("ExtSysRef")]
            public string ExtSysRef
            { get; set; }

            [XmlElement("Key")]
            public string Key
            { get; set; }

            [XmlElement("TranCode")]
            public string TranCode
            { get; set; }

            [XmlElement("Code")]
            public string Code
            { get; set; }

            [XmlElement("Desc")]
            public string Desc
            { get; set; }

            [XmlElement("ExtSysInput")]
            public string ExtSysInput
            { get; set; }
        }

        [XmlRoot("Update")]
        public class EGlobalResponseUpdate
        {
            [XmlElement("UpdExtSysKey")]
            public string UpdExtSysKey
            { get; set; }

            [XmlElement("UpdExtSysRef")]
            public string UpdExtSysRef
            { get; set; }

            [XmlElement("UpdKey")]
            public string UpdKey
            { get; set; }

            [XmlElement("UpdTranCode")]
            public string UpdTranCode
            { get; set; }

            [XmlElement("UpdCode")]
            public string UpdCode
            { get; set; }

            [XmlElement("UpdDesc")]
            public string UpdDesc
            { get; set; }

            [XmlElement("UpdExtSysInput")]
            public string UpdExtSysInput
            { get; set; }

            [XmlElement("_Company")]
            public string Company
            { get; set; }

            [XmlElement("_Branch")]
            public string Branch
            { get; set; }

            [XmlElement("_ClientNo")]
            public int ClientNo
            { get; set; }

            [XmlElement("_CoverNo")]
            public int CoverNo
            { get; set; }

            [XmlElement("_VersionNo")]
            public int VersionNo
            { get; set; }

            [XmlElement("_Tranident")]
            public string Tranident
            { get; set; }

            [XmlElement("_InvoiceNo")]
            public int InvoiceNo
            { get; set; }
        }

        #endregion
    }
}

