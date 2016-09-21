#Region "Microsoft.VisualBasic::da59dc412b1423ce3e4b60b5492ef68b, ..\GCModeller\sub-system\PLAS.NET\SSystem\System\Kernel.vb"

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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.DocumentStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Terminal
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels
Imports SMRUCC.genomics.Analysis.SSystem.Script
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver

Namespace Kernel

    ''' <summary>
    ''' The simulation system kernel.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Kernel : Inherits IterationMathEngine(Of Script.Model)

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
                Return __varsHash.Values
            End Get
            Set(value As IEnumerable(Of var))
                If value Is Nothing Then
                    __varsHash = New Dictionary(Of var)
                Else
                    __varsHash = value.ToDictionary
                End If
            End Set
        End Property

        ''' <summary>
        ''' Alter the system state.(方程，也就是生化反应过程)
        ''' </summary>
        ''' <remarks></remarks>
        Public Channels As Equation()

        Friend __varsHash As Dictionary(Of var)

        ''' <summary>
        ''' 模拟器的数学计算引擎
        ''' </summary>
        ReadOnly __engine As New Mathematical.Expression

        Sub New(Model As Script.Model, Optional dataTick As Action(Of DataSet) = Nothing)
            Call MyBase.New(Model)
            Call Me.Load(Model, dataTick)
        End Sub

        Public Function GetValue(id As String) As var
            Return __varsHash(id)
        End Function

        ''' <summary>
        ''' The kernel loop.(内核循环, 会在这里更新数学表达式计算引擎的环境变量)
        ''' </summary>
        ''' <remarks></remarks>
        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer
            Call dataSvr.Tick()
            Call Kicks.Tick()
            Call (From x As Equation In Channels Select x.Elapsed(__engine)).ToArray
            Return 0
        End Function

        ''' <summary>
        ''' 整个引擎的计算精度
        ''' </summary>
        ''' <returns></returns>
        Public Property Precision As Double = 0.1

        ''' <summary>
        ''' 请注意，当前的线程会被阻塞在这里直到整个计算过程完成
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function Run() As Integer
            Dim proc As New ProgressBar("Running PLAS.NET S-system kernel...")
            Dim prog As New ProgressProvider(_innerDataModel.FinalTime * (1 / Precision))

            _break = False

            If Not TerminalEvents.ConsoleHandleInvalid Then
                For Me._RTime = 0 To prog.Target
                    If _break Then
                        Exit For
                    End If
#If DEBUG Then
                    Call __innerTicks(Me._RTime)
#Else
                Try
                    Call __innerTicks(Me._RTime)
                Catch ex As Exception
                    ex = New Exception("Model calculation error!", ex)
                    Call App.LogException(ex)
                    Call ex.PrintException
                    Return -1
                End Try
#End If
                    Call proc.SetProgress(prog.StepProgress)
                Next
            Else
                For Me._RTime = 0 To prog.Target
                    If _break Then
                        Exit For
                    End If
                    Call __innerTicks(Me._RTime)
                Next
            End If

            Return 0
        End Function

        Dim _break As Boolean = False

        ''' <summary>
        ''' 中断执行
        ''' </summary>
        Public Sub Break()
            _break = True
        End Sub

        Public ReadOnly Property Model() As Script.Model
            Get
                Return MyBase._innerDataModel
            End Get
        End Property

        Public Sub Export(Path As String)
            Call dataSvr.Save(Path)
        End Sub

        Private Sub Load(script As Script.Model, tick As Action(Of DataSet))
            Me._innerDataModel = script
            Me.Vars = LinqAPI.Exec(Of var) <=
 _
                From v As var
                In script.Vars
                Select v
                Order By Len(v.UniqueId) Descending

            For Each declares In script.UserFunc.SafeQuery
                Call __engine.Functions.Add(declares.Declaration)
            Next
            For Each __const In script.Constant.SafeQuery
                Call __engine.Constant.Add(__const.Name, __const.x)
            Next

            For Each x As var In Vars
                __engine(x.UniqueId) = x.Value
            Next

            Me.Channels = script.sEquations.ToArray(Function(x) New Equation(x, __engine))

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

        ''' <summary>
        ''' Gets the system run time ticks
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property RuntimeTicks As Long
            Get
                Return Me._RTime
            End Get
        End Property
    End Class
End Namespace
