#Region "Microsoft.VisualBasic::8b0d2f4a6c8e0b2d4f6a8c0e2b4d6f8a, analysis\SequenceToolkit\gwANI\GwaniCalculator.vb"

' Author:
'
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
'
' Copyright (c) 2018 GPL3 Licensed
'
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

Option Strict On

Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports SMRUCC.genomics.SequenceModel.FASTA


    ''' <summary>
    ''' The gwANI calculation engine.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The engine keeps the mathematics of the original panito tool bit by bit, it only
    ''' replaces the scalar character loop with a blocked, vectorized and parallel sweep:
    ''' </para>
    ''' <list type="number">
    ''' <item> <description> the encoded rows live inside a <see cref="GwaniMatrixStore"/> which is either a single managed array or a memory mapped scratch file; </description> </item>
    ''' <item> <description> the representative rows are processed in blocks, and every block streams the whole target range once, so a target row is read once per block and then reused by all of the block references, which cuts the memory traffic by the block size; </description> </item>
    ''' <item> <description> inside a block the pairs are distributed over the worker threads by target range, each thread writes to its own cells of the block result; </description> </item>
    ''' <item> <description> the rows are yielded one by one, therefore neither the engine nor the caller ever materializes the full N x N matrix. </description> </item>
    ''' </list>
    ''' <para>
    ''' Only the upper triangle of the matrix is computed, exactly like panito: the cell
    ''' below the diagonal is <see cref="GwaniValues.LowerTriangle"/> and the diagonal is
    ''' <see cref="GwaniValues.SelfIdentity"/>.
    ''' </para>
    ''' </remarks>
    Public NotInheritable Class GwaniCalculator

        ''' <summary> The upper bound of the reference block size. </summary>
        Private Const MaxBlockSize As Integer = 64

        ''' <summary> The reference block size is capped by this many result bytes. </summary>
        Private Const BlockResultBudget As Long = 128L * 1024 * 1024

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Computes the gwANI matrix row by row.
        ''' </summary>
        ''' <param name="alignment"></param>
        ''' <param name="options"></param>
        ''' <returns>
        ''' One row per distinct sequence. When the deduplication is disabled this is
        ''' the classic input ordered matrix, otherwise it is the matrix of the distinct
        ''' sequences in the order of their first occurrence.
        ''' </returns>
        Public Shared Iterator Function ComputeRows(alignment As GwaniAlignment, options As GwaniOptions) As IEnumerable(Of GwaniRow)
            If options Is Nothing Then
                options = New GwaniOptions()
            End If

            Dim store As GwaniMatrixStore = alignment.Store
            Dim u As Integer = alignment.UniqueCount
            Dim padded As Integer = store.PaddedLength
            Dim workers As Integer = options.ResolveParallelism()
            Dim useSketch As Boolean = options.EnableSketchPreFilter
            Dim blockSize As Integer = AutoBlockSize(u, useSketch)
            Dim pairs As Long() = Nothing

            If useSketch Then
                Dim sketches As ULong()() = GwaniSketch.Build(store, alignment.AlignmentLength, options)

                pairs = GwaniSketch.CandidatePairs(sketches, options)
                Report(options, $"sketch pre-filter: {pairs.Length} candidate pairs out of {CLng(u) * (u - 1) \ 2}")
            End If

            Dim i0 As Integer = 0
            Dim blockIndex As Integer = 0
            Dim blockTotal As Integer = (u + blockSize - 1) \ blockSize

            While i0 < u
                Dim count As Integer = Math.Min(blockSize, u - i0)
                Dim block As Double()() = New Double(count - 1)() {}
                Dim refs As Byte()() = store.ReadRows(i0, count)
                Dim masks As UInteger() = Nothing

                For b As Integer = 0 To count - 1
                    block(b) = New Double(u - 1) {}
                Next

                If useSketch Then
                    masks = GwaniSketch.BlockMasks(pairs, u, i0, count, i0, u)
                End If

                System.Threading.Tasks.Parallel.ForEach(
                    Partitioner.Create(i0, u),
                    New ParallelOptions With {.MaxDegreeOfParallelism = workers},
                    Sub(range)
                        Dim target As Byte() = Nothing
                        Dim buffer As Byte() = Nothing

                        If store.Backend = GwaniStoreBackend.MemoryMapped Then
                            buffer = New Byte(padded - 1) {}
                        End If

                        For j As Integer = range.Item1 To range.Item2 - 1
                            If store.Backend = GwaniStoreBackend.MemoryMapped Then
                                store.ReadRowInto(j, buffer)
                                target = buffer
                            Else
                                target = store.GetRow(j)
                            End If

                            Dim mask As UInteger = If(masks Is Nothing, &HFFFFFFFFUI, masks(j - i0))

                            For b As Integer = 0 To count - 1
                                Dim r As Integer = i0 + b

                                If j <= r Then
                                    Continue For
                                End If

                                If useSketch AndAlso (mask And (1UI << b)) = 0 Then
                                    block(b)(j) = GwaniValues.Skipped
                                    Continue For
                                End If

                                Dim matches As Integer = 0
                                Dim valids As Integer = 0

                                GwaniKernels.CountPair(refs(b), target, padded, matches, valids)
                                block(b)(j) = If(valids > 0, 100.0 * matches / valids, 0.0)
                            Next
                        Next
                    End Sub)

                For b As Integer = 0 To count - 1
                    Dim r As Integer = i0 + b
                    Dim values As Double() = block(b)

                    For j As Integer = 0 To r
                        values(j) = If(j = r, GwaniValues.SelfIdentity, GwaniValues.LowerTriangle)
                    Next

                    Yield New GwaniRow With {
                        .ID = alignment.Names(alignment.FirstOccurrence(r)),
                        .RowIndex = r,
                        .Identities = values
                    }
                Next

                i0 += count
                blockIndex += 1

                Report(options, $"gwANI block {blockIndex}/{blockTotal} finished")
            End While
        End Function

        ''' <summary>
        ''' The compatibility entry point: it returns the full input ordered matrix as
        ''' a <see cref="DataSet"/> array, exactly like the original panito port did.
        ''' The deduplication is disabled here so that the result never depends on it.
        ''' </summary>
        ''' <param name="msa">A multi-FASTA alignment.</param>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Shared Function Evaluate(msa As FastaFile, Optional options As GwaniOptions = Nothing) As DataSet()
            If options Is Nothing Then
                options = New GwaniOptions()
            End If

            Dim classic As GwaniOptions = options.CloneWithoutDeduplication()

            Using alignment As GwaniAlignment = GwaniAlignment.Build(msa, classic)
                Dim rows As New List(Of GwaniRow)(ComputeRows(alignment, classic))
                Dim matrix As Double()() = alignment.Expand(rows)

                Return ToDataSet(matrix, alignment.Names)
            End Using
        End Function

        ''' <summary>
        ''' Computes the gwANI matrix of a multi-FASTA file and writes it as a tab
        ''' separated matrix file. The rows are streamed, so the memory usage stays
        ''' bounded even for a matrix that would not fit into the memory at all.
        ''' </summary>
        ''' <param name="inPath">The input multi-FASTA alignment.</param>
        ''' <param name="outPath">The output matrix file.</param>
        ''' <param name="options"></param>
        Public Shared Sub EvaluateToCsv(inPath As String, outPath As String, options As GwaniOptions)
            If options Is Nothing Then
                options = New GwaniOptions()
            End If

            Using alignment As GwaniAlignment = GwaniAlignment.BuildFile(inPath, options)
                Dim header As String() = alignment.RowIds()

                ' the FASTA namespace also declares a StreamWriter, so the BCL type
                ' has to be fully qualified here
                Using writer As New System.IO.StreamWriter(outPath, append:=False, encoding:=Encoding.UTF8)
                    writer.Write(String.Join(vbTab, header))
                    writer.Write(vbLf)

                    For Each row As GwaniRow In ComputeRows(alignment, options)
                        writer.Write(row.ID)

                        For Each value As Double In row.Identities
                            writer.Write(vbTab)
                            writer.Write(FormatValue(value))
                        Next

                        writer.Write(vbLf)
                    Next
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Formats one matrix cell the way the original panito tool printed it.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Shared Function FormatValue(value As Double) As String
            If value = GwaniValues.LowerTriangle Then
                Return "-"
            ElseIf value = GwaniValues.Skipped Then
                Return "NA"
            Else
                Return value.ToString("F6", CultureInfo.InvariantCulture)
            End If
        End Function

        ''' <summary>
        ''' Chooses the reference block size: large enough to amortize the target reads,
        ''' small enough to keep the block result buffer inside the budget.
        ''' </summary>
        ''' <param name="uniqueCount"></param>
        ''' <param name="useSketch">
        ''' The sketch pre-filter addresses the candidates with a 32 bit bitmask, so the
        ''' block must not be wider than 32 rows in that case.
        ''' </param>
        ''' <returns></returns>
        Public Shared Function AutoBlockSize(uniqueCount As Integer, useSketch As Boolean) As Integer
            If uniqueCount <= 0 Then
                Return 1
            End If

            Dim byMemory As Long = BlockResultBudget \ (CLng(uniqueCount) * 8)
            Dim size As Integer = CInt(Math.Min(MaxBlockSize, Math.Max(1, byMemory)))

            If useSketch AndAlso size > 32 Then
                size = 32
            End If

            Return Math.Min(size, uniqueCount)
        End Function

        ''' <summary>
        ''' Converts a full matrix into the <see cref="DataSet"/> array of the
        ''' compatibility API.
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="names"></param>
        ''' <returns></returns>
        Private Shared Function ToDataSet(matrix As Double()(), names As String()) As DataSet()
            Dim result As DataSet() = New DataSet(names.Length - 1) {}

            For i As Integer = 0 To names.Length - 1
                Dim row As New DataSet With {.ID = names(i)}

                For j As Integer = 0 To names.Length - 1
                    Call row.Add(names(j), matrix(i)(j))
                Next

                result(i) = row
            Next

            Return result
        End Function

        Private Shared Sub Report(options As GwaniOptions, message As String)
            If options.Progress IsNot Nothing Then
                options.Progress.Invoke(message)
            End If
        End Sub

    End Class


#End Region
