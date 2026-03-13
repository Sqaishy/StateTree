using System;
using UnityEngine;

namespace StateTree
{

    public class StateMachineAgent : MonoBehaviour
    {
        [SerializeField] private StateGraph stateGraph;
        [SerializeField] private StateBlackboard blackboard;

        public new StateGraph StateGraph
        {
            get => stateGraph;
            set => stateGraph = value;
        }

        public new StateBlackboard Blackboard => blackboard;

        #if UNITY_EDITOR

        [SerializeField, HideInInspector] private StateGraph previousGraph;

        #endif

        //TODO Come back and fix this class at some point
        /*protected override void Awake()
        {
            blackboard.TryGetContext(out CharacterContext cc);

            StateModule idleState = new StateModule("Idle State");
            idleState.SetRoot(new StatusAction(Status.Running));

            /*StateModule investigateState = new StateModule("Investigate State");
            Sequence moveSequence = new Sequence();
            Sequence investigateSequence = new Sequence();
            moveSequence.AddChild(new MoveStrategy{ destination = cc.target.position });
            moveSequence.AddChild(investigateSequence);
            investigateSequence.AddChild(new WaitStrategy(3f));
            investigateSequence.AddChild(new ActionStrategy(() => Debug.Log(
                "Investigated the target!")));
            investigateState.SetRoot(moveSequence);

            PredicateCondition predicateTrue = new PredicateCondition(() => cc.isTrue);
            PredicateCondition predicateFalse = new PredicateCondition(() => !cc.isTrue);

            idleState.AddTransition(investigateState, new Condition[] { predicateTrue },
                ConditionOperator.AnyTrue);
            investigateState.AddTransition(idleState, new Condition[] { predicateFalse },
                ConditionOperator.AnyTrue);#1#

            stateGraph = new StateGraphBuilder(this, idleState).Build();
            stateGraph.name = "New State Graph";

            stateGraph.Enter();

            base.Awake();
        }

        protected override void CreateBlackboard()
        {
            RebuildBlackboard();
        }*/

        #region Agent Editor Validation

        #if UNITY_EDITOR

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            ValidateBlackboard();
        }

        private void ValidateBlackboard()
        {
            if (!stateGraph)
            {
                previousGraph = null;
                blackboard = null;
                return;
            }

            if (previousGraph != stateGraph)
            {
                previousGraph = stateGraph;
                blackboard = new StateBlackboard(stateGraph.SourceBlackboard);
            }
        }

        [ContextMenu("Reset Blackboard")]
        internal void RebuildBlackboard()
        {
            blackboard = new StateBlackboard(stateGraph.SourceBlackboard ?? previousGraph.SourceBlackboard);
        }

        #endif

        #endregion
    }
}