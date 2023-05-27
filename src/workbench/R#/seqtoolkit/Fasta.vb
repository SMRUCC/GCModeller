#Region "Microsoft.VisualBasic::77a488daa8099cfa7dee55ffa8942895, R#\seqtoolkit\Fasta.vb"

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

'   Total Lines: 464
'    Code Lines: 349
' Comment Lines: 63
'   Blank Lines: 52
'     File Size: 18.86 KB


' Module Fasta
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: CutSequenceLinear, fasta, GetFastaSeq, MSA, openFasta
'               readFasta, readSeq, SequenceAssembler, sizeof, Tofasta
'               Translates, viewAssembles, viewFasta, viewMSA, writeFasta
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' Fasta sequence toolkit
''' </summary>
''' 
<Package("bioseq.fasta", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
Module Fasta

    Sub New()
        Call printer.AttachConsoleFormatter(Of FastaSeq)(AddressOf viewFasta)
        Call printer.AttachConsoleFormatter(Of FastaFile)(AddressOf viewFasta)
        Call printer.AttachConsoleFormatter(Of MSAOutput)(AddressOf viewMSA)
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
    ''' get the sequence length
    ''' </summary>
    ''' <param name="fa"></param>
    ''' <returns></returns>
    <ExportAPI("size")>
    Public Function sizeof(fa As FastaSeq) As Integer
        If fa Is Nothing Then
            Return 0
        Else
            Return fa.Length
        End If
    End Function

    ''' <summary>
    ''' Read a single fasta sequence file
    ''' </summary>
    ''' <param name="file">
    ''' Just contains one sequence
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("read.seq")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function readSeq(file As String, Optional env As Environment = Nothing) As Object
        Dim firstLine As String = file.ReadFirstLine

        If firstLine.First = ">"c Then
            Return FastaSeq.Load(file)
        ElseIf firstLine.StartsWith("LOCUS") Then
            Return GBFF.File.Load(file).Origin.ToFasta
        Else
            Return Internal.debug.stop({"invalid file format!", "file: " & file, $"required: *.fa, *.gbk"}, env)
        End If
    End Function

    ''' <summary>
    ''' read a fasta sequence collection file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
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

    <ExportAPI("open.fasta")>
    Public Function openFasta(file As String, Optional env As Environment = Nothing)
        Return StreamIterator.SeqSource(file).DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    ''' <summary>
    ''' write a fasta sequence or a collection of fasta sequence object
    ''' </summary>
    ''' <param name="seq"></param>
    ''' <param name="file"></param>
    ''' <param name="lineBreak">
    ''' The sequence length in one line, negative value or ZERo means no line break.
    ''' </param>
    ''' <param name="encoding">The text encoding value of the generated fasta file.</param>
    ''' <returns></returns>
    <ExportAPI("write.fasta")>
    Public Function writeFasta(<RRawVectorArgument> seq As Object, file$,
                               Optional lineBreak% = -1,
                               Optional delimiter As String = " ",
                               Optional encoding As Encodings = Encodings.ASCII,
                               Optional env As Environment = Nothing) As Boolean

        If TypeOf seq Is pipeline Then
            ' save a huge bundle of the fasta sequence collection
            Using buffer As New StreamWriter(file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False))
                For Each fa As FastaSeq In DirectCast(seq, pipeline).populates(Of FastaSeq)(env)
                    Call buffer.WriteLine(fa.GenerateDocument(
                        lineBreak:=lineBreak,
                        [overrides]:=False,
                        delimiter:=delimiter
                    ))
                Next

                Call buffer.Flush()
            End Using

            Return True
        Else
            ' save a collection of the fasta sequence
            Dim seqs = GetFastaSeq(seq, env).ToArray
            Dim fasta As New FastaFile(seqs)

            Return fasta.Save(lineBreak, file, encoding)
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
    ''' <param name="nt">The given fasta collection</param>
    ''' <param name="table">The genetic code for translation table.</param>
    ''' <param name="bypassStop">
    ''' Try ignores of the stop codon.
    ''' </param>
    ''' <returns></returns>
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
    ''' <param name="seqs">A fasta sequence collection</param>
    ''' <returns></returns>
    <ExportAPI("MSA.of")>
    Public Function MSA(<RRawVectorArgument> seqs As Object, Optional env As Environment = Nothing) As MSAOutput
        Return GetFastaSeq(seqs, env).MultipleAlignment(ScoreMatrix.DefaultMatrix)
    End Function

    ''' <summary>
    ''' Create a fasta sequence collection object from any given sequence collection.
    ''' </summary>
    ''' <param name="x">any type of sequence collection</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
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
        Else
            Dim collection As IEnumerable(Of FastaSeq) = GetFastaSeq(x, env)

            If collection Is Nothing Then
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
    ''' <param name="seq"></param>
    ''' <param name="attrs"></param>
    ''' <returns></returns>
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
    ''' <param name="fa"></param>
    ''' <param name="headers"></param>
    ''' <returns></returns>
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
    ''' <param name="fa"></param>
    ''' <returns></returns>
    <ExportAPI("fasta.titles")>
    Public Function fastaTitles(<RRawVectorArgument> fa As Object, Optional env As Environment = Nothing) As String()
        Return GetFastaSeq(fa, env) _
            .Select(Function(a) a.Title) _
            .ToArray
    End Function

    <ExportAPI("cut_seq.linear")>
    Public Function CutSequenceLinear(<RRawVectorArgument> seq As Object,
                                      <RRawVectorArgument> loci As Object,
                                      Optional doNtAutoReverse As Boolean = False,
                                      Optional env As Environment = Nothing) As Object
        If seq Is Nothing Then
            Return Nothing
        ElseIf loci Is Nothing Then
            Return REnv.Internal.debug.stop("Location information can not be null!", env)
        End If

        Dim left, right As Integer
        Dim getAttrs As Func(Of FastaSeq, String())
        Dim reverse As Boolean = False

        If TypeOf loci Is Location Then
            With DirectCast(loci, Location)
                left = .Min
                right = .Max
                getAttrs = Function(fa) {fa.Headers.JoinBy("|") & " " & $"[{left}, {right}]"}
            End With
        ElseIf TypeOf loci Is NucleotideLocation Then
            With DirectCast(loci, NucleotideLocation)
                left = .Min
                right = .Max
                getAttrs = Function(fa) {fa.Headers.JoinBy("|") & " " & .tagStr}

                If doNtAutoReverse AndAlso .Strand = Strands.Reverse Then
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
End Module
