#Region "Microsoft.VisualBasic::a11c25eba211c8dc05cc63651478bfbb, engine\GCModeller\EngineSystem\ObjectModels\ExperimentSystem\TriggerSystem\vbc\DynamicCompiler.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class DynamicCompiler
    ' 
    '         Properties: CompiledCode
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: (+2 Overloads) Compile, CreateTriggerConditionTest, DeclareAssembly, DeclareFunction, GenerateCode
    '                   GetDebugInformation, ImportsNamespace
    ' 
    '         Sub: AddTestModel, (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.CodeDom.Compiler
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Extensions

Namespace Framework.DynamicCode.VBC

    ''' <summary>
    ''' 编译整个LINQ语句的动态代码编译器
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DynamicCompiler : Implements System.IDisposable

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Dim DotNETReferenceAssembliesDir As String = "C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1"

        Public Const CONDITION_TEST_MODULE_NAME As String = "ConditionTest"
        Public Const CONDITION_TEST_NAMESPACE As String = "TriggerConditionTest"
        Public Const TEST_FUNCTION_NAME As String = "Test"

        Dim ObjectModel As CodeDom.CodeNamespace, SystemReference As EngineSystem.ObjectModels.SubSystem.CellSystem

        Public ReadOnly Property CompiledCode As String
            Get
                Return GenerateCode(ObjectModel)
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SDK">.NET Framework Reference Assembly文件夹的位置</param>
        ''' <remarks></remarks>
        Sub New(SystemReference As EngineSystem.ObjectModels.SubSystem.CellSystem, SDK As String)
            Me.SystemReference = SystemReference
            Me.DotNETReferenceAssembliesDir = SDK

            Dim ProgramDirectory As String = My.Application.Info.DirectoryPath
            Me.UsrReferenceAssemblys = New String() {
                ProgramDirectory & "/SMRUCC.genomics.Assembly.dll",
                ProgramDirectory & "/CsvTabular.dll",
                ProgramDirectory & "/GCMarkupLanguage.dll",
                ProgramDirectory & "/SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.dll",
                ProgramDirectory & "/SMRUCC.genomics.GCModeller.ModellingEngine.Kernel.dll",
                ProgramDirectory & "/Microsoft.VisualBasic.Framework.Kernel_Driver.dll",
                ProgramDirectory & "/SBML.dll",
                ProgramDirectory & "/DocumentFormat.Csv.dll"}
        End Sub

        Public Function DeclareAssembly() As CodeDom.CodeCompileUnit
            Dim Assembly As CodeDom.CodeCompileUnit = New CodeDom.CodeCompileUnit
            Dim DynamicCodeNameSpace As CodeDom.CodeNamespace = New CodeDom.CodeNamespace(CONDITION_TEST_NAMESPACE)
            Assembly.Namespaces.Add(DynamicCodeNameSpace)

            DynamicCodeNameSpace.Types.Add(Me.TestMethodsContainer)
            DynamicCodeNameSpace.Imports.AddRange(ImportsNamespace)
            ObjectModel = DynamicCodeNameSpace

            Call SystemReference.SystemLogging.WriteLine(vbCrLf & "Trigger scripts DEBUG information:" & vbCrLf & vbCrLf & vbCrLf & GenerateCode(DynamicCodeNameSpace), "vbc_debug --> Dynamic compiled code()", Type:=MSG_TYPES.INF)

            Return Assembly
        End Function

        Private Function ImportsNamespace() As CodeDom.CodeNamespaceImport()
            Dim Array As CodeDom.CodeNamespaceImport() = New CodeDom.CodeNamespaceImport() {
                New CodeDom.CodeNamespaceImport("Microsoft.VisualBasic"),
                New CodeDom.CodeNamespaceImport("System"),
                New CodeDom.CodeNamespaceImport("System.Collections"),
                New CodeDom.CodeNamespaceImport("System.Collections.Generic"),
                New CodeDom.CodeNamespaceImport("System.Data"),
                New CodeDom.CodeNamespaceImport("System.Diagnostics"),
                New CodeDom.CodeNamespaceImport("System.Linq"),
                New CodeDom.CodeNamespaceImport("System.Xml.Linq"),
                New CodeDom.CodeNamespaceImport("System.Text.RegularExpressions")}
            Return Array
        End Function

        Dim TestMethodsContainer As CodeDom.CodeTypeDeclaration = New CodeDom.CodeTypeDeclaration(CONDITION_TEST_MODULE_NAME)

        Private Shared Function CreateTriggerConditionTest(Handle As Long, Expression As CodeDom.CodeExpression) As CodeDom.CodeTypeMember
            Dim StatementCollection As CodeDom.CodeStatementCollection = New CodeDom.CodeStatementCollection
            StatementCollection.Add(New CodeDom.CodeAssignStatement(New CodeDom.CodeVariableReferenceExpression("rval"), Expression))

            Dim [Function] As CodeDom.CodeMemberMethod = DynamicCode.VBC.DynamicCompiler.DeclareFunction(String.Format("{0}___trigger__{1}", TEST_FUNCTION_NAME, Handle), "System.Boolean", StatementCollection)
            [Function].Attributes = CodeDom.MemberAttributes.Public Or CodeDom.MemberAttributes.Static
            Call [Function].Parameters.AddRange((From Parameter As KeyValuePair(Of String, Type)
                                                 In Prefix.PrefixTypeReference
                                                 Select New CodeDom.CodeParameterDeclarationExpression(type:=Parameter.Value, name:=Parameter.Key)).ToArray)
            Return [Function]
        End Function

        Protected ReadOnly UsrReferenceAssemblys As String()

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Compile() As System.Reflection.Assembly
            Dim Code = DeclareAssembly()
            Dim Assembly = Compile(Code, UsrReferenceAssemblys, DotNETReferenceAssembliesDir, SystemLogging:=Me.SystemReference.SystemLogging)
            Return Assembly
        End Function

        Public Sub AddTestModel(Handle As Long, BoolExpression As CodeDom.CodeExpression)
            Call TestMethodsContainer.Members.Add(DynamicCode.VBC.DynamicCompiler.CreateTriggerConditionTest(Handle, BoolExpression))
        End Sub

        ''' <summary>
        ''' Compile the codedom object model into a binary assembly module file.(将CodeDOM对象模型编译为二进制应用程序文件)
        ''' </summary>
        ''' <param name="ObjectModel">CodeDom dynamic code object model.(目标动态代码的对象模型)</param>
        ''' <param name="Reference">Reference assemby file path collection.(用户代码的引用DLL文件列表)</param>
        ''' <param name="DotNETReferenceAssembliesDir">.NET Framework SDK</param>
        ''' <param name="CodeStyle">VisualBasic, C#</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Compile(ObjectModel As CodeDom.CodeCompileUnit, Reference As String(), DotNETReferenceAssembliesDir As String,
                                       Optional CodeStyle As String = "VisualBasic",
                                       Optional SystemLogging As LogFile = Nothing) _
            As System.Reflection.Assembly

            Dim CodeDomProvider As CodeDom.Compiler.CodeDomProvider = CodeDom.Compiler.CodeDomProvider.CreateProvider(CodeStyle)
            Dim Options As CodeDom.Compiler.CompilerParameters = New CodeDom.Compiler.CompilerParameters

            Options.GenerateInMemory = False
            Options.IncludeDebugInformation = True
            Options.GenerateExecutable = False

            Call FileIO.FileSystem.CreateDirectory(Settings.DataCache & "/ActionScript/")

            Options.OutputAssembly = String.Format("{0}/ActionScript/action_script___trigger___{1}.dll", Settings.DataCache, SystemLogging.FileName.ToLower)

            If Not Reference.IsNullOrEmpty Then
                Call Options.ReferencedAssemblies.AddRange(Reference)
            End If

            Call Options.ReferencedAssemblies.AddRange(New String() {
                   DotNETReferenceAssembliesDir & "\System.dll",
                   DotNETReferenceAssembliesDir & "\System.Core.dll",
                   DotNETReferenceAssembliesDir & "\System.Data.dll",
                   DotNETReferenceAssembliesDir & "\System.Data.DataSetExtensions.dll",
                   DotNETReferenceAssembliesDir & "\System.Xml.dll",
                   DotNETReferenceAssembliesDir & "\System.Xml.Linq.dll"})

            Dim Compiled = CodeDomProvider.CompileAssemblyFromDom(Options, ObjectModel)

            If Not SystemLogging Is Nothing Then
                Call SystemLogging.WriteLine(Options.CompilerOptions, "vbc_debug ==> Options.CompilerOptions()", Type:=MSG_TYPES.INF)
                Call SystemLogging.WriteLine(GetDebugInformation(Compiled), "vbc_debug", Type:=MSG_TYPES.INF)
            End If

            Return Compiled.CompiledAssembly
        End Function

        Public Shared Function GetDebugInformation(CompiledResult As CodeDom.Compiler.CompilerResults) As String
            Dim sBuilder As StringBuilder = New StringBuilder

            sBuilder.AppendLine("Error Information:")
            For Each [Error] In CompiledResult.Errors
                sBuilder.AppendLine([Error].ToString)
            Next
            sBuilder.AppendLine(vbCrLf & "Compiler Output:")
            For Each Line As String In CompiledResult.Output
                sBuilder.AppendLine(Line)
            Next

#If DEBUG Then
            Call sBuilder.ToString.SaveTo(Settings.TEMP & "\CodeDom.log")
#End If
            Return sBuilder.ToString
        End Function

        ''' <summary>
        ''' Generate the source code from the CodeDOM object model.(根据对象模型生成源代码以方便调试程序)
        ''' </summary>
        ''' <param name="NameSpace"></param>
        ''' <param name="CodeStyle">VisualBasic, C#</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' You can easily convert the source code between VisualBasic and C# using this function just by makes change in statement: 
        ''' CodeDomProvider.GetCompilerInfo("VisualBasic").CreateProvider().GenerateCodeFromNamespace([NameSpace], sWriter, Options)
        ''' Modify the VisualBasic in to C#
        ''' </remarks>
        Public Shared Function GenerateCode([NameSpace] As CodeDom.CodeNamespace, Optional CodeStyle As String = "VisualBasic") As String
            Dim sBuilder As StringBuilder = New StringBuilder()

            Using sWriter As IO.StringWriter = New System.IO.StringWriter(sBuilder)
                Dim Options As New CodeGeneratorOptions() With {
                    .IndentString = "  ", .ElseOnClosing = True, .BlankLinesBetweenMembers = True}
                CodeDomProvider.GetCompilerInfo(CodeStyle).CreateProvider().GenerateCodeFromNamespace([NameSpace], sWriter, Options)
                Return sBuilder.ToString()
            End Using
        End Function

        ''' <summary>
        ''' Declare a function with a specific function name and return type. please notice that in this newly 
        ''' declare function there is always a local variable name rval using for return the value.
        ''' (申明一个方法，返回指定类型的数据并且具有一个特定的函数名，请注意，在这个新申明的函数之中，
        ''' 固定含有一个rval的局部变量用于返回数据)
        ''' </summary>
        ''' <param name="Name">Function name.(函数名)</param>
        ''' <param name="Type">Function return value type.(该函数的返回值类型)</param>
        ''' <returns>A codeDOM object model of the target function.(一个函数的CodeDom对象模型)</returns>
        ''' <remarks></remarks>
        Public Shared Function DeclareFunction(Name As String, Type As String, Statements As CodeDom.CodeStatementCollection) As CodeDom.CodeMemberMethod
            Dim CodeMemberMethod As CodeDom.CodeMemberMethod = New CodeDom.CodeMemberMethod() With {.Name = Name, .ReturnType = New CodeDom.CodeTypeReference(Type)}

            If String.Equals(Type, "System.Boolean", StringComparison.OrdinalIgnoreCase) Then
                Call CodeMemberMethod.Statements.Add(New CodeDom.CodeVariableDeclarationStatement(Type, "rval", New CodeDom.CodePrimitiveExpression(True)))   '创建一个用于返回值的局部变量，对于逻辑值，默认为真
            Else
                Call CodeMemberMethod.Statements.Add(New CodeDom.CodeVariableDeclarationStatement(Type, "rval"))   '创建一个用于返回值的局部变量
            End If

            If Not (Statements Is Nothing OrElse Statements.Count = 0) Then
                Call CodeMemberMethod.Statements.AddRange(Statements)
            End If

            Call CodeMemberMethod.Statements.Add(New CodeDom.CodeMethodReturnStatement(New CodeDom.CodeVariableReferenceExpression("rval")))  '引用返回值的局部变量

            Return CodeMemberMethod
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
