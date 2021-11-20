using System.Net.Http.Json;

namespace RiotArcaneMissionsBot;

internal static class Program
{
    private static async Task Main()
    {
        var username = Environment.GetEnvironmentVariable("RIOT_USERNAME") ?? throw new ApplicationException("RIOT_USERNAME missing.");
        var password = Environment.GetEnvironmentVariable("RIOT_PASSWORD") ?? throw new ApplicationException("RIOT_PASSWORD missing.");

        var http = new HttpClient();
        await http.GetAsync("https://xsso.riotgames.com/login?uri=https://riotxarcane.riotgames.com&product_id=riotxarcane");

        var response = await http.PutAsJsonAsync("https://auth.riotgames.com/api/v1/authorization", new { type = "auth", username, password });
        var auth = await response.Content.ReadFromJsonAsync<Authorization>();
        if (auth?.Error != null)
        {
            Console.WriteLine($"Login failed: {auth.Error}");
            return;
        }

        await http.GetAsync(auth?.Response?.Parameters.Uri);

        response = await http.GetAsync("https://xp1missions.riotgames.com/xp1missions/v1/missions/en-us");
        var missions = await response.Content.ReadFromJsonAsync<Missions>();
        if (missions?.CompletedMissions != null)
            foreach (var mission in missions.CompletedMissions)
            {
                Console.WriteLine($"Found completed mission: {mission.Title} ({mission.Uid} {mission.MissionType})");
            }

        while (missions?.AvailableMissions != null && missions.AvailableMissions.Any(mission => mission.MissionType is MissionType.SimpleInteractive))
        {
            var mission = missions.AvailableMissions.First(mission => mission.MissionType is MissionType.SimpleInteractive);
            Console.WriteLine($"Found available mission: {mission.Title} ({mission.Uid} {mission.MissionType})");
            Console.WriteLine($"{mission.Uid}: {mission.Title}");
            var genToken = await http.PostAsJsonAsync("https://xp1missions.riotgames.com/xp1missions/v1/generateToken/en-us", new { mission_uid = mission.Uid });

            var tokenResponse = await genToken.Content.ReadFromJsonAsync<GenerateToken>();
            Console.WriteLine($"Token: {tokenResponse?.Token}");

            response = await http.PostAsJsonAsync("https://xp1missions.riotgames.com/xp1missions/v1/completeMission/en-us", new { mission_uid = mission.Uid, token = tokenResponse?.Token });
            Console.WriteLine($"Completed mission: {mission.Title} ({mission.Uid} {mission.MissionType})");
            missions = await response.Content.ReadFromJsonAsync<Missions>();
        }

        if (missions?.AvailableMissions != null)
            foreach (var mission in missions.AvailableMissions.Where(mission => mission.MissionType is MissionType.ComplexParallel or MissionType.ComplexSerial))
            {
                Console.WriteLine($"Found available mission, but cannot complete yet: {mission.Title} ({mission.Uid} {mission.MissionType})");
            }
    }
}