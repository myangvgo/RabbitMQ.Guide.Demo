using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPCClient
{
    public class Rpc
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("RPC Client");
            var n = args.Length > 0 ? args[0] : "30";
            await InvokeAsync(n);

            Console.WriteLine($" Press [Enter] to exit.");
            Console.ReadLine();
        }

        private static async Task InvokeAsync(string n)
        {
            using var rpcClient = new RpcClient(); 
            Console.WriteLine($" [x] Requesting fib({n})");
            var response = await rpcClient.CallAsync(n);
            Console.WriteLine($" [.] Got '{response}'");
        }
    }
}