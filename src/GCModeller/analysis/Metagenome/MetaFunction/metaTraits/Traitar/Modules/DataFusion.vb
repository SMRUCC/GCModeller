' ============================================================================
'  Module 3 - DataFusion.vb
'  Data fusion & extended sample construction
'
'  Combines the modern-sample phyletic profile X with the ancestral
'  gain/loss events inferred by Module 2 to form an extended training set.
'
'  For each ancestral branch b and each Pfam family f we have:
'    g_bf = posterior probability that f was gained on branch b
'    l_bf = posterior probability that f was lost  on branch b
'  The joint probability that f changed state on b is:
'    x_bf = g_bf + l_bf - g_bf * l_bf
'
'  Similarly for each phenotype p we have gain/loss posteriors on each
'  branch (g_bp, l_bp) and the joint x_bp.
'
'  Discretization at threshold t = 0.5:
'    x_bf -> 1 if x_bf >= t, 0 if x_bf <= 1 - t, otherwise "uncertain" (drop)
'  This yields ancestral "virtual samples" (one per branch) with binary
'  Pfam features and binary phenotype labels, which are appended to the
'  modern-sample matrix to form the extended classification problem.
' ============================================================================
Imports System
Imports System.Collections.Generic
Imports TraitarVBNet.Utils

Namespace Modules

    ''' <summary>
    ''' One ancestral "virtual sample" produced by the data-fusion step.
    ''' Carries a binary Pfam profile and a binary phenotype label, just like
    ''' a modern GenomeSample but representing an ancestral branch.
    ''' </summary>
    Public Class AncestralSample
        Public Property BranchId As String = ""
        Public Property PfamProfile As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Public Property PhenotypeLabel As Integer
    End Class

    Public Module DataFusion

        ''' <summary>
        ''' Joint probability that a Pfam (or phenotype) changed state on a
        ''' branch, given independent gain and loss posteriors:
        '''   x = g + l - g * l
        ''' </summary>
        Public Function JointChangeProbability(g As Double, l As Double) As Double
            Return MathUtils.JointGainLoss(g, l)
        End Function

        ''' <summary>
        ''' Discretize a posterior probability into a binary label at threshold t.
        ''' Returns 1 if p >= t, 0 if p <= 1 - t, and Nothing if uncertain
        ''' (the caller should drop uncertain samples).
        ''' </summary>
        Public Function Discretize(p As Double, t As Double) As Integer?
            Dim d As Double = MathUtils.DiscretizeWithThreshold(p, t)
            If Double.IsNaN(d) Then Return Nothing
            Return CInt(d)
        End Function

        ''' <summary>
        ''' Build the extended training set for one phenotype.
        '''
        ''' Inputs:
        '''   modernSamples      - list of (Pfam profile, label) for modern genomes
        '''   branchPfamEvents   - per branch, per Pfam: (gain posterior, loss posterior)
        '''   branchPhenotypeEvents - per branch: (gain posterior, loss posterior) for this phenotype
        '''   threshold          - discretization threshold (default 0.5)
        '''
        ''' Output:
        '''   A list of (Pfam profile, label) pairs combining modern samples
        '''   and ancestral virtual samples (uncertain branches are dropped).
        ''' </summary>
        Public Function BuildExtendedDataset(
                modernSamples As List(Of Tuple(Of Dictionary(Of String, Integer), Integer)),
                branchPfamEvents As Dictionary(Of String, Dictionary(Of String, Tuple(Of Double, Double))),
                branchPhenotypeEvents As Dictionary(Of String, Tuple(Of Double, Double)),
                Optional threshold As Double = 0.5R) _
            As List(Of Tuple(Of Dictionary(Of String, Integer), Integer))

            Dim extended As New List(Of Tuple(Of Dictionary(Of String, Integer), Integer))()

            ' 1. Copy modern samples unchanged
            For Each s As Tuple(Of Dictionary(Of String, Integer), Integer) In modernSamples
                extended.Add(s)
            Next

            ' 2. For each ancestral branch, build a virtual sample
            For Each branchKv As KeyValuePair(Of String, Tuple(Of Double, Double)) In branchPhenotypeEvents
                Dim branchId As String = branchKv.Key
                Dim phenoG As Double = branchKv.Value.Item1
                Dim phenoL As Double = branchKv.Value.Item2
                Dim phenoJoint As Double = JointChangeProbability(phenoG, phenoL)
                Dim label? As Integer = Discretize(phenoJoint, threshold)
                If label Is Nothing Then Continue For   ' uncertain -> drop
                Dim y As Integer = CInt(label)

                ' Build the Pfam profile for this branch
                Dim profile As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
                If branchPfamEvents.ContainsKey(branchId) Then
                    For Each pfamKv As KeyValuePair(Of String, Tuple(Of Double, Double)) In branchPfamEvents(branchId)
                        Dim pfamG As Double = pfamKv.Value.Item1
                        Dim pfamL As Double = pfamKv.Value.Item2
                        Dim pfamJoint As Double = JointChangeProbability(pfamG, pfamL)
                        Dim v? As Integer = Discretize(pfamJoint, threshold)
                        If v Is Nothing Then Continue For
                        profile(pfamKv.Key) = CInt(v)
                    Next
                End If

                extended.Add(Tuple.Create(profile, y))
            Next

            Return extended
        End Function

        ''' <summary>
        ''' Convert a list of (Pfam profile, label) samples into a dense
        ''' feature matrix X (samples x features) and label vector y.
        ''' Features are the union of all Pfam accessions seen across samples.
        ''' </summary>
        Public Function ToDenseMatrix(
                samples As List(Of Tuple(Of Dictionary(Of String, Integer), Integer)),
                ByRef featureNames As List(Of String)) _
            As Tuple(Of Double(,), Integer())
            ' Collect the union of features
            Dim featSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For Each s As Tuple(Of Dictionary(Of String, Integer), Integer) In samples
                For Each k As String In s.Item1.Keys
                    featSet.Add(k)
                Next
            Next
            featureNames = New List(Of String)(featSet)
            featureNames.Sort(StringComparer.OrdinalIgnoreCase)

            Dim nSamples As Integer = samples.Count
            Dim nFeats As Integer = featureNames.Count
            Dim X(nSamples - 1, nFeats - 1) As Double
            Dim y(nSamples - 1) As Integer
            For i As Integer = 0 To nSamples - 1
                Dim profile As Dictionary(Of String, Integer) = samples(i).Item1
                For j As Integer = 0 To nFeats - 1
                    If profile.ContainsKey(featureNames(j)) Then
                        X(i, j) = CDbl(profile(featureNames(j)))
                    Else
                        X(i, j) = 0.0R
                    End If
                Next
                y(i) = samples(i).Item2
            Next
            Return Tuple.Create(X, y)
        End Function

    End Module

End Namespace
