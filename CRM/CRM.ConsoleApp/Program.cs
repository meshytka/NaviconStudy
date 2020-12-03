using CRM.ConsoleApp.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CRM.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
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

            //UpdateContacts(service);
            UpdateCommunication(service);
        }

        private static void UpdateContacts(IOrganizationService service)
        {
            QueryExpression query = new QueryExpression(nav_communication.EntityLogicalName);
            query.ColumnSet = new ColumnSet(
                nav_communication.Fields.nav_type,
                nav_communication.Fields.nav_phone,
                nav_communication.Fields.nav_email,
                nav_communication.Fields.nav_contactid);

            query.NoLock = true;
            //query.TopCount = 20;
            query.Criteria.AddCondition(nav_communication.Fields.nav_main, ConditionOperator.Equal, true);

            var link = query.AddLink(Contact.EntityLogicalName, nav_communication.Fields.nav_contactid, Contact.Fields.ContactId);
            link.EntityAlias = "co";
            link.Columns = new ColumnSet(Contact.Fields.Telephone1, Contact.Fields.EMailAddress1, Contact.Fields.ContactId);

            var result = service.RetrieveMultiple(query);

            foreach (var entity in result.Entities.Select(e => e.ToEntity<nav_communication>()))
            {
                var contactEmail = (string)entity.GetAttributeValue<AliasedValue>($"{link.EntityAlias}.{Contact.Fields.EMailAddress1}")?.Value;
                var contactPhone = (string)entity.GetAttributeValue<AliasedValue>($"{link.EntityAlias}.{Contact.Fields.Telephone1}")?.Value;
                var id = (Guid)entity.GetAttributeValue<AliasedValue>($"{link.EntityAlias}.{Contact.Fields.ContactId}")?.Value;

                Contact contact = new Contact();
                contact.Id = id;
                contact.ContactId = id;

                if (entity.nav_type == nav_communication_nav_type._ && entity.nav_phone != null && contactPhone == null)
                {
                    contact.Telephone1 = entity.nav_phone;
                    service.Update(contact);
                }
                else if (entity.nav_type == nav_communication_nav_type.Email && entity.nav_email != null && contactEmail == null)
                {
                    contact.EMailAddress1 = entity.nav_email;
                    service.Update(contact);
                }
            }
        }

        private static void UpdateCommunication(IOrganizationService service)
        {
            QueryExpression query = new QueryExpression(Contact.EntityLogicalName);
            query.ColumnSet = new ColumnSet(
                Contact.Fields.ContactId,
                Contact.Fields.Telephone1,
                Contact.Fields.EMailAddress1);

            query.NoLock = true;

            var filter = query.Criteria.AddFilter(LogicalOperator.Or);
            filter.AddCondition(Contact.Fields.Telephone1, ConditionOperator.NotNull);
            filter.AddCondition(Contact.Fields.EMailAddress1, ConditionOperator.NotNull);



            var link = query.AddLink(nav_communication.EntityLogicalName, Contact.Fields.ContactId, nav_communication.Fields.nav_contactid);
            link.EntityAlias = "com";
            link.Columns = new ColumnSet(nav_communication.Fields.nav_phone, nav_communication.Fields.nav_email);

            var linkFiler = query.Criteria.AddFilter(LogicalOperator.And);
            linkFiler.AddCondition(Contact.Fields.Telephone1, ConditionOperator.NotEqual, nav_communication.Fields.nav_phone);
            linkFiler.AddCondition(Contact.Fields.EMailAddress1, ConditionOperator.NotEqual, nav_communication.Fields.nav_email);

            var result = service.RetrieveMultiple(query);

            var contact = result.Entities.Select(e => e.ToEntity<Contact>());

            List<Guid> listOdId = new List<Guid>();

            foreach (var entity in result.Entities.Select(e => e.ToEntity<Contact>()))
            {
                if (listOdId.Any(id => id == entity.Id))
                    continue;

                if (entity.EMailAddress1 != null && entity.Telephone1 != null)
                {
                    service.Create(CreateCommunication(nav_communication_nav_type.Email, entity.EMailAddress1));
                    service.Create(CreateCommunication(nav_communication_nav_type._, entity.Telephone1));
                }
                else if (entity.EMailAddress1 != null && entity.Telephone1 == null)
                {
                    service.Create(CreateCommunication(nav_communication_nav_type.Email, entity.EMailAddress1));
                }
                else if (entity.EMailAddress1 == null && entity.Telephone1 != null)
                {
                    service.Create(CreateCommunication(nav_communication_nav_type._, entity.Telephone1));
                }

                listOdId.Add(entity.Id);
            }
        }

        private static nav_communication CreateCommunication(nav_communication_nav_type type, string commValue)
        {
            nav_communication communication = new nav_communication();

            if (type == nav_communication_nav_type.Email)
            {
                communication.nav_email = commValue;
                communication.nav_main = false;
            }
            else
            {
                communication.nav_phone = commValue;
                communication.nav_main = true;
            }

            return communication;
        }
    }
}
