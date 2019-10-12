using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DealEngine.Infrastructure.RSA.Authentication
{
    public static class RSA
    {
        public static Task GenerateRequest
        {
            get
            {
                try
                {
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("http://"), // new Uri("https://ris.us1.qeadaptiveauth.com/AdaptiveAuthentication/services/AdaptiveAuthentication"), //
                        Method = HttpMethod.Post,
                        //Content = new StringContent(GetSiteActiveSoapBody(), Encoding.UTF8, "text/xml"),
                        Content = new StringContent(RSAAnalyseXML(), Encoding.UTF8, "text/xml"),
                    };
                    httpRequestMessage.Headers.Add("Analyze", "http://ws.csd.rsa.com");

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("soap:Header", GetWsseHeader());
                    //client.DefaultRequestHeaders.Authorization; //"wsse: Security", ""

                    HttpResponseMessage response = client.SendAsync(httpRequestMessage).Result;
                    response.EnsureSuccessStatusCode();
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(responseBody);

                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                return null;
            }
        }

        static string RSAAnalyseXML()
        {
            return @"<soapenv:Envelope xmlns:soapenv = ""http://schemas.xmlsoap.org/soap/envelope/"" xmlns: ws = ""http://ws.csd.rsa.com"" >    
                        < soapenv:Header />     
                        < soapenv:Body > 
                        <AnalyzeRequest xmlns: xsi = ""http://www.w3.org/2001/XMLSchema-instance"" xmlns: xsd = ""http://www.w3.org/2001/XMLSchema"" >         
                        <actionTypeList xmlns = ""http://ws.csd.rsa.com"">      
                            <genericActionTypes > SET_USER_STATUS </genericActionTypes>      
                            <genericActionTypes> SET_USER_GROUP </genericActionTypes>      
                        </actionTypeList>      
                        <deviceRequest xmlns = ""http://ws.csd.rsa.com"">       
                            <devicePrint> version % 3D3 % 2E5 % 2E1 % 5F4 % 26pm % 5Ffpua % 3Dmozilla % 2F5 % 2E0 % 20 % 28windows % 20nt % 2010 % 2E0 % 3B % 20win64 % 3B % 20x64 % 3B % 20rv % 3A68 % 2E0 % 29 % 20gecko % 2F20100101 % 20firefox % 2F68 % 2E0 % 7C5 % 2E0 % 20 % 28Windows % 29 % 7CWin32 % 26pm % 5Ffpsc % 3D24 % 7C1920 % 7C1080 % 7C1080 % 26pm % 5Ffpsw % 3D % 26pm % 5Ffptz % 3D12 % 26pm % 5Ffpln % 3Dlang % 3Den % 2DUS % 7Csyslang % 3D % 7Cuserlang % 3D % 26pm % 5Ffpjv % 3D0 % 26pm % 5Ffpco % 3D1 % 26pm % 5Ffpasw % 3D % 26pm % 5Ffpan % 3DNetscape % 26pm % 5Ffpacn % 3DMozilla % 26pm % 5Ffpol % 3Dtrue % 26pm % 5Ffposp % 3D % 26pm % 5Ffpup % 3D % 26pm % 5Ffpsaw % 3D1920 % 26pm % 5Ffpspd % 3D24 % 26pm % 5Ffpsbd % 3D % 26pm % 5Ffpsdx % 3D % 26pm % 5Ffpsdy % 3D % 26pm % 5Ffpslx % 3D % 26pm % 5Ffpsly % 3D % 26pm % 5Ffpsfse % 3D % 26pm % 5Ffpsui % 3D % 26pm % 5Fos % 3DWindows % 26pm % 5Fbrmjv % 3D68 % 26pm % 5Fbr % 3DFirefox % 26pm % 5Finpt % 3D % 26pm % 5Fexpt % 3D </devicePrint>
                            <httpAccept/>
                            <httpAcceptEncoding/>
                            <httpAcceptLanguage/>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      < httpReferrer > http://localhost:59483/Account/Login?ReturnUrl=%2f</httpReferrer>
                            <ipAddress> 192.168.1.110 </ipAddress>   
                        </deviceRequest>   
                        <identificationData xmlns = ""http://ws.csd.rsa.com"">    
                            <delegated> false </delegated>    
                            <groupName> Clients </groupName>    
                            <orgName> Marsh_Model </orgName>
                            < userName > james@techcertain.com </ userName >       
                            < userStatus > VERIFIED </ userStatus >       
                            < userType > PERSISTENT </ userType >       
                        </ identificationData >       
                        < messageHeader xmlns = ""http://ws.csd.rsa.com"">        
                            < apiType > DIRECT_SOAP_API </ apiType >        
                            < requestType > ANALYZE </ requestType >        
                            < version > 7.0 </ version >        
                        </ messageHeader >
                        < autoCreateUserFlag xmlns = ""http://ws.csd.rsa.com"" > true </ autoCreateUserFlag >         
                        < credentialDataList xmlns = ""http://ws.csd.rsa.com"" >          
                        < acspAuthenticationRequestData />          
                        </ credentialDataList >          
                        < eventDataList xmlns = ""http://ws.csd.rsa.com"">           
                            < eventData >           
                                < clientDefinedAttributeList >           
                                    < fact >           
                                        < name > FILTERED_TAM_GROUP </ name >           
                                        < value > MFA </ value >           
                                        < dataType > STRING </ dataType >           
                                    </ fact >           
                                </ clientDefinedAttributeList >           
                            < eventType > SESSION_SIGNIN </ eventType >           
                            </ eventData >           
                        </ eventDataList >           
                        < runRiskType xmlns = ""http://ws.csd.rsa.com"" > ALL </ runRiskType >            
                        < channelIndicator xmlns = ""http://ws.csd.rsa.com"" > WEB </ channelIndicator >
                    </ AnalyzeRequest > 
                    </soapenv:Body>
                </soapenv:Envelope>";
        }

        static string GetWsseHeader()
        {
            string username = "MarshNZSOAPUser";
            string password = "MarNZ0sa$0Cap16us";

            var wsseHeader = @"<wsse:Security soap:mustUnderstand=""1"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""> 
				                <wsse:UsernameToken wsu:Id=""[TokenId]"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
			                    <wsse:Username>[Username]</wsse:Username><wsse:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">[Password]</wsse:Password>
			                    <wsse:Nonce EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"">[Nonce]</wsse:Nonce>
			                    <wsu:Created>[Created]</wsu:Created></wsse:UsernameToken></wsse:Security>";

            //testing changes to HEADER
            //wsseHeader = wsseHeader.Replace("[TokenId]", usernameToken.Id.Replace("SecurityToken", "UsernameToken"));
            //wsseHeader = wsseHeader.Replace("[Username]", usernameToken.Username);
            //wsseHeader = wsseHeader.Replace("[Password]", usernameToken.Password);
            //wsseHeader = wsseHeader.Replace("[Nonce]", Convert.ToBase64String(usernameToken.Nonce));
            //wsseHeader = wsseHeader.Replace("[Created]", usernameToken.Created.ToString("yyyy-MM-ddThh:mm:ss.fffZ"));

            return wsseHeader;
        }
    }
}
