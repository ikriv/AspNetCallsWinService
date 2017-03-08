using System;
using IKriv.Sample.CallWinService.Communication;
using IKriv.Sample.CallWinService.Interfaces;

namespace IKriv.Sample.CallWinService.ConsoleClient
{
    public class Program
    {
        private readonly string[] _args;
        private IRequestHandler _handler;

        static void Main(string[] args)
        {
            try
            {
                new Program(args).Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        public Program(string[] args)
        {
            _args = args;
        }

        public void Run()
        {
            if (_args.Length == 0)
            {
                Usage();
            }
            else
            {
                ProcessCommand(_args[0]);
            }
        }

        private void ProcessCommand(string command)
        {
            _handler = new WcfWrapper().CreateClient<IRequestHandler>(ServiceConstants.Url);

            switch (command)
            {
                case "add": AddRequest(); break;
                case "list": ListRequests(); break;
                default: throw new InvalidOperationException("Unknown command: " + command);
            }
        }

        private void AddRequest()
        {
            if (_args.Length != 3) throw new InvalidOperationException("add command requires 2 arguments");
            int timeMs = int.Parse(_args[1]);
            var result = _handler.CreateRequest(new Request {Input = _args[2], RequestedProcessingTimeMs = timeMs});
            PrintRequest(result);
        }

        private void ListRequests()
        {
            if (_args.Length == 1)
            {
                ListAllRequests();
            }
            else if (_args.Length == 2)
            {
                ListRequest(int.Parse(_args[1]));
            }
            else
            {
                throw new InvalidOperationException("list command requires zero or one argument");
            }
        }

        private void ListAllRequests()
        {
            var result = _handler.GetAllRequests(0);
            foreach (var request in result.Requests)
            {
                PrintRequest(request);
                Console.WriteLine();
            }
        }

        private void ListRequest(int id)
        {
            var result = _handler.GetRequest(id);
            PrintRequest(result);
        }

        private void PrintRequest(AcceptedRequest request)
        {
            string completed = request.IsCompleted ? "COMPLETE" : "PENDING ";

            Console.WriteLine($"Request #{request.Id} is {completed}");
            Console.WriteLine($"Input: {request.Input}");
            Console.WriteLine($"Created {request.CreatedOn:G}, delay {request.RequestedProcessingTimeMs}ms, scheduled {request.ScheduledCompletionTime:G}");
            if (request.IsCompleted)
            {
                Console.WriteLine($"Completed {request.ActualCompletionTime:G}, actual processing time {request.ActualProcessingTimeMs}ms");
                Console.WriteLine($"Output: {request.Output}");
            }
        }

        private static void Usage()
        {
            Console.WriteLine(@"Usage: MyConsoleClient command [args]

The following commands are defined:

add {time_ms} {text}        add request with given processing time and text
list {request_id}           show request status
list                        show all requests");
        }
    }
}
