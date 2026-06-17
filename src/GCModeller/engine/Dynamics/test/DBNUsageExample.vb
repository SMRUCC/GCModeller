' File: DBNUsageExample.vb
'
' Example usage of the Dynamic Bayesian Network for gene regulatory network simulation.
' Demonstrates:
' 1. Building a DBN from RegulatoryLink topology (topology-only mode)
' 2. Running prediction with metabolite and TF evidence (ODEs -> DBN)
' 3. Getting RNA transcript abundance changes for ODE coupling (DBN -> ODEs)
' 4. Learning parameters from synthetic RNAseq time-series data (data-fitting mode)
' 5. Using the DBNODECoupler for integrated simulation
' 6. Saving and loading the DBN
'
' This example uses a simplified lac operon-like regulatory network:
'   - TF1 (CRP-like): activated by cAMP (Activator effector) -> activates operon1
'   - TF2 (LacI-like): inhibited by allolactose (Inhibitor effector) -> represses operon1
'   - TF3: regulates operon2 directly (no effector, default Activator)
'
' Note: This file assumes RegulatoryLink and Effector are defined in the same project.
' If they are in a different namespace, add the appropriate Imports statement.

Imports Cella
Imports Cella.VirtualCell.DBN

' Note: The RegulatoryLink and Effector types should be defined in your project.
' They are assumed to be in the global namespace or an imported namespace.
' The definitions from the user's code:
'
' Public Enum Effector
'     Unknown
'     Activator
'     Inhibitor
' End Enum
'
' Public Class RegulatoryLink
'     Public Property TF_id As String
'     Public Property TF_family As String
'     Public Property TFBS_id As String
'     Public Property effector As Dictionary(Of String, Effector)
'     Public Property target_operon As String
'     Public Property regulate_genes As String()
' End Class

Module DBNUsageExample

    Sub Main()
        Console.WriteLine(New String("="c, 60))
        Console.WriteLine("Dynamic Bayesian Network - Usage Example")
        Console.WriteLine("Virtual Cell Computational Engine")
        Console.WriteLine(New String("="c, 60))
        Console.WriteLine()

        ' ================================================================
        ' Step 1: Define the regulatory network topology
        ' ================================================================
        Console.WriteLine("--- Step 1: Define Regulatory Network Topology ---")
        Console.WriteLine()

        Dim links As New List(Of RegulatoryLink)()

        ' Link 1: TF1 (CRP-like) + cAMP (Activator) -> operon1
        Dim link1 As New RegulatoryLink()
        link1.TF_id = "CRP"
        link1.TF_family = "CRP_Family"
        link1.TFBS_id = "CRP_TFBS_001"
        link1.effector = New Dictionary(Of String, Effector) From {
            {"cAMP", Effector.Activator}
        }
        link1.target_operon = "lac_operon"
        link1.regulate_genes = New String() {"lacZ", "lacY", "lacA"}
        links.Add(link1)

        ' Link 2: TF2 (LacI) + allolactose (Inhibitor of repression) -> operon1
        ' Note: Inhibitor means high effector -> gene inhibition is relieved
        ' But in our model, Inhibitor means high effector -> gene inhibition
        ' So for LacI (a repressor), allolactose should be modeled as:
        '   - Effector.Inhibitor: high allolactose -> gene inhibition (LacI represses)
        ' Wait, this is the opposite of biology. Let me reconsider.
        '
        ' In our DBN model:
        '   - Effector.Activator: high effector -> gene activation
        '   - Effector.Inhibitor: high effector -> gene inhibition
        '
        ' For the lac operon:
        '   - LacI is a repressor. When allolactose is present, LacI is inactivated,
        '     so the gene is expressed (activated).
        '   - So allolactose should be Effector.Activator (high allolactose -> gene activation)
        '
        ' For CRP:
        '   - CRP is an activator. When cAMP is present, CRP is activated,
        '     so the gene is expressed (activated).
        '   - So cAMP should be Effector.Activator (high cAMP -> gene activation)
        '
        ' Let me use a more biologically accurate example:

        ' Reset links for biologically accurate example
        links.Clear()

        ' Link 1: CRP + cAMP (Activator) -> lac_operon (activation)
        link1 = New RegulatoryLink()
        link1.TF_id = "CRP"
        link1.TF_family = "CRP_Family"
        link1.TFBS_id = "CRP_TFBS_001"
        link1.effector = New Dictionary(Of String, Effector) From {
            {"cAMP", Effector.Activator}  ' cAMP activates CRP -> gene activation
        }
        link1.target_operon = "lac_operon"
        link1.regulate_genes = New String() {"lacZ", "lacY", "lacA"}
        links.Add(link1)

        ' Link 2: LacI + allolactose (Activator, because it relieves repression)
        ' Actually, in our model, we need to think about this differently.
        ' LacI is a repressor. Allolactose inactivates LacI.
        ' So: high allolactose -> LacI inactive -> gene expressed (activated)
        ' => allolactose is Effector.Activator
        Dim link2 As New RegulatoryLink()
        link2.TF_id = "LacI"
        link2.TF_family = "LacI_Family"
        link2.TFBS_id = "LacI_TFBS_001"
        link2.effector = New Dictionary(Of String, Effector) From {
            {"allolactose", Effector.Activator}  ' allolactose relieves LacI repression -> activation
        }
        link2.target_operon = "lac_operon"
        link2.regulate_genes = New String() {"lacZ", "lacY", "lacA"}
        links.Add(link2)

        ' Link 3: TF3 (no effector) -> operon2 (default Activator)
        Dim link3 As New RegulatoryLink()
        link3.TF_id = "TF3"
        link3.TF_family = "AraC_Family"
        link3.TFBS_id = "TF3_TFBS_001"
        link3.effector = Nothing  ' No effector
        link3.target_operon = "ara_operon"
        link3.regulate_genes = New String() {"araA", "araB", "araD"}
        links.Add(link3)

        Console.WriteLine("Defined {0} regulatory links:", links.Count)
        For Each link In links
            Dim effStr = "none"
            If link.effector IsNot Nothing AndAlso link.effector.Count > 0 Then
                effStr = String.Join(", ", link.effector.Select(
                    Function(kv) String.Format("{0}({1})", kv.Key, kv.Value.ToString())))
            End If
            Console.WriteLine("  {0} + [{1}] -> {2} (genes: {3})",
                link.TF_id, effStr, link.target_operon, String.Join(", ", link.regulate_genes))
        Next
        Console.WriteLine()

        ' ================================================================
        ' Step 2: Build the DBN from topology
        ' ================================================================
        Console.WriteLine("--- Step 2: Build DBN from Topology ---")
        Console.WriteLine()

        ' Create DBN with custom configuration
        Dim config As New DBNConfig()
        config.SmoothingAlpha = 1.0
        config.UseMultinomialSampling = False  ' Deterministic (argmax)
        config.LowThreshold = 0.33
        config.HighThreshold = 0.66
        config.Seed = 42

        Dim dbn As New DynamicBayesianNetwork(config)
        dbn.BuildFromTopology(links)

        Console.WriteLine(dbn.GetSummary())
        Console.WriteLine()

        ' ================================================================
        ' Step 3: Run prediction (ODEs -> DBN)
        ' ================================================================
        Console.WriteLine("--- Step 3: Prediction (ODEs -> DBN) ---")
        Console.WriteLine()

        ' Scenario A: High cAMP + High allolactose + High LacI + High CRP
        ' Expected: lac_operon = High (both CRP activated and LacI inactivated)
        Console.WriteLine("Scenario A: High cAMP, High allolactose, High CRP, High LacI")
        Dim metabolitesA As New Dictionary(Of String, Double) From {
            {"cAMP", 0.9},
            {"allolactose", 0.8}
        }
        Dim tfAbundancesA As New Dictionary(Of String, Double) From {
            {"CRP", 0.85},
            {"LacI", 0.7},
            {"TF3", 0.5}
        }

        Dim resultA = dbn.PredictNextState(metabolitesA, tfAbundancesA)
        PrintPredictionResult(resultA)
        Console.WriteLine()

        ' Scenario B: Low cAMP + Low allolactose + High LacI
        ' Expected: lac_operon = Low (CRP not activated, LacI active -> repression)
        Console.WriteLine("Scenario B: Low cAMP, Low allolactose, High CRP, High LacI")
        Dim metabolitesB As New Dictionary(Of String, Double) From {
            {"cAMP", 0.1},
            {"allolactose", 0.1}
        }
        Dim tfAbundancesB As New Dictionary(Of String, Double) From {
            {"CRP", 0.85},
            {"LacI", 0.7},
            {"TF3", 0.5}
        }

        Dim resultB = dbn.PredictNextState(metabolitesB, tfAbundancesB)
        PrintPredictionResult(resultB)
        Console.WriteLine()

        ' Scenario C: High cAMP + Low allolactose + High LacI
        ' Expected: lac_operon = Medium/Low (CRP activated but LacI repressing)
        Console.WriteLine("Scenario C: High cAMP, Low allolactose, High CRP, High LacI")
        Dim metabolitesC As New Dictionary(Of String, Double) From {
            {"cAMP", 0.9},
            {"allolactose", 0.1}
        }
        Dim tfAbundancesC As New Dictionary(Of String, Double) From {
            {"CRP", 0.85},
            {"LacI", 0.7},
            {"TF3", 0.5}
        }

        Dim resultC = dbn.PredictNextState(metabolitesC, tfAbundancesC)
        PrintPredictionResult(resultC)
        Console.WriteLine()

        ' ================================================================
        ' Step 4: Get RNA transcription rates for ODE coupling (DBN -> ODEs)
        ' ================================================================
        Console.WriteLine("--- Step 4: RNA Transcription Rates (DBN -> ODEs) ---")
        Console.WriteLine()

        Dim transcriptionRates = resultA.RNAAbundanceChanges
        Console.WriteLine("Transcription rates for Scenario A:")
        For Each kv In transcriptionRates
            Console.WriteLine("  {0}: {1:F4}", kv.Key, kv.Value)
        Next
        Console.WriteLine()

        ' Get gene-level rates (operon -> individual genes)
        Console.WriteLine("Gene-level transcription rates:")
        For Each operonKv In resultA.OperonGeneMapping
            Dim rate = resultA.RNAAbundanceChanges(operonKv.Key)
            For Each geneId In operonKv.Value
                Console.WriteLine("  {0} (from {1}): {2:F4}", geneId, operonKv.Key, rate)
            Next
        Next
        Console.WriteLine()

        ' ================================================================
        ' Step 5: Learn parameters from synthetic RNAseq data
        ' ================================================================
        Console.WriteLine("--- Step 5: Parameter Learning from RNAseq Data ---")
        Console.WriteLine()

        ' Generate synthetic RNAseq time-series data
        ' In a real application, this would come from actual RNAseq experiments
        Dim rnaSeqData = GenerateSyntheticRNAseqData()
        Console.WriteLine("Generated {0} time points of synthetic RNAseq data", rnaSeqData.Count)

        ' Compute log-likelihood before learning
        Dim llBefore = dbn.ComputeLogLikelihood(rnaSeqData)
        Console.WriteLine("Log-likelihood BEFORE learning: {0:F2}", llBefore)

        ' Learn parameters (topology as Dirichlet prior, data refines)
        dbn.LearnParameters(rnaSeqData)

        ' Compute log-likelihood after learning
        Dim llAfter = dbn.ComputeLogLikelihood(rnaSeqData)
        Console.WriteLine("Log-likelihood AFTER learning:  {0:F2}", llAfter)
        Console.WriteLine("Improvement: {0:F2}", llAfter - llBefore)
        Console.WriteLine()

        ' Run prediction again with learned parameters
        Console.WriteLine("Prediction after parameter learning (Scenario A):")
        Dim resultLearned = dbn.PredictNextState(metabolitesA, tfAbundancesA)
        PrintPredictionResult(resultLearned)
        Console.WriteLine()

        ' ================================================================
        ' Step 6: Integrated simulation using DBNODECoupler
        ' ================================================================
        Console.WriteLine("--- Step 6: Integrated Simulation with DBNODECoupler ---")
        Console.WriteLine()

        Dim coupler As New DBNODECoupler(dbn)

        Console.WriteLine("Running 5-step simulation:")
        For t = 1 To 5
            ' Simulate changing metabolite and TF levels over time
            Dim cAMPLevel = 0.2 + 0.15 * t  ' Increasing cAMP
            Dim allolactoseLevel = 0.1 + 0.2 * t  ' Increasing allolactose
            Dim crpLevel = 0.6
            Dim laciLevel = 0.7 - 0.1 * t  ' Decreasing LacI

            Dim mets As New Dictionary(Of String, Double) From {
                {"cAMP", Math.Min(1.0, cAMPLevel)},
                {"allolactose", Math.Min(1.0, allolactoseLevel)}
            }
            Dim tfs As New Dictionary(Of String, Double) From {
                {"CRP", crpLevel},
                {"LacI", Math.Max(0.0, laciLevel)},
                {"TF3", 0.5}
            }

            ' ODEs -> DBN: predict gene states
            Dim result = coupler.Step(mets, tfs)

            ' DBN -> ODEs: get transcription rates
            Dim rates = coupler.GetRNATranscriptionRates(result)

            Console.WriteLine("  t={0}: cAMP={1:F2}, allolactose={2:F2} -> lac_operon={3}, rate={4:F3}",
                t, cAMPLevel, allolactoseLevel,
                result.GeneStates("lac_operon"),
                result.RNAAbundanceChanges("lac_operon"))
        Next
        Console.WriteLine()

        ' ================================================================
        ' Step 7: Save and load the DBN
        ' ================================================================
        Console.WriteLine("--- Step 7: Save and Load DBN ---")
        Console.WriteLine()

        Dim savePath = "dbn_model.txt"
        dbn.SaveToFile(savePath)
        Console.WriteLine("DBN saved to: {0}", savePath)

        ' Create a new DBN and load
        Dim dbn2 As New DynamicBayesianNetwork()
        dbn2.LoadFromFile(savePath)
        Console.WriteLine("DBN loaded into new instance")
        Console.WriteLine("  Nodes: {0}", dbn2.GetAllNodes().Count)
        Console.WriteLine()

        ' Verify loaded DBN produces same predictions
        Dim resultLoaded = dbn2.PredictNextState(metabolitesA, tfAbundancesA)
        Console.WriteLine("Prediction from loaded DBN (Scenario A):")
        PrintPredictionResult(resultLoaded)

        Console.WriteLine()
        Console.WriteLine(New String("="c, 60))
        Console.WriteLine("Example completed successfully!")
        Console.WriteLine(New String("="c, 60))
    End Sub


    ' ================================================================
    ' Helper: Print prediction result
    ' ================================================================
    Sub PrintPredictionResult(result As DBNPredictionResult)
        For Each kv In result.GeneStates
            Dim probs = result.GeneProbabilities(kv.Key)
            Dim probStr = String.Join(", ", probs.Select(
                Function(p, i) String.Format("{0}:{1:F2}",
                    If(i = 0, "L", If(i = 1, "M", "H")), p)))
            Console.WriteLine("  {0}: state={1} (conf={2:F2}) [{3}]  RNA_rate={4:F4}",
                kv.Key, kv.Value, result.GeneStateProbabilities(kv.Key),
                probStr, result.RNAAbundanceChanges(kv.Key))
        Next
    End Sub


    ' ================================================================
    ' Helper: Generate synthetic RNAseq time-series data
    ' ================================================================
    Function GenerateSyntheticRNAseqData() As List(Of Dictionary(Of String, Double))
        Dim data As New List(Of Dictionary(Of String, Double))
        Dim rng As New Random(123)

        ' Generate 20 time points with correlated changes
        For t = 0 To 19
            Dim tp As New Dictionary(Of String, Double)

            ' cAMP increases over time
            Dim cAMP = 0.1 + 0.04 * t + 0.05 * rng.NextDouble()
            ' allolactose increases over time
            Dim allolactose = 0.05 + 0.045 * t + 0.05 * rng.NextDouble()
            ' CRP is constant
            Dim crp = 0.7 + 0.1 * rng.NextDouble()
            ' LacI decreases over time
            Dim laci = 0.8 - 0.03 * t + 0.05 * rng.NextDouble()
            ' TF3 oscillates
            Dim tf3 = 0.5 + 0.3 * Math.Sin(t * 0.5) + 0.05 * rng.NextDouble()

            ' lac_operon expression: high when cAMP and allolactose are high
            Dim lacExpr = 0.1 + 0.4 * Math.Min(1.0, cAMP) * Math.Min(1.0, allolactose) + 0.05 * rng.NextDouble()
            ' ara_operon expression: follows TF3
            Dim araExpr = 0.2 + 0.6 * Math.Max(0, Math.Min(1.0, tf3)) + 0.05 * rng.NextDouble()

            tp("cAMP") = Math.Max(0, Math.Min(1, cAMP))
            tp("allolactose") = Math.Max(0, Math.Min(1, allolactose))
            tp("CRP") = Math.Max(0, Math.Min(1, crp))
            tp("LacI") = Math.Max(0, Math.Min(1, laci))
            tp("TF3") = Math.Max(0, Math.Min(1, tf3))
            tp("lac_operon") = Math.Max(0, Math.Min(1, lacExpr))
            tp("ara_operon") = Math.Max(0, Math.Min(1, araExpr))

            data.Add(tp)
        Next

        Return data
    End Function

End Module
