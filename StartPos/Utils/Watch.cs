using System;
using System.Threading.Tasks;

namespace StartPos.Utils
{
    public static class Watch
    {
        public static async Task<int> Wait(Action operation, Func<bool> terminationCondition, int retries)
        {
            var attempt = 1;
            while (attempt <= retries && !terminationCondition())
            {
                await Task.Delay(2000);
                operation();
                attempt++;
            }

            return attempt;
        }
    }
}