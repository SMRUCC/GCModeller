#Region "Microsoft.VisualBasic::ad03784d845a905a6b1e24bd71eb42aa, ..\GCModeller\sub-system\PLAS.NET\SSystem\Script\ScriptCompiler.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports System.Text.RegularExpressions.Regex
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Terminal
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver

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
            MyBase._Logging = New Logging.LogFile(App.LocalData & "/.logs/" & Logging.LogFile.NowTimeNormalizedString & "." & path.BaseName & ".log")
        End Sub

        ''' <summary>
        ''' Check the consist of the metabolites and the reactions.(检查代谢物和反应通路之间的一致性，确认是否有缺失的部分)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CheckConsist(Metabolites As var(), Reactions As SEquation()) As NamedValue(Of List(Of SEquation))
            Dim sb As New StringBuilder(capacity:=1024)
            Dim out As New List(Of SEquation)

            '检查每一个反应函数所指向的目标底物的变量是否被正确的初始化了
            For Each r As SEquation In Reactions
                Dim LQuery As var() = LinqAPI.Exec(Of var) <=
 _
                    From var As var
                    In Metabolites
                    Where String.Equals(var.UniqueId, r.x)
                    Select var

                If LQuery.Length = 0 Then '没有找到
                    sb.AppendLine($"Error: Compiler could not find the target metabolite named ""{r.x}"".")
                    out += r
                End If
            Next

            If out.Count > 0 AndAlso Not Me.AutoFixError Then
                Dim exMsg As String = String.Format("Object Null Reference was found:{0}{1}", vbCrLf, sb.ToString)
                Throw New SyntaxErrorException(exMsg)
            End If

            Return New NamedValue(Of List(Of SEquation)) With {
                .Name = sb.ToString,
                .x = out
            }
        End Function

        ''' <summary>
        ''' 包含脚本文件的位置以及其他的一些参数
        ''' </summary>
        ''' <param name="args"></param>
        ''' <returns></returns>
        Public Overrides Function Compile(Optional args As CommandLine = Nothing) As Model
            Dim checked = CheckConsist(CompiledModel.Vars, CompiledModel.sEquations)

            If Not String.IsNullOrEmpty(checked.Name) Then  ' 检测的结果有错误
                Call printf("Trying to fix these problems.\n-----------------------------")

                For Each Var As SEquation In checked.x
                    CompiledModel += New var With {
                        .UniqueId = Var.x,
                        .Value = 0
                    }

                    Call printf("Added a new metabolite:  %s=0  <==  %s\n", Var.x, Var.ToString)
                Next
            End If

            Return WriteProperty(args, CompiledModel)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="path">The file path of the target compile script.(目标脚本的文件路径)</param>
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
        Public Overloads Shared Function Compile(path As String, Optional AutoFix As Boolean = False) As Model
            Using Script As New ScriptCompiler(path) With {
                .AutoFixError = AutoFix
            }
                Return Script.Compile
            End Using
        End Function

        Protected Overrides Function Link() As Integer
            Return -1
        End Function

        Public Overrides Function PreCompile(args As CommandLine) As Integer
            Return -1
        End Function
    End Class
End Namespace
