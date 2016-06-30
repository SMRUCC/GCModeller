Imports System.Numerics

''' <summary>
''' An internal helper class to convert types of arrays, primarily for data operations necessary for .NET types to/from R concepts.
''' </summary>
Module Utility
    
    Public Function AddFirst(Of T)(value As T, array__1 As T()) As T()
        If array__1 Is Nothing Then
            Return New T() {value}
        End If
        Dim newArray = New T(array__1.Length) {}
        newArray(0) = value
        Array.Copy(array__1, 0, newArray, 1, array__1.Length)
        Return newArray
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Friend Function CheckNil(engine As REngine, pointer As IntPtr) As Boolean
        Return engine.NilValue.DangerousGetHandle() = pointer
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Friend Function CheckUnbound(engine As REngine, pointer As IntPtr) As Boolean
        Return engine.UnboundValue.DangerousGetHandle() = pointer
    End Function

    Friend Function ArrayConvertAll(Of T, U)(array As T(,), fun As Func(Of T, U)) As U(,)
        Dim rows As Integer = array.GetLength(0)
        Dim cols As Integer = array.GetLength(1)
        Dim res As U(,) = New U(rows - 1, cols - 1) {}
        For i As Integer = 0 To rows - 1
            For j As Integer = 0 To cols - 1
                res(i, j) = fun(array(i, j))
            Next
        Next
        Return res
    End Function

    Friend Function ArrayConvertAllOneDim(Of T, U)(array As T(,), fun As Func(Of T, U)) As U()
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

    ' TODO: probably room for extension methods around Matrix inheritors
    Friend Function ArrayConvertAllTwoDim(Of T, U)(array As T(), fun As Func(Of T, U), rows As Integer, cols As Integer) As U(,)
        Dim res As U(,) = New U(rows - 1, cols - 1) {}
        For i As Integer = 0 To rows - 1
            For j As Integer = 0 To cols - 1
                res(i, j) = fun(array(rows * j + i))
            Next
        Next
        Return res
    End Function

    Friend Function ArrayConvertOneDim(Of U)(array As U(,)) As U()
        Return ArrayConvertAllOneDim(array, Function(value) value)
    End Function

    ' TODO: probably room for extension methods around Matrix inheritors
    Friend Function ArrayConvertAllTwoDim(Of U)(array As U(), rows As Integer, cols As Integer) As U(,)
        Return ArrayConvertAllTwoDim(array, Function(value) value, rows, cols)
    End Function

    Friend Function SerializeComplexToDouble(values As Complex()) As Double()
        Dim data = New Double(2 * values.Length - 1) {}
        For i As Integer = 0 To values.Length - 1
            data(2 * i) = values(i).Real
            data(2 * i + 1) = values(i).Imaginary
        Next
        Return data
    End Function

    Friend Function DeserializeComplexFromDouble(data As Double()) As Complex()
        Dim dblLen As Integer = data.Length
        If dblLen Mod 2 <> 0 Then
            Throw New ArgumentException("Serialized definition of complexes must be of even length")
        End If
        Dim n As Integer = dblLen \ 2
        Dim res = New Complex(n - 1) {}
        For i As Integer = 0 To n - 1
            res(i) = New Complex(data(2 * i), data(2 * i + 1))
        Next
        Return res
    End Function

    Friend Function Subset(Of T)(array As T(), from As Integer, [to] As Integer) As T()
        Dim res = New T(([to] - from)) {}
        For i As Integer = 0 To res.Length - 1
            res(i) = array(from + i)
        Next
        Return res
    End Function

End Module
