using Microsoft.Xrm.Sdk;
using System;

namespace XRM_Plugin
{
    public class AccountDelete : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var service = HelperClass.ObtainOrganization(serviceProvider);

            Entity accountPreEmployee = (Entity)service.Item1.PreEntityImages["PreDeleteImg"];
            Guid accountGuid = ((EntityReference)accountPreEmployee.Attributes["cp_account"]).Id;


            EntityCollection employeeCollection = service.Item2.RetrieveMultiple(HelperClass.Query(accountPreEmployee.LogicalName, accountGuid));

            int? oldMin = null;

            oldMin = HelperClass.MinValue(employeeCollection);

            HelperClass.UpdateAcc(accountGuid, oldMin, service.Item2);


         
        }
    }
}