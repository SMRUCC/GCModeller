Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' 可以通过这个模块之中的方法来获取文本编码
''' </summary>
''' 
<[Namespace]("Text.Encodings")>
Public Module Encodings

    <ExportAPI("ASCII")>
    Public Function ASCII() As System.Text.Encoding
        Return System.Text.Encoding.ASCII
    End Function

    <ExportAPI("UTF8")>
    Public Function UTF8() As System.Text.Encoding
        Return System.Text.Encoding.UTF8
    End Function

    <ExportAPI("Unicode")>
    Public Function Unicode() As System.Text.Encoding
        Return System.Text.Encoding.Unicode
    End Function

    <ExportAPI("UTF32")>
    Public Function UTF32() As System.Text.Encoding
        Return System.Text.Encoding.UTF32
    End Function

    <ExportAPI("GB2312", Info:="Text file encoding format for chinese characters.")>
    Public Function GB2312() As System.Text.Encoding
        Return System.Text.Encoding.GetEncoding("GB2312")
    End Function

End Module
