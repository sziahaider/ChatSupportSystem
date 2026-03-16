using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Workers
{
    public class PollMonitorWorker : BackgroundService
    {
        private readonly IServiceProvider _provider;

        public PollMonitorWorker(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = _provider.CreateScope();

                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var sessions = db.ChatSessions.Where(x => x.IsActive).ToList();

                    foreach (var s in sessions)
                    {
                        s.MissedPolls++;

                        if (s.MissedPolls >= 3)
                            s.IsActive = false;
                    }

                    await db.SaveChangesAsync();

                    await Task.Delay(1000);
                }
            }
            catch (Exception ex) { 
            }            
        }
    }
}
