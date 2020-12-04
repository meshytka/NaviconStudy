using CRM.Common.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Workflows.nav_invoice.Handlers
{
    public class InvoiceService
    {
        private readonly IOrganizationService _service;
        public InvoiceService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void DeleteAllArgementInvoiceWhithTypeEqAuthomatic(Guid id)
        {

        }

        public void CreatePaymentScheduleForEachMonth()
        {

        }

        public QueryExpression GetQueryExpressionReturnsAllInvoicesRelatedToAgreement(Guid id)
        {
            QueryExpression query = new QueryExpression(Common.Entities.nav_invoice.EntityLogicalName);

            query.NoLock = true;

            query.AddLink(Common.Entities.nav_agreement.EntityLogicalName, Common.Entities.nav_agreement.Fields.Id, Common.Entities.nav_agreement.Fields.nav_agreementId);

            query.Criteria.AddCondition(Common.Entities.nav_agreement.Fields.Id, ConditionOperator.Equal, id);

            return query;
        }

        public bool IsAnyInvoiceRelatedToAgreementWhithTypeEqManually(Guid agreementId)
        {
            var query = GetQueryExpressionReturnsAllInvoicesRelatedToAgreement(agreementId);

            query.ColumnSet = new ColumnSet(Common.Entities.nav_invoice.Fields.Id);

            query.Criteria.AddCondition(
                Common.Entities.nav_invoice.Fields.nav_type,
                ConditionOperator.Equal,
                new OptionSetValue((int)nav_invoice_nav_type.__808630000));

            var result = _service.RetrieveMultiple(query);

            return result.Entities.Count() > 0 ? true : false;
        }

        public bool IsAnyInvoiceRelatedToAgreement(Guid agreementId)
        {
            var query = GetQueryExpressionReturnsAllInvoicesRelatedToAgreement(agreementId);

            query.ColumnSet = new ColumnSet(Common.Entities.nav_invoice.Fields.Id);

            var result = _service.RetrieveMultiple(query);

            return result.Entities.Count() > 0 ? true : false;
        }

        public bool IsAnyInvoiceRelatedToAgreementWhithFactEqTrue(Guid agreementId)
        {
            var query = GetQueryExpressionReturnsAllInvoicesRelatedToAgreement(agreementId);

            query.ColumnSet = new ColumnSet(Common.Entities.nav_invoice.Fields.Id);

            query.Criteria.AddCondition(nav_communication.Fields.nav_main, ConditionOperator.Equal, true);

            var result = _service.RetrieveMultiple(query);

            return result.Entities.Count() > 0 ? true : false;
        }
    }
}
