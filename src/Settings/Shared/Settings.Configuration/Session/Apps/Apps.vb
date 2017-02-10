''' <summary>
''' A list of GCModeller apps 
''' </summary>
Public NotInheritable Class Apps

    Public Shared ReadOnly Property NCBI_tools As String
    Public Shared ReadOnly Property localblast As String
    Public Shared ReadOnly Property MEME As String
    Public Shared ReadOnly Property KEGG_tools As String
    Public Shared ReadOnly Property seqtools As String
    Public Shared ReadOnly Property VennDiagram As GCModellerApps.VennDiagram

    Shared Sub New()
        NCBI_tools = App.HOME & "/NCBI_tools.exe"
        localblast = App.HOME & "/localblast.exe"
        MEME = App.HOME & "/MEME.exe"
        KEGG_tools = App.HOME & "/KEGG_tools.exe"
        seqtools = App.HOME & $"/{NameOf(seqtools)}.exe"
        VennDiagram = New GCModellerApps.VennDiagram(App.HOME & "/venn.exe")
    End Sub
End Class
