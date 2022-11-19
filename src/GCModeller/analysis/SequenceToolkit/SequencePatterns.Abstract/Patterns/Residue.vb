#Region "Microsoft.VisualBasic::8c7f6b2ab0a889a2a9e51e02247a7640, GCModeller\analysis\SequenceToolkit\SequencePatterns.Abstract\Patterns\Residue.vb"

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


    ' Code Statistics:

    '   Total Lines: 89
    '    Code Lines: 68
    ' Comment Lines: 8
    '   Blank Lines: 13
    '     File Size: 2.55 KB


    '     Class Residue
    ' 
    '         Properties: Raw, Regex, RepeatRanges, Type
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: GetComplement, ToString
    ' 
    '         Sub: __newChar
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
