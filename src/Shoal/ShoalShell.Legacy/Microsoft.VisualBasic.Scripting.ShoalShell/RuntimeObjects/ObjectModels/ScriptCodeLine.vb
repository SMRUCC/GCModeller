Namespace Runtime.Objects.ObjectModels

    ''' <summary>
    ''' The <see cref="ScriptCodeLine"></see> is the basically element of the shoal script.(脚本命令行的数据模型)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ScriptCodeLine : Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.IAccessionIdEnumerable
        Implements Microsoft.VisualBasic.ComponentModel.IAddrHandle

        ''' <summary>
        ''' The target varialbe for the value assignment, if this codeline is a method calling then the variable will be set to the system conserved variable $
        ''' </summary>
        ''' <remarks></remarks>
        Public Property VariableAssigned As String
        ''' <summary>
        ''' The Goto flag property can use for identified each script code line uniquely.(每一行代码都会有一个Goto标号进行唯一标识的)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LineFlag As String Implements ComponentModel.Collection.Generic.IAccessionIdEnumerable.UniqueId
        Public Property LineNumber As Long Implements ComponentModel.IAddrHandle.AddrHwnd
        Public InvokeMethod As Func(Of Object)
        Public Property GotoLineFlag As String
        Public GotoCondition As Func(Of Boolean)
        Public Property OrignialScriptLine As String

        ''' <summary>
        ''' If the flag value is TRUE then if the error occur, and the script host is still trying to ignore the error and continute running the script file.
        ''' </summary>
        ''' <remarks></remarks>
        Public Property OnErrorResumeNext As Boolean
        ''' <summary>
        ''' 假若当前的命令行对象为拓展方法的话，则本属性不应该为空
        ''' </summary>
        ''' <remarks></remarks>
        Public Property ExtensionMethodVariable As String
        Public Property ReturnType As String

        Public ReadOnly Property IsGotoStatement As Boolean
            Get
                Return Not String.IsNullOrEmpty(GotoLineFlag)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1}", LineNumber, OrignialScriptLine)
        End Function

        Public Const INTERNAL_EXPRESSION_REGEX As String = "$(.+?)"

        Public Property PreExecuteType As PreExecuteTypes = PreExecuteTypes.Normal

        Public Enum PreExecuteTypes
            Normal
            [Imports]
            Library
        End Enum

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

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
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
End Namespace