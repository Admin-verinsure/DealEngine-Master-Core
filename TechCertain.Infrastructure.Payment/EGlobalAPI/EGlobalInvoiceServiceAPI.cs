using System;
using System.Net;
using System.ServiceModel;



namespace TechCertain.Infrastructure.Payment.EGlobalAPI
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Web.Services.WebServiceBindingAttribute(Name = "InvoiceServiceImplPortBinding", Namespace = "http://jws.ws.invoice.services.au.marsh.com/")]
    [ServiceContract(
        Namespace = "http://jws.ws.invoice.services.au.marsh.com/",
        Name = "InvoiceServiceImplPortBinding"
    )]

    public partial class EGlobalInvoiceServiceAPI : 
    {

        private System.Threading.SendOrPostCallback createInvoiceOperationCompleted;
        private System.Threading.SendOrPostCallback updateInvoiceOperationCompleted;
        private System.Threading.SendOrPostCallback getEGlobalSiteStatusOperationCompleted;

        private string gv_strUserID;
        private string gv_strPassword;
        private string site;

        /// <remarks/>
        public EGlobalInvoiceServiceAPI()
        {
            //    this.Url = "http://staging.ap.mrshmc.com:80/services/invoice/service";

            /*ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 |
                                                   SecurityProtocolType.Tls | SecurityProtocolType.Tls11;*/

            gv_strUserID = "tcwebservices";
            gv_strPassword = "xfmdpnf2";
            site = "https://staging.ap.marsh.com:19443/services/invoice/service"; //"https://eservicesdemo.proposalonline.com:4443/soap11";
            this.Credentials = new System.Net.NetworkCredential(gv_strUserID, gv_strPassword);
            this.Url = site;

            /*var demo = WebConfigurationManager.AppSettings["DemoEnvironment"];

            if (demo == "true")
            {
                DataTable dtApi = TC_Shared.GetAPIAccount(new Guid("e1d3c80c-2f97-4a73-8251-857584ef77ca"), new Guid("a27f88e8-8c29-11e2-abdc-0bcb3012a61e"));
                bool result = TC_Shared.FetchAPICredentials(dtApi, out gv_strUserID, out gv_strPassword, out site);
                if (result == false)
                {
                    TC_Shared.LogEvent(TC_Shared.EventType.UnhandledError, "No API account exists for key e1d3c80c-2f97-4a73-8251-857584ef77ca");
                }

                this.Url = "https://staging.ap.marsh.com:19443/services/invoice/service";
            }
            else
            {
                /*DataTable dtApi = TC_Shared.GetAPIAccount(new Guid("22b225c0-e8eb-4e9f-b567-4630879ff1a5"), new Guid("a27f88e8-8c29-11e2-abdc-0bcb3012a61e"));
                bool result = TC_Shared.FetchAPICredentials(dtApi, out gv_strUserID, out gv_strPassword, out site);
                if (result == false)
                {
                    TC_Shared.LogEvent(TC_Shared.EventType.UnhandledError, "No API account exists for key 22b225c0-e8eb-4e9f-b567-4630879ff1a5");
                }
                this.Credentials = new System.Net.NetworkCredential(gv_strUserID, gv_strPassword);
                this.Url = site;
                this.Url = "https://online.ap.marsh.com:19443/services/invoice/service";
            }*/
        }

        /// <remarks/>
        public event createInvoiceCompletedEventHandler createInvoiceCompleted;

        /// <remarks/>
        public event updateInvoiceCompletedEventHandler updateInvoiceCompleted;

        /// <remarks/>
        public event getEGlobalSiteStatusCompletedEventHandler getEGlobalSiteStatusCompleted;

        /// <remarks/>
        //[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.example.org/invoice-service/createInvoice", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        //[return: System.Xml.Serialization.XmlElementAttribute("createInvoiceResponse", Namespace = "http://www.example.org/invoice-service/", DataType = "base64Binary", IsNullable = true)]
        //public byte[] createInvoice([System.Xml.Serialization.XmlElementAttribute("createInvoice", Namespace = "http://www.example.org/invoice-service/", IsNullable = true)] createInvoiceType createInvoice1)
        //{
        //    object[] results = this.Invoke("createInvoice", new object[] {
        //        createInvoice1
        //    });
        //    return ((byte[])(results[0]));
        //}

        ///// <remarks/>
        //public System.IAsyncResult BegincreateInvoice(createInvoiceType createInvoice1, System.AsyncCallback callback, object asyncState)
        //{
        //    return this.BeginInvoke("createInvoice", new object[] {
        //        createInvoice1
        //    }, callback, asyncState);
        //}

        ///// <remarks/>
        //public byte[] EndcreateInvoice(System.IAsyncResult asyncResult)
        //{
        //    object[] results = this.EndInvoke(asyncResult);
        //    return ((byte[])(results[0]));
        //}

        ///// <remarks/>
        //public void createInvoiceAsync(createInvoiceType createInvoice1)
        //{
        //    this.createInvoiceAsync(createInvoice1, null);
        //}

        ///// <remarks/>
        //public void createInvoiceAsync(createInvoiceType createInvoice1, object userState)
        //{
        //    if ((this.createInvoiceOperationCompleted == null))
        //    {
        //        this.createInvoiceOperationCompleted = new System.Threading.SendOrPostCallback(this.OncreateInvoiceOperationCompleted);
        //    }
        //    this.InvokeAsync("createInvoice", new object[] {
        //        createInvoice1
        //    }, this.createInvoiceOperationCompleted, userState);
        //}

        //private void OncreateInvoiceOperationCompleted(object arg)
        //{
        //    if ((this.createInvoiceCompleted != null))
        //    {
        //        System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
        //        this.createInvoiceCompleted(this, new createInvoiceCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        //    }
        //}

        ///// <remarks/>
        //[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.example.org/invoice-service/updateInvoice", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        //[return: System.Xml.Serialization.XmlElementAttribute("out", Namespace = "http://www.example.org/invoice-service/", IsNullable = true)]
        //public string updateInvoice([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.example.org/invoice-service/", IsNullable = true)] string @in)
        //{
        //    object[] results = this.Invoke("updateInvoice", new object[] {
        //        @in
        //    });
        //    return ((string)(results[0]));
        //}

        ///// <remarks/>
        //public System.IAsyncResult BeginupdateInvoice(string @in, System.AsyncCallback callback, object asyncState)
        //{
        //    return this.BeginInvoke("updateInvoice", new object[] {
        //        @in
        //    }, callback, asyncState);
        //}

        ///// <remarks/>
        //public string EndupdateInvoice(System.IAsyncResult asyncResult)
        //{
        //    object[] results = this.EndInvoke(asyncResult);
        //    return ((string)(results[0]));
        //}

        ///// <remarks/>
        //public void updateInvoiceAsync(string @in)
        //{
        //    this.updateInvoiceAsync(@in, null);
        //}

        ///// <remarks/>
        //public void updateInvoiceAsync(string @in, object userState)
        //{
        //    if ((this.updateInvoiceOperationCompleted == null))
        //    {
        //        this.updateInvoiceOperationCompleted = new System.Threading.SendOrPostCallback(this.OnupdateInvoiceOperationCompleted);
        //    }
        //    this.InvokeAsync("updateInvoice", new object[] {
        //        @in
        //    }, this.updateInvoiceOperationCompleted, userState);
        //}

        //private void OnupdateInvoiceOperationCompleted(object arg)
        //{
        //    if ((this.updateInvoiceCompleted != null))
        //    {
        //        System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
        //        this.updateInvoiceCompleted(this, new updateInvoiceCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        //    }
        //}

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.example.org/invoice-service/getEGlobalSiteStatus", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("getEGlobalSiteStatusResponse", Namespace = "http://www.example.org/invoice-service/", IsNullable = true)]
        public string getEGlobalSiteStatus([System.Xml.Serialization.XmlElementAttribute("getEGlobalSiteStatus", Namespace = "http://www.example.org/invoice-service/", IsNullable = true)] getEGlobalSiteStatus getEGlobalSiteStatus1)
        {

            object[] results = this.Invoke("getEGlobalSiteStatus", new object[] {
                getEGlobalSiteStatus1
            });
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BegingetEGlobalSiteStatus(getEGlobalSiteStatus getEGlobalSiteStatus1, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getEGlobalSiteStatus", new object[] {
                getEGlobalSiteStatus1
            }, callback, asyncState);
        }

        /// <remarks/>
        public string EndgetEGlobalSiteStatus(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void getEGlobalSiteStatusAsync(getEGlobalSiteStatus getEGlobalSiteStatus1)
        {
            this.getEGlobalSiteStatusAsync(getEGlobalSiteStatus1, null);
        }

        /// <remarks/>
        public void getEGlobalSiteStatusAsync(getEGlobalSiteStatus getEGlobalSiteStatus1, object userState)
        {
            if ((this.getEGlobalSiteStatusOperationCompleted == null))
            {
                this.getEGlobalSiteStatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetEGlobalSiteStatusOperationCompleted);
            }
            this.InvokeAsync("getEGlobalSiteStatus", new object[] {
                getEGlobalSiteStatus1
            }, this.getEGlobalSiteStatusOperationCompleted, userState);
        }

        private void OngetEGlobalSiteStatusOperationCompleted(object arg)
        {
            if ((this.getEGlobalSiteStatusCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getEGlobalSiteStatusCompleted(this, new getEGlobalSiteStatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.example.org/billingdesktop/")]
    public partial class createInvoiceType
    {

        private string xmlStrField;

        private string siteField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string xmlStr
        {
            get
            {
                return this.xmlStrField;
            }
            set
            {
                this.xmlStrField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string site
        {
            get
            {
                return this.siteField;
            }
            set
            {
                this.siteField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.example.org/invoice-service/")]
    public partial class getEGlobalSiteStatus
    {

        private string siteField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string site
        {
            get
            {
                return this.siteField;
            }
            set
            {
                this.siteField = value;
            }
        }
    }

    /// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    public delegate void createInvoiceCompletedEventHandler(object sender, createInvoiceCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class createInvoiceCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal createInvoiceCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public byte[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    public delegate void updateInvoiceCompletedEventHandler(object sender, updateInvoiceCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class updateInvoiceCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal updateInvoiceCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    public delegate void getEGlobalSiteStatusCompletedEventHandler(object sender, getEGlobalSiteStatusCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getEGlobalSiteStatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal getEGlobalSiteStatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}