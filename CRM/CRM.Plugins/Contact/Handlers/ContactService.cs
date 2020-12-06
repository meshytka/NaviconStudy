using CRM.Common.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Net;
using System.Text;

namespace CRM.Plugins.Contact.Handlers
{
    public class ContactService
    {
        private readonly IOrganizationService _service;
        private readonly ITracingService _tracingService;

        public ContactService(IOrganizationService service, ITracingService tracingService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tracingService = tracingService ?? throw new ArgumentNullException(nameof(tracingService));
        }

        public bool IsAnyAgreement(LinkEntity entity)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            System.Console.OutputEncoding = Encoding.UTF8;

            var connectionString = "AuthType=OAuth; Url=https://mikhailmaslov.crm4.dynamics.com/; Username=me_shytka@mikhailmaslov.onmicrosoft.com; Password=Zhjckfd02; RequireNewInstance=true; AppId=51f81489-12ee-4a9e-aaae-a2591f45987d; RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97";

            CrmServiceClient client = new CrmServiceClient(connectionString);
            if (client.LastCrmException != null)
            {
                System.Console.WriteLine(client.LastCrmException);
            }

            var service = (IOrganizationService)client;

            QueryExpression query = new QueryExpression(nav_communication.EntityLogicalName);
            query.ColumnSet = new ColumnSet(
                nav_communication.Fields.nav_type,
                nav_communication.Fields.nav_phone,
                nav_communication.Fields.nav_email,
                nav_communication.Fields.nav_contactid);

            query.NoLock = true;
            //query.TopCount = 20;
            query.Criteria.AddCondition(nav_communication.Fields.nav_main, ConditionOperator.Equal, true);

            var link = query.AddLink(Common.Entities.Contact.EntityLogicalName, nav_communication.Fields.nav_contactid, Common.Entities.Contact.Fields.ContactId);
            link.EntityAlias = "co";
            link.Columns = new ColumnSet(Common.Entities.Contact.Fields.Telephone1, Common.Entities.Contact.Fields.EMailAddress1, Common.Entities.Contact.Fields.ContactId);

            var result = service.RetrieveMultiple(query);

            return true;
        }

        public void SetDateOfFrstAgreement(LinkEntity entity, DateTime date)
        { }
    }
}