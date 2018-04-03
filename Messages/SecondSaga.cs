using NServiceBus;
using NServiceBus.Persistence.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class SecondSaga : SqlSaga<SecondSagaData>,
        IAmStartedByMessages<StartSecondSaga>,
        IHandleMessages<SaveLegacyApplicationCompleted>
    {
        public Task Handle(StartSecondSaga message, IMessageHandlerContext context)
        {
            return Task.CompletedTask;
        }

        public Task Handle(SaveLegacyApplicationCompleted message, IMessageHandlerContext context)
        {
            Console.WriteLine("SecondSaga type handled message");

            return Task.CompletedTask;
        }

        protected override string CorrelationPropertyName => nameof(SecondSagaData.LoanId);

        protected override void ConfigureMapping(IMessagePropertyMapper mapper)
        {
            mapper.ConfigureMapping<StartSecondSaga>(_ => _.LoanApplicationId);
            mapper.ConfigureMapping<SaveLegacyApplicationCompleted>(_ => _.LoanApplicationId);
        }
    }

    public class SecondSagaData : ContainSagaData
    {
        public Guid LoanId { get; set; }
    }
}