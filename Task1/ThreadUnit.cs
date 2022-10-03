namespace Task1;

public class ThreadUnit<T>
{
	private readonly T[][] _bigMatrix;

	private readonly T[][] _smallMatrix;

	private readonly int _difference;

	private readonly Func<T[][], T[][], int, T[][]> _func;

	private Thread _thread;

	public T[][] Result { get; private set; }

	public bool IsAlive => _thread is {IsAlive: true};

	public ThreadUnit(T[][] bigMatrix, T[][] smallMatrix, int difference, 
		Func<T[][], T[][], int, T[][]> func)
	{
		_bigMatrix = bigMatrix;
		_smallMatrix = smallMatrix;
		_func = func;
		_difference = difference;
	}

	public void StartThread()
	{
		_thread = new Thread(() => Result = _func.Invoke(
			_bigMatrix, _smallMatrix, _difference));
		_thread.Start();
	}
}