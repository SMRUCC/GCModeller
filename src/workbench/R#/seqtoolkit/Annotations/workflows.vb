#Region "Microsoft.VisualBasic::023af971aa817b16c9403dacc7212ebb, R#\seqtoolkit\Annotations\workflows.vb"

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

    '   Total Lines: 616
    '    Code Lines: 466 (75.65%)
    ' Comment Lines: 83 (13.47%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 67 (10.88%)
    '     File Size: 28.84 KB


    ' Module workflows
    ' 
    '     Function: blast_tabular, blastn_table, diamond_hitgroups, ExportBBHHits, ExportSBHHits
    '               filter_low_level, FilterBesthitStream, flush, grepNames, openBlastReader
    '               openWriter, parseBlastnMaps, read_bbhhits, read_besthits, read_blast_tabular
    '               read_m8, removeProteinSufifx
    ' 
    '     Sub: Main, writeStreamHelper
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework.IO.Linq
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' A pipeline collection for proteins' biological function 
''' annotation based on the sequence alignment.
''' </summary>
<Package("annotation.workflow", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module workflows

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(BlastnMapping()), AddressOf blastn_table)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(HitRecord()), AddressOf blast_tabular)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function blastn_table(blastn As BlastnMapping(), args As list, env As Environment) As Object
        Dim tbl As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call tbl.add("query", From n As BlastnMapping In blastn Select n.ReadQuery)
        Call tbl.add("reference", From n As BlastnMapping In blastn Select n.Reference)
        Call tbl.add("query_len", From n As BlastnMapping In blastn Select n.QueryLength)
        Call tbl.add("score(bits)", From n As BlastnMapping In blastn Select n.Score)
        Call tbl.add("score(raw)", From n As BlastnMapping In blastn Select n.RawScore)
        Call tbl.add("e-value", From n As BlastnMapping In blastn Select n.Evalue)
        Call tbl.add("identities%", From n As BlastnMapping In blastn Select n.identitiesValue)
        Call tbl.add("identities", From n As BlastnMapping In blastn Select n.IdentitiesFraction)
        Call tbl.add("gaps%", From n As BlastnMapping In blastn Select n.gapsValue)
        Call tbl.add("gaps", From n As BlastnMapping In blastn Select n.GapsFraction)
        Call tbl.add("query_strand", From n As BlastnMapping In blastn Select n.QueryStrand)
        Call tbl.add("reference_strand", From n As BlastnMapping In blastn Select n.ReferenceStrand)
        Call tbl.add("strand", From n As BlastnMapping In blastn Select n.Strand)
        Call tbl.add("query_left", From n As BlastnMapping In blastn Select n.QueryLeft)
        Call tbl.add("query_right", From n As BlastnMapping In blastn Select n.QueryRight)
        Call tbl.add("reference_left", From n As BlastnMapping In blastn Select n.ReferenceLeft)
        Call tbl.add("reference_right", From n As BlastnMapping In blastn Select n.ReferenceRight)
        Call tbl.add("is_unique", From n As BlastnMapping In blastn Select n.Unique)
        Call tbl.add("is_full_len", From n As BlastnMapping In blastn Select n.AlignmentFullLength)
        Call tbl.add("is_perfect", From n As BlastnMapping In blastn Select n.PerfectAlignment)

        Return tbl
    End Function

    ''' <summary>
    ''' cast the blastn tabular format hits result as a data frame object
    ''' </summary>
    ''' <param name="hits">
    ''' a vector of the blastn tabular format hit record 
    ''' (<see cref="HitRecord"/>) for cast as the data frame.
    ''' </param>
    ''' <param name="args">
    ''' the additional arguments for the data frame cast, this parameter is 
    ''' not used in this function.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a data frame object of the blastn hits result: each row is one hit, 
    ''' and the columns are the hit details: ``query_id``, ``subject_id``, 
    ''' ``identities``, ``alignment_length``, ``mis_matches``, ``gap_opens``, 
    ''' ``query_start``, ``query_end``, ``subject_start``, ``subject_end``, 
    ''' ``e_value`` and ``bit_score``.
    ''' </returns>
    <RGenericOverloads("as.data.frame")>
    <ExportAPI("blast_tabular")>
    Public Function blast_tabular(hits As HitRecord(), args As list, env As Environment) As Object
        Dim tbl As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call tbl.add("query_id", From hit As HitRecord In hits Select hit.QueryID)
        Call tbl.add("subject_id", From hit As HitRecord In hits Select hit.SubjectIDs)
        Call tbl.add("query acc.ver", From hit As HitRecord In hits Select hit.QueryAccVer)
        Call tbl.add("subject acc.ver", From hit As HitRecord In hits Select hit.SubjectAccVer)
        Call tbl.add("identities", From hit As HitRecord In hits Select hit.Identity)
        Call tbl.add("alignment_length", From hit As HitRecord In hits Select hit.AlignmentLength)
        Call tbl.add("mis_matches", From hit As HitRecord In hits Select hit.MisMatches)
        Call tbl.add("gap_opens", From hit As HitRecord In hits Select hit.GapOpens)
        Call tbl.add("query_start", From hit As HitRecord In hits Select hit.QueryStart)
        Call tbl.add("query_end", From hit As HitRecord In hits Select hit.QueryEnd)
        Call tbl.add("subject_start", From hit As HitRecord In hits Select hit.SubjectStart)
        Call tbl.add("subject_end", From hit As HitRecord In hits Select hit.SubjectEnd)
        Call tbl.add("e_value", From hit As HitRecord In hits Select hit.EValue)
        Call tbl.add("bit_score", From hit As HitRecord In hits Select hit.BitScore)
        Call tbl.add("metadata", From hit As HitRecord In hits Select hit.data.GetJson)

        Return tbl
    End Function

    ''' <summary>
    ''' Open the blast output text file for parse data result.
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the blast output text file.
    ''' </param>
    ''' <param name="type">``nucl`` or ``prot``</param>
    ''' <param name="fastMode">
    ''' run the blastp output parser in the fast mode?
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>a collection of the query hits result details</returns>
    <ExportAPI("read.blast")>
    <RApiReturn(GetType(Query))>
    Public Function openBlastReader(file As String,
                                    Optional type As String = "nucl",
                                    Optional fastMode As Boolean = True,
                                    Optional env As Environment = Nothing) As pipeline

        If Not file.FileExists(True) Then
            Return REnv.Internal.debug.stop($"invalid data source: '{file.ToFileURL}'!", env)
        End If

        If LCase(type) = "nucl" Then
            Return BlastPlus.TryParseUltraLarge(file).Queries.as_iterator
        ElseIf LCase(type) = "prot" Then
            Return BlastpOutputReader.RunParser(file, fast:=fastMode).as_iterator
        Else
            Return RInternal.debug.stop($"invalid program type: {type}!", env)
        End If
    End Function

    ''' <summary>
    ''' export results of fastq reads mapping to genome sequence. 
    ''' </summary>
    ''' <param name="query">
    ''' a lazy pipeline collection of the blast query hits result 
    ''' (<see cref="Query"/>), which is created by the ``read.blast`` api.
    ''' </param>
    ''' <param name="top_best">
    ''' only export the top best mapping for each query?
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a lazy pipeline collection of the <see cref="BlastnMapping"/> reads 
    ''' mapping result;
    ''' 
    ''' this function returns a R# error message object if the input query 
    ''' pipeline data type is not the <see cref="Query"/> object.
    ''' </returns>
    <ExportAPI("blastn.maphit")>
    <RApiReturn(GetType(BlastnMapping))>
    Public Function parseBlastnMaps(query As pipeline,
                                    Optional top_best As Boolean = False,
                                    Optional env As Environment = Nothing) As pipeline
        If query Is Nothing Then
            Return Nothing
        ElseIf Not query.elementType Like GetType(Query) Then
            Return REnv.Internal.debug.stop($"invalid pipeline data type: {query.elementType.ToString}", env)
        End If

        Return query.populates(Of Query)(env) _
            .Export(parallel:=False, topBest:=top_best) _
            .as_iterator
    End Function

    ''' <summary>
    ''' Export single side besthit
    ''' </summary>
    ''' <param name="query">the blast reader result from the ``read.blast`` iterator function.</param>
    ''' <param name="idetities">
    ''' the minimum identity threshold value of the accepted hits.
    ''' </param>
    ''' <param name="coverage">
    ''' the minimum coverage threshold value of the accepted hits.
    ''' </param>
    ''' <param name="topBest">
    ''' only export the top best hit for each query?
    ''' </param>
    ''' <param name="keepsRawName">
    ''' keep the raw query name text in the exported best hit data?
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a lazy pipeline collection of the <see cref="BestHit"/> single side 
    ''' best hit result;
    ''' 
    ''' this function returns a R# error message object if the input query 
    ''' pipeline data type is not the <see cref="Query"/> object.
    ''' </returns>
    <ExportAPI("blasthit.sbh")>
    <Extension>
    <RApiReturn(GetType(BestHit))>
    Public Function ExportSBHHits(query As pipeline,
                                  Optional idetities As Double = 0.3,
                                  Optional coverage As Double = 0.5,
                                  Optional topBest As Boolean = False,
                                  Optional keepsRawName As Boolean = False,
                                  Optional env As Environment = Nothing) As pipeline

        If query Is Nothing Then
            Return Nothing
        ElseIf Not query.elementType Like GetType(Query) Then
            Return REnv.Internal.debug.stop($"invalid pipeline data type: {query.elementType.ToString}", env)
        End If

        Dim hitsPopulator As Func(Of IEnumerable(Of BestHit()))

        If topBest Then
            hitsPopulator = Iterator Function() As IEnumerable(Of BestHit())
                                For Each q As Query In query.populates(Of Query)(env)
                                    Yield {
                                        v228.SBHLines(q, coverage, idetities, keepsRawQueryName:=keepsRawName)(Scan0)
                                    }
                                Next
                            End Function
        Else
            hitsPopulator = Iterator Function() As IEnumerable(Of BestHit())
                                For Each q As Query In query.populates(Of Query)(env)
                                    Yield v228.SBHLines(q, coverage, idetities, keepsRawQueryName:=keepsRawName)
                                Next
                            End Function
        End If

        Return hitsPopulator().IteratesALL.as_iterator
    End Function

    ''' <summary>
    ''' export the bi-directional best hit(BBH) result from the given forward 
    ''' and reverse best hit data streams
    ''' </summary>
    ''' <param name="forward">
    ''' a lazy pipeline collection of the forward direction hits data, which 
    ''' could be the raw query(<see cref="Query"/>) stream, the single side 
    ''' best hit(<see cref="BestHit"/>) stream, or the diamond m8 annotation 
    ''' (<see cref="DiamondAnnotation"/>) stream.
    ''' </param>
    ''' <param name="reverse">
    ''' a lazy pipeline collection of the reverse direction hits data, which 
    ''' accepts the same data models as the ``forward`` parameter.
    ''' </param>
    ''' <param name="algorithm">
    ''' the BBH match algorithm of the bi-directional best hit search.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a lazy pipeline collection of the <see cref="BiDirectionalBesthit"/> 
    ''' bi-directional best hit result;
    ''' 
    ''' this function returns a R# error message object if the input data 
    ''' streams are nothing or their element types are not supported.
    ''' </returns>
    <ExportAPI("blasthit.bbh")>
    Public Function ExportBBHHits(forward As pipeline, reverse As pipeline,
                                  Optional algorithm As BBHAlgorithm = BBHAlgorithm.Naive,
                                  Optional env As Environment = Nothing) As pipeline

        If forward Is Nothing Then
            Return REnv.Internal.debug.stop("No forward alignment data!", env)
        ElseIf reverse Is Nothing Then
            Return REnv.Internal.debug.stop("No reversed alignment data!", env)
        End If

        If forward.elementType Like GetType(Query) Then
            forward = forward.ExportSBHHits(env:=env)
            env.AddMessage($"Best hit result from raw query in forward direction with default parameters.", MSG_TYPES.WRN)
        End If
        If reverse.elementType Like GetType(Query) Then
            reverse = reverse.ExportSBHHits(env:=env)
            env.AddMessage($"Best hit result from raw query in reverse direction with default parameters.", MSG_TYPES.WRN)
        End If
        If forward.elementType Like GetType(DiamondAnnotation) Then
            forward = forward.populates(Of DiamondAnnotation)(env).Select(Function(a) a.GetSingleHit).as_iterator
        End If
        If reverse.elementType Like GetType(DiamondAnnotation) Then
            reverse = reverse.populates(Of DiamondAnnotation)(env).Select(Function(a) a.GetSingleHit).as_iterator
        End If

        If Not forward.elementType Like GetType(BestHit) Then
            Return REnv.Internal.debug.stop($"Invalid data type {forward.ToString} in forward direction for create bbh result!", env)
        ElseIf Not reverse.elementType Like GetType(BestHit) Then
            Return REnv.Internal.debug.stop($"Invalid data type {forward.ToString} in reverse direction for create bbh result!", env)
        End If

        Select Case algorithm
            Case BBHAlgorithm.Naive

                Return BBHParser _
                    .GetBBHTop(
                        qvs:=forward.populates(Of BestHit)(env),
                        svq:=reverse.populates(Of BestHit)(env)
                    ) _
                    .as_iterator

            Case BBHAlgorithm.BHR
                Throw New NotImplementedException
            Case BBHAlgorithm.TaxonomySupports
                Throw New NotImplementedException
            Case BBHAlgorithm.HybridBHR
                Return FastMatch _
                    .BinaryMatch(forward.populates(Of BestHit)(env), reverse.populates(Of BestHit)(env)) _
                    .as_iterator
            Case Else
                Return REnv.Internal.debug.stop("invalid algorithm supports!", env)
        End Select

        Return REnv.Internal.debug.stop(New NotImplementedException, env)
    End Function

    ''' <summary>
    ''' removes protein suffix id
    ''' </summary>
    ''' <param name="hits">a collection of the blast hits result or a character vector of the protein id.</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a vector of the cleaned protein id or the blast hits result object 
    ''' which its query name and hit name have been trimmed of the protein id 
    ''' suffix.
    ''' </returns>
    <ExportAPI("remove_protein_suffix")>
    <RApiReturn(GetType(String), GetType(I_BlastQueryHit))>
    Public Function removeProteinSufifx(<RRawVectorArgument> hits As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of I_BlastQueryHit)(hits, env)

        If pull.isError Then
            Dim protein_ids As String() = CLRVector.asCharacter(hits)

            If protein_ids Is Nothing Then
                Return pull.getError
            Else
                Return protein_ids _
                    .Select(Function(id) HeaderFormats.TrimAccessionVersion(id)) _
                    .ToArray
            End If
        End If

        Dim cleanup As IEnumerable = pull.populates(Of I_BlastQueryHit)(env) _
            .Select(Function(hit)
                        hit.QueryName = HeaderFormats.TrimAccessionVersion(hit.QueryName)
                        hit.HitName = HeaderFormats.TrimAccessionVersion(hit.HitName)
                        Return hit
                    End Function)

        Return New CLRIterator(cleanup, pull.elementType.GetRawElementType)
    End Function

    ''' <summary>
    ''' apply a text grep script on the query name or hit name of the blast 
    ''' query result data
    ''' </summary>
    ''' <param name="query">
    ''' a lazy pipeline collection of the blast query hits result 
    ''' (<see cref="Query"/>), which is created by the ``read.blast`` api.
    ''' </param>
    ''' <param name="operators">
    ''' the text grep script for do the name string replacement: a text grep 
    ''' script string, or a compiled text grep engine object.
    ''' </param>
    ''' <param name="applyOnHits">
    ''' apply the text grep script on the hit name instead of the query name?
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a lazy pipeline collection of the <see cref="Query"/> blast query 
    ''' result which its name data has been processed by the given text grep 
    ''' script;
    ''' 
    ''' this function returns a R# error message object if the given text grep 
    ''' program is invalid.
    ''' </returns>
    <ExportAPI("grep.names")>
    Public Function grepNames(query As pipeline, operators As Object,
                              Optional applyOnHits As Boolean = False,
                              Optional env As Environment = Nothing) As pipeline

        If query Is Nothing Then
            Return Nothing
        ElseIf Not query.elementType Like GetType(Query) Then
            Return REnv.Internal.debug.stop($"Invalid pipeline data type: {query.elementType.ToString}", env)
        End If

        If operators Is Nothing Then
            env.AddMessage("No operations provided!", MSG_TYPES.WRN)
            Return query
        ElseIf TypeOf operators Is String Then
            operators = TextGrepScriptEngine.Compile(operators)
        ElseIf Not TypeOf operators Is TextGrepScriptEngine Then
            Return REnv.Internal.debug.stop($"Invalid program: {operators.GetType.FullName}", env)
        End If

        Dim queryPopulator As Func(Of IEnumerable(Of Query))
        Dim grep As TextGrepMethod = DirectCast(operators, TextGrepScriptEngine).PipelinePointer

        If applyOnHits Then
            queryPopulator = Iterator Function() As IEnumerable(Of Query)
                                 For Each q As Query In query.populates(Of Query)(env)
                                     For Each hit In q.SubjectHits
                                         hit.Name = grep(hit.Name)
                                     Next

                                     Yield q
                                 Next
                             End Function
        Else
            queryPopulator = Iterator Function() As IEnumerable(Of Query)
                                 For Each q As Query In query.populates(Of Query)(env)
                                     q.QueryName = grep(q.QueryName)
                                     Yield q
                                 Next
                             End Function
        End If

        Return queryPopulator().as_iterator
    End Function

    ''' <summary>
    ''' Save the annotation rawdata into the given stream file.
    ''' </summary>
    ''' <param name="data">
    ''' a lazy pipeline collection of the annotation data for write into the 
    ''' target stream file, the supported data element types are: 
    ''' <see cref="BestHit"/>, <see cref="BiDirectionalBesthit"/>, 
    ''' <see cref="BlastnMapping"/> and <see cref="RankTerm"/>.
    ''' </param>
    ''' <param name="stream">
    ''' a stream data handler that generated via the ``open.stream`` function.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' this function returns a R# error message object if the output stream 
    ''' device is nothing, the input data is nothing, or the stream device 
    ''' type is not matched with the incoming data type.
    ''' </returns>
    <ExportAPI("stream.flush")>
    Public Function flush(data As pipeline, stream As Object, Optional env As Environment = Nothing) As Object
        If stream Is Nothing Then
            Return RInternal.debug.stop("No output stream device!", env)
        ElseIf data Is Nothing Then
            Return RInternal.debug.stop("No content data provided!", env)
        ElseIf data.elementType Like GetType(BestHit) AndAlso Not TypeOf stream Is WriteStream(Of BestHit) Then
            Return RInternal.debug.stop("Unmatched stream device with the incoming data type!", env)
        ElseIf data.elementType Like GetType(BiDirectionalBesthit) AndAlso Not TypeOf stream Is WriteStream(Of BiDirectionalBesthit) Then
            Return RInternal.debug.stop("Unmatched stream device with the incoming data type!", env)
        ElseIf data.elementType Like GetType(BlastnMapping) AndAlso Not TypeOf stream Is WriteStream(Of BlastnMapping) Then
            Return RInternal.debug.stop("Unmatched stream device with the incoming data type!", env)
        ElseIf data.elementType Like GetType(RankTerm) AndAlso Not TypeOf stream Is WriteStream(Of RankTerm) Then
            Return RInternal.debug.stop("Unmatched stream device with the incoming data type!", env)
        End If

        Select Case data.elementType.raw
            Case GetType(BestHit)
                Call writeStreamHelper(Of BestHit)(stream, data, env)
            Case GetType(BiDirectionalBesthit)
                Call writeStreamHelper(Of BiDirectionalBesthit)(stream, data, env)
            Case GetType(BlastnMapping)
                Call writeStreamHelper(Of BlastnMapping)(stream, data, env)
            Case GetType(RankTerm)
                Call writeStreamHelper(Of RankTerm)(stream, data, env)
            Case Else
                Return RInternal.debug.stop(New NotImplementedException, env)
        End Select

        Return True
    End Function

    Private Sub writeStreamHelper(Of T As Class)(stream As Object, data As pipeline, env As Environment)
        With DirectCast(stream, WriteStream(Of T))
            For Each hit As T In data.populates(Of T)(env)
                Call .Flush(hit)
            Next

            Call .Flush()
        End With
    End Sub

    ''' <summary>
    ''' make filter of the blast best hits via the given parameter combinations
    ''' </summary>
    ''' <param name="besthits">is a collection of the blastp/blastn parsed result: <see cref="BestHit"/></param>
    ''' <param name="evalue">new cutoff value of the evalue for make filter of the given hits collection</param>
    ''' <param name="delNohits">removes ``HITS_NOT_FOUND``? default is yes.</param>
    ''' <param name="pickTop">pick the top one hit for each query group?</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a filtered lazy pipeline collection of the <see cref="BestHit"/> best 
    ''' hit data.
    ''' </returns>
    <ExportAPI("besthit_filter")>
    Public Function FilterBesthitStream(besthits As pipeline,
                                        Optional evalue As Double? = Nothing,
                                        Optional identities As Double? = Nothing,
                                        Optional delNohits As Boolean = True,
                                        Optional pickTop As Boolean = False,
                                        Optional env As Environment = Nothing) As pipeline
        If besthits Is Nothing Then
            Return REnv.Internal.debug.stop("The input stream data is nothing!", env)
        ElseIf Not besthits.elementType Like GetType(BestHit) Then
            Return REnv.Internal.debug.stop($"could not handle the stream data: {besthits.elementType.fullName}", env)
        End If

        Dim filter_identities As Boolean = Not identities Is Nothing
        Dim filter_evalue As Boolean = Not evalue Is Nothing
        Dim filter As Func(Of BestHit, Boolean) =
            Function(hit)
                If delNohits AndAlso hit.HitName = "HITS_NOT_FOUND" Then
                    Return False
                End If
                If filter_identities AndAlso hit.identities < identities.Value Then
                    Return False
                End If
                If filter_evalue AndAlso hit.evalue > evalue.Value Then
                    Return False
                End If

                Return True
            End Function
        Dim stream As IEnumerable(Of BestHit) = besthits _
            .populates(Of BestHit)(env) _
            .Where(filter)

        If pickTop Then
            Return stream _
                .GroupBy(Function(hit) hit.QueryName) _
                .Select(Function(group)
                            Return group _
                                .OrderByDescending(Function(hit) hit.score) _
                                .First
                        End Function) _
                .as_iterator
        Else
            Return pipeline.CreateFromPopulator(stream)
        End If
    End Function

    ''' <summary>
    ''' filter the bi-directional best hit data by removing the low level 
    ''' hits(SBH and NA level)
    ''' </summary>
    ''' <param name="bbh">
    ''' a collection of the <see cref="BiDirectionalBesthit"/> bi-directional 
    ''' best hit data for make the level filter.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a lazy pipeline collection of the <see cref="BiDirectionalBesthit"/> 
    ''' object which its level value is neither ``SBH`` nor ``NA``;
    ''' 
    ''' this function returns a R# error message object if the input bbh data 
    ''' can not be cast to a collection of the 
    ''' <see cref="BiDirectionalBesthit"/> object.
    ''' </returns>
    <ExportAPI("filter_low_level")>
    Public Function filter_low_level(<RRawVectorArgument> bbh As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of BiDirectionalBesthit)(bbh, env)

        If pull.isError Then
            Return pull.getError
        End If

        Return New CLRIterator(From hit As BiDirectionalBesthit
                               In pull.populates(Of BiDirectionalBesthit)(env)
                               Where hit.level <> Levels.NA AndAlso hit.level <> Levels.SBH, GetType(BiDirectionalBesthit))
    End Function

    ''' <summary>
    ''' read the hits data in pipeline stream style
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the sbh best hit data table file.
    ''' </param>
    ''' <param name="encoding">
    ''' the text encoding value of the target table file.
    ''' </param>
    ''' <returns>
    ''' a lazy pipeline collection of the <see cref="BestHit"/> best hit 
    ''' object that loaded from the given table file.
    ''' </returns>
    <ExportAPI("read.besthits")>
    <RApiReturn(GetType(BestHit))>
    Public Function read_besthits(file As String, Optional encoding As Encodings = Encodings.ASCII) As Object
        Return file _
            .OpenHandle(encoding.CodePage) _
            .AsLinq(Of BestHit) _
            .as_iterator
    End Function

    ''' <summary>
    ''' read the bi-directional best hit data in pipeline stream style
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the bbh bi-directional best hit data table file.
    ''' </param>
    ''' <param name="encoding">
    ''' the text encoding value of the target table file.
    ''' </param>
    ''' <returns>
    ''' a lazy pipeline collection of the <see cref="BiDirectionalBesthit"/> 
    ''' bi-directional best hit object that loaded from the given table file.
    ''' </returns>
    <ExportAPI("read.bbh_hits")>
    <RApiReturn(GetType(BiDirectionalBesthit))>
    Public Function read_bbhhits(file As String, Optional encoding As Encodings = Encodings.ASCII) As Object
        Return file _
            .OpenHandle(encoding.CodePage) _
            .AsLinq(Of BiDirectionalBesthit) _
            .as_iterator
    End Function

    ''' <summary>
    ''' read ncbi blast output format 6 (tabular) file for blastn result mapping to genome sequence
    ''' </summary>
    ''' <param name="file">
    ''' the input source: a file path of the blast output format 6 tabular 
    ''' table file, or a file stream object of the target table file.
    ''' </param>
    ''' <param name="make_query_group">
    ''' group the hits result by the query id? if this parameter is TRUE, then 
    ''' a named list of the hits result group will be returned.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a vector of the <see cref="HitRecord"/> blast hits result object, or a 
    ''' named list of the hits result group when the ``make_query_group`` 
    ''' parameter is TRUE;
    ''' 
    ''' this function returns a R# error message object if the given file can 
    ''' not be opened for read.
    ''' </returns>
    <ExportAPI("read.outfmt6")>
    <RApiReturn(GetType(HitRecord))>
    Public Function read_blast_tabular(<RRawVectorArgument>
                                       file As Object,
                                       Optional make_query_group As Boolean = False,
                                       Optional env As Environment = Nothing) As Object

        Dim is_filepath As Boolean = False
        Dim s = SMRUCC.Rsharp.GetFileStream(file, IO.FileAccess.Read, env, is_filepath:=is_filepath)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Dim tabular As HitRecord() = s.TryCast(Of Stream) _
            .ParseBlastTsvFile() _
            .ToArray

        If is_filepath Then
            Try
                Call s.TryCast(Of Stream).Close()
                Call s.TryCast(Of Stream).Dispose()
            Catch ex As Exception

            End Try
        End If

        If make_query_group Then
            Return New list(tabular _
                .GroupBy(Function(a) a.QueryID) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.ToArray
                              End Function))
        Else
            Return tabular
        End If
    End Function

    ''' <summary>
    ''' Open result table stream writer
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the target result table stream file.
    ''' </param>
    ''' <param name="type">
    ''' the table format type of the target stream data: ``SBH``, ``BBH``, 
    ''' ``Mapping`` or ``Terms``.
    ''' </param>
    ''' <param name="encoding">
    ''' the text encoding value of the target stream file.
    ''' </param>
    ''' <param name="ioRead">
    ''' open the target file in read mode? if this parameter is TRUE, then a 
    ''' lazy pipeline collection of the table data will be returned instead of 
    ''' the stream writer object.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a stream writer object for save the annotation data in a stream manner 
    ''' via the ``stream.flush`` api, or a lazy pipeline collection of the 
    ''' table data when the ``ioRead`` parameter is TRUE.
    ''' </returns>
    <ExportAPI("open.stream")>
    Public Function openWriter(file As String,
                               Optional type As TableTypes = TableTypes.SBH,
                               Optional encoding As Encodings = Encodings.ASCII,
                               Optional ioRead As Boolean = False,
                               Optional env As Environment = Nothing) As Object
        Select Case type
            Case TableTypes.SBH
                If ioRead Then
                    Return read_besthits(file, encoding)
                Else
                    Return New WriteStream(Of BestHit)(file, encoding:=encoding)
                End If
            Case TableTypes.BBH
                If ioRead Then
                    Return file _
                        .OpenHandle(encoding.CodePage) _
                        .AsLinq(Of BiDirectionalBesthit) _
                        .as_iterator
                Else
                    Return New WriteStream(Of BiDirectionalBesthit)(file, encoding:=encoding)
                End If
            Case TableTypes.Mapping
                If ioRead Then
                    Return file _
                        .OpenHandle(encoding.CodePage) _
                        .AsLinq(Of BlastnMapping) _
                        .as_iterator
                Else
                    Return New WriteStream(Of BlastnMapping)(file, encoding:=encoding, metaKeys:={})
                End If
            Case TableTypes.Terms
                If ioRead Then
                    Return file _
                        .OpenHandle(encoding.CodePage) _
                        .AsLinq(Of RankTerm) _
                        .as_iterator
                Else
                    Return New WriteStream(Of RankTerm)(file, encoding:=encoding, metaKeys:={})
                End If
            Case Else
                Return REnv.Internal.debug.stop($"Invalid stream formatter: {type.ToString}", env)
        End Select
    End Function

    ''' <summary>
    ''' read the diamond m8 annotation table file output
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the diamond m8 format annotation table file.
    ''' </param>
    ''' <param name="stream">
    ''' read the table data in a lazy stream manner?
    ''' </param>
    ''' <param name="filter">
    ''' the keyword for filter the annotation result.
    ''' </param>
    ''' <param name="parseHitId">
    ''' the index number of the token in the hit id text for make the hit name 
    ''' parse, a negative value means no parse.
    ''' </param>
    ''' <param name="hitIdDeli">
    ''' the delimiter character for split the hit id text.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="DiamondAnnotation"/> diamond annotation 
    ''' object, or a lazy pipeline collection of this data when the ``stream`` 
    ''' parameter is TRUE.
    ''' </returns>
    <ExportAPI("read_m8")>
    <RApiReturn(GetType(DiamondAnnotation))>
    Public Function read_m8(file As String,
                            Optional stream As Boolean = False,
                            Optional filter As String = "unknown",
                            Optional parseHitId As Integer = -1,
                            Optional hitIdDeli As String = "|") As Object

        Dim source As IEnumerable(Of DiamondAnnotation) = DiamondM8Parser.ParseFile(file)
        Dim output As IEnumerable(Of DiamondAnnotation)

        If Not filter.StringEmpty Then
            output = From a As DiamondAnnotation
                     In source
                     Where a.QseqId <> filter AndAlso
                         a.SseqId <> filter
        Else
            output = source
        End If

        If parseHitId > -1 Then
            Return wrap(
                (Iterator Function() As IEnumerable(Of DiamondAnnotation)
                     For Each diamond As DiamondAnnotation In output
                         Dim hitId As String = diamond.SseqId
                         hitId = hitId.Split(hitIdDeli).ElementAtOrDefault(parseHitId)
                         diamond.SseqId = If(hitId, "")
                         Yield diamond
                     Next
                 End Function)(), stream)
        Else
            Return wrap(output, stream)
        End If
    End Function

    Private Function wrap(output As IEnumerable(Of DiamondAnnotation), stream As Boolean) As Object
        If stream Then
            Return output.as_iterator
        Else
            Return output.ToArray
        End If
    End Function

    ''' <summary>
    ''' Make query group and convert to alignment hit collection
    ''' </summary>
    ''' <param name="x">
    ''' a collection of the <see cref="DiamondAnnotation"/> diamond annotation 
    ''' hits data for make the hit group.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a vector of the <see cref="HitCollection"/> hit group object, one group 
    ''' element for each query id.
    ''' </returns>
    <ExportAPI("diamond_hitgroups")>
    <RApiReturn(GetType(HitCollection))>
    Public Function diamond_hitgroups(<RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of DiamondAnnotation)(x, env)

        If pull.isError Then
            Return pull.getError
        End If

        Return pull.populates(Of DiamondAnnotation)(env) _
            .HitCollection _
            .ToArray
    End Function
End Module
