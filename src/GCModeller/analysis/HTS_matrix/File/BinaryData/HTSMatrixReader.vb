Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
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
