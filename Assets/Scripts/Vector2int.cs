using UnityEngine;

public struct Vector2int  
{
	public int x;
	public int y;
	
	public static Vector2int one = new Vector2int(1, 1);
	public static Vector2int zero = new Vector2int(0, 0);
	
	public static Vector2int right = new Vector2int(1, 0);
	public static Vector2int left = new Vector2int(-1, 0);
	public static Vector2int up = new Vector2int(0, 1);
	public static Vector2int down = new Vector2int(0, -1);
	
	public static Vector2int[] Adjacent = new Vector2int[]{Vector2int.up, Vector2int.right, Vector2int.down, Vector2int.left};
	
	public Vector2int(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	
	public Vector2int(float x, float y)
	{
		this.x = Mathf.RoundToInt(x);
		this.y = Mathf.RoundToInt(y);
	}
	
	public override string ToString ()
	{
		return string.Format ("({0}, {1})", x, y);
	}
	
	//Covert
	public Vector3 ToVector3()
	{
		return new Vector3(x, y, 0);
	}
	
	//Length
	public int sqrMagnitude
    {
        get { return x * x + y * y; }
    }
	
	public float Magnitude
    {
        get { return Mathf.Sqrt(x * x + y * y); }
    }
	
	public Vector2int clamped
	{
		get 
		{
			Vector2int v = this;
			v.x = Mathf.Clamp(v.x, -1, 1);
			v.y = Mathf.Clamp(v.y, -1, 1);
			
			return v; 
		}
	}
	
	public void Clamp()
	{
		x = Mathf.Clamp(x, -1, 1);
		y = Mathf.Clamp(y, -1, 1);
	}
	
	//Addition, Subtraction
	public static Vector2int operator +(Vector2int v1, Vector2int v2)
	{
		v1.x += v2.x;
		v1.y += v2.y;
		return v1;
	}
	
	public static Vector2int operator -(Vector2int v1, Vector2int v2)
	{
		v1.x -= v2.x;
		v1.y -= v2.y;
		return v1;
	}
	
	public static Vector2int operator -(Vector2int v)
	{
		return v * -1;
	}
	
	//Multiplication
	public static Vector2int operator *(Vector2int v, int i)
	{
		v.x *= i;
		v.y *= i;
		return v;
	}
	public static Vector2int operator *(int i, Vector2int v){ return v*i;}
	public static Vector2int operator *(Vector2int v, float f){ return v * Mathf.RoundToInt(f);	}
	public static Vector2int operator *(float f, Vector2int v){ return v * Mathf.RoundToInt(f);	}
	
	
	//Multiplication
	public static Vector2int operator /(Vector2int v, int i)
	{
		v.x = Mathf.RoundToInt(i);
		v.y = Mathf.RoundToInt(i);
		return v;
	}
	public static Vector2int operator /(int i, Vector2int v){ return v/i;}
	public static Vector2int operator /(Vector2int v, float f){ return v / Mathf.RoundToInt(f);	}
	public static Vector2int operator /(float f, Vector2int v){ return v / Mathf.RoundToInt(f);	}
	
	//Comparising
	public static bool operator ==(Vector2int v1, Vector2int v2) 
	{
		return v1.x == v2.x && v1.y == v2.y;
	}
	
	public static bool operator !=(Vector2int v1, Vector2int v2) 
	{
		return !(v1 == v2);
	}
		
	public override bool Equals (object obj)
	{
		return base.Equals (obj);
	}
	
	public override int GetHashCode ()
	{
		return x ^ y;
	}
}