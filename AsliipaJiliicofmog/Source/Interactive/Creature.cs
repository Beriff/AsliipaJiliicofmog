using AsliipaJiliicofmog.Data;
using AsliipaJiliicofmog.Env;
using AsliipaJiliicofmog.Env.Items;
using AsliipaJiliicofmog.Rendering;

using Microsoft.Xna.Framework;

namespace AsliipaJiliicofmog.Interactive
{
	public enum BPToken
	{
		Vital, Grab, Walk, Inside, Attached
	}
	public struct Bodypart
	{
		public int Health;
		public int MaxHealth;
		public string Name;
		public List<BPToken> Tokens;

		public Bodypart(string name, int health, params BPToken[] tokens)
		{
			Name = name;
			MaxHealth = Health = health;
			Tokens = new(tokens);
		}
		public Bodypart(string name, int health, List<BPToken> tokens)
		{
			Name = name;
			MaxHealth = Health = health;
			Tokens = tokens;
		}

		public readonly Bodypart Copy() => new(Name, MaxHealth, new List<BPToken>(Tokens));

		public readonly bool Has(BPToken token) => Tokens.Contains(token);

		public static Func<TreeNode<Bodypart>, bool> Find(string name)
			=>
			(x) => ((Bodypart)x).Name == name;

		public static TreeNode<Bodypart> Humanoid()
		{
			TreeNode<Bodypart> torso = new(new("torso", 100, BPToken.Vital));
			TreeNode<Bodypart> head = new(new("head", 30, BPToken.Vital, BPToken.Attached), torso);
			TreeNode<Bodypart> righthand = new(new("right hand", 35, BPToken.Grab, BPToken.Attached), torso);
			TreeNode<Bodypart> lefthand = new(new("left hand", 35, BPToken.Grab, BPToken.Attached), torso);
			TreeNode<Bodypart> rightleg = new(new("right leg", 50, BPToken.Walk, BPToken.Attached), torso);
			TreeNode<Bodypart> leftleg = new(new("left leg", 50, BPToken.Walk, BPToken.Attached), torso);

			TreeNode<Bodypart> heart = new(new("heart", 10, BPToken.Vital, BPToken.Inside), torso);


			return torso;
		}
	}

	public class Creature : PhysicalEntity
	{
		public TreeNode<Bodypart> RootBodypart;
		public float Speed = 1;
		public Inventory Inventory { get; set; }
		public Creature(string name, IGameTexture texture, TreeNode<Bodypart> root)
			: base(name, texture)
		{
			Inventory = new(name);
			RootBodypart = root;
		}

		public void Move(Vector2 shift, World w) => Shift(shift * Speed, w);
	}
}
