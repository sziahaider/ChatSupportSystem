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
    public class ChatAssignmentWorker : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly RedisQueueService _queue;

        public ChatAssignmentWorker(IServiceProvider provider, RedisQueueService queue)
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
                var assignment = scope.ServiceProvider.GetRequiredService<AgentAssignmentService>();

                var sessionId = await _queue.Dequeue();

                if (sessionId == null)
                {
                    await Task.Delay(500);
                    continue;
                }

                var agents = db.Agents.ToList();
                var agent = assignment.GetNextAgent(agents);

                if (agent == null)
                    continue;

                var session = db.ChatSessions.First(x => x.Id == sessionId);

                session.AgentId = agent.Id;
                agent.CurrentChats++;

                await db.SaveChangesAsync();
            }
        }
    }
}
