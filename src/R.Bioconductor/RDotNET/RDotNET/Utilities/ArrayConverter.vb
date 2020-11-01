#Region "Microsoft.VisualBasic::254a48b82b7e6f06cc5cdc7499687daa, RDotNET\RDotNET\Utilities\ArrayConverter.vb"

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

    ' 	Class ArrayConverter
    ' 
    ' 	    Constructor: (+1 Overloads) Sub New
    ' 	    Function: (+2 Overloads) ArrayConvertAll, ArrayConvertAllOneDim, (+2 Overloads) ArrayConvertAllTwoDim, ArrayConvertOneDim, CreateMatrixJagged
    '                Prepend, Subset
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Namespace Utilities
	''' <summary>
	''' A set of helpers to convert, query and transform arrays
	''' </summary>
	Public NotInheritable Class ArrayConverter
		Private Sub New()
		End Sub
		''' <summary>
		''' Convert all elements of a rectangular array, using a function to cast/transform each element.
		''' </summary>
		''' <typeparam name="T">The type of the input array elements</typeparam>
		''' <typeparam name="U">The type of the output array elements</typeparam>
		''' <param name="array">Input array</param>
		''' <param name="fun">A conversion function taking in an object of type T and returning one of type U</param>
		''' <returns></returns>
		Public Shared Function ArrayConvertAll(Of T, U)(array As T(,), fun As Func(Of T, U)) As U(,)
			Dim rows As Integer = array.GetLength(0)
			Dim cols As Integer = array.GetLength(1)
			Dim res As U(,) = New U(rows - 1, cols - 1) {}
			For i As Integer = 0 To rows - 1
				' TODO: what is the best indexing order to avoid memory cache misses, rowfirst or colfirst?
				For j As Integer = 0 To cols - 1
					res(i, j) = fun(array(i, j))
				Next
			Next
			Return res
		End Function

		''' <summary>
		''' Convert all elements of a rectangular, jagged array, using a function to cast/transform each element.
		''' </summary>
		''' <typeparam name="T">The type of the input array elements</typeparam>
		''' <typeparam name="U">The type of the output array elements</typeparam>
		''' <param name="array">Input array</param>
		''' <param name="fun">A conversion function taking in an object of type T and returning one of type U</param>
		''' <returns></returns>
		Public Shared Function ArrayConvertAll(Of T, U)(array As T()(), fun As Func(Of T, U)) As U()()
			Dim rows As Integer = array.Length
			Dim cols As Integer = 0
			If rows = 0 Then
				Return CreateMatrixJagged(Of U)(rows, cols)
			Else
				cols = array(0).Length
			End If
			Dim res As U()() = CreateMatrixJagged(Of U)(rows, cols)
			For i As Integer = 0 To rows - 1
				If array(i).Length <> cols Then
					Throw New ArgumentException("Each element of the input jagged array must have the same length. Failed for index " & i.ToString())
				End If
				For j As Integer = 0 To cols - 1
					res(i)(j) = fun(array(i)(j))
				Next
			Next
			Return res
		End Function

		''' <summary>
		''' Creates a jagged array with elements of the same length
		''' </summary>
		''' <typeparam name="T">The element type of the array</typeparam>
		''' <param name="outerDim">length of the outer dimension</param>
		''' <param name="innerDim">length of the inner dimension</param>
		''' <returns></returns>
		Public Shared Function CreateMatrixJagged(Of T)(outerDim As Integer, innerDim As Integer) As T()()
			Dim result = New T(outerDim - 1)() {}
			For i As Integer = 0 To outerDim - 1
				result(i) = New T(innerDim - 1) {}
			Next
			Return result
		End Function

		''' <summary>
		''' Convert all elements of a rectangular array to a vector, using a function to cast/transform each element.
		''' The dimension reduction is column-first, appending each line of the input array into the result vector.
		''' </summary>
		''' <typeparam name="T">The type of the input array elements</typeparam>
		''' <typeparam name="U">The type of the output array elements</typeparam>
		''' <param name="array">Input array</param>
		''' <param name="fun">A conversion function taking in an object of type T and returning one of type U</param>
		''' <returns></returns>
		Public Shared Function ArrayConvertAllOneDim(Of T, U)(array As T(,), fun As Func(Of T, U)) As U()
			Dim rows As Integer = array.GetLength(0)
			Dim cols As Integer = array.GetLength(1)
			Dim res As U() = New U(rows * cols - 1) {}
			For i As Integer = 0 To rows - 1
				For j As Integer = 0 To cols - 1
					res(rows * j + i) = fun(array(i, j))
				Next
			Next
			Return res
		End Function

		''' <summary>
		''' Convert a rectangular array to a vector.
		''' The dimension reduction is column-first, appending each line of the input array into the result vector.
		''' </summary>
		''' <typeparam name="U"></typeparam>
		''' <param name="array"></param>
		''' <returns></returns>
		Public Shared Function ArrayConvertOneDim(Of U)(array As U(,)) As U()
			Return ArrayConvertAllOneDim(array, Function(value) value)
		End Function

		' TODO: probably room for extension methods around Matrix inheritors
		''' <summary>
		''' Convert all elements of a vector into a rectangular array, using a function to cast/transform each element.
		''' Vector to matrix augmentation is done column first, i.e. "appending" successive lines to the bottom of the new matrix
		''' </summary>
		''' <typeparam name="T">The type of the input array elements</typeparam>
		''' <typeparam name="U">The type of the output array elements</typeparam>
		''' <param name="array">Input array</param>
		''' <param name="fun">A conversion function taking in an object of type T and returning one of type U</param>
		''' <param name="rows">The number of rows in the output</param>
		''' <param name="cols">The number of columns in the output</param>
		''' <returns></returns>
		Public Shared Function ArrayConvertAllTwoDim(Of T, U)(array As T(), fun As Func(Of T, U), rows As Integer, cols As Integer) As U(,)
			If cols < 0 Then
				Throw New ArgumentException("negative number for column numbers")
			End If
			If rows < 0 Then
				Throw New ArgumentException("negative number for row numbers")
			End If
			If array.Length < (rows * cols) Then
				Throw New ArgumentException("input array has less than rows*cols elements")
			End If
			Dim res As U(,) = New U(rows - 1, cols - 1) {}
			For i As Integer = 0 To rows - 1
				For j As Integer = 0 To cols - 1
					res(i, j) = fun(array(rows * j + i))
				Next
			Next
			Return res
		End Function

		' TODO: probably room for extension methods around Matrix inheritors
		''' <summary>
		''' Converts a vector into a rectangular array.
		''' Vector to matrix augmentation is done column first, i.e. "appending" successive lines to the bottom of the new matrix
		''' </summary>
		''' <typeparam name="U">The type of the output array elements</typeparam>
		''' <param name="array">Input array</param>
		''' <param name="rows">The number of rows in the output</param>
		''' <param name="cols">The number of columns in the output</param>
		''' <returns></returns>
		Public Shared Function ArrayConvertAllTwoDim(Of U)(array As U(), rows As Integer, cols As Integer) As U(,)
			Return ArrayConvertAllTwoDim(array, Function(value) value, rows, cols)
		End Function

        ''' <summary>
        ''' Subset an array
        ''' </summary>
        ''' <typeparam name="T">The type of the input array elements</typeparam>
        ''' <param name="array">Input array</param>
        ''' <param name="from">Index of the first element to subset</param>
        ''' <param name="to">Index of the last element to subset</param>
        ''' <returns></returns>
        Public Shared Function Subset(Of T)(array As T(), from As Integer, [to] As Integer) As T()
            If from > [to] Then
                Throw New ArgumentException("Inconsistent subset: from > to")
            End If
            Dim count As Integer = ([to] - from) + 1
            Dim res = New T(count - 1) {}
            System.Array.Copy(array, from, res, 0, count)
            Return res
		End Function

        Friend Shared Function Prepend(Of T)(value As T, array As T()) As T()
            If array Is Nothing Then
                Return {value}
            End If
            Dim newArray = New T(array.Length) {}
            newArray(0) = value
            System.Array.Copy(array, 0, newArray, 1, array.Length)
            Return newArray
		End Function
	End Class
End Namespace

