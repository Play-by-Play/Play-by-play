using Play_by_Play.Models;
using Should;
using Xunit;

namespace Play_by_Play.Tests.UnitTests {
	public class NodeTests {
		[Fact]
		public void ToStringWorks() {
			var node = new Node {
				X = 1,
				Y = 2
			};

			var result = node.ToString();

			result.ShouldEqual("{X: 1, Y: 2}");
		} 
	}
}