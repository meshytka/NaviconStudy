using CRM.Common.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                targetInvoice.Attributes.Add("nav_type", new OptionSetValue((int)nav_invoice_nav_type.__808630000));
                _tracingService.Trace("Set nav_type value");
            }
        }
    }
}
