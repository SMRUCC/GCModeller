#Region "Microsoft.VisualBasic::8c08f1d7e49bb535bb78a7e4639708c1, CLI_tools\MEME\Cli\Views + Stats\Regulations.vb"

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

    ' Module CLI
    ' 
    '     Function: PathwayRegulations
    ' 
    '     Sub: vennSaveCommon
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("--pathway.regulates",
               Info:="Associates of the pathway regulation information for the predicted virtual footprint information.",
               Usage:="--pathway.regulates -footprints <virtualfootprint.csv> /pathway <DIR.KEGG.Pathways> [/out <./PathwayRegulations/>]")>
    Public Function PathwayRegulations(args As CommandLine) As Integer
        Dim footprintTable$ = args("-footprints")
        Dim Footprints = footprintTable.LoadCsv(Of PredictedRegulationFootprint)
        Dim Pathways = FileIO.FileSystem.GetFiles(args("/pathway"), FileIO.SearchOption.SearchAllSubDirectories, "*.xml") _
            .Select(Function(file) file.LoadXml(Of bGetObject.Pathway))
        Dim PathwayBrites = BriteHEntry.Pathway.LoadFromResource.ToDictionary(Function(entry) entry.EntryId)
        Dim outDIR As String = args.GetValue("/out", footprintTable.TrimSuffix)
        Dim modRegulators As Dictionary(Of String, List(Of String)) =
            New Dictionary(Of String, List(Of String))

        Footprints = (From x In Footprints
                      Where Not String.IsNullOrEmpty(x.Regulator)
                      Select x).AsList

        For Each pathway As bGetObject.Pathway In Pathways
            Dim pwyBrite As BriteHEntry.Pathway = PathwayBrites(pathway.briteID)
            Dim savePath As String = $"{outDIR}/{pwyBrite.GetPathCategory}"
            Dim pathwayGenes As String() = pathway.GetPathwayGenes
            Dim doc = (From vf As PredictedRegulationFootprint
                       In Footprints.AsParallel
                       Where Array.IndexOf(pathwayGenes, vf.ORF) > -1
                       Select vf).ToArray
            If Not doc.IsNullOrEmpty Then
                Call doc.SaveTo(savePath & $"/{pathway.EntryId}.csv")
            End If

            For Each obj In doc
                If InStr(obj.Type, pwyBrite.Class) = 0 Then
                    obj.Type &= "; " & pwyBrite.Class
                End If
                If InStr(obj.Type, pwyBrite.Category) = 0 Then
                    obj.Class &= "; " & pwyBrite.Category
                End If
                If InStr(obj.Category, pwyBrite.EntryId) = 0 Then
                    obj.Category &= "; " & pwyBrite.EntryId
                End If
            Next

            Dim regulators As New List(Of String)
            If Not modRegulators.ContainsKey(pwyBrite.Class) Then
                regulators = New List(Of String)
                Call modRegulators.Add(pwyBrite.Class, regulators)
            End If

            regulators += doc.Where(Function(r) Not String.IsNullOrEmpty(r.Regulator)) _
                .Select(Function(r) r.Regulator)

        Next

        For Each type In modRegulators.ToArray
            Dim lst = modRegulators(type.Key).Distinct.AsList
            If lst.IsNullOrEmpty OrElse lst.IsNullOrEmpty Then
                Call modRegulators.Remove(type.Key)
            Else
                modRegulators(type.Key) = lst
            End If
        Next

        ' 求所有代谢途径之中都出现的核心的调控因子
        Dim sets = modRegulators.Select(Function(x) New [Set](x.Value))
        Dim core As [Set] = sets.First

        For Each [set] As [Set] In sets
            core = core And [set]
        Next

        Dim coreRegulators = core.ToArray.Select(Function(x) Scripting.ToString(x))
        Call coreRegulators.FlushAllLines(
            outDIR & "/Core.Regulators.txt", Encodings.ASCII)
        Dim coreRegulations = (From regulate In Footprints.AsParallel
                               Where Not String.IsNullOrEmpty(regulate.Regulator) AndAlso
                                   Array.IndexOf(coreRegulators, regulate.Regulator) > -1
                               Select regulate).ToArray
        Call coreRegulations.SaveTo(outDIR & "/Core.Regulations.csv")

        coreRegulations = (From regulate In coreRegulations
                           Where regulate.Pcc >= 0.8 OrElse
                               regulate.Pcc <= -0.6
                           Select regulate).ToArray
        '' 导出网络和MEME进一步分析所需要的序列

        'Dim nodes As Dictionary(Of String, FileStream.Node) =
        '    coreRegulations.ToArray(
        '        Function(x) x.ORF).Distinct.ToArray(
        '        Function(id) New FileStream.Node With {
        '            .Identifier = id,
        '            .NodeType = "ORF"}).ToDictionary(Function(x) x.Identifier)

        'For Each regulator As String In coreRegulations.Select(Function(x) x.Regulator).Distinct
        '    If nodes.ContainsKey(regulator) Then
        '        nodes(regulator).NodeType &= "; TF"
        '    Else
        '        nodes.Add(regulator,
        '                  New FileStream.Node With {
        '                     .Identifier = regulator,
        '                     .NodeType = "TF"})
        '    End If
        'Next

        'Dim cytoscape = CytoscapeGraphView.Serialization.Export(nodes.Values.ToArray, coreRegulations)
        'Call cytoscape.Save(outDIR & "/Core-regulation.xml")

        ' 导出meme数据
        Dim Promotes = (From regu As PredictedRegulationFootprint
                        In coreRegulations
                        Where regu.Pcc > 0
                        Select regu
                        Group regu By regu.Regulator Into Group).ToArray
        For Each regulatss In Promotes
            Dim fa = regulatss.Group.Select(Function(x, idx) New FastaSeq With {.Headers = {x.ORF & "_" & idx}, .SequenceData = x.Sequence})
            Dim path As String = outDIR & $"/MEME/UP/fa/{regulatss.Regulator}.fasta"
            Call New FastaFile(fa).Save(path)
        Next

        Promotes = (From regu In coreRegulations Where regu.Pcc < 0 Select regu Group regu By regu.Regulator Into Group).ToArray
        For Each regulatss In Promotes
            Dim fa = regulatss.Group.Select(Function(x, idx) New FastaSeq With {.Headers = {x.ORF & "_" & idx}, .SequenceData = x.Sequence})
            Dim path As String = outDIR & $"/MEME/Supress/fa/{regulatss.Regulator}.fasta"
            Call New FastaFile(fa).Save(path)
        Next

        Dim venn = __modsVenn(modRegulators, modRegulators.Keys.ToArray)
        venn.Title = "KEGG Modules Regulations Compares"
        venn.saveTiff = outDIR & "/kMod.venn.tiff"
        venn.SaveTo(outDIR & "/kMod.venn.R")
        Call vennSaveCommon(outDIR & "/ModsRegulatorViews.csv", modRegulators)

        Return 0
    End Function

    Private Sub vennSaveCommon(saveCsv As String, modRegulators As Dictionary(Of String, List(Of String)))
        Dim modsRegulatorsView As IO.File =
            New IO.File + {"locusId"}.Join(modRegulators.Keys)
        Dim union As String() = modRegulators _
            .Select(Function(x) x.Value) _
            .IteratesALL _
            .Distinct _
            .OrderBy(Function(sId) sId).ToArray

        For Each sId As String In union
            Dim row As New List(Of String) From {sId}
            For Each modPwy In modRegulators
                If modPwy.Value.IndexOf(sId) > -1 Then
                    Call row.Add(sId)
                Else
                    Call row.Add("-")
                End If
            Next
            Call modsRegulatorsView.Add(row)
        Next

        Call modsRegulatorsView.Save(saveCsv, Encoding:=Encoding.ASCII)
    End Sub
End Module
