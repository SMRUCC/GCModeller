Imports SMRUCC.genomics.Analysis.BNLearn.Intervention

Public Interface InsilicoPerturbationExperiment
    Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult
    Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult
    Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult
End Interface
