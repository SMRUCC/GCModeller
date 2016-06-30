Imports System.Text
Imports Microsoft.VisualBasic.Linq.Extensions

''' <summary>
''' The interface of the R script model.(R脚本的数据模型对象的接口)
''' </summary>
''' <remarks></remarks>
Public MustInherit Class IRScript : Inherits IRProvider
    Implements IDisposable
    Implements IScriptProvider

    ''' <summary>
    ''' 这个函数会自动添加library包，继承类只需要复写<see cref="__r_script"/>方法即可
    ''' </summary>
    ''' <returns></returns>
    Public NotOverridable Overrides Function RScript() As String
        Dim libraries As String() = Requires.ToArray(Function(name) $"library({name})")
        Return libraries.JoinBy(vbCrLf) & vbCrLf & vbCrLf & __R_script()
    End Function

    Protected MustOverride Function __R_script() As String

    ''' <summary>
    ''' 保存脚本文件到文件系统之上
    ''' </summary>
    ''' <param name="FilePath"></param>
    ''' <returns></returns>
    Public Overridable Function SaveTo(FilePath As String) As Boolean
        If String.IsNullOrEmpty(FilePath) Then
            Return False
        Else
            Return __save(FilePath)
        End If
    End Function

    Private Function __save(path As String) As Boolean
        Return RScript.SaveTo(path, Encoding.ASCII)  ' 好像R只能够识别ASCII的脚本文件
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
