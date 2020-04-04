#Region "Microsoft.VisualBasic::b2364eda8177171b9e0246efcc85ae6f, sub-system\PLAS.NET\SSystem\System\Kernel.vb"

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

'     Class Kernel
' 
'         Properties: Model, Precision, RuntimeTicks, Vars
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: __innerTicks, GetValue, (+3 Overloads) Run
' 
'         Sub: Break, Export, Load
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Framework
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels

Namespace Kernel

    ''' <summary>
    ''' The simulation system kernel.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Kernel : Inherits Iterator.Kernel

        ''' <summary>
        ''' Data collecting
        ''' </summary>
        ''' <remarks></remarks>
        Dim dataSvr As DataAcquisition

        ''' <summary>
        ''' Object that action the disturbing.(生物扰动实验)
        ''' </summary>
        ''' <remarks></remarks>
        Public Kicks As Kicks

        ''' <summary>
        ''' Store the system state.(变量，也就是生化反应底物)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property Vars As IEnumerable(Of var)
            Get
                Return symbolTable.Values
            End Get
            Set(value As IEnumerable(Of var))
                If value Is Nothing Then
                    symbolTable = New Dictionary(Of var)
                Else
                    symbolTable = value.ToDictionary
                End If
            End Set
        End Property

        ''' <summary>
        ''' Alter the system state.(方程，也就是生化反应过程)
        ''' </summary>
        ''' <remarks></remarks>
        Public Channels As Equation()

        Friend symbolTable As Dictionary(Of var)

        ''' <summary>
        ''' Gets the system run time ticks
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property RuntimeTicks As Long
        Public ReadOnly Property Model As Script.Model

        ''' <summary>
        ''' 模拟器的数学计算引擎
        ''' </summary>
        Friend ReadOnly mathEngine As New ExpressionEngine

        Sub New(model As Script.Model, Optional dataTick As Action(Of DataSet) = Nothing)
            Call Me.Load(model, dataTick)
        End Sub

        Public Function GetValue(id As String) As var
            Return symbolTable(id)
        End Function

        ''' <summary>
        ''' 整个引擎的计算精度
        ''' </summary>
        ''' <returns></returns>
        Public Property Precision As Double = 0.1

        ''' <summary>
        ''' The kernel loop.(内核循环, 会在这里更新数学表达式计算引擎的环境变量)
        ''' </summary>
        ''' <remarks></remarks>
        Protected Overrides Sub [Step](itr As Integer)
            Call dataSvr.Tick()
            Call Kicks.Tick()
            Call (From x As Equation In Channels Select x.Elapsed(mathEngine)).ToArray
        End Sub

        ''' <summary>
        ''' 请注意，当前的线程会被阻塞在这里直到整个计算过程完成
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function Run() As Integer
            Using proc As New ProgressBar("Running PLAS.NET S-system kernel...")
                Dim prog As New ProgressProvider(proc, Model.FinalTime * (1 / Precision))

                For _RuntimeTicks = 0 To prog.Target
                    If is_terminated Then
                        Exit For
                    End If
#If DEBUG Then
                    Call __innerTicks(Me._RTime)
#Else
                    Try
                        Call [Step](RuntimeTicks)
                    Catch ex As Exception
                        ex = New Exception("Model calculation error!", ex)
                        Call App.LogException(ex)
                        Call ex.PrintException
                        Return -1
                    End Try
#End If
                    Call proc.SetProgress(prog.StepProgress)
                Next
            End Using

            Return 0
        End Function

        ''' <summary>
        ''' 中断执行
        ''' </summary>
        Public Sub Break()
            is_terminated = True
        End Sub

        Public Sub Export(Path As String)
            Call dataSvr.Save(Path)
        End Sub

        Private Sub Load(script As Script.Model, tick As Action(Of DataSet))
            Me._Model = script
            Me.Vars = LinqAPI.Exec(Of var) <=
 _
                From v As var
                In script.Vars
                Select v
                Order By Len(v.Id) Descending

            For Each declares In script.UserFunc.SafeQuery
                Call mathEngine.SetFunction(declares.Declaration)
            Next
            For Each __const In script.Constant.SafeQuery
                Call mathEngine.SetSymbol(__const.Name, __const.Value)
            Next

            For Each x As var In Vars
                mathEngine(x.Id) = x.Value
            Next

            Me.Channels = script.sEquations.Select(Function(x) New Equation(x, mathEngine))

            For i As Integer = 0 To Channels.Length - 1
                Channels(i).Set(Me)
            Next

            Kicks = New Kicks(Me)
            dataSvr = New DataAcquisition(Me, tick)
        End Sub

        ''' <summary>
        ''' The file path of the compiled xml model. 
        ''' </summary>
        ''' <param name="Path">The file path of the compiled xml model.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Run(Path As String, Optional precise As Double = 0.1) As List(Of DataSet)
            Return Kernel.Run(Script.Model.Load(Path), precise)
        End Function

        ''' <summary>
        ''' Run a compiled model.(运行一个已经编译好的模型文件)
        ''' </summary>
        ''' <param name="Model"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Run(Model As Script.Model,
                                             precise As Double,
                                             Optional dataTick As Action(Of DataSet) = Nothing) As List(Of DataSet)

            Dim Kernel As New Kernel(Model, dataTick) With {
                .Precision = precise
            }
            Call Kernel.Run()
            Return Kernel.dataSvr.data
        End Function
    End Class
End Namespace
