Imports Microsoft.VisualBasic.Webservices.Bing
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base

Module Module1

    Sub Main()

        Dim list = Academic.Search("ZEBRAFISH")

        Dim info = PubMedServicesExtensions.GetArticleInfo(22007635)

        Pause()
    End Sub
End Module
