Imports LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver.LDM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Logging
Imports Microsoft.VisualBasic

''' <summary>
''' Model file of class type <see cref="ModelBaseType"></see> compiler.
''' </summary>
''' <typeparam name="TModel"></typeparam>
''' <remarks></remarks>
Public MustInherit Class Compiler(Of TModel As ModelBaseType)
    Implements IDisposable
    Implements ISupportLoggingClient

    Protected Friend CompiledModel As TModel
    Protected Friend _Logging As Logging.LogFile

    Public Overridable ReadOnly Property Version As Version
        Get
            Return My.Application.Info.Version
        End Get
    End Property

    Public ReadOnly Property CompileLogging As LogFile Implements ISupportLoggingClient.Logging
        Get
            Return _Logging
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="argvs"><see cref="CommandLine.CommandLine.CLICommandArgvs"></see></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public MustOverride Function PreCompile(argvs As CommandLine.CommandLine) As Integer
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="argvs">
    ''' Property definition parameters for <see cref="ModelBaseType.ModelProperty"></see>, the override function of 
    ''' this mustOverride method should call method <see cref="WriteProperty"></see> to write the property into the 
    ''' compiled model file.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public MustOverride Function Compile(Optional argvs As CommandLine.CommandLine = Nothing) As TModel
    Protected MustOverride Function Link() As Integer

    Public Overridable ReadOnly Property [Return] As TModel
        Get
            Return CompiledModel
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="argvs"></param>
    ''' <param name="Model"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("-write_property", Info:="",
        Usage:="-write_property [-name <name>] [-authors <author1; author2; ...>] [-comment <shot_comment>] [-title <title>] [-emails <address1; address2; ...>] [-publications <pubmed1; pubmed2; ...>] [-urls <url1; url2; ...>]",
        Example:="")>
    Protected Function WriteProperty(argvs As CommandLine.CommandLine, Model As TModel) As TModel
        Call _Logging.WriteLine(vbCrLf & "Write model property into the compiled model file.")

        If Model.ModelProperty Is Nothing Then _
           Model.ModelProperty = New [Property]

        If String.IsNullOrEmpty(Model.ModelProperty.GUID) Then Model.ModelProperty.GUID = Guid.NewGuid.ToString
        If String.IsNullOrEmpty(Model.ModelProperty.CompiledDate) Then Model.ModelProperty.CompiledDate = Now.ToString
        If Model.ModelProperty.Reversion = 0 Then Model.ModelProperty.Reversion = 1
        If Model.ModelProperty.URLs.IsNullOrEmpty Then Model.ModelProperty.URLs = New List(Of String) From {"http://code.google.com/p/genome-in-code/"} Else Call Model.ModelProperty.URLs.Add("http://code.google.com/p/genome-in-code/")
        If Model.ModelProperty.Authors.IsNullOrEmpty Then Model.ModelProperty.Authors = New List(Of String) From {"LANS.SystemsBiology.GCModeller"}

        If Not argvs Is Nothing Then  '请先使用If判断是否为空，因为不知道本方法的调用顺序，不使用if判断可能会丢失已经在调用之前就写入的属性数据

        End If

        Return Model
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("ICompiler: {0}, version: {1}", GetType(Compiler(Of TModel)).FullName, Version.ToString)
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 检测冗余的调用

    ' IDisposable
    Protected Overridable Sub Dispose(Disposing As Boolean)
        If Not Me.disposedValue Then
            If Disposing Then
                ' TODO:  释放托管状态(托管对象)。
                Call WriteLog()
            End If

            ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
            ' TODO:  将大型字段设置为 null。
        End If
        Me.disposedValue = True
    End Sub

    ' TODO:  仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
    'Protected Overrides Sub Finalize()
    '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose( disposing As Boolean)中。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Visual Basic 添加此代码是为了正确实现可处置模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    Public Function WriteLog() As Boolean Implements ISupportLoggingClient.WriteLog
        Call _Logging.Save()
        Return True
    End Function
End Class
