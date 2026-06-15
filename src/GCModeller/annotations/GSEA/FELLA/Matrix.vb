' ============================================================================
' FELLA Algorithm - VB.NET Implementation
' Matrix.vb - Dense matrix operations and linear algebra
' 
' Implements matrix arithmetic, linear system solving (Gaussian elimination
' with partial pivoting), and matrix inversion required for the FELLA
' diffusion and PageRank computations.
' ============================================================================

Namespace Math

    ''' <summary>
    ''' Dense matrix class with basic linear algebra operations.
    ''' Used for graph Laplacian, diffusion kernel, and PageRank computations.
    ''' All operations use only basic .NET math functions.
    ''' </summary>
    Public Class Matrix
        Private _data As Double(,)
        Private _rows As Integer
        Private _cols As Integer

        ''' <summary>Number of rows</summary>
        Public ReadOnly Property Rows As Integer
            Get
                Return _rows
            End Get
        End Property

        ''' <summary>Number of columns</summary>
        Public ReadOnly Property Cols As Integer
            Get
                Return _cols
            End Get
        End Property

        ''' <summary>Access element at (i, j)</summary>
        Default Public Property Item(i As Integer, j As Integer) As Double
            Get
                Return _data(i, j)
            End Get
            Set(value As Double)
                _data(i, j) = value
            End Set
        End Property

        ''' <summary>Get the underlying 2D array (copy)</summary>
        Public ReadOnly Property Data As Double(,)
            Get
                Dim copy(_rows - 1, _cols - 1) As Double
                Array.Copy(_data, copy, _data.Length)
                Return copy
            End Get
        End Property

        ''' <summary>Create an empty matrix of given dimensions</summary>
        Public Sub New(rows As Integer, cols As Integer)
            If rows <= 0 OrElse cols <= 0 Then
                Throw New ArgumentException("Matrix dimensions must be positive")
            End If
            _rows = rows
            _cols = cols
            _data = New Double(rows - 1, cols - 1) {}
        End Sub

        ''' <summary>Create a matrix from a 2D array</summary>
        Public Sub New(data As Double(,))
            If data Is Nothing Then Throw New ArgumentNullException(NameOf(data))
            _rows = data.GetLength(0)
            _cols = data.GetLength(1)
            _data = New Double(_rows - 1, _cols - 1) {}
            Array.Copy(data, _data, data.Length)
        End Sub

        ''' <summary>Create an identity matrix of size n</summary>
        Public Shared Function Identity(n As Integer) As Matrix
            Dim m As New Matrix(n, n)
            For i = 0 To n - 1
                m(i, i) = 1.0
            Next
            Return m
        End Function

        ''' <summary>Create a diagonal matrix from a vector</summary>
        Public Shared Function Diagonal(diag As Double()) As Matrix
            Dim n As Integer = diag.Length
            Dim m As New Matrix(n, n)
            For i = 0 To n - 1
                m(i, i) = diag(i)
            Next
            Return m
        End Function

        ''' <summary>Create a zero matrix</summary>
        Public Shared Function Zeros(rows As Integer, cols As Integer) As Matrix
            Return New Matrix(rows, cols)
        End Function

        ''' <summary>Clone this matrix</summary>
        Public Function Clone() As Matrix
            Return New Matrix(_data)
        End Function

        ''' <summary>Matrix addition</summary>
        Public Shared Operator +(a As Matrix, b As Matrix) As Matrix
            If a.Rows <> b.Rows OrElse a.Cols <> b.Cols Then
                Throw New ArgumentException("Matrix dimensions must match for addition")
            End If
            Dim result As New Matrix(a.Rows, a.Cols)
            For i = 0 To a.Rows - 1
                For j = 0 To a.Cols - 1
                    result(i, j) = a(i, j) + b(i, j)
                Next
            Next
            Return result
        End Operator

        ''' <summary>Matrix subtraction</summary>
        Public Shared Operator -(a As Matrix, b As Matrix) As Matrix
            If a.Rows <> b.Rows OrElse a.Cols <> b.Cols Then
                Throw New ArgumentException("Matrix dimensions must match for subtraction")
            End If
            Dim result As New Matrix(a.Rows, a.Cols)
            For i = 0 To a.Rows - 1
                For j = 0 To a.Cols - 1
                    result(i, j) = a(i, j) - b(i, j)
                Next
            Next
            Return result
        End Operator

        ''' <summary>Scalar multiplication</summary>
        Public Shared Operator *(m As Matrix, s As Double) As Matrix
            Dim result As New Matrix(m.Rows, m.Cols)
            For i = 0 To m.Rows - 1
                For j = 0 To m.Cols - 1
                    result(i, j) = m(i, j) * s
                Next
            Next
            Return result
        End Operator

        ''' <summary>Scalar multiplication (commutative)</summary>
        Public Shared Operator *(s As Double, m As Matrix) As Matrix
            Return m * s
        End Operator

        ''' <summary>Matrix multiplication</summary>
        Public Shared Operator *(a As Matrix, b As Matrix) As Matrix
            If a.Cols <> b.Rows Then
                Throw New ArgumentException("Inner matrix dimensions must match for multiplication")
            End If
            Dim result As New Matrix(a.Rows, b.Cols)
            For i = 0 To a.Rows - 1
                For j = 0 To b.Cols - 1
                    Dim sum As Double = 0.0
                    For k = 0 To a.Cols - 1
                        sum += a(i, k) * b(k, j)
                    Next
                    result(i, j) = sum
                Next
            Next
            Return result
        End Operator

        ''' <summary>Matrix-vector multiplication: result = M * v</summary>
        Public Function MultiplyVector(v As Double()) As Double()
            If _cols <> v.Length Then
                Throw New ArgumentException("Matrix columns must match vector length")
            End If
            Dim result(_rows - 1) As Double
            For i = 0 To _rows - 1
                Dim sum As Double = 0.0
                For j = 0 To _cols - 1
                    sum += _data(i, j) * v(j)
                Next
                result(i) = sum
            Next
            Return result
        End Function

        ''' <summary>Transpose</summary>
        Public Function Transpose() As Matrix
            Dim result As New Matrix(_cols, _rows)
            For i = 0 To _rows - 1
                For j = 0 To _cols - 1
                    result(j, i) = _data(i, j)
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' Solve linear system Ax = b using Gaussian elimination with partial pivoting.
        ''' Returns solution vector x.
        ''' </summary>
        Public Shared Function Solve(A As Matrix, b As Double()) As Double()
            If A.Rows <> A.Cols Then
                Throw New ArgumentException("Matrix must be square for linear system solving")
            End If
            If A.Rows <> b.Length Then
                Throw New ArgumentException("Matrix and vector dimensions must match")
            End If

            Dim n As Integer = A.Rows
            ' Augmented matrix [A|b]
            Dim aug(n - 1, n) As Double
            For i = 0 To n - 1
                For j = 0 To n - 1
                    aug(i, j) = A(i, j)
                Next
                aug(i, n) = b(i)
            Next

            ' Forward elimination with partial pivoting
            For col = 0 To n - 1
                ' Find pivot row
                Dim maxVal As Double = System.Math.Abs(aug(col, col))
                Dim maxRow As Integer = col
                For row = col + 1 To n - 1
                    Dim val As Double = System.Math.Abs(aug(row, col))
                    If val > maxVal Then
                        maxVal = val
                        maxRow = row
                    End If
                Next

                ' Swap rows
                If maxRow <> col Then
                    For j = col To n
                        Dim temp = aug(col, j)
                        aug(col, j) = aug(maxRow, j)
                        aug(maxRow, j) = temp
                    Next
                End If

                ' Check for singularity
                If System.Math.Abs(aug(col, col)) < 1e-12 Then
                    Throw New InvalidOperationException("Matrix is singular or nearly singular")
                End If

                ' Eliminate below
                For row = col + 1 To n - 1
                    Dim factor As Double = aug(row, col) / aug(col, col)
                    For j = col To n
                        aug(row, j) -= factor * aug(col, j)
                    Next
                Next
            Next

            ' Back substitution
            Dim x(n - 1) As Double
            For i = n - 1 To 0 Step -1
                Dim sum As Double = aug(i, n)
                For j = i + 1 To n - 1
                    sum -= aug(i, j) * x(j)
                Next
                x(i) = sum / aug(i, i)
            Next

            Return x
        End Function

        ''' <summary>
        ''' Solve linear system Ax = b using iterative Gauss-Seidel method.
        ''' More memory-efficient for large sparse-like systems.
        ''' </summary>
        Public Shared Function SolveIterative(A As Matrix, b As Double(),
                                              maxIter As Integer, tolerance As Double) As Double()
            If A.Rows <> A.Cols Then
                Throw New ArgumentException("Matrix must be square")
            End If
            Dim n As Integer = A.Rows
            Dim x(n - 1) As Double
            ' Initialize x to b as starting guess
            Array.Copy(b, x, n)

            For iter = 1 To maxIter
                Dim maxChange As Double = 0.0
                For i = 0 To n - 1
                    Dim sigma As Double = 0.0
                    For j = 0 To n - 1
                        If j <> i Then sigma += A(i, j) * x(j)
                    Next
                    Dim newVal As Double = (b(i) - sigma) / A(i, i)
                    Dim change As Double = System.Math.Abs(newVal - x(i))
                    If change > maxChange Then maxChange = change
                    x(i) = newVal
                Next
                If maxChange < tolerance Then Exit For
            Next

            Return x
        End Function

        ''' <summary>
        ''' Compute matrix inverse using Gaussian elimination.
        ''' Returns A^{-1}.
        ''' </summary>
        Public Function Inverse() As Matrix
            If _rows <> _cols Then
                Throw New ArgumentException("Matrix must be square for inversion")
            End If
            Dim n As Integer = _rows

            ' Augmented matrix [A | I]
            Dim aug(n - 1, 2 * n - 1) As Double
            For i = 0 To n - 1
                For j = 0 To n - 1
                    aug(i, j) = _data(i, j)
                Next
                aug(i, n + i) = 1.0
            Next

            ' Forward elimination with partial pivoting
            For col = 0 To n - 1
                Dim maxVal As Double = System.Math.Abs(aug(col, col))
                Dim maxRow As Integer = col
                For row = col + 1 To n - 1
                    Dim val As Double = System.Math.Abs(aug(row, col))
                    If val > maxVal Then
                        maxVal = val
                        maxRow = row
                    End If
                Next

                If maxRow <> col Then
                    For j = 0 To 2 * n - 1
                        Dim temp = aug(col, j)
                        aug(col, j) = aug(maxRow, j)
                        aug(maxRow, j) = temp
                    Next
                End If

                If System.Math.Abs(aug(col, col)) < 1e-12 Then
                    Throw New InvalidOperationException("Matrix is singular, cannot invert")
                End If

                ' Scale pivot row
                Dim pivot As Double = aug(col, col)
                For j = 0 To 2 * n - 1
                    aug(col, j) /= pivot
                Next

                ' Eliminate all other rows
                For row = 0 To n - 1
                    If row <> col Then
                        Dim factor As Double = aug(row, col)
                        For j = 0 To 2 * n - 1
                            aug(row, j) -= factor * aug(col, j)
                        Next
                    End If
                Next
            Next

            ' Extract inverse from right half
            Dim result As New Matrix(n, n)
            For i = 0 To n - 1
                For j = 0 To n - 1
                    result(i, j) = aug(i, n + j)
                Next
            Next
            Return result
        End Function

        ''' <summary>Get a row as a vector</summary>
        Public Function GetRow(i As Integer) As Double()
            Dim row(_cols - 1) As Double
            For j = 0 To _cols - 1
                row(j) = _data(i, j)
            Next
            Return row
        End Function

        ''' <summary>Get a column as a vector</summary>
        Public Function GetColumn(j As Integer) As Double()
            Dim col(_rows - 1) As Double
            For i = 0 To _rows - 1
                col(i) = _data(i, j)
            Next
            Return col
        End Function

        ''' <summary>Set a row from a vector</summary>
        Public Sub SetRow(i As Integer, values As Double())
            For j = 0 To _cols - 1
                _data(i, j) = values(j)
            Next
        End Sub

        ''' <summary>Set a column from a vector</summary>
        Public Sub SetColumn(j As Integer, values As Double())
            For i = 0 To _rows - 1
                _data(i, j) = values(i)
            Next
        End Sub

        ''' <summary>Compute row sums as a vector</summary>
        Public Function RowSums() As Double()
            Dim sums(_rows - 1) As Double
            For i = 0 To _rows - 1
                Dim s As Double = 0.0
                For j = 0 To _cols - 1
                    s += _data(i, j)
                Next
                sums(i) = s
            Next
            Return sums
        End Function

        ''' <summary>Compute column sums as a vector</summary>
        Public Function ColumnSums() As Double()
            Dim sums(_cols - 1) As Double
            For j = 0 To _cols - 1
                Dim s As Double = 0.0
                For i = 0 To _rows - 1
                    s += _data(i, j)
                Next
                sums(j) = s
            Next
            Return sums
        End Function

        ''' <summary>Element-wise absolute value</summary>
        Public Function Abs() As Matrix
            Dim result As New Matrix(_rows, _cols)
            For i = 0 To _rows - 1
                For j = 0 To _cols - 1
                    result(i, j) = System.Math.Abs(_data(i, j))
                Next
            Next
            Return result
        End Function

        ''' <summary>Frobenius norm</summary>
        Public Function FrobeniusNorm() As Double
            Dim sum As Double = 0.0
            For i = 0 To _rows - 1
                For j = 0 To _cols - 1
                    sum += _data(i, j) * _data(i, j)
                Next
            Next
            Return System.Math.Sqrt(sum)
        End Function

        ''' <summary>
        ''' Create matrix from column vectors (each vector is a column)
        ''' </summary>
        Public Shared Function FromColumns(columns As Double()()) As Matrix
            If columns Is Nothing OrElse columns.Length = 0 Then
                Throw New ArgumentException("Columns cannot be empty")
            End If
            Dim nRows As Integer = columns(0).Length
            Dim nCols As Integer = columns.Length
            Dim m As New Matrix(nRows, nCols)
            For j = 0 To nCols - 1
                For i = 0 To nRows - 1
                    m(i, j) = columns(j)(i)
                Next
            Next
            Return m
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New System.Text.StringBuilder()
            For i = 0 To _rows - 1
                For j = 0 To _cols - 1
                    sb.Append(_data(i, j).ToString("F6"))
                    If j < _cols - 1 Then sb.Append(vbTab)
                Next
                If i < _rows - 1 Then sb.AppendLine()
            Next
            Return sb.ToString()
        End Function

    End Class

End Namespace ' Math
