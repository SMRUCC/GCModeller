Public Class Utils

    Public Shared ReadOnly ACGT As String() = New String() {"A", "C", "G", "T"}

    Shared ReadOnly LOG_2 As Double = Math.Log(2)

    Friend Shared Function getSequenceFromPair(sequences As IList(Of KeyValuePair(Of String, Integer))) As IList(Of String)
        Return sequences.[Select](Function(a) a.Key).ToList()
    End Function

    Friend Shared Function getSiteFromPair(sequences As IList(Of KeyValuePair(Of String, Integer))) As List(Of Integer)
        Return sequences.[Select](Function(a) a.Value).ToList()
    End Function

    Public Shared Function indexOfBase(base As Char) As Integer
        Select Case base
            Case "A"c
                Return 0
            Case "C"c
                Return 1
            Case "G"c
                Return 2
            Case "T"c
                Return 3
            Case Else
                Console.WriteLine("Unknown Base")
                Return -1
        End Select
    End Function

    Public Shared Function calcInformationContent(countSum As Double, counts As Integer()) As Double
        Dim probabilities As Double() = counts.[Select](Function(i) i / countSum).ToArray()
        Return Enumerable.Select(Of Double, Global.System.[Double])(probabilities, CType(Function(p)
                                                                                             If p = 0 Then
                                                                                                 Return CDbl(0)
                                                                                             Else
                                                                                                 Return p * (Math.Log(p / 0.25) / LOG_2)
                                                                                             End If
                                                                                         End Function, Func(Of Double, Double))).Sum()
    End Function
End Class
