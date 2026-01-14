#Region "Microsoft.VisualBasic::74bb28df70462b87d6eefae558c36d94, R#\kegg_kit\repository.vb"

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

    '   Total Lines: 918
    '    Code Lines: 667 (72.66%)
    ' Comment Lines: 160 (17.43%)
    '    - Xml Docs: 96.25%
    ' 
    '   Blank Lines: 91 (9.91%)
    '     File Size: 39.31 KB


    ' Module repository
    ' 
    '     Function: createCompound, FetchKEGGOrganism, getCompoundsId, getEnzymeClassDescription, getReactionsId
    '               index, keggMap, LoadCompoundRepo, loadMapRepository, LoadPathways
    '               loadReactionClassRaw, loadReactionClassTable, LoadReactionRepo, parseMapsFromFile, pathway
    '               reaction, reaction_class, readKeggCompounds, readKeggMaps, ReadKEGGOrganism
    '               readKEGGpathway, readKeggReactions, SaveKEGGOrganism, SaveKEGGPathway, shapeAreas
    '               showMapTable, showTable, TableOfReactions, writeKeggCompounds, writeKeggCompoundSet
    '               writeKeggMaps, writeKeggMapSet, writeKeggReactions, writeKeggReactionSet, writeMessagePack
    ' 
    '     Sub: Main
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

Imports System.Data
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.KEGG.Metabolism
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports any = Microsoft.VisualBasic.Scripting
Imports dataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' The kegg data model repository tool
''' </summary>
<Package("repository", Category:=APICategories.SoftwareTools)>
<RTypeExport("kegg_pathway", GetType(Pathway))>
<RTypeExport("kegg_reaction", GetType(Reaction))>
<RTypeExport("kegg_compound", GetType(Compound))>
<RTypeExport("kegg_map", GetType(Map))>
Public Module repository

    Friend Sub Main()
        Call RInternal.ConsolePrinter.AttachConsoleFormatter(Of ReactionTable())(AddressOf showTable)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(Map()), AddressOf showMapTable)

        ' data files
        Call RInternal.generic.add("writeBin", GetType(CompoundRepository), AddressOf writeKeggCompounds)
        Call RInternal.generic.add("writeBin", GetType(MapRepository), AddressOf writeKeggMaps)
        Call RInternal.generic.add("writeBin", GetType(ReactionRepository), AddressOf writeKeggReactions)

        Call RInternal.generic.add("writeBin", GetType(Compound()), AddressOf writeKeggCompoundSet)
        Call RInternal.generic.add("writeBin", GetType(Map()), AddressOf writeKeggMapSet)
        Call RInternal.generic.add("writeBin", GetType(Reaction()), AddressOf writeKeggReactionSet)

        Call RInternal.generic.add("readBin.kegg_compound", GetType(Stream), AddressOf readKeggCompounds)
        Call RInternal.generic.add("readBin.kegg_reaction", GetType(Stream), AddressOf readKeggReactions)
        Call RInternal.generic.add("readBin.kegg_map", GetType(Stream), AddressOf readKeggMaps)
    End Sub

    ''' <summary>
    ''' read kegg compound from messagepack file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Private Function readKeggCompounds(file As Stream, args As list, env As Environment) As Object
        Return KEGGCompoundPack.ReadKeggDb(file)
    End Function

    Private Function readKeggMaps(file As Stream, args As list, env As Environment) As Object
        Return KEGGMapPack.ReadKeggDb(file)
    End Function

    Private Function readKeggReactions(file As Stream, args As list, env As Environment) As Object
        Return KEGGReactionPack.ReadKeggDb(file)
    End Function

    <RGenericOverloads("writeBin")>
    Private Function writeKeggCompounds(compounds As CompoundRepository, args As list, env As Environment) As Object
        Dim stream As Stream = args!con

        Return compounds.Compounds _
            .Select(Function(c) c.Entity) _
            .DoCall(Function(ls)
                        Return KEGGCompoundPack.WriteKeggDb(ls, stream)
                    End Function)
    End Function

    <RGenericOverloads("writeBin")>
    Private Function writeKeggMaps(maps As MapRepository, args As list, env As Environment) As Object
        Return KEGGMapPack.WriteKeggDb(maps.AsEnumerable, args!con)
    End Function

    <RGenericOverloads("writeBin")>
    Private Function writeKeggReactions(reactions As ReactionRepository, args As list, env As Environment) As Object
        Return KEGGReactionPack.WriteKeggDb(reactions.metabolicNetwork, args!con)
    End Function

    <RGenericOverloads("writeBin")>
    Private Function writeKeggCompoundSet(compounds As Compound(), args As list, env As Environment) As Object
        Return KEGGCompoundPack.WriteKeggDb(compounds, args!con)
    End Function

    <RGenericOverloads("writeBin")>
    Private Function writeKeggMapSet(maps As Map(), args As list, env As Environment) As Object
        Return KEGGMapPack.WriteKeggDb(maps, args!con)
    End Function

    <RGenericOverloads("writeBin")>
    Private Function writeKeggReactionSet(reactions As Reaction(), args As list, env As Environment) As Object
        Return KEGGReactionPack.WriteKeggDb(reactions, args!con)
    End Function

    Private Function showTable(table As ReactionTable()) As String
        Return table.Take(6).ToCsvDoc.AsMatrix.Print()
    End Function

    Private Function showMapTable(table As Map(), args As list, env As Environment) As dataframe
        Dim mapTable As New dataframe With {.columns = New Dictionary(Of String, Array)}

        mapTable.columns("id") = table.Select(Function(t) t.EntryId).ToArray
        mapTable.columns("Name") = table.Select(Function(t) t.name.TrimNewLine).ToArray
        mapTable.columns(NameOf(Map.URL)) = table.Select(Function(t) t.URL).ToArray
        mapTable.columns(NameOf(Map.description)) = table.Select(Function(t) t.description.TrimNewLine).ToArray
        mapTable.columns(NameOf(Map.shapes)) = table.Select(Function(t) t.shapes.mapdata.TryCount).ToArray

        Return mapTable
    End Function

    ''' <summary>
    ''' Get the class description category data for EC number
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("enzyme_description")>
    <RApiReturn(TypeCodes.list)>
    Public Function getEnzymeClassDescription() As Object
        Return New list With {
            .slots = ECNumberReader.rootNames _
                .ToDictionary(Function(e) e.Key,
                              Function(e)
                                  Return CObj(e.Value)
                              End Function)
        }
    End Function

    ''' <summary>
    ''' a generic method for write kegg data stream as messagepack 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("write.msgpack")>
    <RApiReturn(GetType(Boolean))>
    Public Function writeMessagePack(<RRawVectorArgument> data As Object, file As Object, Optional env As Environment = Nothing) As Object
        Dim f = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)
        Dim reader As pipeline
        Dim stream As Stream

        If f Like GetType(Message) Then
            Return f.TryCast(Of Message)
        Else
            stream = f.TryCast(Of Stream)
        End If

        If TypeOf data Is CompoundRepository Then
            Return writeKeggCompounds(data, New list(("con", stream)), env)
        ElseIf TypeOf data Is MapRepository Then
            Return writeKeggMaps(data, New list(("con", stream)), env)
        ElseIf TypeOf data Is ReactionRepository Then
            Return writeKeggReactions(data, New list(("con", stream)), env)
        End If

        reader = pipeline.TryCreatePipeline(Of Map)(data, env, suppress:=True)

        If Not reader.isError Then
            Return KEGGMapPack.WriteKeggDb(reader.populates(Of Map)(env), stream)
        Else
            reader = pipeline.TryCreatePipeline(Of ReactionClass)(data, env, suppress:=True)
        End If

        If Not reader.isError Then
            Return ReactionClassPack.WriteKeggDb(reader.populates(Of ReactionClass)(env), stream)
        Else
            reader = pipeline.TryCreatePipeline(Of Compound)(data, env, suppress:=True)
        End If

        If Not reader.isError Then
            Return KEGGCompoundPack.WriteKeggDb(reader.populates(Of Compound)(env), stream)
        Else
            reader = pipeline.TryCreatePipeline(Of Pathway)(data, env, suppress:=True)
        End If

        If Not reader.isError Then
            Return KEGGPathwayPack.WriteKeggDb(reader.populates(Of Pathway)(env), stream)
        Else
            reader = pipeline.TryCreatePipeline(Of Reaction)(data, env, suppress:=True)
        End If

        If Not reader.isError Then
            Return KEGGReactionPack.WriteKeggDb(reader.populates(Of Reaction)(env), stream)
        End If

        Return reader.getError
    End Function

    <ExportAPI("index")>
    Public Function index(<RRawVectorArgument> repo As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of Compound)(repo, env)

        If Not pull.isError Then
            Return New CompoundRepository(pull.populates(Of Compound)(env))
        End If

        Return pull.getError
    End Function

    ''' <summary>
    ''' load repository of kegg <see cref="Compound"/>.
    ''' </summary>
    ''' <param name="repository"></param>
    ''' <returns></returns>
    <ExportAPI("load.compounds")>
    <RApiReturn(GetType(CompoundRepository), GetType(Compound))>
    Public Function LoadCompoundRepo(<RRawVectorArgument>
                                     repository As Object,
                                     Optional rawList As Boolean = False,
                                     Optional ignoreGlycan As Boolean = False,
                                     Optional env As Environment = Nothing) As Object

        If TypeOf repository Is Stream Then
            Using file As Stream = DirectCast(repository, Stream)
                If rawList Then
                    Return KEGGCompoundPack.ReadKeggDb(file)
                Else
                    Return New CompoundRepository(KEGGCompoundPack.ReadKeggDb(file))
                End If
            End Using
        Else
            Dim dataRepo As String() = pipeline _
                .TryCreatePipeline(Of String)(repository, env) _
                .populates(Of String)(env) _
                .ToArray

            If dataRepo.Length = 1 AndAlso dataRepo(Scan0).ExtensionSuffix("msgpack") Then
                Using file As Stream = dataRepo(Scan0).Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                    If rawList Then
                        Return KEGGCompoundPack.ReadKeggDb(file)
                    Else
                        Return New CompoundRepository(KEGGCompoundPack.ReadKeggDb(file))
                    End If
                End Using
            ElseIf rawList Then
                Return dataRepo _
                    .Select(Function(dir)
                                Return CompoundRepository.ScanRepository(directory:=dir, ignoreGlycan:=ignoreGlycan)
                            End Function) _
                    .IteratesALL _
                    .GroupBy(Function(c) c.entry) _
                    .Select(Function(c) c.First) _
                    .ToArray
            Else
                Return CompoundRepository.ScanModels(
                    directories:=dataRepo,
                    ignoreGlycan:=ignoreGlycan
                )
            End If
        End If
    End Function

    ''' <summary>
    ''' ### load kegg reaction data repository
    ''' 
    ''' load reaction data from the given kegg reaction data 
    ''' repository.
    ''' </summary>
    ''' <param name="repository">Could be a data pack file or a directory
    ''' path that contains multiple reaction model data files.
    ''' </param>
    ''' <param name="raw">
    ''' this function will just returns a vector of the kegg reaction data if
    ''' this parameter value is set to TRUE.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("load.reactions")>
    <RApiReturn(GetType(ReactionRepository), GetType(Reaction))>
    Public Function LoadReactionRepo(<RRawVectorArgument>
                                     repository As Object,
                                     Optional raw As Boolean = True,
                                     Optional env As Environment = Nothing) As Object

        If TypeOf repository Is Stream Then
            Using file As Stream = DirectCast(repository, Stream)
                Return KEGGReactionPack.ReadKeggDb(file)
            End Using
        Else
            Dim resource As String() = CLRVector.asCharacter(repository)

            If resource.Length = 1 Then
                Dim handle As String = resource(Scan0)

                If handle.DirectoryExists Then
                    Return ReactionRepository.LoadAuto(handle)
                ElseIf handle.ExtensionSuffix("msgpack") Then
                    If raw Then
                        Return KEGGReactionPack.ReadKeggDb(handle)
                    Else
                        Return ReactionRepository.LoadFromList(KEGGReactionPack.ReadKeggDb(handle))
                    End If
                Else
                    Return handle.LoadXml(Of Reaction)
                End If
            Else
                Return resource _
                    .Select(AddressOf LoadXml(Of Reaction)) _
                    .ToArray
            End If
        End If
    End Function

    ''' <summary>
    ''' load maps data from a file bundle pack or scan from a directory
    ''' </summary>
    ''' <param name="fileHandle"></param>
    ''' <param name="rawMaps"></param>
    ''' <returns></returns>
    Private Function parseMapsFromFile(fileHandle As String, rawMaps As Boolean) As Object
        If fileHandle.ExtensionSuffix("msgpack", "pack", "hds") Then
            Using file As Stream = fileHandle.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                If rawMaps Then
                    Return KEGGMapPack.ReadKeggDb(file)
                Else
                    Return KEGGMapPack _
                        .ReadKeggDb(file) _
                        .DoCall(AddressOf MapRepository.BuildRepository)
                End If
            End Using
        ElseIf fileHandle.ExtensionSuffix("xml") Then
            Return fileHandle.LoadXml(Of Map)
        End If

        If rawMaps Then
            Return MapRepository.GetMapsAuto(fileHandle).ToArray
        Else
            Return MapRepository.BuildRepository(fileHandle)
        End If
    End Function

    ''' <summary>
    ''' load list of kegg reference <see cref="Map"/>.
    ''' </summary>
    ''' <param name="repository">
    ''' repository data source could be:
    ''' 
    ''' 1. a directory of repository data for kegg reference <see cref="Map"/>.
    ''' 2. a file stream for the map vector in messagepack or HDS pack format
    ''' </param>
    ''' <returns>
    ''' a kegg reference map object vector, which can be indexed 
    ''' via <see cref="Map.EntryId"/>.
    ''' 
    ''' the data type will be dertermined by the <paramref name="rawMaps"/> parameter:
    ''' 
    ''' 1. for raw maps TRUE: a vector of the <see cref="Map"/> object will be returns
    ''' 2. for raw maps FALSE: a wrapper of the kegg map collection 
    '''    <see cref="MapRepository"/> will be generates from this function
    ''' </returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("load.maps")>
    <RApiReturn(GetType(Map), GetType(MapRepository))>
    Public Function loadMapRepository(repository As Object, Optional rawMaps As Boolean = True) As Object
        If TypeOf repository Is Stream Then
            Using file As Stream = DirectCast(repository, Stream)
                If rawMaps Then
                    Return KEGGMapPack.ReadKeggDb(file)
                Else
                    Return KEGGMapPack.ReadKeggDb(file).DoCall(AddressOf MapRepository.BuildRepository)
                End If
            End Using
        Else
            Return parseMapsFromFile(fileHandle:=any.ToString(repository), rawMaps)
        End If
    End Function

    ''' <summary>
    ''' load kegg pathway maps from a given repository data directory.
    ''' </summary>
    ''' <param name="repository"></param>
    ''' <returns></returns>
    <ExportAPI("load.pathways")>
    <RApiReturn(GetType(PathwayMap), GetType(Pathway))>
    Public Function LoadPathways(repository As String,
                                 Optional referenceMap As Boolean = True,
                                 Optional env As Environment = Nothing) As Object

        If repository.ExtensionSuffix("msgpack", "messagepack") Then
            Return KEGGPathwayPack.ReadKeggDb(repository)
        ElseIf repository.ExtensionSuffix("db", "hds") Then
            Dim file As Stream = repository.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim models As Pathway() = Data.ReadKeggMaps(buffer:=file)

            Return models
        ElseIf repository.DirectoryExists OrElse referenceMap Then
            Dim maps = ls - l - r - "*.Xml" <= repository
            Dim pathwayMaps As PathwayMap() = maps _
                .Select(AddressOf LoadXml(Of PathwayMap)) _
                .ToArray

            Return pathwayMaps
        Else
            Return RInternal.debug.stop(New NotImplementedException, env)
        End If
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
                Return ReactionTable.LoadFolder(resource).ToArray
            ElseIf resource.FileExists Then
                Return resource.LoadCsv(Of ReactionTable).ToArray
            Else
                Return RInternal.debug.stop({
                    "invalid resource handle for load kegg reaction table!",
                    "resource: " & resource
                }, env)
            End If
        ElseIf repo.GetType Is GetType(ReactionRepository) Then
            Return ReactionTable.Load(DirectCast(repo, ReactionRepository)).ToArray
        Else
            Return RInternal.debug.stop(New InvalidConstraintException(repo.GetType.FullName), env)
        End If
    End Function

    <ExportAPI("reactionsId")>
    Public Function getReactionsId(<RRawVectorArgument> repo As Object, Optional env As Environment = Nothing)
        Dim dataRepo = pipeline.TryCreatePipeline(Of Map)(repo, env)

        If Not dataRepo.isError Then
            Return dataRepo _
                .populates(Of Map)(env) _
                .Select(Function(map)
                            Return map.shapes.mapdata _
                                .Select(Function(poly) poly.IDVector) _
                                .IteratesALL
                        End Function) _
                .IteratesALL _
                .Distinct _
                .Where(Function(id)
                           Return id.IsPattern("R\d+")
                       End Function) _
                .ToArray
        End If

        Return dataRepo.getError
    End Function

    ''' <summary>
    ''' get a vector of kegg compound id from the kegg reaction_class/pathway maps data repository
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("compoundsId")>
    Public Function getCompoundsId(<RRawVectorArgument> repo As Object, Optional env As Environment = Nothing) As Object
        Dim dataRepo As pipeline = pipeline.TryCreatePipeline(Of ReactionClass)(repo, env, suppress:=True)

        If Not dataRepo.isError Then
            Return dataRepo _
                .populates(Of ReactionClass)(env) _
                .Select(Function(r)
                            Return r.reactantPairs
                        End Function) _
                .IteratesALL _
                .Select(Iterator Function(t) As IEnumerable(Of String)
                            Yield t.from
                            Yield t.to
                        End Function) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End If

        dataRepo = pipeline.TryCreatePipeline(Of ReactionClassTable)(repo, env, suppress:=True)

        If Not dataRepo.isError Then
            Return dataRepo _
                .populates(Of ReactionClassTable)(env) _
                .Select(Iterator Function(r) As IEnumerable(Of String)
                            Yield r.from
                            Yield r.to
                        End Function) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End If

        dataRepo = pipeline.TryCreatePipeline(Of Map)(repo, env, suppress:=True)

        If dataRepo.isError Then
            dataRepo = pipeline.TryCreatePipeline(Of MapIndex)(repo, env, suppress:=True)

            If Not dataRepo.isError Then
                dataRepo = pipeline.TryCreatePipeline(Of Map)(dataRepo.populates(Of MapIndex)(env).Select(Function(i) DirectCast(i, Map)), env)
            End If
        End If

        If Not dataRepo.isError Then
            Return dataRepo _
                .populates(Of Map)(env) _
                .Select(Function(map)
                            Return map.shapes.mapdata _
                                .Select(Function(poly) poly.IDVector) _
                                .IteratesALL
                        End Function) _
                .IteratesALL _
                .Distinct _
                .Where(Function(id)
                           Return id.IsPattern("C\d+")
                       End Function) _
                .ToArray
        End If

        Return dataRepo.getError
    End Function

    ''' <summary>
    ''' load stream of the reaction_class data model from kegg data repository.
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("reaction_class.repo")>
    <RApiReturn(GetType(ReactionClass))>
    Public Function loadReactionClassRaw(<RRawVectorArgument> repo As Object, Optional env As Environment = Nothing) As Object
        If repo Is Nothing Then
            Return RInternal.debug.stop("the required repository source can not be nothing!", env)
        ElseIf TypeOf repo Is vector Then
            repo = DirectCast(repo, vector).data
        End If

        If TypeOf repo Is String OrElse TypeOf repo Is String() Then
            Dim repoStr As String() = CLRVector.asCharacter(repo)

            If repoStr.Length = 0 Then
                Return RInternal.debug.stop("the required repository source can not be empty!", env)
            ElseIf repoStr.Length = 1 Then
                Dim con As String = repoStr(Scan0)

                If con.ExtensionSuffix("msgpack") Then
                    Using buffer As Stream = con.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                        Return ReactionClassPack.ReadKeggDb(buffer)
                    End Using
                ElseIf con.DirectoryExists Then
                    Return ReactionClass _
                        .ScanRepository(con, loadsAll:=False) _
                        .DoCall(AddressOf pipeline.CreateFromPopulator)
                Else
                    Return RInternal.debug.stop(New NotImplementedException, env)
                End If
            ElseIf repoStr.All(Function(path) path.ExtensionSuffix("xml")) Then
                Return repoStr _
                    .Select(AddressOf LoadXml(Of ReactionClass)) _
                    .DoCall(AddressOf pipeline.CreateFromPopulator)
            Else
                Return RInternal.debug.stop(New NotImplementedException, env)
            End If
        Else
            Return Message.InCompatibleType(GetType(String), repo.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' load reaction class data from a repository data source.
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("reaction_class.table")>
    <RApiReturn(GetType(ReactionClassTable))>
    Public Function loadReactionClassTable(<RRawVectorArgument> repo As Object, Optional env As Environment = Nothing) As Object
        If repo Is Nothing Then
            Return Nothing
        ElseIf TypeOf repo Is vector Then
            repo = DirectCast(repo, vector).data
        End If

        If TypeOf repo Is String OrElse TypeOf repo Is String() Then
            Dim resource As String = CLRVector.asCharacter(repo)(Scan0)

            If resource.DirectoryExists Then
                Return ReactionClassTable.ScanRepository(repo:=resource).ToArray
            ElseIf resource.ExtensionSuffix("csv") Then
                Return resource.LoadCsv(Of ReactionClassTable).ToArray
            ElseIf resource.ExtensionSuffix("msgpack") Then
                Using file As Stream = resource.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                    Return ReactionClassPack.ReadKeggDb(file).DoCall(AddressOf ReactionClassTable.ScanRepository).ToArray
                End Using
            Else
                Return RInternal.debug.stop({
                    "invalid resource handle for load kegg reaction table!",
                    "resource: " & resource
                }, env)
            End If
        Else
            Dim classes As pipeline = pipeline.TryCreatePipeline(Of ReactionClass)(repo, env)

            If classes.isError Then
                Return classes.getError
            Else
                Return classes.populates(Of ReactionClass)(env) _
                    .DoCall(AddressOf ReactionClassTable.ScanRepository) _
                    .ToArray
            End If
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
        Dim pathwayCollection As pipeline = pipeline.TryCreatePipeline(Of Pathway)(pathway, env)

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
        If Not (file.FileExists OrElse file.DirectoryExists) Then
            Return RInternal.debug.stop({"the given file is not exists on your filesystem!", $"file: " & file}, env)
        ElseIf file.DirectoryExists Then
            Return (ls - l - r - "*.Xml" <= file).Select(AddressOf LoadXml(Of Pathway)).ToArray
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

    <ExportAPI("read.kgml")>
    Public Function read_kgml(file As String) As KGML.pathway
        Return file.LoadXml(Of KGML.pathway)
    End Function

    ''' <summary>
    ''' construct a new kegg compound data model
    ''' </summary>
    ''' <param name="entry">the kegg compound id</param>
    ''' <param name="name">the compound common names</param>
    ''' <param name="formula">the chemical formula of the kegg compound</param>
    ''' <param name="exactMass">the evaluated exact mass value based on the formula string</param>
    ''' <param name="reaction">A set of the related kegg reaction data about current metabolite</param>
    ''' <param name="enzyme">A set of the related enzyme that associated with the reaction list</param>
    ''' <param name="remarks">comment text about this metabolite</param>
    ''' <param name="KCF">the molecular strucutre text</param>
    ''' <param name="DBLinks">A dataframe object that contains the external reference link to 
    ''' other database of current metabolite object, data fields: "db", "id", "link" should 
    ''' be exists in this dataframe object.</param>
    ''' <param name="pathway"></param>
    ''' <param name="modules"></param>
    ''' <returns></returns>
    <ExportAPI("compound")>
    Public Function createCompound(entry As String,
                                   name As String(),
                                   formula As String,
                                   exactMass As Double,
                                   Optional reaction As String() = Nothing,
                                   Optional enzyme As String() = Nothing,
                                   Optional remarks As String() = Nothing,
                                   Optional KCF As String = Nothing,
                                   Optional DBLinks As dataframe = Nothing,
                                   Optional pathway As dataframe = Nothing,
                                   Optional modules As dataframe = Nothing) As Compound

        Return New Compound With {
            .entry = entry,
            .commonNames = name,
            .reactionId = reaction,
            .enzyme = enzyme,
            .formula = formula,
            .exactMass = exactMass,
            .molWeight = exactMass,
            .remarks = remarks,
            .KCF = KCF,
            .DbLinks = DBLinks.GetDbLinks,
            .pathway = pathway.GetNameValues,
            .[Module] = modules.GetNameValues
        }
    End Function

    <ExportAPI("pathway")>
    Public Function pathway(id As String, name As String, description As String,
                            modules As dataframe,
                            DBLinks As dataframe,
                            KO_pathway As String(),
                            references As dataframe,
                            compounds As dataframe,
                            drugs As dataframe,
                            genes As dataframe,
                            organism As list,
                            disease As dataframe,
                            Optional env As Environment = Nothing) As Pathway

        Dim code As String = organism.getValue(Of String)("code", env, "KO")
        Dim sp_name As String = organism.getValue(Of String)("name", env, "")

        Return New Pathway With {
            .EntryId = id, .name = name,
            .description = description,
            .modules = modules.GetNameValues,
            .otherDBs = DBLinks.GetDbLinks,
            .KOpathway = KO_pathway _
                .Select(Function(kid) New NamedValue With {.name = kid}) _
                .ToArray,
            .references = references.GetReference,
            .compound = compounds.GetNameValues,
            .drugs = drugs.GetNameValues,
            .organism = $"[{code}] {sp_name}",
            .genes = genes.GetGeneName,
            .disease = disease.GetNameValues
        }
    End Function

    <ExportAPI("reaction")>
    Public Function reaction(id As String, name As String(), definition As String, equation As String, comment As String,
                             reaction_class As dataframe,
                             enzyme As String(),
                             pathways As dataframe,
                             modules As dataframe,
                             KO As dataframe,
                             links As dataframe) As Reaction

        Return New Reaction With {
            .[Class] = reaction_class.GetNameValues,
            .Comments = comment,
            .CommonNames = name,
            .Definition = definition,
            .Enzyme = enzyme,
            .Equation = equation,
            .ID = id,
            .[Module] = modules.GetNameValues,
            .Orthology = New OrthologyTerms With {.Terms = KO.GetPropertyValues},
            .Pathway = pathways.GetNameValues,
            .DBLink = links.GetDbLinks
        }
    End Function

    ''' <summary>
    ''' create a new data model of kegg reaction_class
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="definition"></param>
    ''' <param name="reactions"></param>
    ''' <param name="enzyme"></param>
    ''' <param name="pathways"></param>
    ''' <param name="KO"></param>
    ''' <param name="transforms"></param>
    ''' <param name="rmodules"></param>
    ''' <returns></returns>
    <ExportAPI("reaction_class")>
    Public Function reaction_class(id As String, definition As String,
                                   reactions As String(),
                                   enzyme As String(),
                                   pathways As dataframe,
                                   KO As dataframe,
                                   transforms As dataframe,
                                   rmodules As dataframe) As ReactionClass

        Return New ReactionClass With {
            .entryId = id,
            .definition = definition,
            .enzymes = enzyme _
                .SafeQuery _
                .Select(Function(ecid)
                            Return New NamedValue With {.name = ecid}
                        End Function) _
                .ToArray,
            .orthology = KO.GetNameValues,
            .pathways = pathways.GetNameValues,
            .reactions = reactions _
                .SafeQuery _
                .Select(Function(rid)
                            Return New NamedValue With {.name = rid}
                        End Function) _
                .ToArray,
            .reactantPairs = transforms.forEachRow({"from", "to"}) _
                .Select(Function(t)
                            Return New ReactionCompoundTransform With {
                                .from = any.ToString(t(Scan0)),
                                .[to] = any.ToString(t(1))
                            }
                        End Function) _
                .ToArray,
            .rModules = rmodules.GetNameValues
        }
    End Function

    ''' <summary>
    ''' create shape area data for kegg graph rendering
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <ExportAPI("shapeAreas")>
    Public Function shapeAreas(data As dataframe) As Area()
        Return data.forEachRow({"id", "shape", "coords", "class", "href", "title", "entry", "refid", "module"}) _
            .Select(Function(i)
                        Return New Area With {
                            .data_id = any.ToString(i(Scan0)),
                            .shape = any.ToString(i(1)),
                            .coords = any.ToString(i(2)),
                            .[class] = any.ToString(i(3)),
                            .href = any.ToString(i(4)),
                            .title = any.ToString(i(5)),
                            .entry = any.ToString(i(6)),
                            .refid = any.ToString(i(7)),
                            .moduleId = any.ToString(i(8))
                        }
                    End Function) _
            .ToArray
    End Function

    <ExportAPI("keggMap")>
    Public Function keggMap(id As String,
                            name As String,
                            description As String,
                            img As String,
                            url As String,
                            area As Area()) As Map

        Return New Map With {
            .EntryId = id,
            .name = name,
            .PathwayImage = img,
            .shapes = New MapData With {.mapdata = area},
            .URL = url,
            .description = description
        }
    End Function
End Module

Public Enum OrganismTypes
    all
    prokaryote
    eukaryotes
End Enum
