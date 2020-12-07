using CRM.Common.Entities;
using CRM.Plugins.nav_agreement.Handlers;
using Microsoft.Xrm.Sdk;
using System;

namespace CRM.Plugins.nav_invoice.Handlers
{
    public class InvoiceService
    {
        private readonly IOrganizationService _service;
        private readonly ITracingService _tracingService;

        public InvoiceService(IOrganizationService service, ITracingService tracingService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tracingService = tracingService ?? throw new ArgumentNullException(nameof(tracingService));
        }

        public void CheckType(Entity targetInvoice)
        {
            if (!targetInvoice.Attributes.Contains("nav_type"))
            {
                _tracingService.Trace("Set nav_type value");
                targetInvoice.Attributes.Add("nav_type", new OptionSetValue((int)nav_invoice_nav_type.__808630000));
            }
        }

        public void PostInvoiceRecalculating(Entity targetInvoice)
        {
            var fact = targetInvoice.GetAttributeValue<bool?>(Common.Entities.nav_invoice.Fields.nav_fact);

            if (fact == true)
            {
                var agreementService = new AgreementService(_service, _tracingService);

                _tracingService.Trace("Set nav_type value");
                agreementService.RecalculateFactSummaAfterPayingInvoice(targetInvoice);

                _service.Delete(targetInvoice.LogicalName, targetInvoice.Id);
            }
        }
    }
}