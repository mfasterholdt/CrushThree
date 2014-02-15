using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimationTheme
{
	public AnimationClip animIdle;
	public AnimationClip animJump;
	public AnimationClip animRun;

	public AnimationClip getAnim(PlayerAnim anim)
	{
		if(anim == PlayerAnim.Idle)
		{
			return animIdle;
		}
		else if(anim == PlayerAnim.Jump)
		{
			return animJump;
		}
		else if(anim == PlayerAnim.Run)
		{
			return animRun;
		}
		else 
		{
			return animIdle;
		}
	}
}

public enum PlayerAnim{Idle, Jump, Run};
