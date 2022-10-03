using System.Diagnostics;
using System.Drawing;

namespace Task1;

public abstract class ContrastMatrixBase<T> 
{
	private readonly T[][] _bigMatrix;
	private readonly Size _bigMatrixSize;

	private readonly T[][] _smallMatrix;
	private readonly Size _smallMatrixSize;

	private bool _isWidthDevided;

	public Stopwatch TimerWithoutPrepare { get; private set; }
	public Stopwatch TimerWithPrepare { get; private set; }

	protected ContrastMatrixBase(T[][] bigMatrix, T[][] smallMatrix)
	{
		_bigMatrix = bigMatrix;
		_bigMatrixSize = new Size(_bigMatrix.Length, _bigMatrix[0].Length);
		if (_bigMatrixSize.Width != _bigMatrixSize.Height)
		{
			throw new ArgumentException($"Big Matrix must be square Size: {_bigMatrixSize}");
		}

		_smallMatrix = smallMatrix;
		_smallMatrixSize = new Size(_smallMatrix.Length, _smallMatrix[0].Length);
		if (_smallMatrixSize.Width != _smallMatrixSize.Height)
		{
			throw new ArgumentException($"Small Matrix must be square Size: {_smallMatrixSize}");
		}
	}

	public T[][] GetResult(int threadsCount = 1)
	{
		Validate(threadsCount);

		TimerWithoutPrepare = new Stopwatch();
		TimerWithPrepare = new Stopwatch();
		var difference = _bigMatrixSize.Width / _smallMatrixSize.Width;
		if (threadsCount == 1)
		{
			TimerWithPrepare.Start();
			TimerWithoutPrepare.Start();
			var result = GetPart(_bigMatrix, _smallMatrix, difference);
			TimerWithoutPrepare.Stop();
			TimerWithPrepare.Stop();
			return result;
		}

		TimerWithPrepare.Start();
		var threads = Split(threadsCount, difference);
		TimerWithoutPrepare.Start();
		foreach (var thread in threads)
		{
			thread.StartThread();
		}

		while (threads.Any(t=>t.IsAlive))
		{
			Thread.Sleep(10);
		}

		TimerWithoutPrepare.Stop();
		var partsResult = Merge(threads.Select(t=>t.Result));

		TimerWithPrepare.Stop();
		return partsResult.First();
	}

	protected abstract T GetDifference(T first, T second);
	
	private void Validate(int threadsCount)
	{
		if (threadsCount <= 0)
		{
			throw new ArgumentException("Threads count cannot be less than 1");
		}

		if (_bigMatrixSize.Height * _bigMatrixSize.Width % threadsCount != 0)
		{
			throw new ArgumentException("Cannot to divide on threads Big Matrix");
		}

		if (_smallMatrixSize.Height * _smallMatrixSize.Width % threadsCount != 0)
		{
			throw new ArgumentException("Cannot to divide on threads Small Matrix");
		}
	}

	private List<T[][]> Merge(IEnumerable<T[][]> countTasks)
	{
		var partsResult = new List<T[][]>(countTasks);
		while (partsResult.Count != 1)
		{
			var newPartsResult = new List<T[][]>();
			for (var i = 0; i < partsResult.Count - 1; i += 2)
			{
				var color1 = partsResult[i];
				var color2 = partsResult[i + 1];
				newPartsResult.Add(MergeMatrix(color1, color2));
			}

			_isWidthDevided = !_isWidthDevided;
			partsResult = newPartsResult;
		}

		return partsResult;
	}

	private List<ThreadUnit<T>> Split(int threadsCount, int difference)
	{
		var big = new List<T[][]> { _bigMatrix };
		var small = new List<T[][]> { _smallMatrix };
		var currentThread = 0;
		while (currentThread < threadsCount)
		{
			var newBig = new List<T[][]>();
			var newSmall = new List<T[][]>();
			for (var i = 0; i < big.Count; i++)
			{
				newBig.AddRange(GetHalf(big[i]));
				newSmall.AddRange(GetHalf(small[i]));
			}
			
			big = newBig;
			small = newSmall;
			currentThread += 2;
		}

		return big.Select((t, i) => new ThreadUnit<T>(t, small[i], difference, GetPart)).ToList();
	}

	private T[][] GetPart(T[][] bigMatrix, T[][] smallMatrix, int difference)
	{
		var result = BitmapManager.CreateMatrix<T>(bigMatrix.Length, bigMatrix[0].Length);
		for (var i = 0; i < smallMatrix.Length; i++)
		{
			for (var j = 0; j < smallMatrix[0].Length; j++)
			{
				for (var k = 0; k < difference; k++)
				{
					for (var l = 0; l < difference; l++)
					{
						var x = i * difference + k;
						var y = j * difference + l;
						var bigValue = bigMatrix[x][y];
						var smallValue = smallMatrix[i][j];
						result[x][y] = GetDifference(bigValue, smallValue);
					}
				}
			}
		}

		return result;
	}

	private T[][] MergeMatrix(T[][] firstMatrix, T[][] secondMatrix)
	{
		var width = firstMatrix.Length;
		var height = firstMatrix[0].Length;
		var newWidth = width;
		var newHeight = height;
		if (_isWidthDevided)
		{
			newWidth += width;
		}
		else
		{
			newHeight += height;
		}

		var result = BitmapManager.CreateMatrix<T>(newWidth, newHeight);
		for (var i = 0; i < width; i++)
		{
			for (var j = 0; j < height; j++)
			{
				result[i][j] = firstMatrix[i][j];
				var x = newWidth != width ? width : 0;
				var y = newHeight != height ? height : 0;
				result[i + x][j + y] = secondMatrix[i][j];
			}
		}

		return result;
	}

	private IEnumerable<T[][]> GetHalf(T[][] matrix)
	{
		var width = matrix.Length;
		var height = matrix[0].Length;
		var widthMultiplier = 0;
		var heightMultiplier = 0;
		if (width > height)
		{
			width /= 2;
			widthMultiplier = 1;
			_isWidthDevided = true;
		}
		else
		{
			height /= 2;
			heightMultiplier = 1;
			_isWidthDevided = false;
		}

		var colors1 = BitmapManager.CreateMatrix<T>(width, height);
		var colors2 = BitmapManager.CreateMatrix<T>(width, height);

		for (var i = 0; i < width; i++)
		{
			for (var j = 0; j < height; j++)
			{
				colors1[i][j] = matrix[i][j];
				var x = i + width * widthMultiplier;
				var y = j + height * heightMultiplier;
				colors2[i][j] = matrix[x][y];
			}
		}

		return new List<T[][]> { colors1, colors2 };
	}
}