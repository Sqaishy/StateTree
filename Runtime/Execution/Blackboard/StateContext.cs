using System;
using UnityEngine;

namespace StateTree
{
	[Serializable]
	public abstract class StateContext
	{
		/// <summary>
		/// Clones a state context including variable setup from 'this' context instance
		/// </summary>
		public abstract StateContext Clone();
	}

	[Serializable]
	public class CharacterContext : StateContext
	{
		public bool isTrue;
		public Transform target;

		public override StateContext Clone()
		{
			return new CharacterContext
			{
				isTrue = isTrue,
				target = target
			};
		}
	}

	[Serializable]
	public class TestContext : StateContext
	{
		public string text;
		public Vector3 direction;

		public override StateContext Clone()
		{
			return new TestContext
			{
				text = text,
				direction = direction,
			};
		}
	}
}