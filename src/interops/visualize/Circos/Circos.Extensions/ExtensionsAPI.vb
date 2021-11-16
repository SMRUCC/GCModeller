#Region "Microsoft.VisualBasic::c34689057f6e4d6d3104122b27d7e4fe, visualize\Circos\Circos.Extensions\ExtensionsAPI.vb"

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

    ' Module ExtensionsAPI
    ' 
    '     Function: CreateDeltaDiffPlots, FromRegulons, (+2 Overloads) FromVirtualFootprint
    ' 
    '     Sub: GeneratePhenotypeRegulations
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.InteractionModel.Regulon
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.Visualize.Circos.Documents.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights

Public Module ExtensionsAPI

    <ExportAPI("Phenotype.Regulation")>
    Public Sub GeneratePhenotypeRegulations(doc As Configurations.Circos,
                                        Regulations As IEnumerable(Of IRegulon),
                                        Pathways As IEnumerable(Of bGetObject.Pathway))
        Dim data As New PhenotypeRegulation(Regulations, Pathways)
        doc.SkeletonKaryotype = data
        Call doc.includes.Add(New Configurations.Ticks(Circos:=doc))
        Call doc.includes.Add(New Configurations.Ideogram(Circos:=doc))
    End Sub

    <ExportAPI("Karyotype.doc.DeltaDiff")>
    Public Function CreateDeltaDiffPlots(SequenceModel As IPolymerSequenceModel,
                                     <Parameter("SlideWindow.Size")> SlideWindowSize As Integer,
                                     steps As Integer) As NtProps.DeltaDiff
        Return New NtProps.DeltaDiff(SequenceModel, SlideWindowSize, steps)
    End Function

    Public Function FromVirtualFootprint(vf As PredictedRegulationFootprint, PTT As PTT) As Connection
        Dim Regulator = PTT(vf.Regulator)
        Dim Target = PTT(vf.ORF)

        If Regulator Is Nothing OrElse Target Is Nothing Then
            Return Nothing
        End If

        Return New Connection With {
                .From = Regulator.Location.Left,
                .To = Target.Location.Left
            }
    End Function

    Public Function FromVirtualFootprint(footprints As IEnumerable(Of PredictedRegulationFootprint),
                                                    PTT As PTT,
                                                    Optional selects As IEnumerable(Of Name) = Nothing) As Connection()

        If Not selects Is Nothing Then
            Dim filters = (From x As Name In selects Select x.Loci, x).ToArray
            footprints = (From x As PredictedRegulationFootprint
                              In footprints
                          Where PTT.ExistsLocusId(x.ORF) AndAlso
                                  PTT.ExistsLocusId(x.Regulator)
                          Select x).ToArray
            footprints = (From x In footprints
                          Let loci = PTT.GeneObject(x.Regulator).Location
                          Where Not (From f In filters
                                     Where f.Loci.Equals(loci, 10)
                                     Select f).FirstOrDefault Is Nothing
                          Select x).ToArray
        End If

        Dim LQuery = (From x As PredictedRegulationFootprint
                          In footprints.AsParallel
                      Where Not String.IsNullOrEmpty(x.Regulator)
                      Select FromVirtualFootprint(x, PTT)).ToArray
        LQuery = (From X In LQuery Where Not X.IsEmpty Select X).ToArray
        Return LQuery
    End Function

    Public Function FromRegulons(DIR As String, PTT As PTT, Optional requiredMaps As Boolean = False) As HighlightLabel
        Dim bbh = (From file As String
                   In FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.xml").AsParallel
                   Select file.LoadXml(Of BacteriaRegulome).regulome.regulators).ToArray.Unlist
        If requiredMaps Then
            Dim Maps As Dictionary(Of String, String) = PTT.LocusMaps
            For Each x As Regulator In bbh
                For Each gene As RegulatedGene In x.Regulates
                    If Maps.ContainsKey(gene.LocusId) Then
                        gene.LocusId = Maps(gene.LocusId)
                    End If
                Next
            Next
        End If
        Dim LQuery = (From x As Regulator
                      In bbh
                      Where Not String.IsNullOrEmpty(x.pathway)
                      Let locus As String() = x.Regulates.Select(Function(g) g.LocusId)
                      Let parts = ContinuouParts(locus)
                      Select parts.Select(Function(xx) New With {.func = x.pathway, .locus_tags = xx})).Unlist
        Dim trims = (From x In LQuery Where x.locus_tags.Length > 1 Select x Group x By x.func Into Group).ToArray
        LQuery = (From x In trims
                  Let locus As String() = x.Group.Select(Function(xx) xx.locus_tags).ToVector
                  Let parts = ContinuouParts(locus)
                  Select (From part As String()
                          In parts
                          Select New With {.func = x.func, .locus_tags = part})).Unlist
        Dim result = (From x In LQuery
                      Let g As GeneBrief = (From gg As String In x.locus_tags
                                            Where PTT.ExistsLocusId(gg)
                                            Select PTT(gg)).FirstOrDefault
                      Where Not g Is Nothing
                      Select g,
                          x.func
                      Group By g.Synonym Into Group).ToArray
        Dim dist As TextTrackData() =
            LinqAPI.Exec(Of TextTrackData) <= From x
                                              In result
                                              Let func As String() = (From s As String
                                                                      In x.Group.Select(Function(xx) xx.func.Split(";"c)).IteratesALL
                                                                      Select s
                                                                      Distinct).ToArray
                                              Let g As GeneBrief = x.Group.First.g
                                              Let txt As String = func.JoinBy(",,")
                                              Let h As TextTrackData = New TextTrackData With {
                                                  .start = CInt(g.Location.Left),
                                                  .end = CInt(g.Location.Right),
                                                  .text = Regex.Replace(txt, "\s+", "_")
                                              }
                                              Select h
        Return New HighlightLabel(dist)
    End Function
End Module
