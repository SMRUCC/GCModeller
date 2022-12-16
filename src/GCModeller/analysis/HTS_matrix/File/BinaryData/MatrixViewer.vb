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
    ''' <summary>
    ''' get gene expression across all sample data
    ''' </summary>
    ''' <param name="geneID">a specific gene target</param>
    ''' <returns>
    ''' expression data is aligned with the <see cref="SampleIDs"/>
    ''' </returns>
    Public MustOverride Function GetGeneExpression(geneID As String) As Double()
    ''' <summary>
    ''' get a set of gene expression across a specific sample data
    ''' </summary>
    ''' <param name="geneID">a set of target gene</param>
    ''' <param name="sampleOrdinal">the order index of the specific sample data</param>
    ''' <returns>
    ''' expression data is aligned with the <paramref name="geneID"/> set.
    ''' </returns>
    Public MustOverride Function GetGeneExpression(geneID As String(), sampleOrdinal As Integer) As Double()
    Public MustOverride Sub SetNewGeneIDs(geneIDs As String())
    Public MustOverride Sub SetNewSampleIDs(sampleIDs As String())

    Public Overrides Function ToString() As String
        Dim size = Me.Size
        Return $"{tag}({size.nsample}x{size.nfeature})"
    End Function

End Class