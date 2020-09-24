#Region "Microsoft.VisualBasic::2088deba272c8c8d98d6ea396bbe37a3, models\Networks\Network.BLAST\API.vb"

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

    ' Module API
    ' 
    ' 
    '     Delegate Function
    ' 
    '         Properties: BuildMethods
    ' 
    '         Function: __buildFromBlastHits, (+2 Overloads) BuildFromBBH, BuildFromBestHits, BuildFromBlastHits, BuildFromBlastOUT
    '                   MetaBuildFromBBH, SaveBLASTNetwork
    ' 
    '         Sub: __buildFromBlastHits
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

''' <summary>
''' 这个多用于宏基因组的研究
''' </summary>
''' 
<Package("BiologicalNetwork.BLAST",
                  Category:=APICategories.ResearchTools,
                  Description:="Metagenome blast network builder",
                  Publisher:="xie.guigang@gcmodeller.org",
                  Url:="http://gcmodeller.org",
                  Cites:="")>
<Cite(Title:="Viral assemblage composition in Yellowstone acidic hot springs assessed by network analysis",
      Abstract:="Understanding of viral assemblage structure in natural environments remains a daunting task. 
Total viral assemblage sequencing (for example, viral metagenomics) provides a tractable approach. However, even with the availability of next-generation sequencing technology it is usually only possible to obtain a fragmented view of viral assemblages in natural ecosystems. 
In this study, we applied a network-based approach in combination with viral metagenomics to investigate viral assemblage structure in the high temperature, acidic hot springs of Yellowstone National Park, USA. 
Our results show that this approach can identify distinct viral groups and provide insights into the viral assemblage structure. We identified 110 viral groups in the hot springs environment, with each viral group likely representing a viral family at the sub-family taxonomic level. 
Most of these viral groups are previously unknown DNA viruses likely infecting archaeal hosts. 
Overall, this study demonstrates the utility of combining viral assemblage sequencing approaches with network analysis to gain insights into viral assemblage structure in natural ecosystems.",
      AuthorAddress:="Thermal Biology Institute, Montana State University, Bozeman, MT, USA.
Department of Plant Sciences and Plant Pathology and, Montana State University, Bozeman, MT, USA.
Bioinformatics Core Facility, Montana State University, Bozeman, MT, USA.",
      Authors:="Bolduc, B.
Wirth, J. F.
Mazurie, A.
Young, M. J.",
      DOI:="10.1038/ismej.2015.28",
      ISSN:="1751-7370 (Electronic)
1751-7362 (Linking)",
      Issue:="10",
      Journal:="ISME J",
      Keywords:="Metavirome network; Yellowstone hot springs",
      Pages:="2162-77",
      PubMed:=26125684,
      StartPage:=0,
      URL:="",
      Volume:=9,
      Year:=2015)>
Public Module API

    Public Delegate Function BuildFromSource(source As String, locusDict As Dictionary(Of String, String)) As LDM.BLAST

    Public ReadOnly Property BuildMethods As IReadOnlyDictionary(Of String, BuildFromSource) =
        New Dictionary(Of String, API.BuildFromSource) From {
 _
        {"bbh", AddressOf API.BuildFromBBH},
        {"sbh", AddressOf API.BuildFromBestHits},
        {"blast_out", AddressOf API.BuildFromBlastOUT}
    }

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="blastout">blast输出文件的路径</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Build.From.BlastOUT")>
    Public Function BuildFromBlastOUT(blastout As String, locusDict As Dictionary(Of String, String)) As BLAST.LDM.BLAST
        Dim blastData = LocalBLAST.BLASTOutput.BlastPlus.TryParse(blastout)
        Dim hits = blastData.ExportOverview.GetExcelData
        Return hits.__buildFromBlastHits(locusDict)
    End Function

    ''' <summary>
    ''' 构建宏基因组网络里面的一个子网络
    ''' </summary>
    ''' <param name="hits"></param>
    ''' <returns></returns>
    <ExportAPI("Build.From.BlastHits"， Info:="Metagenome blast network builder")>
    <Extension>
    Public Function BuildFromBlastHits(hits As IEnumerable(Of BestHit), locusDict As Dictionary(Of String, String)) As BLAST.LDM.BLAST
        Return hits.__buildFromBlastHits(locusDict)
    End Function

    <ExportAPI("Build.From.BlastHits"， Info:="Metagenome blast network builder")>
    Public Function BuildFromBestHits(source As String, locusDict As Dictionary(Of String, String)) As LDM.BLAST
        Dim sbh = source.LoadCsv(Of BestHit)
        Return sbh.__buildFromBlastHits(locusDict)
    End Function

    <Extension>
    Private Function __buildFromBlastHits(Of TBLASTHit As IQueryHits)(hits As IEnumerable(Of TBLASTHit), locusDict As Dictionary(Of String, String)) As LDM.BLAST
        Dim hitEdges As LDM.Hit() = Nothing
        Dim proteins As LDM.Protein() = Nothing

        Call __buildFromBlastHits(hits, proteins, hitEdges, locusDict)

        Return New LDM.BLAST With { ' 得到基因组之间的比对信息的模型用于可视化操作
            .BlastHits = hitEdges,
            .Proteins = proteins
        }
    End Function

    Private Sub __buildFromBlastHits(Of TBLASTHit As IQueryHits)(
                                        hits As IEnumerable(Of TBLASTHit),
                                        ByRef outProt As LDM.Protein(),
                                        ByRef outHits As LDM.Hit(),
                                        locusDict As Dictionary(Of String, String))

        Dim hitEdges As LDM.Hit() = hits.Where(Function(x) Not x.isEmpty).Select(
            Function(x) New LDM.Hit With {
                .genomePairId = $"{locusDict.TryGetValue(x.queryName)}_vs__{locusDict.TryGetValue(x.hitName)}",
                .query = x.queryName,
                .subject = x.hitName,
                .weight = x.identities
        })

        If Not hitEdges.IsNullOrEmpty Then
            ' 生成蛋白质的节点模型
            Dim proteins As List(Of LDM.Protein) = hits _
                .Where(Function(x) Not x.isEmpty) _
                .Select(Function(x)
                            Return New BLAST.LDM.Protein With {
                    .Genome = locusDict.TryGetValue(x.queryName),
                    .LocusId = x.queryName
                }
                        End Function) _
                .AsList

            outProt = proteins + hits _
                .Where(Function(x) Not x.isEmpty) _
                .Select(Function(x)
                            Return New LDM.Protein With {
                            .Genome = locusDict.TryGetValue(x.hitName),
                            .LocusId = x.hitName
                       }
                        End Function)
            outHits = hitEdges
        Else
            outProt = New LDM.Protein() {}
            outHits = New LDM.Hit() {}
        End If
    End Sub

    <ExportAPI("MetaBuild.From.BBH", Info:="Network builder for the metagenomics BLAST result.")>
    Public Function MetaBuildFromBBH(<Parameter("in.DIR", "A directory which contains the bbh BLAST result datas between the fully paired genome combos.")>
                                     inDIR As String,
                                     locusDict As Dictionary(Of String, String)) As LDM.BLAST
        Dim source = (From file As String
                      In FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.csv").AsParallel
                      Let entry = LocalBLAST.Application.BatchParallel.LogNameParser(file)
                      Where entry.IsPairMatch
                      Select entry,
                          bbh = entry.FilePath.LoadCsv(Of BiDirectionalBesthit)).ToArray
        Dim proteins As New List(Of LDM.Protein)
        Dim hits As New List(Of LDM.Hit)

        For Each genome In source
            Dim getProt As LDM.Protein() = Nothing
            Dim getHits As LDM.Hit() = Nothing

            Call __buildFromBlastHits(genome.bbh, getProt, getHits, locusDict)
            Call proteins.AddRange(getProt)
            Call hits.AddRange(getHits)

            Call Console.Write(".")
        Next

        Return New LDM.BLAST With {
            .BlastHits = hits.ToArray,
            .Proteins = proteins.ToArray
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="hits"></param>
    ''' <param name="locusDict">现在是通过查找字典的方法来获取基因组的名称，这样子可以避免不必要的麻烦</param>
    ''' <returns></returns>
    <ExportAPI("Build.From.BBH"， Info:="Metagenome blast network builder")>
    <Extension>
    Public Function BuildFromBBH(hits As IEnumerable(Of BiDirectionalBesthit), locusDict As Dictionary(Of String, String)) As LDM.BLAST
        Return hits.__buildFromBlastHits(locusDict)
    End Function

    <ExportAPI("Build.From.BBH"， Info:="Metagenome blast network builder")>
    Public Function BuildFromBBH(source As String, locusDict As Dictionary(Of String, String)) As LDM.BLAST
        Dim bbh = source.LoadCsv(Of BiDirectionalBesthit)
        Return bbh.__buildFromBlastHits(locusDict)
    End Function

    <ExportAPI("Write.BLAST.Network",
               Info:="Save the build network data, which can be proceeded by the network visualization software Cytoscape or Gephi.")>
    Public Function SaveBLASTNetwork(<Parameter("BLAST.Network", "Generated network data for the metagenome blast result.")> blast As LDM.BLAST,
                                     <Parameter("DIR.save", "The network data saved location.")> saveDIR As String) As Boolean
        Return blast.Save(saveDIR, Encodings.UTF8)
    End Function
End Module
