Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.CellPhenotype
Imports SMRUCC.genomics.Model.Network.Regulons
Imports SMRUCC.genomics.Model.Network.Regulons.LinearRegression
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

<Package("linear")>
<RTypeExport("elastic_net.options", GetType(ElasticNetPerturbationOptions))>
<RTypeExport("linear_grn", GetType(LinearRegression.GeneRegulatoryNetwork))>
Module Linear

    Sub Main()
        Call RInternal.generic.add("writeBin", GetType(LinearRegression.GeneRegulatoryNetwork), AddressOf saveModel)
        Call RInternal.generic.add("readBin.linear_grn", GetType(Stream), AddressOf loadModel)
    End Sub

    ''' <summary>
    ''' R# ``writeBin`` handler: persist a GPS model into the given connection.
    ''' </summary>
    ''' <param name="model">the trained GPS model to be serialized</param>
    ''' <param name="args">the R# named argument list; the ``con`` argument supplies the output stream</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns><c>True</c> when the model has been written successfully</returns>
    Private Function saveModel(model As LinearRegression.GeneRegulatoryNetwork, args As list, env As Environment) As Object
        Dim con As Stream = args!con

        Try
            Call model.Save(con)
            Call con.Flush()
        Catch ex As Exception

        End Try

        Return True
    End Function

    ''' <summary>
    ''' R# ``readBin.modular_bayesian`` handler: restore a GPS model from a connection.
    ''' </summary>
    ''' <param name="s">the stream that contains a GPS model package</param>
    ''' <param name="args">the R# named argument list (unused)</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>the reconstructed <see cref="LinearRegression.GeneRegulatoryNetwork"/> object</returns>
    Private Function loadModel(s As Stream, args As list, env As Environment) As Object
        Return GRNFile.Load(s)
    End Function

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
    <RApiReturn(GetType(LinearRegression.GeneRegulatoryNetwork), GetType(RegulationEdge))>
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

        Dim grn As LinearRegression.GeneRegulatoryNetwork = x.InferRegulatoryNetwork(options)
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
    Public Function coef(grn As LinearRegression.GeneRegulatoryNetwork) As Matrix
        Return grn.CoefMatrix
    End Function

    <ExportAPI("elastic_model")>
    Public Function elastic_model(grn As LinearRegression.GeneRegulatoryNetwork, Optional opts As ElasticNetPerturbationOptions = Nothing) As ElasticNet
        Return New ElasticNet(grn, opts)
    End Function

End Module
