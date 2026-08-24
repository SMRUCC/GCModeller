#Region "Microsoft.VisualBasic::7a3a3a8402b10112578fdd478b865242, annotations\GSEA\FELLA\Enrich.vb"

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

    '   Total Lines: 93
    '    Code Lines: 49 (52.69%)
    ' Comment Lines: 29 (31.18%)
    '    - Xml Docs: 62.07%
    ' 
    '   Blank Lines: 15 (16.13%)
    '     File Size: 4.10 KB


    '     Class Enrich
    ' 
    '         Function: ListMethods, Run
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

