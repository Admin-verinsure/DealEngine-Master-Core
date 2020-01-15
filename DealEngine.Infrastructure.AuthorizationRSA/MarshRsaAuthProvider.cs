
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
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

			XmlSerializer serializer;
			StringReader rdr;
            Envelope analyzeResponse = new Envelope();
            AnalyzeRequest analyzeRequest = GetAnalyzeRequest (rsaUser, hasCookies);
            Console.WriteLine ("XmlSerializer");
			var serxml = new XmlSerializer (analyzeRequest.GetType ());
            var ms = new MemoryStream ();
			serxml.Serialize (ms, analyzeRequest);
            string xml = Encoding.UTF8.GetString (ms.ToArray ());

            //Console.WriteLine (xml);
            _logger.LogDebug(xml);
            _logger.LogDebug(analyzeRequest.ToString());
            var analyzeResponseXmlStr = await _httpClientService.Analyze(xml);
            serializer = new XmlSerializer(typeof(Envelope));
			rdr = new StringReader(analyzeResponseXmlStr);
            try
            {
                analyzeResponse = (Envelope)serializer.Deserialize(rdr);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

			var userStatus = analyzeResponse.Body.analyzeResponse.analyzeReturn.identificationData.userStatus;
			var actionCode = analyzeResponse.Body.analyzeResponse.analyzeReturn.riskResult.triggeredRule.actionCode;

			if (userStatus != UserStatus.LOCKOUT.ToString() || userStatus != UserStatus.DELETE.ToString())
			{

				UpdateUserResponse updateUserResponse = null;
				if (userStatus == UserStatus.UNVERIFIED.ToString())
				{
					// TODO - call updateUser here with analyzeResponse
					//UpdateRsaUserFromResponse(analyzeResponse, rsaUser);
					UpdateUserRequest updateUserRequest = GetUpdateUserRequest(rsaUser);
					//updateUserResponse = binding.updateUser(updateUserRequest);
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
                userStatus = UserStatus.VERIFIED,   // doc default, will need to know how marsh expects these values
				userStatusSpecified = true,
				userType = WSUserType.PERSISTENT,   // doc default, will need to know how marsh expects these values
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
        #region serialiseObject


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
            #endregion
        }
    } 
}

