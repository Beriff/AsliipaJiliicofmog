using AsliipaJiliicofmog.Data;

using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Interactive
{
	enum BPToken
	{
		Vital, Grab, Walk, Inside, Attached
	}
	internal struct Bodypart
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
			TreeNode<Bodypart> head = new(new("head", 30, BPToken.Vital, BPToken.Attached),torso);
			TreeNode<Bodypart> righthand = new(new("right hand", 35, BPToken.Grab, BPToken.Attached),torso);
			TreeNode<Bodypart> lefthand = new(new("left hand", 35, BPToken.Grab, BPToken.Attached), torso);
			TreeNode<Bodypart> rightleg = new(new("right leg", 50, BPToken.Walk, BPToken.Attached), torso);
			TreeNode<Bodypart> leftleg = new(new("left leg", 50, BPToken.Walk, BPToken.Attached), torso);

			TreeNode<Bodypart> heart = new(new("heart", 10, BPToken.Vital, BPToken.Inside), torso);


			return torso;
		}
	}
	internal class Creature : PhysicalEntity
	{
		public TreeNode<Bodypart> RootBodypart;
		public Creature(string name, Texture2D texture)
			: base(name, texture)
		{

		}
	}
}
