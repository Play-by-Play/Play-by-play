using System.Collections.Generic;
using Play_by_Play.Hubs.Models;
using Should;
using Xunit;

namespace Play_by_Play.Tests.UnitTests {
	public class BattleResultTests {
		[Fact]
		public void OffensiveEqualsAllOffensivePlayersCombined() {
			var homePlayers = GetPlayers(1);
			var awayPlayers = GetPlayers(1);
			
			var result = new BattleResult(homePlayers, awayPlayers, BattleType.Scramble, true);

			result.HomeTotal.ShouldEqual(3);
		}

		private static List<Player> GetPlayers(int nr) {
			return new List<Player> {
				new Player {
					Defense = 2,
					Offense = 3,
					Position = Position.C
				}
			};
		}
	}
}