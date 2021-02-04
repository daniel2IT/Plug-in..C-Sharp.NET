using Microsoft.Xrm.Sdk;
using System;

namespace XRM_Plugin
{
    public class AccountUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var service = HelperClass.ObtainOrganization(serviceProvider);

            //  1. Tikrinam ar keiciasi Priority arba Accountas
            //   2. Jei keiciasi priority -> perskaiciuojam paimdami account is preImage
            //   3. Jei keiciais account -> perskaiciuojam jam priority IR is preImage paimam accounta sena ir jam perskaiciuojam
            Entity accountEmployee = (Entity)service.Item1.InputParameters["Target"];

            if (accountEmployee.Contains("cp_account") || accountEmployee.Contains("cp_priority"))
            {
                Entity accountPreEmployee = (Entity)service.Item1.PreEntityImages["UpdateImg"];
                int? oldMin = null;
                int? newMin = null;

                EntityCollection oldEmployeeCollection;

                if (!accountEmployee.Contains("cp_account") && accountEmployee.Contains("cp_priority"))
                {
                    if (accountPreEmployee.Contains("cp_account"))
                    {
                        Guid accountPreGuid = ((EntityReference)accountPreEmployee.Attributes["cp_account"]).Id;

                        oldEmployeeCollection = service.Item2.RetrieveMultiple(HelperClass.Query(accountPreEmployee.LogicalName, accountPreGuid));
                        oldMin = HelperClass.MinValue(oldEmployeeCollection);

                        HelperClass.UpdateAcc(accountPreGuid, oldMin, service.Item2);
                    }
                }
                else
                {
                    Guid accountPostGuid = ((EntityReference)accountEmployee.Attributes["cp_account"]).Id;
                    EntityCollection newEmployeeCollection = service.Item2.RetrieveMultiple(HelperClass.Query(accountEmployee.LogicalName, accountPostGuid));
                    newMin = HelperClass.MinValue(newEmployeeCollection);

                    if (accountPreEmployee.Contains("cp_account")){ 
                        Guid accountPreGuid = ((EntityReference)accountPreEmployee.Attributes["cp_account"]).Id;
                       
                        if(oldMin == null)
                        {
                            oldEmployeeCollection = service.Item2.RetrieveMultiple(HelperClass.Query(accountEmployee.LogicalName, accountPreGuid));
                            oldMin = HelperClass.MinValue(oldEmployeeCollection);
                        }

                        if (!accountPreGuid.Equals(accountPostGuid))
                        {
                            HelperClass.UpdateAcc(accountPreGuid, oldMin, service.Item2);
                            HelperClass.UpdateAcc(accountPostGuid, newMin, service.Item2);
                        }
                    }
                    else
                    {
                        HelperClass.UpdateAcc(accountPostGuid, newMin, service.Item2);
                    }
                }
            }
        }
    }
}