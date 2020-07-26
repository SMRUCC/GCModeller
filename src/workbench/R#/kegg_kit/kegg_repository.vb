#Region "Microsoft.VisualBasic::c5a9b9d893da0e06a89173e49336013f, kegg_kit\kegg_repository.vb"

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

    ' Module kegg_repository
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: FetchKEGGOrganism, LoadCompoundRepo, loadMapRepository, LoadPathways, LoadReactionRepo
    '               ReadKEGGOrganism, readKEGGpathway, SaveKEGGOrganism, SaveKEGGPathway, showTable
    '               TableOfReactions
    ' 
    ' Enum OrganismTypes
    ' 
    '     all, eukaryotes, prokaryote
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime.Internal
Imports Microsoft.VisualBasic.Text.Xml.Models

''' <summary>
''' 
''' </summary>
<Package("kegg.repository", Category:=APICategories.SoftwareTools)>
Public Module kegg_repository

    Sub New()
        Call REnv.ConsolePrinter.AttachConsoleFormatter(Of ReactionTable())(AddressOf showTable)
    End Sub

    Private Function showTable(table As ReactionTable()) As String
        Return table.Take(6).ToCsvDoc.AsMatrix.Print()
    End Function

    ''' <summary>
    ''' load repository of kegg <see cref="Compound"/>.
    ''' </summary>
    ''' <param name="repository"></param>
    ''' <returns></returns>
    <ExportAPI("load.compounds")>
    Public Function LoadCompoundRepo(repository As String) As CompoundRepository
        Return CompoundRepository.ScanModels(repository, ignoreGlycan:=False)
    End Function

    ''' <summary>
    ''' load reaction data from the given kegg reaction data repository
    ''' </summary>
    ''' <param name="repository"></param>
    ''' <returns></returns>
    <ExportAPI("load.reactions")>
    Public Function LoadReactionRepo(repository As String) As ReactionRepository
        Return ReactionRepository.LoadAuto(repository)
    End Function

    ''' <summary>
    ''' load list of kegg reference <see cref="Map"/>.
    ''' </summary>
    ''' <param name="repository">
    ''' a directory of repository data for kegg reference <see cref="Map"/>.
    ''' </param>
    ''' <returns>
    ''' a kegg reference map object vector, which can be indexed 
    ''' via <see cref="Map.id"/>.
    ''' </returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("load.maps")>
    <RApiReturn(GetType(Map), GetType(MapRepository))>
    Public Function loadMapRepository(repository As String, Optional rawMaps As Boolean = True) As Object
        If rawMaps Then
            Return MapRepository.GetMapsAuto(repository).ToArray
        Else
            Return MapRepository.BuildRepository(repository)
        End If
    End Function

    ''' <summary>
    ''' load kegg pathway maps from a given repository data directory.
    ''' </summary>
    ''' <param name="repository"></param>
    ''' <returns></returns>
    <ExportAPI("load.pathways")>
    Public Function LoadPathways(repository As String) As PathwayMap()
        Dim maps = ls - l - r - "*.Xml" <= repository
        Dim pathwayMaps As PathwayMap() = maps _
            .Select(AddressOf LoadXml(Of PathwayMap)) _
            .ToArray

        Return pathwayMaps
    End Function

    ''' <summary>
    ''' load kegg reaction table from a given repository model or resource file on your filesystem.
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("reactions.table")>
    Public Function TableOfReactions(repo As Object, Optional env As Environment = Nothing) As Object
        If repo Is Nothing Then
            Return Nothing
        ElseIf repo.GetType Is GetType(String) Then
            Dim resource As String = DirectCast(repo, String)

            If resource.DirectoryExists Then
                Return ReactionTable.Load(resource).ToArray
            ElseIf resource.FileExists Then
                Return resource.LoadCsv(Of ReactionTable).ToArray
            Else
                Return REnv.debug.stop({
                    "invalid resource handle for load kegg reaction table!",
                    "resource: " & resource
                }, env)
            End If
        ElseIf repo.GetType Is GetType(ReactionRepository) Then
            Return ReactionTable.Load(DirectCast(repo, ReactionRepository)).ToArray
        Else
            Return REnv.debug.stop(New InvalidConstraintException(repo.GetType.FullName), env)
        End If
    End Function

    ''' <summary>
    ''' Fetch the kegg organism table data from a given resource
    ''' </summary>
    ''' <param name="resource">the kegg organism data resource, by default is the kegg online page data.</param>
    ''' <param name="type">
    ''' 0. all
    ''' 1. prokaryote
    ''' 2. eukaryotes
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("fetch.kegg_organism")>
    Public Function FetchKEGGOrganism(Optional resource$ = "http://www.kegg.jp/kegg/catalog/org_list.html",
                                      Optional type As OrganismTypes = OrganismTypes.all) As Prokaryote()

        Dim result As KEGGOrganism = If(resource.StringEmpty, EntryAPI.GetOrganismListFromResource(), EntryAPI.FromResource(resource))
        Dim eukaryotes As List(Of Prokaryote) = result.Eukaryotes _
            .Select(Function(x)
                        Return New Prokaryote(x)
                    End Function) _
            .AsList

        If type = OrganismTypes.all Then
            Return result.Prokaryote + eukaryotes
        ElseIf type = OrganismTypes.prokaryote Then
            Return result.Prokaryote
        ElseIf type = OrganismTypes.eukaryotes Then
            Return eukaryotes
        Else
            Return {}
        End If
    End Function

    ''' <summary>
    ''' save the kegg pathway annotation result data.
    ''' </summary>
    ''' <param name="pathway"></param>
    ''' <param name="file$"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("save.KEGG_pathway")>
    Public Function SaveKEGGPathway(<RRawVectorArgument> pathway As Object, file$, Optional env As Environment = Nothing) As Object
        Dim pathwayCollection As REnv.Object.pipeline = REnv.Object.pipeline.TryCreatePipeline(Of Pathway)(pathway, env)

        If pathwayCollection.isError Then
            Return pathwayCollection.getError
        End If

        Return New XmlList(Of Pathway) With {
            .items = pathwayCollection _
                .populates(Of Pathway)(env) _
                .ToArray
        }.GetXml _
         .SaveTo(file)
    End Function

    ''' <summary>
    ''' read the kegg pathway annotation result data.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.KEGG_pathway")>
    <RApiReturn(GetType(Pathway))>
    Public Function readKEGGpathway(file As String, Optional env As Environment = Nothing) As Object
        If Not file.FileExists Then
            Return Internal.debug.stop({"the given file is not exists on your filesystem!", $"file: " & file}, env)
        Else
            Return file.LoadXml(Of XmlList(Of Pathway)).items
        End If
    End Function

    ''' <summary>
    ''' save the kegg organism data as data table file.
    ''' </summary>
    ''' <param name="organism"></param>
    ''' <param name="file$"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("save.kegg_organism")>
    Public Function SaveKEGGOrganism(organism As Prokaryote(), file$, Optional env As Environment = Nothing) As Boolean
        Return organism.SaveTo(file, silent:=Not env.globalEnvironment.options.verbose)
    End Function

    <ExportAPI("read.kegg_organism")>
    Public Function ReadKEGGOrganism(file As String) As Prokaryote()
        Return file.LoadCsv(Of Prokaryote)
    End Function
End Module

Public Enum OrganismTypes
    all
    prokaryote
    eukaryotes
End Enum
