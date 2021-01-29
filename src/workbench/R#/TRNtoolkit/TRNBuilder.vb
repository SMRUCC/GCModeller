﻿#Region "Microsoft.VisualBasic::8b3c7c241c7325361dfa30b49022f57e, phenotype_kit\TRNBuilder.vb"

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

' Module TRNBuilder
' 
'     Function: exportRegPrecise, ParsePromoterReport, readFootprintSites, readRegPrecise, readRegulations
'               RegulationFootprint, writeRegulationFootprints
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' tools for create a transcription regulation network
''' </summary>
<Package("TRN.builder")>
Module TRNBuilder

    <ExportAPI("as.promoter.models")>
    Public Function ParsePromoterReport(text As String) As GeneReport()
        Return ReportParser.ParseReport(text).ToArray
    End Function

    ''' <summary>
    ''' read a footprint site model data file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.footprints")>
    Public Function readFootprintSites(file As String) As FootprintSite()
        Return file.LoadCsv(Of FootprintSite)
    End Function

    ''' <summary>
    ''' read a regulation prediction result file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.regulations")>
    Public Function readRegulations(file As String) As RegulationFootprint()
        Return file.LoadCsv(Of RegulationFootprint)
    End Function

    ''' <summary>
    ''' save the regulation network data file.
    ''' </summary>
    ''' <param name="regulationFootprints"></param>
    ''' <param name="file$"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("write.regulations")>
    Public Function writeRegulationFootprints(regulationFootprints As Object, file$, Optional env As Environment = Nothing) As Object
        If regulationFootprints Is Nothing Then
            Return Internal.debug.stop("no content data provides!", env)
        ElseIf file.StringEmpty Then
            Return Internal.debug.stop("no file write information provides!", env)
        End If

        If TypeOf regulationFootprints Is RegulationFootprint() Then
            Return DirectCast(regulationFootprints, RegulationFootprint()).SaveTo(file)
        ElseIf TypeOf regulationFootprints Is pipeline AndAlso DirectCast(regulationFootprints, pipeline).elementType Like GetType(RegulationFootprint) Then
            Using writer As New WriteStream(Of RegulationFootprint)(file)
                For Each edge As RegulationFootprint In DirectCast(regulationFootprints, pipeline).populates(Of RegulationFootprint)(env)
                    Call writer.Flush(edge)
                Next
            End Using

            Return True
        Else
            Return Internal.debug.stop($"invalid data type for write: {regulationFootprints.GetType.FullName }", env)
        End If
    End Function

    <ExportAPI("TRN")>
    Public Function TRN(<RRawVectorArgument> factors As Object, familySites As list, Optional env As Environment = Nothing) As Object
        Dim TF As pipeline = pipeline.TryCreatePipeline(Of RegpreciseBBH)(factors, env)

        If TF.isError Then
            Return TF.getError
        End If

        Dim familyIndex = TF.populates(Of RegpreciseBBH)(env) _
            .Select(Function(r)
                        Return r.family.Split("/"c).Select(Function(family) (family, r))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(r) r.family.ToLower) _
            .ToDictionary(Function(family) family.Key,
                          Function(list)
                              Return list.Select(Function(r) r.r).ToArray
                          End Function)
        Dim g As New NetworkGraph
        Dim tfbs As String()

        For Each geneId As String In familySites.getNames
            Call g.CreateNode(geneId)
        Next

        For Each reg In familyIndex.Values.IteratesALL
            If g.GetElementByID(reg.QueryName) Is Nothing Then
                Call g.CreateNode(reg.QueryName)
            End If
        Next

        Dim regData As EdgeData

        For Each geneId As String In familySites.getNames
            tfbs = familySites.getValue(geneId, env, New String() {})

            For Each family As String In tfbs.Select(AddressOf Strings.LCase)
                If familyIndex.ContainsKey(family) Then
                    For Each reg As RegpreciseBBH In familyIndex(family)
                        regData = New EdgeData With {.label = $"{reg.QueryName} -> {geneId}"}
                        regData(NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE) = "regulates"

                        g.CreateEdge(reg.QueryName, geneId, data:=regData)
                        g.GetElementByID(reg.QueryName).data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = reg.family
                        g.GetElementByID(reg.QueryName).data.label = reg.geneName
                    Next
                End If
            Next
        Next

        Return g
    End Function

    <ExportAPI("regulation.footprint")>
    Public Function RegulationFootprint(<RRawVectorArgument>
                                        regulators As Object,
                                        motifLocis As FootprintSite(),
                                        regprecise As TranscriptionFactors,
                                        Optional env As Environment = Nothing) As pipeline
        If regulators Is Nothing Then
            Return Nothing
        End If

        Dim regulatorMaps As BestHit()

        If TypeOf regulators Is BestHit() Then
            regulatorMaps = DirectCast(regulators, BestHit())
        ElseIf TypeOf regulators Is pipeline AndAlso DirectCast(regulators, pipeline).elementType Like GetType(BestHit) Then
            regulatorMaps = DirectCast(regulators, pipeline) _
                .populates(Of BestHit)(env) _
                .ToArray
        Else
            Return Internal.debug.stop($"invalid regulator maps: '{regulators.GetType.FullName }'!", env)
        End If

        Dim regulatorTable As New Dictionary(Of String, List(Of (genome As BacteriaRegulome, Regulator)))
        Dim family$
        Dim regulatorMapTable As Dictionary(Of String, BestHit()) = regulatorMaps _
            .GroupBy(Function(map)
                         Return map.HitName.Split(":"c).Last
                     End Function) _
            .ToDictionary(Function(hit) hit.Key,
                          Function(group)
                              Return group.ToArray
                          End Function)

        For Each genome As BacteriaRegulome In regprecise.AsEnumerable
            For Each regulon As Regulator In genome.regulome _
                .AsEnumerable _
                .Where(Function(reg)
                           Return reg.type = Types.TF
                       End Function)

                family = regulon.family _
                    .Split("/"c, "\"c) _
                    .First

                If Not regulatorTable.ContainsKey(family) Then
                    regulatorTable.Add(family, New List(Of (genome As BacteriaRegulome, Regulator)))
                End If

                regulatorTable(family).Add((genome, regulon))
            Next
        Next

        Return Iterator Function() As IEnumerable(Of RegulationFootprint)
                   Dim regulatorList As List(Of (BacteriaRegulome, Regulator))
                   Dim regulation As RegulationFootprint
                   Dim edgeKeyIndex As New Index(Of String)
                   Dim edgeKey$

                   For Each gene As FootprintSite In motifLocis
                       For Each familyName As String In gene.src
                           regulatorList = regulatorTable(familyName)

                           For Each regulator As (genome As BacteriaRegulome, reg As Regulator) In regulatorList _
                               .Where(Function(reg)
                                          Return regulatorMapTable.ContainsKey(reg.Item2.LocusId)
                                      End Function)

                               For Each hit As BestHit In regulatorMapTable(regulator.reg.LocusId)
                                   edgeKey = $"{hit.QueryName}->{gene.gene}"

                                   If edgeKey Like edgeKeyIndex Then
                                       Continue For
                                   Else
                                       edgeKeyIndex.Add(edgeKey)
                                   End If

                                   regulation = New RegulationFootprint With {
                                       .family = familyName,
                                       .effector = regulator.reg.effector,
                                       .regulator = hit.QueryName,
                                       .biological_process = regulator.reg.biological_process.JoinBy(", "),
                                       .mode = regulator.reg.regulationMode,
                                       .regprecise = regulator.reg.regulog.name,
                                       .regulog = regulator.reg.regulog.name,
                                       .regulated = gene.gene,
                                       .species = regulator.genome.genome.name,
                                       .identities = hit.identities
                                   }

                                   Yield regulation
                               Next
                           Next
                       Next
                   Next
               End Function() _
 _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function
End Module
