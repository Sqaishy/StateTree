using UnityEngine;

namespace StateTree
{
	/// <summary>
	/// Wait for a specified number of seconds
	/// </summary>
	public class WaitAction : ActionNode
	{
		private float waitTime;
		private float currentTime;

		public WaitAction(float waitTimeSeconds)
		{
			waitTime = waitTimeSeconds;
		}

		protected override Status Update()
		{
			currentTime += Time.deltaTime;

			return currentTime >= waitTime ? Status.Success : Status.Running;
		}

		protected override void Exit()
		{
			currentTime = 0;
		}
	}
}