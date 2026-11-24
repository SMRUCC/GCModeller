' *****************************************************************************
' Copyright 2018 sciBASIC.NET Foundation
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'   http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
' *****************************************************************************

Imports System.Linq
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models

Namespace Darwinism.NSGA2

    ''' <summary>
    ''' The evolution loop driver of the NSGA-II engine.
    ''' </summary>
    ''' <typeparam name="Chr">The chromosome type of the genetic algorithm.</typeparam>
    ''' <remarks>
    ''' The ``error`` value which is reported by the 
    ''' <see cref="IterationReporter(Of T).DoReport"/> delegate is only used as a 
    ''' scalar progress indicator (the mean objective sum of the first front); 
    ''' it does NOT participate in the selection procedure.
    ''' </remarks>
    Public Class NSGA2Driver(Of Chr As {Class, Chromosome(Of Chr)}) : Inherits IterationReporter(Of NSGA2Engine(Of Chr))

        Dim core As NSGA2Engine(Of Chr)
        Dim terminated As Boolean

        ''' <summary>
        ''' 需要运行的总的迭代次数
        ''' </summary>
        ''' <returns>The maximum number of the evolution generations.</returns>
        Public Property Generations As Integer

        ''' <summary>
        ''' The NSGA-II engine which is driven by this driver.
        ''' </summary>
        ''' <returns>A <see cref="NSGA2Engine(Of Chr)"/> object.</returns>
        Public ReadOnly Property Model As NSGA2Engine(Of Chr)
            Get
                Return core
            End Get
        End Property

        ''' <summary>
        ''' The first non-dominated front of the current evolution state.
        ''' </summary>
        ''' <returns>An array of the Pareto solutions.</returns>
        Public ReadOnly Property ParetoFront As ParetoSolution(Of Chr)()
            Get
                Return core.ParetoFront
            End Get
        End Property

        ''' <summary>
        ''' Create a evolution loop driver for the given NSGA-II engine.
        ''' </summary>
        ''' <param name="model">The NSGA-II engine which will be evolved.</param>
        ''' <param name="takeSnapshot">
        ''' An optional callback which is invoked at each generation with the 
        ''' iteration number and the engine object; when this parameter is 
        ''' ``Nothing`` the progress will be printed to the debugger output instead.
        ''' </param>
        ''' <param name="generations">The maximum number of the evolution generations, the default value is ``250``.</param>
        Public Sub New(model As NSGA2Engine(Of Chr),
                       Optional takeSnapshot As Action(Of Integer, NSGA2Engine(Of Chr)) = Nothing,
                       Optional generations As Integer = 250)

            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If

            Me.core = model
            Me.Generations = generations

            If takeSnapshot Is Nothing Then
                Call AttachReporter(
                    Sub(i As Integer, progress As Double, engine As NSGA2Engine(Of Chr))
                        Call VBDebugger.EchoLine($"[{i}/{generations}] {progress:F4}")
                    End Sub)
            Else
                Call AttachReporter(
                    Sub(i As Integer, progress As Double, engine As NSGA2Engine(Of Chr))
                        Call takeSnapshot(i, engine)
                    End Sub)
            End If
        End Sub

        ''' <summary>
        ''' Run the NSGA-II evolution loop until the <see cref="Generations"/> 
        ''' limit is reached or the <see cref="Terminate"/> method is called.
        ''' </summary>
        ''' <param name="parallel">
        ''' Whether the objective calculation of the offspring population should 
        ''' be run in parallel mode? The default value of this parameter is 
        ''' ``False``.
        ''' </param>
        Public Overrides Sub Train(Optional parallel As Boolean = False)
            If Generations <= 0 Then
                Throw New InvalidOperationException("The number of the evolution generations should be greater than zero.")
            End If

            terminated = False

            For i As Integer = 1 To Generations
                If terminated Then
                    Exit For
                End If

                Call core.Evolve()

                Dim progress As Double = ScalarProgress(core)

                If Not reporter Is Nothing Then
                    Call reporter(i, progress, core)
                End If
            Next

            Call "Exit NSGA-II training loop due to the reason of reach iteration Upbound...".debug
        End Sub

        ''' <summary>
        ''' Request the running evolution loop to be terminated gracefully at the 
        ''' end of the current generation.
        ''' </summary>
        Public Sub Terminate()
            Me.terminated = True
        End Sub

        ''' <summary>
        ''' Create one iteration report record from the current state of the 
        ''' NSGA-II engine.
        ''' </summary>
        ''' <param name="iteration">The current iteration number.</param>
        ''' <param name="progress">The scalar progress value of the current state.</param>
        ''' <param name="model">The NSGA-II engine of the current evolution process.</param>
        ''' <returns>An <see cref="NSGA2Report"/> record which describes the current iteration.</returns>
        Public Shared Function CreateReport(iteration As Integer, progress As Double,
                                            model As NSGA2Engine(Of Chr)) As NSGA2Report
            Dim front As ParetoSolution(Of Chr)() = model.ParetoFront

            Return New NSGA2Report With {
                .Iteration = iteration,
                .Progress = progress,
                .FrontSize = front.Length,
                .Objectives = front.Select(Function(solution) solution.Objectives).ToArray
            }
        End Function

        ''' <summary>
        ''' The mean objective sum of the first front; this value is only used as 
        ''' a scalar progress indicator for the text report.
        ''' </summary>
        Private Shared Function ScalarProgress(model As NSGA2Engine(Of Chr)) As Double
            Return model.ProgressValue
        End Function
    End Class

    ''' <summary>
    ''' One iteration report record of the NSGA-II evolution process.
    ''' </summary>
    Public Structure NSGA2Report

        ''' <summary>
        ''' The current iteration number.
        ''' </summary>
        ''' <returns>An <see cref="Integer"/> value.</returns>
        Public Property Iteration As Integer
        ''' <summary>
        ''' The scalar progress value of this iteration.
        ''' </summary>
        ''' <returns>A <see cref="Double"/> value.</returns>
        Public Property Progress As Double
        ''' <summary>
        ''' The number of the solutions in the first non-dominated front.
        ''' </summary>
        ''' <returns>An <see cref="Integer"/> value.</returns>
        Public Property FrontSize As Integer
        ''' <summary>
        ''' The objective vectors of the first non-dominated front.
        ''' </summary>
        ''' <returns>A objective matrix.</returns>
        Public Property Objectives As Double()()

        ''' <summary>
        ''' Display this iteration report record as one text line.
        ''' </summary>
        ''' <returns>A string which describes this iteration.</returns>
        Public Overrides Function ToString() As String
            Return $"[{Iteration}] front size={FrontSize}, progress={Progress:F4}"
        End Function
    End Structure
End Namespace
