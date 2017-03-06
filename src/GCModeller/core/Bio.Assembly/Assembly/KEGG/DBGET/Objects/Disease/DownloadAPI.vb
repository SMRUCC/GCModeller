Imports System.Runtime.CompilerServices

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' 从KEGG之上下载疾病的信息
    ''' </summary>
    Public Module DownloadDiseases

        <Extension> Public Function Parse(html$) As Disease
            Dim dis As New Disease

            Return dis
        End Function
    End Module
End Namespace