using System.Collections.Generic;

namespace OthelloLogic
{

	public sealed class ObjectPool<T> where T : class, new()
	{
		private static readonly ObjectPool<T> instance = new ObjectPool<T>();

		private List<T> _pool;
		private int _index;
		
		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static ObjectPool()
		{
		}
		
		private ObjectPool()
		{
			_pool = new List<T>();
		}

		private void Grow()
		{
			for(int i = 0; i < 10; i++)
			{
				_pool.Add (new T());
			}
		}

		public static ObjectPool<T> Instance
		{
			get
			{
				return instance;
			}
		}

		public T GetObject()
		{
			if (_index >= _pool.Count) {
				Grow();
			}

			return _pool [_index++];
		}

		public void Clear()
		{
			_index = 0;
			//_pool.Clear ();
		}
	}
}