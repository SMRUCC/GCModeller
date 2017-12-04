#Region "Microsoft.VisualBasic::5685267a9df284d6b4e7c0cdaacb53e8, ..\workbench\Knowledge_base\test\Module1.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

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

