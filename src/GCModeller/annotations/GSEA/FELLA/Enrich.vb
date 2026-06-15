' ============================================================================
' FELLA Algorithm - VB.NET Implementation
' Enrich.vb - Main enrichment wrapper function
' 
' Provides a unified interface to run one or all enrichment methods
' (hypergeometric, diffusion, PageRank) on a set of input compounds.
' Equivalent to the enrich() function in the R package.
' ============================================================================

Namespace Core

    ''' <summary>
    ''' Main enrichment wrapper. Provides a convenient interface to run
    ''' one or more FELLA enrichment methods on a set of input compounds.
    ''' 
    ''' Usage:
    '''   Dim result = Enrich.Run(compounds, data, methods, approx)
    ''' </summary>
    Public Class Enrich

        ''' <summary>
        ''' Run enrichment analysis with specified methods and approximation.
        ''' </summary>
        ''' <param name="compounds">Input compound KEGG IDs</param>
        ''' <param name="data">Precomputed FELLA database</param>
        ''' <param name="methods">Methods to run (default: all three)</param>
        ''' <param name="approx">Approximation method for p-values</param>
        ''' <param name="nSim">Number of Monte Carlo iterations</param>
        ''' <returns>FellaUser object with all results populated</returns>
        Public Shared Function Run(compounds As IEnumerable(Of String),
                                    data As FellaData,
                                    Optional methods As List(Of EnrichmentMethod) = Nothing,
                                    Optional approx As ApproximationMethod = ApproximationMethod.Normality,
                                    Optional nSim As Integer = 1000) As FellaUser
            If methods Is Nothing Then
                methods = New List(Of EnrichmentMethod) From {
                    EnrichmentMethod.Hypergeometric,
                    EnrichmentMethod.Diffusion,
                    EnrichmentMethod.PageRank
                }
            End If

            ' Create user object and define compounds
            Dim user As New FellaUser()
            user.DefineCompounds(compounds, data)

            If Not user.HasValidInput Then
                Console.WriteLine("WARNING: No valid input compounds were mapped to the KEGG graph.")
                Return user
            End If

            Console.WriteLine($"FELLA Enrichment Analysis")
            Console.WriteLine($"  Input compounds: {user.InputCompounds.Count} mapped, {user.ExcludedCompounds.Count} excluded")
            Console.WriteLine($"  Background size: {user.BackgroundCompounds.Count}")
            Console.WriteLine($"  Methods: {String.Join(", ", methods.Select(Function(m) m.ToString()))}")
            Console.WriteLine($"  Approximation: {approx}")
            Console.WriteLine()

            ' Run each requested method
            For Each method In methods
                Console.WriteLine($"Running {method}...")
                Dim sw = System.Diagnostics.Stopwatch.StartNew()

                Select Case method
                    Case EnrichmentMethod.Hypergeometric
                        user.HypergeomResult = RunHypergeom.Run(user, data)

                    Case EnrichmentMethod.Diffusion
                        user.DiffusionResult = RunDiffusion.Run(user, data, approx, nSim)

                    Case EnrichmentMethod.PageRank
                        user.PagerankResult = RunPagerank.Run(user, data, approx, nSim)
                End Select

                sw.Stop()
                Console.WriteLine($"  Completed in {sw.ElapsedMilliseconds} ms")
            Next

            Console.WriteLine()
            Console.WriteLine("Enrichment analysis complete.")
            Return user
        End Function

        ''' <summary>
        ''' List all available enrichment methods.
        ''' </summary>
        Public Shared Function ListMethods() As List(Of EnrichmentMethod)
            Return [Enum].GetValues(GetType(EnrichmentMethod)).Cast(Of EnrichmentMethod)().ToList()
        End Function

    End Class

End Namespace
