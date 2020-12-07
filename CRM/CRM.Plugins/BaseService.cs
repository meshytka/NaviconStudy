
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Plugins
{
    public class BaseService
    {
        protected readonly IOrganizationService _service;
        protected readonly ITracingService _tracingService;
        protected readonly string _connectionString = "AuthType=OAuth; Url=https://mikhailmaslov.crm4.dynamics.com/; Username=me_shytka@mikhailmaslov.onmicrosoft.com; Password=Zhjckfd02; RequireNewInstance=true; AppId=51f81489-12ee-4a9e-aaae-a2591f45987d; RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97";

        public BaseService(IOrganizationService service, ITracingService tracingService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tracingService = tracingService ?? throw new ArgumentNullException(nameof(tracingService));
        }
    }
}
