using CRM.Plugins.Contact.Handlers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Net;
using System.Text;

namespace CRM.Plugins.nav_agreement.Handlers
{
    public class AgreementService : BaseService
    {
        public AgreementService(IOrganizationService service, ITracingService tracingService)
            : base(service, tracingService)
        { }

        public void SetContractDate(Entity targetAgreement)
        {
            if (targetAgreement.Attributes.Contains("nav_contact") && targetAgreement.Attributes.Contains("nav_date"))
            {
                var agreement = targetAgreement.ToEntity<Common.Entities.nav_agreement>();

                var serviceContact = new ContactService(_service, _tracingService);
                var contactId = (Guid)agreement.GetAttributeValue<AliasedValue>($"{Common.Entities.Contact.EntityLogicalName}.{Common.Entities.Contact.Fields.Id}")?.Value;

                if (serviceContact.IsFirstAgreement(contactId, agreement.Id))
                {
                    var date = targetAgreement.GetAttributeValue<DateTime>("nav_date");

                    serviceContact.SetDateOfFrstAgreement(contactId, date);
                }
            }
        }

        public void RecalculateFactSummaAfterPayingInvoice(Entity newInvoice)
        {
            var invoice = newInvoice.ToEntity<Common.Entities.nav_invoice>();
            var ageementId = (Guid) invoice.GetAttributeValue<AliasedValue>($"{Common.Entities.nav_agreement.EntityLogicalName}.{Common.Entities.nav_agreement.Fields.Id}")?.Value;

            if (invoice.nav_amount.Value != 0)
            {
                QueryExpression query = new QueryExpression(Common.Entities.nav_agreement.EntityLogicalName);
                query.ColumnSet = new ColumnSet(Common.Entities.nav_agreement.Fields.Id, Common.Entities.nav_agreement.Fields.nav_factsumma, Common.Entities.nav_agreement.Fields.nav_fact);

                query.NoLock = true;

                var link = query.AddLink(Common.Entities.nav_agreement.EntityLogicalName, Common.Entities.Contact.Fields.Id, Common.Entities.nav_agreement.Fields.nav_contact);
                link.EntityAlias = "co";

                var result = _service.RetrieveMultiple(query);
            }
        }
    }
}