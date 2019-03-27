Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization

Namespace ComponentModel

    Public Class WebQuery(Of Context)

        Dim url As Func(Of Context, String)
        Dim contextGuid As IToString(Of Context)
        Dim cache$
        Dim deserialization As IObjectBuilder

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

        Private Iterator Function queryText(query As IEnumerable(Of Context), type$) As IEnumerable(Of String)
            For Each context As Context In query
                Dim url = Me.url(context)
                Dim id$ = Me.contextGuid(context)
                Dim cache$ = $"{Me.cache}/{id}{type}"

                If cache.FileLength <= 0 Then
                    Call url.GET.SaveTo(cache)
                    Call Thread.Sleep(2000)
                End If

                Yield cache
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Query(Of T)(context As IEnumerable(Of Context), Optional type$ = ".xml") As IEnumerable(Of T)
            Return queryText(context, type) _
                .Select(Function(file) deserialization(file.ReadAllText, GetType(T))) _
                .As(Of T)
        End Function
    End Class
End Namespace