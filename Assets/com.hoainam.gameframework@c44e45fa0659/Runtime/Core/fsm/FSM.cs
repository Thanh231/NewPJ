
using UnityEngine;
public class FSM : MonoBehaviour
{
	private FSM_State currentState;

	private void Update()
	{
		if (currentState != null)
		{
			currentState.OnUpdate();
		}
	}

	public void ChangeState(FSM_State newState)
	{
		newState.fsm = this;

		if (currentState != null)
		{
			currentState.OnExit();
		}
		currentState = newState;
		newState.OnBegin();
	}
}