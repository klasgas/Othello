using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

using UnityEngine;

namespace OthelloLogic
{
	
	public class MoveSearcher
	{
		private static List<Board> _boardPool;
		private static int _boardIndex = 0;
		private static int _boardCount = 1000000;

		public static void InitBoardPool()
		{
			if(_boardPool == null)
			{
				_boardPool = new List<Board>();

				for(int i = 0; i < _boardCount; i++)
				{
					_boardPool.Add (new Board());
				}
			}
			_boardIndex = 0;
		}

		private static void Grow()
		{
			for(int i = 0; i < 1000; i++)
			{
				_boardPool.Add (new Board());
			}
		}

		public static Board GetBoardFromPool()
		{
			if (_boardIndex >= _boardPool.Count) {
				Grow();
				UnityEngine.Debug.Log("After Grow: " + _boardPool.Count);
			}
			//UnityEngine.Debug.Log(_boardIndex.ToString ());
			return _boardPool [_boardIndex++];
		}

		public static Board CloneBoard(Board other)
		{
			var board = GetBoardFromPool ();
			board.InitFromOther(other);
			return board;
		}





		public static IntPair FindMove(Board boardBeforeMove)
		{
			InitBoardPool ();
			ObjectPool<List<IntPair>>.Instance.Clear ();

			uint searchDepth = 5;
			
			Move bestMove = new Move(-1, -1, float.NegativeInfinity);
			
			//ArrayList allMoves = board.GetAllLegalMoves();
			//Board boardBeforeMove = new Board(board);
			//Board boardBeforeMove = CloneBoard(board);

			foreach (IntPair coordinates in boardBeforeMove.GetAllLegalMoves())
			{
				Board board = CloneBoard(boardBeforeMove); 
				board.MakeMove(coordinates._x, coordinates._y);
				
				//float score = -MinMaxSearch(new Board(board), searchDepth);
				float score = -MinMaxSearch(board, searchDepth);
				
				if (score > bestMove._score)
				{
					bestMove._score = score;
					bestMove._coordinates._x = coordinates._x;
					bestMove._coordinates._y = coordinates._y;
				}
				
				//board = new Board(boardBeforeMove);
				//board = CloneBoard(boardBeforeMove); // verkar onödigt. kolla om vi bara kan sätta den.
			}
			
			return bestMove._coordinates;
		}
		
		
		public static float MinMaxSearch(Board boardBeforeMove, uint depth)
		{
			float bestScore = float.NegativeInfinity;
			
			if (depth == 0 || boardBeforeMove.CurrentPlayerMustPass())
			{
				PositionEvaluator evaluator = new PositionEvaluator(boardBeforeMove);
				return evaluator.GetPositionScore();
			}
			else
			{
				var allMoves = boardBeforeMove.GetAllLegalMoves();
				
				//Board boardBeforeMove = new Board(board);
				//Board boardBeforeMove = CloneBoard(board);
				
				foreach (IntPair coordinates in allMoves)
				{
					Board board = CloneBoard(boardBeforeMove);
					board.MakeMove(coordinates._x, coordinates._y);
					
					//float score = -MinMaxSearch(new Board(board), depth - 1);
					float score = -MinMaxSearch(board, depth - 1);
					
					if (score > bestScore)
					{
						bestScore = score;
					}
					
					//board = new Board(boardBeforeMove);

				}
			}
			
			return bestScore;
		}
	}
	
	
	
	
}
