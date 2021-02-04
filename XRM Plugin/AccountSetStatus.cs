using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;

namespace XRM_Plugin
{
    public class AccountSetStatus : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                var service = HelperClass.ObtainOrganization(serviceProvider);

                Entity accountPostEmployee = (Entity)service.Item1.PostEntityImages["PostSetStateImg"];
                Guid accountGuid = ((EntityReference)accountPostEmployee.Attributes["cp_account"]).Id;

                int? oldMin = null;

                EntityCollection employeeCollection = service.Item2.RetrieveMultiple(HelperClass.Query(accountPostEmployee.LogicalName, accountGuid));
                oldMin = HelperClass.MinValue(employeeCollection);

                HelperClass.UpdateAcc(accountGuid, oldMin, service.Item2);
            }
            catch (InvalidPluginExecutionException)
            {
                throw;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException("An FaultException occurred in the SetStatePlugin plug-in.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("An Exception occurred in the SetStatePlugin plug-in.", ex);
            }
        }
    }
}
