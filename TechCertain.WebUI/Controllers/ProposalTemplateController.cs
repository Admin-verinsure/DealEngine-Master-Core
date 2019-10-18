using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;
using DealEngine.Infrastructure.Identity.Data;
using TechCertain.WebUI.Models.Proposal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace TechCertain.WebUI.Controllers
{
    public class ProposalTemplateController : BaseController
    {
        IMapperSession<ProposalTemplate> _proposalTemplateRepository;
        IHttpContextAccessor _httpContextAccessor;

        public ProposalTemplateController(IUserService userRepository, 
            DealEngineDBContext dealEngineDBContext, 
            IHttpContextAccessor httpContextAccessor, 
            SignInManager<DealEngineUser> signInManager) : base (userRepository)
        {

        }

        [HttpGet]
        public ActionResult Index()
        {
            var proposalTemplates = _proposalTemplateRepository.FindAll().Where(pt => !pt.IsPrivate).ToArray();

            throw new Exception("Method will need to be re-written");
            //var viewModel = Mapper.Map<ProposalTemplateIndexViewModel>(proposalTemplates);

            //return View(viewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new CreateProposalTemplateViewModel());
        }

        [HttpPost]
        public ActionResult Create(CreateProposalTemplateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            if (viewModel.Id == Guid.Empty)
            {
                ProposalTemplate proposalTemplate = new ProposalTemplate(
                    CurrentUser,
                    CurrentUser as Owner,
                    viewModel.Name, false);
                    
                    viewModel.Id = proposalTemplate.Id;
            }

            return View(viewModel);
        }
    }
}