#Region "Microsoft.VisualBasic::4677828b158bc663ad0e3337536ba436, GCModeller\analysis\HTS_matrix\File\BinaryMatrix.vb"

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

'   Total Lines: 101
'    Code Lines: 72
' Comment Lines: 12
'   Blank Lines: 17
'     File Size: 3.17 KB


' Module BinaryMatrix
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: LoadStream, (+2 Overloads) readMatrix, Save
' 
'     Sub: save
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Serialization.BinaryDumping
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class HTSMatrixReader : Implements IDisposable

    ReadOnly bin As New NetworkByteOrderBuffer
    ReadOnly file As BinaryReader
    ReadOnly sampleID As String()
    ReadOnly geneIDs As Index(Of String)
    ReadOnly scan0 As Long
    ReadOnly blockSize As Integer

    Public ReadOnly Property TagString As String

    Public ReadOnly Property SampleIDs As IEnumerable(Of String)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return sampleID
        End Get
    End Property

    Public ReadOnly Property FeatureIDs As IEnumerable(Of String)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return geneIDs.Objects
        End Get
    End Property

    Public ReadOnly Property Size As (nsample As Integer, nfeature As Integer)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return (sampleID.Length, geneIDs.Count)
        End Get
    End Property

    Private disposedValue As Boolean

    Sub New(file As Stream)
        Me.file = New BinaryReader(file)

        Dim bytes As Byte() = Me.file.ReadBytes(magic.Length)

        If Not bytes.SequenceEqual(magic) Then
            Throw New InvalidDataException("invalid magic header string!")
        Else
            ' read tag string
            TagString = Me.file.ReadString
            ' read nsamples
            Dim nsamples = Me.file.ReadInt32
            Dim mfeatures = Me.file.ReadInt32
            Dim str As String

            str = Me.file.ReadString
            sampleID = str.LoadJSON(Of String())
            str = Me.file.ReadString
            geneIDs = New Index(Of String)(str.LoadJSON(Of String()))
            scan0 = Me.file.BaseStream.Position
            blockSize = sampleID.Length * Marshal.SizeOf(GetType(Double))
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetSampleOrdinal(sampleID As String) As Integer
        Return Me.sampleID.IndexOf(sampleID)
    End Function

    Public Function GetGeneExpression(geneID As String) As Double()
        If geneID Like geneIDs Then
            Dim i As Integer = geneIDs.IndexOf(geneID)
            Dim offset As Long = scan0 + blockSize * i
            Dim buffer As Byte() = New Byte(blockSize - 1) {}

            Call file.BaseStream.Seek(offset, SeekOrigin.Begin)
            Call file.BaseStream.Read(buffer, scan0, buffer.Length)

            Return bin.decode(buffer)
        Else
            Return New Double(sampleID.Length - 1) {}
        End If
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call file.Dispose()
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
            ' TODO: 将大型字段设置为 null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
    ' Protected Overrides Sub Finalize()
    '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class

''' <summary>
''' data encoded in network byte order
''' </summary>
Public Module BinaryMatrix

    Friend ReadOnly magic As Byte()
    ReadOnly bin As New NetworkByteOrderBuffer

    Sub New()
        magic = Encoding.ASCII.GetBytes("GCModeller/HTS_matrix")
    End Sub

    Public Function LoadStream(file As Stream) As Matrix
        Using reader As New BinaryReader(file, Encoding.UTF8)
            Return reader.readMatrix
        End Using
    End Function

    <Extension>
    Private Function readMatrix(file As BinaryReader) As Matrix
        ' read magic
        Dim bytes As Byte() = file.ReadBytes(magic.Length)
        Dim str As String
        Dim mat As New Matrix

        If Not bytes.SequenceEqual(magic) Then
            Throw New InvalidDataException("invalid magic header string!")
        Else
            ' read tag string
            str = file.ReadString
            mat.tag = str
        End If

        ' read nsamples
        Dim nsamples = file.ReadInt32
        Dim mfeatures = file.ReadInt32
        Dim geneIds As Pointer(Of String)

        str = file.ReadString
        mat.sampleID = str.LoadJSON(Of String())
        str = file.ReadString
        geneIds = New Pointer(Of String)(str.LoadJSON(Of String()))
        mat.expression = readMatrix(geneIds, file, nsamples).ToArray

        Return mat
    End Function

    Private Iterator Function readMatrix(geneIds As Pointer(Of String), file As BinaryReader, nsamples As Integer) As IEnumerable(Of DataFrameRow)
        Dim buffer As Stream = file.BaseStream
        Dim bytes As Byte() = New Byte(nsamples * 8 - 1) {}
        Dim exp As Double()

        Do While buffer.Length > buffer.Position
            bytes = file.ReadBytes(bytes.Length)
            exp = bin.decode(bytes)

            Yield New DataFrameRow With {
                .experiments = exp,
                .geneID = ++geneIds
            }
        Loop
    End Function

    <Extension>
    Public Function Save(mat As Matrix, file As Stream) As Boolean
        Using writer As New BinaryWriter(file, Encoding.UTF8)
            Call writer.Write(magic)
            Call mat.save(writer)
            Call writer.Flush()
        End Using

        Return True
    End Function

    <Extension>
    Private Sub save(mat As Matrix, file As BinaryWriter)
        ' write matrix tag
        Call file.Write(If(mat.tag, "NA"))
        ' n samples
        Call file.Write(mat.sampleID.Length)
        ' m features
        Call file.Write(mat.size)
        ' write all sample id
        Call file.Write(mat.sampleID.GetJson)
        ' write all feature id
        Call file.Write(mat.rownames.GetJson)

        ' write each feature row data
        ' data block is fixed size:
        '
        '    sizeof(double) * [sampleID length]
        '
        For Each feature As DataFrameRow In mat.expression
            Call file.Write(bin.encode(feature.experiments))
        Next
    End Sub

End Module
