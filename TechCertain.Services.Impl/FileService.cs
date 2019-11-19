using System;
using System.Linq;
using System.Net.Mime;
using System.IO;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace TechCertain.Services.Impl
{
	public class FileService : IFileService
	{
		const uint _jpegMagicNumber = 0xD8FF;
		const uint _jfifAPP0Marker = 0xE0FF;
		const uint _exifAPP1Marker = 0xE1FF;
		const uint _jfifMarker = 0x4649464A;
		const uint _exifMarker = 0x66697845;

		const uint _gifMagicNumber = 0x38464947;//0x47494638;
		const ulong _pngMagicNumber = 0x0A1A0A0D474E5089;//0x89504E470D0A1A0A;
		const uint _tiffMagicNumberIntel = 0x002A4949;
		const uint _tiffMagicNumberMotorola = 0x2A004D4D;

		IMapperSession<Image> _imageRepository;
		IMapperSession<Document> _documentRepository;
		IClientAgreementMVTermService _clientAgreementMVTermService;
        IClientAgreementBVTermService _clientAgreementBVTermService;        

        public FileService(IMapperSession<Image> imageRepository, IMapperSession<Document> documentRepository, IClientAgreementMVTermService clientAgreementMVTermService, IClientAgreementBVTermService clientAgreementBVTermService)
		{
			_imageRepository = imageRepository;
			_documentRepository = documentRepository;
			_clientAgreementMVTermService = clientAgreementMVTermService;
            _clientAgreementBVTermService = clientAgreementBVTermService;            

			FileDirectory = Path.Combine (
				Directory.GetCurrentDirectory (),
				"App_Data",
				"uploads"
			);
		}

		#region IFileService implementation

		public string FileDirectory { get; protected set; }

		public bool IsApplication(byte [] buffer, string contentType, string fileName)
		{
			throw new NotImplementedException ();
		}

		public bool IsImageFile(byte [] buffer, string contentType, string fileName)
		{
			// references
            //can we make this a task async -- potentially screwing up the system if its not converted..
			// https://en.wikipedia.org/wiki/Magic_number_%28programming%29
			// http://stackoverflow.com/a/8755028

			string extension = Path.GetExtension (fileName);
			BinaryReader br = new BinaryReader (new MemoryStream (buffer));

			if ((extension == ".jpg" || extension == ".jpeg") && contentType == MediaTypeNames.Image.Jpeg) {
				bool isJpeg = br.ReadUInt16 () == _jpegMagicNumber;
				uint appMarker = br.ReadUInt16 ();
				br.ReadUInt16 ();   // need to skip ahead 2 bytes
				uint format = br.ReadUInt32 ();
				return isJpeg && (
					(appMarker == _jfifAPP0Marker && format == _jfifMarker) ||
					(appMarker == _exifAPP1Marker && format == _exifMarker)
				);
			}
			if (extension == ".gif" && contentType == MediaTypeNames.Image.Gif)
				return br.ReadUInt32 () == _gifMagicNumber;
			if (extension == ".tiff" && contentType == MediaTypeNames.Image.Tiff) {
				uint magicNumber = br.ReadUInt32 ();
				return magicNumber == _tiffMagicNumberIntel || magicNumber == _tiffMagicNumberMotorola;
			}
			if (extension == ".png" && contentType == "image/png")
				return br.ReadUInt64 () == _pngMagicNumber;
			return false;
		}

		public bool IsTextFile(byte [] buffer, string contentType, string fileName)
		{
			// references
			// http://stackoverflow.com/a/14587821

			string extension = Path.GetExtension (fileName);
			if (extension == ".html" && contentType == MediaTypeNames.Text.Html)
				return true;
			if (extension == ".txt" && contentType == MediaTypeNames.Text.Plain)
				return true;
			if (extension == ".rtf" && contentType == MediaTypeNames.Text.RichText)
				return true;
			if (extension == ".xml" && contentType == MediaTypeNames.Text.Xml)
				return true;
			return false;
		}

		public async Task UploadFile(Document document)
		{
            await _documentRepository.AddAsync(document);
		}

        public async Task UploadFile(Image image)
		{
		    _imageRepository.AddAsync(image);
		}

		public async Task<Document> GetDocument(string documentName)
		{
			return await _documentRepository.FindAll().FirstOrDefaultAsync(i => i.Name == documentName);
        }

        public async Task<Document> GetDocumentByType(Organisation primaryOrganisation, int DocumentType)
        {
            Document document = await _documentRepository.FindAll().FirstOrDefaultAsync(i => i.OwnerOrganisation == primaryOrganisation && i.DocumentType == DocumentType);
            return document;
        }

        public async Task<Image> GetImage(string imageName)
		{
			return await _imageRepository.FindAll().FirstOrDefaultAsync(i => i.Name == imageName);
		}

		public async Task<T> RenderDocument<T>(User renderedBy, T template, ClientAgreement agreement) where T : Document
		{
			Document doc = new Document (renderedBy, template.Name, template.ContentType, template.DocumentType);

			// store all the fields to be merged
			List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>> ();
			mergeFields.Add (new KeyValuePair<string, string> ("[[InsuredName]]", agreement.InsuredName));
			mergeFields.Add (new KeyValuePair<string, string> ("[[NameOfInsured]]", agreement.InsuredName));
            mergeFields.Add(new KeyValuePair<string, string>("[[Reference]]", agreement.ClientInformationSheet.ReferenceId));
            mergeFields.Add(new KeyValuePair<string, string>("[[BrokerName]]", agreement.ClientInformationSheet.Programme.BrokerContactUser.FullName));
            mergeFields.Add(new KeyValuePair<string, string>("[[BrokerJobTitle]]", agreement.ClientInformationSheet.Programme.BrokerContactUser.JobTitle));
            mergeFields.Add(new KeyValuePair<string, string>("[[BrokerPhone]]", agreement.ClientInformationSheet.Programme.BrokerContactUser.Phone));
            mergeFields.Add(new KeyValuePair<string, string>("[[BrokerEmail]]", agreement.ClientInformationSheet.Programme.BrokerContactUser.Email));

            mergeFields.Add(new KeyValuePair<string, string>("[[ClientBranchCode]]", agreement.ClientInformationSheet.Programme.EGlobalBranchCode));
            mergeFields.Add(new KeyValuePair<string, string>("[[ClientNumber]]", agreement.ClientInformationSheet.Programme.EGlobalClientNumber));
            //Eglobal merge fields
            if (agreement.ClientInformationSheet.Programme.ClientAgreementEGlobalResponses.Count > 0)
            {
                EGlobalResponse eGlobalResponse = agreement.ClientInformationSheet.Programme.ClientAgreementEGlobalResponses.Where(er => er.DateDeleted == null && er.ResponseType == "update").OrderByDescending(er => er.VersionNumber).FirstOrDefault();
                if (eGlobalResponse != null)
                {
                    if (agreement.MasterAgreement && (agreement.ReferenceId == eGlobalResponse.MasterAgreementReferenceID))
                    {
                        mergeFields.Add(new KeyValuePair<string, string>("[[InvoiceDate]]", eGlobalResponse.DateCreated.GetValueOrDefault().ToString("dd/MM/yyyy")));
                        mergeFields.Add(new KeyValuePair<string, string>("[[InvoiceReference]]", eGlobalResponse.InvoiceNumber.ToString()));
                        mergeFields.Add(new KeyValuePair<string, string>("[[CoverNo]]", eGlobalResponse.CoverNumber.ToString()));
                        mergeFields.Add(new KeyValuePair<string, string>("[[Version]]", eGlobalResponse.VersionNumber.ToString()));
                    }
                }
            }
            //mergeFields.Add(new KeyValuePair<string, string>("​[[InsuredPostalAddress]]", 
            //    agreement.ClientInformationSheet.Owner.OrganisationalUnits.FirstOrDefault().Locations.FirstOrDefault().Street));//Address needs re-work
            mergeFields.Add (new KeyValuePair<string, string> ("[[InceptionDate]]", agreement.InceptionDate.ToString ("dd/MM/yyyy")));
			mergeFields.Add (new KeyValuePair<string, string> ("[[ExpiryDate]]", agreement.ExpiryDate.ToString ("dd/MM/yyyy")));
			if (agreement.Bound == true) {
				mergeFields.Add (new KeyValuePair<string, string> ("[[BoundOrQuoteDate]]", agreement.BoundDate.ToString ("dd/MM/yyyy")));
			} else {
				mergeFields.Add (new KeyValuePair<string, string> ("[[BoundOrQuoteDate]]", agreement.QuoteDate.ToString ("dd/MM/yyyy")));
			}
			mergeFields.Add (new KeyValuePair<string, string> ("[[BoundDate]]", agreement.BoundDate.ToString("dd/MM/yyyy")));
			mergeFields.Add (new KeyValuePair<string, string> ("[[QuoteDate]]", agreement.QuoteDate.ToString ("dd/MM/yyyy")));
			mergeFields.Add (new KeyValuePair<string, string> ("[[PolicyNumber]]", agreement.PolicyNumber));
			mergeFields.Add (new KeyValuePair<string, string> ("[[Brokerage]]", (agreement.Brokerage / 100).ToString ("P2", CultureInfo.CreateSpecificCulture("en-NZ"))));
			mergeFields.Add (new KeyValuePair<string, string> ("[[AdministrationFee]]", agreement.BrokerFee.ToString ("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
            if (agreement.Status == "Bound" || agreement.Status == "Bound and invoice pending" || agreement.Status == "Bound and invoiced")
            {
                mergeFields.Add(new KeyValuePair<string, string>("[[CreditCardType]]", agreement.ClientInformationSheet.Programme.Payment.CreditCardType));
                mergeFields.Add(new KeyValuePair<string, string>("[[CreditCardNumber]]", agreement.ClientInformationSheet.Programme.Payment.CreditCardNumber));
            }
            NumberFormatInfo currencyFormat = new CultureInfo (CultureInfo.CurrentCulture.ToString ()).NumberFormat;
			currencyFormat.CurrencyNegativePattern = 2;

			// loop over terms and set merge feilds
			foreach (var term in agreement.ClientAgreementTerms) {

				mergeFields.Add (new KeyValuePair<string, string> (string.Format ("[[RetroactiveDate_{0}]]", term.SubTermType), ""));
				mergeFields.Add (new KeyValuePair<string, string> (string.Format ("[[BoundLimit_{0}]]", term.SubTermType), term.TermLimit.ToString ("C0", CultureInfo.CreateSpecificCulture("en-NZ"))));
				mergeFields.Add (new KeyValuePair<string, string> (string.Format ("[[BoundExcess_{0}]]", term.SubTermType), term.Excess.ToString ("C0", CultureInfo.CreateSpecificCulture("en-NZ"))));

				if (agreement.ClientInformationSheet.IsChange && agreement.ClientInformationSheet.PreviousInformationSheet != null) {
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremiumAdjustment_{0}]]", term.SubTermType), (term.PremiumDiffer - term.FSLDiffer).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremium_{0}]]", term.SubTermType), term.PremiumDiffer.ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremiumInclFee_{0}]]", term.SubTermType), (term.PremiumDiffer + agreement.BrokerFee).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundGST_{0}]]", term.SubTermType), ((term.PremiumDiffer) * agreement.Product.TaxRate).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundFSL_{0}]]", term.SubTermType), term.FSLDiffer.ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremiumInclFeeGST_{0}]]", term.SubTermType), ((term.PremiumDiffer + agreement.BrokerFee) * agreement.Product.TaxRate).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremiumInclFeeInclGST_{0}]]", term.SubTermType), ((term.PremiumDiffer + agreement.BrokerFee) * (1 + agreement.Product.TaxRate)).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[CreditCardSurcharge_{0}]]", term.SubTermType), ((term.PremiumDiffer + agreement.BrokerFee) * (0.02m)).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremiumInclFeeCCSurchargeGST_{0}]]", term.SubTermType), ((term.PremiumDiffer + agreement.BrokerFee) * (1 + 0.02m) * agreement.Product.TaxRate).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremiumInclGSTCreditCardCharge_{0}]]", term.SubTermType), ((term.PremiumDiffer + agreement.BrokerFee) * (1 + agreement.Product.TaxRate) * 1.02m).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                } else {
					mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremiumAdjustment_{0}]]", term.SubTermType), (term.Premium - term.FSL).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremium_{0}]]", term.SubTermType), term.Premium.ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremiumInclFee_{0}]]", term.SubTermType), (term.Premium + agreement.BrokerFee).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundGST_{0}]]", term.SubTermType), ((term.Premium) * agreement.Product.TaxRate).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundFSL_{0}]]", term.SubTermType), term.FSL.ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremiumInclFeeGST_{0}]]", term.SubTermType), ((term.Premium + agreement.BrokerFee) * agreement.Product.TaxRate).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremiumInclFeeInclGST_{0}]]", term.SubTermType), ((term.Premium + agreement.BrokerFee) * (1 + agreement.Product.TaxRate)).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[CreditCardSurcharge_{0}]]", term.SubTermType), ((term.Premium + agreement.BrokerFee) * 0.02m).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremiumInclFeeCCSurchargeGST_{0}]]", term.SubTermType), ((term.Premium + agreement.BrokerFee) * (1 + 0.02m) * agreement.Product.TaxRate).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                    mergeFields.Add(new KeyValuePair<string, string>(string.Format("[[BoundPremiumInclGSTCreditCardCharge_{0}]]", term.SubTermType), ((term.Premium + agreement.BrokerFee) * (1 + agreement.Product.TaxRate) * 1.02m).ToString("C", CultureInfo.CreateSpecificCulture("en-NZ"))));
                }

			}

			//MV Details
			if (agreement.ClientAgreementTerms.Any (cat => cat.SubTermType == "MV")) {
				int intMVNumberOfUnits = 0;
				string strFinancialIPList = null;
				int intFinancialIPCount = 0;

				DataTable dt = new DataTable ();
				dt.Columns.Add ("Category");
				dt.Columns.Add ("Year");
				dt.Columns.Add ("Make");
				dt.Columns.Add ("Model");
				dt.Columns.Add ("Fleet No.");
				dt.Columns.Add ("Registration");
				dt.Columns.Add ("Interest Parties");
				dt.Columns.Add ("Sum Insured");

                var AgreementMVTerms = await _clientAgreementMVTermService.GetAllAgreementMVTermFor(agreement.ClientAgreementTerms.FirstOrDefault(at => at.SubTermType == "MV"));
                AgreementMVTerms.OrderBy(camvt => camvt.Registration);

                foreach (ClientAgreementMVTerm mVTerm in AgreementMVTerms) {
					DataRow dr = dt.NewRow ();

					dr ["Category"] = mVTerm.VehicleCategory;
					dr ["Year"] = mVTerm.Year;
					dr ["Make"] = mVTerm.Make;
					dr ["Model"] = mVTerm.Model;
					dr ["Fleet No."] = mVTerm.FleetNumber;
					dr ["Registration"] = mVTerm.Registration;

					string strInterestPartiesNamesList = null;
					int intIPCount = 0;
					if (mVTerm.Vehicle.InterestedParties.Count > 0) {
						foreach (Organisation InterestParty in mVTerm.Vehicle.InterestedParties) {
							if (intIPCount == 0) {
								strInterestPartiesNamesList = InterestParty.Name;
							} else {
								strInterestPartiesNamesList = strInterestPartiesNamesList + ", " + InterestParty.Name;
							}

							intIPCount += 1;

							if (InterestParty.OrganisationType.Name == "financial") {
								if (intFinancialIPCount == 0) {
									strFinancialIPList = InterestParty.Name;
								} else {
									strFinancialIPList = strInterestPartiesNamesList + ", " + InterestParty.Name;
								}
								intFinancialIPCount += 1;
							}
						}
					}
					dr ["Interest Parties"] = strInterestPartiesNamesList;

					dr ["Sum Insured"] = mVTerm.TermLimit.ToString ("C0", CultureInfo.CreateSpecificCulture("en-NZ"));

					dt.Rows.Add (dr);

					intMVNumberOfUnits += 1;
				}

				dt.TableName = "MVDetailsTable";

				DataTable dt2 = new DataTable ();
				dt2.Columns.Add ("Year");
				dt2.Columns.Add ("Make");
				dt2.Columns.Add ("Model");
				dt2.Columns.Add ("Registration");
				dt2.Columns.Add ("Sum Insured");
                //dt2.Columns.Add ("End Date");

                AgreementMVTerms = await _clientAgreementMVTermService.GetAllAgreementMVTermFor(agreement.ClientAgreementTerms.FirstOrDefault(at => at.SubTermType == "MV"));
                AgreementMVTerms.OrderBy(camvt => camvt.Registration);

                foreach (ClientAgreementMVTerm mVTerm2 in AgreementMVTerms) {
					DataRow dr2 = dt2.NewRow ();

					dr2 ["Year"] = mVTerm2.Year;
					dr2 ["Make"] = mVTerm2.Make;
					dr2 ["Model"] = mVTerm2.Model;
					dr2 ["Registration"] = mVTerm2.Registration;
					dr2 ["Sum Insured"] = mVTerm2.TermLimit.ToString ("C0", CultureInfo.CreateSpecificCulture("en-NZ"));
					//dr2 ["End Date"] = "01/09/2018";

					dt2.Rows.Add (dr2);

				}

				dt2.TableName = "MVDetailsTable2";

				mergeFields.Add (new KeyValuePair<string, string> ("[[MVNumberOfUnits]]", intMVNumberOfUnits.ToString ()));
				mergeFields.Add (new KeyValuePair<string, string> ("[[MVDetailsTable]]", ConvertDataTableToHTML (dt)));
				mergeFields.Add (new KeyValuePair<string, string> ("[[MVDetailsTable2]]", ConvertDataTableToHTML (dt2)));

				//if (intFinancialIPCount > 1)
				//{
				//    strFinancialIP = strFinancialIPList + " are";
				//} else
				//{
				//    strFinancialIP = strFinancialIPList + " is";
				//}
				mergeFields.Add (new KeyValuePair<string, string> ("[[FinancialIP]]", strFinancialIPList));
			}

            //BV Details
            if (agreement.ClientAgreementTerms.Any(cat => cat.SubTermType == "BV"))
            {
                int intBVNumberOfUnits = 0;

                DataTable dtbv = new DataTable();
                dtbv.Columns.Add("Name");
                dtbv.Columns.Add("Year");
                dtbv.Columns.Add("Make");
                dtbv.Columns.Add("Model");

                /*foreach (ClientAgreementBVTerm bVTerm in _clientAgreementBVTermService.GetAllAgreementBVTermFor(agreement.ClientAgreementTerms.FirstOrDefault(at => at.SubTermType == "BV")).OrderBy(cabvt => cabvt.BoatName))
                {
                    DataRow drbv = dtbv.NewRow();

                    drbv["Name"] = bVTerm.BoatName;
                    drbv["Year"] = bVTerm.YearOfManufacture;
                    drbv["Make"] = bVTerm.BoatMake;
                    drbv["Model"] = bVTerm.BoatModel;

                    string strBVInterestPartiesNamesList = null;
                    int intBVIPCount = 0;
                    if (bVTerm.Boat.InterestedParties.Count > 0)
                    {
                        foreach (Organisation InterestParty in bVTerm.Boat.InterestedParties)
                        {
                            if (intBVIPCount == 0)
                            {
                                strBVInterestPartiesNamesList = InterestParty.Name;
                            }
                            else
                            {
                                strBVInterestPartiesNamesList = strBVInterestPartiesNamesList + ", " + InterestParty.Name;
                            }

                            intBVIPCount += 1;
                        }
                    }
                    //drbv["Interest Parties"] = strBVInterestPartiesNamesList;

                    dtbv.Rows.Add(drbv);

                    
                }*/

                DataTable dtbv2 = new DataTable();
                dtbv2.Columns.Add("Name");
                dtbv2.Columns.Add("Year");
                dtbv2.Columns.Add("Make");
                dtbv2.Columns.Add("Model");
                dtbv2.Columns.Add("Sum Insured");


                DataTable dtbv3 = new DataTable();
                dtbv3.Columns.Add("Name");
                //dtbv3.Columns.Add("Year");
                //dtbv3.Columns.Add("Make");
                //dtbv3.Columns.Add("Model");
                dtbv3.Columns.Add("Excess");

                DataTable dtbv4 = new DataTable();
                dtbv4.Columns.Add("Name");
                //dtbv4.Columns.Add("Year");
                //dtbv4.Columns.Add("Make");
                //dtbv4.Columns.Add("Model");
                dtbv4.Columns.Add("Yacht Racing Risk");

                DataTable dtbv5 = new DataTable();
                dtbv5.Columns.Add("Name");
                //dtbv4.Columns.Add("Year");
                //dtbv4.Columns.Add("Make");
                //dtbv4.Columns.Add("Model");
                dtbv5.Columns.Add("Interest Party");

                var AgreementBVTerms = await _clientAgreementBVTermService.GetAllAgreementBVTermFor(agreement.ClientAgreementTerms.FirstOrDefault(at => at.SubTermType == "BV"));
                AgreementBVTerms.OrderBy(cabvt => cabvt.BoatName);

                foreach (ClientAgreementBVTerm bVTerm in AgreementBVTerms)
                {
                    intBVNumberOfUnits += 1;

                    DataRow drbv = dtbv.NewRow();
                    DataRow drbv2 = dtbv2.NewRow();
                    DataRow drbv3 = dtbv3.NewRow();
                    DataRow drbv4 = dtbv4.NewRow();
                    DataRow drbv5 = dtbv5.NewRow();

                    drbv["Name"] = bVTerm.BoatName;
                    drbv["Year"] = bVTerm.YearOfManufacture;
                    drbv["Make"] = bVTerm.BoatMake;
                    drbv["Model"] = bVTerm.BoatModel;

                    dtbv.Rows.Add(drbv);

                    drbv2["Name"] = bVTerm.BoatName;
                    drbv2["Year"] = bVTerm.YearOfManufacture;
                    drbv2["Make"] = bVTerm.BoatMake;
                    drbv2["Model"] = bVTerm.BoatModel;
                    drbv2["Sum Insured"] = bVTerm.TermLimit.ToString("C0", CultureInfo.CreateSpecificCulture("en-NZ"));

                    dtbv2.Rows.Add(drbv2);

                    drbv3["Name"] = bVTerm.BoatName;
                    //drbv3["Year"] = bVTerm.YearOfManufacture;
                    //drbv3["Make"] = bVTerm.BoatMake;
                    //drbv3["Model"] = bVTerm.BoatModel;
                    drbv3["Excess"] = bVTerm.Excess.ToString("C0", CultureInfo.CreateSpecificCulture("en-NZ")) + " any one Accident or Occurrence";

                    dtbv3.Rows.Add(drbv3);

                    drbv4["Name"] = bVTerm.BoatName;
                    //drbv4["Year"] = bVTerm.YearOfManufacture;
                    //drbv4["Make"] = bVTerm.BoatMake;
                    //drbv4["Model"] = bVTerm.BoatModel;
                    drbv4["Yacht Racing Risk"] = "Not Included";
                    if (bVTerm.Boat.BoatType1 == "YachtsandCatamarans" && bVTerm.Boat.BoatUse.Where(ycbu => ycbu.BoatUseCategory == "Race" && !ycbu.Removed && ycbu.DateDeleted == null).Count() > 0)
                    {
                        foreach (BoatUse boatuse in bVTerm.Boat.BoatUse)
                        {
                            drbv4["Yacht Racing Risk"] = "Included";
                        }
                    }
                    dtbv4.Rows.Add(drbv4);

                    drbv5["Name"] = bVTerm.BoatName;
                    //drbv4["Year"] = bVTerm.YearOfManufacture;
                    //drbv4["Make"] = bVTerm.BoatMake;
                    //drbv4["Model"] = bVTerm.BoatModel;
                    string strBVInterestPartiesNamesList = null;
                    int intBVIPCount = 0;
                    if (bVTerm.Boat.InterestedParties.Count > 0)
                    {
                        foreach (Organisation InterestParty in bVTerm.Boat.InterestedParties)
                        {
                            if (intBVIPCount == 0)
                            {
                                strBVInterestPartiesNamesList = InterestParty.Name;
                            }
                            else
                            {
                                strBVInterestPartiesNamesList = strBVInterestPartiesNamesList + ", " + InterestParty.Name;
                            }

                            intBVIPCount += 1;
                        }
                    } else
                    {
                        strBVInterestPartiesNamesList = "None";
                    }
                    drbv5["Interest Party"] = strBVInterestPartiesNamesList;
                    dtbv5.Rows.Add(drbv5);

                }

                dtbv.TableName = "BVDetailsTable";
                dtbv2.TableName = "BVDetailsTable2";
                dtbv3.TableName = "BVExcessDetailsTable";
                dtbv4.TableName = "BVRaceDetailsTable";
                dtbv5.TableName = "BVInterestPartyTable";

                mergeFields.Add(new KeyValuePair<string, string>("[[BVNumberOfUnits]]", intBVNumberOfUnits.ToString()));
                mergeFields.Add(new KeyValuePair<string, string>("[[BVDetailsTable]]", ConvertDataTableToHTML(dtbv)));
                mergeFields.Add(new KeyValuePair<string, string>("[[BVDetailsTable2]]", ConvertDataTableToHTML(dtbv2)));
                mergeFields.Add(new KeyValuePair<string, string>("[[BVExcessDetailsTable]]", ConvertDataTableToHTML(dtbv3)));
                mergeFields.Add(new KeyValuePair<string, string>("[[BVRaceDetailsTable]]", ConvertDataTableToHTML(dtbv4)));
                mergeFields.Add(new KeyValuePair<string, string>("[[BVInterestPartyTable]]", ConvertDataTableToHTML(dtbv5)));

                var clientAgreementMVTerm = await _clientAgreementMVTermService.GetAllAgreementMVTermFor(agreement.ClientAgreementTerms.FirstOrDefault(at => at.SubTermType == "BV" && at.DateDeleted == null));
                if (clientAgreementMVTerm.Count() > 0)
                {
                    var AgreementMVTerm = await _clientAgreementMVTermService.GetAllAgreementMVTermFor(agreement.ClientAgreementTerms.FirstOrDefault(at => at.SubTermType == "BV" && at.DateDeleted == null));
                    AgreementMVTerm.OrderBy(camvt => camvt.Registration);

                    foreach (ClientAgreementMVTerm mVTerm in AgreementMVTerm)
                    {

                        DataTable dtmv1 = new DataTable();
                        dtmv1.Columns.Add("Year");
                        dtmv1.Columns.Add("Make");
                        dtmv1.Columns.Add("Model");
                        dtmv1.Columns.Add("Registration");
                        dtmv1.Columns.Add("Sum Insured");

                        AgreementMVTerm = await _clientAgreementMVTermService.GetAllAgreementMVTermFor(agreement.ClientAgreementTerms.FirstOrDefault(at => at.SubTermType == "BV"));
                        AgreementMVTerm.OrderBy(camvt => camvt.Registration);

                        foreach (ClientAgreementMVTerm bVMVTerm in AgreementMVTerm)
                        {
                            DataRow drmv1 = dtmv1.NewRow();

                            drmv1["Year"] = bVMVTerm.Year;
                            drmv1["Make"] = bVMVTerm.Make;
                            drmv1["Model"] = bVMVTerm.Model;
                            drmv1["Registration"] = bVMVTerm.Registration;
                            drmv1["Sum Insured"] = bVMVTerm.TermLimit.ToString("C0", CultureInfo.CreateSpecificCulture("en-NZ"));

                            dtmv1.Rows.Add(drmv1);

                        }

                        dtmv1.TableName = "BVMVDetailsTable";

                        mergeFields.Add(new KeyValuePair<string, string>("[[BVMVDetailsTable]]", ConvertDataTableToHTML(dtmv1)));
                    }
                } else
                {
                    mergeFields.Add(new KeyValuePair<string, string>("[[BVMVDetailsTable]]", "No Trailer insured under this policy."));
                }

                //if (intFinancialIPCount > 1)
                //{
                //    strFinancialIP = strFinancialIPList + " are";
                //} else
                //{
                //    strFinancialIP = strFinancialIPList + " is";
                //}
                //mergeFields.Add(new KeyValuePair<string, string>("[[FinancialIP]]", strBVInterestPartiesNamesList));
            }


            //Client Agreement Rule
            if (agreement.ClientAgreementRules.Count > 0) {
				if (agreement.ClientAgreementRules.FirstOrDefault (cr => cr.Name == "PaymentPremium") != null) {
					string strPaymentPremium = agreement.ClientAgreementRules.FirstOrDefault (cr => cr.Name == "PaymentPremium").Value;
					mergeFields.Add (new KeyValuePair<string, string> ("[[PremiumInclusive]]", strPaymentPremium));
				}

			} else {
				mergeFields.Add (new KeyValuePair<string, string> ("[[PremiumInclusive]]", ""));
			}

			//Endorsements
			if (agreement.ClientAgreementEndorsements.Count > 0) {
				DataTable dt = new DataTable ();
				dt.Columns.Add ("Endorsement Name");
				dt.Columns.Add ("Product Name");
				dt.Columns.Add ("Endorsement Text");

				foreach (ClientAgreementEndorsement ClientAgreementEndorsement in agreement.ClientAgreementEndorsements) {
					DataRow dr = dt.NewRow ();

					dr ["Endorsement Name"] = ClientAgreementEndorsement.Name;
					dr ["Product Name"] = agreement.ClientInformationSheet.Product.Name;
					dr ["Endorsement Text"] = ClientAgreementEndorsement.Value;

					dt.Rows.Add (dr);
				}

				dt.TableName = "EndorsementTable";

				mergeFields.Add (new KeyValuePair<string, string> ("[[EndorsementTable]]", ConvertDataTableToHTML (dt)));
			} else {
				mergeFields.Add (new KeyValuePair<string, string> ("[[EndorsementTable]]", ""));
			}

			// merge the configured merge feilds into the document
			string content = FromBytes (template.Contents);
			foreach (KeyValuePair<string, string> field in mergeFields)
				content = content.Replace (field.Key, field.Value);

			// save the merged content
			doc.Contents = ToBytes (content);

			return (T)doc;
		}



		public byte [] ToBytes (string contents)
		{
			return System.Text.Encoding.UTF8.GetBytes (contents);
		}

		public string FromBytes (byte [] bytes)
		{
			return System.Text.Encoding.UTF8.GetString (bytes);
		}


		public string ConvertDataTableToHTML (DataTable dt)
		{
			decimal deccolumnwidth = 100 / dt.Columns.Count;

			string html = "<table border=\"0\" cellpadding=\"0\" cellspacing=\"5\" style=\"width: 100 % \">";
			//add header row
			html += "<tr valign=\"top\">";
			for (int i = 0; i < dt.Columns.Count; i++)
				html += "<td width=\"" + deccolumnwidth + "%\"><strong>" + dt.Columns [i].ColumnName + "</strong></td>";
			html += "</tr>";
			//add rows
			for (int i = 0; i < dt.Rows.Count; i++) {
				html += "<tr valign=\"top\">";
				for (int j = 0; j < dt.Columns.Count; j++)
					html += "<td>" + dt.Rows [i] [j].ToString () + "</td>";
				html += "</tr>";
			}
			html += "</table>";
			return html;
		}

		#endregion
	}
}

