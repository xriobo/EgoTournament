using EgoTournament.Models;
using EgoTournament.Services;
using EgoTournament.Tests.Infrastructue;
using FluentAssertions;

namespace EgoTournament.Tests;

[Collection(TestCollections.Integration)]
public class FirebaseServiceTests : IClassFixture<MauiProgramFixture>
{
    private IFirebaseService _firebaseService;

    public FirebaseServiceTests(MauiProgramFixture fixture)
    {
        _firebaseService = fixture.ServiceProvider.GetService<IFirebaseService>();
    }

    [Fact]
	public async Task UpsertTournamentsHistoryAsync_Ok()
	{
        var tournamentResult = new TournamentResultDto()
        {
            FinishDate = DateTime.Now,
            HadReward = true,
            Name = "Test",
            OwnerId = Guid.NewGuid().ToString(),
            ParticipantsResults = new List<Models.ParticipantResultDto>()
            {
                new ParticipantResultDto()
                {
                    LeaguePoints = 70,
                    Losses = 10,
                    Rank = "III" ,
                    SummonerName = "test#test",
                    TierType = Models.Enums.TierEnum.GOLD,
                    Winrate = "50%",
                    Wins = 10
                }
            }
        };

        var historyResult = await _firebaseService.UpsertTournamentsHistoryAsync(tournamentResult);

        historyResult.Should().NotBeNull();
        historyResult.Should().BeOfType<TournamentResultDto>();
    }

    [Fact]
    public async Task DeleteTournamentsHistoryAsync_Ok()
    {
        var tournamentResult = new TournamentResultDto()
        {
            FinishDate = DateTime.Now,
            HadReward = true,
            Name = "Test",
            OwnerId = Guid.NewGuid().ToString(),
            ParticipantsResults = new List<Models.ParticipantResultDto>()
            {
                new ParticipantResultDto()
                {
                    LeaguePoints = 70,
                    Losses = 10,
                    Rank = "III" ,
                    SummonerName = "test#test",
                    TierType = Models.Enums.TierEnum.GOLD,
                    Winrate = "50%",
                    Wins = 10
                }
            }
        };

        await _firebaseService.UpsertTournamentsHistoryAsync(tournamentResult);

        var deletedResult = await _firebaseService.DeleteTournamentsHistoryAsync(tournamentResult.Uid);

        deletedResult.Should().Be(true);
    }

    [Fact]
    public async Task GetTournamentsHistoryAsync_Ok()
    {
        var tournamentResult = new TournamentResultDto()
        {
            FinishDate = DateTime.Now,
            HadReward = true,
            Name = "Test",
            OwnerId = Guid.NewGuid().ToString(),
            ParticipantsResults = new List<Models.ParticipantResultDto>()
            {
                new ParticipantResultDto()
                {
                    LeaguePoints = 70,
                    Losses = 10,
                    Rank = "III" ,
                    SummonerName = "test#test",
                    TierType = Models.Enums.TierEnum.GOLD,
                    Winrate = "50%",
                    Wins = 10
                }
            },
            Rules = new List<string> { "1. Regla.", "2. Regla" },
            Uid = Guid.NewGuid()
        };

        await _firebaseService.UpsertTournamentsHistoryAsync(tournamentResult);

        var tournamentResultFrombbdd =  await _firebaseService.GetTournamentsHistoryAsync(tournamentResult.Uid);

        tournamentResultFrombbdd.Should().NotBeNull();
        tournamentResultFrombbdd.FinishDate.Should().Be(tournamentResult.FinishDate);
        tournamentResultFrombbdd.Name.Should().Be(tournamentResult.Name);
        tournamentResultFrombbdd.OwnerId.Should().Be(tournamentResult.OwnerId);
        tournamentResultFrombbdd.HadReward.Should().Be(tournamentResult.HadReward);
        tournamentResultFrombbdd.Uid.Should().Be(tournamentResult.Uid);
        tournamentResultFrombbdd.Rules.Count().Should().Be(tournamentResult.Rules.Count());
        tournamentResultFrombbdd.Rules.ToList()[0].Should().Be(tournamentResult.Rules.ToList()[0]);
        tournamentResultFrombbdd.ParticipantsResults.Count().Should().Be(tournamentResult.ParticipantsResults.Count());
        tournamentResultFrombbdd.ParticipantsResults.ToList()[0].LeaguePoints.Should().Be(tournamentResult.ParticipantsResults.ToList()[0].LeaguePoints);
        tournamentResultFrombbdd.ParticipantsResults.ToList()[0].Losses.Should().Be(tournamentResult.ParticipantsResults.ToList()[0].Losses);
        tournamentResultFrombbdd.ParticipantsResults.ToList()[0].Rank.Should().Be(tournamentResult.ParticipantsResults.ToList()[0].Rank);
        tournamentResultFrombbdd.ParticipantsResults.ToList()[0].SummonerName.Should().Be(tournamentResult.ParticipantsResults.ToList()[0].SummonerName);
        tournamentResultFrombbdd.ParticipantsResults.ToList()[0].TierType.Should().Be(tournamentResult.ParticipantsResults.ToList()[0].TierType);
        tournamentResultFrombbdd.ParticipantsResults.ToList()[0].Wins.Should().Be(tournamentResult.ParticipantsResults.ToList()[0].Wins);
        tournamentResultFrombbdd.ParticipantsResults.ToList()[0].Winrate.Should().Be(tournamentResult.ParticipantsResults.ToList()[0].Winrate);

        await _firebaseService.DeleteTournamentsHistoryAsync(tournamentResult.Uid);
    }
}
