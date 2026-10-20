Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Model.Network.Regulons.LinearRegression
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

<Package("linear")>
Module Linear

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="top"></param>
    ''' <param name="candidates"></param>
    ''' <param name="maxreg"></param>
    ''' <param name="l1_ratio"></param>
    ''' <param name="alphas"></param>
    ''' <param name="mutual"></param>
    ''' <returns></returns>
    <ExportAPI("elastic_net")>
    <RApiReturn(GetType(GeneRegulatoryNetwork), GetType(RegulationEdge))>
    Public Function elastic_net(x As Matrix, Optional top As Integer = 1000,
                                Optional candidates As Integer = 1000,
                                Optional maxreg As Integer = 10,
                                Optional l1_ratio As Double = 0.7,
                                Optional alphas As Double = 40,
                                Optional mutual As Boolean = False)

        Dim options As New GrnFittingOptions With {
            .silent = True,
            .method = FittingMethod.ElasticNet,
            .maxCandidates = candidates,
            .maxRegulatorsPerGene = maxreg,
            .mutualOnly = mutual,
            .elasticNet = New ElasticNetOptions With {
                .l1Ratio = l1_ratio,
                .numberOfAlphas = alphas
            }
        }
        Dim t0 = Now

        If mutual Then
            Console.WriteLine("     >> only the mutual (reversible) regulation pairs are kept")
        End If

        Dim grn As GeneRegulatoryNetwork = x.InferRegulatoryNetwork(options)
        Dim edges As RegulationEdge() = grn.Regulations().ToArray
        Dim failed As Integer = grn.Models.Where(Function(m) Not m.IsValid).Count()

        Console.WriteLine($"     >> fitting cost {StringFormats.ReadableElapsedTime(Now - t0)}, " &
                          $"{edges.Length} regulation edges, {failed} failed model(s)")

        Console.WriteLine("     >> top regulons (by the number of target genes):")

        For Each item In edges.RegulonSizes().Take(8)
            Console.WriteLine($"        {item.regulator} ({item.size} targets)")
        Next

        Console.WriteLine("     >> top regulation edges (by |beta|):")

        For Each edge As RegulationEdge In edges.TopEdges(10)
            Console.WriteLine($"        {edge.interaction}: {edge.regulator} -> {edge.target}, " &
                              $"beta={edge.weight:G4}, alpha={edge.alpha:G4}, R2={edge.r2:F3}")
        Next

        Dim significant As Integer = edges.Count(Function(e) e.r2 > 0.9)

        Console.WriteLine($"     >> {significant} edges come from the gene models with R2 > 0.9")

        Return New list(slot("grn") = grn, slot("edges") = edges)
    End Function

    <ExportAPI("coef")>
    Public Function coef(grn As GeneRegulatoryNetwork) As Matrix
        Return grn.CoefMatrix
    End Function

End Module
