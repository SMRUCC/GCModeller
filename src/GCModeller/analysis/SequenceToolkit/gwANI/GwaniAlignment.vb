#Region "Microsoft.VisualBasic::7a9b1c3d5e7f9a1b3c5d7e9f1a3b5c7d, analysis\SequenceToolkit\gwANI\GwaniAlignment.vb"

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

Imports System.Collections.Generic
Imports System.IO
Imports SMRUCC.genomics.SequenceModel.FASTA


    ''' <summary>
    ''' An aligned sequence collection that is ready for the gwANI kernel.
    ''' </summary>
    ''' <remarks>
    ''' The alignment owns a <see cref="GwaniMatrixStore"/> of encoded rows. When the
    ''' deduplication is enabled the store only keeps one row per distinct sequence,
    ''' and <see cref="RepresentativeOf"/> maps every input sequence onto its
    ''' representative row.
    ''' </remarks>
    Public NotInheritable Class GwaniAlignment
        Implements IDisposable

        ''' <summary> The sequence id of every input sequence, in the input order. </summary>
        Public ReadOnly Property Names As String()

        ''' <summary> The encoded row store. </summary>
        Public ReadOnly Property Store As GwaniMatrixStore

        ''' <summary>
        ''' Maps an input sequence index onto the store row of its representative.
        ''' </summary>
        Public ReadOnly Property RepresentativeOf As Integer()

        ''' <summary>
        ''' Maps a store row onto the input index of the first sequence that produced it.
        ''' </summary>
        Public ReadOnly Property FirstOccurrence As Integer()

        ''' <summary> The number of distinct sequences, i.e. the number of stored rows. </summary>
        Public ReadOnly Property UniqueCount As Integer
            Get
                Return Store.SequenceCount
            End Get
        End Property

        ''' <summary> The number of input sequences. </summary>
        Public ReadOnly Property SequenceCount As Integer
            Get
                Return Names.Length
            End Get
        End Property

        ''' <summary> The unpadded alignment length. </summary>
        Public ReadOnly Property AlignmentLength As Integer

        ''' <summary> The padded row length of the store. </summary>
        Public ReadOnly Property PaddedLength As Integer
            Get
                Return Store.PaddedLength
            End Get
        End Property

        ''' <summary> True when at least one sequence was collapsed by the deduplication. </summary>
        Public ReadOnly Property HasDuplicates As Boolean
            Get
                Return UniqueCount < SequenceCount
            End Get
        End Property

        ''' <summary>
        ''' The sequence ids in the row order of the computed matrix, i.e. the id of the
        ''' first occurrence of every distinct sequence.
        ''' </summary>
        ''' <returns></returns>
        Public Function RowIds() As String()
            Dim ids As String() = New String(UniqueCount - 1) {}

            For r As Integer = 0 To UniqueCount - 1
                ids(r) = Names(FirstOccurrence(r))
            Next

            Return ids
        End Function

        Private Sub New(names As String(), representativeOf As Integer(), firstOccurrence As Integer(), store As GwaniMatrixStore, alignmentLength As Integer)
            Me.Names = names
            Me.RepresentativeOf = representativeOf
            Me.FirstOccurrence = firstOccurrence
            Me.Store = store
            Me.AlignmentLength = alignmentLength
        End Sub

        ''' <summary>
        ''' Builds an alignment from a stream of FASTA records. The records are consumed
        ''' lazily, so a file that is far larger than the memory can be loaded.
        ''' </summary>
        ''' <param name="sequences"></param>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Shared Function Build(sequences As IEnumerable(Of FastaSeq), options As GwaniOptions) As GwaniAlignment
            If options Is Nothing Then
                options = New GwaniOptions()
            End If

            Dim store As GwaniMatrixStore = GwaniMatrixStore.Create(options)
            Dim names As New List(Of String)()
            Dim representatives As New List(Of Integer)()
            Dim firstOccurrence As New List(Of Integer)()
            Dim dedup As GwaniDeduplicator = If(options.EnableDeduplication, New GwaniDeduplicator(store), Nothing)
            Dim alignmentLength As Integer = -1
            Dim index As Integer = 0

            For Each seq As FastaSeq In sequences
                Dim data As String = If(seq.SequenceData, "")

                If alignmentLength < 0 Then
                    alignmentLength = data.Length
                ElseIf data.Length <> alignmentLength Then
                    Throw New InvalidDataException(
                        $"Alignment contains sequences of unequal length. Expected length is {alignmentLength} but got {data.Length} in sequence '{seq.Title}'.")
                End If

                Dim row As Byte() = NucleotideCodec.EncodeRow(data)

                names.Add(seq.Title)

                If dedup Is Nothing Then
                    representatives.Add(store.Append(row))
                    firstOccurrence.Add(index)
                Else
                    Dim existing As Integer = dedup.Lookup(row)

                    If existing >= 0 Then
                        representatives.Add(existing)
                    Else
                        existing = store.Append(row)
                        representatives.Add(existing)
                        firstOccurrence.Add(index)
                        dedup.Register(row, existing)
                    End If
                End If

                index += 1
            Next

            store.Seal()

            Return New GwaniAlignment(names.ToArray(), representatives.ToArray(), firstOccurrence.ToArray(), store, Math.Max(0, alignmentLength))
        End Function

        ''' <summary>
        ''' Builds an alignment from a multi-FASTA file with a constant memory reader.
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Shared Function BuildFile(path As String, options As GwaniOptions) As GwaniAlignment
            Using reader As New StreamIterator(path, tqdm_wrap:=False)
                Return Build(reader.ReadStream(), options)
            End Using
        End Function

        ''' <summary>
        ''' Builds an alignment from rows that have already been encoded with
        ''' <see cref="NucleotideCodec.EncodeRow"/>. This entry point is used by the
        ''' in-memory benchmark, which generates the encoded rows directly instead of
        ''' materializing millions of sequence strings.
        ''' </summary>
        ''' <param name="rows"></param>
        ''' <param name="names"></param>
        ''' <param name="alignmentLength">The unpadded alignment length.</param>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Shared Function Build(rows As IEnumerable(Of Byte()), names As IEnumerable(Of String), alignmentLength As Integer, options As GwaniOptions) As GwaniAlignment
            If options Is Nothing Then
                options = New GwaniOptions()
            End If

            Dim store As GwaniMatrixStore = GwaniMatrixStore.Create(options)
            Dim nameList As New List(Of String)(names)
            Dim representatives As New List(Of Integer)()
            Dim firstOccurrence As New List(Of Integer)()
            Dim dedup As GwaniDeduplicator = If(options.EnableDeduplication, New GwaniDeduplicator(store), Nothing)
            Dim expected As Integer = If(alignmentLength > 0, NucleotideCodec.PaddedLength(alignmentLength), -1)
            Dim length As Integer = alignmentLength
            Dim index As Integer = 0
            Dim rowEnum As IEnumerator(Of Byte()) = rows.GetEnumerator()

            While rowEnum.MoveNext()
                Dim row As Byte() = rowEnum.Current

                If row Is Nothing Then
                    Throw New InvalidDataException($"The encoded row at index {index} is null.")
                End If
                If expected >= 0 AndAlso row.Length <> expected Then
                    Throw New InvalidDataException(
                        $"The encoded row at index {index} has length {row.Length}, but the alignment length {alignmentLength} requires {expected}.")
                End If
                If store.SequenceCount = 0 AndAlso expected < 0 Then
                    length = row.Length
                ElseIf store.SequenceCount > 0 AndAlso row.Length <> store.PaddedLength Then
                    Throw New InvalidDataException(
                        $"The encoded row at index {index} has length {row.Length}, but the previous rows have length {store.PaddedLength}.")
                End If

                If dedup Is Nothing Then
                    representatives.Add(store.Append(row))
                    firstOccurrence.Add(index)
                Else
                    Dim existing As Integer = dedup.Lookup(row)

                    If existing >= 0 Then
                        representatives.Add(existing)
                    Else
                        existing = store.Append(row)
                        representatives.Add(existing)
                        firstOccurrence.Add(index)
                        dedup.Register(row, existing)
                    End If
                End If

                index += 1
            End While

            rowEnum.Dispose()

            If nameList.Count <> index Then
                Throw New InvalidDataException($"The alignment has {index} rows but {nameList.Count} sequence names.")
            End If

            store.Seal()

            Return New GwaniAlignment(nameList.ToArray(), representatives.ToArray(), firstOccurrence.ToArray(), store, length)
        End Function

        ''' <summary>
        ''' Expands the representative rows into the full input ordered matrix. The
        ''' expansion needs the whole matrix in memory, so it is only meant for the
        ''' small alignments of the compatibility API.
        ''' </summary>
        ''' <param name="rows"></param>
        ''' <returns></returns>
        Public Function Expand(rows As IEnumerable(Of GwaniRow)) As Double()()
            Dim u As Integer = UniqueCount
            Dim matrix As Double()() = New Double(u - 1)() {}
            Dim lookup As New Dictionary(Of Integer, Double())(u)

            For Each row As GwaniRow In rows
                lookup(row.RowIndex) = row.Identities
            Next

            For r As Integer = 0 To u - 1
                Dim values As Double() = New Double(u - 1) {}
                Dim source As Double() = lookup(r)

                Array.Copy(source, values, u)
                matrix(r) = values
            Next

            Dim result As Double()() = New Double(SequenceCount - 1)() {}

            For i As Integer = 0 To SequenceCount - 1
                Dim a As Integer = RepresentativeOf(i)
                Dim row As Double() = New Double(SequenceCount - 1) {}

                For j As Integer = 0 To SequenceCount - 1
                    If j < i Then
                        row(j) = GwaniValues.LowerTriangle
                    ElseIf j = i Then
                        row(j) = GwaniValues.SelfIdentity
                    Else
                        Dim b As Integer = RepresentativeOf(j)

                        If a < b Then
                            row(j) = matrix(a)(b)
                        Else
                            row(j) = matrix(b)(a)
                        End If
                    End If
                Next

                result(i) = row
            Next

            Return result
        End Function

        Private Sub DisposeInternal()
            If Store IsNot Nothing Then
                Store.Dispose()
            End If
        End Sub

        ''' <summary>
        ''' Releases the alignment and its scratch file.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            DisposeInternal()
            GC.SuppressFinalize(Me)
        End Sub

    End Class

    ''' <summary>
    ''' The exact duplicate detector that is used while the alignment is being built.
    ''' </summary>
    ''' <remarks>
    ''' A 32 bit hash bucket plus a full row comparison against the store keeps the
    ''' false positive rate at zero, which matters because silently merging two
    ''' distinct sequences would corrupt the result.
    ''' </remarks>
    Friend NotInheritable Class GwaniDeduplicator

        Private ReadOnly _buckets As New Dictionary(Of Integer, List(Of Integer))()
        Private ReadOnly _store As GwaniMatrixStore
        Private _compareBuffer As Byte()

        Friend Sub New(store As GwaniMatrixStore)
            _store = store
        End Sub

        ''' <summary>
        ''' Returns the store index of an identical row, or -1 when the row is new.
        ''' </summary>
        ''' <param name="row"></param>
        ''' <returns></returns>
        Friend Function Lookup(row As Byte()) As Integer
            Dim hash As Integer = RowHash(row)
            Dim bucket As List(Of Integer) = Nothing

            If Not _buckets.TryGetValue(hash, bucket) Then
                Return -1
            End If

            Dim padded As Integer = _store.PaddedLength

            For Each index As Integer In bucket
                If _store.Backend = GwaniStoreBackend.MemoryMapped Then
                    If _compareBuffer Is Nothing OrElse _compareBuffer.Length <> padded Then
                        _compareBuffer = New Byte(padded - 1) {}
                    End If

                    _store.ReadRowInto(index, _compareBuffer)

                    If RowsEqual(row, _compareBuffer) Then
                        Return index
                    End If
                Else
                    If RowsEqual(row, _store.GetRow(index)) Then
                        Return index
                    End If
                End If
            Next

            Return -1
        End Function

        ''' <summary>
        ''' Registers a new distinct row.
        ''' </summary>
        ''' <param name="row"></param>
        ''' <param name="storeIndex"></param>
        Friend Sub Register(row As Byte(), storeIndex As Integer)
            Dim hash As Integer = RowHash(row)
            Dim bucket As List(Of Integer) = Nothing

            If _buckets.TryGetValue(hash, bucket) Then
                bucket.Add(storeIndex)
            Else
                _buckets(hash) = New List(Of Integer)({storeIndex})
            End If
        End Sub

        Private Shared Function RowsEqual(a As Byte(), b As Byte()) As Boolean
            Dim n As Integer = a.Length

            If n <> b.Length Then
                Return False
            End If

            Dim p As Integer = 0

            While p + 8 <= n
                If BitConverter.ToUInt64(a, p) <> BitConverter.ToUInt64(b, p) Then
                    Return False
                End If

                p += 8
            End While

            While p < n
                If a(p) <> b(p) Then
                    Return False
                End If

                p += 1
            End While

            Return True
        End Function

        Private Shared Function RowHash(row As Byte()) As Integer
            Dim hc As New HashCode()
            Dim n As Integer = row.Length
            Dim p As Integer = 0

            While p + 8 <= n
                hc.Add(BitConverter.ToUInt64(row, p))
                p += 8
            End While

            While p < n
                hc.Add(CULng(row(p)))
                p += 1
            End While

            ' the hash is kept as a signed 32 bit value on purpose: mixing a signed
            ' and an unsigned operand in a bitwise And makes the Visual Basic runtime
            ' convert the signed operand to unsigned, which overflows on a negative
            ' hash
            Return hc.ToHashCode()
        End Function

    End Class


#End Region
