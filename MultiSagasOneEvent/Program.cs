using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Persistence.Sql;

namespace MultiSagasOneEvent
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Run().GetAwaiter().GetResult();
        }

        public async Task Run()
        {
            var endpointConfiguration = new EndpointConfiguration("Repro");
            endpointConfiguration.SendFailedMessagesTo("error");

            var transport = endpointConfiguration.UseTransport<MsmqTransport>();
            transport.Transactions(TransportTransactionMode.TransactionScope);

            var routing = transport.Routing();
            routing.RegisterPublisher(typeof(BaseCheckResult<>).Assembly, "Repro");

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));
            var connection = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=SagaTest;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            persistence.ConnectionBuilder(
                connectionBuilder: () =>
                {
                    return new SqlConnection(connection);
                });

            endpointConfiguration.PurgeOnStartup(true);
            endpointConfiguration.EnableInstallers();

            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Enter");
            Console.ReadLine();

            var loanAppId = Guid.NewGuid();

            // Start 2 instances of sagas with the same Id
            await endpointInstance.SendLocal(new StartFirstSaga { LoanApplicationId = loanAppId });
            await endpointInstance.SendLocal(new StartSecondSaga { LoanApplicationId = loanAppId });

            Console.WriteLine("Enter");
            Console.ReadLine();

            // Publish one event that should go to both sagas
            await endpointInstance.Publish(new SaveLegacyApplicationCompleted { LoanApplicationId = loanAppId });

            Console.ReadLine();
        }
    }
}