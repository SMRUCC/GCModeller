#Region "Microsoft.VisualBasic::a80767f1413b47ccb045c92ac949e510, sub-system\PLAS.NET\SSystem\System\ODEs.vb"

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


' Code Statistics:

'   Total Lines: 65
'    Code Lines: 50 (76.92%)
' Comment Lines: 8 (12.31%)
'    - Xml Docs: 62.50%
' 
'   Blank Lines: 7 (10.77%)
'     File Size: 2.83 KB


'     Module ODEs
' 
'         Function: RunSystem
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics.Data
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.SSystem.Runner

Namespace Kernel

    Public Module ODEs

        ''' <summary>
        ''' 方程所指向的目标变量没有被显式的申明初始量的时候所使用的默认初始量，
        ''' 取值与离散内核之中方程对象的实现保持一致
        ''' </summary>
        Public Const DefaultInitialValue As Double = 10.0R

        ''' <summary>
        ''' 变量缺省的通量值上下区间，与离散内核的非严格模式之下所使用的区间保持一致
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property DefaultBounds As DoubleRange
            Get
                Return New DoubleRange(-5, 5)
            End Get
        End Property

        ''' <summary>
        ''' 将常微分方程组的求解结果导出为与离散内核一致的数据表（首列的列名为 #Time）
        ''' </summary>
        ''' <param name="output"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 底层库的 DataFrame 扩展方法会把 y0 与 params 追加到前几行数据之中从而破坏数据表的结构，
        ''' 故这里自行完成数据表的生成
        ''' </remarks>
        Public Function ToDataFrame(output As ODESolution) As File
            Dim names As String() = output.y _
                .Values _
                .Select(Function(v) v.name) _
                .ToArray
            Dim table As New File
            Dim head As New RowObject

            Call head.Add("#Time")

            For Each name As String In names
                Call head.Add(name)
            Next

            table += head

            For i As Integer = 0 To output.x.Length - 1
                Dim row As New RowObject

                Call row.Add(CStr(output.x(i)))

                For Each name As String In names
                    Call row.Add(CStr(output.y(name).value(i)))
                Next

                table += row
            Next

            Return table
        End Function

        ''' <summary>
        ''' 使用常微分方程组来解模型的计算
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="bounds">
        ''' 可选的通量值上下区间，为空的时候会自动回落到<see cref="DefaultBounds"/>所定义的区间
        ''' </param>
        ''' <param name="options">
        ''' 仿真配置对象，其中指定了求解引擎、积分步数、积分精度等参数
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function RunSystem(model As Script.Model,
                                  Optional bounds As Dictionary(Of String, DoubleRange) = Nothing,
                                  Optional options As RunOptions = Nothing) As ODESolution

            If options Is Nothing Then
                options = New RunOptions()
            End If

            Dim finalTime As Double = options.GetFinalTime(model)
            Dim rowBounds As Dictionary(Of String, DoubleRange) = If(bounds, options.Bounds)

            If rowBounds Is Nothing Then
                rowBounds = New Dictionary(Of String, DoubleRange)
            End If

            Dim vars = LinqAPI.Exec(Of var) <=
                From x As ObjectModels.var
                In model.Vars
                Select New var With {
                    .Name = x.Id,
                    .Value = x.Value
                }

            If vars.Length = 0 Then
                Throw New InvalidOperationException("There is no state variable was initialized in the target S-system model!")
            End If

            ' 与离散内核的行为保持一致：方程所指向的目标变量如果并没有被显式的申明初始量，
            ' 则在这里补上使用默认值的状态变量，否则这些方程会被静默的丢弃掉
            Dim declared As New HashSet(Of String)(vars.Select(Function(v) v.Name))
            Dim missings As New List(Of var)

            For Each channel As Script.SEquation In model.sEquations.SafeQuery
                If Not declared.Contains(channel.x) Then
                    Call declared.Add(channel.x)
                    Call missings.Add(New var With {
                        .Name = channel.x,
                        .Value = DefaultInitialValue
                    })
                End If
            Next

            If missings.Count > 0 Then
                Call Console.WriteLine($"[WARN] There are {missings.Count} equation target(s) without any initial value declaration, using the default value {DefaultInitialValue}.")
                vars = vars.Concat(missings).ToArray
            End If

            Dim engine As New ExpressionEngine

            ' 将脚本之中所申明的常量与用户函数注册到表达式计算引擎之中，
            ' 否则方程求值的时候会因为缺失符号而得到错误的结果
            For Each userFunc In model.UserFunc.SafeQuery
                Call engine.SetFunction(userFunc.Declaration)
            Next
            For Each constant As NamedValue In model.Constant.SafeQuery
                Call engine.SetSymbol(constant.name, constant.text)
            Next

            Dim dynamics As New Dictionary(Of String, List(Of ObjectModels.Equation))

            For Each x As Script.SEquation In model.sEquations.SafeQuery
                Dim bound As DoubleRange = Nothing

                If Not rowBounds.TryGetValue(x.x, bound) Then
                    bound = DefaultBounds
                End If

                Dim channel As New ObjectModels.Equation(x, bound)
                Dim list As List(Of ObjectModels.Equation) = Nothing

                If Not dynamics.TryGetValue(x.x, list) Then
                    list = New List(Of ObjectModels.Equation)
                    Call dynamics.Add(x.x, list)
                End If

                Call list.Add(channel)
            Next

            Dim channels = dynamics _
                .ToDictionary(Function(x) x.Key,
                              Function(x)
                                  Return x.Value.ToArray
                              End Function)
            Dim symbols As Dictionary(Of String, Ivar) = vars _
                .Select(Function(x) DirectCast(x, Ivar)) _
                .ToDictionary(Function(a)
                                  Return a.Identity
                              End Function)
            ' 生物扰动实验必须要按照仿真时间轴进行触发，而不是按照 df 的调用次数，
            ' 否则 adaptive solver 与 Runge Kutta 的多阶段求值会让扰动被重复施加
            Dim currentTime As Double = 0
            Dim experimentTrigger As New Kicks(symbols, model, Function() currentTime)
            Dim nonNegative As Boolean = options.NonNegative
            Dim method As ODESolverMethod = options.Solver
            Dim steps As Integer = options.GetSteps(finalTime)

            If Not SolverRegistry.IsSupported(method) Then
                Call Console.WriteLine($"[WARN] The solver engine ""{method.ToString}"" is not supported by the S-system model, fallback to RK4.")
                method = ODESolverMethod.RK4
            End If

            Return New GenericODEs(vars) With {
                .df = Sub(dx, ByRef dy)
                          currentTime = dx

                          ' 在这里执行生物扰动实验
                          Call experimentTrigger.Tick()

                          For Each var As var In vars
                              ' 首先将所有的变量值更新到计算引擎之中
                              Dim value As Double = var.Value

                              ' S-system 的幂律之中存在分数指数，负数会越界扩散 NaN
                              If nonNegative AndAlso value < 0 Then
                                  value = 0
                              End If

                              Call engine.SetSymbol(var.Name, value)
                          Next

                          Dim flux As ObjectModels.Equation() = Nothing

                          For Each var As var In vars
                              ' 然后分别计算常微分方程
                              If Not channels.TryGetValue(var.Name, flux) Then
                                  ' 没有被任何反应通道所关联的底物，其通量为零
                                  Continue For
                              End If

                              For Each eq As ObjectModels.Equation In flux
                                  Dim delta As Double = eq.Evaluate(engine)

                                  If Double.IsNaN(delta) OrElse Double.IsInfinity(delta) Then
                                      delta = 0
                                  End If

                                  dy(index:=var) = delta
                              Next
                          Next
                      End Sub
            }.Solve(steps, 0, finalTime, method, incept:=False, options:=options.SolverTuning)
        End Function
    End Module
End Namespace
