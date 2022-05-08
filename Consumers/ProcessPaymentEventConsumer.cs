using MassTransit;
using Models.Payments;

namespace Payments.Consumers
{
    public class ProcessPaymentEventConsumer : IConsumer<ProcessPaymentEvent>
    {
        private Random random = new();
        public async Task Consume(ConsumeContext<ProcessPaymentEvent> context)
        {
            var @event = context.Message;
            var res = await ProcessPayment(@event.Card);
            if (res)
                // payment succeeded
                await context.Publish(new ProcessPaymentReplyEvent(ProcessPaymentReplyEvent.State.ACCEPTED, @event.CorrelationId));
            else
            {
                // payment failed
                if(random.Next(100) < 20 || @event.Card.CVV % 100 == 44)
                    await context.Publish(new ProcessPaymentReplyEvent(ProcessPaymentReplyEvent.State.INVALID_CARD_CREDENTIALS, @event.CorrelationId));
                else
                    await context.Publish(new ProcessPaymentReplyEvent(ProcessPaymentReplyEvent.State.LIMITS_TOO_LOW, @event.CorrelationId));
            }
        }

        public async Task<bool> ProcessPayment(CardCredentials credentials)
        {
            await Task.Delay(4000);
            if (credentials.CVV % 10 == 4)
                return false;
            if (random.Next(100) > 10)
                return true;
            await Task.Delay(4000);
            if (random.Next(100) > 10)
                return true;
            await Task.Delay(4000);
            if (random.Next(100) > 10)
                return true;
            return false;               
        }
    }
}
