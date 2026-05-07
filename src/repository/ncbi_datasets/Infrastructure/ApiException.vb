' ============================================================================
' ApiException.vb
' API 异常类 - 封装 REST API 调用中的错误信息
' ============================================================================

Imports System
Imports System.Net

Namespace ncbi_datasets.Infrastructure

    ''' <summary>
    ''' 表示 REST API 调用过程中发生的异常。
    ''' 包含 HTTP 状态码和错误响应内容，便于调用方进行错误处理。
    ''' </summary>
    Public Class ApiException
        Inherits Exception

        ''' <summary>
        ''' HTTP 状态码
        ''' </summary>
        Public ReadOnly Property StatusCode As HttpStatusCode

        ''' <summary>
        ''' 创建 ApiException 实例。
        ''' </summary>
        ''' <param name="message">错误消息</param>
        ''' <param name="statusCode">HTTP 状态码</param>
        Public Sub New(message As String, statusCode As HttpStatusCode)
            MyBase.New(message)
            Me.StatusCode = statusCode
        End Sub

        ''' <summary>
        ''' 创建 ApiException 实例（含内部异常）。
        ''' </summary>
        Public Sub New(message As String, statusCode As HttpStatusCode, innerException As Exception)
            MyBase.New(message, innerException)
            Me.StatusCode = statusCode
        End Sub

        ''' <summary>
        ''' 返回异常的字符串表示，包含状态码信息。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return "[ApiException] StatusCode=" & CInt(StatusCode).ToString() & " (" & StatusCode.ToString() & ") - " & Message
        End Function

    End Class

End Namespace
