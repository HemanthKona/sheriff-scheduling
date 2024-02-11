using System;
using Microsoft.Extensions.Configuration;
using Quartz;

namespace SS.Api.cronjobs
{
    public static class QuartzConfigService
    {
        public static void AddJobAndTrigger<T>(
            this IServiceCollectionQuartzConfigurator quartz,
            IConfiguration config)
            where T : IJob
        {
            
            string jobName = typeof(T).Name;            
            var configKey = $"{jobName}";
            var cronSchedule = config[configKey];
            
            if (string.IsNullOrEmpty(cronSchedule))
            {
                throw new Exception($"No Quartz.NET Cron schedule found for job in configuration at {configKey}");
            }

            var jobKey = new JobKey(jobName);
            quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));
            
            quartz.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity(jobName + "-trigger")
                .WithCronSchedule(cronSchedule));
                
        }
    }
}