#Region "Microsoft.VisualBasic::96ee32bf3700bb8a62c3a6c3f8ee6fdb, R#\seqtoolkit\Fasta.vb"

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

' Module Fasta
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: MSA, readFasta, readSeq, viewFasta
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
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
        Call printer.AttachConsoleFormatter(Of AssembleResult)(AddressOf viewAssembles)
    End Sub

    Private Function viewAssembles(asm As AssembleResult) As String
        Dim sb As New StringBuilder

        Using text As New StringWriter(sb)
            Call asm.alignments.TableView(asm.GetAssembledSequence, text)
        End Using

        Return sb.ToString
    End Function

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

    Public Function GetFastaSeq(a As Object) As IEnumerable(Of FastaSeq)
        If a Is Nothing Then
            Return {}
        ElseIf TypeOf a Is vector Then
            a = DirectCast(a, vector).data
        End If

        Dim type As Type = a.GetType

        Select Case type
            Case GetType(FastaSeq)
                Return {DirectCast(a, FastaSeq)}
            Case GetType(FastaFile)
                Return DirectCast(a, FastaFile)
            Case GetType(FastaSeq())
                Return a
            Case Else
                If type.IsArray AndAlso REnv.MeasureArrayElementType(a) Is GetType(FastaSeq) Then
                    Dim populator As IEnumerable(Of FastaSeq) =
                        Iterator Function() As IEnumerable(Of FastaSeq)
                            Dim vec As Array = DirectCast(a, Array)

                            For i As Integer = 0 To vec.Length - 1
                                Yield DirectCast(vec.GetValue(i), FastaSeq)
                            Next
                        End Function()

                    Return populator
                Else
                    Return Nothing
                End If
        End Select
    End Function

    ''' <summary>
    ''' Read a single fasta sequence file
    ''' </summary>
    ''' <param name="file">
    ''' Just contains one sequence
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("read.seq")>
    Public Function readSeq(file As String) As FastaSeq
        Return FastaSeq.Load(file)
    End Function

    <ExportAPI("read.fasta")>
    Public Function readFasta(file As String) As FastaFile
        Return FastaFile.Read(file)
    End Function

    ''' <summary>
    ''' 
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
                               Optional encoding As Encodings = Encodings.ASCII) As Boolean

        Return New FastaFile(GetFastaSeq(seq)).Save(lineBreak, file, encoding)
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
            If table = GeneticCodes.Auto Then
                Dim prot = TranslationTable.Translate(DirectCast(nt, FastaSeq))
                prot.Headers = DirectCast(nt, FastaSeq).Headers.Join(prot.Headers).ToArray
                Return prot
            Else
                Return New FastaSeq With {
                    .Headers = DirectCast(nt, FastaSeq).Headers.ToArray,
                    .SequenceData = translTable.Translate(DirectCast(nt, FastaSeq), bypassStop, checkNt)
                }
            End If
        Else
            Dim collection As IEnumerable(Of FastaSeq) = GetFastaSeq(nt)

            If collection Is Nothing Then
                Return REnv.Internal.debug.stop(New NotImplementedException(nt.GetType.FullName), env)
            Else
                Dim prot As New FastaFile
                Dim fa As FastaSeq
                Dim checkInvalids As New List(Of String)

                For Each ntSeq As FastaSeq In collection
                    If table = GeneticCodes.Auto Then
                        fa = TranslationTable.Translate(ntSeq)
                        fa.Headers = ntSeq.Headers.Join(fa.Headers).ToArray
                    Else
                        fa = New FastaSeq With {
                            .Headers = ntSeq.Headers.ToArray,
                            .SequenceData = translTable.Translate(
                                nucleicAcid:=ntSeq.SequenceData,
                                bypassStop:=bypassStop,
                                checkNt:=checkNt
                            )
                        }
                    End If

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
    ''' <param name="seqs"></param>
    ''' <returns></returns>
    <ExportAPI("MSA.of")>
    Public Function MSA(<RRawVectorArgument> seqs As Object) As MSAOutput
        Return GetFastaSeq(seqs).MultipleAlignment(ScoreMatrix.DefaultMatrix)
    End Function

    ''' <summary>
    ''' Create a fasta sequence collection object from any given sequence collection.
    ''' </summary>
    ''' <param name="x"></param>
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
        Else
            Dim collection As IEnumerable(Of FastaSeq) = GetFastaSeq(x)

            If collection Is Nothing Then
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
    ''' Do short reads assembling
    ''' </summary>
    ''' <param name="reads"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("Assemble.of")>
    Public Function SequenceAssembler(<RRawVectorArgument> reads As Object, Optional env As Environment = Nothing) As Object
        Dim readSeqs As FastaSeq() = GetFastaSeq(reads).ToArray
        Dim data As String() = readSeqs _
            .Select(Function(fa) fa.SequenceData) _
            .ToArray
        Dim result = data.ShortestCommonSuperString

        Return New AssembleResult(result)
    End Function

    <ExportAPI("cut_seq.linear")>
    Public Function CutSequenceLinear(<RRawVectorArgument> seq As Object,
                                      <RRawVectorArgument> loci As Object,
                                      Optional env As Environment = Nothing) As Object
        If seq Is Nothing Then
            Return Nothing
        ElseIf loci Is Nothing Then
            Return REnv.Internal.debug.stop("Location information can not be null!", env)
        End If

        Dim left, right As Integer

        If TypeOf loci Is Location Then
            With DirectCast(loci, Location)
                left = .Min
                right = .Max
            End With
        ElseIf TypeOf loci Is NucleotideLocation Then
            With DirectCast(loci, NucleotideLocation)
                left = .Min
                right = .Max
            End With
        Else
            With REnv.asVector(Of Long)(loci)
                left = .GetValue(0)
                right = .GetValue(1)
            End With
        End If

        If TypeOf seq Is FastaSeq Then
            Dim fa = DirectCast(seq, FastaSeq)
            Dim sequence = fa.CutSequenceLinear(left, right)

            Return New FastaSeq With {
                .Headers = fa.Headers.ToArray,
                .SequenceData = sequence.SequenceData
            }
        Else
            Dim collection As IEnumerable(Of FastaSeq) = GetFastaSeq(NT)

            If collection Is Nothing Then
                Return REnv.Internal.debug.stop(New NotImplementedException(NT.GetType.FullName), env)
            Else
                collection = collection _
                    .Select(Function(fa)
                                Dim sequence = fa.CutSequenceLinear(left, right)
                                Dim fragment As New FastaSeq With {
                                    .Headers = fa.Headers.ToArray,
                                    .SequenceData = sequence.SequenceData
                                }

                                Return fragment
                            End Function) _
                    .ToArray

                Return New FastaFile(collection)
            End If
        End If
    End Function
End Module

Public Class AssembleResult

    Friend alignments As String()

    Sub New(result As String())
        alignments = result
    End Sub

    Public Function GetAssembledSequence() As String
        Return alignments(Scan0)
    End Function
End Class

