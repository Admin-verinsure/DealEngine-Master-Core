/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';

	config.skin = 'bootstrapck';

	// Enable token plugin
	//config.extraPlugins = 'token';

	// Configure available tokens
	config.availableTokens = [
	    ["", ""],
	    ["Full Name", "FullName"],
	    ["First Name", "FirstName"],
	    ["Last Name", "LastName"],
        ["Insured Name", "InsuredName"],
        ["Contact Broker Name", "BrokerName"],
	    ["Inception Date", "InceptionDate"],
	    ["Expiry Date", "ExpiryDate"],
	    ["Bound or Quote Date", "BoundOrQuoteDate"],
	    ["Limit (inclusive)", "LimitInclusive"],
	    ["Limit (exclusive)", "LimitExclusive"],
	    ["Premium (inclusive)", "PremiumInclusive"],
	    ["Premium (exclusive)", "PremiumExclusive"],
	    ["Excess", "Excess"],
	    ["Retroactive Date (Motor Vehicle)", "RetroactiveDate_MV"],
	    ["Limit (Motor Vehicle)", "BoundLimit_MV"],
        ["Excess (Motor Vehicle)", "BoundExcess_MV"],
        ["Information Sheet Reference", "Reference"],
        ["Vessel Details", "BVDetailsTable2"],
        ["Vessel Excess Details", "BVExcessDetailsTable"],
        ["Vessel Race Use Details", "BVRaceDetailsTable"],
        ["Vessel Interest Party Details", "BVInterestPartyTable"],
        ["Trailer Details", "BVMVDetailsTable"],
        ["Administration Fee", "AdministrationFee"],
        ["Bound Limit (Vessel)", "BoundLimit_BV"],
        ["Bound Premium (Vessel)", "BoundPremiumAdjustment_BV"],
        ["Bound FSL (Vessel)", "BoundFSL_BV"],
        ["Bound Premium GST Admin Fee Incl (Vessel)", "BoundPremiumInclFeeGST_BV"],
        ["Bound Total Premium Admin Fee Incl (Vessel)", "BoundPremiumInclFeeInclGST_BV"],
        ["Credit Card Surcharge (Vessel)", "CreditCardSurcharge_BV"],
        ["Credit Card Type", "CreditCardType"],
        ["Credit Card Number ()", "CreditCardNumber"],
        ["Bound Premium GST Admin Fee CC Surcharge Incl (Vessel)", "BoundPremiumInclFeeCCSurchargeGST_BV"],
        ["Bound Total Premium Admin Fee CC Surcharge Incl (Vessel)", "BoundPremiumInclGSTCreditCardCharge_BV"],
        ["EGlobal Client Branch Code", "ClientBranchCode"],
        ["EGlobal Client Number", "ClientNumber"],
        ["EGlobal Invoice Number", "InvoiceReference"],
        ["EGlobal Invoice Cover Number", "CoverNo"],
        ["EGlobal Invoice Version Number", "Version"],
        
	    //["Retroactive Date ()", "RetroactiveDate_"],
	    //["Limit ()", "BoundLimit_"],
	    //["Excess ()", "BoundExcess_"],
	];

	// Configure token string
	config.tokenStart = '[[';
	config.tokenEnd = ']]';

	// Configure Size
	config.width = "100%";
	config.height = "300";
};
