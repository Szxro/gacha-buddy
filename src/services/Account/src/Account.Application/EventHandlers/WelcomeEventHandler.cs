using Account.Application.Contracts;
using Account.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Account.Application.EventHandlers;

public class WelcomeEventHandler : IEventHandler<WelcomeEvent>
{
    private readonly ILogger<WelcomeEventHandler> _logger;

    public WelcomeEventHandler(ILogger<WelcomeEventHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(WelcomeEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Welcome to GachaBuddy, username: {username}", @event.Username);
        
        return Task.CompletedTask;
    }
}