Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text
Imports System.Runtime.CompilerServices

Public Class HTMLReport

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 这个主要是为了兼容PC端的HTML报告和移动端的HTML报告所设置了
    ''' 因为PC端的HTML报告可能就只有一个index.html
    ''' 但是对于移动端，由于设备屏幕比较小以及为了方便在内容分区之间跳转，所以HTML报告往往会被按照内容分区分为多个html文档构成的
    ''' </remarks>
    Public ReadOnly Property Templates As Dictionary(Of String, TemplateHandler)
    Public ReadOnly Property Directory As String

    Default Public WriteOnly Property Assign(name As String) As String
        Set(value As String)
            For Each template In Templates.Values
                template.Builder.Assign(name) = value
            Next
        End Set
    End Property

    Public Function Replace(find$, value$) As HTMLReport
        For Each template In Templates.Values
            Call template.Builder.Replace(find, value)
        Next

        Return Me
    End Function

    Sub New(folder As String)
        Templates = (ls - l - r - {"*.html", "*.htm"} <= folder) _
            .ToDictionary(Function(path) path.BaseName,
                          Function(path)
                              Return New TemplateHandler(path)
                          End Function)
        Directory = folder.GetDirectoryFullPath
    End Sub

    Public Overrides Function ToString() As String
        Return Directory
    End Function

    Public Sub Save()
        For Each template In Templates.Values
            Call template.Flush()
        Next
    End Sub
End Class

Public Class TemplateHandler

    Public ReadOnly Property Path As String
    Public ReadOnly Property Builder As ScriptBuilder

    Sub New(file As String)
        ' 可能在将报告写入硬盘文件之前，文件系统的上下文已经变了
        ' 所以需要在这里获取得到全路径
        Path = file.GetFullPath
        Builder = New ScriptBuilder(Path.ReadAllText)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Flush()
        Call Builder.Save(Path, TextEncodings.UTF8WithoutBOM)
    End Sub

    Public Overrides Function ToString() As String
        Return Path
    End Function
End Class
