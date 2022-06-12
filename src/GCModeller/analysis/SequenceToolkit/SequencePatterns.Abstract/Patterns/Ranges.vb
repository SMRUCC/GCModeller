#Region "Microsoft.VisualBasic::fa510a263189c706d79236ee5359172f, analysis\SequenceToolkit\SequencePatterns.Abstract\Patterns\Ranges.vb"

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

'     Class Ranges
' 
'         Properties: MaxExpression, raw
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Data
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Motif.Patterns

    Public Class Ranges : Inherits IntRange

        Public Property raw As String
        Public Property MaxExpression As String

        Sub New(token As PatternToken)
            raw = token.text

            Dim tokens As String() = raw.Split(","c)

            If tokens.Length = 2 Then
                Min = Scripting.CTypeDynamic(Of Integer)(tokens(0))
                MaxExpression = tokens(1)
                If Information.IsNumeric(MaxExpression) Then
                    Max = Scripting.CTypeDynamic(Of Integer)(MaxExpression)
                End If
            ElseIf tokens.Length = 1 Then
                MaxExpression = tokens(0)
                If Information.IsNumeric(MaxExpression) Then
                    Max = Scripting.CTypeDynamic(Of Integer)(MaxExpression)
                End If
            Else
                Throw New SyntaxErrorException(raw)
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
