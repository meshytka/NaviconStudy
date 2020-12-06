using CRM.Workflows.nav_agreement.Handlers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace CRM.Workflows.nav_agreement
{
    public class CreatePaymentPlanActivity : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            var wfContext = context.GetExtension<IWorkflowContext>();

            var id = wfContext.PrimaryEntityId;

            var servicefactory = context.GetExtension<IOrganizationServiceFactory>();

            var service = servicefactory.CreateOrganizationService(null);

            AgreementService agreementService = new AgreementService(service);

            agreementService.DeleteAllArgementInvoiceWhithTypeEqAuthomatic(id);
            agreementService.CreatePaymentScheduleForEachMonth();
            agreementService.SetPaymentPlanDate();
        }
    }
}