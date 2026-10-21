#Region "Microsoft.VisualBasic::f688da653c68692eae25514c358dd9a6, sub-system\PLAS.NET\SSystem\RunModel.vb"

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

    '   Total Lines: 73
    '    Code Lines: 53 (72.60%)
    ' Comment Lines: 7 (9.59%)
    '    - Xml Docs: 71.43%
    ' 
    '   Blank Lines: 13 (17.81%)
    '     File Size: 2.56 KB


    ' Module RunModel
    ' 
    ' 
    '     Delegate Function
    ' 
    '         Properties: RunMethods
    ' 
    '         Function: (+2 Overloads) RunModel, RunSBML, RunScript
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Math.Calculus
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics.Data
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SSystem.Runner
Imports SMRUCC.genomics.Analysis.SSystem.Script

Public Module RunModel

    ''' <summary>
    ''' Run a S-system model by using the simulation configuration object.
    ''' (以配置对象作为驱动的模型运行入口)
    ''' </summary>
    ''' <param name="options"></param>
    ''' <returns></returns>
    Public Delegate Function IRunModel(options As RunOptions) As Integer

    ''' <summary>
    ''' 依据输入模型的格式的名称，大小写不敏感
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property RunMethods As IReadOnlyDictionary(Of String, IRunModel) =
        New HashDictionary(Of IRunModel) From {
 _
            {"script", AddressOf RunScript},
            {"model", AddressOf RunModel},
            {"sbml", AddressOf RunSBML}
    }

    ''' <summary>
    ''' 从 PLAS 脚本文本之中编译出模型并运行
    ''' </summary>
    ''' <param name="options"></param>
    ''' <returns></returns>
    Public Function RunScript(options As RunOptions) As Integer
        Return Script.ScriptCompiler _
            .Compile(path:=options.Input, AutoFix:=options.AutoFix) _
            .RunModel(options:=options)
    End Function

    ''' <summary>
    ''' 直接运行一个已经编译好的模型文件
    ''' </summary>
    ''' <param name="options"></param>
    ''' <returns></returns>
    Public Function RunModel(options As RunOptions) As Integer
        Return Script.Model _
            .Load(options.Input) _
            .RunModel(options:=options)
    End Function

    ''' <summary>
    ''' 从 SBML 文件之中编译出模型并运行
    ''' </summary>
    ''' <param name="options"></param>
    ''' <returns></returns>
    Public Function RunSBML(options As RunOptions) As Integer
        Return Script.SBML _
            .Compile(path:=options.Input, AutoFix:=options.AutoFix, options:=options) _
            .RunModel(options:=options)
    End Function

    ''' <summary>
    ''' Run model from the simulation configuration object.(依据配置对象运行模型并返回退出码)
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="options"></param>
    ''' <returns></returns>
    <Extension>
    Public Function RunModel(model As Script.Model, options As RunOptions) As Integer
        If options Is Nothing Then
            options = New RunOptions()
        End If

        Call options.Validate()

        Dim out As String = options.GetOutputPath()

        model.FinalTime = options.GetFinalTime(model)

        If options.UseODEs Then
            Call Console.WriteLine($"PLAS using ODEs solver {options.Solver.ToString}....")

            Dim output As ODEsOut = Kernel.ODEs.RunSystem(model, Nothing, options)
            Dim df As File = Kernel.ODEs.ToDataFrame(output)

            Return df.Save(out, Encodings.ASCII)
        Else
            Call Console.WriteLine($"PLAS using discrete iteration kernel, ticks={Kernel.Kernel.TickCount(model.FinalTime, options.Precise)}....")

            Dim ds As IEnumerable(Of DataSet) = Kernel.Kernel.Run(model, options)
            Dim maps As New Dictionary(Of String, String) From {
                {NameOf(DataSet.ID), "#Time"}
            }

            Return ds _
                .SaveTo(path:=out, nonParallel:=True, maps:=maps) _
                .CLICode
        End If
    End Function
End Module
