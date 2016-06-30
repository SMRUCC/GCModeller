Imports System.Text.RegularExpressions.Regex
Imports System.Text
Imports LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver
Imports LANS.SystemsBiology.AnalysisTools.CellPhenotype.SSystem.Kernel.ObjectModels
Imports Microsoft.VisualBasic

Namespace Script

    ''' <summary>
    ''' Modeling script compiler
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ScriptCompiler : Inherits Compiler(Of Model)

        Public Const Comment As String = "/\*.+\*/"

        Public Property AutoFixError As Boolean = False

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="path">The file path of the PLAS script.</param>
        Sub New(path As String)
            MyBase.CompiledModel = ScriptParser.ParseFile(path)
        End Sub

        ''' <summary>
        ''' Check the consist of the metabolites and the reactions.(检查代谢物和反应通路之间的一致性，确认是否有缺失的部分)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CheckConsist(Metabolites As Var(), Reactions As SEquation()) As KeyValuePair(Of String, List(Of SEquation))
            Dim sBuilder As StringBuilder = New StringBuilder(capacity:=1024)
            Dim ResultList As List(Of SEquation) = New List(Of SEquation)

            '检查每一个反应函数所指向的目标底物的变量是否被正确的初始化了
            For Each Reaction In Reactions
                Dim LQuery = (From var In Metabolites Where String.Equals(var.UniqueId, Reaction.x) Select var).ToArray
                If LQuery.Length = 0 Then '没有找到
                    sBuilder.AppendLine($"Error: Compiler could not find the target metabolite named ""{Reaction.x}"".")
                    Call ResultList.Add(Reaction)
                End If
            Next

            If ResultList.Count > 0 AndAlso Not Me.AutoFixError Then
                Dim exMsg As String = String.Format("Object Null Reference was found:{0}{1}", vbCrLf, sBuilder.ToString)
                Throw New SyntaxErrorException(exMsg)
            End If

            Return New KeyValuePair(Of String, List(Of SEquation))(sBuilder.ToString, ResultList)
        End Function

        ''' <summary>
        ''' 包含脚本文件的位置以及其他的一些参数
        ''' </summary>
        ''' <param name="args"></param>
        ''' <returns></returns>
        Public Overrides Function Compile(Optional args As CommandLine.CommandLine = Nothing) As Model
            Dim Checked = CheckConsist(CompiledModel.Vars, CompiledModel.sEquations)
            If Not String.IsNullOrEmpty(Checked.Key) Then  '检测的结果有错误
                Console.WriteLine("Trying to fix these problems.{0}-----------------------------", vbCrLf)

                For Each Var In Checked.Value
                    Call CompiledModel.Add(New Var With {.UniqueId = Var.x, .Value = 0})
                    Console.WriteLine("Added a new metabolite:  {0}=0  <==  {1}", Var.x, Var.ToString)
                Next
            End If
            Return WriteProperty(args, CompiledModel)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Path">The file path of the target compile script.(目标脚本的文件路径)</param>
        ''' <param name="AutoFix">
        ''' Optional，when error occur in the procedure of the script compiled, then if TRUE, the program was 
        ''' trying to fix the error automatically, if FALSE, then the program throw an exception and then 
        ''' exit the compile procedure.
        ''' (可选参数，当在脚本文件的编译的过程之中出现错误的话，假若本参数为真，则程序会尝试着自己修复这个错误并给出
        ''' 警告，假若不为真，则程序会抛出一个错误并退出整个编译过程。请注意，即使本参数为真，当遭遇重大错误程序无法
        ''' 处理的时候，整个编译过程还是会被意外终止的。)
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Compile(Path As String, Optional AutoFix As Boolean = False) As Model
            Dim Script As ScriptCompiler = New ScriptCompiler(Path) With {
                .AutoFixError = AutoFix
            }
            Return Script.Compile
        End Function

        Protected Overrides Function Link() As Integer
            Return -1
        End Function

        Public Overrides Function PreCompile(ARGV As CommandLine.CommandLine) As Integer
            Return -1
        End Function
    End Class
End Namespace