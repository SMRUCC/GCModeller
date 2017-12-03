#Region "Microsoft.VisualBasic::8e8e96c6c4990324ba491b19de18d6b6, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\LinkDB\KEGGgenes.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.LinkDB
Imports SMRUCC.genomics.ComponentModel

Namespace Assembly.KEGG.DBGET.LinkDB

    ''' <summary>
    ''' http://www.genome.jp/dbget-bin/www_bget?
    ''' </summary>
    Public Module KEGGgenes

        Public Const URL_MODULE_GENES As String = "http://www.genome.jp/dbget-bin/get_linkdb?-t+genes+md:{0}"
        Public Const URL_PATHWAY_GENES As String = "http://www.genome.jp/dbget-bin/get_linkdb?-t+genes+path:{0}"

        Public Function Download(url As String) As IEnumerable(Of KeyValuePair)
            Return url.LinkDbEntries
        End Function
    End Module
End Namespace
