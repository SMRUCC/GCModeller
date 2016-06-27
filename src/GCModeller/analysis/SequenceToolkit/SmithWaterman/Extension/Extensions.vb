Imports System.Data.Linq
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic
Imports System.Collections.Generic

Public Module Extensions

    ''' <summary>
    ''' 可能会有重叠或者不连续，这个函数是为了计算高分区的Coverage而创建的
    ''' </summary>
    ''' <param name="regions"></param>
    ''' <returns></returns>
    Public Function Length(regions As IEnumerable(Of Coords)) As Integer
        Dim points As New Microsoft.VisualBasic.List(Of Integer)

        For Each x In regions
            Call points.Add((x.Y - x.X).Sequence(offset:=x.X))
        Next

        Dim array = (From x As Integer In points Select x Distinct).ToArray
        Return array.Length
    End Function

    <Extension> Public Function QueryLength(source As IEnumerable(Of HSP)) As Integer
        Dim nlst As Coords() =
            source.ToArray(Function(x) New Coords With {.X = x.FromA, .Y = x.ToA})
        Return Length(nlst)
    End Function

    <Extension> Public Function SubjectLength(source As IEnumerable(Of HSP)) As Integer
        Dim nlst As Coords() =
            source.ToArray(Function(x) New Coords With {.X = x.FromB, .Y = x.ToB})
        Return Length(nlst)
    End Function
End Module
