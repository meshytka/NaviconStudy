using CRM.Common.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Net;
using System.Text;

namespace CRM.Plugins.Contact.Handlers
{
    public class ContactService : BaseService
    {
        public ContactService(IOrganizationService service, ITracingService tracingService)
            : base(service, tracingService)
        {
        }

        public bool IsFirstAgreement(Guid contactId, Guid agreementId)
        {
            QueryExpression query = new QueryExpression(Common.Entities.nav_agreement.EntityLogicalName);
            query.ColumnSet = new ColumnSet(Common.Entities.nav_agreement.Fields.Id);

            query.NoLock = true;

            var link = query.AddLink(Common.Entities.Contact.EntityLogicalName, Common.Entities.nav_agreement.Fields.nav_contact, Common.Entities.Contact.Fields.Id);
            link.EntityAlias = "co";

            query.Criteria.AddCondition(Common.Entities.Contact.Fields.Id, ConditionOperator.Equal, contactId);
            query.Criteria.AddCondition(Common.Entities.nav_agreement.Fields.Id, ConditionOperator.NotEqual, agreementId);


            var result = _service.RetrieveMultiple(query);

            if (result.TotalRecordCount == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetDateOfFrstAgreement(Guid contactId, DateTime? date)
        {
            Common.Entities.Contact contact = new Common.Entities.Contact()
            {
                Id = contactId,
                nav_date = date
            };

            _service.Update(contact);
        }
    }
}