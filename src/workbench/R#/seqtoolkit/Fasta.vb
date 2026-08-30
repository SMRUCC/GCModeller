#Region "Microsoft.VisualBasic::47b037b607e214ffa84b9ff0d9d0c404, R#\seqtoolkit\Fasta.vb"

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

    '   Total Lines: 974
    '    Code Lines: 686 (70.43%)
    ' Comment Lines: 168 (17.25%)
    '    - Xml Docs: 94.05%
    ' 
    '   Blank Lines: 120 (12.32%)
    '     File Size: 39.55 KB


    ' Module Fasta
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: chars, createFingerprintMatrix, createSequenceCollectionTable, createSequenceTable, CutSequenceLinear
    '               fasta, fastaTitle, fastaTitles, formula, list_index
    '               makeClusterTree, mass, MSA, openFasta, openFingerpintWriter
    '               parseFasta, read_assembly, read_stockholm, readFasta, readFingerprintBson
    '               readSeq, seq_sgt, seq_vector, sizeof, slicer
    '               take_byId, Tofasta, Translates, translateSingleNtSeq, viewFasta
    '               viewMSA, writeFasta
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA
Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA.Tabular
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure
Imports SMRUCC.genomics.Model.OperonMapper
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation
Imports SMRUCC.genomics.SequenceModel.Slicer
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports ASCII = Microsoft.VisualBasic.Text.ASCII
Imports FastaWriter = SMRUCC.genomics.SequenceModel.FASTA.StreamWriter
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' Fasta sequence toolkit
''' </summary>
''' 
''' <remarks>
''' This R# package module provides the toolkit for manipulate the biological 
''' sequence data in fasta format:
''' 
''' + read the fasta sequence data from a file: ``read.fasta``, ``read.seq``, 
'''   ``open.fasta``, ``parse.fasta``;
''' + save the fasta sequence data to a file: ``write.fasta``, ``open.fasta``;
''' + create the fasta sequence object or cast the other sequence data model to 
'''   the fasta sequence data: ``fasta``, ``as.fasta``;
''' + the sequence data analysis tools: ``MSA.of``, ``translate``, ``mass``, 
'''   ``seq_formula``, ``seq_vector``, ``cut_seq.linear``, etc.
''' 
''' The fasta sequence data object in R# environment is a tuple list that its 
''' element type is <see cref="FastaSeq"/>, which can be cast to a data frame 
''' via the ``as.data.frame`` api, or be printed to the console with a pretty 
''' format via the registered console formatter.
''' </remarks>
<Package("bioseq.fasta", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
<RTypeExport("MSA_result", GetType(MSAOutput))>
Module Fasta

    ''' <summary>
    ''' Initialize the internal environment of this R# package module
    ''' </summary>
    ''' 
    ''' <remarks>
    ''' this function is invoked automatically at the start of the R# runtime 
    ''' environment, it registers:
    ''' 
    ''' 1. the console formatter of the <see cref="FastaSeq"/> and 
    '''    <see cref="FastaFile"/> sequence data object, and the multiple 
    '''    sequence alignment result(<see cref="MSAOutput"/>);
    ''' 2. the data frame cast handler of the fasta sequence collection, so that 
    '''    the fasta sequence data can be converted to a data frame via the 
    '''    ``as.data.frame`` api.
    ''' </remarks>
    Sub New()
        Call printer.AttachConsoleFormatter(Of FastaSeq)(AddressOf viewFasta)
        Call printer.AttachConsoleFormatter(Of FastaFile)(AddressOf viewFasta)
        Call printer.AttachConsoleFormatter(Of MSAOutput)(AddressOf viewMSA)

        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(FastaSeq()), AddressOf createSequenceTable)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(FastaFile), AddressOf createSequenceTable)
    End Sub

    Private Function viewMSA(msa As MSAOutput) As String
        Dim sb As New StringBuilder

        Using text As New StringWriter(sb)
            Call msa.Print(16, text)
        End Using

        Return sb.ToString
    End Function

    Private Function viewFasta(seq As Object) As String
        If seq Is Nothing Then
            Return "NULL"
        End If

        Select Case seq.GetType
            Case GetType(FastaSeq)
                With DirectCast(seq, FastaSeq)
                    Return "> " & .Title & ASCII.LF & .SequenceData
                End With
            Case GetType(FastaFile)
                With DirectCast(seq, FastaFile)
                    Return $"Fasta collection contains { .Count} fasta sequence:" & vbCrLf & vbCrLf &
                        .Take(10) _
                        .Select(Function(fa) "> " & fa.Title) _
                        .JoinBy(vbCrLf) & vbCrLf & "..."
                End With
            Case Else
                Throw New NotImplementedException
        End Select
    End Function

    ''' <summary>
    ''' cast the fasta sequence collection as a data frame
    ''' </summary>
    ''' <param name="fa">
    ''' a <see cref="FastaFile"/> object that contains a set of the fasta 
    ''' sequence data.
    ''' </param>
    ''' <param name="args">
    ''' the additional arguments for the data frame cast, this parameter is not 
    ''' used in this function.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a data frame object that each row is a fasta sequence, and the columns 
    ''' are: ``id``, ``title``, ``len`` and ``seq``.
    ''' </returns>
    <RGenericOverloads("as.data.frame")>
    Public Function createSequenceCollectionTable(fa As FastaFile, args As list, env As Environment) As dataframe
        Return createSequenceTable(fa.ToArray, args, env)
    End Function

    ''' <summary>
    ''' overloads function for cast the fasta sequence collection as a data 
    ''' frame for save to file by ``write.csv``.
    ''' </summary>
    ''' <param name="fa">
    ''' a tuple list or a vector of the <see cref="FastaSeq"/> sequence object.
    ''' </param>
    ''' <param name="args">
    ''' the additional arguments for the data frame cast, this parameter is not 
    ''' used in this function.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a data frame object that each row is a fasta sequence, and the columns 
    ''' are: the ``id`` column is the locus_tag id of the sequence, the ``title`` 
    ''' column is the fasta headers title text, the ``len`` column is the sequence 
    ''' length in chars, and the ``seq`` column is the raw sequence data.
    ''' </returns>
    <RGenericOverloads("as.data.frame")>
    Public Function createSequenceTable(fa As FastaSeq(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call df.add("id", From i In fa Select i.locus_tag)
        Call df.add("title", From i In fa Select i.Title)
        Call df.add("len", From i In fa Select i.Length)
        Call df.add("seq", From i In fa Select i.SequenceData)

        Return df
    End Function

    ''' <summary>
    ''' get the sequence length
    ''' </summary>
    ''' <param name="fa">
    ''' a <see cref="FastaSeq"/> sequence object for measure the sequence length.
    ''' </param>
    ''' <returns>
    ''' the sequence length in chars of the given fasta sequence data, ZERO will 
    ''' be returned when the given sequence object is nothing.
    ''' </returns>
    <ExportAPI("size")>
    Public Function sizeof(fa As FastaSeq) As Integer
        If fa Is Nothing Then
            Return 0
        Else
            Return fa.Length
        End If
    End Function

    ''' <summary>
    ''' get alphabets represents of the fasta sequence 
    ''' </summary>
    ''' <param name="type">
    ''' the sequence data type.
    ''' </param>
    ''' <returns>
    ''' a character vector of the alphabet letters of the given molecule type: 
    ''' the A/C/G/T/U/N letters for the DNA or RNA nucleotide sequence, or the 
    ''' 20 standard amino acid letters for the protein sequence.
    ''' 
    ''' an error will be thrown if the given sequence type is not a valid 
    ''' biological sequence type(DNA/RNA/Protein).
    ''' </returns>
    <ExportAPI("chars")>
    <RApiReturn(TypeCodes.string)>
    Public Function chars(Optional type As SeqTypes = SeqTypes.Protein) As Object
        Select Case type
            Case SeqTypes.DNA : Return DirectCast(TypeExtensions.NT, Char())
            Case SeqTypes.Protein : Return DirectCast(TypeExtensions.AA, Char())
            Case SeqTypes.RNA : Return DirectCast(TypeExtensions.RNA, Char())
            Case Else
                Throw New InvalidDataException(type.ToString)
        End Select
    End Function

    ''' <summary>
    ''' evaluate the molecule mass of the given sequence
    ''' </summary>
    ''' <param name="seqs">
    ''' a fasta sequence collection for evaluate the molecule mass, which can be 
    ''' a <see cref="FastaFile"/> object, a collection of the 
    ''' <see cref="FastaSeq"/> object, or a character vector of the raw sequence 
    ''' data.
    ''' </param>
    ''' <param name="type">
    ''' the molecule type of the input sequence data, if this parameter is not 
    ''' specified(<see cref="SeqTypes.Generic"/>), then the molecule type will be 
    ''' evaluated from the input sequence data automatically: the most common 
    ''' sequence type of the input sequence collection will be used.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a number value of the molecule mass if there is only one sequence in the 
    ''' input sequence collection, or a named list of the molecule mass value of 
    ''' each sequence if there are multiple sequence in the input sequence 
    ''' collection, the name of the list element is the fasta title of the 
    ''' corresponding sequence.
    ''' </returns>
    <ExportAPI("mass")>
    Public Function mass(<RRawVectorArgument> seqs As Object,
                         Optional type As SeqTypes = SeqTypes.Generic,
                         Optional env As Environment = Nothing) As Object

        Dim seq_pool = GetFastaSeq(seqs, env).ToArray

        If type = SeqTypes.Generic Then
            type = seq_pool _
                .Select(Function(s) s.GetSeqType) _
                .GroupBy(Function(t) t) _
                .OrderByDescending(Function(t) t.Count) _
                .First _
                .Key
        End If

        Dim vals As list = list.empty

        Select Case type
            Case SeqTypes.DNA
                If seq_pool.Length = 1 Then
                    Return MolecularWeightCalculator.CalcMW_Nucleotides(seq_pool(0), is_rna:=False)
                End If
                For Each seq As FastaSeq In seq_pool
                    Call vals.add(seq.Title, MolecularWeightCalculator.CalcMW_Nucleotides(seq, is_rna:=False))
                Next
            Case SeqTypes.RNA
                If seq_pool.Length = 1 Then
                    Return MolecularWeightCalculator.CalcMW_Nucleotides(seq_pool(0), is_rna:=True)
                End If
                For Each seq As FastaSeq In seq_pool
                    Call vals.add(seq.Title, MolecularWeightCalculator.CalcMW_Nucleotides(seq, is_rna:=True))
                Next
            Case Else
                ' protein/polypeptide
                If seq_pool.Length = 1 Then
                    Return MolecularWeightCalculator.CalcMW_Polypeptide(seq_pool(0))
                End If
                For Each seq As FastaSeq In seq_pool
                    Call vals.add(seq.Title, MolecularWeightCalculator.CalcMW_Polypeptide(seq))
                Next
        End Select

        Return vals
    End Function

    ''' <summary>
    ''' evaluate the chemical formula of the given sequence data
    ''' </summary>
    ''' <param name="seqs">
    ''' a fasta sequence collection for evaluate the chemical formula, which can 
    ''' be a <see cref="FastaFile"/> object, a collection of the 
    ''' <see cref="FastaSeq"/> object, or a character vector of the raw sequence 
    ''' data.
    ''' </param>
    ''' <param name="type">
    ''' the molecule type of the input sequence data, if this parameter is not 
    ''' specified(<see cref="SeqTypes.Generic"/>), then the molecule type will be 
    ''' evaluated from the input sequence data automatically: the most common 
    ''' sequence type of the input sequence collection will be used.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a character value of the chemical formula if there is only one sequence 
    ''' in the input sequence collection, or a named list of the chemical formula 
    ''' of each sequence if there are multiple sequence in the input sequence 
    ''' collection, the name of the list element is the fasta title of the 
    ''' corresponding sequence.
    ''' </returns>
    <ExportAPI("seq_formula")>
    Public Function formula(<RRawVectorArgument> seqs As Object,
                            Optional type As SeqTypes = SeqTypes.Generic,
                            Optional env As Environment = Nothing) As Object

        Dim seq_pool = GetFastaSeq(seqs, env).ToArray
        Dim vals As list = list.empty

        If type = SeqTypes.Generic Then
            type = seq_pool _
                .Select(Function(s) s.GetSeqType) _
                .GroupBy(Function(t) t) _
                .OrderByDescending(Function(t) t.Count) _
                .First _
                .Key
        End If

        Select Case type
            Case SeqTypes.DNA
                If seq_pool.Length = 1 Then
                    Return MolecularWeightCalculator.DeoxyribonucleotideFormula(seq_pool(0).SequenceData).ToString
                End If
                For Each seq As FastaSeq In seq_pool
                    Call vals.add(seq.Title, MolecularWeightCalculator.DeoxyribonucleotideFormula(seq.SequenceData).ToString)
                Next
            Case SeqTypes.RNA
                If seq_pool.Length = 1 Then
                    Return MolecularWeightCalculator.RibonucleotideFormula(seq_pool(0).SequenceData).ToString
                End If
                For Each seq As FastaSeq In seq_pool
                    Call vals.add(seq.Title, MolecularWeightCalculator.RibonucleotideFormula(seq.SequenceData).ToString)
                Next
            Case Else
                ' protein/polypeptide
                If seq_pool.Length = 1 Then
                    Return MolecularWeightCalculator.PolypeptideFormula(seq_pool(0).SequenceData).ToString
                End If
                For Each seq As FastaSeq In seq_pool
                    Call vals.add(seq.Title, MolecularWeightCalculator.PolypeptideFormula(seq.SequenceData).ToString)
                Next
        End Select

        Return vals
    End Function

    ''' <summary>
    ''' Create algorithm for make sequence embedding
    ''' </summary>
    ''' <param name="moltype">
    ''' the molecule type of the target sequence data for make the sequence 
    ''' embedding: protein, DNA or RNA sequence.
    ''' </param>
    ''' <param name="kappa">
    ''' the decay factor of the sequence graph transform algorithm, the smaller 
    ''' value of this parameter makes the far distance k-mer composition weight 
    ''' less.
    ''' </param>
    ''' <param name="lengthsensitive">
    ''' is the generated embedding vector sensitive to the sequence length? if 
    ''' this parameter is FALSE(the default value), then the embedding vector will 
    ''' be normalized by the sequence length, so that two sequences with the same 
    ''' k-mer composition but different lengths get the same embedding vector; if 
    ''' this parameter is TRUE, then the vector norm value grows with the sequence 
    ''' length.
    ''' </param>
    ''' <returns>
    ''' a <see cref="CreateMatrix"/> algorithm object for embedding the given 
    ''' sequence data as a numeric vector, which can be applied on a collection 
    ''' of the sequence data via the ``seq_vector`` api.
    ''' </returns>
    <ExportAPI("seq_sgt")>
    Public Function seq_sgt(Optional moltype As SeqTypes = SeqTypes.Protein,
                            Optional kappa As Double = 1,
                            Optional lengthsensitive As Boolean = False) As CreateMatrix

        Return New CreateMatrix(moltype, kappa, lengthsensitive)
    End Function

    ''' <summary>
    ''' embedding the given fasta sequence as vector
    ''' </summary>
    ''' <param name="sgt">
    ''' the sequence graph transform algorithm object, which is created by the 
    ''' ``seq_sgt`` api in this package module.
    ''' </param>
    ''' <param name="seqs">
    ''' a fasta sequence collection for make the sequence embedding, which can be 
    ''' a <see cref="FastaFile"/> object, a collection of the 
    ''' <see cref="FastaSeq"/> object, or a character vector of the raw sequence 
    ''' data.
    ''' </param>
    ''' <param name="as_dataframe">
    ''' when there are multiple sequence in the input sequence collection: cast 
    ''' the embedding matrix as a data frame object? if this parameter is FALSE(the 
    ''' default value), then a named list of the embedding vector will be returned 
    ''' for each sequence.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a numeric vector of the embedding result if there is only one sequence in 
    ''' the input sequence collection, or a data frame object(each row is the 
    ''' embedding vector of one sequence, and the column names are ``v1``, ``v2``, 
    ''' ...) when the ``as_dataframe`` parameter is TRUE, or a named list of the 
    ''' embedding vector of each sequence.
    ''' </returns>
    ''' <example>
    ''' imports "bioseq.fasta" from "seqtoolkit";
    ''' 
    ''' # get fasta sequence data
    ''' let seqs = read.fasta("./proteins.fa");
    ''' let sgt = seq_sgt(moltype = "prot");
    ''' let vec = sgt |> seq_vector(seqs);
    ''' 
    ''' # run data analysis on the generated embedding vectors
    ''' 
    ''' </example>
    <ExportAPI("seq_vector")>
    <RApiReturn(GetType(Double))>
    Public Function seq_vector(sgt As CreateMatrix, <RRawVectorArgument> seqs As Object,
                               Optional as_dataframe As Boolean = False,
                               Optional env As Environment = Nothing) As Object

        Dim seq_pool = GetFastaSeq(seqs, env).ToArray

        If seq_pool.Length = 1 Then
            Return sgt.ToVector(seq_pool(0))
        Else
            Dim vec As New Dictionary(Of String, Double())

            For Each seq As FastaSeq In seq_pool
                Call vec.Add(seq.Title, sgt.ToVector(seq))
            Next

            If as_dataframe Then
                Dim vlen As Integer = vec.Values.First.Length
                Dim vrows = vec.ToArray
                Dim m As New dataframe With {
                    .rownames = vrows.Keys,
                    .columns = New Dictionary(Of String, Array)
                }
                Dim offset As Integer
                Dim data As IEnumerable(Of Double)

                For i As Integer = 0 To vlen - 1
                    offset = i
                    data = From s As KeyValuePair(Of String, Double())
                           In vrows
                           Let vi As Double = s.Value(offset)
                           Select vi

                    Call m.add("v" & (i + 1), data)
                Next

                Return m
            Else
                Return New list(vec)
            End If
        End If
    End Function

    ''' <summary>
    ''' Read a single fasta sequence file
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the target sequence file, Just contains one sequence
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a <see cref="FastaSeq"/> object that read from the given sequence file;
    ''' 
    ''' this function returns a R# error message object if the given file is not a 
    ''' valid fasta sequence file or a genbank database file.
    ''' </returns>
    ''' <remarks>
    ''' for input a genbank database file, this function will extract the origin sequence fasta object
    ''' </remarks>
    ''' <keywords>read data</keywords>
    <ExportAPI("read.seq")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function readSeq(file As String, Optional env As Environment = Nothing) As Object
        Dim firstLine As String = file.ReadFirstLine

        If firstLine.First = ">"c Then
            Return FastaSeq.Load(file)
        ElseIf firstLine.StartsWith("LOCUS") Then
            ' is a genbank file, returns the genome origin sequence
            Return GBFF.File.Load(file).Origin.ToFasta
        Else
            Return RInternal.debug.stop({"invalid file format!", "file: " & file, $"required: *.fa, *.gbk"}, env)
        End If
    End Function

    ''' <summary>
    ''' read a fasta sequence collection file
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the fasta sequence file for read the sequence data.
    ''' </param>
    ''' <param name="lazyStream">
    ''' read the fasta sequence data in a lazy stream mode? if this parameter is 
    ''' TRUE, then a pipeline object of the <see cref="FastaSeq"/> sequence data 
    ''' will be returned, which is helpful for read a huge fasta sequence file 
    ''' without loading all of the sequence data into the memory at once.
    ''' </param>
    ''' <returns>
    ''' A collection of the fasta sequence object: a vector of the 
    ''' <see cref="FastaSeq"/> object that contains all of the sequence data in 
    ''' the given fasta file, or a lazy pipeline object of the 
    ''' <see cref="FastaSeq"/> sequence data when the ``lazyStream`` parameter is 
    ''' TRUE.
    ''' </returns>
    ''' <keywords>read data</keywords>
    <ExportAPI("read.fasta")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function readFasta(file As String, Optional lazyStream As Boolean = False) As Object
        If lazyStream Then
            Return StreamIterator _
                .SeqSource(handle:=file) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return FastaFile.Read(file).ToArray
        End If
    End Function

    ''' <summary>
    ''' read genome assembly fasta sequence file
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the genome assembly fasta sequence file, or a file stream 
    ''' object of the target sequence file.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a named list of the <see cref="ChunkedNtFasta"/> chunk sequence object, 
    ''' the name of the list element is the fasta title of the corresponding 
    ''' chromosome or contigs sequence.
    ''' 
    ''' this function returns a R# error message object if the given file can not 
    ''' be opened for read.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' unlike the ``read.fasta`` api, this function reads the whole genome 
    ''' sequence in a chunked manner: the sequence data of each chromosome is 
    ''' stored as a <see cref="ChunkedNtFasta"/> object, so that we can slice a 
    ''' sequence region from a huge chromosome sequence in a memory efficient 
    ''' manner via the ``slicer`` api.
    ''' </remarks>
    <ExportAPI("read_assembly")>
    <RApiReturn(GetType(ChunkedNtFasta))>
    Public Function read_assembly(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim is_filepath As Boolean = False
        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env, is_filepath:=is_filepath)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Dim chrom = ChunkedNtFasta.LoadDocument(s.TryCast(Of IO.Stream)).ToArray
        Dim chromSet = chrom.ToDictionary(Function(a) a.title)

        If is_filepath Then
            Call s.TryCast(Of System.IO.Stream).Dispose()
        End If

        Return New list(chromSet)
    End Function

    ''' <summary>
    ''' open the fasta sequence file 
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the target fasta sequence file for open.
    ''' </param>
    ''' <param name="read">
    ''' load a set of fasta sequence data in lazy mode? default is yes.
    ''' </param>
    ''' <param name="line_break">
    ''' the sequence length in one line of the generated fasta document when this 
    ''' function is used for open a fasta file in write mode, a negative value 
    ''' means that all of the sequence data will be written in a single line.
    ''' </param>
    ''' <param name="delimiter">
    ''' the delimiter character for merge the fasta headers title when this 
    ''' function is used for open a fasta file in write mode.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a lazy collection of the fasta sequence data(a pipeline object of the 
    ''' <see cref="FastaSeq"/> sequence data) when the ``read`` parameter is TRUE, 
    ''' or a fasta stream writer(<see cref="FastaWriter"/>) object for write the 
    ''' sequence data into the target file in a stream manner when the ``read`` 
    ''' parameter is FALSE.
    ''' </returns>
    ''' <keywords>read data</keywords>
    <ExportAPI("open.fasta")>
    <RApiReturn(GetType(FastaSeq), GetType(FastaWriter))>
    Public Function openFasta(file As String,
                              Optional read As Boolean = True,
                              Optional line_break As Integer = -1,
                              Optional delimiter As String = "|",
                              Optional env As Environment = Nothing) As Object

        If read Then
            Return StreamIterator.SeqSource(file).DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return New FastaWriter(file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False),
                                   lineBreak:=line_break,
                                   deli:=delimiter
            )
        End If
    End Function

    ''' <summary>
    ''' parse the fasta sequence object from the given text data
    ''' </summary>
    ''' <param name="x">
    ''' a character vector of the fasta sequence text data, each element in the 
    ''' given character vector is one line of the fasta document text.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="FastaSeq"/> sequence object that parsed from 
    ''' the given fasta document text data.
    ''' </returns>
    <ExportAPI("parse.fasta")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function parseFasta(x As Object) As Object
        Dim txt As String = CLRVector.asCharacter(x).JoinBy(vbCrLf)
        Dim fasta = FastaFile.DocParser(txt.LineTokens).ToArray
        Return fasta
    End Function

    ''' <summary>
    ''' takes the sequence subset from the given sequence collection by a set of 
    ''' the sequence id
    ''' </summary>
    ''' <param name="x">
    ''' a fasta sequence collection for make the subset, which can be a 
    ''' <see cref="FastaFile"/> object, a collection of the <see cref="FastaSeq"/> 
    ''' object, or a character vector of the raw sequence data.
    ''' </param>
    ''' <param name="gene_ids">
    ''' a character vector of the sequence id for takes the sequence subset, the 
    ''' sequence id is the first token of the fasta headers title text, which is 
    ''' splitted by the space, ``|``, ``(`` or the TAB character.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a vector of the <see cref="FastaSeq"/> sequence object that its sequence id 
    ''' is in the given id set;
    ''' 
    ''' this function returns a R# error message object if the input sequence data 
    ''' can not be cast to a fasta sequence collection.
    ''' </returns>
    <ExportAPI("takes")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function take_byId(<RRawVectorArgument> x As Object, <RRawVectorArgument> gene_ids As Object, Optional env As Environment = Nothing) As Object
        Dim idIndex As Index(Of String) = CLRVector.asCharacter(gene_ids).Indexing
        Dim collection As IEnumerable(Of FastaSeq) = GetFastaSeq(x, env)

        If collection Is Nothing Then
            Return REnv.Internal.debug.stop(New NotImplementedException(x.GetType.FullName), env)
        Else
            Dim subset = (From s As FastaSeq
                          In collection.ToArray.AsParallel
                          Let key As String = s.Title _
                              .Split({" "c, "|"c, "("c, ASCII.TAB}) _
                              .First
                          Where key Like idIndex
                          Select s).ToArray

            Return subset
        End If
    End Function

    ''' <summary>
    ''' make sequence list index
    ''' </summary>
    ''' <param name="x">
    ''' a fasta sequence collection for make the sequence index, which can be a 
    ''' <see cref="FastaFile"/> object, a collection of the <see cref="FastaSeq"/> 
    ''' object, or a character vector of the raw sequence data.
    ''' </param>
    ''' <param name="ids">
    ''' a character vector of the index key of each sequence, the length of this 
    ''' vector should be equals to the size of the input sequence collection. If 
    ''' this parameter is not specified, then the first token of the fasta headers 
    ''' title text of each sequence will be used as the index key.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a named list of the <see cref="FastaSeq"/> sequence object, the name of the 
    ''' list element is the corresponding index key of the sequence, so that we can 
    ''' get the target sequence object by the index key directly.
    ''' 
    ''' this function returns a R# error message object if the input sequence data 
    ''' can not be cast to a fasta sequence collection.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' the index key of the generated list object is unique: the duplicated key 
    ''' will be renamed automatically by appending an unique numeric suffix.
    ''' </remarks>
    <ExportAPI("list_index")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function list_index(<RRawVectorArgument> x As Object,
                               <RRawVectorArgument>
                               Optional ids As Object = Nothing,
                               Optional env As Environment = Nothing) As Object

        Dim collection As IEnumerable(Of FastaSeq) = GetFastaSeq(x, env)

        If collection Is Nothing Then
            Return REnv.Internal.debug.stop(New NotImplementedException(x.GetType.FullName), env)
        End If

        Dim seqs As FastaSeq() = collection.ToArray
        Dim idset As String() = CLRVector.asCharacter(ids)

        If idset.IsNullOrEmpty Then
            idset = seqs _
                .Select(Function(s)
                            Return s.Headers.First.Split.First
                        End Function) _
                .ToArray
        End If

        idset = idset.UniqueNames

        Dim index As New Dictionary(Of String, Object)

        For i As Integer = 0 To idset.Length - 1
            Call index.Add(idset(i), seqs(i))
        Next

        Return New list(index)
    End Function

    ''' <summary>
    ''' write a fasta sequence or a collection of fasta sequence object
    ''' </summary>
    ''' <param name="seq">
    ''' the fasta sequence data for write into the target file, which can be a 
    ''' single <see cref="FastaSeq"/> object, a <see cref="FastaFile"/> object, a 
    ''' collection of the <see cref="FastaSeq"/> object, a character vector of the 
    ''' raw sequence data, a fastq sequence collection, or a pipeline object that 
    ''' produces a set of the <see cref="FastaSeq"/> sequence data.
    ''' </param>
    ''' <param name="file">
    ''' the output target: a file path of the generated fasta sequence file, a file 
    ''' stream object, or a fasta stream writer object that is created by the 
    ''' ``open.fasta`` api in write mode.
    ''' </param>
    ''' <param name="lineBreak">
    ''' The sequence length in one line, negative value or ZERo means no line break.
    ''' </param>
    ''' <param name="delimiter">
    ''' the delimiter character for merge the fasta headers title of the sequence 
    ''' data.
    ''' </param>
    ''' <param name="filter_empty">
    ''' skip write sequence if the sequence object has no sequence data
    ''' </param>
    ''' <param name="encoding">The text encoding value of the generated fasta file.</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a boolean value of the file save result: TRUE means the sequence data has 
    ''' been written into the target file successfully;
    ''' 
    ''' this function returns a R# error message object if the given sequence data 
    ''' can not be cast to a fasta sequence collection, or the target file can not 
    ''' be opened for write.
    ''' </returns>
    ''' <keywords>save data</keywords>
    <ExportAPI("write.fasta")>
    <RApiReturn(TypeCodes.boolean)>
    Public Function writeFasta(<RRawVectorArgument> seq As Object, file As Object,
                               Optional lineBreak% = -1,
                               Optional delimiter As String = " ",
                               Optional filter_empty As Boolean = False,
                               Optional encoding As Encodings = Encodings.ASCII,
                               Optional env As Environment = Nothing) As Object

        If TypeOf seq Is pipeline Then
            If TypeOf file Is FastaWriter Then
                Call DirectCast(file, FastaWriter).Add(DirectCast(seq, pipeline).populates(Of FastaSeq)(env), filter_empty)
            Else
                Dim filepath As String = CStr(file)
                Dim buffer = filepath.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)

                ' save a huge bundle of the fasta sequence collection
                Using s As New IO.StreamWriter(buffer)
                    For Each fa As FastaSeq In DirectCast(seq, pipeline).populates(Of FastaSeq)(env)
                        If filter_empty And fa.Length = 0 Then
                            Continue For
                        End If

                        Call s.WriteLine(fa.GenerateDocument(
                            lineBreak:=lineBreak,
                            [overrides]:=False,
                            delimiter:=delimiter
                        ))
                    Next

                    Call s.Flush()
                End Using
            End If

            Return True
        ElseIf TypeOf file Is FastaWriter Then
            Call DirectCast(file, FastaWriter).Add(GetFastaSeq(seq, env), filter_empty)
            Return True
        Else
            ' save a collection of the fasta sequence
            Dim seqs = pipHelper.GetFastaSeq(seq, env)
            Dim fasta As FastaFile
            Dim is_filepath As Boolean
            Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env, is_filepath:=is_filepath)

            If seqs Is Nothing Then
                If TypeOf seq Is FastQFile Then
                    fasta = New FastaFile(From fq As FastQ In DirectCast(seq, FastQFile).AsEnumerable Select New FastaSeq(fq.SequenceData, fq.SEQ_ID))
                ElseIf TypeOf seq Is FastQ() Then
                    fasta = New FastaFile(From fq As FastQ In DirectCast(seq, IEnumerable(Of FastQ)) Select New FastaSeq(fq.SequenceData, fq.SEQ_ID))
                Else
                    Return Message.InCompatibleType(GetType(FastaSeq), seq.GetType, env)
                End If
            Else
                fasta = New FastaFile(From fa As FastaSeq
                                      In seqs
                                      Where If(filter_empty AndAlso fa.Length = 0, False, True))
            End If

            If s Like GetType(Message) Then
                Return s.TryCast(Of Message)
            End If

            Dim result = fasta.Save(lineBreak:=lineBreak,
                                    s:=s.TryCast(Of System.IO.Stream),
                                    encoding:=encoding.CodePage,
                                    deli:=delimiter
            )

            Try
                If is_filepath Then
                    Call s.TryCast(Of System.IO.Stream).Flush()
                    Call s.TryCast(Of System.IO.Stream).Dispose()
                End If
            Catch ex As Exception

            End Try

            Return result
        End If
    End Function

    <Extension>
    Private Function translateSingleNtSeq(translTable As TranslTable,
                                          nt As FastaSeq,
                                          table As GeneticCodes,
                                          bypassStop As Boolean,
                                          checkNt As Boolean) As FastaSeq

        If table = GeneticCodes.Auto Then
            Dim fa = TranslationTable.Translate(nt)
            fa.Headers = nt.Headers.Join(fa.Headers).ToArray
            Return fa
        Else
            Return New FastaSeq With {
                .Headers = nt.Headers.ToArray,
                .SequenceData = translTable.Translate(
                    nucleicAcid:=nt.SequenceData,
                    bypassStop:=bypassStop,
                    checkNt:=checkNt
                )
            }
        End If
    End Function

    ''' <summary>
    ''' Do translation of the nt sequence to protein sequence
    ''' </summary>
    ''' <param name="nt">
    ''' The given fasta collection, which can be a single <see cref="FastaSeq"/> 
    ''' object, a <see cref="FastaFile"/> object, a collection of the 
    ''' <see cref="FastaSeq"/> object, or a character vector of the raw nucleotide 
    ''' sequence data.
    ''' </param>
    ''' <param name="table">The genetic code for translation table.</param>
    ''' <param name="bypassStop">
    ''' Try ignores of the stop codon.
    ''' </param>
    ''' <param name="checkNt">
    ''' check the input nucleotide sequence data is a valid nucleotide sequence? 
    ''' if this parameter is TRUE and the input sequence data contains the invalid 
    ''' nucleotide letters, then an error will be thrown.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a protein <see cref="FastaSeq"/> object if the input is a single nucleotide 
    ''' sequence, or a <see cref="FastaFile"/> protein sequence collection if the 
    ''' input is a collection of the nucleotide sequence data;
    ''' 
    ''' this function returns a R# error message object if the input sequence data 
    ''' can not be cast to a nucleotide fasta sequence collection.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' when the ``bypassStop`` parameter is TRUE and there are some invalid gene 
    ''' sequence that contains the stop codon symbol in the translated protein 
    ''' sequence, a warning message will be pushed into the R# environment message 
    ''' buffer.
    ''' </remarks>
    <ExportAPI("translate")>
    Public Function Translates(<RRawVectorArgument>
                               nt As Object,
                               Optional table As GeneticCodes = GeneticCodes.BacterialArchaealAndPlantPlastidCode,
                               Optional bypassStop As Boolean = True,
                               Optional checkNt As Boolean = True,
                               Optional env As Environment = Nothing) As Object

        Dim translTable As TranslTable = TranslTable.GetTable(index:=table)

        If nt Is Nothing Then
            Return Nothing
        ElseIf TypeOf nt Is FastaSeq Then
            Return translTable.translateSingleNtSeq(DirectCast(nt, FastaSeq), table, bypassStop, checkNt)
        Else
            Dim collection As IEnumerable(Of FastaSeq) = GetFastaSeq(nt, env)

            If collection Is Nothing Then
                Return REnv.Internal.debug.stop(New NotImplementedException(nt.GetType.FullName), env)
            Else
                Dim prot As New FastaFile
                Dim fa As FastaSeq
                Dim checkInvalids As New List(Of String)

                For Each ntSeq As FastaSeq In collection
                    fa = translTable.translateSingleNtSeq(ntSeq, table, bypassStop, checkNt)

                    If bypassStop Then
                        If fa.SequenceData.Any(Function(c) c = TranslTable.SymbolStopCoden) Then
                            checkInvalids += fa.Title
                        End If
                    End If

                    prot.Add(fa)
                Next

                If bypassStop AndAlso checkInvalids > 0 Then
                    Call env.AddMessage({
                        $"There are {checkInvalids.Count} gene sequence is invalids under current genetic code.",
                        $"genetic_code: {table.Description}"
                    }.Join(checkInvalids.Select(Function(seq) $"invalid: {seq}")).ToArray, MSG_TYPES.WRN)
                End If

                Return prot
            End If
        End If
    End Function

    ''' <summary>
    ''' Do multiple sequence alignment
    ''' </summary>
    ''' <param name="seqs">
    ''' A fasta sequence collection, which can be a <see cref="FastaFile"/> object, 
    ''' a collection of the <see cref="FastaSeq"/> object, or a character vector of 
    ''' the raw sequence data.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' an <see cref="MSAOutput"/> object that contains the multiple sequence 
    ''' alignment result: the aligned sequence data of each input sequence and the 
    ''' alignment cost value.
    ''' </returns>
    <ExportAPI("MSA.of")>
    Public Function MSA(<RRawVectorArgument> seqs As Object, Optional env As Environment = Nothing) As MSAOutput
        Return GetFastaSeq(seqs, env).MultipleAlignment(ScoreMatrix.DefaultMatrix)
    End Function

    ''' <summary>
    ''' read stockholm MSA file.
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the stockholm format multiple sequence alignment file.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="Stockholm"/> alignment object that contains the 
    ''' aligned sequence data of the target stockholm file.
    ''' </returns>
    <ExportAPI("read_stockholm")>
    <RApiReturn(GetType(Stockholm))>
    Public Function read_stockholm(file As String) As Object
        Return Reader.Read(file).ToArray
    End Function

    ''' <summary>
    ''' Create a fasta sequence collection object from any given sequence collection.
    ''' </summary>
    ''' <param name="x">
    ''' any type of sequence collection, which can be:
    ''' 
    ''' 1. a <see cref="FastaFile"/> object or a collection of the 
    '''    <see cref="FastaSeq"/> object;
    ''' 2. a multiple sequence alignment result(<see cref="MSAOutput"/>);
    ''' 3. a set of the <see cref="SimpleSegment"/> sequence segment object;
    ''' 4. a sequence motif object(<see cref="SequenceMotif"/>);
    ''' 5. a ncbi genbank feature object(``Feature``) for extract the nucleotide 
    '''    sequence data of the target feature site;
    ''' 6. a fastq sequence collection or a character vector of the raw sequence 
    '''    data.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a <see cref="FastaFile"/> sequence collection object that created from the 
    ''' given sequence data source;
    ''' 
    ''' this function returns a R# error message object if the input data source 
    ''' can not be cast to a fasta sequence collection.
    ''' </returns>
    ''' <keywords>conversion</keywords>
    <ExportAPI("as.fasta")>
    <RApiReturn(GetType(FastaFile))>
    Public Function Tofasta(<RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As Object
        If x Is Nothing Then
            Return Nothing
        ElseIf x.GetType Is GetType(MSAOutput) Then
            Return DirectCast(x, MSAOutput).ToFasta
        ElseIf x.GetType Is GetType(SimpleSegment()) Then
            Return DirectCast(x, SimpleSegment()) _
                .Select(Function(sg) sg.SimpleFasta) _
                .DoCall(Function(seqs)
                            Return New FastaFile(seqs)
                        End Function)
        ElseIf x.GetType Is GetType(SequenceMotif) Then
            Dim motif As SequenceMotif = DirectCast(x, SequenceMotif)
            Dim fasta As FastaFile = motif.seeds.names _
                .Select(Function(name, i)
                            Return New FastaSeq With {
                                .Headers = {name},
                                .SequenceData = motif.seeds.MSA(i)
                            }
                        End Function) _
                .DoCall(Function(seqs)
                            Return New FastaFile(seqs)
                        End Function)

            Return fasta
        ElseIf TypeOf x Is GBFF.Keywords.FEATURES.Feature Then
            Dim feature As GBFF.Keywords.FEATURES.Feature = x
            Dim fa As New FastaSeq With {
               .SequenceData = Strings.UCase(feature.SequenceData),
               .Headers = {feature.Query(FeatureQualifiers.gene), feature.Location.ToString}
            }

            Return fa
        Else
            Dim collection As IEnumerable(Of FastaSeq) = GetFastaSeq(x, env)

            If collection Is Nothing Then
                Dim pullFq As pipeline = pipeline.TryCreatePipeline(Of FastQ)(x, env)

                If Not pullFq.isError Then
                    If TypeOf x Is FastQFile Then
                        pullFq = pipeline.CreateFromPopulator(DirectCast(x, FastQFile).ToArray)
                    End If
                    Return New FastaFile(From fq As FastQ In pullFq.populates(Of FastQ)(env) Select New FastaSeq(fq.SequenceData, title:=fq.SEQ_ID))
                End If

                If x.GetType.IsArray Then
                    If DirectCast(x, Array).AsObjectEnumerator.All(Function(a) TypeOf a Is SimpleSegment) Then
                        Return DirectCast(x, Array) _
                            .AsObjectEnumerator(Of SimpleSegment) _
                            .Select(Function(sg) sg.SimpleFasta) _
                            .DoCall(Function(seqs)
                                        Return New FastaFile(seqs)
                                    End Function)
                    ElseIf DirectCast(x, Array).AsObjectEnumerator.All(Function(a) TypeOf a Is FastaSeq) Then
                        Return DirectCast(x, Array) _
                            .AsObjectEnumerator(Of FastaSeq) _
                            .DoCall(Function(seqs) New FastaFile(seqs))
                    End If
                End If

                Return REnv.Internal.debug.stop(New NotImplementedException(x.GetType.FullName), env)
            Else
                Return New FastaFile(collection)
            End If
        End If
    End Function

    ''' <summary>
    ''' Create a new fasta sequence objects
    ''' </summary>
    ''' <param name="seq">the raw sequence data text of the target fasta sequence.</param>
    ''' <param name="attrs">
    ''' a character vector of the fasta headers data: the first element of this 
    ''' vector is the sequence id, and the other elements are the description 
    ''' information of the target sequence.
    ''' </param>
    ''' <returns>a new <see cref="FastaSeq"/> sequence object.</returns>
    <ExportAPI("fasta")>
    Public Function fasta(seq$, attrs As String()) As Object
        Return New FastaSeq With {
            .Headers = attrs,
            .SequenceData = seq
        }
    End Function

    ''' <summary>
    ''' get/set the fasta headers title
    ''' </summary>
    ''' <param name="fa">
    ''' a <see cref="FastaSeq"/> sequence object for get or set the headers title 
    ''' data.
    ''' </param>
    ''' <param name="headers">
    ''' a character vector of the new fasta headers data for overwrite the headers 
    ''' data of the given sequence object. If this parameter is not specified(or is 
    ''' an empty vector), then the headers data of the given sequence object will 
    ''' not be modified, and the current headers data will be returned.
    ''' </param>
    ''' <returns>
    ''' a character vector of the fasta headers title data of the given sequence 
    ''' object.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' this api can be used as a property setter in R# environment: the headers 
    ''' data of the given fasta sequence object can be overwritten via the value 
    ''' assign syntax:
    ''' 
    ''' ```r
    ''' fasta.headers(seq) &lt;- c("seq_id", "description");
    ''' ```
    ''' </remarks>
    <ExportAPI("fasta.headers")>
    Public Function fastaTitle(fa As FastaSeq, <RByRefValueAssign> Optional headers As String() = Nothing) As String()
        If Not headers.IsNullOrEmpty Then
            fa.Headers = headers
        End If

        Return fa.Headers
    End Function

    ''' <summary>
    ''' get the fasta titles from a collection of fasta sequence
    ''' </summary>
    ''' <param name="fa">
    ''' a fasta sequence collection, which can be a <see cref="FastaFile"/> object, 
    ''' a collection of the <see cref="FastaSeq"/> object, or a character vector of 
    ''' the raw sequence data.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a character vector of the fasta title text of each sequence in the given 
    ''' fasta sequence collection.
    ''' </returns>
    <ExportAPI("fasta.titles")>
    Public Function fastaTitles(<RRawVectorArgument> fa As Object, Optional env As Environment = Nothing) As String()
        Return GetFastaSeq(fa, env) _
            .Select(Function(a) a.Title) _
            .ToArray
    End Function

    ''' <summary>
    ''' create a sequence region slicer for cut a specific sequence region from 
    ''' the given sequence data
    ''' </summary>
    ''' <param name="fa">
    ''' the target sequence data source, which can be:
    ''' 
    ''' 1. a <see cref="FastaSeq"/> sequence object, then a 
    '''    <see cref="FastaSlicer"/> will be created;
    ''' 2. a chromosome or contigs sequence object(<see cref="ChunkedNtFasta"/>) 
    '''    that is read from the genome assembly sequence file via the 
    '''    ``read_assembly`` api, then a <see cref="ChunkSlicer"/> will be created;
    ''' 3. a ncbi genbank database file object(``GBFF.File``), then a 
    '''    <see cref="GenBankSlicer"/> will be created.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' an <see cref="ISlicer"/> object for slice the sequence region from the 
    ''' given sequence data source;
    ''' 
    ''' this function returns a R# error message object if the given sequence data 
    ''' source is not a supported sequence data model.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' the slicer object is used for cut a sequence region from a huge genome 
    ''' sequence in a memory efficient manner, which is very helpful for the 
    ''' sequence data extraction of a specific gene locus site.
    ''' </remarks>
    <ExportAPI("slicer")>
    <RApiReturn(GetType(ISlicer), GetType(FastaSlicer), GetType(ChunkSlicer), GetType(GenBankSlicer))>
    Public Function slicer(fa As Object, Optional env As Environment = Nothing) As Object
        If TypeOf fa Is FastaSeq Then
            Return New FastaSlicer(DirectCast(fa, FastaSeq))
        ElseIf TypeOf fa Is ChunkedNtFasta Then
            Return New ChunkSlicer(DirectCast(fa, ChunkedNtFasta))
        ElseIf TypeOf fa Is GBFF.File Then
            Return New GenBankSlicer(DirectCast(fa, GBFF.File))
        Else
            Return Message.InCompatibleType(GetType(FastaSeq), fa.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' cut part of the sequence
    ''' </summary>
    ''' <param name="seq">
    ''' the target sequence data source, which can be a single 
    ''' <see cref="FastaSeq"/> object, a collection of the <see cref="FastaSeq"/> 
    ''' object, or a character vector of the raw sequence data.
    ''' </param>
    ''' <param name="loci">
    ''' the location region data for make cut of the sequence site, data model could be:
    ''' 
    ''' 1. for nucleotide sequence, <see cref="NucleotideLocation"/> should be used,
    ''' 2. for general sequence data, <see cref="SMRUCC.genomics.ComponentModel.Loci.Location"/> should be used.
    ''' </param>
    ''' <param name="nt_auto_reverse">
    ''' make auto reverse of the nucleotide sequence if the given location is on 
    ''' the <see cref="Strands.Reverse"/> direction.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a new <see cref="FastaSeq"/> object of the cut sequence fragment if the 
    ''' input is a single sequence object, or a <see cref="FastaFile"/> object of 
    ''' the cut sequence fragments of each input sequence if the input is a 
    ''' sequence collection;
    ''' 
    ''' this function returns a R# error message object if the given location 
    ''' information is nothing, or the input sequence data can not be cast to a 
    ''' fasta sequence collection.
    ''' </returns>
    <ExportAPI("cut_seq.linear")>
    Public Function CutSequenceLinear(<RRawVectorArgument> seq As Object,
                                      <RRawVectorArgument> loci As Object,
                                      Optional nt_auto_reverse As Boolean = False,
                                      Optional env As Environment = Nothing) As Object
        If seq Is Nothing Then
            Return Nothing
        ElseIf loci Is Nothing Then
            Return REnv.Internal.debug.stop("Location information can not be null!", env)
        End If

        Dim left, right As Integer
        Dim getAttrs As Func(Of FastaSeq, String())
        Dim reverse As Boolean = False

        If TypeOf loci Is SMRUCC.genomics.ComponentModel.Loci.Location Then
            With DirectCast(loci, SMRUCC.genomics.ComponentModel.Loci.Location)
                left = .Min
                right = .Max
                getAttrs = Function(fa) {fa.Headers.JoinBy("|") & " " & $"[{left}, {right}]"}
            End With
        ElseIf TypeOf loci Is NucleotideLocation Then
            With DirectCast(loci, NucleotideLocation)
                left = .Min
                right = .Max
                getAttrs = Function(fa) {fa.Headers.JoinBy("|") & " " & .tagStr}

                If nt_auto_reverse AndAlso .Strand = Strands.Reverse Then
                    reverse = True
                End If
            End With
        Else
            With CLRVector.asLong(loci)
                left = .GetValue(0)
                right = .GetValue(1)
                getAttrs = Function(fa) {fa.Headers.JoinBy("|") & " " & $"[{left}, {right}]"}
            End With
        End If

        If TypeOf seq Is FastaSeq Then
            Dim fa As FastaSeq = DirectCast(seq, FastaSeq)
            Dim sequence As SimpleSegment = fa.CutSequenceLinear(left, right)

            If reverse Then
                sequence.SequenceData = sequence.SequenceData.Reverse.CharString
            End If

            Return New FastaSeq With {
                .Headers = getAttrs(fa),
                .SequenceData = sequence.SequenceData
            }
        Else
            Dim collection As IEnumerable(Of FastaSeq) = GetFastaSeq(NT, env)

            If collection Is Nothing Then
                Return REnv.Internal.debug.stop(New NotImplementedException(NT.GetType.FullName), env)
            Else
                collection = collection _
                    .Select(Function(fa)
                                Dim sequence = fa.CutSequenceLinear(left, right)
                                Dim fragment As New FastaSeq With {
                                    .Headers = getAttrs(fa),
                                    .SequenceData = sequence.SequenceData
                                }

                                If reverse Then
                                    fragment.SequenceData = fragment.SequenceData.Reverse.CharString
                                End If

                                Return fragment
                            End Function) _
                    .ToArray

                Return New FastaFile(collection)
            End If
        End If
    End Function

    ''' <summary>
    ''' open a fingerprint matrix writer for write the sequence fingerprint data 
    ''' into a binary BSON file
    ''' </summary>
    ''' <param name="file">
    ''' the output target: a file path of the generated fingerprint matrix file, or 
    ''' a file stream object for write the fingerprint data.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a <see cref="FingerprintMatrixWriter"/> object for write the sequence 
    ''' fingerprint data into the target file in a stream manner, which can be used 
    ''' by the ``write_fingerprint`` api;
    ''' 
    ''' this function returns a R# error message object if the target file can not 
    ''' be opened for write.
    ''' </returns>
    <ExportAPI("open.fingerprint_writer")>
    <RApiReturn(GetType(FingerprintMatrixWriter))>
    Public Function openFingerpintWriter(file As Object, Optional env As Environment = Nothing) As Object
        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Return New FingerprintMatrixWriter(s.TryCast(Of IO.Stream))
    End Function

    ''' <summary>
    ''' make the sequence fingerprint data of the given nucleotide sequence 
    ''' collection, and then write the generated fingerprint data into the target 
    ''' fingerprint matrix file
    ''' </summary>
    ''' <param name="file">
    ''' a <see cref="FingerprintMatrixWriter"/> object that is created by the 
    ''' ``open.fingerprint_writer`` api.
    ''' </param>
    ''' <param name="seqs">
    ''' a nucleotide fasta sequence collection for make the sequence fingerprint 
    ''' data, which can be a <see cref="FastaFile"/> object, a collection of the 
    ''' <see cref="FastaSeq"/> object, or a character vector of the raw sequence 
    ''' data.
    ''' </param>
    ''' <param name="debug">
    ''' only make the fingerprint data of the first n sequence for debug test? a 
    ''' negative value means that all of the input sequence will be processed.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' the input <see cref="FingerprintMatrixWriter"/> object, so that this api can 
    ''' be used in a pipeline manner;
    ''' 
    ''' this function returns a R# error message object if the input sequence data 
    ''' can not be cast to a fasta sequence collection.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' the fasta headers title of the input sequence data should be formatted as: 
    ''' ``{gb_acc}.{locus_tag} {left} {right} {strand}|{biom_string}``, the 
    ''' ``strand`` token should be ``forward`` or ``reverse``, or the target 
    ''' sequence will be skipped with a warning message.
    ''' </remarks>
    <ExportAPI("write_fingerprint")>
    <RApiReturn(GetType(FingerprintMatrixWriter))>
    Public Function createFingerprintMatrix(file As FingerprintMatrixWriter, <RRawVectorArgument> seqs As Object,
                                            Optional debug As Integer = -1,
                                            Optional env As Environment = Nothing) As Object
        Dim fasta = GetFastaSeq(seqs, env)

        If fasta Is Nothing Then
            Return Message.InCompatibleType(GetType(FastaFile), seqs.GetType, env)
        End If

        If debug > 0 Then
            For Each seed As NTCluster In NTCluster.MakeFingerprint(fasta).Take(debug)
                Call file.Add(seed)
                Call seed.ToString.info
            Next
        Else
            For Each seed As NTCluster In NTCluster.MakeFingerprint(fasta)
                Call file.Add(seed)
                Call seed.ToString.info
            Next
        End If

        Return file
    End Function

    ''' <summary>
    ''' read the sequence fingerprint data from a binary BSON format fingerprint 
    ''' matrix file
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the fingerprint matrix file that is generated by the 
    ''' ``write_fingerprint`` api, or a file stream object of the target 
    ''' fingerprint matrix file.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a pipeline object of the <see cref="NTCluster"/> sequence fingerprint data;
    ''' 
    ''' this function returns a R# error message object if the given file can not be 
    ''' opened for read.
    ''' </returns>
    <ExportAPI("read.fingerprint_bson")>
    <RApiReturn(GetType(NTCluster))>
    Public Function readFingerprintBson(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Return FingerprintMatrixWriter.BSONReader(s.TryCast(Of IO.Stream))
    End Function

    ''' <summary>
    ''' make the cluster tree of the given sequence fingerprint data
    ''' </summary>
    ''' <param name="fingerprints">
    ''' a collection of the <see cref="NTCluster"/> sequence fingerprint data, which 
    ''' can be the output of the ``read.fingerprint_bson`` api, or a pipeline object 
    ''' that produces a set of the <see cref="NTCluster"/> fingerprint data.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a vector of the <see cref="NTCluster"/> fingerprint data that the ``cluster`` 
    ''' property of the fingerprint object has been assigned with the cluster id of 
    ''' the corresponding cluster: the fingerprints are grouped by the cluster id, 
    ''' and the clusters are sorted by the cluster size in descending order;
    ''' 
    ''' this function returns a R# error message object if the input data can not be 
    ''' cast to a collection of the <see cref="NTCluster"/> fingerprint data.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' the cluster tree is built based on the fingerprint similarity: the 
    ''' fingerprint data will be clustered into the same cluster when the 
    ''' similarity between them is greater than or equals to 0.8, and the 
    ''' fingerprints that their similarity is greater than 0.6 will be treated as 
    ''' the neighbours of each other.
    ''' </remarks>
    <ExportAPI("make_clusterTree")>
    <RApiReturn(GetType(NTCluster))>
    Public Function makeClusterTree(<RRawVectorArgument> fingerprints As Object, Optional env As Environment = Nothing) As Object
        Dim seeds = pipeline.TryCreatePipeline(Of NTCluster)(fingerprints, env)

        If seeds.isError Then
            Return seeds.getError
        End If

        Dim tree As New NTTree(0.8, 0.6)
        Call tree.MakeTtree(seeds.populates(Of NTCluster)(env))
        Dim cluster = tree.GetClusters _
            .GroupBy(Function(a) a.cluster) _
            .OrderByDescending(Function(a) a.Count) _
            .IteratesALL _
            .ToArray

        Return cluster
    End Function
End Module
