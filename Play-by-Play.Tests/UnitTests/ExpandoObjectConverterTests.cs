using System.Dynamic;
using System.Web.Script.Serialization;
using Play_by_Play.Models;
using Xunit;

namespace Play_by_Play.Tests.UnitTests {
	public class ExpandoObjectConverterTests {
		[Fact]
		public void It_Generates_An_Empty_Dictionary_If_Object_Is_Empty() {
			dynamic data = new ExpandoObject();
			var serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new JavaScriptConverter[] { new ExpandoObjectConverter() });
			
			var result = serializer.Serialize(data);

			result.ToString();
		} 
	}
}