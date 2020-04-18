''' <summary>
''' Protein Modification
''' 
''' ``{polypeptide} + compounds -> protein``
''' </summary>
Public Structure Protein

    Dim polypeptides As String()
    Dim compounds As String()

    Public Property ProteinID As String

    ''' <summary>
    ''' 这个蛋白质是由一条多肽链所构成的
    ''' </summary>
    ''' <param name="proteinId"></param>
    Sub New(proteinId As String)
        polypeptides = {proteinId}
    End Sub

End Structure