Imports System.CodeDom
Imports System.CodeDom.Compiler
Imports System.Reflection
Imports System.Text
Imports Microsoft.VisualBasic.Emit.CodeDOM_VBC
Imports Microsoft.VisualBasic.Linq.Framework.Provider
Imports Microsoft.VisualBasic.Linq.Framework.Provider.ImportsAPI
Imports Microsoft.VisualBasic.Linq.LDM.Statements

Namespace Framework.DynamicCode

    ''' <summary>
    ''' 编译整个Linq语句的动态代码编译器
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DynamicCompiler : Implements IDisposable

        Public ReadOnly Property EntityProvider As TypeRegistry
        Public ReadOnly Property ApiProvider As APIProvider

        Sub New(entity As TypeRegistry, api As APIProvider)
            ApiProvider = api
            EntityProvider = entity

            Call ReferenceList.Add(GetType(DynamicCompiler).Assembly.Location)
            Call ReferenceList.Add(GetType(Microsoft.VisualBasic.App).Assembly.Location)
            Call ImportsNamespace.Add("System.Linq.Enumerable")
            Call ImportsNamespace.Add("System")
            Call ImportsNamespace.Add("System.Text")
            Call ImportsNamespace.Add("Microsoft.VisualBasic")
            Call ImportsNamespace.Add("Microsoft.VisualBasic.Linq")
        End Sub

        Sub New()
            Call Me.New(TypeRegistry.LoadDefault, APIProvider.LoadDefault)
        End Sub

        Public ReadOnly Property ImportsNamespace As List(Of String) = New List(Of String)
        Public ReadOnly Property ReferenceList As New List(Of String)

        ''' <summary>
        ''' 使用Linq的注册表数据创建一个默认的编译器
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function DefaultCompiler() As DynamicCompiler
            Dim DIR As String = App.GetProductSharedDIR(GetType(DynamicCompiler))
            Dim types As String = DIR & "/" & FileIO.FileSystem.GetFileInfo(TypeRegistry.DefaultFile).Name
            Dim api As String = DIR & "/" & FileIO.FileSystem.GetFileInfo(APIProvider.DefaultFile).Name
            Return New DynamicCompiler(TypeRegistry.Load(types), APIProvider.Load(api))
        End Function

        Public Sub [Imports](ns As String)
            Dim types As Type() = ApiProvider.GetType(ns)
            For Each nsDef As Type In types
                Dim name As String = nsDef.FullName
                If Not ImportsNamespace.Contains(name) Then
                    Call ImportsNamespace.Add(name)
                End If
                Dim assm As String = nsDef.Assembly.Location
                If Not ReferenceList.Contains(assm) Then
                    Call ReferenceList.Add(assm)
                End If
            Next
        End Sub

        ''' <summary>
        ''' 编译Linq代码
        ''' </summary>
        ''' <param name="LinqClosure"></param>
        ''' <returns></returns>
        Public Function Compile(LinqClosure As String, Optional ByRef err As String = "") As IProject
            Dim lstImports = ImportsNamespace.ToArray(Function(ns) "Imports " & ns)
            Dim code As String = lstImports.JoinBy(vbCrLf) & vbCrLf & LinqClosure
            Dim dll = CreateParameters(ReferenceList, EntityProvider.SDK)
            Dim assm As Assembly = CompileCode(code, dll, err)
            Return assm.GetProjectAbstract
        End Function

        Public Function Compile(Linq As LinqStatement, Optional ByRef err As String = "") As IProject
            Dim code As String = LinqClosure.BuildClosure(Linq.var.Name,
                                                          Linq.var.GetType(EntityProvider),
                                                          Linq.PreDeclare.ToArray(Function(x) x.Code),
                                                          Linq.AfterDeclare.ToArray(Function(x) x.Code),
                                                          Linq.SelectClosure.Projects, Linq.Where.Code)
            Return Compile(code, err)
        End Function

        Public Function Compile([declare] As CodeTypeDeclaration) As Type
            Dim assmUnit As CodeCompileUnit = DeclareAssembly()
            Dim ns As CodeNamespace = assmUnit.Namespaces.Item(0)
            Call ns.Types.Add([declare])
            Call ns.Imports.AddRange(Me.ImportsNamespace.ImportsNamespace)
            Dim assm As Assembly = CompileDll(assmUnit, ReferenceList, EntityProvider.SDK)
            Dim types As Type() = assm.GetTypes
            Dim name As String = [declare].Name
            Dim LQuery = (From x As Type In types
                          Where String.Equals(x.Name, name)
                          Select x).FirstOrDefault
            Return LQuery
        End Function

        Public Shared Function DeclareAssembly() As CodeCompileUnit
            Dim Assembly As New CodeDom.CodeCompileUnit
            Dim DynamicCodeNameSpace As New CodeNamespace("LINQDynamicCodeCompiled")
            Assembly.Namespaces.Add(DynamicCodeNameSpace)
            Return Assembly
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
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
    End Class
End Namespace