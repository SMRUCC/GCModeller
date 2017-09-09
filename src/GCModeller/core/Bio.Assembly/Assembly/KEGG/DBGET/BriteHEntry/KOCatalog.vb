#Region "Microsoft.VisualBasic::185dfa7faa3b6d2409355aabd59ab8f0, ..\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\KOCatalog.vb"

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

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Public Class KOCatalog : Inherits CatalogProfiling

        Public Property [Class] As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' ###### KEGG Orthology (KO) - All Categories
        ''' 
        ''' > http://www.kegg.jp/kegg-bin/get_htext?ko00000.keg
        ''' 
        ''' In general KO grouping of functional orthologs is defined in the context of KEGG molecular networks (KEGG pathway maps, 
        ''' BRITE hierarchies and KEGG modules), which are in fact represented as networks of nodes identified by K numbers. 
        ''' The relationships between KOs and corresponding molecular networks are represented in the following KO system.
        ''' 
        ''' The fact that functional information is associated with ortholog groups is a unique aspect of the KEGG resource. 
        ''' The sequence similarity based inference as a generalization of limited amount of experimental evidence is predefined 
        ''' in KEGG. As implemented in BlastKOALA and other tools, the sequence similarity search against KEGG GENES is a search 
        ''' for most appropriate K numbers. Once K numbers are assigned to genes in the genome, the KEGG pathways maps, Brite 
        ''' hierarchies, and KEGG modules are automatically reconstructed, enabling biological interpretation of high-level 
        ''' functions. 
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function ko00000() As Dictionary(Of String, BriteHText())
            Dim htext As htext = htext.StreamParser(My.Resources.ko00000)
            Dim maps = htext.Hierarchical _
                .EnumerateEntries _
                .GroupBy(Function(x) x.EntryId) _
                .ToDictionary(Function(x) x.Key,
                              Function(g) g.ToArray)
            Return maps
        End Function
    End Class
End Namespace
