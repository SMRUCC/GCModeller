#Region "Microsoft.VisualBasic::5e7f9a1b3c5d7e9f1a3b5c7d9e1f3a5b, analysis\SequenceToolkit\gwANI\GwaniMatrixStore.vb"

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
Imports System.IO.MemoryMappedFiles
Imports System.Threading


    ''' <summary>
    ''' The backing store kind of a <see cref="GwaniMatrixStore"/>.
    ''' </summary>
    Public Enum GwaniStoreBackend
        ''' <summary> All of the encoded rows live inside a single managed array. </summary>
        Memory
        ''' <summary> The encoded rows live inside a memory mapped scratch file. </summary>
        MemoryMapped
    End Enum

    ''' <summary>
    ''' A row major store of encoded MSA rows.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The store starts in <see cref="GwaniStoreBackend.Memory"/> mode and keeps the
    ''' rows in a managed list. As soon as the accumulated bytes exceed the configured
    ''' memory budget it spills everything into a scratch file and switches to
    ''' <see cref="GwaniStoreBackend.MemoryMapped"/>, so that alignments which are far
    ''' larger than the physical memory can still be processed: the operating system
    ''' page cache decides which pages stay resident.
    ''' </para>
    ''' <para>
    ''' Rows are appended once and never mutated afterwards, therefore the memory
    ''' backend hands out its internal arrays without copying.
    ''' </para>
    ''' </remarks>
    Public NotInheritable Class GwaniMatrixStore
        Implements IDisposable

        ''' <summary> Write buffer size of the scratch file stream, in bytes. </summary>
        Private Const WriteBufferSize As Integer = 1 << 20

        Private _rows As List(Of Byte())
        Private _writer As FileStream
        Private _mapping As MemoryMappedFile
        Private _readers As ThreadLocal(Of MemoryMappedViewStream)
        Private _scratchPath As String
        Private _scratchDirectory As String
        Private _budget As Long
        Private _accumulated As Long
        Private _paddedLength As Integer
        Private _count As Integer
        Private _sealed As Boolean
        Private _disposed As Boolean

        ''' <summary> Number of the stored rows. </summary>
        Public ReadOnly Property SequenceCount As Integer
            Get
                Return _count
            End Get
        End Property

        ''' <summary> The padded row length in bytes. </summary>
        Public ReadOnly Property PaddedLength As Integer
            Get
                Return _paddedLength
            End Get
        End Property

        ''' <summary> The active backing store kind. </summary>
        Public ReadOnly Property Backend As GwaniStoreBackend
            Get
                Return If(_rows Is Nothing, GwaniStoreBackend.MemoryMapped, GwaniStoreBackend.Memory)
            End Get
        End Property

        ''' <summary> Total number of the stored payload bytes. </summary>
        Public ReadOnly Property TotalBytes As Long
            Get
                Return CLng(_count) * _paddedLength
            End Get
        End Property

        Private Sub New(options As GwaniOptions)
            _rows = New List(Of Byte())()
            _budget = options.MemoryBudgetBytes
            _scratchDirectory = options.ResolveScratchDirectory()
        End Sub

        ''' <summary>
        ''' Creates an empty store.
        ''' </summary>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Shared Function Create(options As GwaniOptions) As GwaniMatrixStore
            Return New GwaniMatrixStore(If(options, New GwaniOptions()))
        End Function

        ''' <summary>
        ''' Appends one encoded row. All of the rows must share the same padded length.
        ''' </summary>
        ''' <param name="row"></param>
        ''' <returns>The index of the appended row.</returns>
        Public Function Append(row As Byte()) As Integer
            If _sealed Then
                Throw New InvalidOperationException("The matrix store has already been sealed.")
            End If
            If row Is Nothing Then
                Throw New ArgumentNullException(NameOf(row))
            End If

            If _count = 0 Then
                _paddedLength = row.Length
            ElseIf row.Length <> _paddedLength Then
                Throw New InvalidDataException(
                    $"Alignment contains sequences of unequal length. Expected padded length is {_paddedLength} but got {row.Length}.")
            End If

            If _rows Is Nothing Then
                _writer.Seek(CLng(_count) * _paddedLength, SeekOrigin.Begin)
                _writer.Write(row, 0, _paddedLength)
            Else
                _rows.Add(row)
                _accumulated += _paddedLength

                If _budget > 0 AndAlso _accumulated > _budget Then
                    Spill()
                End If
            End If

            _count += 1

            Return _count - 1
        End Function

        ''' <summary>
        ''' Finalizes the store. No further row can be appended after this call.
        ''' </summary>
        Public Sub Seal()
            If _sealed Then
                Return
            End If

            If _rows Is Nothing Then
                _writer.Flush()
                _writer.Dispose()
                _writer = Nothing

                _mapping = MemoryMappedFile.CreateFromFile(
                    _scratchPath, FileMode.Open, Nothing, 0, MemoryMappedFileAccess.ReadWrite)
                _readers = New ThreadLocal(Of MemoryMappedViewStream)(
                    Function() _mapping.CreateViewStream(0, 0), trackAllValues:=True)
            End If

            _sealed = True
        End Sub

        ''' <summary>
        ''' Gets the internal row array of the memory backend without copying.
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        Public Function GetRow(index As Integer) As Byte()
            If _rows Is Nothing Then
                Throw New InvalidOperationException("GetRow is only available on the in-memory backend, use ReadRowInto instead.")
            End If

            Return _rows(index)
        End Function

        ''' <summary>
        ''' Copies the row at <paramref name="index"/> into <paramref name="buffer"/>.
        ''' The row is also readable while the store is still being built, which is
        ''' required by the duplicate detector.
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="buffer">Must be at least <see cref="PaddedLength"/> bytes long.</param>
        Public Sub ReadRowInto(index As Integer, buffer As Byte())
            If _disposed Then
                Throw New ObjectDisposedException(NameOf(GwaniMatrixStore))
            End If

            If _rows Is Nothing Then
                If _sealed Then
                    Dim view = _readers.Value

                    view.Seek(CLng(index) * _paddedLength, SeekOrigin.Begin)
                    ReadExact(view, buffer, _paddedLength)
                Else
                    ' the store is still being built, read the row back through
                    ' the write stream of the scratch file
                    _writer.Flush()
                    _writer.Seek(CLng(index) * _paddedLength, SeekOrigin.Begin)
                    ReadExact(_writer, buffer, _paddedLength)
                End If
            Else
                Array.Copy(_rows(index), buffer, _paddedLength)
            End If
        End Sub

        ''' <summary>
        ''' Reads a contiguous block of rows. On the memory backend the returned arrays
        ''' are the internal arrays, on the mapped backend they are freshly allocated.
        ''' </summary>
        ''' <param name="startIndex"></param>
        ''' <param name="count"></param>
        ''' <returns></returns>
        Public Function ReadRows(startIndex As Integer, count As Integer) As Byte()()
            Dim rows As Byte()() = New Byte(count - 1)() {}

            For i As Integer = 0 To count - 1
                If _rows Is Nothing Then
                    Dim buffer As Byte() = New Byte(_paddedLength - 1) {}

                    ReadRowInto(startIndex + i, buffer)
                    rows(i) = buffer
                Else
                    rows(i) = _rows(startIndex + i)
                End If
            Next

            Return rows
        End Function

        ''' <summary>
        ''' Reads exactly <paramref name="count"/> bytes, a mapped view stream is
        ''' allowed to return the data in several chunks.
        ''' </summary>
        Private Shared Sub ReadExact(stream As Stream, buffer As Byte(), count As Integer)
            Dim offset As Integer = 0

            While offset < count
                Dim read As Integer = stream.Read(buffer, offset, count - offset)

                If read <= 0 Then
                    Throw New EndOfStreamException("The gwANI scratch file ended unexpectedly.")
                End If

                offset += read
            End While
        End Sub

        ''' <summary>
        ''' Moves all of the buffered rows into a memory mapped scratch file.
        ''' </summary>
        Private Sub Spill()
            Dim scratchPath As String = Path.Combine(_scratchDirectory, $"gwANI-{Guid.NewGuid():N}.dat")

            _scratchPath = scratchPath
            _writer = New FileStream(scratchPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, WriteBufferSize)

            For Each row As Byte() In _rows
                _writer.Write(row, 0, _paddedLength)
            Next

            _rows = Nothing
        End Sub

        Private Sub DisposeInternal()
            If _disposed Then
                Return
            End If

            If _readers IsNot Nothing Then
                For Each view As MemoryMappedViewStream In _readers.Values
                    view.Dispose()
                Next

                _readers.Dispose()
                _readers = Nothing
            End If

            If _mapping IsNot Nothing Then
                _mapping.Dispose()
                _mapping = Nothing
            End If

            If _writer IsNot Nothing Then
                _writer.Dispose()
                _writer = Nothing
            End If

            If _scratchPath IsNot Nothing AndAlso File.Exists(_scratchPath) Then
                Try
                    File.Delete(_scratchPath)
                Catch ex As IOException
                    ' a scratch file that cannot be removed right now is not fatal
                End Try

                _scratchPath = Nothing
            End If

            _disposed = True
        End Sub

        ''' <summary>
        ''' Releases the store and removes the scratch file if one was created.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            DisposeInternal()
            GC.SuppressFinalize(Me)
        End Sub

    End Class


#End Region
