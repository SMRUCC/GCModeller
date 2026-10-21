#Region "Microsoft.VisualBasic::bf9230e532f5f595249044f5f3945653, sub-system\PLAS.NET\PLAS\CLI.vb"

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

    ' Module CLI
    ' 
    '     Function: Compile, Run
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SSystem
Imports SMRUCC.genomics.Analysis.SSystem.Runner

<Package("PLAS.CLI", Category:=APICategories.CLI_MAN, Publisher:="xie.guigang@gmail.com")>
Public Module CLI

''' <summary>
''' 命令行到仿真配置对象的适配层：算法库之内不依赖于命令行参数对象，
''' 所有的命令行开关都在这个函数之中被翻译为<see cref="RunOptions"/>配置对象
''' </summary>
''' <param name="args"></param>
''' <returns></returns>
Private Function ParseOptions(args As CommandLine) As RunOptions
    Dim options As New RunOptions With {
        .Input = args("-i"),
        .Output = args("-o"),
        .FinalTime = Val(args("/time")),
        .UseODEs = args("/ODEs"),
        .AutoFix = CBool(args("/auto-fix")),
        .Chart = CBool(args("-chart")),
        .Strict = CBool(args("/strict")),
        .NonNegative = Not CBool(args("/allow-negative")),
        .Steps = CInt(Val(args("/steps"))),
        .InitialAmountFallback = If(Val(args("/initial")) > 0, Val(args("/initial")), 1.0R),
        .InitialJitter = Val(args("/jitter")),
        .SolverName = (args <= "/solver").Trim
    }

    If Val(args("/precise")) > 0 Then
        options.Precise = Val(args("/precise"))
    End If

    options.Solver = SolverRegistry.ParseSolver(options.SolverName, ODESolverMethod.RK4)

    If Val(args("/atol")) > 0 Then
        options.SolverTuning.Atol = Val(args("/atol"))
    End If
    If Val(args("/rtol")) > 0 Then
        options.SolverTuning.Rtol = Val(args("/rtol"))
    End If
    If Val(args("/initial-step")) > 0 Then
        options.SolverTuning.InitialStep = Val(args("/initial-step"))
    End If
    If Val(args("/max-steps")) > 0 Then
        options.SolverTuning.MaxSteps = CInt(Val(args("/max-steps")))
    End If

    Return options
End Function

<ExportAPI("Run")>
<Description("run a model file of the biochemical network system.")>
<Usage("run -i <model_file> -f <script/model/sbml> [-o <output_csv> /time <-1> /ODEs /solver <solver_name>]")>
<Example("run -i ""/home/xieguigang/proj/xcc8004.sbml"" -f sbml -chart T -o ""/home/xieguigang/Desktop/xcc8004.csv""")>
<ArgumentAttribute("-i", False, Description:="The file path of the input model file that will be run on the PLAS program.")>
<ArgumentAttribute("-f", True,
    Description:="This parameter specific that the file format of the model file which will be run on the PLAS program." & vbCrLf &
                 " script - The input file that specific by the switch parameter ""-i"" is a PLAS script file," & vbCrLf &
                 " model - The input file is a compiled PLAS model, run it directly," & vbCrLf &
                 " sbml - The input file is a sbml model file, it needs to be compiled to a PLAS model first.")>
<ArgumentAttribute("-chart", True,
    Description:="Optional, This switch specific that PLAS displaying a chart windows after the calculation or not, default is F for not displaying." & vbCrLf &
                 " T - (True) display a chart window after the calculation," & vbCrLf &
                 " F - (False) not display a chart window after the calculation.")>
<ArgumentAttribute("-o", False, Description:="The file path of the output data file for the calculation.")>
<ArgumentAttribute("/ODEs", True, Description:="Optional, using the ODEs solver for the simulation instead of the discrete iteration kernel.")>
<ArgumentAttribute("/time", True, Description:="Optional, the final time value of the simulation, value <= 0 means using the FINALTIME config in the model.")>
<ArgumentAttribute("/precise", True, Description:="Optional, the integration step size of the simulation, the default value is 0.1.")>
<ArgumentAttribute("/steps", True, Description:="Optional, force the sampling point count of the ODEs integration, zero means auto derive from /time and /precise.")>
<ArgumentAttribute("/solver", True,
    Description:="Optional, the ODEs integration solver engine, the available solver engines are: " &
                 "euler, rk2, gill, rk4, abm4, backwardeuler, trapezoidal, bdf2, rosenbrockros2, rk23, rk45. The default engine is rk4.")>
<ArgumentAttribute("/atol", True, Description:="Optional, the absolute tolerance of the adaptive solvers.")>
<ArgumentAttribute("/rtol", True, Description:="Optional, the relative tolerance of the adaptive solvers.")>
<ArgumentAttribute("/initial", True, Description:="Optional, the default initial amount value of the sbml species that was missing the initialAmount attribute, the default value is 1.")>
<ArgumentAttribute("/jitter", True, Description:="Optional, the deterministic jitter ratio of the fallback initial amount value, the default value is 0.01.")>
Public Function Run(args As CommandLine) As Integer
    Dim options As RunOptions = ParseOptions(args)

    Call options.Validate()
    Call Console.WriteLine(options.ToString)

    Return RunModel.RunMethods(args <= "-f")(options)
End Function

<ExportAPI("Compile")>
<Description("Compile a script file or sbml file into the plas model file.")>
<Usage("compile -i <file> -f <script/sbml> -o <output_file> [/auto-fix]")>
<Example("compile -i ""/home/xieguigang/proj/metacyc/xcc8004/17.0/data/metabolic-reactions.sbml"" -f sbml -o ""/home/xieguigang/Desktop/xcc8004.xml""")>
Public Function Compile(args As CommandLine) As Integer
    Return Compilers(args <= "-f")(args <= "-i", args <= "-o", CBool(args("/auto-fix"))).CLICode
End Function

<ExportAPI("Solvers")>
<Description("List all of the available ODEs integration solver engines of the PLAS S-system simulation.")>
<Usage("solvers")>
Public Function Solvers(args As CommandLine) As Integer
    Call Console.WriteLine($"Available ODEs solver engines: {SolverRegistry.AvailableList()}")

    Return 0
End Function
End Module
