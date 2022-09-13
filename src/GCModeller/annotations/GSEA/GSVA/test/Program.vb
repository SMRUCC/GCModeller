#Region "Microsoft.VisualBasic::cad63e6164cfa7048c73ce4b4551e960, GCModeller\annotations\GSEA\GSVA\test\Program.vb"

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

    '   Total Lines: 54
    '    Code Lines: 48
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 2.40 KB


    ' Module Program
    ' 
    '     Function: createGene
    ' 
    '     Sub: Main
    ' 
    ' Class metainfo
    ' 
    '     Properties: KEGG, name
    ' 
    ' Class enrichment
    ' 
    '     Properties: compounds, FDR, Hits, Holm_adjust, Impact
    '                 links, logp, names, pathway, Rawp
    '                 term, Total
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSVA

Module Program
    Sub Main(args As String())
        Dim expr As Matrix = Matrix.LoadData("E:\GCModeller\src\GCModeller\annotations\GSEA\data\HTS\metabolome.csv")
        Dim background = "E:\GCModeller\src\GCModeller\annotations\GSEA\data\HTS\kegg_enrichment.xls".LoadTsv(Of enrichment)().ToArray
        Dim maps = "E:\GCModeller\src\GCModeller\annotations\GSEA\data\HTS\metainfo.csv" _
            .LoadCsv(Of metainfo) _
            .GroupBy(Function(d) d.KEGG) _
            .Select(Function(l) l.First) _
            .ToDictionary(Function(d) d.KEGG)
        Dim geneset = background _
            .Select(Function(d)
                        Return New Cluster With {.ID = d.term, .description = d.term, .members = d.compounds _
                            .Select(Function(id)
                                        Return createGene(maps(id).name)
                                    End Function) _
                            .ToArray, .names = d.term}
                    End Function).ToArray
        Dim scores = GSVA.gsva(expr, New Background With {.clusters = geneset})


        Pause()
    End Sub

    Private Function createGene(name As String) As BackgroundGene
        Return New BackgroundGene With {.accessionID = name, .name = name, .[alias] = {name}, .locus_tag = New NamedValue With {.name = name, .text = name}, .term_id = {name}}
    End Function
End Module

Public Class metainfo
    Public Property name As String
    Public Property KEGG As String
End Class

Public Class enrichment
    Public Property term As String
    Public Property Total As String
    Public Property Hits As String
    <Column("Raw p")> Public Property Rawp As String
    <Column("-log(p)")> Public Property logp As String
    <Column("Holm adjust")> Public Property Holm_adjust As String
    Public Property FDR As String
    Public Property Impact As String
    <Collection("compounds", ";")> Public Property compounds As String()
    Public Property pathway As String
    Public Property links As String
    Public Property names As String
End Class
