using Microsoft.EntityFrameworkCore;
using MassTransit;
using Payments.Consumers;
using Models.Payments;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(cfg =>
{
    // adding consumers
    cfg.AddConsumer<ProcessPaymentEventConsumer>();

    // telling masstransit to use rabbitmq
    cfg.UsingRabbitMq((context, rabbitCfg) =>
    {
        // rabbitmq config
        rabbitCfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        // automatic endpoint configuration (and I think the reason why naming convention is important
        rabbitCfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// bus for publishing a message, to check if everything works
// THIS SHOULD NOT EXIST IN FINAL PROJECT

/*var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("rabbitmq", "/", h =>
    {
        h.Username("guest");
        h.Password("guest");
    });
});
busControl.Start();
await busControl.Publish(new ProcessPaymentEvent(new CardCredentials() { CVV = 244, ExpDate = "04/22", FullName = "Gienek", Number = "7358923759823753928" }, 2516.26));
busControl.Stop();*/

app.Run();