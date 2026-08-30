#Region "Microsoft.VisualBasic::70930fe9f42206226e65959a80ad60a0, R#\TRNtoolkit\TRNBuilder.vb"

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

    '   Total Lines: 291
    '    Code Lines: 195 (67.01%)
    ' Comment Lines: 57 (19.59%)
    '    - Xml Docs: 35.09%
    ' 
    '   Blank Lines: 39 (13.40%)
    '     File Size: 13.50 KB


    ' Module TRNBuilder
    ' 
    '     Function: motif_search, open_motifdb, readFootprintSites, readRegulations, RegulationFootprint
    '               writeRegulationFootprints
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO.Linq
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' tools for create a transcription regulation network
''' </summary>
''' 
''' <remarks>
''' This R# package module provides the toolkit for build the transcription 
''' regulation network(TRN) from the regulon database and the motif site scan 
''' result:
''' 
''' + ``open_motifdb``: open the position weight matrix(PWM) motif database;
''' + ``motif_search``: scan the TF binding site(TFBS) motif site on the given 
'''   promoter/upstream sequence regions;
''' + ``regulation.footprint``: create the regulation footprint(regulation network 
'''   edges) from the regulator mapping data(bbh), the motif site data and the 
'''   regprecise regulon database;
''' + ``read.regulations``/``write.regulations``: read and save the regulation 
'''   footprint data table;
''' + ``read.footprints``: read the motif site(footprint site) table data.
''' </remarks>
<Package("TRN.builder")>
Module TRNBuilder

    ''' <summary>
    ''' open the motif database
    ''' </summary>
    ''' <param name="file">
    ''' the motif database source:
    ''' 
    ''' 1. a directory path that contains a set of the MEME format motif files 
    '''    (*.meme), then a <see cref="MEMEMotifRepository"/> object will be created 
    '''    from this directory;
    ''' 2. a file path or a file stream object of the binary motif database file, 
    '''    then the database will be opened from the given data stream in read only 
    '''    mode.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a motif database object for get the position weight matrix data of each 
    ''' transcription factor family, which could be used by the ``motif_search`` api 
    ''' for run the TFBS motif site scan;
    ''' 
    ''' this function returns a R# error message object if the given motif database 
    ''' file can not be opened for read.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' NOTE: the ``db`` parameter of the ``motif_search`` api requires a 
    ''' ``SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.PWMDatabase`` 
    ''' object, but this function returns a ``MEMEMotifRepository`` or a 
    ''' ``SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.PWMDatabase`` 
    ''' object, so the database object that is created by this api can not be 
    ''' consumed by the ``motif_search`` api directly at this moment.
    ''' </remarks>
    <ExportAPI("open_motifdb")>
    <RApiReturn(GetType(SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.PWMDatabase))>
    Public Function open_motifdb(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        If isScalarVector(file) AndAlso TypeOf getFirst(file) Is String AndAlso CLRVector.asScalarCharacter(file).DirectoryExists Then
            Return New MEMEMotifRepository(CLRVector.asScalarCharacter(file))
        Else
            Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

            If s Like GetType(Message) Then
                Return s.TryCast(Of Message)
            End If

            Return Motif.PWMDatabase.OpenReadOnly(s.TryCast(Of Stream))
        End If
    End Function

    ''' <summary>
    ''' scan the TF binding site motif on the given sequence regions
    ''' </summary>
    ''' <param name="db">
    ''' the position weight matrix(PWM) motif database object, which contains the 
    ''' motif model of each transcription factor family.
    ''' </param>
    ''' <param name="search_regions">
    ''' the sequence regions for run the motif site scan, which can be a fasta 
    ''' sequence collection, a <see cref="FastaFile"/> object or a character vector 
    ''' of the raw sequence data, each sequence is a candidate promoter/upstream 
    ''' sequence region of one gene.
    ''' </param>
    ''' <param name="family">
    ''' an optional character vector of the transcription factor family name for 
    ''' restrict the motif scan: only the motif model of the given families will be 
    ''' used for the scan, all of the motif models in the database will be used if 
    ''' this parameter is not specified.
    ''' </param>
    ''' <param name="pval_cutoff">
    ''' the p-value cutoff of the motif site match: the candidate site that its 
    ''' match p-value is greater than this cutoff will be ignored, by default is 
    ''' 0.05.
    ''' </param>
    ''' <param name="minW">
    ''' the minimum score ratio cutoff of the motif site match, by default is 0.85.
    ''' 
    ''' NOTE: this parameter is not applied by the current implementation, the motif 
    ''' site match result is filtered by the ``pval_cutoff`` and the ``top`` 
    ''' parameter only.
    ''' </param>
    ''' <param name="top">
    ''' the top n best matched site of each motif model on each sequence region, by 
    ''' default is 3.
    ''' </param>
    ''' <param name="bg">
    ''' the background model of the motif site scan, the uniform background model 
    ''' will be used if this parameter is not specified.
    ''' </param>
    ''' <param name="scan_reverse">
    ''' scan the motif site on the reverse complement strand of the sequence region 
    ''' or not? by default is TRUE.
    ''' </param>
    ''' <param name="tqdm_bar">
    ''' display the progress bar of the motif site scan task on the console? by 
    ''' default is TRUE.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a vector of the <see cref="MotifMatch"/> motif site match result: the 
    ''' ``title`` property is the sequence title of the corresponding sequence 
    ''' region, the ``motif`` property is the matched motif model, the ``start``, 
    ''' ``ends``, ``strand`` and ``segment`` property is the location and the 
    ''' sequence data of the matched site, and the ``score1``, ``score2`` and 
    ''' ``pvalue`` property is the match score data;
    ''' 
    ''' this function returns a R# error message object if the given sequence source 
    ''' can not be cast to a fasta sequence collection.
    ''' </returns>
    <ExportAPI("motif_search")>
    <RApiReturn(GetType(MotifMatch))>
    Public Function motif_search(db As SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.PWMDatabase, <RRawVectorArgument> search_regions As Object,
                                 <RRawVectorArgument(TypeCodes.string)>
                                 Optional family As Object = Nothing,
                                 Optional pval_cutoff As Double = 0.05,
                                 Optional minW As Double = 0.85,
                                 Optional top As Integer = 3,
                                 Optional bg As BackgroundModel = Nothing,
                                 Optional scan_reverse As Boolean = True,
                                 Optional tqdm_bar As Boolean = True,
                                 Optional env As Environment = Nothing) As Object

        Dim seqs As IEnumerable(Of FastaSeq) = pipHelper.GetFastaSeq(search_regions, env)
        Dim familyIds As String() = CLRVector.asCharacter(family)

        If seqs Is Nothing Then
            Return RInternal.debug.stop("invalid fasta sequence source for run TFBS motif site search!", env)
        End If

        Dim motifs As Dictionary(Of String, Probability())

        If familyIds.IsNullOrEmpty Then
            motifs = db.LoadMotifs
        Else
            motifs = familyIds _
                .Distinct _
                .ToDictionary(Function(name) name,
                              Function(name)
                                  Return db _
                                      .LoadFamilyMotifs(name) _
                                      .ToArray
                              End Function)
        End If

        Dim scanner As New MotifScanner(If(bg, BackgroundModel.Uniform))
        Dim tfbs_hits As New List(Of MotifMatch)

        For Each site As FastaSeq In TqdmWrapper.Wrap(seqs.ToArray, wrap_console:=tqdm_bar)
            Dim site_id As String = site.Title

            For Each familyName As String In motifs.Keys
                For Each pwm As Probability In motifs(familyName)
                    For Each match As MotifMatch In scanner.Scan(pwm.CreateModel, site.SequenceData,
                                                                 pValueThreshold:=pval_cutoff,
                                                                 topN:=top,
                                                                 scanReverseStrand:=scan_reverse)
                        match.title = site_id
                        tfbs_hits.Add(match)
                    Next
                Next
            Next
        Next

        Return tfbs_hits.ToArray
    End Function

    ''' <summary>
    ''' read a footprint site model data file
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the footprint site csv table file, which contains the 
    ''' motif site location data and the downstream gene information of each site.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="FootprintSite"/> object that is loaded from the 
    ''' given csv table file.
    ''' </returns>
    <ExportAPI("read.footprints")>
    Public Function readFootprintSites(file As String) As FootprintSite()
        Return file.LoadCsv(Of FootprintSite)
    End Function

    ''' <summary>
    ''' read a regulation prediction result file
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the regulation footprint csv table file, which could be 
    ''' created by the ``write.regulations`` api.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="SMRUCC.genomics.Data.Regprecise.RegulationFootprint"/> 
    ''' object that is loaded from the given csv table file, each object is a 
    ''' regulation network edge of the regulator to its regulated target gene.
    ''' </returns>
    <ExportAPI("read.regulations")>
    Public Function readRegulations(file As String) As RegulationFootprint()
        Return file.LoadCsv(Of RegulationFootprint)
    End Function

    ''' <summary>
    ''' save the regulation network data file.
    ''' </summary>
    ''' <param name="regulationFootprints">
    ''' the regulation network edge data for save, which can be a vector of the 
    ''' <see cref="SMRUCC.genomics.Data.Regprecise.RegulationFootprint"/> object, or 
    ''' a pipeline object that produces a set of the regulation footprint data.
    ''' </param>
    ''' <param name="file">the file path of the generated regulation footprint csv 
    ''' table file.</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a boolean value for indicates that the regulation network data has been 
    ''' saved into the target file successfully or not;
    ''' 
    ''' this function returns a R# error message object if the given data is nothing, 
    ''' the output file path is empty, or the given data is not a collection of the 
    ''' regulation footprint data.
    ''' </returns>
    <ExportAPI("write.regulations")>
    Public Function writeRegulationFootprints(regulationFootprints As Object, file$, Optional env As Environment = Nothing) As Object
        If regulationFootprints Is Nothing Then
            Return RInternal.debug.stop("no content data provides!", env)
        ElseIf file.StringEmpty Then
            Return RInternal.debug.stop("no file write information provides!", env)
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
            Return RInternal.debug.stop($"invalid data type for write: {regulationFootprints.GetType.FullName }", env)
        End If
    End Function

    '<ExportAPI("regulations")>
    '<RApiReturn(GetType(RegulationFootprint))>
    'Public Function RegulationFootprints(regDb As RegPreciseScan,
    '                                     <RRawVectorArgument> factors As Object,
    '                                     <RRawVectorArgument> tfbs As Object,
    '                                     seqs As list,
    '                                     Optional env As Environment = Nothing) As Object

    '    Dim TF As pipeline = pipeline.TryCreatePipeline(Of RegpreciseBBH)(factors, env)
    '    Dim TFBSlist As pipeline = pipeline.TryCreatePipeline(Of MotifMatch)(tfbs, env)
    '    Dim seqList As Dictionary(Of String, String) = seqs.AsGeneric(Of String)(env)

    '    If TF.isError Then
    '        Return TF.getError
    '    ElseIf TFBSlist.isError Then
    '        Return TFBSlist.getError
    '    End If

    '    Return regDb.CreateFootprints(
    '        regulators:=TF.populates(Of RegpreciseBBH)(env),
    '        tfbs:=TFBSlist.populates(Of MotifMatch)(env)
    '    ) _
    '        .Select(Function(r)
    '                    r.distance = -(seqList(r.regulated).Length - seqList(r.regulated).IndexOf(r.sequenceData)) + 1
    '                    Return r
    '                End Function) _
    '        .Where(Function(r) r.distance <> -seqList(r.regulated).Length) _
    '        .ToArray
    'End Function

    '<ExportAPI("TRN")>
    'Public Function TRN(<RRawVectorArgument> footprints As Object, Optional env As Environment = Nothing) As Object
    '    Dim network As pipeline = pipeline.TryCreatePipeline(Of RegulationFootprint)(footprints, env)

    '    If network.isError Then
    '        Return network.getError
    '    Else
    '        Return network _
    '            .populates(Of RegulationFootprint)(env) _
    '            .RegulationFootprintTRN
    '    End If
    'End Function

    ''' <summary>
    ''' create the regulation footprint(regulation network edges) from the regulator 
    ''' mapping data, the motif site data and the regprecise regulon database
    ''' </summary>
    ''' <param name="regulators">
    ''' the regulator mapping data, which can be a vector of the 
    ''' <see cref="BestHit"/> object(the bbh best hit mapping result of the 
    ''' regulator protein to the target genome), or a pipeline object that produces a 
    ''' set of the <see cref="BestHit"/> data.
    ''' </param>
    ''' <param name="motifLocis">
    ''' a vector of the <see cref="FootprintSite"/> motif site data, which could be 
    ''' loaded from a csv table file via the ``read.footprints`` api: the ``src`` 
    ''' property of the site data is the transcription factor family name set of the 
    ''' corresponding motif site and the ``gene`` property is the regulated target 
    ''' gene of the site.
    ''' </param>
    ''' <param name="regprecise">
    ''' the regprecise regulon database object(<see cref="TranscriptionFactors"/>), 
    ''' which provides the regulator information(the effector, the regulation mode, 
    ''' the regulog, the biological process, etc) of each transcription factor 
    ''' family.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a pipeline object of the 
    ''' <see cref="SMRUCC.genomics.Data.Regprecise.RegulationFootprint"/> regulation 
    ''' network edge data: each edge is a regulation of one regulator to one target 
    ''' gene, which is created by mapping the motif site to the regulator of the 
    ''' corresponding transcription factor family in the regprecise database, the 
    ''' duplicated edge(``{regulator}-&gt;{regulated}``) will be removed 
    ''' automatically;
    ''' 
    ''' this function returns NULL if the given regulator mapping data is nothing, or 
    ''' a R# error message object if the given regulator data is not a collection of 
    ''' the <see cref="BestHit"/> data.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' only the regulator of the ``TF`` type in the regprecise database will be used 
    ''' for create the regulation network, and the transcription factor family name 
    ''' of the regulator is the first token of the family data which is splitted by 
    ''' the ``/`` or the ``\`` character.
    ''' 
    ''' the regulator mapping is created by the bbh best hit: the ``HitName`` of the 
    ''' <see cref="BestHit"/> data is mapped to the regprecise regulator via its 
    ''' locus id(the text after the last ``:`` character), and the ``QueryName`` is 
    ''' used as the regulator gene id in the target genome.
    ''' </remarks>
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
            Return RInternal.debug.stop($"invalid regulator maps: '{regulators.GetType.FullName }'!", env)
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
