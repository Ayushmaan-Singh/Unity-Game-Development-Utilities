using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Astek
{
    public readonly struct Matrix<T> : IEquatable<Matrix<T>>, IEquatable<T[,]>
    {
        private readonly T[,] _matrix;
        /// <param name="col">x in matrix i.e. left and right</param>
        /// <param name="row">y in matrix i.e. up and down</param>
        public T this[int col, int row]
        {
            get => _matrix[row, col];
            set => _matrix[row, col] = value;
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

        /// <param name="columnCount">column count/width of matrix</param>
        /// <param name="rowCount">row count/height of matrix</param>
        public Matrix(int columnCount, int rowCount)
        {
            Type type = typeof(T);
            if (!typeof(T).IsNumericType())
                throw new InvalidOperationException("Cannot make a matrix of type " + type.ToString());

            _matrix = new T[columnCount, rowCount];
            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        /// <param name="size">row and column count of square matrix</param>
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
            for (int r = 0; r < RowCount; r++)
                for (int c = 0; c < ColumnCount; c++)
                    _matrix[r, c] = copy._matrix[c, r];
        }

        public T[] GetRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex > RowCount)
                throw new ArgumentOutOfRangeException(nameof(rowIndex));

            T[] rowArray = new T[ColumnCount];
            for (int c = 0; c < ColumnCount; c++)
                rowArray[c] = _matrix[rowIndex, c];

            return rowArray;
        }
        public T[] GetColumn(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex > ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(columnIndex));

            T[] colArray = new T[RowCount];
            for (int r = 0; r < RowCount; r++)
                colArray[r] = _matrix[r, columnIndex];

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

            int rowCount = matrix.RowCount;
            // Create an augmented matrix [A | I]
            T[,] aug = new T[rowCount, 2 * rowCount];
            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < rowCount; c++)
                    aug[r, c] = matrix[r, c];
                aug[r, r + rowCount] = _one;
            }

            // Gauss-Jordan Elimination
            for (int r = 0; r < rowCount; r++)
            {
                // 1. Partial Pivoting: Find the largest pivot to reduce numerical error
                int pivotRow = r;
                for (int k = r + 1; k < rowCount; k++)
                {
                    if (Comparer<T>.Default.Compare(Abs(aug[k, r]), Abs(aug[pivotRow, r])) > 0)
                        pivotRow = k;
                }

                // Swap current row with pivot row
                if (pivotRow != r)
                {
                    for (int c = 0; c < 2 * rowCount; c++)
                    {
                        (aug[r, c], aug[pivotRow, c]) = (aug[pivotRow, c], aug[r, c]);
                    }
                }

                // 2. Normalize the pivot row
                T pivotVal = aug[r, r];
                if (pivotVal.Equals(_zero)) throw new Exception("Matrix is singular (not invertible).");

                for (int c = 0; c < 2 * rowCount; c++)
                    aug[r, c] = _divFunc(aug[r, c], pivotVal);

                // 3. Eliminate other entries in the current column
                for (int k = 0; k < rowCount; k++)
                {
                    if (k != r)
                    {
                        T factor = aug[k, r];
                        for (int j = 0; j < 2 * rowCount; j++)
                            aug[k, j] = _subtractFunc(aug[k, j], _multFunc(factor, aug[r, j]));
                    }
                }
            }

            // Extract the right side (the inverse)
            Matrix<T> inverse = new Matrix<T>(rowCount, rowCount);
            for (int i = 0; i < rowCount; i++)
                for (int j = 0; j < rowCount; j++)
                    inverse[i, j] = aug[i, j + rowCount];

            return inverse;
        }

        public static Matrix<T> operator +(Matrix<T> a, Matrix<T> b)
        {
            if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
                throw new ArgumentException("Matrices must have the same dimensions.");

            Matrix<T> result = new Matrix<T>(a.RowCount, a.ColumnCount);

            for (int r = 0; r < a.RowCount; r++)
                for (int c = 0; c < a.ColumnCount; c++)
                    result[r, c] = _addFunc(a[r, c], b[r, c]);

            return result;
        }
        public static Matrix<T> operator +(Matrix<T> a, T[,] b)
        {
            int rows = b.GetLength(0);
            int cols = b.GetLength(1);

            if (a.RowCount != rows || a.ColumnCount != cols)
                throw new ArgumentException("Matrices must have the same dimensions.");

            Matrix<T> result = new Matrix<T>(a.RowCount, a.ColumnCount);

            for (int r = 0; r < a.RowCount; r++)
                for (int c = 0; c < a.ColumnCount; c++)
                    result[r, c] = _addFunc(a[r, c], b[r, c]);

            return result;
        }
        public static Matrix<T> operator +(T[,] a, Matrix<T> b)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            if (rows != b.RowCount || cols != b.ColumnCount)
                throw new ArgumentException("Matrices must have the same dimensions.");

            Matrix<T> result = new Matrix<T>(cols, rows);

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    result[r, c] = _addFunc(a[r, c], b[r, c]);

            return result;
        }

        public static Matrix<T> operator *(Matrix<T> a, T b)
        {
            Matrix<T> result = new Matrix<T>(a.RowCount, a.ColumnCount);

            for (int r = 0; r < a.RowCount; r++)
                for (int c = 0; c < a.ColumnCount; c++)
                    result[r, c] = _multFunc(a[r, c], b);

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
            int rowCount = matrix.RowCount;
            int columnCount = matrix.ColumnCount;

            T[,] result = new T[rowCount, columnCount];
            for (int r = 0; r < rowCount; r++)
                for (int c = 0; c < columnCount; c++)
                    result[c, r] = matrix[r, c];

            return result;
        }
        public static implicit operator Matrix<T>(T[,] arr2d)
        {
            int rowCount = arr2d.GetLength(0);
            int columnCount = arr2d.GetLength(1);

            Matrix<T> result = new Matrix<T>(rowCount, columnCount);
            for (int r = 0; r < rowCount; r++)
                for (int c = 0; c < columnCount; c++)
                    result[c, r] = arr2d[r, c];

            return result;
        }

        private static T Abs(T val) => Comparer<T>.Default.Compare(val, default(T)) < 0 ? _negateFunc(val) : val;

        public bool Equals(Matrix<T> other) => Equals(_matrix, other._matrix) && RowCount == other.RowCount && ColumnCount == other.ColumnCount;
        public bool Equals(T[,] other) =>
            other != null &&
            Equals(_matrix, other) &&
            RowCount == other.GetLength(0) &&
            ColumnCount == other.GetLength(1);

        public override bool Equals(object obj)
        {
            if (obj is Matrix<T> matrix)
                return Equals(matrix);
            if (obj is T[,] arr2d)
                return Equals(arr2d);
            return false;
        }
        public override int GetHashCode() => HashCode.Combine(_matrix, RowCount, ColumnCount);
    }
}