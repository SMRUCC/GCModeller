Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic

Namespace bnlearn

    Public Class ConditionalProbability

        <XmlAttribute> Public Property Variable As String
        <XmlAttribute> Public Property ConditionalVariable As String
        Public Property CPT As Double()()()

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(ConditionalVariable) Then
                Return String.Format("P({0})", Variable)
            Else
                Return String.Format("P({0}|{1})", Variable, ConditionalVariable)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="strData"></param>
        ''' <returns></returns>
        ''' <remarks>  
        ''' list(node = "B", parents = "A", children = "E", prob = c(0.855072463768116, 0.0257944422284271, 0.119133094003457, 0.44468791500664, 0.221181938911023, 0.334130146082337, 0.11535895382973, 0.0949426207632773, 0.789698425406992))
        ''' </remarks>
        Friend Shared Function TryParse(strData As String, numberOfFactors As Integer) As ConditionalProbability
            Dim NodeName As String = Regex.Match(Regex.Match(strData, "node = "".+?""").Value, """.+?""").Value
            Dim Parents As String = Regex.Match(strData, "parents = ("".+?""|c\(.+?\)|character\(0\))").Value
            Dim Probs As String() = Strings.Split(Regex.Match(strData, "prob = c\(.+?\)").Value, ", ")

            NodeName = Mid(NodeName, 2, Len(NodeName) - 2)
            If InStr(Parents, "character(0)") Then
                Parents = ""
            Else
                Parents = Regex.Match(Parents, """.+?""").Value
                Parents = Mid(Parents, 2, Len(Parents) - 2)
            End If
            Probs(0) = Mid(Probs.First, 10)
            Probs(Probs.Count - 1) = Probs.Last.Replace(")", "")

            Dim retValue As ConditionalProbability = New ConditionalProbability With {.Variable = NodeName, .ConditionalVariable = Parents}
            Dim CPT = (From s In Probs Select Val(s)).ToArray

            If String.IsNullOrEmpty(Parents) Then
                retValue.CPT = New Double()()() {New Double()() {CPT}}
            Else
                Dim n As Integer = numberOfFactors ^ 2
                Dim ListArray = New List(Of Double()())
                For i As Integer = 0 To Probs.Count - 1 Step n
                    Dim ChunkBuffer = New Double(n - 1) {}
                    Call Array.ConstrainedCopy(CPT, i, ChunkBuffer, 0, n)

                    Dim ElementList = New List(Of Double())
                    For j As Integer = 0 To ChunkBuffer.Count - 1 Step numberOfFactors
                        Dim TempChunk = New Double(numberOfFactors - 1) {}
                        Call Array.ConstrainedCopy(ChunkBuffer, j, TempChunk, 0, numberOfFactors)
                        Call ElementList.Add(TempChunk)
                    Next

                    Call ListArray.Add(ElementList.ToArray)
                Next

                retValue.CPT = ListArray.ToArray
            End If

            Return retValue
        End Function
    End Class
End Namespace