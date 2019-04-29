Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization

Namespace ComponentModel

    Public Class WebQuery(Of Context)

        Dim url As Func(Of Context, String)
        Dim contextGuid As IToString(Of Context)
        Dim deserialization As IObjectBuilder
        Dim sleepInterval As Integer
        Dim prefix As Func(Of String, String)

        ''' <summary>
        ''' 原始请求结果数据的缓存文件夹,同时也可以用这个文件夹来存放错误日志
        ''' </summary>
        Protected cache$

        Shared ReadOnly interval As DefaultValue(Of Integer)

        Shared Sub New()
            Static defaultInterval As DefaultValue(Of String) = "3000"

            With Val(App.GetVariable("sleep") Or defaultInterval)
                ' controls of the interval by /@set sleep=xxxxx
                interval = CInt(.ByRef).AsDefault(Function(x) x <= 0)
            End With

            ' display debug info
            Call $"WebQuery download worker thread sleep interval is {interval}ms".__INFO_ECHO
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="url"></param>
        ''' <param name="contextGuid"></param>
        ''' <param name="parser"></param>
        ''' <param name="prefix">
        ''' 如果查询的结果文件很多, 则缓存放在同一下文件夹下, 打开的效率会非常低,
        ''' 在这里使用这个函数来得到分组前缀用作为文件夹名,分组存放缓存数据
        ''' </param>
        ''' <param name="cache$"></param>
        ''' <param name="interval%"></param>
        Sub New(url As Func(Of Context, String),
                Optional contextGuid As IToString(Of Context) = Nothing,
                Optional parser As IObjectBuilder = Nothing,
                Optional prefix As Func(Of String, String) = Nothing,
                <CallerMemberName>
                Optional cache$ = Nothing,
                Optional interval% = -1)

            Me.url = url
            Me.cache = cache
            Me.contextGuid = contextGuid Or Scripting.ToString(Of Context)
            Me.deserialization = parser Or XmlParser
            Me.sleepInterval = interval Or WebQuery(Of Context).interval
            Me.prefix = prefix
        End Sub

        ''' <summary>
        ''' 这个函数返回的是缓存的本地文件的路径列表
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="type">文件拓展名，可以不带有小数点</param>
        ''' <returns></returns>
        Protected Iterator Function queryText(query As IEnumerable(Of Context), type$) As IEnumerable(Of String)
            ' 因为在这里是进行批量的数据库查询
            ' 所以在这个函数内的代码的执行效率不会被考虑在内

            For Each context As Context In query
                Dim url = Me.url(context)
                Dim id$ = Me.contextGuid(context)
                Dim cache$

                If prefix Is Nothing Then
                    cache = $"{Me.cache}/{id}.{type.Trim("."c, "*"c)}"
                Else
                    cache = $"{Me.cache}/{prefix(id)}/{id}.{type.Trim("."c, "*"c)}"
                End If

                If cache.FileLength <= 0 Then
                    Call url.GET.SaveTo(cache)
                    Call $"Worker thread sleep {interval}ms...".__INFO_ECHO
                    Call Thread.Sleep(interval)
                Else
                    Call "hit cache!".__DEBUG_ECHO
                End If

                Yield cache
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="context"></param>
        ''' <param name="cacheType">缓存文件的文本格式拓展名</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Query(Of T)(context As Context, Optional cacheType$ = ".xml") As T
            Return deserialization(queryText({context}, cacheType).First.ReadAllText, GetType(T))
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="context"></param>
        ''' <param name="cacheType">缓存文件的文本格式拓展名</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Query(Of T)(context As IEnumerable(Of Context), Optional cacheType$ = ".xml") As IEnumerable(Of T)
            Return queryText(context, cacheType) _
                .Select(Function(file) deserialization(file.ReadAllText, GetType(T))) _
                .As(Of T)
        End Function
    End Class
End Namespace