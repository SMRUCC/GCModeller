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

Imports System.Text
Imports System.Xml.Serialization

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    ''' <summary>
    ''' ``COMMENT``段: 记录的附加说明信息
    ''' </summary>
    Public Class COMMENT : Inherits KeyWord

        <XmlIgnore> Public Property Comment As String
        ''' <summary>
        ''' 原始的文本行(用于写出的时候保持原始的换行位置)
        ''' </summary>
        <XmlIgnore> Public Property Lines As String()

        ''' <summary>
        ''' This constant using for NCBI.Genbank keywords parsing.
        ''' </summary>
        ''' <remarks></remarks>
        Public Const BLANKS_INDEX As UInteger = 12

        Public Overrides Function ToString() As String
            Return Comment
        End Function

        Public Shared Narrowing Operator CType(data As COMMENT) As String
            Return data.Comment
        End Operator

        Public Shared Widening Operator CType(s_Data As String()) As COMMENT
            If s_Data.IsNullOrEmpty Then
                Return New COMMENT With {.Comment = String.Empty, .Lines = New String() {}}
            End If

            Dim sb As New StringBuilder(ACCESSION.__fieldValue(s_Data(Scan0)))

            For i As Integer = 1 To s_Data.Length - 1
                Call sb.Append(" "c).Append(s_Data(i).Trim())
            Next

            Return New COMMENT With {
                .Comment = sb.ToString(),
                .Lines = s_Data
            }
        End Operator
    End Class
End Namespace
