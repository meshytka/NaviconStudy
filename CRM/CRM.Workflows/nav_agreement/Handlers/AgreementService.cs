using CRM.Common.Entities;
using CRM.Workflows.nav_invoice.Handlers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Workflows.nav_agreement.Handlers
{
    public class AgreementService
    {
        private readonly IOrganizationService _service;
        private readonly InvoiceService _invoiceService;

        public AgreementService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _invoiceService = new InvoiceService(service);
        }

        public bool IsAgreementHaveNoAnyInvoice(Guid id)
        {
            return !_invoiceService.IsAnyInvoiceRelatedToAgreement(id);
        }

        public bool IsAgreementHaveAnyInvoiceWhithFactEqTrue(Guid id)
        {
            return _invoiceService.IsAnyInvoiceRelatedToAgreementWhithFactEqTrue(id);
        }

        public bool IsAgreementHaveAnyInvoiceWhithTypeEqManually(Guid id)
        {
            return _invoiceService.IsAnyInvoiceRelatedToAgreementWhithTypeEqManually(id);
        }

        public void DeleteAllArgementInvoiceWhithTypeEqAuthomatic(Guid id)
        {

        }

        public void CreatePaymentScheduleForEachMonth()
        {

        }

        public void SetPaymentPlanDate()
        {

        }
    }
}
