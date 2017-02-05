using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

namespace OthelloLogic
{
	
	public class Board
	{
		
		public Board()
		{
			InitSquaresArray();

			// Svart b鰎jar
			_currentPlayer = Square.SquareValue.Black;

			_lastMoveCoordinates = new IntPair ();
			_lastMoveCoordinates._x = -1;
			_lastMoveCoordinates._y = -1;
		}

		public void SetupInitialPosition()
		{
			_squares[3][3].SetValue(Square.SquareValue.Black);
			_squares[4][4].SetValue(Square.SquareValue.Black);
			_squares[3][4].SetValue(Square.SquareValue.White);
			_squares[4][3].SetValue(Square.SquareValue.White);
		}
		
		public void InitSquaresArray()
		{
			_squares = new Square[8][];
			for (int i = 0; i < 8; i++)
			{
				_squares[i] = new Square[8];
			}
			
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					_squares[x][y] = new Square();
				}
			}
		}

		public void InitFromOther(Board other)
		{
			_currentPlayer = other._currentPlayer;
			
			//InitSquaresArray();
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					_squares[x][y].SetValue(other._squares[x][y].GetValue());
				}
			}
			
			_lastMoveCoordinates = other._lastMoveCoordinates;
		}
		
		// G鰎 en deep copy av other.
		public Board(Board other)
		{
			_currentPlayer = other._currentPlayer;
			
			InitSquaresArray();
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					_squares[x][y].SetValue(other._squares[x][y].GetValue());
				}
			}
			
			_lastMoveCoordinates = other._lastMoveCoordinates;
		}

		public ArrayList GetAdjacentSquares(int x, int y)
		{
			ArrayList squares = new ArrayList();
			
			int lowestX = x - 1;
			if (x == 0)
			{
				lowestX = 0;
			}
			
			int lowestY = y - 1;
			if (y == 0)
			{
				lowestY = 0;
			}
			
			int highestX = x + 1;
			if (x == 7)
			{
				highestX = 7;
			}
			
			int highestY = y + 1;
			if (y == 7)
			{
				highestY = 7;
			}
			
			for (int x1 = lowestX; x1 <= highestX; x1++)
			{
				for (int y1 = lowestY; y1 <= highestY; y1++)
				{
					if (x1 != x && y1 != y)
					{
						IntPair coordinates = new IntPair(x1, y1);
						squares.Add(coordinates);
					}
				}
			}
			
			return squares;
		}
		
		public bool LineCanBeFlipped(int x, int y, int dx, int dy, Square.SquareValue playerColor)
		{
			bool first = true;
			x += dx;
			y += dy;
			while (x >= 0 && x <= 7 && y >= 0 && y <= 7)
			{
				Square square = _squares[x][y];
				if (square.IsEmpty())
				{
					return false;
				}
				if (first)
				{
					first = false;
					if (square.IsSameColor(playerColor))
					{
						return false;
					}
				}
				else {
					if (square.IsSameColor(playerColor))
					{
						return true;
					}
				}
				x += dx;
				y += dy;
			}
			return false;
		}
		
		public void FlipLine(int x, int y, int dx, int dy, Square.SquareValue playerColor)
		{
			x += dx;
			y += dy;
			while (x >= 0 && x <= 7 && y >= 0 && y <= 7)
			{
				Square square = _squares[x][y];
				if (square.IsSameColor(playerColor))
				{
					return;
				}
				square.Flip();
				x += dx;
				y += dy;
			}
		}
		
		public bool MoveIsLegal(int x, int y)
		{
			//Debugging.Assert(_currentPlayer != Square.SquareValue.Empty);
			//Debugging.Assert(x >= 0 && x <= 7);
			//Debugging.Assert(y >= 0 && y <= 7);
			Square square = _squares[x][y];
			if (square.IsEmpty())
			{
				// Kolla alla 錿ta riktningar
				// dx: -1 = v鋘ster, +1 = h鰃er
				// dy: -1 = upp, +1 = ner
				for (int dx = -1; dx <= 1; dx++)
				{
					for (int dy = -1; dy <= 1; dy++)
					{
						if (dx == 0 && dy == 0)
						{
							continue;
						}
						else {
							if (LineCanBeFlipped(x, y, dx, dy, _currentPlayer))
							{
								return true;
							}
						}
					}
				}
			}
			
			return false;
		}
		
		public void Pass()
		{
			//Debugging.Assert(GetAllLegalMoves().Count == 0);
			
			_lastMoveCoordinates._x = -1;
			_lastMoveCoordinates._y = -1;
			ChangeCurrentPlayer();
		}
		
		public void MakeMove(int x, int y)
		{
			//Debugging.Assert(MoveIsLegal(x, y));
			
			_squares[x][y].SetValue(_currentPlayer);
			
			// Kolla alla 錿ta riktningar
			// dx: -1 = v鋘ster, +1 = h鰃er
			// dy: -1 = upp, +1 = ner
			for (int dx = -1; dx <= 1; dx++)
			{
				for (int dy = -1; dy <= 1; dy++)
				{
					if (dx == 0 && dy == 0)
					{
						continue;
					}
					
					if (LineCanBeFlipped(x, y, dx, dy, _currentPlayer))
					{
						FlipLine(x, y, dx, dy, _currentPlayer);
					}
				}
			}
			
			_lastMoveCoordinates._x = x;
			_lastMoveCoordinates._y = y;
			
			ChangeCurrentPlayer();
		}
		
		private void ChangeCurrentPlayer()
		{
			if (_currentPlayer == Square.SquareValue.Black)
			{
				_currentPlayer = Square.SquareValue.White;
			}
			else
			{
				_currentPlayer = Square.SquareValue.Black;
			}
		}
		
		public ArrayList GetAllLegalMoves()
		{
			ArrayList legalMoves = new ArrayList(); // Prova att ha en ObjectPool med endast en ArrayList. Eller: Prova att returnera med yield.
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					if (MoveIsLegal(x, y))
					{
						IntPair move = new IntPair(x, y);
						legalMoves.Add(move);
					}
				}
			}
			return legalMoves;
		}

		public IEnumerable<IntPair> GetAllLegalMoves2()
		{
			ObjectPool<IntPair>.Instance.Clear ();
			//ArrayList legalMoves = new ArrayList(); // Prova att ha en ObjectPool med endast en ArrayList. Eller: Prova att returnera med yield.
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					if (MoveIsLegal(x, y))
					{
						IntPair move = ObjectPool<IntPair>.Instance.GetObject();
						move._x = x;
						move._y = y;
						yield return move;
						//yield return new IntPair(x, y); // testa att ha en objectpool med IntPair, behöver inte vara så många (10-20 st). så många som maximalt antal möjliga drag i en position. 
						//legalMoves.Add(move);
					}
				}
			}
			//return legalMoves;
		}

		
		public bool GameIsOver()
		{
			bool gameIsOver = false;
			if (CurrentPlayerMustPass())
			{
				ChangeCurrentPlayer();
				if (CurrentPlayerMustPass())
				{
					gameIsOver = true;
				}
				ChangeCurrentPlayer();
			}
			return gameIsOver;
		}
		
		public bool CurrentPlayerMustPass()
		{
			bool mustPass = false;
			if (GetAllLegalMoves().Count == 0)
			{
				mustPass = true;
			}
			return mustPass;
		}
		
		
		public string TraceBoard(string label)
		{
			var sb = new System.Text.StringBuilder();
			
			sb.AppendLine(string.Empty);
			sb.AppendLine(label);
			for (int y = 0; y < 8; y++)
			{
				for (int x = 0; x < 8; x++)
				{
					Square square = _squares[x][y];
					sb.Append(square.ToString());
				}
				sb.AppendLine("");
			}
			
			string player = "White";
			if (_currentPlayer == Square.SquareValue.Black)
			{
				player = "Black";
			}
			sb.AppendLine("fCurrentPlayer = " + player);
			
			return sb.ToString();
		}
		
		// data
		public Square[][] _squares;
		public Square.SquareValue _currentPlayer;
		public IntPair _lastMoveCoordinates;
	}
	
	
	
	
}
