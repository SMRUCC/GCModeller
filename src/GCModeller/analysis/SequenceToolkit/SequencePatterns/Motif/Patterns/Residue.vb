Imports System.Text
Imports System.Xml.Serialization

Namespace Motif.Patterns

    Public Class Residue
        <XmlAttribute> Public Property Raw As String
        <XmlAttribute> Public Property Regex As String
        Public Property RepeatRanges As Ranges

        Public ReadOnly Property Type As Tokens
            Get
                If Not Raw Is Nothing AndAlso Raw.Length = 1 Then
                    Return Tokens.Residue
                ElseIf Raw.First = "["c AndAlso Raw.Last = "]"c Then
                    Return Tokens.QualifyingMatches
                Else
                    Return Tokens.Fragment
                End If
            End Get
        End Property

        ''' <summary>
        ''' 从残基里面构建
        ''' </summary>
        ''' <param name="c"></param>
        Sub New(c As Char)
            Call __newChar(c)
        End Sub

        Private Sub __newChar(c As Char)
            If Char.IsLower(c) Then
                Regex = "."c
            Else
                Regex = c
            End If
        End Sub

        ''' <summary>
        ''' 从片段里面构建
        ''' </summary>
        ''' <param name="s"></param>
        Sub New(s As String)
            If s.Length = 1 Then
                Call __newChar(s.First)
            Else
                Dim r As New StringBuilder(s)

                For Each c As Char In {"a"c, "t"c, "g"c, "c"c}
                    r.Replace(c, "."c)
                Next

                Raw = s
                Regex = r.ToString
            End If
        End Sub

        Public Function GetComplement() As Residue
            If Raw.Length = 1 Then
                Dim c As Char = Raw.First
                If c = "."c Then
                    Return New Residue("."c)
                Else
                    Select Case c
                        Case "A"c
                            Return New Residue("T"c)
                        Case "T"c
                            Return New Residue("A"c)
                        Case "G"c
                            Return New Residue("C"c)
                        Case "C"c
                            Return New Residue("G"c)
                        Case Else
                            Return New Residue(c)
                    End Select
                End If
            Else

            End If

            Throw New NotImplementedException
        End Function

        Public Overrides Function ToString() As String
            Return Raw.ToString
        End Function
    End Class
End Namespace