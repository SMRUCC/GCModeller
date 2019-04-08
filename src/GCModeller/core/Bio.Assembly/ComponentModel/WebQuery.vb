Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization

Namespace ComponentModel

    Public Class WebQuery(Of Context)

        Dim url As Func(Of Context, String)
        Dim contextGuid As IToString(Of Context)
        Dim cache$
        Dim deserialization As IObjectBuilder

        Shared ReadOnly interval As Integer

        Shared Sub New()
            Static defaultInterval As DefaultValue(Of String) = "3000"

            ' controls of the interval by /@set sleep=xxxxx
            interval = Val(App.GetVariable("sleep") Or defaultInterval)
        End Sub

        Sub New(url As Func(Of Context, String),
                Optional contextGuid As IToString(Of Context) = Nothing,
                Optional parser As IObjectBuilder = Nothing,
                <CallerMemberName>
                Optional cache$ = Nothing)

            Me.url = url
            Me.cache = cache
            Me.contextGuid = contextGuid Or Scripting.ToString(Of Context)
            Me.deserialization = parser Or XmlParser
        End Sub

        ''' <summary>
        ''' 这个函数返回的是缓存的本地文件的路径列表
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="type">文件拓展名，可以不带有小数点</param>
        ''' <returns></returns>
        Protected Iterator Function queryText(query As IEnumerable(Of Context), type$) As IEnumerable(Of String)
            For Each context As Context In query
                Dim url = Me.url(context)
                Dim id$ = Me.contextGuid(context)
                Dim cache$ = $"{Me.cache}/{id}.{type.Trim("."c, "*"c)}"

                If cache.FileLength <= 0 Then
                    Call url.GET.SaveTo(cache)
                    Call Thread.Sleep(interval)
                End If

                Yield cache
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Query(Of T)(content As Context, Optional type$ = ".xml") As T
            Return deserialization(queryText({content}, type).First.ReadAllText, GetType(T))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Query(Of T)(context As IEnumerable(Of Context), Optional type$ = ".xml") As IEnumerable(Of T)
            Return queryText(context, type) _
                .Select(Function(file) deserialization(file.ReadAllText, GetType(T))) _
                .As(Of T)
        End Function
    End Class
End Namespace