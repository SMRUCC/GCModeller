#Region "Microsoft.VisualBasic::fa67038dcf62138dab806023e9ba2ef3, G:/GCModeller/src/repository/Bio.Repository//HDS/PtfReader.vb"

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

    '   Total Lines: 147
    '    Code Lines: 106
    ' Comment Lines: 14
    '   Blank Lines: 27
    '     File Size: 5.39 KB


    ' Class PtfReader
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: GetAnnotation, getExternalReferenceList, LoadCrossReference, ReadBytes
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.My.FrameworkInternal
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Annotation.Ptf

Public Class PtfReader : Implements IDisposable

    ReadOnly stream As StreamPack
    ReadOnly cache As New Dictionary(Of String, ProteinAnnotation)

    Private disposedValue As Boolean

    Sub New(file As Stream)
        Call Me.New(New StreamPack(file, [readonly]:=True))
    End Sub

    Sub New(stream As StreamPack)
        Me.stream = stream
    End Sub

    ''' <summary>
    ''' enumerate all database name from a HDS stream
    ''' </summary>
    ''' <returns></returns>
    Public Function getExternalReferenceList() As String()
        Return DirectCast(stream.GetObject("/db_xref/"), StreamGroup).files _
            .Select(Function(a) a.fileName.BaseName) _
            .ToArray
    End Function

    Public Function GetAnnotation(id As String) As ProteinAnnotation
        If App.MemoryLoad <> MemoryLoads.Light Then
            If cache.ContainsKey(id) Then
                Return cache(id)
            End If
        End If

        Dim path As String = $"/annotation/{id}.ptf"
        Dim file As StreamObject = stream.GetObject(path)

        If file Is Nothing Then
            Return Nothing
        Else
            Dim info As ProteinAnnotation = ReadBytes(New BinaryDataReader(stream.OpenBlock(file)) With {
                .ByteOrder = ByteOrder.BigEndian,
                .Encoding = Encodings.ASCII.CodePage
            })

            If App.MemoryLoad <> MemoryLoads.Light Then
                Call cache.Add(id, info)
            End If

            Return info
        End If
    End Function

    Public Shared Function ReadBytes(data As BinaryDataReader) As ProteinAnnotation
        Dim geneId As String = data.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim locus_id As String = data.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim geneName As String = data.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim desc As String = data.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim seq As String = data.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim nattrs As Integer = data.ReadInt32
        Dim attrs As New Dictionary(Of String, String())

        For i As Integer = 0 To nattrs - 1
            Dim key As String = data.ReadString(BinaryStringFormat.ZeroTerminated)
            Dim size As Integer = data.ReadInt32
            Dim vals As String() = New String(size - 1) {}

            For j As Integer = 0 To size - 1
                vals(j) = data.ReadString(BinaryStringFormat.ZeroTerminated)
            Next

            Call attrs.Add(key, vals)
        Next

        Return New ProteinAnnotation With {
            .attributes = attrs,
            .description = desc,
            .geneId = geneId,
            .geneName = geneName,
            .locus_id = locus_id,
            .sequence = seq
        }
    End Function

    Public Function LoadCrossReference(key As String) As Dictionary(Of String, String())
        Dim intptr As String = $"/db_xref/{key}.txt"

        If Not stream.FileExists(intptr) Then
            Return New Dictionary(Of String, String())
        End If

        Dim rawtext As String = stream.ReadText(filename:=intptr)
        Dim data = BencodeDecoder.Decode(rawtext)

        If data.IsNullOrEmpty Then
            Return New Dictionary(Of String, String())
        Else
            Dim xrefs As New Dictionary(Of String, String())
            Dim list = DirectCast(data(Scan0), BDictionary)

            For Each id As BElement In list.Keys
                Dim cluster = list(DirectCast(id, BString).Value)
                Dim geneSymbols As String() = DirectCast(cluster, BList) _
                    .Select(Function(b) DirectCast(b, BString).Value) _
                    .Distinct _
                    .ToArray

                xrefs.Add(DirectCast(id, BString).Value, geneSymbols)
            Next

            Return xrefs
        End If
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call stream.Dispose()
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

