using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public class ContainerUI : ElementUI
	{
		protected static RenderTarget2D? CurrentRenderTarget;

		public List<ElementUI> Children = new();
		public RenderTarget2D ContainerRT;

		protected RenderTarget2D? PreviousTarget;

		public ContainerUI(DimUI dims, string name = "ui-container") : base(dims, name)
		{
			ContainerRT = new(
							GraphicsDevice,
							(int)AbsoluteSize.X,
							(int)AbsoluteSize.Y,
							false,
							SurfaceFormat.Color,
							DepthFormat.None,
							0, RenderTargetUsage.PreserveContents);
        }

		public ElementUI GetChild(string name) 
		{
			return Children.Find(x => x.Name == name) ??
				throw new KeyNotFoundException("Child ui element not found");
		}

		public virtual void Add(ElementUI element)
		{
			Children.Add(element);
			element.Parent = this;
		}

		public virtual void Remove(ElementUI element)
		{
			element.Parent = null;
			Children.Remove(element);
		}

		public override void Update()
		{
			if (ContainerRT.Bounds.Size != AbsoluteSize.ToPoint())
			{
				ContainerRT.Dispose();
				ContainerRT = new(
					GraphicsDevice,
					(int)AbsoluteSize.X,
					(int)AbsoluteSize.Y,
					false,
					SurfaceFormat.Color,
					DepthFormat.None,
					0, RenderTargetUsage.PreserveContents);
			}

			foreach (var child in Children)
			{
				if (!child.Active) continue;
                child.Update();
			}

			base.Update();
		}

		protected void SetContainerRenderer(SpriteBatch sb)
		{
			PreviousTarget = CurrentRenderTarget;
			CurrentRenderTarget = ContainerRT;
			sb.End();
			sb.GraphicsDevice.SetRenderTarget(CurrentRenderTarget);
			sb.GraphicsDevice.Clear(Color.Black);
			sb.Begin(samplerState: SamplerState.PointWrap);
		}

		protected void RetractContainerRenderer(SpriteBatch sb)
		{
			CurrentRenderTarget = PreviousTarget;
			sb.End();
			sb.GraphicsDevice.SetRenderTarget(CurrentRenderTarget);
			sb.Begin(samplerState: SamplerState.PointWrap);
			sb.Draw(ContainerRT, AbsolutePosition, Color.White);
		}

		protected virtual void RenderChildren(SpriteBatch sb, GroupUI group)
		{
			foreach (var child in Children)
			{
				if (!child.Visible) continue;
				child.Dimensions.Offset -= AbsolutePosition;
				child.Render(sb, group);
				child.Dimensions.Offset += AbsolutePosition;
			}
				
		}


		public void Dispose()
		{
            ContainerRT.Dispose();
            foreach (var child in Children)
			{
                
                if (child.GetType().IsSubclassOf(typeof(ContainerUI)))
				{
					(child as ContainerUI).Dispose();
				}
			}
		}

		~ContainerUI()
		{
			Dispose();
        }
	}
}
