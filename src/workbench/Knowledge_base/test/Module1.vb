Imports Microsoft.VisualBasic.Webservices.Bing
Imports Microsoft.VisualBasic.Webservices.Bing.Academic
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base

Module Module1

    Sub Main()

        'Dim article = "G:\GCModeller\src\workbench\Knowledge_base\0588ab54a3fe72d189a969a73d29c7f5.xml".LoadXml(Of ArticleProfile)

        'Dim id = article.GetProfileID

        Call Academic.Build_KB("xanthomonas", "./test1111111/", pages:=20)

        Pause()


        Dim list = Academic.Search("xanthomonas")
        Dim info = list(1).GetDetails

        Dim info3333 = ProfileResult.GetProfile("https://cn.bing.com/academic/profile?id=0588ab54a3fe72d189a969a73d29c7f5&encoded=0&v=paper_preview&mkt=zh-cn")

        Call info3333.GetXml.SaveTo($"./{info.GetProfileID}.xml")


        Pause()

        Dim info2 = ProfileResult.GetProfile("D:\GCModeller\src\workbench\Knowledge_base\bing\profile.html")


        '  Dim info = PubMedServicesExtensions.GetArticleInfo(22007635)

        Pause()
    End Sub
End Module
