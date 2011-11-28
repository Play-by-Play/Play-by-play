using NUnit.Framework;
using Play_by_Play.Models;
using Should;
using Xunit;

namespace Play_by_Play.Tests.UnitTests {
	[TestFixture]
	public class NodeTests {
		[Test]
		public void ToStringWorks() {
			var node = new Node {
				X = 1,
				Y = 2
			};

			var result = node.ToString();

			ObjectAssertExtensions.ShouldEqual(result, "{X: 1, Y: 2}");
		} 
	}
}