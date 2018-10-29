Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Language.UnixBash

Namespace HTML

    ''' <summary>
    ''' A report template directory
    ''' </summary>
    Public Class HTMLReport : Implements IDisposable

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

        ''' <summary>
        ''' html报告的根目录
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Directory As String

        Public ReadOnly Property HtmlFiles As String()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Templates _
                    .Values _
                    .Select(Function(handler) handler.Path) _
                    .ToArray
            End Get
        End Property

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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Replace(find$, value As XElement) As HTMLReport
            Return Replace(find, value.ToString)
        End Function

        Sub New(folder$, Optional searchLevel As SearchOption = SearchOption.SearchTopLevelOnly)
            Templates = (ls - l - {"*.html", "*.htm"} << searchLevel <= folder) _
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

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call Save()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace