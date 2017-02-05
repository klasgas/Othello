using System;

namespace OthelloLogic
{
	class PositionEvaluator
	{
		public PositionEvaluator(Board board)
		{
			_board = board;
		}
		
		// Ger poäng på positionen. En hög siffra är bra för CurrentPlayer.
		public float GetPositionScore()
		{
			int blackBricks;
			int whiteBricks;
			GetBrickCount(out blackBricks, out whiteBricks);
			
			int blackCorners;
			int whiteCorners;
			GetCornerCount(out blackCorners, out whiteCorners);
			
			int cornerWeight = 10;
			
			// > 0 bra för svart, < 0 bra för vit.
			float positionScore = blackBricks - whiteBricks + (blackCorners * cornerWeight) - (whiteCorners * cornerWeight);
			
			if (_board._currentPlayer == Square.SquareValue.White)
			{
				positionScore = -positionScore;
			}
			
			return positionScore;
		}
		
		public void GetBrickCount(out int blackBricks, out int whiteBricks)
		{
			blackBricks = 0;
			whiteBricks = 0;
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					if (_board._squares[x][y].IsSameColor(Square.SquareValue.Black))
					{
						blackBricks++;
					}
					else if (_board._squares[x][y].IsSameColor(Square.SquareValue.White))
					{
						whiteBricks++;
					}
				}
			}
			//Debugging.Assert(blackBricks >= 0 && whiteBricks >= 0);
			//Debugging.Assert(blackBricks + whiteBricks <= 64);
			return;
		}
		
		public void GetCornerCount(out int blackCorners, out int whiteCorners)
		{
			blackCorners = 0;
			whiteCorners = 0;
			Square[] corners = new Square[4];
			corners[0] = _board._squares[0][0];
			corners[1] = _board._squares[7][0];
			corners[2] = _board._squares[0][7];
			corners[3] = _board._squares[7][7];
			for (int i = 0; i < 4; i++)
			{
				if (corners[i].GetValue() == Square.SquareValue.White)
				{
					whiteCorners++;
				}
				else if (corners[i].GetValue() == Square.SquareValue.Black)
				{
					blackCorners++;
				}
			}
			return;
		}
		
		// data
		public Board _board;
	}
}
