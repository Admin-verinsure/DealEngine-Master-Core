using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
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
                responseMessage = ex.Message;
            }

            _socketsHttpHandler.Dispose();
            _httpRequestMessage.Dispose();

            return responseMessage;
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
    }
}
