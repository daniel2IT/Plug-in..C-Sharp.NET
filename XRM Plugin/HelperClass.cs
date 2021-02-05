using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace XRM_Plugin
{
    public class HelperClass
    {
        public static Tuple<IPluginExecutionContext, IOrganizationService> ObtainOrganization(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            return Tuple.Create(context, service);
        }

        public static QueryExpression Query(string LogicalName, Guid accountId)
        {
            QueryExpression query = new QueryExpression(LogicalName);

            query.ColumnSet.AddColumns("cp_priority", "cp_account");
            query.Criteria.AddCondition("cp_priority", ConditionOperator.NotNull);
            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("cp_account", ConditionOperator.Equal, accountId);

            return query;
        }

        //Find Min Value
        public static int? GetMinPriority(EntityCollection employeeCollection)
        {
            int? minPriority = null;

            if (employeeCollection.Entities.Count != 0)
            {
                minPriority = (int)employeeCollection.Entities[0].GetAttributeValue<OptionSetValue>("cp_priority").Value;

                foreach (var contact in employeeCollection.Entities)
                {
                    int optionsetvalue = (int)contact.GetAttributeValue<OptionSetValue>("cp_priority").Value;

                    if (minPriority > optionsetvalue)
                    {
                        minPriority = optionsetvalue;
                    }
                }
            }
            return minPriority;
        }

        public static void UpdateAcc(Guid accountGuid, int? minPriority, IOrganizationService service)
        {
            Entity accountEntity = new Entity("cp_course_account", accountGuid);
            accountEntity["cp_priority"] = minPriority.HasValue ? new OptionSetValue(minPriority.Value) : null;

            service.Update(accountEntity);
        }
    }
}