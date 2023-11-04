using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Source.Rendering.UI
{
	internal interface IClickable
	{
		public Action OnClick { get; set; }
	}
}
