Namespace DBN

    ' ==================== Enums ====================

    ''' <summary>
    ''' Type of node in the Dynamic Bayesian Network.
    ''' Determines how the node participates in inference and coupling.
    ''' </summary>
    Public Enum DBNNodeType
        ''' <summary>Target gene or operon being regulated. Expression predicted by CPT.</summary>
        Gene
        ''' <summary>Transcription factor (protein or RNA). State provided as evidence from ODEs.</summary>
        TranscriptionFactor
        ''' <summary>Effector metabolite that modulates TF activity. Concentration from ODEs.</summary>
        EffectorMetabolite
    End Enum
End Namespace