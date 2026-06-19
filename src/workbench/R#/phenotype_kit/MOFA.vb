Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Microarray.MultiOmics.MOFA
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports std = System.Math

<Package("MOFA")>
<RTypeExport("mofa_opts", GetType(MOFAOptions))>
Module MOFATools

    <ExportAPI("create_mofa")>
    Public Function create_mofa(<RListObjectArgument> data As list, Optional opts As MOFAOptions = Nothing, Optional env As Environment = Nothing) As Object
        Dim dataList As New List(Of DataView)

        For Each name As String In data.getNames
            Dim val As Matrix = TryCast(data.getByName(name), Matrix)

            If Not val Is Nothing Then
                Call dataList.Add(val.CreateDataView(name))
            End If
        Next

        Dim unionSampleIDs As String() = dataList _
            .SelectMany(Function(d) d.SampleIds) _
            .Distinct _
            .OrderBy(Function(id) id) _
            .ToArray

        opts = If(opts, New MOFAOptions With {
            .NumFactors = 10,                ' Start with 10, will be pruned
            .MaxIterations = 300,
            .ConvergenceTolerance = 0.0001,
            .DropFactorThreshold = 0.02,     ' 2% variance explained cutoff
            .DropIterations = 30,
            .StandardizeViews = True,        ' Z-score each view
            .Verbose = True,
            .PrintEvery = 20,
            .Seed = 42
        })

        Return New MOFA(dataList, opts)
    End Function

    <ExportAPI("elbo_trace")>
    Public Function ELBO(model As MOFA) As Object
        ' 5e) ELBO convergence trace
        Console.WriteLine("[ELBO Convergence Trace (first 10 and last 10 iterations)]")
        Dim nIters = model.ElboHistory.Count
        Dim showCount = std.Min(10, nIters)
        For i = 0 To showCount - 1
            Console.WriteLine($"   Iter {i + 1,4}: ELBO = {model.ElboHistory(i),12:F4}")
        Next
        If nIters > 20 Then
            Console.WriteLine($"   ...")
            For i = nIters - 10 To nIters - 1
                Console.WriteLine($"   Iter {i + 1,4}: ELBO = {model.ElboHistory(i),12:F4}")
            Next
        End If
        Console.WriteLine($"   Converged: {model.Converged}")
        Console.WriteLine()

        Dim trace As Double() = model.ElboHistory.ToArray

        Return trace
    End Function

    <ExportAPI("reconstruct")>
    Public Function reconstruct_func(model As MOFA) As list
        Dim reconstruct As New list

        For i As Integer = 0 To model.Views.Count - 1
            ' 5d) Impute missing samples in View 1 (Transcriptome)
            Console.WriteLine("[Imputation: Reconstructing View 1 (Transcriptome) for ALL 6 samples]")
            Console.WriteLine("   (S4, S5, S6 were NOT measured in transcriptome — they are imputed from Z*W)")
            Dim rnaReconstructed As Tensor = model.ReconstructView(i)
            Dim rnaSamples = model.Views(i).SampleIds
            Dim rnaFeatures = model.Views(i).FeatureNames
            Console.WriteLine($"   Reconstructed matrix shape: {rnaReconstructed.Shape(0)} samples × {rnaReconstructed.Shape(1)} features")
            Console.WriteLine($"   First 5 features for each sample:")
            Console.Write($"   {"Sample",8} ")
            For d = 0 To 4
                Console.Write($"{rnaFeatures(d),12} ")
            Next
            Console.WriteLine()
            For n = 0 To model.N - 1
                Dim isObserved = Array.IndexOf(rnaSamples, model.GlobalSampleIds(n)) >= 0
                Dim tag = If(isObserved, "(obs)", "(imp)")
                Console.Write($"   {model.GlobalSampleIds(n) + tag,8} ")
                For d = 0 To 4
                    Console.Write($"{rnaReconstructed(n, d),12:F4} ")
                Next
                Console.WriteLine()
            Next
            Console.WriteLine()

            Call reconstruct.add(model.Views(i).Name, rnaReconstructed.CreateExpressionMatrix(model.GlobalSampleIds, rnaFeatures))
        Next

        Return reconstruct
    End Function

    <ExportAPI("run_mofa")>
    Public Function run_mofa(model As MOFA) As MOFA
        ' ----- Step 4: Create and train the MOFA model -----
        Console.WriteLine()
        Console.WriteLine("[Step 3] Training MOFA model...")

        Call model.Train()

        ' ----- Step 5: Report results -----
        Console.WriteLine()
        Console.WriteLine("="c, 80)
        Console.WriteLine("  Results")
        Console.WriteLine("="c, 80)
        Console.WriteLine()

        ' 5a) Variance explained per view
        Console.WriteLine("[Variance Explained per View]")
        For m = 0 To model.Views.Count - 1
            Dim totalVE = model.ComputeTotalVarianceExplained(m)
            Console.WriteLine($"   View {m} ({ model.Views(m).Name}): total R² = {totalVE:P}")
        Next
        Console.WriteLine()

        ' 5b) Variance explained per factor per view
        Console.WriteLine("[Variance Explained per Factor per View]")
        Console.Write($"   {"Factor",8} ")
        For m = 0 To model.Views.Count - 1
            Console.Write($"{ model.Views(m).Name,18} ")
        Next
        Console.WriteLine()
        Console.Write($"   {"",8} ")
        For m = 0 To model.Views.Count - 1
            Console.Write($"{"R²",18} ")
        Next
        Console.WriteLine()
        Console.WriteLine(New String("-"c, 8 + 19 * model.Views.Count))

        For k = 0 To model.K - 1
            If Not model.ActiveFactors(k) Then Continue For
            Console.Write($"   {k,8} ")
            For m = 0 To model.Views.Count - 1
                Dim ve = model.ComputeVarianceExplained(m, k)
                Console.Write($"{ve,18:P} ")
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()

        ' 5c) Factor scores Z (sample × factor)
        Console.WriteLine("[Factor Scores Z (sample × factor)]")
        Dim Z = model.GetFactors()
        Console.Write($"   {"Sample",8} ")
        For k = 0 To Z.Shape(1) - 1
            Console.Write($"Factor{k + 1,12} ")
        Next
        Console.WriteLine()
        For n = 0 To model.N - 1
            Console.Write($"   {model.GlobalSampleIds(n),8} ")
            For k = 0 To Z.Shape(1) - 1
                Console.Write($"{Z(n, k),12:F4} ")
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()

        Return model
    End Function

End Module
