' =====================================================================================
'  MOFA Example Usage — Handling 3 vs 6 Biological Replicate Mismatch
'  
'  This module demonstrates how to use the MOFA framework when two omics views
'  have different numbers of biological replicates (e.g. transcriptome with 3
'  replicates vs metabolome with 6 replicates).
'  
'  Scenario:
'    - View 1 "Transcriptome": 3 biological replicates × 1000 genes
'    - View 2 "Metabolome":    6 biological replicates × 200 metabolites
'    - Some samples are shared between views, some are not
'  
'  MOFA handles this naturally:
'    1. Build a global sample space (union of all samples)
'    2. Each view only contributes its observed samples to the inference
'    3. Shared factors Z are inferred for ALL samples using all available info
'    4. Missing samples can be imputed via Z * W^m^T
' =====================================================================================

Imports System
Imports System.IO
Imports System.Linq
Imports MultiOmics.MOFA

Public Module MOFAExample

    Public Sub Main()
        Console.WriteLine("="c, 80)
        Console.WriteLine("  MOFA Example: Handling 3 vs 6 Biological Replicate Mismatch")
        Console.WriteLine("="c, 80)
        Console.WriteLine()

        ' ----- Step 1: Generate synthetic multi-omics data with mismatched samples -----
        Console.WriteLine("[Step 1] Generating synthetic data...")
        Dim rng As New Random(123)

        ' Define the global sample space — 6 unique biological replicates
        Dim globalSamples = {"S1", "S2", "S3", "S4", "S5", "S6"}

        ' View 1 (Transcriptome): only 3 replicates observed (S1, S2, S3)
        Dim rnaSamples = {"S1", "S2", "S3"}
        Dim rnaFeatures = Enumerable.Range(1, 100).Select(Function(i) $"Gene_{i}").ToArray()
        Dim rnaData = New Tensor(rnaSamples.Length, rnaFeatures.Length)
        For i = 0 To rnaSamples.Length - 1
            For d = 0 To rnaFeatures.Length - 1
                ' Simulated log-expression: 3 latent factors + noise
                Dim factor1 = Math.Sin((i + 1) * 0.7) * 2.0
                Dim factor2 = Math.Cos((d + 1) * 0.05) * 1.5
                Dim factor3 = If(i Mod 2 = 0, 1.0, -1.0) * 0.8
                rnaData(i, d) = factor1 * Math.Sin(d * 0.1) +
                                factor2 * Math.Cos(i * 0.5) +
                                factor3 * (d / 100.0) +
                                rng.NextGaussian() * 0.5
            Next
        Next

        ' View 2 (Metabolome): all 6 replicates observed
        Dim metabSamples = {"S1", "S2", "S3", "S4", "S5", "S6"}
        Dim metabFeatures = Enumerable.Range(1, 50).Select(Function(i) $"Metab_{i}").ToArray()
        Dim metabData = New Tensor(metabSamples.Length, metabFeatures.Length)
        For i = 0 To metabSamples.Length - 1
            For d = 0 To metabFeatures.Length - 1
                ' Same latent factors (so MOFA should recover them) + view-specific noise
                Dim factor1 = Math.Sin((i + 1) * 0.7) * 2.0
                Dim factor2 = Math.Cos((d + 1) * 0.1) * 1.5
                Dim factor3 = If(i Mod 2 = 0, 1.0, -1.0) * 0.8
                metabData(i, d) = factor1 * Math.Cos(d * 0.2) +
                                  factor2 * Math.Sin(i * 0.3) +
                                  factor3 * (d / 50.0) +
                                  rng.NextGaussian() * 0.3
            Next
        Next

        Console.WriteLine($"   View 1 (Transcriptome): {rnaSamples.Length} samples × {rnaFeatures.Length} features")
        Console.WriteLine($"      Samples: {String.Join(", ", rnaSamples)}")
        Console.WriteLine($"   View 2 (Metabolome):    {metabSamples.Length} samples × {metabFeatures.Length} features")
        Console.WriteLine($"      Samples: {String.Join(", ", metabSamples)}")
        Console.WriteLine($"   Global sample space:    {globalSamples.Length} samples")
        Console.WriteLine()

        ' ----- Step 2: Build DataView objects -----
        Console.WriteLine("[Step 2] Building DataView objects...")
        Dim rnaView As New DataView("Transcriptome", rnaData, rnaSamples, rnaFeatures)
        Dim metabView As New DataView("Metabolome", metabData, metabSamples, metabFeatures)

        Dim views As New List(Of DataView) From {rnaView, metabView}

        ' ----- Step 3: Configure MOFA options -----
        Dim options As New MOFAOptions() With {
            .NumFactors = 10,                ' Start with 10, will be pruned
            .MaxIterations = 300,
            .ConvergenceTolerance = 1.0E-4,
            .DropFactorThreshold = 0.02,     ' 2% variance explained cutoff
            .DropIterations = 30,
            .StandardizeViews = True,        ' Z-score each view
            .Verbose = True,
            .PrintEvery = 20,
            .Seed = 42
        }

        ' ----- Step 4: Create and train the MOFA model -----
        Console.WriteLine()
        Console.WriteLine("[Step 3] Training MOFA model...")
        Using model As New MOFA(views, options)
            model.Train()

            ' ----- Step 5: Report results -----
            Console.WriteLine()
            Console.WriteLine("="c, 80)
            Console.WriteLine("  Results")
            Console.WriteLine("="c, 80)
            Console.WriteLine()

            ' 5a) Variance explained per view
            Console.WriteLine("[Variance Explained per View]")
            For m = 0 To views.Count - 1
                Dim totalVE = model.ComputeTotalVarianceExplained(m)
                Console.WriteLine($"   View {m} ({views(m).Name}): total R² = {totalVE:P}")
            Next
            Console.WriteLine()

            ' 5b) Variance explained per factor per view
            Console.WriteLine("[Variance Explained per Factor per View]")
            Console.Write($"   {"Factor",8} ")
            For m = 0 To views.Count - 1
                Console.Write($"{views(m).Name,18} ")
            Next
            Console.WriteLine()
            Console.Write($"   {"",8} ")
            For m = 0 To views.Count - 1
                Console.Write($"{"R²",18} ")
            Next
            Console.WriteLine()
            Console.WriteLine(New String("-"c, 8 + 19 * views.Count))

            For k = 0 To model.K - 1
                If Not model.ActiveFactors(k) Then Continue For
                Console.Write($"   {k,8} ")
                For m = 0 To views.Count - 1
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

            ' 5d) Impute missing samples in View 1 (Transcriptome)
            Console.WriteLine("[Imputation: Reconstructing View 1 (Transcriptome) for ALL 6 samples]")
            Console.WriteLine("   (S4, S5, S6 were NOT measured in transcriptome — they are imputed from Z*W)")
            Dim rnaReconstructed = model.ReconstructView(0)
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

            ' 5e) ELBO convergence trace
            Console.WriteLine("[ELBO Convergence Trace (first 10 and last 10 iterations)]")
            Dim nIters = model.ElboHistory.Count
            Dim showCount = Math.Min(10, nIters)
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

            ' 5f) Save results to file
            Console.WriteLine("[Saving results to file...]")
            SaveResults(model, views, "/home/z/my-project/download/MOFA_results.txt")
            Console.WriteLine("   Saved to: /home/z/my-project/download/MOFA_results.txt")
        End Using

        Console.WriteLine()
        Console.WriteLine("="c, 80)
        Console.WriteLine("  Example completed successfully.")
        Console.WriteLine("="c, 80)
    End Sub

    ''' <summary>Save MOFA results to a text file for downstream analysis</summary>
    Private Sub SaveResults(model As MOFA, views As List(Of DataView), filePath As String)
        Using sw As New StreamWriter(filePath)
            sw.WriteLine("MOFA Results")
            sw.WriteLine("============")
            sw.WriteLine()
            sw.WriteLine($"Number of samples (global): {model.N}")
            sw.WriteLine($"Number of views: {model.M}")
            sw.WriteLine($"Initial factors: {model.Options.NumFactors}")
            sw.WriteLine($"Active factors: {model.CountActiveFactorsPublic()}")
            sw.WriteLine($"Converged: {model.Converged}")
            sw.WriteLine()

            ' Global sample IDs
            sw.WriteLine("Global Sample IDs:")
            For Each s In model.GlobalSampleIds
                sw.WriteLine($"  {s}")
            Next
            sw.WriteLine()

            ' Variance explained
            sw.WriteLine("Variance Explained per View:")
            For m = 0 To views.Count - 1
                Dim ve = model.ComputeTotalVarianceExplained(m)
                sw.WriteLine($"  View {m} ({views(m).Name}): {ve:P}")
            Next
            sw.WriteLine()

            ' Factor scores
            sw.WriteLine("Factor Scores Z (sample × factor):")
            Dim Z = model.GetFactors()
            sw.Write("Sample" & vbTab)
            For k = 0 To Z.Shape(1) - 1
                sw.Write($"Factor{k + 1}" & vbTab)
            Next
            sw.WriteLine()
            For n = 0 To model.N - 1
                sw.Write(model.GlobalSampleIds(n) & vbTab)
                For k = 0 To Z.Shape(1) - 1
                    sw.Write(Z(n, k).ToString("F6") & vbTab)
                Next
                sw.WriteLine()
            Next
            sw.WriteLine()

            ' Weights per view
            For m = 0 To views.Count - 1
                sw.WriteLine($"Weights W for View {m} ({views(m).Name}):")
                Dim Wm = model.GetWeights(m)
                sw.Write("Feature" & vbTab)
                For k = 0 To Wm.Shape(1) - 1
                    sw.Write($"Factor{k + 1}" & vbTab)
                Next
                sw.WriteLine()
                For d = 0 To views(m).D - 1
                    sw.Write(views(m).FeatureNames(d) & vbTab)
                    For k = 0 To Wm.Shape(1) - 1
                        sw.Write(Wm(d, k).ToString("F6") & vbTab)
                    Next
                    sw.WriteLine()
                Next
                sw.WriteLine()
            Next

            ' ELBO history
            sw.WriteLine("ELBO History:")
            For i = 0 To model.ElboHistory.Count - 1
                sw.WriteLine($"  Iter {i + 1}: {model.ElboHistory(i):F6}")
            Next
        End Using
    End Sub

    ' Extension method to expose private CountActiveFactors for the example
    <System.Runtime.CompilerServices.Extension>
    Public Function CountActiveFactorsPublic(model As MOFA) As Integer
        ' Use reflection or just count via GetFactors shape
        Return model.GetFactors().Shape(1)
    End Function

End Module

' =====================================================================================
'  Extension: Random.NextGaussian — Box-Muller transform for synthetic data
' =====================================================================================
Public Module RandomExtensions
    <System.Runtime.CompilerServices.Extension>
    Public Function NextGaussian(rng As Random, Optional mean As Double = 0.0, Optional stdDev As Double = 1.0) As Double
        Dim u1 = 1.0 - rng.NextDouble()
        Dim u2 = 1.0 - rng.NextDouble()
        Dim randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2)
        Return mean + stdDev * randStdNormal
    End Function
End Module
