
''' <summary>
''' Gene regulatory network
''' </summary>
Public Class RegulatoryLink

    ''' <summary>
    ''' transcript factor protein/rna id
    ''' </summary>
    ''' <returns></returns>
    Public Property TF_id As String
    ''' <summary>
    ''' family of the TF
    ''' </summary>
    ''' <returns></returns>
    Public Property TF_family As String
    ''' <summary>
    ''' motif id of the TFBS site
    ''' </summary>
    ''' <returns></returns>
    Public Property TFBS_id As String
    ''' <summary>
    ''' effector metabolite of this TF its regulation function
    ''' </summary>
    ''' <returns></returns>
    Public Property effector As Dictionary(Of String, Effector)
    ''' <summary>
    ''' target operon id that this TF regulates
    ''' </summary>
    ''' <returns></returns>
    Public Property target_operon As String
    ''' <summary>
    ''' the operon member genes, TF regulates this operon member genes theirs transcription.
    ''' </summary>
    ''' <returns></returns>
    Public Property regulate_genes As String()

End Class

''' <summary>
''' effects of the effector to the TF protein
''' </summary>
Public Enum Effector
    Unknown
    Activator
    Inhibitor
End Enum
