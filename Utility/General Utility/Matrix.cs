using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Astek
{
    public readonly struct Matrix<T> : IEquatable<Matrix<T>>
    {
        private readonly T[,] _matrix;
        public T this[int column, int row]
        {
            get => _matrix[row, column];
            set => _matrix[row, column] = value;
        }
        public readonly int RowCount;
        public readonly int ColumnCount;

        // Static "adder" function compiled at runtime
        private static readonly Func<T, T, T> _addFunc;
        private static readonly Func<T, T, T> _subtractFunc;
        private static readonly Func<T, T, T> _multFunc;
        private static readonly Func<T, T, T> _divFunc;
        private static readonly Func<T, T> _negateFunc;
        private static readonly T _zero;
        private static readonly T _one;

        static Matrix()
        {
            // 1. Define parameters (T left, T right)
            ParameterExpression left = Expression.Parameter(typeof(T), "left");
            ParameterExpression right = Expression.Parameter(typeof(T), "right");

            //----- Add Func -----//
            // 2. Compile it into a reusable Func<T, T, T>
            try
            {
                _addFunc = Expression.Lambda<Func<T, T, T>>(Expression.Add(left, right), left, right).Compile();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not support the '+' operator.");
            }

            //----- Subtract Func -----//
            // 2. Compile it into a reusable Func<T, T, T>
            try
            {
                _subtractFunc = Expression.Lambda<Func<T, T, T>>(Expression.Subtract(left, right), left, right).Compile();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not support the '-' operator.");
            }

            //----- Mult Func -----//
            // 2. Compile it into a reusable Func<T, T, T>
            try
            {
                _multFunc = Expression.Lambda<Func<T, T, T>>(Expression.Multiply(left, right), left, right).Compile();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not support the '*' operator.");
            }

            //----- Div Func -----//
            //Compile it into a reusable Func<T, T, T>
            try
            {
                _divFunc = Expression.Lambda<Func<T, T, T>>(Expression.Divide(left, right), left, right).Compile();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not support the '/' operator.");
            }

            //----- Negate Func -----//
            // Compile it into a reusable Func<T, T>
            try
            {
                _negateFunc = Expression.Lambda<Func<T, T>>(Expression.Negate(left), left).Compile();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not support the '-' as negate operator.");
            }

            _zero = default(T);
            _one = (T)Convert.ChangeType(1, typeof(T));
        }

        public Matrix(int column, int row)
        {
            Type type = typeof(T);
            if (!typeof(T).IsNumericType())
                throw new InvalidOperationException("Cannot make a matrix of type " + type.ToString());

            _matrix = new T[column, row];
            RowCount = row;
            ColumnCount = column;
        }
        public Matrix(int size)
        {
            _matrix = new T[size, size];
            RowCount = size;
            ColumnCount = size;
        }
        public Matrix(Matrix<T> copy)
        {
            RowCount = copy.RowCount;
            ColumnCount = copy.ColumnCount;

            _matrix = new T[RowCount, ColumnCount];
            for (int row = 0; row < RowCount; row++)
            {
                for (int column = 0; column < ColumnCount; column++)
                    _matrix[row, column] = copy._matrix[column, row];
            }
        }

        public T[] GetRow(int row)
        {
            if (row < 0 || row > RowCount)
                throw new ArgumentOutOfRangeException(nameof(row));

            T[] rowArray = new T[ColumnCount];
            for (int i = 0; i < ColumnCount; i++)
                rowArray[i] = _matrix[row, i];

            return rowArray;
        }
        public T[] GetColumn(int col)
        {
            if (col < 0 || col > ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(col));

            T[] colArray = new T[RowCount];
            for (int i = 0; i < RowCount; i++)
                colArray[i] = _matrix[i, col];

            return colArray;
        }

        /// <summary> 
        ///For checking if this matric can be added with other matrix of same type
        /// </summary>
        /// <returns></returns>
        public static Matrix<T> Transpose(T[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            // Create a new matrix with swapped dimensions
            Matrix<T> result = new Matrix<T>(columns, rows);

            //Space complexity: O(mxn) for any matrix
            if (rows != columns)
            {
                for (int row = 0; row < rows; row++)
                {
                    for (int column = 0; column < columns; column++)
                        // Assign original row i, col j to result row j, col i
                        result[column, row] = matrix[row, column];
                }
            }
            //Space complexity:O(m) Only for square matrix (In place transpose)
            else
            {
                for (int row = 0; row < rows; row++)
                {
                    // Only iterate through the upper triangle (j > i) to avoid double-swapping
                    for (int col = row + 1; col < rows; col++)
                    {
                        result[row, col] = matrix[col, row];
                        result[col, row] = matrix[row, col];
                    }
                }
            }

            return result;
        }
        public static Matrix<T> Transpose(Matrix<T> matrix)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            // Create a new matrix with swapped dimensions
            Matrix<T> result = new Matrix<T>(columns, rows);

            //Space complexity: O(mxn) for any matrix
            if (rows != columns)
            {
                for (int row = 0; row < rows; row++)
                {
                    for (int column = 0; column < columns; column++)
                        // Assign original row i, col j to result row j, col i
                        result[column, row] = matrix[row, column];
                }
            }
            //Space complexity:O(m) Only for square matrix (In place transpose)
            else
            {
                for (int row = 0; row < rows; row++)
                {
                    // Only iterate through the upper triangle (j > i) to avoid double-swapping
                    for (int col = row + 1; col < rows; col++)
                    {
                        result[row, col] = matrix[col, row];
                        result[col, row] = matrix[row, col];
                    }
                }
            }

            return result;
        }
        public static Matrix<T> Inverse(Matrix<T> matrix)
        {
            if (matrix.RowCount != matrix.ColumnCount)
                throw new InvalidOperationException("Only square matrices are invertible.");

            int n = matrix.RowCount;
            // Create an augmented matrix [A | I]
            T[,] aug = new T[n, 2 * n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    aug[i, j] = matrix[i, j];
                aug[i, i + n] = _one;
            }

            // Gauss-Jordan Elimination
            for (int i = 0; i < n; i++)
            {
                // 1. Partial Pivoting: Find the largest pivot to reduce numerical error
                int pivotRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Comparer<T>.Default.Compare(Abs(aug[k, i]), Abs(aug[pivotRow, i])) > 0)
                        pivotRow = k;
                }

                // Swap current row with pivot row
                if (pivotRow != i)
                {
                    for (int j = 0; j < 2 * n; j++)
                    {
                        (aug[i, j], aug[pivotRow, j]) = (aug[pivotRow, j], aug[i, j]);
                    }
                }

                // 2. Normalize the pivot row
                T pivotVal = aug[i, i];
                if (pivotVal.Equals(_zero)) throw new Exception("Matrix is singular (not invertible).");

                for (int j = 0; j < 2 * n; j++)
                    aug[i, j] = _divFunc(aug[i, j], pivotVal);

                // 3. Eliminate other entries in the current column
                for (int k = 0; k < n; k++)
                {
                    if (k != i)
                    {
                        T factor = aug[k, i];
                        for (int j = 0; j < 2 * n; j++)
                            aug[k, j] = _subtractFunc(aug[k, j], _multFunc(factor, aug[i, j]));
                    }
                }
            }

            // Extract the right side (the inverse)
            Matrix<T> inverse = new Matrix<T>(n, n);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    inverse[i, j] = aug[i, j + n];

            return inverse;
        }

        public static Matrix<T> operator +(Matrix<T> a, Matrix<T> b)
        {
            if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
                throw new ArgumentException("Matrices must have the same dimensions.");

            Matrix<T> result = new Matrix<T>(a.RowCount, a.ColumnCount);

            for (int i = 0; i < a.RowCount; i++)
                for (int j = 0; j < a.ColumnCount; j++)
                    result[i, j] = _addFunc(a[i, j], b[i, j]);

            return result;
        }
        public static Matrix<T> operator +(Matrix<T> a, T[,] b)
        {
            int rows = b.GetLength(0);
            int cols = b.GetLength(1);

            if (a.RowCount != rows || a.ColumnCount != cols)
                throw new ArgumentException("Matrices must have the same dimensions.");

            Matrix<T> result = new Matrix<T>(a.RowCount, a.ColumnCount);

            for (int i = 0; i < a.RowCount; i++)
                for (int j = 0; j < a.ColumnCount; j++)
                    result[i, j] = _addFunc(a[i, j], b[i, j]);

            return result;
        }
        public static Matrix<T> operator +(T[,] a, Matrix<T> b)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            if (rows != b.RowCount || cols != b.ColumnCount)
                throw new ArgumentException("Matrices must have the same dimensions.");

            Matrix<T> result = new Matrix<T>(cols, rows);

            for (int row = 0; row < rows; row++)
                for (int column = 0; column < cols; column++)
                    result[row, column] = _addFunc(a[row, column], b[row, column]);

            return result;
        }

        public static Matrix<T> operator *(Matrix<T> a, T b)
        {
            Matrix<T> result = new Matrix<T>(a.RowCount, a.ColumnCount);

            for (int i = 0; i < a.RowCount; i++)
                for (int j = 0; j < a.ColumnCount; j++)
                    result[i, j] = _multFunc(a[i, j], b);

            return result;
        }
        public static Matrix<T> operator *(Matrix<T> a, Matrix<T> b)
        {
            if (a.ColumnCount != b.RowCount)
                throw new ArgumentException("Dimension mismatch");

            int rowsA = a.RowCount;
            int colsA = a.ColumnCount;
            int colsB = b.ColumnCount;

            Matrix<T> result = new Matrix<T>(rowsA, colsB);

            // OPTIMIZATION: Transpose B to improve CPU Cache Locality (Row-major access)
            T[,] bT = new T[colsB, colsA];
            for (int i = 0; i < colsA; i++)
                for (int j = 0; j < colsB; j++)
                    bT[j, i] = b[i, j];

            // Main Multiplication Loop
            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    T sum = _multFunc(a[i, 0], bT[j, 0]);
                    for (int k = 1; k < colsA; k++)
                    {
                        // Accessing A[i,k] and bT[j,k] is sequential in memory
                        sum = _addFunc(sum, _multFunc(a[i, k], bT[j, k]));
                    }

                    result[i, j] = sum;
                }
            }

            return result;
        }

        public static implicit operator T[,](Matrix<T> matrix)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            T[,] result = new T[rows, columns];
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                    result[col, row] = matrix[row, col];
            }

            return result;
        }
        public static implicit operator Matrix<T>(T[,] arr2d)
        {
            int rows = arr2d.GetLength(0);
            int columns = arr2d.GetLength(1);

            Matrix<T> result = new Matrix<T>(rows, columns);
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                    result[col, row] = arr2d[row, col];
            }

            return result;
        }

        private static T Abs(T val) => Comparer<T>.Default.Compare(val, default(T)) < 0 ? _negateFunc(val) : val;

        public bool Equals(Matrix<T> other) => Equals(_matrix, other._matrix) && RowCount == other.RowCount && ColumnCount == other.ColumnCount;
        public override bool Equals(object obj) => obj is Matrix<T> other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_matrix, RowCount, ColumnCount);
    }
}