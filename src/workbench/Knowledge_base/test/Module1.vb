Imports Microsoft.VisualBasic.Webservices.Bing
Imports Microsoft.VisualBasic.Webservices.Bing.Academic
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base

Module Module1

    Sub Main()

        '  Dim list = Academic.Search("xanthomonas")
        '  Dim info = list(1).GetDetails

        Dim info = ProfileResult.GetProfile("D:\GCModeller\src\workbench\Knowledge_base\bing\profile.html")


        '  Dim info = PubMedServicesExtensions.GetArticleInfo(22007635)

        Pause()
    End Sub
End Module
