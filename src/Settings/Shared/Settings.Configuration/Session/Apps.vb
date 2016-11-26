''' <summary>
''' A list of GCModeller apps 
''' </summary>
Public Module Apps

    Public ReadOnly Property NCBI_tools As String
    Public ReadOnly Property localblast As String
    Public ReadOnly Property MEME As String
    Public ReadOnly Property KEGG_tools As String
    Public ReadOnly Property seqtools As String

    Sub New()
        NCBI_tools = App.HOME & "/NCBI_tools.exe"
        localblast = App.HOME & "/localblast.exe"
        MEME = App.HOME & "/MEME.exe"
        KEGG_tools = App.HOME & "/KEGG_tools.exe"
        seqtools = App.HOME & $"/{NameOf(seqtools)}.exe"
    End Sub
End Module
