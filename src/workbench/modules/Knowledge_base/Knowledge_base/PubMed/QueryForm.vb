#Region "Microsoft.VisualBasic::325834bb0b869ac77398236d000b2bf9, modules\Knowledge_base\Knowledge_base\PubMed\QueryForm.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 87
    '    Code Lines: 4
    ' Comment Lines: 81
    '   Blank Lines: 2
    '     File Size: 12.94 KB


    '     Class QueryForm
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace PubMed

    Public Class QueryForm
        ' curl 'https://www.ncbi.nlm.nih.gov/pubmed' -H 'authority: www.ncbi.nlm.nih.gov' -H 'pragma: no-cache' -H 'cache-control: no-cache' -H 'upgrade-insecure-requests: 1' -H 'user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3724.8 Safari/537.36' -H 'origin: https://www.ncbi.nlm.nih.gov' -H 'content-type: application/x-www-form-urlencoded' -H 'accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3' -H 'referer: https://www.ncbi.nlm.nih.gov/pubmed/?term=microRNA' -H 'accept-encoding: gzip, deflate, br' -H 'accept-language: en-US,en;q=0.9,zh-CN;q=0.8,zh;q=0.7' -H 'cookie: _ga=GA1.2.457891432.1550472545; ncbi_sid=BD72304A3975D6A1_79EDSID; _ga=GA1.4.457891432.1550472545; _ga=GA1.3.457891432.1550472545; _gid=GA1.2.1468903296.1567045758; entrezSort=pubmed:Title; _gat_ncbiSg=1; _gat_dap=1; ncbi_pinger=N4IgDgTgpgbg+mAFgSwCYgFwgMIFEAcAnAOwDMAQgIIAiAbMQCwBi5+AjAAxffeH64cArACZSAOgC2cNoRAAaEAGMANskUBrAHZQAHgBdMoDphDJNq7XFUBnPXOVQAZnoAEqZNbDKAhgE841lB6emYA5tZy0NYArsp61gFBIZrhcooA9pp6UFlp6cpygTA5cBnK8iBsJlGxenDeiiGZcABG3hByEt5mGVk5BgqkslgMpOwVDMZYgsQcwhMMJipqWroDIAyCJsYKDLQmjt7KgRPEJoIcsrv4JmzCOxvDIABWyKEAtJqKLchg6WBgKAQCoXEyIYJgawYAD00IA7gixF8fkjlBIkchEGJQukYNCwNEWhIoKhoQB+bIQCQAXgkagg6QASgA5SgAYhBVSw1A8Xj8IPmWCY6Sp3j0GBcAGVohIuhBfCAAL4KaLmdLeVDafSGECkUgHI4nQaLLB6CDRKAVMYmK1PfAMYhXDZTEB8LgLEyq5TqzVrCZbLBVBSCfZYB4zbYVWhckD4QSyZVKdKyzJagwYUCCkD9aAALytJgJRJJExMqHSihiEhBNoUoZA5crMoqZyw4Ik5QUNyw9wqTxCegcFTYMaDlX1WEIHHwHDOCjYJpAgXaikQw/rR07lVbIEOx0t8+7u8NB5AvaweCIZCodEYLHYPEffAEInEUhkFWEMZzUFzGCLxKoBgjZVhgzIAPLMrgn5ZgicJIt8yCouimiYtiuKfk8bC0O6gwulODykKOOGEVm+A4RME4gBwYjCIIYhjoIVHYbhICCIuxBEEqipAA==; WebEnv=1ZmYQ5eTvoApWcDeum3VsfIqFyCN8ipoZxCy0VlwT-E58okZUyAHBzo-4H6AviATGKJNuQZGgMCMRcOEzxbap7dnWIhWuuY36U6kX%40BD72304A3975D6A1_79EDSID' --data 'term=microRNA+&EntrezSystem2.PEntrez.PubMed.Pubmed_PageController.PreviousPageName=results&EntrezSystem2.PEntrez.PubMed.Pubmed_PageController.SpecialPageName=&EntrezSystem2.PEntrez.PubMed.Pubmed_Facets.FacetsUrlFrag=filters%3D&EntrezSystem2.PEntrez.PubMed.Pubmed_Facets.FacetSubmitted=false&EntrezSystem2.PEntrez.PubMed.Pubmed_Facets.BMFacets=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.sPresentation=xml&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.sSort=Title&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.sPageSize=20&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.FFormat=docsum&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.FSort=Title&email_format=docsum&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.email_sort=Title&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.email_count=20&email_start=1&email_address=&email_subj=microRNA+-+PubMed&email_add_text=&EmailCheck1=&EmailCheck2=&coll_start=1&BibliographyUser=&BibliographyUserName=my&citman_count=20&citman_start=1&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.FileFormat=docsum&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.LastPresentation=docsum&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.Presentation=xml&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.PageSize=20&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.LastPageSize=20&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.Sort=Title&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.LastSort=Title&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.FileSort=Title&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.Format=text&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.LastFormat=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.PrevPageSize=20&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.PrevPresentation=docsum&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.PrevSort=Title&CollectionStartIndex=1&CitationManagerStartIndex=1&CitationManagerCustomRange=false&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_ResultsController.ResultCount=90807&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_ResultsController.RunLastQuery=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_Pager.cPage=1&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_Pager.CurrPage=1&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_Pager.cPage=1&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailReport=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailFormat=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailCount=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailStart=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailSort=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.Email=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailSubject=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailText=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailQueryKey=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailHID=1HYSIMMiOonWucw78ychkStxocw0l7mI5ibSi9bQ6ldEiyisal7Jmylz6lBQMKHs0JMb42DaUFGC_QAcZRvY5pGQZPXD4301yf&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.QueryDescription=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.Key=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.Answer=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.Holding=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.HoldingFft=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.HoldingNdiSet=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.OToolValue=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.SubjectList=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.TimelineAdPlaceHolder.CurrTimelineYear=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.TimelineAdPlaceHolder.BlobID=NCID_1_140496281_130.14.18.48_9001_1567055714_288602440_0MetA0_S_MegaStore_F_1&EntrezSystem2.PEntrez.DbConnector.Db=pubmed&EntrezSystem2.PEntrez.DbConnector.LastDb=pubmed&EntrezSystem2.PEntrez.DbConnector.Term=microRNA&EntrezSystem2.PEntrez.DbConnector.LastTabCmd=&EntrezSystem2.PEntrez.DbConnector.LastQueryKey=10&EntrezSystem2.PEntrez.DbConnector.IdsFromResult=&EntrezSystem2.PEntrez.DbConnector.LastIdsFromResult=&EntrezSystem2.PEntrez.DbConnector.LinkName=&EntrezSystem2.PEntrez.DbConnector.LinkReadableName=&EntrezSystem2.PEntrez.DbConnector.LinkSrcDb=&EntrezSystem2.PEntrez.DbConnector.Cmd=displaychanged&EntrezSystem2.PEntrez.DbConnector.TabCmd=&EntrezSystem2.PEntrez.DbConnector.QueryKey=&p%24a=EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.sPresentation&p%24l=EntrezSystem2&p%24st=pubmed' --compressed

        ' term=microRNA+&
        'EntrezSystem2.PEntrez.PubMed.Pubmed_PageController.PreviousPageName=results&
        'EntrezSystem2.PEntrez.PubMed.Pubmed_PageController.SpecialPageName=&
        'EntrezSystem2.PEntrez.PubMed.Pubmed_Facets.FacetsUrlFrag=filters%3D&
        'EntrezSystem2.PEntrez.PubMed.Pubmed_Facets.FacetSubmitted=false&
        'EntrezSystem2.PEntrez.PubMed.Pubmed_Facets.BMFacets=&
        'EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.sPresentation=xml&
        'EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.sSort=Title&
        'EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.sPageSize=20&
        'EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.FFormat=docsum&
        'EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.FSort=Title&
        'email_format=docsum&
        'EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.email_sort=Title&
        'EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.email_count=20&
        'email_start=1&
        'email_address=&
        'email_subj=microRNA+-+PubMed&
        'email_add_text=&
        'EmailCheck1=&
        'EmailCheck2=&
        'coll_start=1&
        'BibliographyUser=&
        'BibliographyUserName=my&
        'citman_count=20&
        'citman_start=1&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.FileFormat=docsum&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.LastPresentation=docsum&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.Presentation=xml&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.PageSize=20&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.LastPageSize=20&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.Sort=Title&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.LastSort=Title&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.FileSort=Title&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.Format=text&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.LastFormat=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.PrevPageSize=20&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.PrevPresentation=docsum&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.PrevSort=Title&
        ' CollectionStartIndex=1&
        ' CitationManagerStartIndex=1&
        ' CitationManagerCustomRange=false&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_ResultsController.ResultCount=90807&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_ResultsController.RunLastQuery=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_Pager.cPage=1&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_Pager.CurrPage=1&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_Pager.cPage=1&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailReport=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailFormat=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailCount=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailStart=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailSort=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.Email=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailSubject=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailText=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailQueryKey=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.EmailHID=1HYSIMMiOonWucw78ychkStxocw0l7mI5ibSi9bQ6ldEiyisal7Jmylz6lBQMKHs0JMb42DaUFGC_QAcZRvY5pGQZPXD4301yf&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.QueryDescription=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.Key=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.Answer=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.Holding=&EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.HoldingFft=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.HoldingNdiSet=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.OToolValue=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.EmailTab.SubjectList=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.TimelineAdPlaceHolder.CurrTimelineYear=&
        ' EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.TimelineAdPlaceHolder.BlobID=NCID_1_140496281_130.14.18.48_9001_1567055714_288602440_0MetA0_S_MegaStore_F_1&
        ' EntrezSystem2.PEntrez.DbConnector.Db=pubmed&
        ' EntrezSystem2.PEntrez.DbConnector.LastDb=pubmed&
        ' EntrezSystem2.PEntrez.DbConnector.Term=microRNA&
        ' EntrezSystem2.PEntrez.DbConnector.LastTabCmd=&
        ' EntrezSystem2.PEntrez.DbConnector.LastQueryKey=10&
        ' EntrezSystem2.PEntrez.DbConnector.IdsFromResult=&
        ' EntrezSystem2.PEntrez.DbConnector.LastIdsFromResult=&
        ' EntrezSystem2.PEntrez.DbConnector.LinkName=&
        ' EntrezSystem2.PEntrez.DbConnector.LinkReadableName=&
        ' EntrezSystem2.PEntrez.DbConnector.LinkSrcDb=&
        ' EntrezSystem2.PEntrez.DbConnector.Cmd=displaychanged&
        ' EntrezSystem2.PEntrez.DbConnector.TabCmd=&
        ' EntrezSystem2.PEntrez.DbConnector.QueryKey=&
        ' p%24a=EntrezSystem2.PEntrez.PubMed.Pubmed_ResultsPanel.Pubmed_DisplayBar.sPresentation&
        ' p%24l=EntrezSystem2&
        ' p%24st=pubmed
    End Class
End Namespace
