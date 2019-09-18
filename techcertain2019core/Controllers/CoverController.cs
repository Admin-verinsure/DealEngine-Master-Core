using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

namespace techcertain2019core.Controllers
{
	public class CoverController 
	{
        //private readonly IInsuranceProductService _insuranceProductService;

        //public CoverController (IInsuranceProductService insuranceProductService)
        //{
        //    _insuranceProductService = insuranceProductService;
        //}

        //public ActionResult Index ()
        //{
        //    return View ();
        //}

        //public ActionResult Editing_Popup ()
        //{
        //    return View ();
        //}

        //public ActionResult EditingPopup_Read ([DataSourceRequest] DataSourceRequest request)
        //{
        //    IEnumerable<InsuranceProductViewModel> items = _insuranceProductService.GetAllInsuranceProducts ()
        //        .Select (x => new InsuranceProductViewModel {
        //        Code = x.Code,
        //        Description = x.Description,
        //        EnableRetroactiveDate = x.EnableRetroactiveDate,
        //        Name = x.Name,
        //        ID = x.ID				
        //    });

        //    return Json (items.ToDataSourceResult (request));
        //}

        //[AcceptVerbs (HttpVerbs.Post)]
        //public ActionResult EditingPopup_Create ([DataSourceRequest] DataSourceRequest request, InsuranceProductViewModel insuranceProduct)
        //{
        //    if (insuranceProduct != null && ModelState.IsValid) {

        //        _insuranceProductService.CreateInsuranceProduct (new InsuranceProduct () {
        //            Code = insuranceProduct.Code, 
        //            Description = insuranceProduct.Description, 
        //            EnableRetroactiveDate = insuranceProduct.EnableRetroactiveDate,
        //            Name = insuranceProduct.Name,
        //            ID = insuranceProduct.ID
        //        });
        //    } 

        //    return Json (new[] { insuranceProduct }.ToDataSourceResult (request, ModelState));
        //}

        //[AcceptVerbs (HttpVerbs.Post)]
        //public ActionResult EditingPopup_Update ([DataSourceRequest] DataSourceRequest request, InsuranceProductViewModel insuranceProduct)
        //{
        //    if (insuranceProduct != null && ModelState.IsValid) {

        //        InsuranceProduct updateInsuranceProduct = _insuranceProductService.GetInsuranceProduct (insuranceProduct.ID);

        //        updateInsuranceProduct.Code = insuranceProduct.Code;
        //        updateInsuranceProduct.Description = insuranceProduct.Description;
        //        updateInsuranceProduct.EnableRetroactiveDate = insuranceProduct.EnableRetroactiveDate;
        //        updateInsuranceProduct.ID = insuranceProduct.ID;
        //        updateInsuranceProduct.Name = insuranceProduct.Name;

        //        _insuranceProductService.UpdateInsuranceProduct (updateInsuranceProduct);
        //    }			

        //    return Json (new[] { insuranceProduct }.ToDataSourceResult (request, ModelState));
        //}

        //[AcceptVerbs (HttpVerbs.Post)]
        //public ActionResult EditingPopup_Destroy ([DataSourceRequest] DataSourceRequest request, InsuranceProductViewModel insuranceProduct)
        //{
        //    if (insuranceProduct != null) {

        //        InsuranceProduct updateProduct = _insuranceProductService.GetInsuranceProduct (insuranceProduct.ID);
        //        updateProduct.DeletedDate = DateTime.UtcNow;

        //        _insuranceProductService.DeleteInsuranceProduct (updateProduct);
        //    }

        //    return Json (new[] { insuranceProduct }.ToDataSourceResult (request, ModelState));
        //}
	}
}
