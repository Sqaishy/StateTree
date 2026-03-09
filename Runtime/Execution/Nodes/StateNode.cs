using System;
using UnityEngine;

namespace StateTree
{
    public abstract class StateNode
    {
        public Status CurrentStatus { get; private set; }
        public StateModule Module { get; internal set; }
        public GameObject Agent => Module.Graph.Agent.AgentObject;
        public StateBlackboard Blackboard => Module.Graph.Agent.Blackboard;
        public IParent Parent { get; internal set; }

        internal Status OnEnter()
        {
            CurrentStatus = Enter();
            return CurrentStatus;
        }

        internal Status OnUpdate()
        {
            CurrentStatus = Update();
            return CurrentStatus;
        }

        internal void OnExit()
        {
            if (CurrentStatus == Status.Running)
                CurrentStatus = Status.Interrupted;

            Exit();
        }

        internal void ResetStatus() => CurrentStatus = Status.Inactive;

        protected virtual Status Enter() => Status.Running;
        protected virtual Status Update() => Status.Success;

        protected virtual void Exit() { }

        public StateNode GetActiveLeaf()
        {
            StateNode leaf = this;

            while (leaf is IParent parent)
                leaf = parent.GetActiveChild();

            return leaf;
        }
    }

    public enum Status
    {
        Inactive,
        Running,
        Success,
        Failure,
        Interrupted,
    }
}
