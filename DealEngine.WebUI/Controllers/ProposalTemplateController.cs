using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;
using DealEngine.WebUI.Models.Proposal;

namespace DealEngine.WebUI.Controllers
{
    public class ProposalTemplateController : BaseController
    {
        IMapperSession<ProposalTemplate> _proposalTemplateRepository;

        public ProposalTemplateController(IUserService userRepository) : base (userRepository)
        {

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var proposalTemplates = _proposalTemplateRepository.FindAll().Where(pt => !pt.IsPrivate).ToArray();

            throw new Exception("Method will need to be re-written");
            //var viewModel = Mapper.Map<ProposalTemplateIndexViewModel>(proposalTemplates);

            //return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new CreateProposalTemplateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProposalTemplateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var user = await CurrentUser();
            if (viewModel.Id == Guid.Empty)
            {
                ProposalTemplate proposalTemplate = new ProposalTemplate(
                    user,
                    user as Owner,
                    viewModel.Name, false);
                    
                    viewModel.Id = proposalTemplate.Id;
            }

            return View(viewModel);
        }
    }
}