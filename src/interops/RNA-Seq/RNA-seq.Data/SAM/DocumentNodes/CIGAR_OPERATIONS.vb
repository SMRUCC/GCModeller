Imports System.ComponentModel
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace SAM.DocumentElements

    ''' <summary>
    ''' 
    ''' </summary>
    Public Enum CIGAR_OPERATIONS As Integer
        ''' <summary>
        ''' alignment match (can be a sequence match Or mismatch)
        ''' </summary>
        M = 0
        ''' <summary>
        ''' insertion To the reference
        ''' </summary>
        I = 1
        ''' <summary>
        ''' deletion from the reference
        ''' </summary>
        D = 2
        ''' <summary>
        ''' skipped region from the reference
        ''' 
        ''' For mRNA -To -genome alignment, an N operation represents an intron. For other types Of alignments, the interpretation of N Is Not defined.
        ''' </summary>
        N = 3
        ''' <summary>
        ''' soft clipping (clipped sequences present In SEQ)
        ''' 
        ''' S may only have H operations between them And the ends Of the CIGAR String.
        ''' </summary>
        S = 4
        ''' <summary>
        ''' hard clipping (clipped sequences Not present In SEQ)
        ''' 
        ''' H can only be present As the first And/Or last operation.
        ''' </summary>
        H = 5
        ''' <summary>
        ''' padding (silent deletion from padded reference)
        ''' </summary>
        P = 6
        ''' <summary>
        ''' sequence match
        ''' </summary>
        EQ = 7
        ''' <summary>
        ''' sequence mismatch
        ''' </summary>
        X = 8
    End Enum
End Namespace