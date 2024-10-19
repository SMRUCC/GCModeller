Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

Namespace edu.illinois
    'using Pair = javafx.util.Pair;


    Public Class Utils
        Public Shared ReadOnly ACGT As String() = New String() {"A", "C", "G", "T"}
        Private Shared ReadOnly LOG_2 As Double = Math.Log(2)

        Friend Shared Function getSequenceFromPair(sequences As IList(Of KeyValuePair(Of String, Integer))) As IList(Of String)
            Return sequences.[Select](Function(a) a.Key).ToList()
        End Function

        Friend Shared Function getSiteFromPair(sequences As IList(Of KeyValuePair(Of String, Integer))) As List(Of Integer)
            Return sequences.[Select](Function(a) a.Value).ToList()
        End Function

        Public Shared Function randomBases(len As Integer, r As Random) As String
            Return Enumerable.Select(Of Integer, Global.System.String)(Enumerable.Range(CInt(0), CInt(len)), CType(Function(i) CStr(ACGT(CInt(r.Next(CInt(4))))), Func(Of Integer, String))).JoinBy("")
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
                                                                                                     Return p * (Math.Log(p / .25) / LOG_2)
                                                                                                 End If
                                                                                             End Function, Func(Of Double, Double))).Sum()
        End Function
    End Class

End Namespace
