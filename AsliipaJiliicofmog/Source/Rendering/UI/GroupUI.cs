
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public class PaletteUI
		(Color bg, Color bgd, Color bgi, 
		Color fg, Color fgd, Color fgi, 
		Color txt, Color txtd, Color asp,
		Color aspd, Color cmpl)
	{
		public Color Background = bg;
		public Color BackgroundDark = bgd;
		public Color BackgroundInactive = bgi;

		public Color Foreground = fg;
		public Color ForegroundDark = fgd;
		public Color ForegroundInactive = fgi;

		public Color Text = txt;
		public Color TextDark = txtd;

		public Color Aspect = asp;
		public Color AspectDark = aspd;
		public Color Complementary = cmpl;

		public static PaletteUI Default
		{ get => new(
			new(50,48,47), new(32,32,31), new(21,21,20),
			new(68,63,60), new(55,52,50), new(66,66,66),
			new(235,219,178), new(195,178,139), new(250,189,47),
			new(219,145,47), new(254,124,1)
			); 
		}
	}
	public enum GroupAppendMode
	{
		Discard,
		Replace
	}
	public class GroupUI
	{
		public PaletteUI Palette;
		public string Name;
		public List<ElementUI> Elements;
		public GroupAppendMode AppendMode;

		public GroupUI(string name, GroupAppendMode appendMode = GroupAppendMode.Discard) 
		{
			Name = name;
			Elements = new List<ElementUI>();
			Palette = PaletteUI.Default;
			AppendMode = appendMode;
		}

		public void Add(ElementUI element) => Elements.Add(element);

		public void Render(SpriteBatch sb)
		{
			foreach (var element in Elements)
				element.Render(sb, this);
		}
		public void Update()
		{
			foreach(var element in Elements)
				element.Update();
		}

		public void Disable()
		{
			foreach (var e in Elements) { e.Visible = false; e.Active = false; }
		}
		public void Enable()
		{
			foreach (var e in Elements) { e.Visible = false; e.Active = false; }
		}

		public ElementUI this[string name]
		{
			get => Elements.Find(x => x.Name == name) ?? throw new KeyNotFoundException("invalid element name");
			set => Elements[Elements.FindIndex(x => x.Name == name)] = value;
		}
	}

	public class UI
	{
		public List<GroupUI> Groups;

		public void AddGroup(GroupUI group)
		{
			switch(group.AppendMode)
			{
				case GroupAppendMode.Discard:
					foreach (var g in Groups)
						if (g.Name == group.Name)
							return;
					Groups.Add(group);
					break;
				case GroupAppendMode.Replace:
					int i = Groups.FindIndex(x => x.Name == group.Name);
					if(i != -1)
						Groups[i] = group;
					else
						Groups.Add(group);
					break;
			}
		}

		public GroupUI this[string name]
		{
			get => 
				Groups.Find(x => x.Name == name) ?? throw new KeyNotFoundException("invalid group name");
			set =>
				Groups[Groups.FindIndex(x => x.Name == value.Name)] = value;
		}

		public UI()
		{
			Groups = new();
		}

		public void Render(SpriteBatch sb) { foreach (var g in Groups) g.Render(sb); }
		public void Update() { foreach (var g in Groups) g.Update(); }
	}
}
