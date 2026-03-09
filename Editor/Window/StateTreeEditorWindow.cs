using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateTree.Editor.Window
{
    public class StateTreeEditorWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private ThemeStyleSheet themeStyleSheet;

        private static StateGraph _currentStateGraph;
        private ScrollView moduleList;

        [OnOpenAsset]
        private static bool OpenAsset(int instanceID)
        {
            _currentStateGraph = EditorUtility.InstanceIDToObject(instanceID) as StateGraph;

            if (!_currentStateGraph)
                return false;

            GetWindow<StateTreeEditorWindow>(_currentStateGraph.name);
            return true;
        }

        public void CreateGUI()
        {
            visualTreeAsset.CloneTree(rootVisualElement);
            rootVisualElement.styleSheets.Add(themeStyleSheet);
            moduleList = rootVisualElement.Q<ScrollView>("ModuleList");
            rootVisualElement.Q<ModuleViewer>().OnSetDefaultModule += SetDefaultModule;

            ToolbarButton addButton = rootVisualElement.Q<ToolbarButton>("AddModule");
            addButton.clicked += () =>
            {
                StateModule stateModule = new StateModule("New Module");
                _currentStateGraph.nodes.Add(stateModule);
                StateModuleElement moduleElement = new StateModuleElement(stateModule);
                moduleElement.OnModuleSelected += ModuleSelected;
                moduleList.Add(moduleElement);

                ModuleSelected(stateModule);
            };

            ToolbarButton removeButton = rootVisualElement.Q<ToolbarButton>("RemoveModule");
            removeButton.clicked += () =>
            {
                StateModuleElement element = moduleList.Children().Last() as StateModuleElement;
                element.OnModuleSelected -= ModuleSelected;
                moduleList.Remove(element);
                _currentStateGraph.nodes.RemoveAt(_currentStateGraph.nodes.Count - 1);

                ModuleSelected(_currentStateGraph.nodes[^1]);
            };

            BuildList();
        }

        internal static void MarkGraphDirty() => EditorUtility.SetDirty(_currentStateGraph);

        private void ModuleSelected(StateModule module)
        {
            rootVisualElement.Q<ModuleViewer>().ChangeModule(module);
        }

        private void BuildList()
        {
            if (_currentStateGraph.nodes.Count == 0)
                return;

            foreach (StateModule module in _currentStateGraph.nodes)
            {
                StateModuleElement moduleElement = new StateModuleElement(module);
                moduleElement.OnModuleSelected += ModuleSelected;
                moduleList.Add(moduleElement);
            }

            ModuleSelected(_currentStateGraph.nodes[0]);
        }

        private void SetDefaultModule(StateModule module)
        {
            _currentStateGraph.DefaultState = module;

            foreach (VisualElement child in moduleList.Children())
            {
                if (child is not StateModuleElement moduleElement)
                    continue;

                if (!moduleElement.StateModule.Equals(module))
                    continue;

                //Grab the name first as I want to append (default) on top of the name
                string moduleName = moduleElement.ModuleName;
                moduleElement.ModuleName = $"{moduleName} (Default)";
                break;
            }
        }
    }
}