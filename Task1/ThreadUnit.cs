namespace Task1;

public class ThreadUnit<T>
{
	private readonly Func<T[][]> _func;

	private Thread _thread;

	public T[][] Result { get; private set; }

	public bool IsAlive => _thread is {IsAlive: true};

	public ThreadUnit(Func<T[][]> func)
	{
		_func = func;
	}

	public void StartThread()
	{
		_thread = new Thread(() => Result = _func.Invoke());
		_thread.Start();
	}
}