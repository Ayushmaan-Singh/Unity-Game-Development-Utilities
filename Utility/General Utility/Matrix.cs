using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Astek
{
    public struct Matrix<T>
    {
        private readonly T[,] _matrix;
        public T this[int column, int row]
        {
            get => _matrix[column, row];
            set => _matrix[column, row] = value;
        }
        public readonly int RowCount;
        public readonly int ColumnCount;

        // --- Logic for Type Validation --- //
        private readonly bool IsAddable;
        private readonly MethodInfo _addMethod;
        private readonly bool IsMultiplicative;
        private readonly MethodInfo _multiplyMethod;

        public Matrix(int column, int row)
        {
            _matrix = new T[column, row];
            RowCount = _matrix.GetLength(0);
            ColumnCount = _matrix.GetLength(1);

            //Type checking for addable
            Type t = typeof(T);
            // Check for primitives (int, float, etc.)
            if (t.IsPrimitive && t != typeof(bool) && t != typeof(char))
            {
                IsAddable = true;
                _addMethod = null;

                IsMultiplicative = true;
                _multiplyMethod = null;
            }
            else
            {
                // Check for op_Addition (Vector3, Color, custom classes)
                _addMethod = t.GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
                IsAddable = _addMethod != null;

                _multiplyMethod = t.GetMethod("op_Multiply", BindingFlags.Static | BindingFlags.Public);
                IsMultiplicative = _multiplyMethod != null;
            }
        }
        public Matrix(int size)
        {
            _matrix = new T[size, size];
            RowCount = _matrix.GetLength(0);
            ColumnCount = _matrix.GetLength(1);

            //Type checking for addable
            Type t = typeof(T);
            // Check for primitives (int, float, etc.)
            if (t.IsPrimitive && t != typeof(bool) && t != typeof(char))
            {
                IsAddable = true;
                _addMethod = null;

                IsMultiplicative = true;
                _multiplyMethod = null;
            }
            else
            {
                // Check for op_Addition (Vector3, Color, custom classes)
                _addMethod = t.GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
                IsAddable = _addMethod != null;

                _multiplyMethod = t.GetMethod("op_Multiply", BindingFlags.Static | BindingFlags.Public);
                IsMultiplicative = _multiplyMethod != null;
            }
        }
        public Matrix(Matrix<T> copy)
        {
            int rows = copy._matrix.GetLength(0);
            int columns = copy._matrix.GetLength(1);

            _matrix = new T[rows, columns];
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                    _matrix[row, column] = copy._matrix[column, row];
            }

            RowCount = rows;
            ColumnCount = columns;

            //Type checking for addable
            Type t = typeof(T);
            // Check for primitives (int, float, etc.)
            if (t.IsPrimitive && t != typeof(bool) && t != typeof(char))
            {
                IsAddable = true;
                _addMethod = null;

                IsMultiplicative = true;
                _multiplyMethod = null;
            }
            else
            {
                // Check for op_Addition (Vector3, Color, custom classes)
                _addMethod = t.GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
                IsAddable = _addMethod != null;

                _multiplyMethod = t.GetMethod("op_Multiply", BindingFlags.Static | BindingFlags.Public);
                IsMultiplicative = _multiplyMethod != null;
            }
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

        public static Matrix<T> operator +(Matrix<T> a, Matrix<T> b)
        {
            if (!a.IsAddable)
                throw new InvalidOperationException($"Type {typeof(T).Name} does not support +.");

            if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
                throw new ArgumentException("Matrices must have the same dimensions.");

            Matrix<T> result = new Matrix<T>(a.RowCount, a.ColumnCount);

            for (int i = 0; i < a.RowCount; i++)
            {
                for (int j = 0; j < a.ColumnCount; j++)
                {
                    // For primitives, dynamic is the fastest Unity-safe bridge
                    if (typeof(T).IsPrimitive)
                    {
                        result[i, j] = (T)((dynamic)a[i, j] + (dynamic)b[i, j]);
                    }
                    else
                    {
                        // For Unity types (Vector3), invoke the cached method
                        result[i, j] = (T)a._addMethod.Invoke(null, new object[] { a[i, j], b[i, j] });
                    }
                }
            }

            return result;
        }
        public static Matrix<T> operator +(Matrix<T> a, T[,] b)
        {
            Matrix<T> matrixB = b;

            if (!a.IsAddable)
                throw new InvalidOperationException($"Type {typeof(T).Name} does not support +.");

            if (a.RowCount != matrixB.RowCount || a.ColumnCount != matrixB.ColumnCount)
                throw new ArgumentException("Matrices must have the same dimensions.");

            Matrix<T> result = new Matrix<T>(a.RowCount, a.ColumnCount);

            for (int i = 0; i < a.RowCount; i++)
            {
                for (int j = 0; j < a.ColumnCount; j++)
                {
                    // For primitives, dynamic is the fastest Unity-safe bridge
                    if (typeof(T).IsPrimitive)
                    {
                        result[i, j] = (T)((dynamic)a[i, j] + (dynamic)b[i, j]);
                    }
                    else
                    {
                        // For Unity types (Vector3), invoke the cached method
                        result[i, j] = (T)a._addMethod.Invoke(null, new object[] { a[i, j], b[i, j] });
                    }
                }
            }

            return result;
        }
        public static Matrix<T> operator +(T[,] a, Matrix<T> b)
        {
            Matrix<T> matrixA = a;

            if (!matrixA.IsAddable)
                throw new InvalidOperationException($"Type {typeof(T).Name} does not support +.");

            if (matrixA.RowCount != b.RowCount || matrixA.ColumnCount != b.ColumnCount)
                throw new ArgumentException("Matrices must have the same dimensions.");

            Matrix<T> result = new Matrix<T>(matrixA.RowCount, matrixA.ColumnCount);

            for (int i = 0; i < matrixA.RowCount; i++)
            {
                for (int j = 0; j < matrixA.ColumnCount; j++)
                {
                    // For primitives, dynamic is the fastest Unity-safe bridge
                    if (typeof(T).IsPrimitive)
                    {
                        result[i, j] = (T)((dynamic)a[i, j] + (dynamic)b[i, j]);
                    }
                    else
                    {
                        // For Unity types (Vector3), invoke the cached method
                        result[i, j] = (T)matrixA._addMethod.Invoke(null, new object[] { a[i, j], b[i, j] });
                    }
                }
            }

            return result;
        }

        public static Matrix<T> operator *(Matrix<T> a, T b)
        {
            if (!a.IsMultiplicative)
                throw new InvalidOperationException($"Type {typeof(T).Name} does not support *.");

            Matrix<T> result = new Matrix<T>(a.RowCount, a.ColumnCount);

            for (int i = 0; i < a.RowCount; i++)
            {
                for (int j = 0; j < a.ColumnCount; j++)
                {
                    // For primitives, dynamic is the fastest Unity-safe bridge
                    if (typeof(T).IsPrimitive)
                    {
                        if ((dynamic)a[i, j] * (dynamic)b is not T val)
                            throw new Exception($"Cannot do scalar multiplication");
                        result[i, j] = val;
                    }
                    else
                    {
                        // For Unity types (Vector3), invoke the cached method
                        result[i, j] = (T)a._addMethod.Invoke(null, new object[] { a[i, j], b });
                    }
                }
            }

            return result;
        }
        public static Matrix<T> operator *(Matrix<T> a, Matrix<T> b)
        {
            if (!a.IsAddable)
                throw new InvalidOperationException($"Type {typeof(T).Name} does not support +.");
            if (!a.IsMultiplicative)
                throw new InvalidOperationException($"Type {typeof(T).Name} does not support *.");

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
                    T sum = (T)a._multiplyMethod.Invoke(null, new object[] { a[i, 0], bT[j, 0] });
                    for (int k = 1; k < colsA; k++)
                    {
                        // Accessing A[i,k] and bT[j,k] is sequential in memory
                        sum = (T)a._addMethod.Invoke(null, new object[] { sum, (T)a._addMethod.Invoke(null, new object[] { a[i, k], bT[j, k] }) });
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
    }
}