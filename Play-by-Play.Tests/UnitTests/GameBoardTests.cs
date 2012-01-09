using System.Linq;
using Play_by_Play.Hubs.Models;
using Play_by_Play.Tests.Helpers;
using Should;
using Xunit;

namespace Play_by_Play.Tests.UnitTests {
	public class GameBoardTests {
		[Fact]
		public void Execute_MovingPlayerWhoWinsBattle_Move() {
			GameBoard gameboard = new GameBoard();
			TacticCard tactic = Factory.GetMovementCard();
			GameArea area = gameboard.Areas.First(a => a.X == tactic.StartNode.X && a.Y == tactic.StartNode.Y);
			area.AddHomePlayer(Factory.GetPlayer());
			gameboard.AwayGoalie = Factory.GetPlayer("G");

			gameboard.ExecuteTactic(tactic, true);

			area.HomePlayers.ShouldBeEmpty();
		}

		[Fact]
		public void Execute_MovingPlayerWhoLoosesBattle_DoesNotMove() {
			GameBoard gameboard = new GameBoard();
			TacticCard tactic = Factory.GetMovementCard();
			GameArea area = gameboard.Areas.First(a => a.X == tactic.StartNode.X && a.Y == tactic.StartNode.Y);
			area.AddHomePlayer(Factory.GetPlayer());
			area.AddAwayPlayer(Factory.GetPlayer());
			area.AddAwayPlayer(Factory.GetPlayer());
			area.AddAwayPlayer(Factory.GetPlayer());
			area.AddAwayPlayer(Factory.GetPlayer());
			gameboard.AwayGoalie = Factory.GetPlayer("G");

			gameboard.ExecuteTactic(tactic, true);

			area.HomePlayers.ShouldNotBeEmpty();
		}
	}
}