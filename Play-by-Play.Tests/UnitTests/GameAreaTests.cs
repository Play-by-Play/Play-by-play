using System.Collections.Generic;
using System.Linq;
using Play_by_Play.Hubs;
using Play_by_Play.Hubs.Models;
using Play_by_Play.Tests.Helpers;
using Should;
using Xunit;

namespace Play_by_Play.Tests.UnitTests.GameAreaTests {
	public class GetAreaNameTests {
		[Fact]
		public void GetAreaNameReturnsTheCorrectName() {
			var coords = new[] { 1, 1 };
			var name = GameArea.GetAreaName(coords[0], coords[1]);
			name.ShouldEqual("gameBoardRCW");
		}
	}

	public class GetCoordsTests {
		[Fact]
		public void GetCoordsReturnsTheCorrectCoordinates() {
			var name = "gameBoardLCD";
			var coords = GameArea.GetCoords(name);
			coords.Length.ShouldEqual(2);
			coords[0].ShouldEqual(0);
			coords[1].ShouldEqual(2);
		}
	}

	public class AddHomePlayerTests {
		[Fact]
		public void ItDoesNotAddBonusForCenter() {
			var player = Factory.GetPlayers(1).First();
			var area = Factory.GetArea(0, 0);

			area.AddHomePlayer(player);

			area.HomePlayers.ShouldNotBeEmpty();
			area.HomePlayers.First().Bonus.ShouldEqual(Bonus.None);
		}

		[Fact]
		public void ItAddsBonusForLeftWingOnUpperLeftArea() {
			var player = Factory.GetPlayers(1, "LW").First();
			var area = Factory.GetArea(0, 0);

			area.AddHomePlayer(player);

			area.HomePlayers.ShouldNotBeEmpty();
			area.HomePlayers.First().Bonus.ShouldEqual(Bonus.Offense);
		}

		[Fact]
		public void ItAddsBonusForRightWingOnUpperRightArea() {
			var player = Factory.GetPlayers(1, "RW").First();
			var area = Factory.GetArea(1, 0);

			area.AddHomePlayer(player);

			area.HomePlayers.ShouldNotBeEmpty();
			area.HomePlayers.First().Bonus.ShouldEqual(Bonus.Offense);
		}

		[Fact]
		public void ItAddsBonusForLeftDefenderOnLowerLeftArea() {
			var player = Factory.GetPlayers(1, "LD").First();
			var area = Factory.GetArea(0, 2);

			area.AddHomePlayer(player);

			area.HomePlayers.ShouldNotBeEmpty();
			area.HomePlayers.First().Bonus.ShouldEqual(Bonus.Defense);
		}

		[Fact]
		public void ItAddsBonusForRightDefenderOnLowerRightArea() {
			var player = Factory.GetPlayers(1, "RD").First();
			var area = Factory.GetArea(1, 2);

			area.AddHomePlayer(player);

			area.HomePlayers.ShouldNotBeEmpty();
			area.HomePlayers.First().Bonus.ShouldEqual(Bonus.Defense);
		}
	}

	public class AddAwayPlayerTests {
		[Fact]
		public void ItDoesNotAddBonusForCenter() {
			var player = Factory.GetPlayers(1).First();
			var area = Factory.GetArea(0, 0);

			area.AddAwayPlayer(player);

			area.AwayPlayers.ShouldNotBeEmpty();
			area.AwayPlayers.First().Bonus.ShouldEqual(Bonus.None);
		}

		[Fact]
		public void ItAddsBonusForLeftWingOnUpperLeftOppositeArea() {
			var player = Factory.GetPlayers(1, "LW").First();
			var area = Factory.GetArea(1, 3);

			area.AddAwayPlayer(player);

			area.AwayPlayers.ShouldNotBeEmpty();
			area.AwayPlayers.First().Bonus.ShouldEqual(Bonus.Offense);
		}

		[Fact]
		public void ItAddsBonusForRightWingOnUpperRightOppositeArea() {
			var player = Factory.GetPlayers(1, "RW").First();
			var area = Factory.GetArea(0, 3);

			area.AddAwayPlayer(player);

			area.AwayPlayers.ShouldNotBeEmpty();
			area.AwayPlayers.First().Bonus.ShouldEqual(Bonus.Offense);
		}

		[Fact]
		public void ItAddsBonusForLeftDefenderOnLowerLeftOppositeArea() {
			var player = Factory.GetPlayers(1, "LD").First();
			var area = Factory.GetArea(1, 1);

			area.AddAwayPlayer(player);

			area.AwayPlayers.ShouldNotBeEmpty();
			area.AwayPlayers.First().Bonus.ShouldEqual(Bonus.Defense);
		}

		[Fact]
		public void ItAddsBonusForRightDefenderOnLowerRightOppositeArea() {
			var player = Factory.GetPlayers(1, "RD").First();
			var area = Factory.GetArea(0, 1);

			area.AddAwayPlayer(player);

			area.AwayPlayers.ShouldNotBeEmpty();
			area.AwayPlayers.First().Bonus.ShouldEqual(Bonus.Defense);
		}
	}

	public class GetHomeTotalTests {
		[Fact]
		public void BonusIsAddedToPoorLeftWingerAtGameArea00OnAttack() {
			var home = Factory.GetPlayers(1, "LW").First();
			var area = Factory.GetArea(0, 0);
			area.AddHomePlayer(home);

			var result = area.GetHomeTotal(true);

			result.ShouldEqual(4);
		}

		[Fact]
		public void BonusIsAddedToGoodLeftWingerAtGameArea00OnAttack() {
			var home = Factory.GetPlayers(1, "LW").First();
			home.Offense = 6;
			var area = Factory.GetArea(0, 0);
			area.AddHomePlayer(home);

			var result = area.GetHomeTotal(true);

			result.ShouldEqual(7);
		}

		[Fact]
		public void BonusIsNotAddedToCenterAtGameArea00OnAttack() {
			var home = Factory.GetPlayers(1).First();
			var area = Factory.GetArea(0, 0);
			area.AddHomePlayer(home);

			var result = area.GetHomeTotal(true);

			result.ShouldEqual(3);
		}

		[Fact]
		public void BonusIsNotAddedToLeftWingerAtGameArea00OnDefense() {
			var home = Factory.GetPlayers(1, "LW").First();
			var area = Factory.GetArea(0, 0);
			area.AddHomePlayer(home);

			var result = area.GetHomeTotal(false);

			result.ShouldEqual(2);
		}

		[Fact]
		public void BonusIsAddedToPlayersAtGameArea00OnAttack() {
			var area = Factory.GetArea(0, 0);
			area.AddHomePlayer(Factory.GetPlayers(1).First());
			area.AddHomePlayer(Factory.GetPlayers(1, "LW").First());

			var result = area.GetHomeTotal(true);

			result.ShouldEqual(7);
		}

		[Fact]
		public void BonusIsAddedToLeftWingerAtGameArea01OnAttack() {
			var home = Factory.GetPlayers(1, "LW").First();
			var area = Factory.GetArea(0, 1);
			area.AddHomePlayer(home);

			var result = area.GetHomeTotal(true);

			result.ShouldEqual(4);
		}

		[Fact]
		public void BonusIsAddedToRightWingerAtGameArea11OnAttack() {
			var home = Factory.GetPlayers(1, "RW").First();
			var area = Factory.GetArea(1, 1);
			area.AddHomePlayer(home);

			var result = area.GetHomeTotal(true);

			result.ShouldEqual(4);
		}

		[Fact]
		public void BonusIsAddedToRightDefenderAtGameArea12OnAttack() {
			var home = Factory.GetPlayers(1, "RD").First();
			var area = Factory.GetArea(1, 2);
			area.AddHomePlayer(home);

			var result = area.GetHomeTotal(false);

			result.ShouldEqual(3);
		}
	}

	public class GetAwayTotalTests {
		[Fact]
		public void BonusIsAddedToPoorLeftWingerAtGameArea13OnAttack() {
			var home = Factory.GetPlayers(1, "LW").First();
			var area = Factory.GetArea(1, 3);
			area.AddAwayPlayer(home);

			var result = area.GetAwayTotal(true);

			result.ShouldEqual(4);
		}

		[Fact]
		public void BonusIsAddedToGoodLeftWingerAtGameArea13OnAttack() {
			var home = Factory.GetPlayers(1, "LW").First();
			home.Offense = 6;
			var area = Factory.GetArea(1, 3);
			area.AddAwayPlayer(home);

			var result = area.GetAwayTotal(true);

			result.ShouldEqual(7);
		}

		[Fact]
		public void BonusIsNotAddedToCenterAtGameArea13OnAttack() {
			var home = Factory.GetPlayers(1).First();
			var area = Factory.GetArea(1, 3);
			area.AddAwayPlayer(home);

			var result = area.GetAwayTotal(true);

			result.ShouldEqual(3);
		}

		[Fact]
		public void BonusIsNotAddedToLeftWingerAtGameArea13OnDefense() {
			var home = Factory.GetPlayers(1, "LW").First();
			var area = Factory.GetArea(1, 3);
			area.AddAwayPlayer(home);

			var result = area.GetAwayTotal(false);

			result.ShouldEqual(2);
		}

		[Fact]
		public void BonusIsAddedToPlayersAtGameArea13OnAttack() {
			var area = Factory.GetArea(1, 3);
			area.AddAwayPlayer(Factory.GetPlayers(1).First());
			area.AddAwayPlayer(Factory.GetPlayers(1, "LW").First());

			var result = area.GetAwayTotal(true);

			result.ShouldEqual(7);
		}

		[Fact]
		public void BonusIsAddedToLeftWingerAtGameArea12OnAttack() {
			var home = Factory.GetPlayers(1, "LW").First();
			var area = Factory.GetArea(1, 2);
			area.AddAwayPlayer(home);

			var result = area.GetAwayTotal(true);

			result.ShouldEqual(4);
		}

		[Fact]
		public void BonusIsAddedToRightWingerAtGameArea02OnAttack() {
			var home = Factory.GetPlayers(1, "RW").First();
			var area = Factory.GetArea(0, 2);
			area.AddAwayPlayer(home);

			var result = area.GetAwayTotal(true);

			result.ShouldEqual(4);
		}

		[Fact]
		public void BonusIsAddedToRightDefenderAtGameArea01OnAttack() {
			var home = Factory.GetPlayers(1, "RD").First();
			var area = Factory.GetArea(0, 1);
			area.AddAwayPlayer(home);

			var result = area.GetAwayTotal(false);

			result.ShouldEqual(3);
		}
	}

}