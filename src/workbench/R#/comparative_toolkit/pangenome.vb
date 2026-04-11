#Region "Microsoft.VisualBasic::dff93b2bef8f827c271873b6a6871d6b, R#\comparative_toolkit\pangenome.vb"

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

'   Total Lines: 240
'    Code Lines: 182 (75.83%)
' Comment Lines: 13 (5.42%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 45 (18.75%)
'     File Size: 10.33 KB


' Module pangenome
' 
'     Function: analysis, build_context, pav_df, pav_table, report_html
'               set_ortho_group, set_sourceID, sv_df, sv_table
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.PanGenome
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' pan-genome analysis toolkit
''' </summary>
<Package("pangenome")>
<RTypeExport("pangenome", GetType(PanGenomeResult))>
<RTypeExport("ortho_groups", GetType(UnionFind))>
Module pangenome

    Sub Main()
        Call Converts.makeDataframe.addHandler(GetType(PAVTable()), AddressOf pav_df)
        Call Converts.makeDataframe.addHandler(GetType(SVTable()), AddressOf sv_df)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function sv_df(sv As SVTable(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .rownames = sv.Select(Function(a) a.SV_ID).ToArray,
            .columns = New Dictionary(Of String, Array)
        }

        Call df.add(NameOf(SVTable.Type), From svi As SVTable
                                          In sv
                                          Let label As String = svi.Type.ToString
                                          Select label)
        Call df.add(NameOf(SVTable.GenomeName), From svi As SVTable In sv Select svi.GenomeName)
        Call df.add(NameOf(SVTable.FamilyID), From svi As SVTable In sv Select svi.FamilyID)
        Call df.add(NameOf(SVTable.ClusterSize), From svi As SVTable In sv Select svi.ClusterSize)
        Call df.add(NameOf(SVTable.RelatedGenes), From svi As SVTable In sv Select svi.RelatedGenes.JoinBy("; "))

        Call df.add(NameOf(SVTable.Category), From svi As SVTable In sv Let label As String = svi.Category.ToString Select label)
        Call df.add(NameOf(SVTable.Dispensable), From svi As SVTable In sv Select svi.Dispensable)
        Call df.add(NameOf(SVTable.SingleCopyOrtholog), From svi As SVTable In sv Select svi.SingleCopyOrtholog)

        Call df.add(NameOf(SVTable.CopyNumber), From svi As SVTable In sv Select svi.CopyNumber)
        Call df.add(NameOf(SVTable.Median), From svi As SVTable In sv Select svi.Median)

        Call df.add(NameOf(SVTable.Description), From svi As SVTable In sv Select svi.Description)

        Return df
    End Function

    <RGenericOverloads("as.data.frame")>
    Private Function pav_df(pav As PAVTable(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .rownames = pav.Select(Function(a) a.FamilyID).ToArray,
            .columns = New Dictionary(Of String, Array)
        }
        Dim genome_names As String() = pav _
            .Select(Function(a) a.PAV.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim prefixPAV As Boolean = CLRVector.asScalarLogical(args.getBySynonyms("prefix.pav", "prefix_pav", "pav_prefix", "pav.prefix"))

        Call df.add("cluster_genes", From a As PAVTable In pav Select a.ClusterGenes.JoinBy("; "))
        Call df.add("size", From a As PAVTable In pav Select a.ClusterGenes.TryCount)

        Call df.add("category", From a As PAVTable
                                In pav
                                Let label As String = a.Category.ToString
                                Select label)
        Call df.add("dispensable", From a As PAVTable In pav Select a.Dispensable)
        Call df.add("singlecopy_ortholog", From a As PAVTable In pav Select a.SingleCopyOrtholog)

        For Each genome_name As String In genome_names
            Call df.add(If(prefixPAV, "PAV-" & genome_name, genome_name), From a As PAVTable In pav Select a(genome_name))
        Next

        Return df
    End Function

    ''' <summary>
    ''' Load the pangenome analysis context
    ''' </summary>
    ''' <param name="genomes">should be a collection of the genome GFF feature tables</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("build_context")>
    <RApiReturn(GetType(GenomeAnalyzer))>
    Public Function build_context(<RRawVectorArgument> genomes As Object,
                                  Optional soft_core_threshold As Double = 0.95,
                                  Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of GFFTable)(genomes, env, suppress:=True)
        Dim context As GenomeAnalyzer

        If Not pull.isError Then
            context = New GenomeAnalyzer(pull.populates(Of GFFTable)(env))
        ElseIf TypeOf genomes Is list Then
            context = New GenomeAnalyzer(DirectCast(genomes, list).AsGeneric(Of GeneTable())(env))
        Else
            Return pull.getError
        End If

        context.SoftCoreThreshold = soft_core_threshold

        Return context
    End Function

    <ExportAPI("analysis")>
    <RApiReturn(GetType(PanGenomeResult))>
    Public Function analysis(pangenome As GenomeAnalyzer, orthologSet As list, Optional env As Environment = Nothing) As Object
        Dim orthologDict As New Dictionary(Of String, BiDirectionalBesthit())
        Dim referenceMap As New Dictionary(Of String, RankTerm())

        For Each compareMap As String In orthologSet.slotKeys
            Dim maps_val As Object = orthologSet(compareMap)
            Dim cast As pipeline = pipeline.TryCreatePipeline(Of BiDirectionalBesthit)(maps_val, env, suppress:=True)

            If cast.isError Then
                cast = pipeline.TryCreatePipeline(Of RankTerm)(maps_val, env)

                If Not cast.isError Then
                    Call referenceMap.Add(compareMap, cast.populates(Of RankTerm)(env).ToArray)
                Else
                    Return cast.getError
                End If
            Else
                Dim linkSet As BiDirectionalBesthit() = cast.populates(Of BiDirectionalBesthit)(env) _
                    .Where(Function(link)
                               Return link.level <> Levels.NA AndAlso
                                    link.level <> Levels.SBH
                           End Function) _
                    .ToArray

                For i As Integer = 0 To linkSet.Length - 1
                    linkSet(i).QueryName = HeaderFormats.TrimAccessionVersion(linkSet(i).QueryName)
                    linkSet(i).HitName = HeaderFormats.TrimAccessionVersion(linkSet(i).HitName)
                Next

                Call orthologDict.Add(compareMap, linkSet)
            End If
        Next

        If Not referenceMap.IsNullOrEmpty Then
            Dim linkSet = OrthoGroupsHelper.BuildHomologyRelations(referenceMap) _
                .GroupBy(Function(a) $"{a.GenomeA}_vs_{a.GenomeB}") _
                .ToArray

            orthologDict = linkSet _
                .ToDictionary(Function(group) group.Key,
                              Function(group)
                                  Return (From link As HomologyPair
                                          In group
                                          Select link.CreateAlignmentHit).ToArray
                              End Function)
        End If

        Return pangenome.AnalyzePanGenome(orthologDict)
    End Function

    ''' <summary>
    ''' set orthology group for make gene family
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="uf"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("set_ortho_group")>
    Public Function set_ortho_group(<RRawVectorArgument> x As Object, uf As UnionFind, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of RankTerm)(x, env)

        If pull.isError Then
            Return pull.getError
        End If

        For Each gene As RankTerm In pull.populates(Of RankTerm)(env)
            Call uf.AddElement(gene.queryName)
            Call uf.AddElement(gene.term)
            Call uf.Union(referID:=gene.term, gene.queryName)
        Next

        Return uf
    End Function

    <ExportAPI("source_id")>
    Public Function set_sourceID(genome As GFFTable, source_name As String) As GFFTable
        genome.species = source_name
        Return genome
    End Function

    <ExportAPI("report_html")>
    Public Function report_html(result As PanGenomeResult) As String
        Return PanGenomeReportGenerator.GenerateReport(result, PanGenomeReportGenerator.DefaultHtmlTemplate)
    End Function

    <ExportAPI("sv_table")>
    Public Function sv_table(result As PanGenomeResult, Optional index As list = Nothing, Optional env As Environment = Nothing) As SVTable()
        Dim table = result.SVTable.ToArray

        If index Is Nothing Then
            Return table
        End If

        Dim idset As Dictionary(Of String, String()) = index.AsGeneric(Of String())(env)
        Dim subset = idset.GetClusters(table).ToArray
        Dim idc = idset.ToArray

        For i As Integer = 0 To subset.Length - 1
            subset(i).FamilyID = idc(i).Key
        Next

        Return subset
    End Function

    <ExportAPI("pav_table")>
    Public Function pav_table(result As PanGenomeResult, Optional index As list = Nothing, Optional env As Environment = Nothing) As PAVTable()
        Dim table = result.PAVTable.ToArray

        If index Is Nothing Then
            Return table
        End If

        Dim idset As Dictionary(Of String, String()) = index.AsGeneric(Of String())(env)
        Dim subset = idset.GetClusters(table).ToArray
        Dim idc = idset.ToArray

        For i As Integer = 0 To subset.Length - 1
            subset(i).FamilyID = idc(i).Key

            If subset(i).PAV Is Nothing Then
                subset(i).PAV = New Dictionary(Of String, Integer)
            End If
        Next

        Return subset
    End Function
End Module

