using NServiceBus;
using NServiceBus.Persistence.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class FirstSaga : SqlSaga<FirstSagaData>,
        IAmStartedByMessages<StartFirstSaga>,
        IHandleMessages<SaveLegacyApplicationCompleted>
    {
        public Task Handle(StartFirstSaga message, IMessageHandlerContext context)
        {
            return Task.CompletedTask;
        }

        public Task Handle(SaveLegacyApplicationCompleted message, IMessageHandlerContext context)
        {
            Console.WriteLine("FirstSaga type handled message");

            return Task.CompletedTask;
        }

        protected override string CorrelationPropertyName => nameof(FirstSagaData.LoanId);

        protected override void ConfigureMapping(IMessagePropertyMapper mapper)
        {
            mapper.ConfigureMapping<StartFirstSaga>(_ => _.LoanApplicationId);
            mapper.ConfigureMapping<SaveLegacyApplicationCompleted>(_ => _.LoanApplicationId);
        }
    }

    public class FirstSagaData : ContainSagaData
    {
        public Guid LoanId { get; set; }
    }
}