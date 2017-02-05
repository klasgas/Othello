using System;
using System.Diagnostics;
namespace OthelloLogic
{
	
	public class Square
	{
		public enum SquareValue
		{
			Empty,
			Black,
			White
		};
		
		public Square()
		{
			fValue = SquareValue.Empty;
		}
		
		public Square(SquareValue squareValue)
		{
			fValue = squareValue;
		}
		
		public SquareValue GetValue()
		{
			return fValue;
		}
		
		public void SetValue(SquareValue squareValue)
		{
			fValue = squareValue;
		}
		
		public bool IsSameColor(Square other)
		{
			return this.fValue == other.fValue;
		}
		
		public bool IsSameColor(Square.SquareValue color)
		{
			return this.fValue == color;
		}
		
		public bool IsEmpty()
		{
			return fValue == SquareValue.Empty;
		}
		
		// Obs: Returnerar false om n錱on av rutorna 鋜 tomma.
		public bool IsOtherColor(Square other)
		{
			if (this.fValue == SquareValue.Black && other.fValue == SquareValue.White)
			{
				return true;
			}
			if (this.fValue == SquareValue.White && other.fValue == SquareValue.Black)
			{
				return true;
			}
			return false;
		}
		
		// Obs: Man kan inte v鋘da en tom ruta.
		public void Flip()
		{
			//Debug.Assert(fValue != SquareValue.Empty);
			if (fValue == SquareValue.Black)
			{
				fValue = SquareValue.White;
			}
			else if (fValue == SquareValue.White)
			{
				fValue = SquareValue.Black;
			}
		}
		
		public override string ToString()
		{
			string s = ". ";
			if (fValue == SquareValue.Black)
			{
				s = "* ";
			}
			else if (fValue == SquareValue.White)
			{
				s = "o ";
			}
			return s;
		}
		
		private SquareValue fValue;
	}
	
	
	
	
}
