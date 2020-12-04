using CRM.Workflows.nav_agreement.Handlers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Workflows.nav_agreement
{
    public class CreatePaymentPlanActivity : CodeActivity
    {
        [Output("Is agreement have any invoice whith fact eq true")]
        public OutArgument<bool> IsAnyInvoiceWhithFactEqTrue { get; set; }

        [Output("Is agreement Have any invoice whith type eq manually")]
        public OutArgument<bool> IsAnyInvoiceWhithTypeEqManually { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var wfContext = context.GetExtension<IWorkflowContext>();

            var id = wfContext.PrimaryEntityId;

            var servicefactory = context.GetExtension<IOrganizationServiceFactory>();

            var service = servicefactory.CreateOrganizationService(null);

            AgreementService agreementService = new AgreementService(service);

            IsAnyInvoiceWhithFactEqTrue.Set(context, agreementService.IsAgreementHaveAnyInvoiceWhithFactEqTrue(id));
            IsAnyInvoiceWhithTypeEqManually.Set(context, agreementService.IsAgreementHaveAnyInvoiceWhithTypeEqManually(id));

        }
    }
}
