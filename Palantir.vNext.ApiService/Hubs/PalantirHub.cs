using Microsoft.AspNetCore.SignalR;
using Palantir.vNext.Core.Model;
using Palantir.vNext.Core.Services;

namespace Palantir.vNext.ApiService.Hubs;

public class PalantirHub : Hub
{
    private readonly ImesService _imes;

    public PalantirHub(ImesService imes)
    {
        _imes = imes;
    }

    public async Task ProcessConversation(Conversation payload)
    {
        Console.WriteLine($"Received conversation with {payload.Turns.Count()} turns");

        var response = await _imes.ProcessConversation(payload);

        await PropagateModelResponse(response);
    }

    public async Task PropagateModelResponse(Hypothesis payload)
    {
        await Clients.All.SendAsync("ReceiveModelResponse", payload);
    }
}
