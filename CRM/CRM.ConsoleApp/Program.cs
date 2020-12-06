using CRM.ConsoleApp.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;

namespace CRM.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logger.InitLogger();//инициализация - требуется один раз в начале
            Logger.Log.Info("Старт работы программы");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            System.Console.OutputEncoding = Encoding.UTF8;

            var connectionString = ConfigurationManager.ConnectionStrings["DynamicsDB"].ConnectionString;

            CrmServiceClient client = new CrmServiceClient(connectionString);
            if (client.LastCrmException != null)
            {
                Logger.Log.Error($"Ошибка Common Data Service {client.LastCrmException}!");
            }

            var service = (IOrganizationService)client;

            try
            {
                UpdateContacts(service);
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Произошла ошибка при обновлении контактов {ex.Message}");
            }

            try
            {
                UpdateCommunication(service);
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Произошла ошибка при обновлении средств связи {ex.Message}");
            }

            Logger.Log.Info("Конец работы программы");
        }

        private static void UpdateContacts(IOrganizationService service)
        {
            Logger.Log.Info("Начало процесса обновления контактов");

            Logger.Log.Info("Формирование запроса к БД");
            QueryExpression query = new QueryExpression(nav_communication.EntityLogicalName);
            query.ColumnSet = new ColumnSet(
                nav_communication.Fields.nav_type,
                nav_communication.Fields.nav_phone,
                nav_communication.Fields.nav_email,
                nav_communication.Fields.nav_contactid);

            query.NoLock = true;
            query.Criteria.AddCondition(nav_communication.Fields.nav_main, ConditionOperator.Equal, true);

            var link = query.AddLink(Contact.EntityLogicalName, nav_communication.Fields.nav_contactid, Contact.Fields.ContactId);
            link.EntityAlias = "co";
            link.Columns = new ColumnSet(Contact.Fields.Telephone1, Contact.Fields.EMailAddress1, Contact.Fields.ContactId);

            Logger.Log.Info("Запрос к БД");
            var result = service.RetrieveMultiple(query);

            Logger.Log.Info("Начало обнавления контактов на основе полученной информации");
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

                    Logger.Log.Info($"Обновление телефона у контакта Id = {contact.Id}");
                    service.Update(contact);
                    Logger.Log.Info($"Контакт Id = {contact.Id} успешно обновлен");
                }
                else if (entity.nav_type == nav_communication_nav_type.Email && entity.nav_email != null && contactEmail == null)
                {
                    Logger.Log.Info($"Обновление email у контакта Id = {contact.Id}");

                    contact.EMailAddress1 = entity.nav_email;
                    service.Update(contact);

                    Logger.Log.Info($"Контакт Id = {contact.Id} успешно обновлен");
                }
            }

            Logger.Log.Info("Все контакты успешно обновлены");
        }

        private static void UpdateCommunication(IOrganizationService service)
        {
            Logger.Log.Info("Начало процесса обновления средств связи");

            Logger.Log.Info("Формирование запроса к БД");
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

            Logger.Log.Info("Запрос к БД");
            var result = service.RetrieveMultiple(query);

            List<Guid> listOdId = new List<Guid>();

            Logger.Log.Info("Начало обнавления средств связи на основе полученной информации");
            foreach (var entity in result.Entities.Select(e => e.ToEntity<Contact>()))
            {
                if (listOdId.Any(id => id == entity.Id))
                    continue;

                if (entity.EMailAddress1 != null && entity.Telephone1 != null)
                {
                    Logger.Log.Info($"Создание средств связи с email {entity.EMailAddress1} и телефоном {entity.Telephone1}");

                    service.Create(CreateCommunication(nav_communication_nav_type.Email, entity.EMailAddress1));
                    service.Create(CreateCommunication(nav_communication_nav_type._, entity.Telephone1));
                }
                else if (entity.EMailAddress1 != null && entity.Telephone1 == null)
                {
                    Logger.Log.Info($"Создание средства связи с email {entity.EMailAddress1}");

                    service.Create(CreateCommunication(nav_communication_nav_type.Email, entity.EMailAddress1));
                }
                else if (entity.EMailAddress1 == null && entity.Telephone1 != null)
                {
                    Logger.Log.Info($"Создание средства связи с телефоном {entity.Telephone1}");
                    service.Create(CreateCommunication(nav_communication_nav_type._, entity.Telephone1));
                }

                listOdId.Add(entity.Id);
            }

            Logger.Log.Info("Средства связи успешно обновлены");
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