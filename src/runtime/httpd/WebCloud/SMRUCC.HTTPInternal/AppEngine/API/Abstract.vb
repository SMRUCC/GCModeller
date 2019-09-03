Imports SMRUCC.WebCloud.HTTPInternal.Core

Namespace AppEngine.APIMethods

    ''' <summary>
    ''' WebApp API的抽象接口
    ''' </summary>
    ''' <param name="api">URL</param>
    ''' <param name="request">URL后面的参数请求</param>
    ''' <param name="response">返回的html页面的文档</param>
    ''' <returns>是否执行成功</returns>
    Public Delegate Function APIAbstract(api As String, request As HttpRequest, response As HttpResponse) As Boolean

    ''' <summary>
    ''' <see cref="[GET]"/> API interface
    ''' </summary>
    ''' <param name="request">url arguments</param>
    ''' <param name="response">output json or html page</param>
    ''' <returns>Execute success or not?</returns>
    Public Delegate Function _GET(request As HttpRequest, response As HttpResponse) As Boolean

    ''' <summary>
    ''' <see cref="POST"/> API interface
    ''' </summary>
    ''' <param name="request">url arguments and Form data</param>
    ''' <param name="response"></param>
    ''' <returns>Execute success or not?</returns>
    Public Delegate Function _POST(request As HttpPOSTRequest, response As HttpResponse) As Boolean
End Namespace