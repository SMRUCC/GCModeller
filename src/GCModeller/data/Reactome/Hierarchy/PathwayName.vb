#Region "Microsoft.VisualBasic::e7b848db7f879424ab6a4071d14873e8, data\Reactome\Hierarchy\PathwayName.vb"

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

    '   Total Lines: 26
    '    Code Lines: 20
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 723 B


    ' Class PathwayName
    ' 
    '     Properties: id, name, tax_name
    ' 
    '     Function: LoadInternal, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text

Public Class PathwayName

    Public Property id As String
    Public Property name As String
    Public Property tax_name As String

    Public Overrides Function ToString() As String
        Return $"{id}: {name}"
    End Function

    Public Shared Iterator Function LoadInternal() As IEnumerable(Of PathwayName)
        For Each line As String In My.Resources.ReactomePathways.LineTokens
            Dim t As String() = line.Split(ASCII.TAB)
            Dim name As New PathwayName With {
                .id = t(0),
                .name = t(1),
                .tax_name = t(2)
            }

            Yield name
        Next
    End Function

End Class
