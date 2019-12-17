using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class HttpClientService : IHttpClientService
    {
        IMapperSession<LogInfo> _logInfoMapperSession;
        public HttpClientService(IMapperSession<LogInfo> logInfoMapperSession)
        {
            _logInfoMapperSession = logInfoMapperSession;
        }

        public async Task<string> Analyze(string request)
        {
            var responseMessage = "";
            string service = "https://ris.us1.qeadaptiveauth.com/AdaptiveAuthentication/services/AdaptiveAuthentication";
            string SOAPAction = "rsa:analyze:Analyze";

            SocketsHttpHandler _socketsHttpHandler;
            HttpRequestMessage _httpRequestMessage;
            HttpResponseMessage response;
            XmlSerializer serializer;
            StringReader rdr;
            RSAEnvelope result = new RSAEnvelope();

            _socketsHttpHandler = new SocketsHttpHandler()
            {
                Credentials = new NetworkCredential("MarshNZSOAPUser", "MarNZ0sa$0Cap16us"),
            };

            _httpRequestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(service),
                Method = HttpMethod.Post,
                Content = new StringContent(HardCodedRSABody(), Encoding.UTF8, "text/xml"),
            };
            _httpRequestMessage.Headers.Add("SOAPAction", SOAPAction);

            try
            {
                var logInfo = new LogInfo();
                logInfo.AnalyzeXMLRSA = "test save";
                //_logInfoMapperSession.AddAsync(logInfo);

                Console.WriteLine("Content: ");
                Console.WriteLine(HardCodedRSABody());
                Console.WriteLine("Timestamp: ");
                Console.WriteLine(DateTime.UtcNow.ToString());

                HttpClient client = new HttpClient(_socketsHttpHandler);
                response = await client.SendAsync(_httpRequestMessage);
                response.EnsureSuccessStatusCode();
                responseMessage = await response.Content.ReadAsStringAsync();
                var msg = ResposeMessage();
                serializer = new XmlSerializer(typeof(RSAEnvelope));
                rdr = new StringReader(msg);
                try
                {
                    result = (RSAEnvelope)serializer.Deserialize(rdr);
                }
                catch(Exception ex)
                {
                    throw ex;
                }

                client.Dispose();
            }
            catch (HttpRequestException e)
            {
                
                responseMessage = e.Message + " status code not 200";
                Console.WriteLine("Exception1: ");
                Console.WriteLine(responseMessage);
            }
            catch (Exception ex)
            {
                responseMessage = ex.Message;
                Console.WriteLine("Exception2: ");
                Console.WriteLine(responseMessage);
            }
            _socketsHttpHandler.Dispose();
            _httpRequestMessage.Dispose();

            Console.WriteLine("ResponseMessage: ");
            Console.WriteLine(responseMessage);

            return responseMessage;
        }

        public async Task<string> CreateEGlobalInvoice(string xmlPayload)
        {
            var responseMessage ="";            
            var SOAPAction = @"http://www.example.org/invoice-service/createInvoice";
            var service = "https://stg.eglobalinvp.marsh.com/services/invoice/service"; //"https://staging.ap.marsh.com:19443/services/invoice/service"; old staging end point
            var body = generateBody(xmlPayload);
            HttpResponseMessage response;
            SocketsHttpHandler _socketsHttpHandler;
            HttpRequestMessage _httpRequestMessage;

            _socketsHttpHandler = new SocketsHttpHandler()
            {
                Credentials = new NetworkCredential("tcwebservices", "xfmdpnf2"),
            };

            _httpRequestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(service),
                Method = HttpMethod.Post,
                Content = new StringContent(body, Encoding.UTF8, "text/xml"),
            };
            _httpRequestMessage.Headers.Add("SOAPAction", SOAPAction);

            try
            {
                HttpClient client = new HttpClient(_socketsHttpHandler);
                response = await client.SendAsync(_httpRequestMessage);
                Thread.Sleep(1000);
                response.EnsureSuccessStatusCode();
                responseMessage = await response.Content.ReadAsStringAsync();
                client.Dispose();
            }
            catch (HttpRequestException e)
            {
               responseMessage = e.Message + " status code not 200";
            }
            catch (Exception ex)
            {
               responseMessage = ex.Message + ex.InnerException + ex.StackTrace;
            }
            _socketsHttpHandler.Dispose();
            _httpRequestMessage.Dispose();

            return responseMessage;
        }

        public async Task<string> GetEglobalStatus()
        {
            var responseMessage = "";
            var SOAPAction = "http://www.example.org/invoice-service/getEGlobalSiteStatus";
            var service = "https://stg.eglobalinvp.marsh.com/services/invoice/service"; //"https://staging.ap.marsh.com:19443/services/invoice/service"; old staging end point
            var body = GenerateGetSiteActiveSoapBody();
            Envelope result = new Envelope();
            HttpResponseMessage response;
            SocketsHttpHandler _socketsHttpHandler;
            HttpRequestMessage _httpRequestMessage;
            XmlSerializer serializer; 
            StringReader rdr;           

            _socketsHttpHandler = new SocketsHttpHandler()
            {
                Credentials = new NetworkCredential("tcwebservices", "xfmdpnf2"),                
            };

            _httpRequestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(service),
                Method = HttpMethod.Post,
                Content = new StringContent(body, Encoding.UTF8, "text/xml"),
            };
            _httpRequestMessage.Headers.Add("SOAPAction", SOAPAction);

            try
            {
                HttpClient client = new HttpClient(_socketsHttpHandler);
                response = await client.SendAsync(_httpRequestMessage);
                response.EnsureSuccessStatusCode();
                responseMessage = await response.Content.ReadAsStringAsync();
                serializer = new XmlSerializer(typeof(Envelope));
                rdr = new StringReader(responseMessage);
                result = (Envelope)serializer.Deserialize(rdr);
                client.Dispose();
            }
            catch (HttpRequestException e)
            {
                responseMessage = e.Message + " status code not 200";
            }
            catch (Exception ex)
            {
                responseMessage = ex.Message;
            }

            _socketsHttpHandler.Dispose();
            _httpRequestMessage.Dispose();

            return result.Body.getEGlobalSiteStatusResponse;
        }

        private string generateBody(string xmlPayload)
        {
            //var formattedString = xmlPayload.Remove(0, 22);
            string htmlEncodedString = HttpUtility.HtmlEncode(xmlPayload);
            string body = @"<?xml version=""1.0"" encoding=""utf-8""?><soapenv:Envelope xmlns:soapenv = ""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:inv = ""http://www.example.org/invoice-service/"">    
                            <soapenv:Header/>     
                                <soapenv:Body>      
                                    <inv:createInvoice>       
                                        <xmlStr>{0}</xmlStr>       
                                        <site>NZL</site>       
                                    </inv:createInvoice>       
                                </soapenv:Body>
                            </soapenv:Envelope>";
            string strxml = string.Format(body, htmlEncodedString);
            
            return strxml;
        }
        private string GenerateGetSiteActiveSoapBody()
        {
            string body = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:inv=""http://www.example.org/invoice-service/"">
                <soapenv:Header/>
                    <soapenv:Body>
                        <inv:getEGlobalSiteStatus>
                            <site>NZL</site>
                        </inv:getEGlobalSiteStatus>    
                    </soapenv:Body>
                </soapenv:Envelope>";

            return body;
        }
        private string HardCodedRSABody()
        {
            return @"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                        <soap:Header>
                        <wsse:Security soap:mustUnderstand = ""1"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">     
                        <wsse:UsernameToken wsu:Id=""UsernameToken-bd15e0d7-37fa-4de8-8bd9-758caa95112c"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">         
                        <wsse:Username>MarshNZSOAPUser</wsse:Username>              
                        <wsse:Password Type = ""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">MarNZ0sa$0Cap16us</wsse:Password>                     
                        <wsse:Nonce EncodingType = ""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"">ufoWBVGZ+MgRHRcw5j0EQQ==</wsse:Nonce>                              
                        <wsu:Created>" + DateTime.UtcNow.ToString() + @"</wsu:Created>                                        
                        </wsse:UsernameToken>                                         
                        </wsse:Security>                                          
                        </soap:Header> 
                                             <soap:Body>                                            
                                                  <analyze xmlns = ""http://ws.csd.rsa.com"">                                             
                                                      <request>                                             
                                                         <actionTypeList>                                             
                                                            <genericActionTypes>SET_USER_STATUS</genericActionTypes>                                             
                                                            <genericActionTypes>SET_USER_GROUP</genericActionTypes>                                             
                                                         </actionTypeList>                                             
                                                         <deviceRequest>                                             
                                                            <devicePrint>version%3D3%2E5%2E1%5F4%26pm%5Ffpua%3Dmozilla%2F5%2E0%20%28windows%20nt%2010%2E0%3B%20win64%3B%20x64%3B%20rv%3A68%2E0%29%20gecko%2F20100101%20firefox%2F68%2E0%7C5%2E0%20%28Windows%29%7CWin32%26pm%5Ffpsc%3D24%7C1920%7C1080%7C1050%26pm%5Ffpsw%3D%26pm%5Ffptz%3D12%26pm%5Ffpln%3Dlang%3Den%2DUS%7Csyslang%3D%7Cuserlang%3D%26pm%5Ffpjv%3D0%26pm%5Ffpco%3D1%26pm%5Ffpasw%3Dnpswf64%5F32%5F0%5F0%5F223%26pm%5Ffpan%3DNetscape%26pm%5Ffpacn%3DMozilla%26pm%5Ffpol%3Dtrue%26pm%5Ffposp%3D%26pm%5Ffpup%3D%26pm%5Ffpsaw%3D1920%26pm%5Ffpspd%3D24%26pm%5Ffpsbd%3D%26pm%5Ffpsdx%3D%26pm%5Ffpsdy%3D%26pm%5Ffpslx%3D%26pm%5Ffpsly%3D%26pm%5Ffpsfse%3D%26pm%5Ffpsui%3D%26pm%5Fos%3DWindows%26pm%5Fbrmjv%3D68%26pm%5Fbr%3DFirefox%26pm%5Finpt%3D%26pm%5Fexpt%3D</devicePrint>                                                                                                                                                                                                                                                                                                        
                                                            <httpAccept/>
                                                            <httpAcceptEncoding/>
                                                            <httpAcceptLanguage/>                                                                                                                                                                                                                                                                                                        
                                                            <httpReferrer>https://staging.mydealslive.com/Account/Login?ReturnUrl=%2f</httpReferrer>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                                                            <ipAddress>172.30.1.245</ipAddress>                                                                                                                                                                                                                                                                                                           
                                                            </deviceRequest>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                                                            <identificationData>
                                                            <delegated>false</delegated>                                                                                                                                                                                                                                                                                                           
                                                            <groupName>Clients</groupName>                                                                                                                                                                                                                                                                                                           
                                                            <orgName>Marsh_Model</orgName>                                                                                                                                                                                                                                                                                                           
                                                            <userName>ray@techcertain.com</userName>                                                                                                                                                                                                                                                                                                              
                                                            <userStatus>VERIFIED</userStatus>                                                                                                                                                                                                                                                                                                              
                                                            <userType>PERSISTENT</userType>                                                                                                                                                                                                                                                                                                              
                                                            </identificationData>                                                                                                                                                                                                                                                                                                              
                                                            <messageHeader>                                                                                                                                                                                                                                                                                                              
                                                            <apiType>DIRECT_SOAP_API</apiType>                                                                                                                                                                                                                                                                                                              
                                                            <requestType>ANALYZE</requestType>                                                                                                                                                                                                                                                                                                              
                                                            <version>7.0</version>                                                                                                                                                                                                                                                                                                              
                                                            </messageHeader>                                                                                                                                                                                                                                                                                                              
                                                            <autoCreateUserFlag>true</autoCreateUserFlag>                                                                                                                                                                                                                                                                                                              
                                                            <credentialDataList>                                                                                                                                                                                                                                                                                                              
                                                            <acspAuthenticationRequestData/>                                                                                                                                                                                                                                                                                                              
                                                            </credentialDataList>                                                                                                                                                                                                                                                                                                              
                                                            <eventDataList>                                                                                                                                                                                                                                                                                                              
                                                            <eventData>                                                                                                                                                                                                                                                                                                              
                                                            <clientDefinedAttributeList>                                                                                                                                                                                                                                                                                                              
                                                            <fact>                                                                                                                                                                                                                                                                                                              
                                                            <name>FILTERED_TAM_GROUP</name>                                                                                                                                                                                                                                                                                                              
                                                            <value>MFA</value>                                                                                                                                                                                                                                                                                                              
                                                            <dataType>STRING</dataType>                                                                                                                                                                                                                                                                                                              
                                                            </fact>                                                                                                                                                                                                                                                                                                              
                                                            </clientDefinedAttributeList>                                                                                                                                                                                                                                                                                                              
                                                            <eventType>SESSION_SIGNIN</eventType>                                                                                                                                                                                                                                                                                                              
                                                            </eventData>                                                                                                                                                                                                                                                                                                              
                                                            </eventDataList>                                                                                                                                                                                                                                                                                                              
                                                            <runRiskType>ALL</runRiskType>                                                                                                                                                                                                                                                                                                              
                                                            <channelIndicator>WEB</channelIndicator>                                                                                                                                                                                                                                                                                                              
                                                            </request>                                                                                                                                                                                                                                                                                                              
                                                            </analyze>                                                                                                                                                                                                                                                                                                              
                                                            </soap:Body>
                                                            </soap:Envelope>";

        }
        private string ResposeMessage()
        {
            return "<?xml version = '1.0' encoding = 'utf-8'?><soapenv:Envelope xmlns:soapenv =\"http://schemas.xmlsoap.org/soap/envelope/\"><soapenv:Body><ns1:analyzeResponse xmlns:ns1=\"http://ws.csd.rsa.com\"><ns1:analyzeReturn xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"ns1:AnalyzeResponse\"><ns1:deviceResult><ns1:authenticationResult><ns1:authStatusCode>FAIL</ns1:authStatusCode><ns1:risk>0</ns1:risk></ns1:authenticationResult><ns1:callStatus><ns1:statusCode>SUCCESS</ns1:statusCode></ns1:callStatus><ns1:deviceData><ns1:bindingType>NONE</ns1:bindingType><ns1:deviceTokenCookie>PMV61pYQVSs1ggLnIxiqnJ4QWBmDrFX40bSP7yLHfUlDTrm7QS6XL5cgBPhG5LEqJ%2BSrUupdmaC%2BXoqzAFcglUWK0Sm02ONa297xBLee9YamXtzBk%3D</ns1:deviceTokenCookie><ns1:deviceTokenFSO>PMV61pYQVSs1ggLnIxiqnJ4QWBmDrFX40bSP7yLHfUlDTrm7QS6XL5cgBPhG5LEqJ%2BSrUupdmaC%2BXoqzAFcglUWK0Sm02ONa297xBLee9YamXtzBk%3D</ns1:deviceTokenFSO></ns1:deviceData></ns1:deviceResult><ns1:identificationData><ns1:delegated>false</ns1:delegated><ns1:groupName>Clients</ns1:groupName><ns1:orgName>Marsh_Model</ns1:orgName><ns1:sessionId>812834347167267526393-ksat-reganam-krow||1576541731958</ns1:sessionId><ns1:transactionId>TRX_work-manager-task-39-6652711369631216351</ns1:transactionId><ns1:userName>ray@techcertain.com</ns1:userName><ns1:userStatus>UNVERIFIED</ns1:userStatus><ns1:userType>PERSISTENT</ns1:userType></ns1:identificationData><ns1:messageHeader><ns1:apiType>DIRECT_SOAP_API</ns1:apiType><ns1:requestType>ANALYZE</ns1:requestType><ns1:timeStamp>2019-12-17T00:05:31.958Z</ns1:timeStamp><ns1:version>7.0</ns1:version></ns1:messageHeader><ns1:statusHeader><ns1:reasonCode>0</ns1:reasonCode><ns1:reasonDescription>Operations were completed successfully\n\n</ns1:reasonDescription><ns1:statusCode>200</ns1:statusCode></ns1:statusHeader><ns1:requiredCredentialList><ns1:requiredCredential><ns1:credentialType>USER_DEFINED</ns1:credentialType><ns1:genericCredentialType>OTP</ns1:genericCredentialType><ns1:groupName>DEFAULT</ns1:groupName><ns1:preference>0</ns1:preference><ns1:required>true</ns1:required></ns1:requiredCredential></ns1:requiredCredentialList><ns1:riskResult><ns1:riskScore>345</ns1:riskScore><ns1:riskScoreBand>SCORE_BAND_1</ns1:riskScoreBand><ns1:triggeredRule><ns1:actionCode>CHALLENGE</ns1:actionCode><ns1:actionName>User_Device_Not_Bound</ns1:actionName><ns1:actionType>STRICT</ns1:actionType><ns1:clientFactList/><ns1:ruleId>User_Device_Not_Bound</ns1:ruleId><ns1:ruleName>User_Device_Not_Bound</ns1:ruleName></ns1:triggeredRule><ns1:deviceAssuranceLevel>LOW</ns1:deviceAssuranceLevel></ns1:riskResult></ns1:analyzeReturn></ns1:analyzeResponse></soapenv:Body></soapenv:Envelope>";
        }
        #region RSA

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
        public partial class RSAEnvelope
        {

            public RSAEnvelopeBody evbodyField { get; set; }

            /// <remarks/>
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public partial class RSAEnvelopeBody
        {

            private analyzeResponse analyzeResponseField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://ws.csd.rsa.com")]
            public analyzeResponse analyzeResponse
            {
                get
                {
                    return this.analyzeResponseField;
                }
                set
                {
                    this.analyzeResponseField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.csd.rsa.com")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ws.csd.rsa.com", IsNullable = false)]
        public partial class analyzeResponse
        {

            private analyzeResponseAnalyzeReturn analyzeReturnField;

            /// <remarks/>
            public analyzeResponseAnalyzeReturn analyzeReturn
            {
                get
                {
                    return this.analyzeReturnField;
                }
                set
                {
                    this.analyzeReturnField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.csd.rsa.com")]
        public partial class analyzeResponseAnalyzeReturn
        {

            private analyzeResponseAnalyzeReturnDeviceResult deviceResultField;

            private analyzeResponseAnalyzeReturnIdentificationData identificationDataField;

            private analyzeResponseAnalyzeReturnMessageHeader messageHeaderField;

            private analyzeResponseAnalyzeReturnStatusHeader statusHeaderField;

            private analyzeResponseAnalyzeReturnRequiredCredentialList requiredCredentialListField;

            private analyzeResponseAnalyzeReturnRiskResult riskResultField;

            /// <remarks/>
            public analyzeResponseAnalyzeReturnDeviceResult deviceResult
            {
                get
                {
                    return this.deviceResultField;
                }
                set
                {
                    this.deviceResultField = value;
                }
            }

            /// <remarks/>
            public analyzeResponseAnalyzeReturnIdentificationData identificationData
            {
                get
                {
                    return this.identificationDataField;
                }
                set
                {
                    this.identificationDataField = value;
                }
            }

            /// <remarks/>
            public analyzeResponseAnalyzeReturnMessageHeader messageHeader
            {
                get
                {
                    return this.messageHeaderField;
                }
                set
                {
                    this.messageHeaderField = value;
                }
            }

            /// <remarks/>
            public analyzeResponseAnalyzeReturnStatusHeader statusHeader
            {
                get
                {
                    return this.statusHeaderField;
                }
                set
                {
                    this.statusHeaderField = value;
                }
            }

            /// <remarks/>
            public analyzeResponseAnalyzeReturnRequiredCredentialList requiredCredentialList
            {
                get
                {
                    return this.requiredCredentialListField;
                }
                set
                {
                    this.requiredCredentialListField = value;
                }
            }

            /// <remarks/>
            public analyzeResponseAnalyzeReturnRiskResult riskResult
            {
                get
                {
                    return this.riskResultField;
                }
                set
                {
                    this.riskResultField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.csd.rsa.com")]
        public partial class analyzeResponseAnalyzeReturnDeviceResult
        {

            private analyzeResponseAnalyzeReturnDeviceResultAuthenticationResult authenticationResultField;

            private analyzeResponseAnalyzeReturnDeviceResultCallStatus callStatusField;

            private analyzeResponseAnalyzeReturnDeviceResultDeviceData deviceDataField;

            /// <remarks/>
            public analyzeResponseAnalyzeReturnDeviceResultAuthenticationResult authenticationResult
            {
                get
                {
                    return this.authenticationResultField;
                }
                set
                {
                    this.authenticationResultField = value;
                }
            }

            /// <remarks/>
            public analyzeResponseAnalyzeReturnDeviceResultCallStatus callStatus
            {
                get
                {
                    return this.callStatusField;
                }
                set
                {
                    this.callStatusField = value;
                }
            }

            /// <remarks/>
            public analyzeResponseAnalyzeReturnDeviceResultDeviceData deviceData
            {
                get
                {
                    return this.deviceDataField;
                }
                set
                {
                    this.deviceDataField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.csd.rsa.com")]
        public partial class analyzeResponseAnalyzeReturnDeviceResultAuthenticationResult
        {

            private string authStatusCodeField;

            private byte riskField;

            /// <remarks/>
            public string authStatusCode
            {
                get
                {
                    return this.authStatusCodeField;
                }
                set
                {
                    this.authStatusCodeField = value;
                }
            }

            /// <remarks/>
            public byte risk
            {
                get
                {
                    return this.riskField;
                }
                set
                {
                    this.riskField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.csd.rsa.com")]
        public partial class analyzeResponseAnalyzeReturnDeviceResultCallStatus
        {

            private string statusCodeField;

            /// <remarks/>
            public string statusCode
            {
                get
                {
                    return this.statusCodeField;
                }
                set
                {
                    this.statusCodeField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.csd.rsa.com")]
        public partial class analyzeResponseAnalyzeReturnDeviceResultDeviceData
        {

            private string bindingTypeField;

            private string deviceTokenCookieField;

            private string deviceTokenFSOField;

            /// <remarks/>
            public string bindingType
            {
                get
                {
                    return this.bindingTypeField;
                }
                set
                {
                    this.bindingTypeField = value;
                }
            }

            /// <remarks/>
            public string deviceTokenCookie
            {
                get
                {
                    return this.deviceTokenCookieField;
                }
                set
                {
                    this.deviceTokenCookieField = value;
                }
            }

            /// <remarks/>
            public string deviceTokenFSO
            {
                get
                {
                    return this.deviceTokenFSOField;
                }
                set
                {
                    this.deviceTokenFSOField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.csd.rsa.com")]
        public partial class analyzeResponseAnalyzeReturnIdentificationData
        {

            private bool delegatedField;

            private string groupNameField;

            private string orgNameField;

            private string sessionIdField;

            private string transactionIdField;

            private string userNameField;

            private string userStatusField;

            private string userTypeField;

            /// <remarks/>
            public bool delegated
            {
                get
                {
                    return this.delegatedField;
                }
                set
                {
                    this.delegatedField = value;
                }
            }

            /// <remarks/>
            public string groupName
            {
                get
                {
                    return this.groupNameField;
                }
                set
                {
                    this.groupNameField = value;
                }
            }

            /// <remarks/>
            public string orgName
            {
                get
                {
                    return this.orgNameField;
                }
                set
                {
                    this.orgNameField = value;
                }
            }

            /// <remarks/>
            public string sessionId
            {
                get
                {
                    return this.sessionIdField;
                }
                set
                {
                    this.sessionIdField = value;
                }
            }

            /// <remarks/>
            public string transactionId
            {
                get
                {
                    return this.transactionIdField;
                }
                set
                {
                    this.transactionIdField = value;
                }
            }

            /// <remarks/>
            public string userName
            {
                get
                {
                    return this.userNameField;
                }
                set
                {
                    this.userNameField = value;
                }
            }

            /// <remarks/>
            public string userStatus
            {
                get
                {
                    return this.userStatusField;
                }
                set
                {
                    this.userStatusField = value;
                }
            }

            /// <remarks/>
            public string userType
            {
                get
                {
                    return this.userTypeField;
                }
                set
                {
                    this.userTypeField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.csd.rsa.com")]
        public partial class analyzeResponseAnalyzeReturnMessageHeader
        {

            private string apiTypeField;

            private string requestTypeField;

            private System.DateTime timeStampField;

            private decimal versionField;

            /// <remarks/>
            public string apiType
            {
                get
                {
                    return this.apiTypeField;
                }
                set
                {
                    this.apiTypeField = value;
                }
            }

            /// <remarks/>
            public string requestType
            {
                get
                {
                    return this.requestTypeField;
                }
                set
                {
                    this.requestTypeField = value;
                }
            }

            /// <remarks/>
            public System.DateTime timeStamp
            {
                get
                {
                    return this.timeStampField;
                }
                set
                {
                    this.timeStampField = value;
                }
            }

            /// <remarks/>
            public decimal version
            {
                get
                {
                    return this.versionField;
                }
                set
                {
                    this.versionField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.csd.rsa.com")]
        public partial class analyzeResponseAnalyzeReturnStatusHeader
        {

            private byte reasonCodeField;

            private string reasonDescriptionField;

            private byte statusCodeField;

            /// <remarks/>
            public byte reasonCode
            {
                get
                {
                    return this.reasonCodeField;
                }
                set
                {
                    this.reasonCodeField = value;
                }
            }

            /// <remarks/>
            public string reasonDescription
            {
                get
                {
                    return this.reasonDescriptionField;
                }
                set
                {
                    this.reasonDescriptionField = value;
                }
            }

            /// <remarks/>
            public byte statusCode
            {
                get
                {
                    return this.statusCodeField;
                }
                set
                {
                    this.statusCodeField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.csd.rsa.com")]
        public partial class analyzeResponseAnalyzeReturnRequiredCredentialList
        {

            private analyzeResponseAnalyzeReturnRequiredCredentialListRequiredCredential requiredCredentialField;

            /// <remarks/>
            public analyzeResponseAnalyzeReturnRequiredCredentialListRequiredCredential requiredCredential
            {
                get
                {
                    return this.requiredCredentialField;
                }
                set
                {
                    this.requiredCredentialField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.csd.rsa.com")]
        public partial class analyzeResponseAnalyzeReturnRequiredCredentialListRequiredCredential
        {

            private string credentialTypeField;

            private string genericCredentialTypeField;

            private string groupNameField;

            private byte preferenceField;

            private bool requiredField;

            /// <remarks/>
            public string credentialType
            {
                get
                {
                    return this.credentialTypeField;
                }
                set
                {
                    this.credentialTypeField = value;
                }
            }

            /// <remarks/>
            public string genericCredentialType
            {
                get
                {
                    return this.genericCredentialTypeField;
                }
                set
                {
                    this.genericCredentialTypeField = value;
                }
            }

            /// <remarks/>
            public string groupName
            {
                get
                {
                    return this.groupNameField;
                }
                set
                {
                    this.groupNameField = value;
                }
            }

            /// <remarks/>
            public byte preference
            {
                get
                {
                    return this.preferenceField;
                }
                set
                {
                    this.preferenceField = value;
                }
            }

            /// <remarks/>
            public bool required
            {
                get
                {
                    return this.requiredField;
                }
                set
                {
                    this.requiredField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.csd.rsa.com")]
        public partial class analyzeResponseAnalyzeReturnRiskResult
        {

            private ushort riskScoreField;

            private string riskScoreBandField;

            private analyzeResponseAnalyzeReturnRiskResultTriggeredRule triggeredRuleField;

            private string deviceAssuranceLevelField;

            /// <remarks/>
            public ushort riskScore
            {
                get
                {
                    return this.riskScoreField;
                }
                set
                {
                    this.riskScoreField = value;
                }
            }

            /// <remarks/>
            public string riskScoreBand
            {
                get
                {
                    return this.riskScoreBandField;
                }
                set
                {
                    this.riskScoreBandField = value;
                }
            }

            /// <remarks/>
            public analyzeResponseAnalyzeReturnRiskResultTriggeredRule triggeredRule
            {
                get
                {
                    return this.triggeredRuleField;
                }
                set
                {
                    this.triggeredRuleField = value;
                }
            }

            /// <remarks/>
            public string deviceAssuranceLevel
            {
                get
                {
                    return this.deviceAssuranceLevelField;
                }
                set
                {
                    this.deviceAssuranceLevelField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.csd.rsa.com")]
        public partial class analyzeResponseAnalyzeReturnRiskResultTriggeredRule
        {

            private string actionCodeField;

            private string actionNameField;

            private string actionTypeField;

            private object clientFactListField;

            private string ruleIdField;

            private string ruleNameField;

            /// <remarks/>
            public string actionCode
            {
                get
                {
                    return this.actionCodeField;
                }
                set
                {
                    this.actionCodeField = value;
                }
            }

            /// <remarks/>
            public string actionName
            {
                get
                {
                    return this.actionNameField;
                }
                set
                {
                    this.actionNameField = value;
                }
            }

            /// <remarks/>
            public string actionType
            {
                get
                {
                    return this.actionTypeField;
                }
                set
                {
                    this.actionTypeField = value;
                }
            }

            /// <remarks/>
            public object clientFactList
            {
                get
                {
                    return this.clientFactListField;
                }
                set
                {
                    this.clientFactListField = value;
                }
            }

            /// <remarks/>
            public string ruleId
            {
                get
                {
                    return this.ruleIdField;
                }
                set
                {
                    this.ruleIdField = value;
                }
            }

            /// <remarks/>
            public string ruleName
            {
                get
                {
                    return this.ruleNameField;
                }
                set
                {
                    this.ruleNameField = value;
                }
            }
        }


        #endregion
        #region GetEGlobalResponse
        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
        public partial class Envelope
        {

            private EnvelopeBody bodyField;

            /// <remarks/>
            public EnvelopeBody Body
            {
                get
                {
                    return this.bodyField;
                }
                set
                {
                    this.bodyField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public partial class EnvelopeBody
        {

            private string getEGlobalSiteStatusResponseField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.example.org/invoice-service/")]
            public string getEGlobalSiteStatusResponse
            {
                get
                {
                    return this.getEGlobalSiteStatusResponseField;
                }
                set
                {
                    this.getEGlobalSiteStatusResponseField = value;
                }
            }
        }
        #endregion
    }
}
