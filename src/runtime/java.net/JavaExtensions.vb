Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

Public Module JavaExtensions

    '<Extension> Public Function StringSplit(source As String, regexDelimiter As String, trimTrailingEmptyStrings As Boolean) As String()
    '    Dim splitArray As String() = Global.System.Text.RegularExpressions.Regex.Split(source, regexDelimiter)

    '    If trimTrailingEmptyStrings Then
    '        If splitArray.Length > 1 Then
    '            For i As Integer = splitArray.Length To 1 Step -1
    '                If splitArray(i - 1).Length > 0 Then
    '                    If i < splitArray.Length Then
    '                        Global.System.Array.Resize(splitArray, i)
    '                    End If

    '                    Exit For
    '                End If
    '            Next
    '        End If
    '    End If

    '    Return splitArray
    'End Function

    <Extension> Public Function Size(Of T)(list As Generic.IEnumerable(Of T)) As Integer
        If list.IsNullOrEmpty Then
            Return 0
        End If

        Return list.Count
    End Function

    Public Function Remove(Of T)(list As List(Of T), index As Integer) As List(Of T)
        Call list.RemoveAt(index)
        Return list
    End Function

    Public Sub Output(s As String)
        Call Console.WriteLine(s)
    End Sub

    <Extension> Public Function Bytes(str As String) As SByte()
        Return (From b As Byte In Encoding.Default.GetBytes(str) Select CType(b, SByte)).ToArray
    End Function

    <Extension> Public Function Asc(c As Char) As Integer
        Return AscW(c)
    End Function

    <Extension> Public Function CompareTo(a As Integer?, b As Integer?) As Integer
        Return a - b
    End Function

    <Extension> Public Function SubString(sb As StringBuilder, startIndex As Integer) As String
        Dim str As String = sb.ToString
        Return str.Substring(startIndex:=startIndex)
    End Function

    <Extension> Public Function CharValue(data As SByte()) As String
        Dim ChunkBuffer = (From b In data Select CType(b, Byte)).ToArray
        Return Encoding.Default.GetString(ChunkBuffer)
    End Function

End Module

Public Module Collections

    Public Function CubeArray(Of T)(x As Integer, y As Integer) As T()()()
        Dim LQuery = (From i As Integer In x.Sequence Select New T(y - 1)() {}).ToArray
        Return LQuery
    End Function

    Public Function Min(data As List(Of Integer)) As Integer
        Return data.Min
    End Function

    Public Function Min(data As List(Of Integer?)) As Integer
        Return data.Min
    End Function

    Public Function Max(data As List(Of Integer)) As Integer
        Return data.Max
    End Function

    Public Function Max(data As List(Of Integer?)) As Integer
        Return data.Max
    End Function

    Public Sub Sort(Of T)(ByRef list As List(Of T))
        Call list.Sort()
    End Sub

    Public Function ReverseOrder() As Boolean
        Return True
    End Function

    Public Function AscendingOrder() As Boolean
        Return False
    End Function

    Public Sub Sort(Of T)(ByRef list As List(Of T), orderReverse As Boolean)
        Call list.Sort()

        If orderReverse Then
            Call list.Reverse()
        End If
    End Sub

    Public Function ReturnRectangularIntArray(Size1 As Integer, Size2 As Integer) As Integer()()
        Dim Array As Integer()() = New Integer(Size1 - 1)() {}
        For Array1 As Integer = 0 To Size1 - 1
            Array(Array1) = New Integer(Size2 - 1) {}
        Next
        Return Array
    End Function

    Public Function ReturnRectangularDoubleArray(Size1 As Integer, Size2 As Integer) As Double()()
        Dim Array As Double()() = New Double(Size1 - 1)() {}
        For Array1 As Integer = 0 To Size1 - 1
            Array(Array1) = New Double(Size2 - 1) {}
        Next
        Return Array
    End Function

    Public Function ReturnRectangularLongArray(Size1 As Integer, Size2 As Integer) As Long()()
        Dim Array As Long()() = New Long(Size1 - 1)() {}
        For Array1 As Integer = 0 To Size1 - 1
            Array(Array1) = New Long(Size2 - 1) {}
        Next
        Return Array
    End Function

End Module