using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Domain.Services.Factories;
using TechCertain.Services.Interfaces;
using techcertain2015rebuildcore.Models.ViewModels.Proposal;

namespace techcertain2015rebuildcore.Controllers
{
    public class ProposalTemplateController : BaseController
    {
        IRepository<ProposalTemplate> _proposalTemplateRepository;
        ProposalTemplateFactory _proposalTemplateFacory;

		public ProposalTemplateController(IUserService userRepository) : base (userRepository)
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
				ProposalTemplate proposalTemplate = _proposalTemplateFacory.CreateProposalTemplate (
					CurrentUser,
					CurrentUser as Owner,
					viewModel.Name, false,
					CurrentUser.Organisations.First ());

                viewModel.Id = proposalTemplate.Id;
            }

            return View(viewModel);
        }
    }
}