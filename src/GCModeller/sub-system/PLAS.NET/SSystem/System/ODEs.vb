#Region "Microsoft.VisualBasic::4f4722ec0c50a06cbc6d41cdffb7b015, sub-system\PLAS.NET\SSystem\System\ODEs.vb"

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

    '     Module ODEs
    ' 
    '         Function: RunSystem
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics.Data
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression

Namespace Kernel

    Public Module ODEs

        ''' <summary>
        ''' 使用常微分方程组来解模型的计算
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        <Extension>
        Public Function RunSystem(model As Script.Model) As ODEsOut
            Dim vars = LinqAPI.Exec(Of var) <=
                From x As ObjectModels.var
                In model.Vars
                Select New var With {
                    .Name = x.Id,
                    .Value = x.Value
                }
            Dim engine As New ExpressionEngine
            Dim dynamics = (From x As Script.SEquation
                            In model.sEquations
                            Select y = New ObjectModels.Equation(x)
                            Group y By y.Id Into Group) _
                                 .ToDictionary(Function(x) x.Id,
                                               Function(x)
                                                   Return x.Group.ToArray
                                               End Function)
            Dim tick As Long
            Dim symbols As Dictionary(Of String, Ivar) = vars _
                .Select(Function(x) DirectCast(x, Ivar)) _
                .ToDictionary(Function(a)
                                  Return a.Identity
                              End Function)
            Dim experimentTrigger As New Kicks(symbols, model, Function() tick)

            Return New GenericODEs(vars) With {
                .df = Sub(dx, ByRef dy)
                          For Each var As var In vars
                              ' 首先将所有的变量值更新到计算引擎之中
                              Call engine.SetSymbol(var.Name, var.Value)
                          Next

                          For Each var As var In vars
                              ' 然后分别计算常微分方程
                              For Each eq In dynamics(var.Name)
                                  dy(index:=var) = eq.Evaluate(engine)
                              Next
                          Next

                          tick += 1

                          ' 在这里执行生物扰动实验
                          Call experimentTrigger.Tick()
                      End Sub
            }.Solve(100000, 0, model.FinalTime)
        End Function
    End Module
End Namespace
