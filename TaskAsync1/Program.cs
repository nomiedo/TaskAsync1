using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TaskAsync1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Calculator> listOfOperations = new List<Calculator>();
            while (true)
            {
                Console.WriteLine("Enter N:");
                var n = Console.ReadLine();

                // cancel previous calculations
                var listForCancellation = listOfOperations.Where(o => o.Canceled == false);
                foreach (var o in listForCancellation) { o.Cancel(); }

                //initial new calculation
                listOfOperations.Add(new Calculator(Convert.ToInt64(n)));
            }
        }
    }
    class Calculator
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        public long Answer = 0;
        public bool Canceled = false;

        public Calculator(long n)
        {
           Run(Convert.ToInt64(n));
        }

        public async Task Run(long n)
        {
            try
            {
                await Sum(cts.Token, n);          
                Console.WriteLine($"Answer for {n} : {Answer}");
            }
            catch (AggregateException x)
            {
                x.Handle(e => e is OperationCanceledException);
                Console.WriteLine($"Calculation for {n} is canseled");
            }
        }

        public void Cancel()
        {
            if (cts != null)
            {
                cts.Cancel();
                Canceled = true;
            }
        }

        async Task Sum(CancellationToken ct, long n)
        {
            await Task.Delay(5000);
            Parallel.For(0, n + 1, i => DoWork(i, ct));
        }

        private void DoWork(long i, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            Answer += i;
        }
    }
}
