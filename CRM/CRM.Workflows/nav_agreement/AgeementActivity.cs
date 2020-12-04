using CRM.Common.Entities;
using CRM.Workflows.nav_agreement.Handlers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Workflows.nav_agreement
{
    public class AgeementActivity : CodeActivity
    {
        [Output("Is agreement first")]
        public OutArgument<bool> IsFirst { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var wfContext = context.GetExtension<IWorkflowContext>();

            var id = wfContext.PrimaryEntityId;

            var servicefactory = context.GetExtension<IOrganizationServiceFactory>();
            
            var service = servicefactory.CreateOrganizationService(null);

            AgreementService agreementService = new AgreementService(service);

            IsFirst.Set(context, agreementService.IsAgreementHaveNoAnyInvoice(id));
        }
    }
}
