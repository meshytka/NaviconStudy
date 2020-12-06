using CRM.Plugins.nav_invoice.Handlers;
using Microsoft.Xrm.Sdk;
using System;

namespace CRM.Plugins.nav_invoice
{
    public sealed class PreInvoiceCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var traceService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            traceService.Trace("Info about trace");

            var pluginContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var targetInvoice = (Entity)pluginContext.InputParameters["Target"];

            traceService.Trace("Name " + targetInvoice.LogicalName);

            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(Guid.Empty);// null

            try
            {
                InvoiceService invoiceService = new InvoiceService(service, traceService);

                invoiceService.CheckType(targetInvoice);
            }
            catch (Exception exc)
            {
                traceService.Trace(exc.ToString());

                throw new InvalidPluginExecutionException(exc.Message);
            }
        }
    }
}