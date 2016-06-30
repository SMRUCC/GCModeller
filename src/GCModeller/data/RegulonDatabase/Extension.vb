Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

Public Module Extensions

    ''' <summary>
    ''' 判断这个locus_tag的集合是否是连续的
    ''' </summary>
    ''' <param name="lstId"></param>
    ''' <returns></returns>
    Public Function IsContinuous(lstId As IEnumerable(Of String)) As Boolean
        Dim n As Integer() = lstId.ToArray(Function(x) CInt(Val(Regex.Matches(x, "\d+").ToArray.LastOrDefault)))
        n = (From x In n Select x Order By x Ascending).ToArray
        Dim pre As Integer = n.First

        For Each x In n.Skip(1)
            If x - pre <> 1 Then
                Return False
            Else
                pre = x
            End If
        Next

        Return True
    End Function

    ''' <summary>
    ''' 得到连续的部分
    ''' </summary>
    ''' <param name="lstId"></param>
    ''' <returns></returns>
    Public Function ContinuouParts(lstId As IEnumerable(Of String)) As String()()
        Dim n = (From s As String In lstId
                 Let x As Integer = CInt(Val(Regex.Matches(s, "\d+").ToArray.LastOrDefault))
                 Select x,
                     s
                 Order By x Ascending).ToArray
        Dim pre = n.First
        Dim parts As New List(Of String())
        Dim i As Integer = 1

        Do While True
            Dim tmp As New List(Of String)

            Do While True
                If i = n.Length OrElse n(i).x - pre.x <> 1 Then
                    Call tmp.Add(pre.s)
                    Call parts.Add(tmp.ToArray)
                    pre = n.Get(i.MoveNext)
                    Exit Do
                Else
                    Call tmp.Add(pre.s)
                    pre = n(i.MoveNext)
                End If
            Loop

            If i = n.Length Then
                Call parts.Add({pre.s})
                Exit Do
            ElseIf i = n.Length + 1 Then
                Exit Do
            End If
        Loop

        Return parts.ToArray
    End Function
End Module
