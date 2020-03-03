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
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' Fasta sequence toolkit
''' </summary>
''' 
<Package("bioseq.fasta")>
Module Fasta

    Sub New()
        Call printer.AttachConsoleFormatter(Of FastaSeq)(AddressOf viewFasta)
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

        Select Case seq
            Case GetType(FastaSeq)
                With DirectCast(seq, FastaSeq)
                    Return "> " & .Title & ASCII.LF & .SequenceData
                End With
            Case GetType(FastaFile)
                Return DirectCast(seq, FastaFile).Select(Function(fa) "> " & fa.Title).JoinBy(vbCrLf)
            Case Else
                Throw New NotImplementedException
        End Select
    End Function

    Public Function GetFastaSeq(a As Object) As IEnumerable(Of FastaSeq)
        If a Is Nothing Then
            Return {}
        Else
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
    Public Function readSeq(file As String) As FastaSeq
        Return FastaSeq.Load(file)
    End Function

    <ExportAPI("read.fasta")>
    Public Function readFasta(file As String) As FastaFile
        Return FastaFile.Read(file)
    End Function

    <ExportAPI("write.fasta")>
    Public Function writeFasta(seq As Object, file$, Optional lineBreak% = -1, Optional encoding As Encodings = Encodings.ASCII) As Boolean
        Return New FastaFile(GetFastaSeq(seq)).Save(lineBreak, file, encoding)
    End Function

    ''' <summary>
    ''' Do translation of the nt sequence to protein sequence
    ''' </summary>
    ''' <param name="nt"></param>
    ''' <returns></returns>
    <ExportAPI("translate")>
    Public Function Translates(<RRawVectorArgument>
                               nt As Object,
                               Optional code As GeneticCodes = GeneticCodes.BacterialArchaealAndPlantPlastidCode,
                               Optional forceStop As Boolean = True,
                               Optional env As Environment = Nothing) As Object

        Dim translTable As TranslTable = TranslTable.GetTable(code)

        If nt Is Nothing Then
            Return Nothing
        ElseIf TypeOf nt Is FastaSeq Then
            Return New FastaSeq With {
                .Headers = DirectCast(nt, FastaSeq).Headers.ToArray,
                .SequenceData = translTable.Translate(DirectCast(nt, FastaSeq).SequenceData, forceStop)
            }
        ElseIf TypeOf nt Is FastaFile OrElse TypeOf nt Is FastaSeq() Then
            Dim prot As New FastaFile
            Dim fa As FastaSeq

            For Each ntSeq As FastaSeq In DirectCast(nt, IEnumerable(Of FastaSeq))
                fa = New FastaSeq With {
                    .Headers = ntSeq.Headers.ToArray,
                    .SequenceData = translTable.Translate(ntSeq.SequenceData, forceStop)
                }
                prot.Add(fa)
            Next

            Return prot
        Else
            Dim collection As IEnumerable(Of FastaSeq) = GetFastaSeq(nt)

            If collection Is Nothing Then
                Return REnv.Internal.debug.stop(New NotImplementedException(nt.GetType.FullName), env)
            Else
                Dim prot As New FastaFile
                Dim fa As FastaSeq

                For Each ntSeq As FastaSeq In collection
                    fa = New FastaSeq With {
                    .Headers = ntSeq.Headers.ToArray,
                    .SequenceData = translTable.Translate(ntSeq.SequenceData, forceStop)
                }
                    prot.Add(fa)
                Next

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

    <ExportAPI("fasta")>
    <RApiReturn(GetType(FastaFile))>
    Public Function Tofasta(<RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As Object
        If x Is Nothing Then
            Return Nothing
        ElseIf x.GetType Is GetType(MSAOutput) Then
            Return DirectCast(x, MSAOutput).ToFasta
        Else
            Dim collection As IEnumerable(Of FastaSeq) = GetFastaSeq(x)

            If collection Is Nothing Then
                Return REnv.Internal.debug.stop(New NotImplementedException(x.GetType.FullName), env)
            Else
                Return New FastaFile(collection)
            End If
        End If
    End Function

    <ExportAPI("Assemble.of")>
    Public Function SequenceAssembler(<RRawVectorArgument> reads As Object, Optional env As Environment = Nothing) As Object
        Dim readSeqs As FastaSeq() = GetFastaSeq(reads).ToArray
        Dim data As String() = readSeqs _
            .Select(Function(fa) fa.SequenceData) _
            .ToArray
        Dim result = data.ShortestCommonSuperString

        Return New AssembleResult(result)
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

