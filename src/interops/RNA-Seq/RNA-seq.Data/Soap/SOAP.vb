Namespace SOAP

    Public Class SOAPFile

    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Example line:
    ''' 
    ''' ```
    ''' SIMU_0001_00000370/2	ACGTTAACGTTGAGCCAGGCTGGCATGCACGGAAC	hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh	1	b	35	-	refseq	73466	2	C->31G40	G->20T40
    ''' ```
    ''' </remarks>
    Public Class Read
        Implements IPolymerSequenceModel

        Public Property Reads As String
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData
        Public Property Quality As String
        '1
        'b
        '35
        Public Property Strand As String
        Public Property RefSeq As String
        Public Property Start As Integer
        '0
        'C->31G40	
        'G->20T40

    End Class
End Namespace