#Region "Microsoft.VisualBasic::5b9d80577640509c41dde4c51db2fb7d, R#\seqtoolkit\hmmer.vb"

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

    '   Total Lines: 78
    '    Code Lines: 57 (73.08%)
    ' Comment Lines: 6 (7.69%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 15 (19.23%)
    '     File Size: 2.68 KB


    ' Module hmmer
    ' 
    '     Function: hmmer_search, load_hmmer, load_interprodb, parse_hmmer_model, parse_kofamscan
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.IO.HDF5.struct
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER
Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER.InterPro.Xml
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' HMMER profile hidden markov model search tools
''' </summary>
''' 
''' <remarks>
''' This R# package module provides the api for run the HMMER3 profile HMM 
''' search based protein function annotation:
''' 
''' + ``load_interprodb``: load the InterPro database term entries;
''' + ``parse_hmmer_model``: parse the HMMER3 profile HMM model text data;
''' + ``load_hmmer``: load a collection of the HMMER3 profile HMM model files;
''' + ``hmmer_search``: run the HMMER profile HMM search for protein function 
'''   annotation;
''' + ``parse_kofamscan``: parse the kofamscan annotation table output.
''' </remarks>
<Package("hmmer")>
Module hmmer

    ''' <summary>
    ''' load the InterPro database term entries from a given interpro database 
    ''' xml document file
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the InterPro database document file(``interpro.xml``) 
    ''' for load the term entries.
    ''' </param>
    ''' <returns>
    ''' a lazy pipeline collection of the <see cref="Interpro"/> term entry 
    ''' object that loaded from the given InterPro database document file.
    ''' </returns>
    <ExportAPI("load_interprodb")>
    <RApiReturn(GetType(Interpro))>
    Public Function load_interprodb(file As String) As Object
        Return pipeline.CreateFromPopulator(interprodb.ReadTerms(file))
    End Function

    ''' <summary>
    ''' parse the HMMER3 profile HMM model text data
    ''' </summary>
    ''' <param name="x">
    ''' the HMMER3 profile HMM model text data, or a file path of the HMMER3 
    ''' profile model document(``*.hmm``) for parse.
    ''' </param>
    ''' <returns>
    ''' a <see cref="ProfileHMM"/> profile hidden markov model object that 
    ''' parsed from the given HMMER3 profile model text data.
    ''' </returns>
    <ExportAPI("parse_hmmer_model")>
    Public Function parse_hmmer_model(x As String) As ProfileHMM
        Return HMMER3Parser.ParseContent(x.SolveStream)
    End Function

    ''' <summary>
    ''' load a collection of the HMMER3 profile HMM model files for protein 
    ''' function annotation
    ''' </summary>
    ''' <param name="x">
    ''' a character vector of the HMMER3 profile model file paths(``*.hmm``) 
    ''' for load into the protein annotator. this parameter also can be a 
    ''' directory path that contains a set of the HMMER3 profile model files, 
    ''' then all of the profile model files inside the given directory will be 
    ''' loaded.
    ''' </param>
    ''' <returns>
    ''' a <see cref="ProteinAnnotator"/> object that contains the loaded HMMER3 
    ''' profile models, which can be used for run the protein function 
    ''' annotation via the ``hmmer_search`` api;
    ''' 
    ''' this function returns NULL if the given input is an empty character 
    ''' vector.
    ''' </returns>
    <ExportAPI("load_hmmer")>
    Public Function load_hmmer(<RRawVectorArgument> x As Object) As ProteinAnnotator
        Dim list = CLRVector.asCharacter(x)

        If list.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim hmmer As New ProteinAnnotator

        If list.Length = 1 AndAlso list(0).DirectoryExists Then
            Call hmmer.LoadModelsFromDirectory(list(0))
        Else
            For Each file As String In list
                Call hmmer.LoadModel(file)
            Next
        End If

        Return hmmer
    End Function

    ''' <summary>
    ''' run the HMMER profile HMM search for protein function annotation
    ''' </summary>
    ''' <param name="hmmer">
    ''' a <see cref="ProteinAnnotator"/> object that contains the loaded HMMER3 
    ''' profile models, which is created by the ``load_hmmer`` api.
    ''' </param>
    ''' <param name="x">
    ''' a protein fasta sequence collection for run the HMMER search, which can 
    ''' be a <see cref="SMRUCC.genomics.SequenceModel.FASTA.FastaFile"/> object, 
    ''' a collection of the 
    ''' <see cref="SMRUCC.genomics.SequenceModel.FASTA.FastaSeq"/> object, or a 
    ''' character vector of the raw sequence data.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a lazy pipeline collection of the <see cref="AnnotationResult"/> domain 
    ''' annotation result, one result element for each of the profile HMM hit 
    ''' that found in the input protein sequence collection;
    ''' 
    ''' this function returns NULL if the input sequence data can not be cast to 
    ''' a fasta sequence collection.
    ''' </returns>
    <ExportAPI("hmmer_search")>
    <RApiReturn(GetType(AnnotationResult))>
    Public Function hmmer_search(hmmer As ProteinAnnotator, <RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As Object
        Dim seqs = GetFastaSeq(x, env)

        If seqs Is Nothing Then
            Return Nothing
        End If

        Return pipeline.CreateFromPopulator(seqs.Select(Function(fa) hmmer.Annotate(fa)).IteratesALL)
    End Function

    ''' <summary>
    ''' Parse the kofamscan table output
    ''' </summary>
    ''' <param name="file">
    ''' the input source: a file path of the kofamscan annotation table output 
    ''' file, or a file stream object of the target table file.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a lazy pipeline collection of the <see cref="KOFamScan"/> KEGG orthology 
    ''' annotation result record object;
    ''' 
    ''' this function returns a R# error message object if the given file can 
    ''' not be opened for read.
    ''' </returns>
    <ExportAPI("parse_kofamscan")>
    <RApiReturn(GetType(KOFamScan))>
    Public Function parse_kofamscan(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim s = SMRUCC.Rsharp.GetFileStream(file, IO.FileAccess.Read, env)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Return pipeline.CreateFromPopulator(KOFamScan.ParseTable(s.TryCast(Of Stream)))
    End Function

End Module
