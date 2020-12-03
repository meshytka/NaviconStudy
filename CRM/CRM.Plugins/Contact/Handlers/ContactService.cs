using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return true;
        }

        public void SetDateOfFrstAgreement(LinkEntity entity, DateTime date)
        { }
    }
}
