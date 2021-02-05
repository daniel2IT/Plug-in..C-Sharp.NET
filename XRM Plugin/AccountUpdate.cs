using Microsoft.Xrm.Sdk;
using System;

namespace XRM_Plugin
{
    public class AccountUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var service = HelperClass.ObtainOrganization(serviceProvider);
            Entity accountEmployee = (Entity)service.Item1.InputParameters["Target"];

            if (accountEmployee.Contains("cp_account") || accountEmployee.Contains("cp_priority"))
            {
                Entity preAccountEmployee = (Entity)service.Item1.PreEntityImages["UpdateImg"];
                EntityCollection oldEmployeeCollection;

                if (!accountEmployee.Contains("cp_account") && accountEmployee.Contains("cp_priority"))
                {
                    if (preAccountEmployee.Contains("cp_account"))
                    {
                        Guid accountPreGuid = ((EntityReference)preAccountEmployee.Attributes["cp_account"]).Id;

                        oldEmployeeCollection = service.Item2.RetrieveMultiple(HelperClass.Query(preAccountEmployee.LogicalName, accountPreGuid));

                        HelperClass.UpdateAcc(accountPreGuid, HelperClass.GetMinPriority(oldEmployeeCollection), service.Item2);
                    }
                }
                else
                {
                    Guid accountPostGuid = ((EntityReference)accountEmployee.Attributes["cp_account"]).Id;
                    EntityCollection newEmployeeCollection = service.Item2.RetrieveMultiple(HelperClass.Query(accountEmployee.LogicalName, accountPostGuid));

                    if (preAccountEmployee.Contains("cp_account"))
                    { 
                        Guid accountPreGuid = ((EntityReference)preAccountEmployee.Attributes["cp_account"]).Id;
                        oldEmployeeCollection = service.Item2.RetrieveMultiple(HelperClass.Query(accountEmployee.LogicalName, accountPreGuid));

                        if (!accountPreGuid.Equals(accountPostGuid))
                        {
                            HelperClass.UpdateAcc(accountPreGuid, HelperClass.GetMinPriority(oldEmployeeCollection), service.Item2);
                            HelperClass.UpdateAcc(accountPostGuid, HelperClass.GetMinPriority(newEmployeeCollection), service.Item2);
                        }
                    }
                    else
                    {
                        HelperClass.UpdateAcc(accountPostGuid, HelperClass.GetMinPriority(newEmployeeCollection), service.Item2);
                    }
                }
            }
        }
    }
}