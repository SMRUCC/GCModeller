Imports System.Runtime.CompilerServices

Public MustInherit Class MatrixViewer

    Public ReadOnly Property tag As String
        Get
            Return tagString
        End Get
    End Property

    Protected tagString As String

    Public MustOverride ReadOnly Property SampleIDs As IEnumerable(Of String)

    ''' <summary>
    ''' get gene row features name
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride ReadOnly Property FeatureIDs As IEnumerable(Of String)

    Public Overridable ReadOnly Property Size As (nsample As Integer, nfeature As Integer)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return (SampleIDs.Count, FeatureIDs.Count)
        End Get
    End Property

    Public MustOverride Function GetSampleOrdinal(sampleID As String) As Integer
    Public MustOverride Function GetGeneExpression(geneID As String) As Double()

    Public Overrides Function ToString() As String
        Dim size = Me.Size
        Return $"{tag}({size.nsample}x{size.nfeature})"
    End Function

End Class