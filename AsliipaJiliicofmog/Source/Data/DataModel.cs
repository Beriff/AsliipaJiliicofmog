﻿
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections;
using System.Collections.Generic;

namespace AsliipaJiliicofmog.Data
{
	/// <summary>
	/// Acts like a dictionary, but every value can be accessed by two differently typed keys.
	/// Guarantees that all keys are unique.
	/// </summary>
	/// <typeparam name="K1">Key 1 type</typeparam>
	/// <typeparam name="K2">Key 2 type</typeparam>
	/// <typeparam name="V">Value</typeparam>
	public class DoubleKeyDict<K1, K2, V>
	{
		private List<K1> FirstKeys = new();
		private List<K2> SecondKeys = new();
		private List<V> _Values = new();

		public void Add(K1 k1, K2 k2, V v)
		{
			if (FirstKeys.Contains(k1)) { throw new ArgumentException("dictionary already has first key"); }
			if (SecondKeys.Contains(k2)) { throw new ArgumentException("dictionary already has second key"); }

			FirstKeys.Add(k1);
			SecondKeys.Add(k2);
			_Values.Add(v);
		}
		public void RemoveAt(int index)
		{
			FirstKeys.RemoveAt(index);
			SecondKeys.RemoveAt(index);
			_Values.RemoveAt(index);
		}
		public int IndexOf(K1 k1) => FirstKeys.IndexOf(k1);
		public int IndexOf(K2 k2) => SecondKeys.IndexOf(k2);
		public (K1 key1, K2 key2) GetKey(V val)
		{
			int i = _Values.IndexOf(val);
			return (FirstKeys[i], SecondKeys[i]);
		}

		public V this[K1 i]
		{
			get { return _Values[FirstKeys.IndexOf(i)]; }
		}
		public V this[K2 i]
		{
			get { return _Values[SecondKeys.IndexOf(i)]; }
		}

		public (List<K1> first, List<K2> second) Keys()
		{
			return (new(FirstKeys), new(SecondKeys));
		}

		public List<V> Values() => new(_Values);
	}

	public class TreeNode<T>
	{
		public TreeNode<T>? Parent;
		public List<TreeNode<T>> Children;
		public T Content;

		public static implicit operator T(TreeNode<T> val)
		{
			return val.Content;
		}

		public TreeNode(T data)
		{
			Children = new();
			Parent = null;
			Content = data;
		}
		public TreeNode(T data, TreeNode<T> parent)
		{
			Content = data;
			Children = new();
			SetParent(parent);
		}

		public void AddChild(TreeNode<T> node)
		{
			Children.Add(node);
			node.Parent = this;
		}

		public void RemoveChild(TreeNode<T> node)
		{
			Children.Remove(node);
			node.Parent = null;
		}

		public void SetParent(TreeNode<T> node)
		{
			RemoveParent();
			Parent = node;
			node.Children.Add(this);
		}

		public void RemoveParent()
		{
			if (Parent != null)
			{
				Parent.Children.Remove(this);
				Parent = null;
			}
		}

		public TreeNode<T> Search(Func<TreeNode<T>, bool> action)
		{
			foreach (var child in Children)
			{
				if (action(child))
					return child;
			}
			return null;
		}
	}

	public static class ExtendGraphicsDevice
	{
		public static void SetTarget(this GraphicsDevice a, RenderTarget2D? b)
		{
			a.SetRenderTarget(b);
			Registry.CurrentRenderTarget = b;
		}
	}
}
