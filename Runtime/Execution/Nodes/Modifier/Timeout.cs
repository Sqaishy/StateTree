using UnityEngine;

namespace StateTree
{
	/// <summary>
	/// Fails the branch after a specified number of seconds
	/// </summary>
	public class Timeout : Modifier
	{
		private float duration;
		private float currentTimer;

		public Timeout(float timeoutSeconds)
		{
			duration = timeoutSeconds;
		}

		protected override Status Enter()
		{
			if (duration <= 0)
			{
				Debug.LogError("Duration for timeout set to zero", Module.Graph);
				return Status.Failure;
			}

			return Module.StartNode(Child) switch
			{
				Status.Success => Status.Success,
				Status.Failure => Status.Failure,
				_ => Status.Running
			};
		}

		protected override Status Update()
		{
			currentTimer += Time.deltaTime;

			if (currentTimer >= duration)
				return Status.Failure;

			return Child.OnUpdate() switch
			{
				Status.Success => Status.Success,
				Status.Failure => Status.Failure,
				_ => Status.Running
			};
		}

		protected override void Exit()
		{
			currentTimer = 0;

			base.Exit();
		}
	}
}