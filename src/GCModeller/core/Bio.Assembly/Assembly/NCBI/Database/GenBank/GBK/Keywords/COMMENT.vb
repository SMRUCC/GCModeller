﻿#Region "Microsoft.VisualBasic::a85c277dd44de5f2685db612b97c7904, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\COMMENT.vb"

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

    '   Total Lines: 38
    '    Code Lines: 24 (63.16%)
    ' Comment Lines: 4 (10.53%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (26.32%)
    '     File Size: 1.24 KB


    '     Class COMMENT
    ' 
    '         Properties: Comment
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    Public Class COMMENT : Inherits KeyWord

        <XmlIgnore> Public Property Comment As String

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
            If s_Data Is Nothing OrElse s_Data.Length = 0 Then _
                Return New COMMENT With {.Comment = String.Empty}

            Dim sBuilder As StringBuilder =
                New StringBuilder(Mid$(s_Data.First, BLANKS_INDEX).Trim)

            For i As Integer = 1 To s_Data.Length - 1
                sBuilder.AppendFormat(" {0}", s_Data(i).Trim)
            Next

            Return New COMMENT With {.Comment = sBuilder.ToString}
        End Operator
    End Class
End Namespace
