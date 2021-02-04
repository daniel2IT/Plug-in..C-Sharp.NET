using Microsoft.Xrm.Sdk;
using System;

namespace XRM_Plugin
{
    public class AccountCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var service = HelperClass.ObtainOrganization(serviceProvider);
            
            Entity accountEmployee = (Entity)service.Item1.InputParameters["Target"];

            if (accountEmployee.Contains("cp_account") && accountEmployee.Contains("cp_priority"))
            {
                Guid accountGuid = ((EntityReference)accountEmployee.Attributes["cp_account"]).Id;

                int? oldMin = null;

                EntityCollection employeeCollection = service.Item2.RetrieveMultiple(HelperClass.Query(accountEmployee.LogicalName, accountGuid));
                oldMin = HelperClass.MinValue(employeeCollection);

                HelperClass.UpdateAcc(accountGuid, oldMin, service.Item2);
            }
        }
    }
}