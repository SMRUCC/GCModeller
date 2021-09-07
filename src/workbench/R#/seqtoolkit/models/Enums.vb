
Imports System.ComponentModel

Public Enum TableTypes
    SBH
    BBH
    ''' <summary>
    ''' blastn mapping of the short reads
    ''' </summary>
    Mapping
End Enum

Public Enum BBHAlgorithm
    Naive
    BHR
    <Description("Hybrid-BHR")>
    HybridBHR
    TaxonomySupports
End Enum
