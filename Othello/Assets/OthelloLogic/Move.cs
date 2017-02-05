using System;
using System.Diagnostics;

namespace OthelloLogic
{
	struct Move
	{
		public Move(int x, int y, float score)
		{
			_coordinates = new IntPair ();
			_coordinates._x = x;
			_coordinates._y = y;
			_score = score;
		}
		
		public IntPair _coordinates;
		public float _score;
	}
	
	public /*struct*/ class IntPair
	{
		public IntPair()
		{
			_x = 0;
			_y = 0;
		}

		public IntPair(int x, int y)
		{
			_x = x;
			_y = y;
		}
		public int _x;
		public int _y;
	}
	
	public class Debugging
	{
		public static void Assert(bool condition)
		{
			if (!condition)
			{
				Debug.Assert(condition);
			}
		}
	}
}
