#Region "Microsoft.VisualBasic::422e0514370fe9b52edabcf0c1667b03, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\COG\Category.vb"

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

    '   Total Lines: 39
    '    Code Lines: 33
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.31 KB


    '     Class Catalog
    ' 
    '         Properties: [Class], Description, SubClasses
    ' 
    '         Function: GetDescription, ToArray, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Assembly.NCBI.COG

    Public Class Catalog

        <XmlAttribute> Public Property [Class] As COGCategories
        <XmlAttribute> Public Property Description As String
        <XmlElement> Public Property SubClasses As Dictionary(Of Char, String)

        Public Overrides Function ToString() As String
            Return Description
        End Function

        Public Function ToArray() As COGFunction()
            Return SubClasses.Select(
                Function(x) New COGFunction With {
                    .Category = [Class],
                    .Catalog = x.Key,
                    .Description = x.Value
            }).ToArray
        End Function

        Public Function GetDescription(COG As Char, ByRef description As String) As Boolean
            If _SubClasses.ContainsKey(COG) Then
                description = _SubClasses(COG)
                Return True
            Else
                description = ""
                Return False
            End If
        End Function
    End Class
End Namespace
