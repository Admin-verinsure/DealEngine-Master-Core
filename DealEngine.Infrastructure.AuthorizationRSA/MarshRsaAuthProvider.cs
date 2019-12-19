using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using TechCertain.Services.Interfaces;

namespace DealEngine.Infrastructure.AuthorizationRSA
{
    public enum RsaStatus
	{
		Allow,
		Deny,
		RequiresOtp
	}

	public class MarshRsaAuthProvider
	{
		ILogger _logger;
        IHttpClientService _httpClientService;

		public MarshRsaAuthProvider (ILogger logger, IHttpClientService httpClientService)
		{
			_logger = logger;
            _httpClientService = httpClientService;

        }

		public MarshRsaUser GetRsaUser(string email)
		{
			return new MarshRsaUser (email);
		}

		/// <summary>
		/// Calculates the hash of the given user Id. Copied from Marsh supplied MFAUtils.
		/// </summary>
		/// <returns>The hashed identifier.</returns>
		/// <param name="userId">User identifier.</param>
		public string GetHashedId (string userId)
		{
			// Copied from Marsh supplied MFAUtils.
			SHA256 sha256 = new SHA256Managed ();
			var sha256Bytes = Encoding.Default.GetBytes (userId);
			var cryString = sha256.ComputeHash (sha256Bytes);
			var sha256Str = string.Empty;
			for (var i = 0; i < cryString.Length; i++) {
				sha256Str += cryString [i].ToString ("X").PadLeft (2, '0');
			}

			return sha256Str.ToLower ();
		}

		public async Task<RsaStatus> Analyze (MarshRsaUser rsaUser, bool hasCookies)
		{
			/*
			 * Needs to be called at the server level, so needs to be converted into a Membership type object provider.
			 * This should only be controlling access to organisational content, and rejecting unauthorized access based
			 * on what RSA says.
			 */
			AnalyzeResponse analyzeResponse = new AnalyzeResponse();
            AnalyzeRequest analyzeRequest = GetAnalyzeRequest (rsaUser, hasCookies);
			var serxml = new XmlSerializer (analyzeRequest.GetType ());
            var ms = new MemoryStream ();
			serxml.Serialize (ms, analyzeRequest);
            string analyseXml = Encoding.UTF8.GetString (ms.ToArray ());

            //Console.WriteLine (xml);
            _logger.LogDebug(analyseXml);
            _logger.LogDebug(analyzeRequest.ToString());
            var analyzeResponseXmlStr = await _httpClientService.Analyze(analyseXml);
            var xDoc = XDocument.Parse(analyzeResponseXmlStr);
            
            try
            {
				var identificationData = xDoc.Descendants().Where(x => x.Name.LocalName == "identificationData").Select(x => new IdentificationData
				{
					delegated = (bool)x.Element(x.Name.Namespace + "delegated"),
					groupName = (string)x.Element(x.Name.Namespace + "groupName"),
					orgName = (string)x.Element(x.Name.Namespace + "orgName"),
					sessionId = (string)x.Element(x.Name.Namespace + "sessionId"),
					transactionId = (string)x.Element(x.Name.Namespace + "transactionId"),
					userName = (string)x.Element(x.Name.Namespace + "userName"),
					userStatus = (string)x.Element(x.Name.Namespace + "userStatus"),
					userType = (string)x.Element(x.Name.Namespace + "userType")
					
				}).FirstOrDefault();
				var riskResult = xDoc.Descendants().Where(x => x.Name.LocalName == "riskResult").Select(x => new RiskResult
				{
					riskScore = (int)x.Element(x.Name.Namespace + "riskScore"),
					deviceAssuranceLevel = (string)x.Element(x.Name.Namespace + "deviceAssuranceLevel")
					
				}).FirstOrDefault();
				var triggeredRule = xDoc.Descendants().Where(x => x.Name.LocalName == "triggeredRule").Select(x => new TriggeredRule
				{
					actionCode = (string)x.Element(x.Name.Namespace + "actionCode"),
					actionName = (string)x.Element(x.Name.Namespace + "actionName"),
					actionType = (string)x.Element(x.Name.Namespace + "actionType"),
					ruleId = (string)x.Element(x.Name.Namespace + "ruleId"),
					ruleName = (string)x.Element(x.Name.Namespace + "ruleName")

				}).FirstOrDefault();
				var deviceData = xDoc.Descendants().Where(x => x.Name.LocalName == "deviceData").Select(x => new DeviceData
				{
					deviceTokenCookie = (string)x.Element(x.Name.Namespace + "deviceTokenCookie")

				}).FirstOrDefault();

				analyzeResponse.deviceResult = new DeviceResult();
				analyzeResponse.identificationData = identificationData;
				analyzeResponse.riskResult = riskResult;
				analyzeResponse.riskResult.triggeredRule = triggeredRule;
				analyzeResponse.deviceResult.deviceData = deviceData;

			}
            catch(Exception ex)
            {
				_logger.LogInformation("Marsh user failed to login");
				return RsaStatus.Deny;
			}

			var userStatus = analyzeResponse.identificationData.userStatus;
			var actionCode = analyzeResponse.riskResult.triggeredRule.actionCode;

			if (userStatus != UserStatus.LOCKOUT.ToString() || userStatus != UserStatus.DELETE.ToString())
			{

				UpdateUserResponse updateUserResponse = null;
				if (userStatus == UserStatus.UNVERIFIED.ToString())
				{
					// TODO - call updateUser here with analyzeResponse
					UpdateRsaUserFromResponse(analyzeResponse, rsaUser);
					UpdateUserRequest updateUserRequest = GetUpdateUserRequest(rsaUser);
					serxml = new XmlSerializer(updateUserRequest.GetType());					
					serxml.Serialize(ms, updateUserRequest);
					var updateXml = Encoding.UTF8.GetString(ms.ToArray());					
					var test = await _httpClientService.UpdateUser(updateXml);
				}
				if (actionCode == ActionCode.CHALLENGE.ToString())
				{
					if (userStatus == UserStatus.UNVERIFIED.ToString())
					{
						// TODO - call challenge here with updateUserResponse
						UpdateRsaUserFromResponse(updateUserResponse, rsaUser);
					}
					else
					{
						// TODO - call challenge here with analyzeResponse
						//UpdateRsaUserFromResponse(analyzeResponse, rsaUser);
					}
					return RsaStatus.RequiresOtp;
					//GetOneTimePassword (rsaUser);
					//AuthenticateInternal (rsaUser);
				}
				if (actionCode == ActionCode.ALLOW.ToString())
				{
					// Need to save the deviceTokenCookie from analyzeReponse
					//UpdateRsaUserFromResponse(analyzeResponse, rsaUser);
					return RsaStatus.Allow;
				}
			}

			if (userStatus == UserStatus.LOCKOUT.ToString() || userStatus == UserStatus.DELETE.ToString())
			{
				_logger.LogInformation("Marsh user failed to login");
				throw new Exception("unable to login user: "+ rsaUser.Username);
			}
			// user not allowed in if we get here.
			return RsaStatus.Deny;
		}

        public string GetOneTimePassword(MarshRsaUser rsaUser)
        {
            ChallengeRequest challengeRequest = GetChallengeRequest(rsaUser);
            //var challengeResponse = binding.challenge(challengeRequest);
            //UpdateRsaUserFromResponse(challengeResponse, rsaUser);
            //rsaUser.Otp = ((OTPChallengeResponse)challengeResponse.credentialChallengeList.acspChallengeResponseData.payload).otp;
            return rsaUser.Otp;
        }

        //public bool Authenticate (MarshRsaUser rsaUser)
        //{
        //	AuthenticateRequest authenticateRequest = GetAuthenticateRequest (rsaUser);
        //	var authenticateResponse = binding.authenticate (authenticateRequest);

        //	UserStatus userStatus = authenticateResponse.identificationData.userStatus;
        //	var statusCode = authenticateResponse.credentialAuthResultList.acspAuthenticationResponseData.callStatus.statusCode;

        //	if (userStatus == UserStatus.LOCKOUT || userStatus == UserStatus.DELETE) {
        //		// user locked out or deleted
        //	}
        //	else if (statusCode == "FAIL") {
        //		// invalid otp
        //	}
        //	else if (statusCode == "SUCCESS") {
        //		UpdateRsaUserFromResponse (authenticateResponse, rsaUser);
        //		// invalid otp
        //		return true;
        //	}
        //	return false;
        //}

        void UpdateRsaUserFromResponse (GenericResponse response, MarshRsaUser rsaUser)
		{
			rsaUser.CurrentSessionId = response.identificationData.clientSessionId;
			rsaUser.CurrentTransactionId = response.identificationData.transactionId;
			rsaUser.DeviceTokenCookie = response.deviceResult.deviceData.deviceTokenCookie;
		}

		#region Create Request Elements

		GenericActionTypeList GetGenericActionTypes ()
		{
			return new GenericActionTypeList {
				genericActionTypes = new GenericActionType [] { GenericActionType.SET_USER_STATUS, GenericActionType.SET_USER_GROUP },
			};
		}

        private string  GetIP()
        {
            var strHostName = System.Net.Dns.GetHostName();
            System.Net.IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            System.Net.IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 1].ToString();
        }

		DeviceRequest GetDeviceRequest (MarshRsaUser rsaUser)
		{
			return new DeviceRequest {
				devicePrint = rsaUser.DevicePrint,
				deviceTokenCookie = rsaUser.DeviceTokenCookie,
				// following fields required? if so, will need provide web request data - specialized web api?
				httpAccept = "",
				httpAcceptEncoding = "",
				httpAcceptLanguage = "",
				httpReferrer = rsaUser.HttpReferer,

                //for testing purposes
                //ipAddress = rsaUser.IpAddress,
                ipAddress = GetIP(),
				userAgent = rsaUser.UserAgent
			};
		}

		IdentificationData GetIdentificationData (MarshRsaUser rsaUser)
		{
            return new IdentificationData {
                delegated = false,          // confirm
                delegatedSpecified = true,
                groupName = "Clients",              // Clients in sample, probably changes
                orgName = rsaUser.OrgName,  // confirm
                //userLoginName = rsaUser.Username,   // hased username from JS here
                userName = rsaUser.Username,        // see above
                //userEmailAddress = rsaUser.Email,
                userStatus = UserStatus.VERIFIED.ToString(),   // doc default, will need to know how marsh expects these values
				userStatusSpecified = true,
				userType = WSUserType.PERSISTENT.ToString(),   // doc default, will need to know how marsh expects these values
				userTypeSpecified = true,
			};
		}

		MessageHeader GetMessageHeader (RequestType requestType)
		{
			return new MessageHeader {
				apiType = APIType.DIRECT_SOAP_API,  // doc default, will need to know how marsh expects these values
				apiTypeSpecified = true,
				requestType = requestType,          // this looks obvious
				requestTypeSpecified = true,
				version = MessageVersion.Item70,    // Probably safe to use the default
				versionSpecified = true
			};
		}

		EventData [] GetEventData (MarshRsaUser rsaUser)
		{
			return new EventData [] {
				new EventData {
					clientDefinedAttributeList = new ClientDefinedFact[] {
						new ClientDefinedFact {
							name = "FILTERED_TAM_GROUP",
							value = "MFA",
							dataType = DataType.STRING,
							dataTypeSpecified = true,
						}
					},
					eventType = EventType.SESSION_SIGNIN,
					eventTypeSpecified = true,
				}
			};
		}

		CredentialChallengeRequestList GetCredentialChallengeRequestList ()
		{
			return new CredentialChallengeRequestList {
				acspChallengeRequestData = new AcspChallengeRequestData {
					payload = new OTPChallengeRequest ()
				}
			};
		}

		CredentialDataList GetCredentialDataList ()
		{
			return new CredentialDataList {
				acspAuthenticationRequestData = new AcspAuthenticationRequestData ()
			};
		}

		CredentialDataList GetCredentialDataList (MarshRsaUser rsaUser)
		{
			return new CredentialDataList {
				acspAuthenticationRequestData = new AcspAuthenticationRequestData {
					payload = new OTPAuthenticationRequest {
						otp = rsaUser.Otp
					}
				}
			};
		}

		#endregion

		#region Create Requests

		AnalyzeRequest GetAnalyzeRequest (MarshRsaUser rsaUser, bool hasCookies)
		{
			return new AnalyzeRequest {
				actionTypeList = GetGenericActionTypes (),
				deviceRequest = GetDeviceRequest (rsaUser),
				identificationData = GetIdentificationData (rsaUser),
				messageHeader = GetMessageHeader (RequestType.ANALYZE),     // this looks obvious
				autoCreateUserFlag = true,                                  // confirm value
				autoCreateUserFlagSpecified = true,
				credentialDataList = GetCredentialDataList (),
				eventDataList = GetEventData (rsaUser),
				runRiskType = RunRiskType.ALL,                  // confirm value
				channelIndicator = ChannelIndicatorType.WEB,    // fairly sure that this is supposed to be web
				channelIndicatorSpecified = true,
			};
		}

		UpdateUserRequest GetUpdateUserRequest (MarshRsaUser rsaUser)
		{
			return new UpdateUserRequest {
				actionTypeList = GetGenericActionTypes (),
				deviceRequest = GetDeviceRequest (rsaUser),
				identificationData = GetIdentificationData (rsaUser),
				messageHeader = GetMessageHeader (RequestType.UPDATEUSER)
			};
		}

		ChallengeRequest GetChallengeRequest (MarshRsaUser rsaUser)
		{
			return new ChallengeRequest {
				deviceRequest = GetDeviceRequest (rsaUser),
				identificationData = GetIdentificationData (rsaUser),
				messageHeader = GetMessageHeader (RequestType.CHALLENGE),
				credentialChallengeRequestList = GetCredentialChallengeRequestList ()
			};
		}

		AuthenticateRequest GetAuthenticateRequest (MarshRsaUser rsaUser)
		{
			return new AuthenticateRequest {
				deviceRequest = GetDeviceRequest (rsaUser),
				identificationData = GetIdentificationData (rsaUser),
				messageHeader = GetMessageHeader (RequestType.CHALLENGE),     // this looks obvious
				credentialDataList = GetCredentialDataList (rsaUser)
			};
		}

		AuthenticateRequest GetAuthenticateRequest (ChallengeResponse challengeResponse)
		{
			return new AuthenticateRequest {

				identificationData = new IdentificationData {
					sessionId = challengeResponse.identificationData.clientSessionId,
					transactionId = challengeResponse.identificationData.transactionId,
				},
				credentialDataList = new CredentialDataList {
					acspAuthenticationRequestData = new AcspAuthenticationRequestData {
						payload = new OTPAuthenticationRequest {
							otp = ((OTPChallengeResponse)challengeResponse.credentialChallengeList.acspChallengeResponseData.payload).otp
						}
					}
				},
			};
		}

        #endregion
    }
} 


