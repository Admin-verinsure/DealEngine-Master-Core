

namespace TechCertain.Infrastructure.Payment.EGlobalAPI
{
    public class EGlobalSerializerAPI : ISerializer
    {
        protected EGlobalAPI EGlobalAPI;

        public bool SiteActive()
        {
            EGlobalAPI = new EGlobalAPI();
            var res = EGlobalAPI.TestSiteStatus();

            if (res != "ACTIVE")
                return false;

            return true;
        }

        /*public static EGlobalAPI GetEGlobalPolicyInvoice(ClientProgramme clientProgramme)
        {
            return GetEGlobalSerializer(clientProgramme).GetEGlobalXML(clientProgramme);
        }

        public static EGlobalAPI GetEGlobalSerializer(ClientProgramme clientProgramme)
        {
            Type serializerType = Type.GetType("TCSerializer." + policy.SchemeProject.Scheme.SerializerClass + ",TCShared");
            return ((BaseEGlobalSerializer)Activator.CreateInstance(serializerType));
        }*/
    }
}

