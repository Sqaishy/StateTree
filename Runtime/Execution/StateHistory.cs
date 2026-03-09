using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateTree
{
	public class StateHistory : IDisposable
	{
		public int ActiveIndex { get; private set; }
		public LinkedListNode<StateModule> Current { get; private set; }
		public LinkedList<StateModule> History { get; } = new();
		public Dictionary<int, float> StateTimers { get; } = new();

		public event Action<LinkedListNode<StateModule>> OnStateSelected;

		private StateTreeAgent agent;
		private float currentStateTimer;

		public StateHistory(StateTreeAgent agent)
		{
			this.agent = agent;
		}

		internal void InitializeGraph() => agent.StateGraph.OnStateChanged += OnStateChanged;

		internal void Update()
		{
			currentStateTimer += Time.deltaTime;

			StateTimers[History.Count - 1] = currentStateTimer;
		}

		public void SelectNext()
		{
			if (Current?.Next == null)
				return;

			ActiveIndex++;

			Select(Current.Next);
		}

		public void SelectPrevious()
		{
			if (Current?.Previous == null)
				return;

			ActiveIndex--;

			Select(Current.Previous);
		}

		public void SelectFirst()
		{
			ActiveIndex = 0;

			Select(History.First);
		}

		public void SelectLast()
		{
			ActiveIndex = History.Count - 1;

			Select(History.Last);
		}

		private void Select(LinkedListNode<StateModule> node)
		{
			Current = node;

			if (StateTimers.TryAdd(ActiveIndex, currentStateTimer))
				currentStateTimer = 0;

			OnStateSelected?.Invoke(Current);
		}

		private void OnStateChanged(StateModule from, StateModule to)
		{
			History.AddLast(to);

			SelectLast();
		}

		public void Dispose()
		{
			agent.StateGraph.OnStateChanged -= OnStateChanged;
		}
	}
}