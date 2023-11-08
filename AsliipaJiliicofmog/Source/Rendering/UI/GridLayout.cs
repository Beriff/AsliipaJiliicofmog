
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace AsliipaJiliicofmog.Rendering.UI
{
	/// <summary>
	/// Places items accordingly to a specified grid. Unlike ordinary containers,
	/// doesn't change element.Parent value.
	/// </summary>
	internal class GridLayout : UIContainer
	{
		protected Vector2 _CellSize;
		protected Dictionary<Vector2, UIElement> Grid = new();

		public Vector2 CellScale { get; set; }
		public Vector2 AbsoluteCellSize { get => Parent == null? _CellSize : CellScale * Parent.AbsoluteSize; }
		public Vector2 CellSize 
		{ 
			get => _CellSize; 
			set
			{
				if(Parent == null) { _CellSize = value; }
				else
				{
					throw new UIException("Cannot change cell size (parent UIelement present). Did you mean CellScale?");
				}
			} 
		}

		public Vector2 AbsoluteCellPos(Vector2 cellPos) => cellPos * AbsoluteCellSize;

		public GridLayout(UIElement? parent, Vector2 pos, Vector2 scale, Vector2 cellcount)
			: base(parent, pos, scale)
		{
			CellScale = scale / cellcount;
		}
		public GridLayout(Vector2 pos, Vector2 size, Vector2 cellcount)
			: base(pos, size)
		{
			CellSize = size / cellcount;
		}

		/// <summary>
		/// Use the 3-argument UIElement constructor as it allows to set the Scale
		/// property, which is used for rendering inside GridLayout (set parent to null).
		/// </summary>
		/// <exception cref="UIException"></exception>
		public void PlaceElement(Vector2 cellpos, UIElement element)
		{
			if(element.Parent == null)
			{
				Grid[cellpos] = element;
			} else
			{
				throw new UIException("Cannot assign element to a layout that has a parent.");
			}
		}

		public override void AddElement(UIElement element)
		{
			throw new UIException("Cannot directly add new elements to layout. Use PlaceElement() instead");
		}

		public UIElement ElementAt(Vector2 coord) => Grid[coord];

		public override void RenderAt(SpriteBatch sb, UIGroup group, Vector2 p)
		{
			foreach((Vector2 _, UIElement e) in Grid)
			{
				if(e.Visible)
					e.RenderAt(sb, group, e.Position + p - AbsolutePosition);
			}
		}
		public override void UpdateAt(Vector2 pos)
		{
			pos -= AbsolutePosition;
			foreach((Vector2 p, UIElement e) in Grid)
			{
				e.Size = e.Scale * AbsoluteCellSize;
				e.Position = AbsoluteCellPos(p) + pos;
				e.Update();
            }
		}
	}
}
