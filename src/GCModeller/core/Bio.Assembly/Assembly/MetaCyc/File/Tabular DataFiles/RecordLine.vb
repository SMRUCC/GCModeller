#Region "Microsoft.VisualBasic::a26edffb93c398913ac8d62206578832, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\Tabular DataFiles\RecordLine.vb"

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

    '   Total Lines: 31
    '    Code Lines: 23
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 764 B


    '     Class RecordLine
    ' 
    '         Properties: Data
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Namespace Assembly.MetaCyc.File

    Public Class RecordLine

        Public Property Data As String()

        Sub New()
        End Sub

        Sub New(line As String)
            Data = Strings.Split(line, vbTab)
        End Sub

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder =
                    New StringBuilder(128)

            For i As Integer = 0 To Data.Length - 1
                If String.Equals(Data(i), String.Empty) Then
                    sBuilder.Append("NULL, ")
                Else
                    sBuilder.AppendFormat("{0}, ", Data(i))
                End If
            Next

            Return sBuilder.ToString
        End Function
    End Class
End Namespace
