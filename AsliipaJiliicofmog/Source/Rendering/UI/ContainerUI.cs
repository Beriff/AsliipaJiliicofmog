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

		public ContainerUI(DimUI dims) : base(dims)
		{
			ContainerRT = new RenderTarget2D(GraphicsDevice, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);
		}
		public override void Update()
		{
			if (ContainerRT.Bounds.Size != AbsoluteSize.ToPoint())
			{
				ContainerRT.Dispose();
				ContainerRT = new RenderTarget2D(GraphicsDevice, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);
			}

			foreach (var child in Children)
				child.Update();
		}

		protected void BeforeRender(SpriteBatch sb)
		{
			PreviousTarget = CurrentRenderTarget;
			CurrentRenderTarget = ContainerRT;
			sb.GraphicsDevice.SetRenderTarget(CurrentRenderTarget);
		}

		protected void AfterRender(SpriteBatch sb)
		{
			CurrentRenderTarget = PreviousTarget;
			sb.GraphicsDevice.SetRenderTarget(CurrentRenderTarget);
			sb.Draw(ContainerRT, AbsolutePosition, Color.White);
		}

		public override void Render(SpriteBatch sb)
		{
			BeforeRender(sb);
			foreach (var child in Children)
				child.Render(sb);
			AfterRender(sb);
		}

	}
}
