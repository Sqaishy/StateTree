using System;
using UnityEditor;
using UnityEngine.UIElements;
using Clickable = UnityEngine.UIElements.Clickable;
using TextField = UnityEngine.UIElements.TextField;

namespace StateTree.Editor.Window
{
    public class StateModuleElement : VisualElement
    {
        public string ModuleName
        {
            get => this.Q<TextField>().text;
            set => this.Q<TextField>().value = value;
        }

        public event Action<StateModule> OnModuleSelected;

        public StateModule StateModule { get; }

        public StateModuleElement(StateModule module)
        {
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                AssetDatabase.GUIDToAssetPath("dc0f14542ad641b44802f8fdb301c488"))
                .CloneTree(this);

            StateModule = module;
            ModuleName = StateModule.Name;

            this.AddManipulator(new Clickable(Clicked));

            this.Q<TextField>().RegisterValueChangedCallback(ChangeModuleName);
        }

        private void ChangeModuleName(ChangeEvent<string> evt)
        {
            StateModule.Name = evt.newValue;
        }

        private void Clicked(EventBase obj)
        {
            OnModuleSelected?.Invoke(StateModule);
        }
    }
}
