#Region "Microsoft.VisualBasic::ace4a7983048a7b49af23affc7f50008, KEGG\ReactionNetwork\Models\ReactionClassTable.vb"

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

    '     Class ReactionClassTable
    ' 
    '         Properties: [from], [to], category, define, rId
    ' 
    '         Function: CreateIndexKey, IndexTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace ReactionNetwork

    Public Class ReactionClassTable

        Public Property rId As String
        Public Property category As String
        Public Property [from] As String
        Public Property [to] As String
        Public Property define As String

        Public Shared Function IndexTable(table As IEnumerable(Of ReactionClassTable)) As Index(Of String)
            Return table _
                .Select(Function(r) CreateIndexKey(r.from, r.to)) _
                .GroupBy(Function(key) key) _
                .Keys _
                .Indexing
        End Function

        Public Shared Function CreateIndexKey(a As String, b As String) As String
            Return {a, b}.OrderBy(Function(s) s).JoinBy("+")
        End Function

    End Class
End Namespace
