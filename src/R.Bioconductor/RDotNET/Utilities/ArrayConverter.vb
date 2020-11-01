Imports System

Namespace Utilities
    ''' <summary>
    ''' A set of helpers to convert, query and transform arrays
    ''' </summary>
    Public Module ArrayConverter
        ''' <summary>
        ''' Convert all elements of a rectangular array, using a function to cast/transform each element.
        ''' </summary>
        ''' <typeparam name="T">The type of the input array elements</typeparam>
        ''' <typeparam name="U">The type of the output array elements</typeparam>
        ''' <param name="array">Input array</param>
        ''' <param name="fun">A conversion function taking in an object of type T and returning one of type U</param>
        ''' <returns></returns>
        Public Function ArrayConvertAll(Of T, U)(ByVal array As T(,), ByVal fun As Func(Of T, U)) As U(,)
            Dim rows = array.GetLength(0)
            Dim cols = array.GetLength(1)
            Dim res = New U(rows - 1, cols - 1) {}

            For i = 0 To rows - 1 ' TODO: what is the best indexing order to avoid memory cache misses, rowfirst or colfirst?

                For j = 0 To cols - 1
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
        Public Function ArrayConvertAll(Of T, U)(ByVal array As T()(), ByVal fun As Func(Of T, U)) As U()()
            Dim rows = array.Length
            Dim cols = 0

            If rows = 0 Then
                Return CreateMatrixJagged(Of U)(rows, cols)
            Else
                cols = array(0).Length
            End If

            Dim res = CreateMatrixJagged(Of U)(rows, cols)

            For i = 0 To rows - 1
                If array(i).Length <> cols Then Throw New ArgumentException("Each element of the input jagged array must have the same length. Failed for index " & i.ToString())

                For j = 0 To cols - 1
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
        Public Function CreateMatrixJagged(Of T)(ByVal outerDim As Integer, ByVal innerDim As Integer) As T()()
            Dim result = New T(outerDim - 1)() {}

            For i = 0 To outerDim - 1
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
        Public Function ArrayConvertAllOneDim(Of T, U)(ByVal array As T(,), ByVal fun As Func(Of T, U)) As U()
            Dim rows = array.GetLength(0)
            Dim cols = array.GetLength(1)
            Dim res = New U(rows * cols - 1) {}

            For i = 0 To rows - 1

                For j = 0 To cols - 1
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
        Public Function ArrayConvertOneDim(Of U)(ByVal array As U(,)) As U()
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
        Public Function ArrayConvertAllTwoDim(Of T, U)(ByVal array As T(), ByVal fun As Func(Of T, U), ByVal rows As Integer, ByVal cols As Integer) As U(,)
            If cols < 0 Then Throw New ArgumentException("negative number for column numbers")
            If rows < 0 Then Throw New ArgumentException("negative number for row numbers")
            If array.Length < rows * cols Then Throw New ArgumentException("input array has less than rows*cols elements")
            Dim res = New U(rows - 1, cols - 1) {}

            For i = 0 To rows - 1

                For j = 0 To cols - 1
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
        Public Function ArrayConvertAllTwoDim(Of U)(ByVal array As U(), ByVal rows As Integer, ByVal cols As Integer) As U(,)
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
        Public Function Subset(Of T)(ByVal array As T(), ByVal from As Integer, ByVal [to] As Integer) As T()
            If from > [to] Then Throw New ArgumentException("Inconsistent subset: from > to")
            Dim count = [to] - from + 1
            Dim res = New T(count - 1) {}
            System.Array.Copy(array, from, res, 0, count)
            Return res
        End Function

        Friend Function Prepend(Of T)(ByVal value As T, ByVal array As T()) As T()
            If array Is Nothing Then
                Return {value}
            End If

            Dim newArray = New T(array.Length + 1 - 1) {}
            newArray(0) = value
            System.Array.Copy(array, 0, newArray, 1, array.Length)
            Return newArray
        End Function
    End Module
End Namespace
