#Region "Microsoft.VisualBasic::27537cf82cf656a2ab497e588f8641d0, analysis\Metagenome\Metagenome\Kmers\ShardingReader.vb"

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

    '   Total Lines: 162
    '    Code Lines: 111 (68.52%)
    ' Comment Lines: 21 (12.96%)
    '    - Xml Docs: 47.62%
    ' 
    '   Blank Lines: 30 (18.52%)
    '     File Size: 5.98 KB


    '     Class ShardingReader
    ' 
    '         Properties: k
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetKmer, LoadIndex, LoadIndexInternal, LoadIntoMemory, ParseKmerBuffer
    '                   ReadKmer
    ' 
    '         Sub: (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.IO

Namespace Kmers

    Public Class ShardingReader : Implements IDisposable

        ''' <summary>
        ''' length of the k-mer
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property k As Integer

        ReadOnly index As New Dictionary(Of String, (offset&, size%))
        ReadOnly kmers As BinaryDataReader
        ReadOnly cache As New Dictionary(Of String, KmerSeed)

        Dim disposedValue As Boolean

        Sub New(database As String)
            Using index As New BinaryDataReader($"{database}/index.dat".Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=True))
                If LoadIndex(index, k, cache:=Me.index) Then
                    kmers = New BinaryDataReader($"{database}/kmers.dat".Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=True))
                    kmers.ByteOrder = ByteOrder.BigEndian
                Else
                    Throw New InvalidDataException("invalid index data file!")
                End If
            End Using
        End Sub

        Public Function LoadIntoMemory() As ShardingReader
            For Each kmer As String In index.Keys
                Call GetKmer(kmer)
            Next

            Return Me
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="kmer"></param>
        ''' <returns>
        ''' return nothing means not exists
        ''' </returns>
        Public Function GetKmer(kmer As String) As KmerSeed
            If cache.ContainsKey(kmer) Then
                Return cache(kmer)
            End If

            If index.ContainsKey(kmer) Then
                With index(kmer)
                    Dim kseq As KmerSeed = ReadKmer(.offset, .size, k, kmers)
                    cache.Add(kmer, kseq)
                    Return kseq
                End With
            Else
                Return Nothing
            End If
        End Function

        Public Shared Function ReadKmer(offset&, size%, k As Integer, db As BinaryDataReader) As KmerSeed
            Dim buf As Byte()
            Dim check As Integer

            db.Position = offset
            check = db.ReadInt32()
            buf = db.ReadBytes(size)
            db = New BinaryDataReader(buf) With {
                .ByteOrder = ByteOrder.BigEndian
            }

            If check <> size Then
                Throw New InvalidDataException("buffer size verification failured, database file is damaged!")
            Else
                Return ParseKmerBuffer(db, k)
            End If
        End Function

        Public Shared Function ParseKmerBuffer(db As BinaryDataReader, k As Integer) As KmerSeed
            Dim kmer As String = Encoding.ASCII.GetString(db.ReadBytes(k))
            Dim n As Integer = db.ReadInt32
            Dim sources As KmerSource() = New KmerSource(n - 1) {}

            For i As Integer = 0 To n - 1
                Dim nsize As Integer = db.ReadInt32
                Dim uints As UInteger() = db.ReadUInt32s(nsize)
                Dim source As New KmerSource With {
                    .count = uints.Length - 1,
                    .seqid = uints(0),
                    .locations = uints.Skip(1).ToArray
                }

                sources(i) = source
            Next

            Return New KmerSeed With {
                .kmer = kmer,
                .source = sources
            }
        End Function

        Friend Shared Function LoadIndex(index As BinaryDataReader, ByRef k As Integer, cache As Dictionary(Of String, (Long, Integer))) As Boolean
            index.ByteOrder = ByteOrder.BigEndian

            If index.Length > KmerWriter.Magic.Length + 4 Then
                Dim check = Encoding.ASCII.GetString(index.ReadBytes(KmerWriter.Magic.Length)) = KmerWriter.Magic

                If check Then
                    Return LoadIndexInternal(index, k, cache)
                End If
            End If

            Return False
        End Function

        Private Shared Function LoadIndexInternal(index As BinaryDataReader, ByRef k As Integer, cache As Dictionary(Of String, (Long, Integer))) As Boolean
            Dim n As Integer = index.ReadInt32

            k = index.ReadInt32

            For i As Integer = 0 To n - 1
                Dim kmer As String = Encoding.ASCII.GetString(index.ReadBytes(k))
                Dim offset As Long = index.ReadInt64
                Dim size As Integer = index.ReadInt32

                Call cache.Add(kmer, (offset, size))
            Next

            Return True
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call index.Clear()
                    Call cache.Clear()
                    Call kmers.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
