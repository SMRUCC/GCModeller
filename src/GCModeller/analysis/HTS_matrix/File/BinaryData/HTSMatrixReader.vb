Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.BinaryDumping
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class HTSMatrixReader : Inherits MatrixViewer
    Implements IDisposable

    ReadOnly bin As New NetworkByteOrderBuffer
    ReadOnly file As BinaryReader
    ReadOnly sampleID As Index(Of String)
    ''' <summary>
    ''' the data reader offset is evaluated via this index object
    ''' </summary>
    ReadOnly geneIDs As Index(Of String)
    ReadOnly scan0 As Long
    ReadOnly blockSize As Integer

    ''' <summary>
    ''' sample data id in columns
    ''' </summary>
    ''' <returns></returns>
    Public Overrides ReadOnly Property SampleIDs As IEnumerable(Of String)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return sampleID.Objects
        End Get
    End Property

    ''' <summary>
    ''' gene feature ids in rows
    ''' </summary>
    ''' <returns></returns>
    Public Overrides ReadOnly Property FeatureIDs As IEnumerable(Of String)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return geneIDs.Objects
        End Get
    End Property

    Public Overrides ReadOnly Property Size As (nsample As Integer, nfeature As Integer)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return (sampleID.Count, geneIDs.Count)
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
            tagString = Me.file.ReadString
            ' read nsamples
            Dim nsamples = Me.file.ReadInt32
            Dim mfeatures = Me.file.ReadInt32
            Dim str As String

            str = Me.file.ReadString
            sampleID = str.LoadJSON(Of String()).Indexing
            str = Me.file.ReadString
            geneIDs = New Index(Of String)(str.LoadJSON(Of String()))
            scan0 = Me.file.BaseStream.Position
            blockSize = sampleID.Count * Marshal.SizeOf(GetType(Double))
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetSampleOrdinal(sampleID As String) As Integer
        Return Me.sampleID.IndexOf(sampleID)
    End Function

    Public Overrides Function GetGeneExpression(geneID() As String, sampleOrdinal As Integer) As Double()
        Dim v As Double() = New Double(geneID.Length - 1) {}

        ' no target sample
        If sampleOrdinal < 0 Then
            Return v
        End If

        For i As Integer = 0 To v.Length - 1
            If geneID(i) Like geneIDs Then
                Dim blocks As Integer = geneIDs.IndexOf(geneID(i))
                Dim offset As Long = scan0 + blockSize * blocks
                Dim buffer As Byte() = New Byte(RawStream.DblFloat - 1) {}

                Call file.BaseStream.Seek(offset + sampleOrdinal * RawStream.DblFloat, SeekOrigin.Begin)
                Call file.BaseStream.Read(buffer, scan0, buffer.Length)

                v(i) = bin.decode(buffer)(0)
            Else
                ' v(i) = 0.0
            End If
        Next

        Return v
    End Function

    Public Overrides Sub SetNewSampleIDs(sampleIDs() As String)
        Call Me.sampleID.Clear()
        Call Me.sampleID.Add(sampleIDs).ToArray
    End Sub

    ''' <summary>
    ''' just updates of the gene id index
    ''' </summary>
    ''' <param name="geneIDs"></param>
    Public Overrides Sub SetNewGeneIDs(geneIDs() As String)
        Call Me.geneIDs.Clear()
        Call Me.geneIDs.Add(geneIDs).ToArray
    End Sub

    Public Overrides Function GetGeneExpression(geneID As String) As Double()
        If geneID Like geneIDs Then
            Dim i As Integer = geneIDs.IndexOf(geneID)
            Dim offset As Long = scan0 + blockSize * i
            Dim buffer As Byte() = New Byte(blockSize - 1) {}

            Call file.BaseStream.Seek(offset, SeekOrigin.Begin)
            Call file.BaseStream.Read(buffer, scan0, buffer.Length)

            Return bin.decode(buffer)
        Else
            Return New Double(sampleID.Count - 1) {}
        End If
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: �ͷ��й�״̬(�йܶ���)
                Call file.Dispose()
            End If

            ' TODO: �ͷ�δ�йܵ���Դ(δ�йܵĶ���)����д�ս���
            ' TODO: �������ֶ�����Ϊ null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: ������Dispose(disposing As Boolean)��ӵ�������ͷ�δ�й���Դ�Ĵ���ʱ������ս���
    ' Protected Overrides Sub Finalize()
    '     ' ��Ҫ���Ĵ˴��롣�뽫����������롰Dispose(disposing As Boolean)��������
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' ��Ҫ���Ĵ˴��롣�뽫����������롰Dispose(disposing As Boolean)��������
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class