using System.Data.SqlTypes;

namespace Task1;

public class ContrastMatrixNumbers : ContrastMatrixBase<int>
{
	public ContrastMatrixNumbers(int[][] bigMatrix, int[][] smallMatrix) : base(bigMatrix, smallMatrix)
	{
	}

	protected override int GetDifference(int first, int second)
	{
		return first - second;
	}
}