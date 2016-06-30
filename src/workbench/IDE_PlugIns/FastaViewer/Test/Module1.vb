Imports FastaViewer
Imports LANS.SystemsBiology.SequenceModel.FASTA

Module Module1

    Sub Main()

        '    Dim html As String = HTMLRenderer.VisualNts(New FastaFile("D:\Xanthomonas_oryzae_oryzicola_BLS256_uid16740\CP003057.ffn"))
        ' Call html.SaveTo("./test.html")
        Call New FastaViewer.FormViwer().ShowDialog()
    End Sub
End Module
