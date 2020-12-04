using CRM.Common.Entities;
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
        public AgreementService(IOrganizationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public bool IsAgreementHaveNoAnyInvoice(Guid id)
        {
            var query = GetQueryExpressionReturnsAllAgreementInvoice(id);

            query.ColumnSet = new ColumnSet(nav_invoice.Fields.Id);

            var result = _service.RetrieveMultiple(query);

            return result.Entities.Count() > 0 ? true : false;
        }

        public bool IsAgreementHaveAnyInvoiceWhithFactEqTrue(Guid id)
        {
            var query = GetQueryExpressionReturnsAllAgreementInvoice(id);

            query.ColumnSet = new ColumnSet(nav_invoice.Fields.Id);

            query.Criteria.AddCondition(nav_communication.Fields.nav_main, ConditionOperator.Equal, true);

            var result = _service.RetrieveMultiple(query);

            return result.Entities.Count() > 0 ? true : false;
        }

        public bool IsAgreementHaveAnyInvoiceWhithTypeEqManually(Guid id)
        {
            var query = GetQueryExpressionReturnsAllAgreementInvoice(id);

            query.ColumnSet = new ColumnSet(nav_invoice.Fields.Id);

            query.Criteria.AddCondition(
                nav_invoice.Fields.nav_type, 
                ConditionOperator.Equal, 
                new OptionSetValue((int) nav_invoice_nav_type.__808630000));

            var result = _service.RetrieveMultiple(query);

            return result.Entities.Count() > 0 ? true : false;
        }

        public void DeleteAllArgementInvoiceWhithTypeEqAuthomatic(Guid id)
        {

        }

        public void CreatePaymentScheduleForEachMonth()
        {

        }

        public QueryExpression GetQueryExpressionReturnsAllAgreementInvoice(Guid id)
        {
            QueryExpression query = new QueryExpression(nav_invoice.EntityLogicalName);

            query.NoLock = true;

            query.AddLink(Common.Entities.nav_agreement.EntityLogicalName, Common.Entities.nav_agreement.Fields.Id, Common.Entities.nav_agreement.Fields.nav_agreementId);

            query.Criteria.AddCondition(Common.Entities.nav_agreement.Fields.Id, ConditionOperator.Equal, id);

            return query;
        }
    }
}
