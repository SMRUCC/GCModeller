
<AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)>
Public Class out : Inherits Attribute

    ''' <summary>
    ''' 从R变量之中解析出一个.NET对象
    ''' </summary>
    ''' <param name="var">R变量名称</param>
    ''' <returns>.NET对象</returns>
    Public Delegate Function RObjectParser(var As String) As Object

    Public ReadOnly Property Parser As RObjectParser

    Sub New(func As RObjectParser)
        Parser = func
    End Sub

    Public Overrides Function ToString() As String
        Return Parser.ToString
    End Function
End Class
