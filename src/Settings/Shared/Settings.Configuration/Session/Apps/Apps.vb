''' <summary>
''' A list of GCModeller apps 
''' </summary>
Public NotInheritable Class Apps

    Public Shared ReadOnly Property NCBI_tools As GCModellerApps.NCBI_tools
    Public Shared ReadOnly Property localblast As GCModellerApps.localblast
    Public Shared ReadOnly Property MEME As GCModellerApps.meme
    Public Shared ReadOnly Property KEGG_tools As String
    Public Shared ReadOnly Property seqtools As GCModellerApps.seqtools
    Public Shared ReadOnly Property VennDiagram As GCModellerApps.VennDiagram

    Shared Sub New()
        NCBI_tools = New GCModellerApps.NCBI_tools(App.HOME & "/NCBI_tools.exe")
        localblast = New GCModellerApps.localblast(App.HOME & "/localblast.exe")
        MEME = New GCModellerApps.meme(App.HOME & "/MEME.exe")
        KEGG_tools = App.HOME & "/KEGG_tools.exe"
        seqtools = New GCModellerApps.seqtools(App.HOME & $"/{NameOf(seqtools)}.exe")
        VennDiagram = New GCModellerApps.VennDiagram(App.HOME & "/venn.exe")
    End Sub
End Class
