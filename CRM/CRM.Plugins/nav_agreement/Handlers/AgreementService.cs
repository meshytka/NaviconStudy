﻿using CRM.Common.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Plugins.nav_agreement.Handlers
{
    public class AgreementService
    {
        private readonly IOrganizationService _service;
        private readonly ITracingService _tracingService;

        public AgreementService(IOrganizationService service, ITracingService tracingService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tracingService = tracingService ?? throw new ArgumentNullException(nameof(tracingService));
        }

        public void SetContractDate(Entity targetAgreement)
        {
            if (targetAgreement.Attributes.Contains("nav_date"))
            {
                var date = targetAgreement.GetAttributeValue<DateTime>("nav_date");

                
            }
        }
    }
}
