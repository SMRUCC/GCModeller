Imports System.Runtime.CompilerServices

Namespace ComponentModel

    Public MustInherit Class WebQueryModule(Of Context) : Inherits WebQuery(Of Context)

        Sub New(<CallerMemberName>
                Optional cache$ = Nothing,
                Optional interval% = -1,
                Optional offline As Boolean = False)

            Call MyBase.New(cache, interval, offline)

            Me.contextGuid = AddressOf doParseGuid
            Me.deserialization = AddressOf doParseObject
            Me.url = AddressOf doParseUrl
            Me.prefix = AddressOf contextPrefix
        End Sub

        Protected MustOverride Function doParseUrl(context As Context) As String
        Protected MustOverride Function doParseObject(html As String, schema As Type) As Object

        ''' <summary>
        ''' 生成缓存所使用的一个唯一标识符的生成函数
        ''' </summary>
        ''' <param name="context"></param>
        ''' <returns></returns>
        Protected MustOverride Function doParseGuid(context As Context) As String

        Protected Overridable Function contextPrefix(guid As String) As String
            Return ""
        End Function

    End Class
End Namespace