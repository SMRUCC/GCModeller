Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Text

Namespace HTML

    ''' <summary>
    ''' A html template file handler
    ''' </summary>
    Public Class TemplateHandler

        Public ReadOnly Property Path As String
        Public ReadOnly Property Builder As ScriptBuilder

        Sub New(file As String)
            ' 可能在将报告写入硬盘文件之前，文件系统的上下文已经变了
            ' 所以需要在这里获取得到全路径
            Path = file.GetFullPath
            Builder = New ScriptBuilder(Path.ReadAllText)
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