Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call NetworkGenerator.LoadJson("G:\1.13.RegPrecise_network\Cellular Phenotypes\KEGG_Modules-SimpleModsNET").SaveTo("F:\GCModeller.Workbench\d3js\Force-Directed Graph\test.json")

        Call NetworkGenerator.LoadJson("G:\4.15\MEME\footprints\xcb-modules.TestFootprints2,2.csv").SaveTo("G:\4.15\MEME\footprints\xcb-modules.TestFootprints2,2.json")
        Call NetworkGenerator.LoadJson("G:\4.15\MEME\footprints\xcb-pathways.TestFootprints2,2.csv").SaveTo("G:\4.15\MEME\footprints\xcb-pathways.TestFootprints2,2.json")

    End Sub
End Class
