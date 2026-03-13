using UnityEngine;

namespace StateTree.Authoring.Code
{
	public class ClownAuthoringGraph : CodeAuthoringGraph
	{
		public override StateGraph CreateRuntimeGraph()
		{
			//Put all clown related state logic in here
			return CreateInstance<StateGraph>();
		}

		public override StateBlackboard CreateRuntimeBlackboard()
		{
			return new StateBlackboard()
			{
				Contexts =
				{
					new CharacterContext(),
					new CustomStateContext(),
				}
			};
		}
	}
}