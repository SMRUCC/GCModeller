#Region "Microsoft.VisualBasic::d18920e9770fef11cc65e003d51d3825, sciBASIC#\Data\Trinity\Model\Sentence.vb"

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

'   Total Lines: 100
'    Code Lines: 70
' Comment Lines: 10
'   Blank Lines: 20
'     File Size: 2.98 KB


'     Class Sentence
' 
'         Properties: IsEmpty, segments
' 
'         Function: matchIndex, Parse, searchIndex, ToString, Trim
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq

Namespace Model

    Public Class Sentence

        ''' <summary>
        ''' 带有前后顺序的单词列表
        ''' </summary>
        ''' <returns></returns>
        Public Property tokens As String()

        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return tokens.IsNullOrEmpty OrElse tokens.All(AddressOf TextRank.IsEmpty)
            End Get
        End Property

        Public Function has(token As String) As Boolean
            Return Array.IndexOf(tokens, token) > -1
        End Function

        ''' <summary>
        ''' exactly token matched
        ''' </summary>
        ''' <param name="token"></param>
        ''' <returns></returns>
        Public Function matchIndex(token As String) As Integer
            Return Array.IndexOf(tokens, token)
        End Function

        ''' <summary>
        ''' search for starts with [prefix]
        ''' </summary>
        ''' <param name="token"></param>
        ''' <returns></returns>
        Public Function searchIndex(token As String) As Integer
            For i As Integer = 0 To tokens.Length - 1
                If _tokens(i).StartsWith(token) Then
                    Return i
                End If
            Next

            Return -1
        End Function

        Public Overrides Function ToString() As String
            Return tokens.JoinBy(" ")
        End Function

        Friend Shared Function Parse(line As String) As Sentence
            Return New Sentence With {
               .tokens = New SentenceCharWalker(line) _
                   .GetTokens _
                   .ToArray
            }
        End Function

        Friend Function Trim() As Sentence
            Return New Sentence With {
               .tokens = tokens _
                   .Where(Function(si) Not TextRank.IsEmpty(si)) _
                   .ToArray
            }
        End Function

    End Class
End Namespace
