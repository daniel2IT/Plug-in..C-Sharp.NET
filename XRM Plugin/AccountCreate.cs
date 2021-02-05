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
                EntityCollection employeeCollection = service.Item2.RetrieveMultiple(HelperClass.Query(accountEmployee.LogicalName, accountGuid));
                HelperClass.UpdateAcc(accountGuid, HelperClass.GetMinPriority(employeeCollection), service.Item2);
            }
        }
    }
}