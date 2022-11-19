#Region "Microsoft.VisualBasic::828f149cbff637f482095dd908159ae0, GCModeller\models\Networks\Microbiome\UniProtBuilder\CacheGenerator.vb"

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

    '   Total Lines: 122
    '    Code Lines: 79
    ' Comment Lines: 26
    '   Blank Lines: 17
    '     File Size: 4.49 KB


    ' Class CacheGenerator
    ' 
    '     Properties: counts, KO_list, taxonomy
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: ScanInternal
    ' 
    '     Sub: CopyTo, doScan
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Public Class CacheGenerator

    Public ReadOnly Property KO_list As String
    Public ReadOnly Property taxonomy As String
    Public ReadOnly Property counts As String

    Const KEGG_list$ = "KO.list"
    Const Taxonomy_data$ = "taxonomy.txt"
    Const gene_counts$ = "gene.counts"
    Const blank$ = "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="cache">
    ''' The cache data directory
    ''' </param>
    Sub New(cache As String)
        KO_list = $"{cache}/{KEGG_list}"
        taxonomy = $"{cache}/{Taxonomy_data}"
        counts = $"{cache}/{gene_counts}"
    End Sub

    Public Sub CopyTo(destination As String)
        With destination & "/"
            Call KO_list.FileCopy(.ByRef)
            Call taxonomy.FileCopy(.ByRef)
            Call counts.FileCopy(.ByRef)
        End With
    End Sub

    ''' <summary>
    ''' 函数返回来的是临时文件夹的路径位置
    ''' </summary>
    ''' <param name="UniProtXml"></param>
    ''' <returns></returns>
    Public Function ScanInternal(UniProtXml As IEnumerable(Of entry)) As CacheGenerator
        Using KO As StreamWriter = Me.KO_list.OpenWriter,
            taxon As StreamWriter = Me.taxonomy.OpenWriter,
            counts As StreamWriter = Me.counts.OpenWriter

            Call doScan(UniProtXml, KO, taxon, counts)

            Call KO.Flush()
            Call counts.Flush()
            Call taxon.Flush()
        End Using

        Return Me
    End Function

    Private Shared Sub doScan(UniProtXml As IEnumerable(Of entry), KO As StreamWriter, taxon As StreamWriter, counts As StreamWriter)
        ' KO 缓存文件示例
        ' 
        ' taxonomy_id KO
        ' taxonomy_id KO
        ' taxonomy_id KO
        '
        ' 
        Dim indexedTaxonomy As New Index(Of String)
        Dim i As i32 = 0

        For Each protein As entry In UniProtXml _
            .Where(Function(org)
                       ' 因为在这里构建的是微生物组的参考库
                       ' 所以在这里只需要细菌的数据
                       Return Not org.organism Is Nothing AndAlso
                              Not org.organism.dbReference Is Nothing AndAlso
                                  org.organism.lineage.taxonlist(Scan0).TextEquals("Bacteria")
                   End Function)

            Dim taxonomy As organism = protein.organism
            Dim taxonID$ = taxonomy.dbReference _
                .Where(Function(a) a.type = "NCBI Taxonomy") _
                .First _
                .id

            ' 为了计算出覆盖度，需要知道基因组之中有多少个基因的记录
            ' 在这里直接记录下物种的编号，在后面分组计数就可以了解基因组
            ' 之中的基因的总数了
            Call counts.WriteLine(taxonID)

            ' 如果已经存在index了，则不会写入taxo文件之中
            If Not taxonID Like indexedTaxonomy Then
                Call taxon.WriteLine(taxonomy.GetXml)
                Call taxon.WriteLine(blank)
                Call indexedTaxonomy.Add(taxonID)
            End If

            Dim KOlist$() = protein.xrefs _
                .TryGetValue("KO") _
                .SafeQuery _
                .Select(Function(KEGG) KEGG.id) _
                .ToArray
            Dim subCellularLocations = protein.SubCellularLocations

            If KOlist.Length > 0 Then
                Call KO.WriteLine(
                    value:=KOlist _
                        .Select(Function(KOid)
                                    ' taxonID KOid acc都是固定的
                                    ' subcellular location可能有些蛋白的注释是空的
                                    Return taxonID & vbTab & KOid & vbTab & protein.accessions(Scan0) & vbTab & subCellularLocations.JoinBy("; ")
                                End Function) _
                        .JoinBy(KO.NewLine)
                )
            End If

            If ++i Mod 300 = 0 Then
                Call Console.Write(i)
                Call Console.Write(vbTab)
            End If
        Next
    End Sub
End Class
