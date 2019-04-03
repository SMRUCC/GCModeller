Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Text
Imports r = System.Text.RegularExpressions.Regex

Namespace HTML

    ''' <summary>
    ''' A html template file handler
    ''' </summary>
    Public Class TemplateHandler

        ''' <summary>
        ''' 模板文件的文件全路径
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Path As String
        ''' <summary>
        ''' 模板文本字符串的缓存
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Builder As ScriptBuilder

        Sub New(file As String)
            ' 可能在将报告写入硬盘文件之前，文件系统的上下文已经变了
            ' 所以需要在这里获取得到全路径
            Path = file.GetFullPath
            Builder = New ScriptBuilder(Path.ReadAllText)

            Call HtmlInterpolate()
        End Sub

        Const InterpolateRef As String = "[$]\{.+?\}"

        ''' <summary>
        ''' 在模板之中可能还会存在html碎片的插值
        ''' 在这里进行模板的html碎片的加载
        ''' </summary>
        Private Sub HtmlInterpolate()
            ' 模板的插值格式为${relpath}
            Dim relpath = r.Matches(Builder.ToString, InterpolateRef, RegexICSng).ToArray
            Dim dir As String = Path.ParentPath

            For Each refpath As String In relpath
                With refpath.GetStackValue("{", "}")
                    Call Builder.Replace(refpath, $"{dir}/{ .ByRef}".ReadAllText)
                End With
            Next
        End Sub

        ''' <summary>
        ''' Interpolated html report file save
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Flush()
            Call Builder.Save(Path, TextEncodings.UTF8WithoutBOM)
        End Sub

        Public Overrides Function ToString() As String
            Return Path
        End Function
    End Class
End Namespace