using Infrastructure.Data;
using Infrastructure.Queue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services;

namespace Application.Workers
{
    public class ChatDispatcherWorker : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly InMemoryQueue _queue;

        public ChatDispatcherWorker(IServiceProvider provider, InMemoryQueue queue)
        {
            _provider = provider;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _provider.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var agentService = scope.ServiceProvider.GetRequiredService<AgentService>();

                var sessionId = await _queue.Dequeue();

                if (sessionId == null)
                {
                    await Task.Delay(500);
                    continue;
                }

                var agent = await agentService.NextAgent();

                if (agent == null) continue;

                var session = db.ChatSessions.First(x => x.Id == sessionId);


                session.AgentId = agent.Id;
                agent.CurrentChats++;

                await db.SaveChangesAsync();
            }
        }
    }
}
