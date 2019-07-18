Namespace Assembly.KEGG.DBGET.BriteHEntry

    Public Class EnzymeEntry

        ''' <summary>
        ''' 从卫星资源程序集之中加载数据库数据
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Load resource using <see cref="ResourcesSatellite"/></remarks>
        Public Shared Function GetResource() As htext
            Dim res$ = GetType(EnzymeEntry) _
                .Assembly _
                .ResourcesSatellite() _
                .GetString("ko01000")
            Dim htext As htext = htext.StreamParser(res)
            Return htext
        End Function
    End Class
End Namespace