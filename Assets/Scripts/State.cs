public class State
{
	public delegate void StateFunction();
	
	public StateFunction Update;
	public StateFunction FixedUpdate;
	
	public void SetState(StateFunction fixedUpdate, StateFunction update)
	{
		this.FixedUpdate = fixedUpdate;
		this.Update = update;
	}
}