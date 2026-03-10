using UnityEngine;

namespace StateTree.Authoring.Code
{
	[CreateAssetMenu(menuName = "State Tree/AI/Clown")]
	public class ClownAuthoringGraph : CodeAuthoringGraph
	{
		public override StateGraph CreateRuntimeGraph()
		{
			//Put all clown related state logic in here
			return CreateInstance<StateGraph>();
		}

		public override StateBlackboard CreateRuntimeBlackboard()
		{
			return new StateBlackboard();
		}
	}
}