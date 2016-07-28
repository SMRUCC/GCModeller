Imports Microsoft.VisualBasic.LINQ.Framework
Imports Microsoft.VisualBasic.LINQ.Framework.Provider
Imports Microsoft.VisualBasic.Linq.LDM.Statements

Namespace LDM.Expression

    ''' <summary>
    ''' Linq expression model
    ''' </summary>
    Public Class Expression

        ''' <summary>
        ''' An object element in the target query collection.(目标待查询集合之中的一个元素)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property var As FromClosure
        ''' <summary>
        ''' Target query collection expression, this can be a file path or a database connection string.
        ''' (目标待查询集合，值可以为一个文件路径或者数据库连接字符串)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property source As InClosure
        ''' <summary>
        ''' A read only object collection which were construct by the LET statement token in the LINQ statement.
        ''' (使用Let语句所构造出来的只读对象类型的对象申明集合)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PreDeclare As LetClosure()
        ''' <summary>
        ''' Where test condition for the query.(查询所使用的Where条件测试语句)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Where As WhereClosure
        Public Property AfterDeclare As LetClosure()
        ''' <summary>
        ''' A expression for return the query result.(用于生成查询数据返回的语句)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SelectClosure As SelectClosure

        Sub New(statement As LinqStatement, registry As TypeRegistry)
            var = New FromClosure(statement.var, registry)
            source = New InClosure(statement.source, var, registry)
            PreDeclare = statement.PreDeclare.ToArray(Function(x) New LetClosure(x, registry))
            Where = New WhereClosure(statement.Where)
            AfterDeclare = statement.AfterDeclare.ToArray(Function(x) New LetClosure(x, registry))
            SelectClosure = New SelectClosure(statement.SelectClosure)
        End Sub

        Public Sub Execute()
            'Using Compiler As DynamicCompiler = New DynamicCompiler(Statement, SDK_PATH.AvaliableSDK) 'Dynamic code compiling.(动态编译代码)
            '    Dim LINQEntityLibFile As String = Statement.Object.RegistryType.AssemblyFullPath '

            '    If Not String.Equals(FileIO.FileSystem.GetParentPath(LINQEntityLibFile), System.Windows.Forms.Application.StartupPath) Then
            '        LINQEntityLibFile = String.Format("{0}\TEMP_LINQ.Entity.lib", System.Windows.Forms.Application.StartupPath)

            '        If FileIO.FileSystem.FileExists(LINQEntityLibFile) Then
            '            Call FileIO.FileSystem.DeleteFile(LINQEntityLibFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
            '        End If
            '        Call FileIO.FileSystem.CopyFile(Statement.Object.RegistryType.AssemblyFullPath, LINQEntityLibFile)
            '    End If

            '    Dim ReferenceAssemblys As String() = New String() {LQueryFramework.ReferenceAssembly, LINQEntityLibFile}
            '    Dim CompiledAssembly = Compiler.Compile(ReferenceAssemblys)
            '    Statement.ILINQProgram = DynamicInvoke.GetType(CompiledAssembly, Framework.DynamicCode.VBC.DynamicCompiler.ModuleName).First
            '    Statement._CompiledCode = Compiler.CompiledCode
            'End Using

            'Return Statement.Initialzie
        End Sub
    End Class
End Namespace