using TechCertain.Services.Interfaces;
using TechCertain.Domain.Entities;


namespace TechCertain.Services.Impl.UnderwritingModuleServices
{
    public class EmptyUWModule : IUnderwritingModule
    {
        public EmptyUWModule()
        {
        }

        public string Name
        {
            get
            {
                return "NoUnderwrite";
            }
        }

        public bool Underwrite(User underwritingUser, ClientInformationSheet informationSheet)
        {
            return true;
        }

        public bool Underwrite(User underwritingUser, ClientInformationSheet informationSheet, Product product, string reference)
        {
            return true;
        }
    }
}

