using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SmartSolar.Device.Core.Services;

namespace SmartSolar.Device.Test
{
	[TestClass]
	public class ElementStrategyTests
	{
		[TestMethod]
		public void TestMethod1()
		{
			// Arrange
			var objectUnderTest = new ElementStrategy();

			// Act
			var result = objectUnderTest.ShouldElementBeOn();

			// Assert


		}
	}
}
