Imports System.Runtime.CompilerServices
Imports System.Text
Imports Flute.Http.Core.HttpStream
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.Default

Namespace Core.Message

    Public Class HttpPOSTRequest : Inherits HttpRequest

        Public ReadOnly Property POSTData As PostReader

        Default Public Overrides ReadOnly Property Argument(name As String) As DefaultString
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                If URL.query.ContainsKey(name) Then
                    Return New DefaultString(URL.query(name).ElementAtOrNull(Scan0))
                Else
                    Return New DefaultString(POSTData.Form(name))
                End If
            End Get
        End Property

        Shared ReadOnly uploadfile As [Default](Of String) = NameOf(uploadfile)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="request"></param>
        ''' <param name="inputData">一个临时文件的文件路径,POST上传的原始数据都被保存在这个临时文件中</param>
        Sub New(request As HttpProcessor, inputData$)
            Call MyBase.New(request)

            If inputData.FileLength > 0 AndAlso HttpHeaders.ContainsKey(HttpHeader.RequestHeaders.ContentType) Then
                POSTData = New PostReader(
                    inputData,
                    HttpHeaders(HttpHeader.RequestHeaders.ContentType),
                    Encoding.UTF8,
                    HttpHeaders.TryGetValue("fileName") Or uploadfile
                )
            Else
                POSTData = New PostReader(
                    input:=inputData,
                    contentType:="application/octet-stream",
                    encoding:=Encoding.ASCII,
                    fileName:=HttpHeaders.TryGetValue("fileName") Or uploadfile
                )
            End If
        End Sub

        Public Overrides Function GetBoolean(name As String) As Boolean
            If HasValue(name) Then
                Return Argument(name).DefaultValue.ParseBoolean
            Else
                Return False
            End If
        End Function

        Public Overrides Function HasValue(name As String) As Boolean
            If Not URL.query.ContainsKey(name) Then
                Return POSTData.Form.ContainsKey(name)
            Else
                Return True
            End If
        End Function
    End Class
End Namespace