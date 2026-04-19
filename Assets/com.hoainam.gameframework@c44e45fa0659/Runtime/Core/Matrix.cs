
using System.Text;

public class Matrix<T>
{
    public T[,] data;

    public void CopyToArray(T[,] arr)
    {
        var row = data.GetLength(0);
        var column = data.GetLength(1);

        for (var r = 0; r < row; r++)
        {
            for (var c = 0; c < column; c++)
            {
                arr[r, c] = data[r, c];
            }
        }
    }

    public Matrix(T[,] data)
    {
        this.data = (T[,])data.Clone();
    }

    public Matrix<T> FlipVertically()
    {
        var newData = (T[,])data.Clone();
        var row = newData.GetLength(0);
        var column = newData.GetLength(1);

        var count = row / 2;
        for (var c = 0; c < column; c++)
        {
            for (var i = 0; i < count; i++)
            {
                var j = -i + row - 1;
                var t = newData[i, c];
                newData[i, c] = newData[j, c];
                newData[j, c] = t;
            }
        }

        return new Matrix<T>(newData);
    }

    public Matrix<T> FlipHorizontally()
    {
        var newData = (T[,])data.Clone();
        var row = newData.GetLength(0);
        var column = newData.GetLength(1);

        var count = column / 2;
        for (var r = 0; r < row; r++)
        {
            for (var i = 0; i < count; i++)
            {
                var j = -i + column - 1;
                var t = newData[r, i];
                newData[r, i] = newData[r, j];
                newData[r, j] = t;
            }
        }

        return new Matrix<T>(newData);
    }

    public Matrix<T> RotateRight()
    {
        var originalRows = data.GetLength(0);
        var originalCols = data.GetLength(1);

        var newData = new T[originalCols, originalRows];

        for (var r = 0; r < originalRows; r++)
        {
            for (var c = 0; c < originalCols; c++)
            {
                newData[c, originalRows - 1 - r] = data[r, c];
            }
        }

        return new Matrix<T>(newData);
    }

    public Matrix<T> RotateLeft()
    {
        var originalRows = data.GetLength(0);
        var originalCols = data.GetLength(1);

        var newData = new T[originalCols, originalRows];

        for (var r = 0; r < originalRows; r++)
        {
            for (var c = 0; c < originalCols; c++)
            {
                newData[originalCols - 1 - c, r] = data[r, c];
            }
        }

        return new Matrix<T>(newData);
    }

    public Matrix<T> Rotate180()
    {
        var originalRows = data.GetLength(0);
        var originalCols = data.GetLength(1);

        var newData = new T[originalRows, originalCols];

        for (var r = 0; r < originalRows; r++)
        {
            for (var c = 0; c < originalCols; c++)
            {
                newData[originalRows - 1 - r, originalCols - 1 - c] = data[r, c];
            }
        }

        return new Matrix<T>(newData);
    }

    public override string ToString()
    {
        var row = data.GetLength(0);
        var column = data.GetLength(1);

        var sb = new StringBuilder();
        for (var r = 0; r < row; r++)
        {
            for (var c = 0; c < column; c++)
            {
                sb.Append(data[r, c]).Append(' ');
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }
}