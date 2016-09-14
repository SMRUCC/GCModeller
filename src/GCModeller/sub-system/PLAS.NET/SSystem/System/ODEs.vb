Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Mathematical
Imports Microsoft.VisualBasic.Mathematical.diffEq
Imports SMRUCC.genomics.Analysis.SSystem.Kernel

Namespace Kernel

    Public Module ODEs

        ''' <summary>
        ''' 使用常微分方程组来解模型的计算
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        <Extension>
        Public Function RunSystem(model As Script.Model) As out
            Dim vars = LinqAPI.Exec(Of var) <=
                From x As ObjectModels.var
                In model.Vars
                Select New var With {
                    .Name = x.UniqueId,
                    .value = x.Value
                }
            Dim engine As New Expression
            Dim dynamics = (From x As Script.SEquation
                            In model.sEquations
                            Select y = New ObjectModels.Equation(x, engine)
                            Group y By y.Identifier Into Group) _
                                 .ToDictionary(Function(x) x.Identifier,
                                               Function(x) x.Group.ToArray)
            Dim tick As Long
            Dim experimentTrigger As New Kicks(
                vars.Select(Function(x) DirectCast(x, Ivar)).ToDictionary,
                model,
                Function() tick)

            Return New GenericODEs(vars) With {
                .df = Sub(dx, ByRef dy)
                          For Each var As var In vars ' 首先将所有的变量值更新到计算引擎之中
                              Call engine.Variables.Set(var.Name, var.value)
                          Next

                          For Each var In vars  ' 然后分别计算常微分方程
                              For Each eq In dynamics(var.Name)
                                  dy(var) = eq.Evaluate
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