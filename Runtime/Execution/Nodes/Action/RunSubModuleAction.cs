namespace StateTree
{
	public class RunSubModuleAction : ActionNode
	{
		private StateModule stateModule;

		public RunSubModuleAction(StateModule stateModule)
		{
			this.stateModule = stateModule;
		}

		protected override Status Enter()
		{
			return stateModule.Enter();
		}

		protected override Status Update()
		{
			stateModule.Update();

			return stateModule.Root.CurrentStatus;
		}

		protected override void Exit()
		{
			stateModule.Exit();
		}
	}
}