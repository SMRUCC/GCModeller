' ============================================================================
' FELLA Algorithm - VB.NET Implementation
' FellaUser.vb - User analysis object for FELLA enrichment
' 
' Equivalent to the FELLA.USER S4 class in the R package.
' Stores the user's input compounds, background definition,
' and results from each enrichment method (hypergeometric, diffusion, PageRank).
' ============================================================================

Namespace Core

    ''' <summary>
    ''' Approximation method for p-value computation.
    ''' </summary>
    Public Enum ApproximationMethod
        ''' <summary>Normal distribution approximation (fast, analytical)</summary>
        Normality
        ''' <summary>Monte Carlo simulation (slower, more accurate for small samples)</summary>
        Simulation
    End Enum

    ''' <summary>
    ''' Enrichment method type.
    ''' </summary>
    Public Enum EnrichmentMethod
        Hypergeometric
        Diffusion
        PageRank
    End Enum

    ''' <summary>
    ''' Result for a single node from enrichment analysis.
    ''' </summary>
    Public Class NodeResult
        ''' <summary>Node index in the graph</summary>
        Public Property NodeIndex As Integer

        ''' <summary>KEGG ID</summary>
        Public Property KeggId As String

        ''' <summary>Node name</summary>
        Public Property Name As String

        ''' <summary>Node type</summary>
        Public Property NodeType As KeggNodeType

        ''' <summary>Raw score (diffusion score or PageRank score)</summary>
        Public Property RawScore As Double

        ''' <summary>Z-score (normalized score)</summary>
        Public Property ZScore As Double

        ''' <summary>P-score (significance measure, lower = more significant)</summary>
        Public Property PScore As Double

        ''' <summary>Adjusted p-value (after multiple testing correction)</summary>
        Public Property AdjustedPValue As Double

        Public Overrides Function ToString() As String
            Return $"{KeggId} ({NodeType}): raw={RawScore:F6}, z={ZScore:F4}, p={PScore:E4}, adj={AdjustedPValue:E4}"
        End Function
    End Class

    ''' <summary>
    ''' Results from a single enrichment method.
    ''' </summary>
    Public Class EnrichmentResult
        ''' <summary>The method used</summary>
        Public Property Method As EnrichmentMethod

        ''' <summary>Approximation method used</summary>
        Public Property Approximation As ApproximationMethod

        ''' <summary>Results for all scored nodes</summary>
        Public Property NodeResults As New List(Of NodeResult)

        ''' <summary>Number of Monte Carlo iterations (if simulation was used)</summary>
        Public Property NSim As Integer

        ''' <summary>Get significant nodes below threshold</summary>
        Public Function GetSignificantNodes(threshold As Double) As List(Of NodeResult)
            Return NodeResults.Where(Function(r) r.PScore < threshold).OrderBy(Function(r) r.PScore).ToList()
        End Function

        ''' <summary>Get significant nodes of a specific type below threshold</summary>
        Public Function GetSignificantNodes(nodeType As KeggNodeType, threshold As Double) As List(Of NodeResult)
            Return NodeResults.Where(Function(r) r.NodeType = nodeType AndAlso r.PScore < threshold).
                               OrderBy(Function(r) r.PScore).ToList()
        End Function

        ''' <summary>Apply BH correction to p-scores</summary>
        Public Sub ApplyBHCorrection()
            If NodeResults.Count = 0 Then Return

            Dim pScores = NodeResults.Select(Function(r) r.PScore).ToArray()
            Dim adjusted = Math.Statistics.BenjaminiHochberg(pScores)

            For i = 0 To NodeResults.Count - 1
                NodeResults(i).AdjustedPValue = adjusted(i)
            Next
        End Sub
    End Class

    ''' <summary>
    ''' User analysis object. Stores input compounds and enrichment results.
    ''' Equivalent to the FELLA.USER S4 class in the R package.
    ''' </summary>
    Public Class FellaUser
        ''' <summary>Input compound KEGG IDs that were successfully mapped</summary>
        Public Property InputCompounds As New List(Of String)

        ''' <summary>Input compound KEGG IDs that could not be mapped</summary>
        Public Property ExcludedCompounds As New List(Of String)

        ''' <summary>Background compound KEGG IDs (defaults to all compounds in graph)</summary>
        Public Property BackgroundCompounds As New HashSet(Of String)

        ''' <summary>Results from hypergeometric test</summary>
        Public Property HypergeomResult As EnrichmentResult

        ''' <summary>Results from diffusion analysis</summary>
        Public Property DiffusionResult As EnrichmentResult

        ''' <summary>Results from PageRank analysis</summary>
        Public Property PagerankResult As EnrichmentResult

        ''' <summary>
        ''' Define input compounds and background.
        ''' Maps input compound IDs to the graph and identifies excluded compounds.
        ''' </summary>
        Public Sub DefineCompounds(compounds As IEnumerable(Of String), data As FellaData)
            InputCompounds.Clear()
            ExcludedCompounds.Clear()

            ' Set background to all compounds in the graph if not explicitly set
            If BackgroundCompounds.Count = 0 Then
                BackgroundCompounds = New HashSet(Of String)(data.BackgroundCompounds)
            End If

            ' Map compounds
            For Each id In compounds
                If data.Graph.ContainsId(id) AndAlso BackgroundCompounds.Contains(id) Then
                    InputCompounds.Add(id)
                Else
                    ExcludedCompounds.Add(id)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Check if any compounds were successfully mapped.
        ''' </summary>
        Public ReadOnly Property HasValidInput As Boolean
            Get
                Return InputCompounds.Count > 0
            End Get
        End Property

        ''' <summary>
        ''' Get the result for a specific method.
        ''' </summary>
        Public Function GetResult(method As EnrichmentMethod) As EnrichmentResult
            Select Case method
                Case EnrichmentMethod.Hypergeometric : Return HypergeomResult
                Case EnrichmentMethod.Diffusion : Return DiffusionResult
                Case EnrichmentMethod.PageRank : Return PagerankResult
                Case Else : Return Nothing
            End Select
        End Function

    End Class

End Namespace
