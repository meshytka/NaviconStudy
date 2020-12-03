using CRM.Common.Entities;
using CRM.Plugins.nav_agreement.Handlers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Plugins.nav_agreement
{
    public class PostAgreementCreateAsynk : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var traceService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            traceService.Trace("Info about trace");

            var pluginContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var targetAgreement = (Entity)pluginContext.InputParameters["Target"];

            traceService.Trace("Name " + targetAgreement.GetAttributeValue<string>(Common.Entities.nav_agreement.Fields.nav_name));

            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(Guid.Empty);// null

            try
            {
                AgreementService agreementService = new AgreementService(service, traceService);

                agreementService.SetContractDate(targetAgreement);
            }
            catch (Exception exc)
            {
                traceService.Trace(exc.ToString());

                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}
