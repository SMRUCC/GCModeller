#Region "Microsoft.VisualBasic::8d1cb3df50a087312c9b68682bff42ac, PLAS.NET\SSystem\System\KernelLoader.vb"

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

    '     Module KernelLoader
    ' 
    '         Function: initializeEnvironment, loadModel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels

Namespace Kernel

    Public Module KernelLoader

        ''' <summary>
        ''' 初始化变量集合（模拟环境）
        ''' </summary>
        ''' <param name="kernel"></param>
        ''' <param name="script"></param>
        ''' <returns></returns>
        <Extension>
        Private Function initializeEnvironment(kernel As Kernel, script As Script.Model) As var()
            Dim vars As var()

            For Each declares In script.UserFunc.SafeQuery
                Call kernel.mathEngine.SetFunction(declares.Declaration)
            Next
            For Each __const__ In script.Constant.SafeQuery
                Call kernel.mathEngine.SetSymbol(__const__.Name, __const__.Value)
            Next

            vars = LinqAPI.Exec(Of var) _
 _
                () <= From v As var
                      In script.Vars
                      Select v
                      Order By Len(v.Id) Descending

            For Each x As var In vars
                kernel.mathEngine(x.Id) = x.Value
            Next

            Return vars
        End Function

        <Extension>
        Public Function loadModel(kernel As Kernel, script As Script.Model) As Kernel
            kernel.Vars = kernel.initializeEnvironment(script)
            kernel.kicks = New Kicks(kernel, script)
            kernel.Channels = script.sEquations _
                .Select(Function(x)
                            ' 在初始化动力学方程组之前必须要先初始化变量集合
                            Return New Equation(x, kernel)
                        End Function) _
                .ToArray

            Return kernel
        End Function
    End Module
End Namespace
