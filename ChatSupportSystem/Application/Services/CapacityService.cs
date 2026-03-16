using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Calculates the combined chat handling capacity of a set of agents.
    /// </summary>
    /// <remarks>
    /// Each agent contributes a base capacity (10) multiplied by a seniority-based factor.
    /// The final result is floored to an integer. This is used for rough capacity planning and routing decisions.
    /// </remarks>
    public class CapacityService
    {
        /// <summary>
        /// Computes total capacity available from the provided agents.
        /// </summary>
        /// <param name="agents">List of agents to include in the calculation.</param>
        /// <returns>
        /// The total capacity as an integer. The decimal result is floored to avoid overcommitting capacity.
        /// </returns>
        public int Calculate(List<Agent> agents)
        {
            double total = 0;

            // Sum each agent's capacity: base capacity (10) multiplied by their seniority multiplier.
            foreach (var a in agents)
                total += 10 * Mult(a.Seniority);

            // Floor the total to return a conservative whole-number capacity.
            return (int)Math.Floor(total);
        }

        // Multiplier by seniority level:
        // - Junior  : 0.4
        // - Mid     : 0.6
        // - Senior  : 0.8
        // - TeamLead: 0.5
        // Default fallback is 0.4 for unknown values.
        double Mult(SeniorityLevel s) => s switch
        {
            SeniorityLevel.Junior => 0.4,
            SeniorityLevel.Mid => 0.6,
            SeniorityLevel.Senior => 0.8,
            SeniorityLevel.TeamLead => 0.5,
            _ => 0.4
        };
    }    
}
