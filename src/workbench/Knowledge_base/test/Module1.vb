Imports Microsoft.VisualBasic.Webservices.Bing
Imports Microsoft.VisualBasic.Webservices.Bing.Academic
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base

Module Module1

    Sub Main()

        Dim list = Academic.Search("xanthomonas")
        Dim info = List(1).GetDetails

        Call info.GetXml.SaveTo("./testssssss.xml")


        Pause()

        Dim info2 = ProfileResult.GetProfile("D:\GCModeller\src\workbench\Knowledge_base\bing\profile.html")


        '  Dim info = PubMedServicesExtensions.GetArticleInfo(22007635)

        Pause()
    End Sub
End Module
