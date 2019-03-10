﻿#Region "Microsoft.VisualBasic::173ee3b7130f9afbf1bea74965895f9a, Data_science\Mathematica\Math\Math\Algebra\Matrix.NET\GeneralMatrix.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class GeneralMatrix
    ' 
    '         Properties: ArrayCopy, ColumnDimension, ColumnPackedCopy, DiagonalVector, RowDimension
    '                     RowPackedCopy
    ' 
    '         Constructor: (+6 Overloads) Sub New
    ' 
    '         Function: Add, AddEquals, ArrayLeftDivide, ArrayLeftDivideEquals, ArrayMultiply
    '                   ArrayMultiplyEquals, ArrayRightDivide, ArrayRightDivideEquals, chol, Clone
    '                   Condition, Copy, Create, Determinant, Eigen
    '                   (+4 Overloads) GetMatrix, Identity, Inverse, LUD, (+2 Overloads) Multiply
    '                   MultiplyEquals, Norm1, Norm2, NormF, NormInf
    '                   QRD, Rank, RowVectors, Solve, SolveTranspose
    '                   Subtract, SubtractEquals, SVD, ToString, Trace
    '                   Transpose
    ' 
    '         Sub: CheckMatrixDimensions, (+2 Overloads) Dispose, Finalize, ISerializable_GetObjectData, (+4 Overloads) SetMatrix
    ' 
    '         Operators: (+2 Overloads) -, *, +
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Vectorization
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace Matrix

    ''' <summary>
    ''' ### .NET GeneralMatrix class.
    ''' 
    ''' The .NET GeneralMatrix Class provides the fundamental operations of numerical
    ''' linear algebra.  Various constructors create Matrices from two dimensional
    ''' arrays of double precision floating point numbers.  Various "gets" and
    ''' "sets" provide access to submatrices and matrix elements.  Several methods 
    ''' implement basic matrix arithmetic, including matrix addition and
    ''' multiplication, matrix norms, and element-by-element array operations.
    ''' Methods for reading and printing matrices are also included.  All the
    ''' operations in this version of the GeneralMatrix Class involve real matrices.
    ''' Complex matrices may be handled in a future version.
    ''' 
    ''' Five fundamental matrix decompositions, which consist of pairs or triples
    ''' of matrices, permutation vectors, and the like, produce results in five
    ''' decomposition classes.  These decompositions are accessed by the GeneralMatrix
    ''' class to compute solutions of simultaneous linear equations, determinants,
    ''' inverses and other matrix functions.  
    ''' 
    ''' The five decompositions are:
    ''' 
    ''' + Cholesky Decomposition of symmetric, positive definite matrices.
    ''' + LU Decomposition of rectangular matrices.
    ''' + QR Decomposition of rectangular matrices.
    ''' + Singular Value Decomposition of rectangular matrices.
    ''' + Eigenvalue Decomposition of both symmetric and nonsymmetric square matrices.
    ''' 
    ''' Example of use:
    ''' 
    ''' Solve a linear system A x = b and compute the residual norm, ||b - A x||.
    ''' 
    ''' ```csharp
    ''' double[][] vals;
    ''' GeneralMatrix A = new GeneralMatrix(vals);
    ''' GeneralMatrix b = GeneralMatrix.Random(3,1);
    ''' GeneralMatrix x = A.Solve(b);
    ''' GeneralMatrix r = A.Multiply(x).Subtract(b);
    ''' double rnorm = r.NormInf();
    ''' ```
    ''' </summary>
    ''' <author>  
    ''' The MathWorks, Inc. and the National Institute of Standards and Technology.
    ''' 
    ''' http://www.codeproject.com/Articles/5835/DotNetMatrix-Simple-Matrix-Library-for-NET
    ''' https://github.com/fiji/Jama/blob/master/src/main/java/Jama/Matrix.java
    ''' </author>
    ''' <version>  5 August 1998
    ''' </version>
    ''' <remarks>
    ''' Access the internal two-dimensional array.
    ''' Pointer to the two-dimensional array of matrix elements.
    ''' </remarks>
    <Serializable>
    Public Class GeneralMatrix : Inherits Vector(Of Double())
        Implements ICloneable
        Implements ISerializable
        Implements IDisposable

#Region "Class variables"

        ''' <summary>Row and column dimensions.
        ''' @serial row dimension.
        ''' @serial column dimension.
        ''' </summary>
        Dim m As Integer, n As Integer

#End Region

#Region "Constructors"

        ''' <summary>Construct an m-by-n matrix of zeros. </summary>
        ''' <param name="m">   Number of rows.
        ''' </param>
        ''' <param name="n">   Number of colums.
        ''' </param>

        Public Sub New(m As Integer, n As Integer)
            Me.m = m
            Me.n = n
            Dim A = New Double(m - 1)() {}

            For i As Integer = 0 To m - 1
                A(i) = New Double(n - 1) {}
            Next

            buffer = A
        End Sub

        ''' <summary>Construct an m-by-n constant matrix.</summary>
        ''' <param name="m">   Number of rows.
        ''' </param>
        ''' <param name="n">   Number of colums.
        ''' </param>
        ''' <param name="s">   Fill the matrix with this scalar value.
        ''' </param>

        Public Sub New(m As Integer, n As Integer, s As Double)
            Me.m = m
            Me.n = n
            Dim A = New Double(m - 1)() {}

            For i As Integer = 0 To m - 1
                A(i) = New Double(n - 1) {}
            Next
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    A(i)(j) = s
                Next
            Next

            buffer = A
        End Sub

        ''' <summary>Construct a matrix from a 2-D array.</summary>
        ''' <param name="A">   Two-dimensional array of doubles.
        ''' </param>
        ''' <exception cref="System.ArgumentException">   All rows must have the same length
        ''' </exception>
        ''' <seealso cref="Create">
        ''' </seealso>

        Public Sub New(A As Double()())
            m = A.Length
            n = A(0).Length
            For i As Integer = 0 To m - 1
                If A(i).Length <> n Then
                    Throw New ArgumentException("All rows must have the same length.")
                End If
            Next
            Me.buffer = A
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(rows As IEnumerable(Of Vector))
            Call Me.New(rows.Select(Function(v) v.ToArray).ToArray)
        End Sub

        ''' <summary>Construct a matrix quickly without checking arguments.</summary>
        ''' <param name="A">   Two-dimensional array of doubles.
        ''' </param>
        ''' <param name="m">   Number of rows.
        ''' </param>
        ''' <param name="n">   Number of colums.
        ''' </param>

        Public Sub New(A As Double()(), m As Integer, n As Integer)
            Me.buffer = A
            Me.m = m
            Me.n = n
        End Sub

        ''' <summary>Construct a matrix from a one-dimensional packed array</summary>
        ''' <param name="vals">One-dimensional array of doubles, packed by columns (ala Fortran).
        ''' </param>
        ''' <param name="m">   Number of rows.
        ''' </param>
        ''' <exception cref="System.ArgumentException">   Array length must be a multiple of m.
        ''' </exception>

        Public Sub New(vals As Double(), m As Integer)
            Me.m = m
            n = (If(m <> 0, vals.Length \ m, 0))
            If m * n <> vals.Length Then
                Throw New System.ArgumentException("Array length must be a multiple of m.")
            End If
            Dim A = New Double(m - 1)() {}
            For i As Integer = 0 To m - 1
                A(i) = New Double(n - 1) {}
            Next
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    A(i)(j) = vals(i + j * m)
                Next
            Next

            buffer = A
        End Sub
#End Region


#Region "Public Properties"

        ''' <summary>Copy the internal two-dimensional array.</summary>
        ''' <returns>     Two-dimensional array copy of matrix elements.
        ''' </returns>
        Public Overridable ReadOnly Property ArrayCopy() As Double()()
            Get
                Dim C As Double()() = New Double(m - 1)() {}
                For i As Integer = 0 To m - 1
                    C(i) = New Double(n - 1) {}
                Next
                For i As Integer = 0 To m - 1
                    For j As Integer = 0 To n - 1
                        C(i)(j) = buffer(i)(j)
                    Next
                Next
                Return C
            End Get
        End Property

        ''' <summary>Make a one-dimensional column packed copy of the internal array.</summary>
        ''' <returns>     Matrix elements packed in a one-dimensional array by columns.
        ''' </returns>
        Public Overridable ReadOnly Property ColumnPackedCopy() As Double()
            Get
                Dim vals As Double() = New Double(m * n - 1) {}
                For i As Integer = 0 To m - 1
                    For j As Integer = 0 To n - 1
                        vals(i + j * m) = buffer(i)(j)
                    Next
                Next
                Return vals
            End Get
        End Property


        ''' <summary>Make a one-dimensional row packed copy of the internal array.</summary>
        ''' <returns>     Matrix elements packed in a one-dimensional array by rows.
        ''' </returns>
        Public Overridable ReadOnly Property RowPackedCopy() As Double()
            Get
                Dim vals As Double() = New Double(m * n - 1) {}
                For i As Integer = 0 To m - 1
                    For j As Integer = 0 To n - 1
                        vals(i * n + j) = buffer(i)(j)
                    Next
                Next
                Return vals
            End Get
        End Property

        ''' <summary>Get row dimension.</summary>
        ''' <returns>     m, the number of rows.
        ''' </returns>
        Public Overridable ReadOnly Property RowDimension() As Integer
            Get
                Return m
            End Get
        End Property

        ''' <summary>Get column dimension.</summary>
        ''' <returns>     n, the number of columns.
        ''' </returns>
        Public Overridable ReadOnly Property ColumnDimension() As Integer
            Get
                Return n
            End Get
        End Property

        Public ReadOnly Property DiagonalVector As Vector
            Get
                Dim v As New List(Of Double)

                For i As Integer = 0 To m - 1
                    v += buffer(i)(i)
                Next

                Return New Vector(v)
            End Get
        End Property
#End Region

#Region "Public Methods"

        ''' <summary>Construct a matrix from a copy of a 2-D array.</summary>
        ''' <param name="A">   Two-dimensional array of doubles.
        ''' </param>
        ''' <exception cref="System.ArgumentException">   All rows must have the same length
        ''' </exception>

        Public Shared Function Create(A As Double()()) As GeneralMatrix
            Dim m As Integer = A.Length
            Dim n As Integer = A(0).Length
            Dim X As New GeneralMatrix(m, n)
            Dim C As Double()() = X.Array
            For i As Integer = 0 To m - 1
                If A(i).Length <> n Then
                    Throw New System.ArgumentException("All rows must have the same length.")
                End If
                For j As Integer = 0 To n - 1
                    C(i)(j) = A(i)(j)
                Next
            Next
            Return X
        End Function

        ''' <summary>Make a deep copy of a matrix</summary>

        Public Overridable Function Copy() As GeneralMatrix
            Dim X As New GeneralMatrix(m, n)
            Dim C As Double()() = X.Array
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    C(i)(j) = buffer(i)(j)
                Next
            Next
            Return X
        End Function

        ''' <summary>Get a single element.</summary>
        ''' <param name="i">   Row index.
        ''' </param>
        ''' <param name="j">   Column index.
        ''' </param>
        ''' <returns>     A(i,j)
        ''' </returns>
        ''' <exception cref="System.IndexOutOfRangeException">  
        ''' </exception>

        Default Public Shadows Property Value(i%, j%) As Double
            Get
                Return buffer(i)(j)
            End Get
            Set(value As Double)
                buffer(i)(j) = value
            End Set
        End Property

        Default Public Shadows ReadOnly Property Value(indices As IEnumerable(Of Integer)) As GeneralMatrix
            Get
                Dim index%() = indices.ToArray
                Dim subMAT = buffer _
                    .Select(Function(x) x.Takes(index)) _
                    .ToArray

                Return New GeneralMatrix(subMAT)
            End Get
        End Property

        ''' <summary>Get a submatrix.</summary>
        ''' <param name="i0">  Initial row index
        ''' </param>
        ''' <param name="i1">  Final row index
        ''' </param>
        ''' <param name="j0">  Initial column index
        ''' </param>
        ''' <param name="j1">  Final column index
        ''' </param>
        ''' <returns>     A(i0:i1,j0:j1)
        ''' </returns>
        ''' <exception cref="System.IndexOutOfRangeException">   Submatrix indices
        ''' </exception>

        Public Overridable Function GetMatrix(i0 As Integer, i1 As Integer, j0 As Integer, j1 As Integer) As GeneralMatrix
            Dim X As New GeneralMatrix(i1 - i0 + 1, j1 - j0 + 1)
            Dim B As Double()() = X.Array
            Try
                For i As Integer = i0 To i1
                    For j As Integer = j0 To j1
                        B(i - i0)(j - j0) = buffer(i)(j)
                    Next
                Next
            Catch e As System.IndexOutOfRangeException
                Throw New System.IndexOutOfRangeException("Submatrix indices", e)
            End Try
            Return X
        End Function

        ''' <summary>Get a submatrix.</summary>
        ''' <param name="r">   Array of row indices.
        ''' </param>
        ''' <param name="c">   Array of column indices.
        ''' </param>
        ''' <returns>     A(r(:),c(:))
        ''' </returns>
        ''' <exception cref="System.IndexOutOfRangeException">   Submatrix indices
        ''' </exception>

        Public Overridable Function GetMatrix(r As Integer(), c As Integer()) As GeneralMatrix
            Dim X As New GeneralMatrix(r.Length, c.Length)
            Dim B As Double()() = X.Array
            Try
                For i As Integer = 0 To r.Length - 1
                    For j As Integer = 0 To c.Length - 1
                        B(i)(j) = buffer(r(i))(c(j))
                    Next
                Next
            Catch e As System.IndexOutOfRangeException
                Throw New System.IndexOutOfRangeException("Submatrix indices", e)
            End Try
            Return X
        End Function

        ''' <summary>Get a submatrix.</summary>
        ''' <param name="i0">  Initial row index
        ''' </param>
        ''' <param name="i1">  Final row index
        ''' </param>
        ''' <param name="c">   Array of column indices.
        ''' </param>
        ''' <returns>     A(i0:i1,c(:))
        ''' </returns>
        ''' <exception cref="System.IndexOutOfRangeException">   Submatrix indices
        ''' </exception>

        Public Overridable Function GetMatrix(i0 As Integer, i1 As Integer, c As Integer()) As GeneralMatrix
            Dim X As New GeneralMatrix(i1 - i0 + 1, c.Length)
            Dim B As Double()() = X.Array
            Try
                For i As Integer = i0 To i1
                    For j As Integer = 0 To c.Length - 1
                        B(i - i0)(j) = buffer(i)(c(j))
                    Next
                Next
            Catch e As System.IndexOutOfRangeException
                Throw New System.IndexOutOfRangeException("Submatrix indices", e)
            End Try
            Return X
        End Function

        ''' <summary>Get a submatrix.</summary>
        ''' <param name="r">   Array of row indices.
        ''' </param>
        ''' <param name="j0">  Initial column index
        ''' </param>
        ''' <param name="j1">  Final column index
        ''' </param>
        ''' <returns>     A(r(:),j0:j1)
        ''' </returns>
        ''' <exception cref="System.IndexOutOfRangeException">   Submatrix indices
        ''' </exception>

        Public Overridable Function GetMatrix(r As Integer(), j0 As Integer, j1 As Integer) As GeneralMatrix
            Dim X As New GeneralMatrix(r.Length, j1 - j0 + 1)
            Dim B As Double()() = X.Array
            Try
                For i As Integer = 0 To r.Length - 1
                    For j As Integer = j0 To j1
                        B(i)(j - j0) = buffer(r(i))(j)
                    Next
                Next
            Catch e As System.IndexOutOfRangeException
                Throw New System.IndexOutOfRangeException("Submatrix indices", e)
            End Try
            Return X
        End Function

        ''' <summary>Set a submatrix.</summary>
        ''' <param name="i0">  Initial row index
        ''' </param>
        ''' <param name="i1">  Final row index
        ''' </param>
        ''' <param name="j0">  Initial column index
        ''' </param>
        ''' <param name="j1">  Final column index
        ''' </param>
        ''' <param name="X">   A(i0:i1,j0:j1)
        ''' </param>
        ''' <exception cref="System.IndexOutOfRangeException">  Submatrix indices
        ''' </exception>

        Public Overridable Sub SetMatrix(i0 As Integer, i1 As Integer, j0 As Integer, j1 As Integer, X As GeneralMatrix)
            Try
                For i As Integer = i0 To i1
                    For j As Integer = j0 To j1
                        buffer(i)(j) = X(i - i0, j - j0)
                    Next
                Next
            Catch e As System.IndexOutOfRangeException
                Throw New System.IndexOutOfRangeException("Submatrix indices", e)
            End Try
        End Sub

        ''' <summary>Set a submatrix.</summary>
        ''' <param name="r">   Array of row indices.
        ''' </param>
        ''' <param name="c">   Array of column indices.
        ''' </param>
        ''' <param name="X">   A(r(:),c(:))
        ''' </param>
        ''' <exception cref="System.IndexOutOfRangeException">  Submatrix indices
        ''' </exception>

        Public Overridable Sub SetMatrix(r As Integer(), c As Integer(), X As GeneralMatrix)
            Try
                For i As Integer = 0 To r.Length - 1
                    For j As Integer = 0 To c.Length - 1
                        buffer(r(i))(c(j)) = X(i, j)
                    Next
                Next
            Catch e As System.IndexOutOfRangeException
                Throw New System.IndexOutOfRangeException("Submatrix indices", e)
            End Try
        End Sub

        ''' <summary>Set a submatrix.</summary>
        ''' <param name="r">   Array of row indices.
        ''' </param>
        ''' <param name="j0">  Initial column index
        ''' </param>
        ''' <param name="j1">  Final column index
        ''' </param>
        ''' <param name="X">   A(r(:),j0:j1)
        ''' </param>
        ''' <exception cref="System.IndexOutOfRangeException"> Submatrix indices
        ''' </exception>

        Public Overridable Sub SetMatrix(r As Integer(), j0 As Integer, j1 As Integer, X As GeneralMatrix)
            Try
                For i As Integer = 0 To r.Length - 1
                    For j As Integer = j0 To j1
                        buffer(r(i))(j) = X(i, j - j0)
                    Next
                Next
            Catch e As System.IndexOutOfRangeException
                Throw New System.IndexOutOfRangeException("Submatrix indices", e)
            End Try
        End Sub

        ''' <summary>Set a submatrix.</summary>
        ''' <param name="i0">  Initial row index
        ''' </param>
        ''' <param name="i1">  Final row index
        ''' </param>
        ''' <param name="c">   Array of column indices.
        ''' </param>
        ''' <param name="X">   A(i0:i1,c(:))
        ''' </param>
        ''' <exception cref="System.IndexOutOfRangeException">  Submatrix indices
        ''' </exception>

        Public Overridable Sub SetMatrix(i0 As Integer, i1 As Integer, c As Integer(), X As GeneralMatrix)
            Try
                For i As Integer = i0 To i1
                    For j As Integer = 0 To c.Length - 1
                        buffer(i)(c(j)) = X(i - i0, j)
                    Next
                Next
            Catch e As System.IndexOutOfRangeException
                Throw New System.IndexOutOfRangeException("Submatrix indices", e)
            End Try
        End Sub

        ''' <summary>Matrix transpose.</summary>
        ''' <returns>    A'
        ''' </returns>

        Public Overridable Function Transpose() As GeneralMatrix
            Dim X As New GeneralMatrix(n, m)
            Dim C As Double()() = X.Array
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    C(j)(i) = buffer(i)(j)
                Next
            Next
            Return X
        End Function

        ''' <summary>One norm</summary>
        ''' <returns>    maximum column sum.
        ''' </returns>

        Public Overridable Function Norm1() As Double
            Dim f As Double = 0
            For j As Integer = 0 To n - 1
                Dim s As Double = 0
                For i As Integer = 0 To m - 1
                    s += System.Math.Abs(buffer(i)(j))
                Next
                f = System.Math.Max(f, s)
            Next
            Return f
        End Function

        ''' <summary>Two norm</summary>
        ''' <returns>    maximum singular value.
        ''' </returns>

        Public Overridable Function Norm2() As Double
            Return (New SingularValueDecomposition(Me).Norm2())
        End Function

        ''' <summary>Infinity norm</summary>
        ''' <returns>    maximum row sum.
        ''' </returns>

        Public Overridable Function NormInf() As Double
            Dim f As Double = 0
            For i As Integer = 0 To m - 1
                Dim s As Double = 0
                For j As Integer = 0 To n - 1
                    s += System.Math.Abs(buffer(i)(j))
                Next
                f = System.Math.Max(f, s)
            Next
            Return f
        End Function

        ''' <summary>Frobenius norm</summary>
        ''' <returns>    sqrt of sum of squares of all elements.
        ''' </returns>

        Public Overridable Function NormF() As Double
            Dim f As Double = 0
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    f = Hypot(f, buffer(i)(j))
                Next
            Next
            Return f
        End Function

        ''' <summary>Unary minus</summary>
        ''' <returns>    -A
        ''' </returns>
        Public Shared Operator -(m As GeneralMatrix) As GeneralMatrix
            Dim X As New GeneralMatrix(m.m, m.n)
            Dim C As Double()() = X.Array
            For i As Integer = 0 To m.m - 1
                For j As Integer = 0 To m.n - 1
                    C(i)(j) = -m.buffer(i)(j)
                Next
            Next
            Return X
        End Operator

        ''' <summary>C = A + B</summary>
        ''' <param name="B">   another matrix
        ''' </param>
        ''' <returns>     A + B
        ''' </returns>

        Public Overridable Function Add(B As GeneralMatrix) As GeneralMatrix
            CheckMatrixDimensions(B)
            Dim X As New GeneralMatrix(m, n)
            Dim C As Double()() = X.Array
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    C(i)(j) = buffer(i)(j) + B.buffer(i)(j)
                Next
            Next
            Return X
        End Function

        ''' <summary>A = A + B</summary>
        ''' <param name="B">   another matrix
        ''' </param>
        ''' <returns>     A + B
        ''' </returns>

        Public Overridable Function AddEquals(B As GeneralMatrix) As GeneralMatrix
            CheckMatrixDimensions(B)
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    buffer(i)(j) = buffer(i)(j) + B.buffer(i)(j)
                Next
            Next
            Return Me
        End Function

        ''' <summary>C = A - B</summary>
        ''' <param name="B">   another matrix
        ''' </param>
        ''' <returns>     A - B
        ''' </returns>

        Public Overridable Function Subtract(B As GeneralMatrix) As GeneralMatrix
            CheckMatrixDimensions(B)
            Dim X As New GeneralMatrix(m, n)
            Dim C As Double()() = X.Array
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    C(i)(j) = buffer(i)(j) - B.buffer(i)(j)
                Next
            Next
            Return X
        End Function

        ''' <summary>A = A - B</summary>
        ''' <param name="B">   another matrix
        ''' </param>
        ''' <returns>     A - B
        ''' </returns>

        Public Overridable Function SubtractEquals(B As GeneralMatrix) As GeneralMatrix
            CheckMatrixDimensions(B)
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    buffer(i)(j) = buffer(i)(j) - B.buffer(i)(j)
                Next
            Next
            Return Me
        End Function

        ''' <summary>Element-by-element multiplication, C = A.*B</summary>
        ''' <param name="B">   another matrix
        ''' </param>
        ''' <returns>     A.*B
        ''' </returns>

        Public Overridable Function ArrayMultiply(B As GeneralMatrix) As GeneralMatrix
            CheckMatrixDimensions(B)
            Dim X As New GeneralMatrix(m, n)
            Dim C As Double()() = X.Array
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    C(i)(j) = buffer(i)(j) * B.buffer(i)(j)
                Next
            Next
            Return X
        End Function

        ''' <summary>Element-by-element multiplication in place, A = A.*B</summary>
        ''' <param name="B">   another matrix
        ''' </param>
        ''' <returns>     A.*B
        ''' </returns>

        Public Overridable Function ArrayMultiplyEquals(B As GeneralMatrix) As GeneralMatrix
            CheckMatrixDimensions(B)
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    buffer(i)(j) = buffer(i)(j) * B.buffer(i)(j)
                Next
            Next
            Return Me
        End Function

        ''' <summary>Element-by-element right division, C = A./B</summary>
        ''' <param name="B">   another matrix
        ''' </param>
        ''' <returns>     A./B
        ''' </returns>

        Public Overridable Function ArrayRightDivide(B As GeneralMatrix) As GeneralMatrix
            CheckMatrixDimensions(B)
            Dim X As New GeneralMatrix(m, n)
            Dim C As Double()() = X.Array
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    C(i)(j) = buffer(i)(j) / B.buffer(i)(j)
                Next
            Next
            Return X
        End Function

        ''' <summary>Element-by-element right division in place, A = A./B</summary>
        ''' <param name="B">   another matrix
        ''' </param>
        ''' <returns>     A./B
        ''' </returns>

        Public Overridable Function ArrayRightDivideEquals(B As GeneralMatrix) As GeneralMatrix
            CheckMatrixDimensions(B)
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    buffer(i)(j) = buffer(i)(j) / B.buffer(i)(j)
                Next
            Next
            Return Me
        End Function

        ''' <summary>Element-by-element left division, C = A.\B</summary>
        ''' <param name="B">   another matrix
        ''' </param>
        ''' <returns>     A.\B
        ''' </returns>

        Public Overridable Function ArrayLeftDivide(B As GeneralMatrix) As GeneralMatrix
            CheckMatrixDimensions(B)
            Dim X As New GeneralMatrix(m, n)
            Dim C As Double()() = X.Array
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    C(i)(j) = B.buffer(i)(j) / buffer(i)(j)
                Next
            Next
            Return X
        End Function

        ''' <summary>Element-by-element left division in place, A = A.\B</summary>
        ''' <param name="B">   another matrix
        ''' </param>
        ''' <returns>     A.\B
        ''' </returns>

        Public Overridable Function ArrayLeftDivideEquals(B As GeneralMatrix) As GeneralMatrix
            CheckMatrixDimensions(B)
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    buffer(i)(j) = B.buffer(i)(j) / buffer(i)(j)
                Next
            Next
            Return Me
        End Function

        ''' <summary>Multiply a matrix by a scalar, C = s*A</summary>
        ''' <param name="s">   scalar
        ''' </param>
        ''' <returns>     s*A
        ''' </returns>

        Public Overridable Function Multiply(s As Double) As GeneralMatrix
            Dim X As New GeneralMatrix(m, n)
            Dim C As Double()() = X.Array
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    C(i)(j) = s * buffer(i)(j)
                Next
            Next
            Return X
        End Function

        ''' <summary>Multiply a matrix by a scalar in place, A = s*A</summary>
        ''' <param name="s">   scalar
        ''' </param>
        ''' <returns>     replace A by s*A
        ''' </returns>

        Public Overridable Function MultiplyEquals(s As Double) As GeneralMatrix
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    buffer(i)(j) = s * buffer(i)(j)
                Next
            Next
            Return Me
        End Function

        ''' <summary>
        ''' Linear algebraic matrix multiplication, ``A * B``
        ''' 
        ''' ``Jama.Matrix.times``
        ''' </summary>
        ''' <param name="B">another matrix</param>
        ''' <returns>Matrix product, A * B</returns>
        ''' <exception cref="System.ArgumentException">Matrix inner dimensions must agree.
        ''' </exception>
        Public Overridable Function Multiply(B As GeneralMatrix) As GeneralMatrix
            If B.m <> n Then
                Throw New System.ArgumentException("GeneralMatrix inner dimensions must agree.")
            End If
            Dim X As New GeneralMatrix(m, B.n)
            Dim C As Double()() = X.Array
            Dim Bcolj As Double() = New Double(n - 1) {}
            For j As Integer = 0 To B.n - 1
                For k As Integer = 0 To n - 1
                    Bcolj(k) = B.buffer(k)(j)
                Next
                For i As Integer = 0 To m - 1
                    Dim Arowi As Double() = buffer(i)
                    Dim s As Double = 0
                    For k As Integer = 0 To n - 1
                        s += Arowi(k) * Bcolj(k)
                    Next
                    C(i)(j) = s
                Next
            Next
            Return X
        End Function

#Region "Operator Overloading"

        ''' <summary>
        '''  Addition of matrices
        ''' </summary>
        ''' <param name="m1"></param>
        ''' <param name="m2"></param>
        ''' <returns></returns>
        Public Shared Operator +(m1 As GeneralMatrix, m2 As GeneralMatrix) As GeneralMatrix
            Return m1.Add(m2)
        End Operator

        ''' <summary>
        ''' Subtraction of matrices
        ''' </summary>
        ''' <param name="m1"></param>
        ''' <param name="m2"></param>
        ''' <returns></returns>
        Public Shared Operator -(m1 As GeneralMatrix, m2 As GeneralMatrix) As GeneralMatrix
            Return m1.Subtract(m2)
        End Operator

        ''' <summary>
        ''' Multiplication of matrices
        ''' </summary>
        ''' <param name="m1"></param>
        ''' <param name="m2"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator *(m1 As GeneralMatrix, m2 As GeneralMatrix) As GeneralMatrix
            Return m1.Multiply(m2)
        End Operator

#End Region

        ''' <summary>LU Decomposition</summary>
        ''' <returns>     LUDecomposition
        ''' </returns>
        ''' <seealso cref="LUDecomposition">
        ''' </seealso>

        Public Overridable Function LUD() As LUDecomposition
            Return New LUDecomposition(Me)
        End Function

        ''' <summary>QR Decomposition</summary>
        ''' <returns>     QRDecomposition
        ''' </returns>
        ''' <seealso cref="QRDecomposition">
        ''' </seealso>

        Public Overridable Function QRD() As QRDecomposition
            Return New QRDecomposition(Me)
        End Function

        ''' <summary>Cholesky Decomposition</summary>
        ''' <returns>     CholeskyDecomposition
        ''' </returns>
        ''' <seealso cref="CholeskyDecomposition">
        ''' </seealso>

        Public Overridable Function chol() As CholeskyDecomposition
            Return New CholeskyDecomposition(Me)
        End Function

        ''' <summary>Singular Value Decomposition</summary>
        ''' <returns>     SingularValueDecomposition
        ''' </returns>
        ''' <seealso cref="SingularValueDecomposition">
        ''' </seealso>

        Public Overridable Function SVD() As SingularValueDecomposition
            Return New SingularValueDecomposition(Me)
        End Function

        ''' <summary>Eigenvalue Decomposition</summary>
        ''' <returns>     EigenvalueDecomposition
        ''' </returns>
        ''' <seealso cref="EigenvalueDecomposition">
        ''' </seealso>

        Public Overridable Function Eigen() As EigenvalueDecomposition
            Return New EigenvalueDecomposition(Me)
        End Function

        ''' <summary>Solve A*X = B</summary>
        ''' <param name="B">   right hand side
        ''' </param>
        ''' <returns>     solution if A is square, least squares solution otherwise
        ''' </returns>

        Public Overridable Function Solve(B As GeneralMatrix) As GeneralMatrix
            Return (If(m = n, (New LUDecomposition(Me)).Solve(B), (New QRDecomposition(Me)).Solve(B)))
        End Function

        ''' <summary>Solve X*A = B, which is also A'*X' = B'</summary>
        ''' <param name="B">   right hand side
        ''' </param>
        ''' <returns>     solution if A is square, least squares solution otherwise.
        ''' </returns>

        Public Overridable Function SolveTranspose(B As GeneralMatrix) As GeneralMatrix
            Return Transpose().Solve(B.Transpose())
        End Function

        ''' <summary>Matrix inverse or pseudoinverse</summary>
        ''' <returns>     inverse(A) if A is square, pseudoinverse otherwise.
        ''' </returns>

        Public Overridable Function Inverse() As GeneralMatrix
            Return Solve(Identity(m, m))
        End Function

        ''' <summary>GeneralMatrix determinant</summary>
        ''' <returns>     determinant
        ''' </returns>

        Public Overridable Function Determinant() As Double
            Return New LUDecomposition(Me).Determinant()
        End Function

        ''' <summary>GeneralMatrix rank</summary>
        ''' <returns>     effective numerical rank, obtained from SVD.
        ''' </returns>

        Public Overridable Function Rank() As Integer
            Return New SingularValueDecomposition(Me).Rank()
        End Function

        ''' <summary>Matrix condition (2 norm)</summary>
        ''' <returns>     ratio of largest to smallest singular value.
        ''' </returns>

        Public Overridable Function Condition() As Double
            Return New SingularValueDecomposition(Me).Condition()
        End Function

        ''' <summary>Matrix trace.</summary>
        ''' <returns>     sum of the diagonal elements.
        ''' </returns>

        Public Overridable Function Trace() As Double
            Dim t As Double = 0
            For i As Integer = 0 To System.Math.Min(m, n) - 1
                t += buffer(i)(i)
            Next
            Return t
        End Function

        ''' <summary>Generate identity matrix</summary>
        ''' <param name="m">   Number of rows.
        ''' </param>
        ''' <param name="n">   Number of colums.
        ''' </param>
        ''' <returns>     An m-by-n matrix with ones on the diagonal and zeros elsewhere.
        ''' </returns>

        Public Shared Function Identity(m As Integer, n As Integer) As GeneralMatrix
            Dim A As New GeneralMatrix(m, n)
            Dim X As Double()() = A.Array
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    X(i)(j) = (If(i = j, 1.0, 0.0))
                Next
            Next
            Return A
        End Function

#End Region

#Region "Private Methods"

        ''' <summary>Check if size(A) == size(B) *</summary>

        Private Sub CheckMatrixDimensions(B As GeneralMatrix)
            If B.m <> m OrElse B.n <> n Then
                Throw New System.ArgumentException("GeneralMatrix dimensions must agree.")
            End If
        End Sub
#End Region

#Region "Implement IDisposable"
        ''' <summary>
        ''' Do not make this method virtual.
        ''' A derived class should not be able to override this method.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub

        ''' <summary>
        ''' Dispose(bool disposing) executes in two distinct scenarios.
        ''' If disposing equals true, the method has been called directly
        ''' or indirectly by a user's code. Managed and unmanaged resources
        ''' can be disposed.
        ''' If disposing equals false, the method has been called by the 
        ''' runtime from inside the finalizer and you should not reference 
        ''' other objects. Only unmanaged resources can be disposed.
        ''' </summary>
        ''' <param name="disposing"></param>
        Private Sub Dispose(disposing As Boolean)
            ' This object will be cleaned up by the Dispose method.
            ' Therefore, you should call GC.SupressFinalize to
            ' take this object off the finalization queue 
            ' and prevent finalization code for this object
            ' from executing a second time.
            If disposing Then
                GC.SuppressFinalize(Me)
            End If
        End Sub

        ''' <summary>
        ''' This destructor will run only if the Dispose method 
        ''' does not get called.
        ''' It gives your base class the opportunity to finalize.
        ''' Do not provide destructors in types derived from this class.
        ''' </summary>
        Protected Overrides Sub Finalize()
            Try
                ' Do not re-create Dispose clean-up code here.
                ' Calling Dispose(false) is optimal in terms of
                ' readability and maintainability.
                Dispose(False)
            Finally
                MyBase.Finalize()
            End Try
        End Sub
#End Region

        ''' <summary>Clone the GeneralMatrix object.</summary>
        Public Function Clone() As System.Object Implements ICloneable.Clone
            Return Me.Copy()
        End Function

        Public Overrides Function ToString() As String
            Return $"[Row:{RowDimension}, Column:{ColumnDimension}]"
        End Function

        ''' <summary>
        ''' A method called when serializing this class
        ''' </summary>
        ''' <param name="info"></param>
        ''' <param name="context"></param>
        Private Sub ISerializable_GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
        End Sub

        Public Shared Widening Operator CType(data#(,)) As GeneralMatrix
            Return New GeneralMatrix(data.RowIterator.ToArray)
        End Operator

        Public Shared Widening Operator CType(data#()()) As GeneralMatrix
            Return New GeneralMatrix(data)
        End Operator

        Public Iterator Function RowVectors() As IEnumerable(Of Vector)
            For Each row As Double() In buffer
                Yield row.AsVector
            Next
        End Function
    End Class
End Namespace
