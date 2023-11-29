using AsliipaJiliicofmog.Rendering.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsliipaJiliicofmog.Tests
{
	[TestClass]
	public class UITests
	{
		[TestMethod]
		public void UI_RelativeDimCalculation()
		{
			ElementUI.Viewport = new Viewport(0, 0, 500, 500);
			var element = new ElementUI(
				new DimUI(new(.5f, .5f), new(0), new(0), new(.5f, .5f))
				);

			Assert.AreEqual(new Vector2(250, 250), element.AbsoluteSize);
			Assert.AreEqual(new Vector2(250, 250), element.AbsolutePosition);
		}

		[TestMethod]
		public void UI_AbsoluteDimCalculation() 
		{
			ElementUI.Viewport = new Viewport(0, 0, 500, 500);
			var element = new ElementUI(
				new DimUI(new(.5f, .5f), new(50), new(50), new(.5f, .5f))
				);

			Assert.AreEqual(new Vector2(300, 300), element.AbsoluteSize);
			Assert.AreEqual(new Vector2(300, 300), element.AbsolutePosition);
		}

		[TestMethod]
		public void UI_ParentDimCalculation()
		{
			ElementUI.Viewport = new Viewport(0, 0, 500, 500);
			var parent = new ElementUI(
				new DimUI(new(.5f, .5f), new(0), new(0), new(.5f, .5f))
				);
			var element = new ElementUI(
				new DimUI(new(.5f, .5f), new(0), new(0), new(.5f, .5f))
				).WithParent(parent);

			Assert.AreEqual(new Vector2(125, 125), element.AbsoluteSize);
			Assert.AreEqual(new Vector2(125, 125), element.AbsolutePosition);
		}

		[TestMethod]
		public void UI_PivotCalculation()
		{
			ElementUI.Viewport = new Viewport(0, 0, 500, 500);
			var element = new ElementUI(
				new DimUI(new(.5f, .5f), new(0), new(0), new(.5f, .5f))
				)
			{ Pivot = new(.5f) };

			Assert.AreEqual(new Vector2(250, 250), element.AbsoluteSize);
			Assert.AreEqual(new Vector2(375, 375), element.AbsolutePosition);
		}
	}
}
