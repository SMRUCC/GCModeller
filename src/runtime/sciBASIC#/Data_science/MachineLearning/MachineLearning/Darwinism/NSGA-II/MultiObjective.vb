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

Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models

Namespace Darwinism.NSGA2

    ''' <summary>
    ''' The evaluation model of a multi-objective optimization problem which 
    ''' produces one objective vector for each candidate chromosome.
    ''' </summary>
    ''' <typeparam name="Chr">
    ''' The chromosome type of the genetic algorithm. This interface does not 
    ''' apply the <see cref="Chromosome(Of Chr)"/> constraint, so that it can be 
    ''' used by the other evolutionary modules such as the differential 
    ''' evolution in the same way as the <see cref="Fitness(Of Chr)"/> interface.
    ''' </typeparam>
    ''' <remarks>
    ''' All of the objective values are treated as the minimization problems; a 
    ''' smaller value is always better. If one objective should be maximized, 
    ''' flip its sign in the implementation or use the 
    ''' <see cref="SingleObjectiveAdapter(Of Chr)"/> helper.
    ''' </remarks>
    Public Interface IMultiObjective(Of Chr)

        ''' <summary>
        ''' 这个多目标计算模块是否会缓存计算结果?
        ''' </summary>
        ''' <returns>
        ''' ``True`` when the evaluation result can be cached by the unique 
        ''' identity of the target chromosome, otherwise ``False``.
        ''' </returns>
        ReadOnly Property Cacheable As Boolean

        ''' <summary>
        ''' The number of the objective functions which are defined by this model.
        ''' </summary>
        ''' <returns>An <see cref="Integer"/> value which should be greater than zero.</returns>
        ReadOnly Property Objectives As Integer

        ''' <summary>
        ''' Evaluate the objective vector of the given chromosome.
        ''' </summary>
        ''' <param name="chromosome">The candidate chromosome which will be evaluated.</param>
        ''' <param name="parallel">
        ''' 在计算函数的内部是否是应该是并行的?
        ''' 
        ''' 应该遵循以下的准则:
        ''' 
        ''' 1. 如果外部调用这个计算函数是并行的,那么这个parallel参数应该设置为false
        ''' 2. 如果是单线程的外部代码调用这个计算函数,那么这个parallel参数可以是true,即在函数的内部实现并行化
        ''' </param>
        ''' <returns>
        ''' The objective vector of the given <paramref name="chromosome"/>; the 
        ''' length of the returned vector must be equals to the value of the 
        ''' <see cref="Objectives"/> property.
        ''' </returns>
        Function Evaluate(chromosome As Chr, parallel As Boolean) As Double()

    End Interface

    ''' <summary>
    ''' A <see cref="IMultiObjective(Of Chr)"/> adapter which creates the 
    ''' objective vector from a delegate function.
    ''' </summary>
    ''' <typeparam name="Chr">The chromosome type of the genetic algorithm.</typeparam>
    Public Class DelegateObjective(Of Chr) : Implements IMultiObjective(Of Chr)

        ReadOnly evaluator As Func(Of Chr, Boolean, Double())
        ReadOnly objectiveCount%

        ''' <summary>
        ''' Create a multi-objective evaluation model from a delegate function.
        ''' </summary>
        ''' <param name="evaluate">The objective vector provider function.</param>
        ''' <param name="objectives">The number of the objective functions.</param>
        ''' <param name="cacheable">Does the evaluation result can be cached?</param>
        Public Sub New(evaluate As Func(Of Chr, Boolean, Double()), objectives As Integer, Optional cacheable As Boolean = False)
            If evaluate Is Nothing Then
                Throw New ArgumentNullException(NameOf(evaluate))
            End If
            If objectives <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(objectives), "The number of the objective functions should be greater than zero.")
            End If

            Me.evaluator = evaluate
            Me.objectiveCount = objectives
            Me.Cacheable = cacheable
        End Sub

        ''' <summary>
        ''' The number of the objective functions which are defined by this model.
        ''' </summary>
        ''' <returns>An <see cref="Integer"/> value.</returns>
        Public ReadOnly Property Objectives As Integer Implements IMultiObjective(Of Chr).Objectives
            Get
                Return objectiveCount
            End Get
        End Property

        ''' <summary>
        ''' Does the evaluation result can be cached?
        ''' </summary>
        ''' <returns>A <see cref="Boolean"/> value.</returns>
        Public ReadOnly Property Cacheable As Boolean Implements IMultiObjective(Of Chr).Cacheable

        ''' <summary>
        ''' Evaluate the objective vector of the given chromosome.
        ''' </summary>
        ''' <param name="chromosome">The candidate chromosome which will be evaluated.</param>
        ''' <param name="parallel">Whether the evaluation should be run in parallel mode?</param>
        ''' <returns>The objective vector of the given <paramref name="chromosome"/>.</returns>
        Public Function Evaluate(chromosome As Chr, parallel As Boolean) As Double() Implements IMultiObjective(Of Chr).Evaluate
            Return evaluator(chromosome, parallel)
        End Function
    End Class

    ''' <summary>
    ''' Adapts the single objective <c>Fitness(Of Chr)</c> model of the 
    ''' classic genetic algorithm into the multi-objective evaluation model, so 
    ''' that the NSGA-II engine is able to optimize the existing fitness 
    ''' functions directly.
    ''' </summary>
    ''' <typeparam name="Chr">
    ''' The chromosome type of the genetic algorithm.
    ''' </typeparam>
    ''' <remarks>
    ''' When the ``maximize`` flag of the constructor is ``True`` the sign of 
    ''' the fitness value is flipped, because the NSGA-II implementation always 
    ''' treats a smaller objective value as a better one.
    ''' </remarks>
    Public Class SingleObjectiveAdapter(Of Chr) : Implements IMultiObjective(Of Chr)

        ReadOnly fitness As Fitness(Of Chr)
        ReadOnly orientation As Double

        ''' <summary>
        ''' Create the multi-objective adapter of a single objective fitness 
        ''' function.
        ''' </summary>
        ''' <param name="fitness">The single objective fitness model.</param>
        ''' <param name="maximize">
        ''' Does the target fitness value should be maximized? 
        ''' The default value is ``False`` which means the fitness value will be 
        ''' minimized, just like the classic genetic algorithm does.
        ''' </param>
        Public Sub New(fitness As Fitness(Of Chr), Optional maximize As Boolean = False)
            If fitness Is Nothing Then
                Throw New ArgumentNullException(NameOf(fitness))
            End If

            Me.fitness = fitness
            Me.orientation = If(maximize, -1, 1)
        End Sub

        ''' <summary>
        ''' The number of the objective functions which is always ``1`` for the 
        ''' single objective adapter.
        ''' </summary>
        ''' <returns>Always returns ``1``.</returns>
        Public ReadOnly Property Objectives As Integer Implements IMultiObjective(Of Chr).Objectives
            Get
                Return 1
            End Get
        End Property

        ''' <summary>
        ''' The cacheable flag of the wrapped <c>Fitness(Of Chr)</c> model.
        ''' </summary>
        ''' <returns>A <see cref="Boolean"/> value.</returns>
        Public ReadOnly Property Cacheable As Boolean Implements IMultiObjective(Of Chr).Cacheable
            Get
                Return fitness.Cacheable
            End Get
        End Property

        ''' <summary>
        ''' Evaluate the fitness value and wrap it as a single element objective 
        ''' vector.
        ''' </summary>
        ''' <param name="chromosome">The candidate chromosome which will be evaluated.</param>
        ''' <param name="parallel">Whether the fitness calculation should be run in parallel mode?</param>
        ''' <returns>
        ''' A single element objective vector which is created from the 
        ''' <c>Fitness(Of Chr).Calculate</c> function.
        ''' </returns>
        Public Function Evaluate(chromosome As Chr, parallel As Boolean) As Double() Implements IMultiObjective(Of Chr).Evaluate
            Return {orientation * fitness.Calculate(chromosome, parallel)}
        End Function
    End Class
End Namespace
