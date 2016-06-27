Imports System.ComponentModel

Namespace Assembly.NCBI.COG

    Public Enum COGCategories As Integer

        ''' <summary>
        ''' No function category assigned to this gene
        ''' </summary>
        <Description("NOT ASSIGNED")> NotAssigned = -100
        ''' <summary>
        ''' INFORMATION STORAGE AND PROCESSING
        ''' </summary>
        <Description("INFORMATION STORAGE AND PROCESSING")> Genetics = 2
        ''' <summary>
        ''' CELLULAR PROCESSES AND SIGNALING
        ''' </summary>
        <Description("CELLULAR PROCESSES AND SIGNALING")> Signaling = 4
        ''' <summary>
        ''' METABOLISM
        ''' </summary>
        <Description("METABOLISM")> Metabolism = 8
        ''' <summary>
        ''' POORLY CHARACTERIZED
        ''' </summary>
        <Description("POORLY CHARACTERIZED")> Unclassified = 16
    End Enum

End Namespace