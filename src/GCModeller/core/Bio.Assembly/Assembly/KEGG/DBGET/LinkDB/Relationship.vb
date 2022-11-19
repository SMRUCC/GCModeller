#Region "Microsoft.VisualBasic::e677e3ea61e436ca9e69e2b7e4b78fe3, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\LinkDB\Relationship.vb"

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

    '   Total Lines: 41
    '    Code Lines: 16
    ' Comment Lines: 18
    '   Blank Lines: 7
    '     File Size: 1.31 KB


    '     Enum Relationships
    ' 
    '         equivalent, indirect, original, reverse
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class Relationship
    ' 
    '         Properties: left, relationship, right
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Assembly.KEGG.DBGET.LinkDB

    Public Enum Relationships

        unknown = 0

        ''' <summary>
        ''' links are special original links to signify equivalent contents between 
        ''' KEGG GENES, COMPOUND, DRUG, REACTION databases and databases other 
        ''' than KEGG.
        ''' </summary>
        equivalent
        ''' <summary>
        ''' links are derived by combining two or more original links. Currently, 
        ''' links from KEGG GENES to REACTION via KO, and to COMPOUND via REACTION 
        ''' are available.
        ''' </summary>
        indirect
        ''' <summary>
        ''' links are extracted from the database entries provided by the GenomeNet 
        ''' DBGET system.
        ''' </summary>
        original
        ''' <summary>
        ''' links are derived from the original links by exchanging a source entry 
        ''' and its target entry.
        ''' </summary>
        reverse
    End Enum

    Public Class Relationship

        Public Property left As String
        Public Property relationship As Relationships
        Public Property right As NamedValue(Of String)

    End Class
End Namespace
