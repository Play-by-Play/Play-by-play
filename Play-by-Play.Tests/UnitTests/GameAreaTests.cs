using Play_by_Play.Hubs;
using Play_by_Play.Hubs.Models;
using Should;
using Xunit;

namespace Play_by_Play.Tests.UnitTests {
	public class GameAreaTests {
		[Fact]
		public void GetAreaName_Returns_The_Correct_Name() {
			var coords = new[] {1, 1};
			var name = GameArea.GetAreaName(coords[0], coords[1]);
			name.ShouldEqual("gameBoardRCW");
		}

		[Fact]
		public void GetCoords_Returns_The_Correct_Coordinates() {
			var name = "gameBoardLCD";
			var coords = GameArea.GetCoords(name);
			coords.Length.ShouldEqual(2);
			coords[0].ShouldEqual(0);
			coords[1].ShouldEqual(2);
		}
	}
}