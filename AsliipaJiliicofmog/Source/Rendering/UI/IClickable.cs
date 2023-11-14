using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public interface IClickable
	{
		public Action OnClick { get; set; }
	}
}
