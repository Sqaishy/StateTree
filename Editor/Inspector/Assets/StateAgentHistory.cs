using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateTree.Editor
{
    [UxmlElement]
    public partial class StateAgentHistory : VisualElement, IDisposable
    {
        private StateTreeAgent agent;
        private Button firstButton;
        private Button previousButton;
        private Button nextButton;
        private Button lastButton;
        private Label index;
        private StateInfo currentInfo;
        private VisualElement infoContainer;
        private StateModule currentModule;
        private Dictionary<int, StateInfo> infoHistory = new();

        public StateAgentHistory()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/_Project/StateTree/Editor/Inspector/Assets/StateAgentHistory.uxml");
            visualTree.CloneTree(this);

            firstButton = this.Q<Button>("First");
            previousButton = this.Q<Button>("Previous");
            nextButton = this.Q<Button>("Next");
            lastButton = this.Q<Button>("Last");
            index = this.Q<Label>("Index");
            currentInfo = this.Q<StateInfo>("CurrentInfo");
            currentInfo.Title = "CURRENT STATE";
            infoContainer = this.Q<VisualElement>("CurrentInfoContainer");
        }

        internal void Init(StateTreeAgent stateMachineAgent)
        {
            agent = stateMachineAgent;

            if (agent.StateHistory == null)
                return;

            firstButton.clicked += agent.StateHistory.SelectFirst;
            previousButton.clicked += agent.StateHistory.SelectPrevious;
            nextButton.clicked += agent.StateHistory.SelectNext;
            lastButton.clicked += agent.StateHistory.SelectLast;

            agent.StateHistory.OnStateSelected += OnStateSelected;

            if (agent.StateHistory.Current != null)
                OnStateSelected(agent.StateHistory.Current);
        }

        internal void Update()
        {
            if (agent.StateHistory.StateTimers.TryGetValue(agent.StateHistory.ActiveIndex,
                    out float currentTime))
                currentInfo.Time = Math.Round(currentTime, 2).ToString(CultureInfo.InvariantCulture) + "s";
        }

        private void OnStateSelected(LinkedListNode<StateModule> state)
        {
            index.text = $"{agent.StateHistory.ActiveIndex} / {agent.StateHistory.History.Count - 1}";

            //Maybe look at creating a new current info if it is a new state
            //Store the current infos in a dictionary based on active index

            if (!infoHistory.TryGetValue(agent.StateHistory.ActiveIndex, out StateInfo info))
            {
                if (currentModule is not null)
                    currentModule.OnModuleChanged -= ModuleChanged;
                currentModule = state.Value;
                currentModule.OnModuleChanged += ModuleChanged;

                info = new StateInfo
                {
                    style =
                    {
                        flexGrow = 1
                    }
                };
                infoHistory.Add(agent.StateHistory.ActiveIndex, info);

                info.CurrentState.StateName = state.Value.Name;
                info.Transitions = state.Value.Transitions.Select(transition =>
                    transition.to.Name).ToArray();
                info.InitializeTreeView(state.Value);

                currentInfo = info;
            }

            infoContainer.Clear();
            infoContainer.Add(info);
        }

        private void ModuleChanged(StateNode nodeChanged)
        {
            currentInfo.UpdateTreeView(nodeChanged);

            //If the root node has changed it means you are transitioning modules because the root node
            //has either failed or succeeded
            if (currentModule.Root != nodeChanged)
                return;

            currentInfo.Status = $"[{nodeChanged.CurrentStatus}]";

            foreach (StateNode child in currentModule.GetChildren())
                child.ResetStatus();
        }

        public void Dispose()
        {
            if (agent.StateHistory == null)
                return;

            firstButton.clicked -= agent.StateHistory.SelectFirst;
            previousButton.clicked -= agent.StateHistory.SelectPrevious;
            nextButton.clicked -= agent.StateHistory.SelectNext;
            lastButton.clicked -= agent.StateHistory.SelectLast;

            agent.StateHistory.OnStateSelected -= OnStateSelected;

        }
    }
}
