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
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports std = System.Math

Namespace Darwinism.NSGA2

    ''' <summary>
    ''' Evaluate the objective vector of the chromosomes and cache the results 
    ''' in this pool, so that the repeated chromosomes will not be evaluated 
    ''' again during the evolution iterations.
    ''' </summary>
    ''' <typeparam name="Chr">
    ''' The chromosome type which has an unique <see cref="IReadOnlyId.Identity"/> 
    ''' value for the cache key.
    ''' </typeparam>
    ''' <remarks>
    ''' Different from the <see cref="Darwinism.Models.GeneralFitnessPool(Of Individual)"/> 
    ''' which caches a single <see cref="Double"/> value, this pool caches the 
    ''' whole objective vectors. The objective vector will be cloned when it is 
    ''' written into the cache table and when it is read from the cache table, 
    ''' because the sorting procedure may modify the returned vector in place.
    ''' </remarks>
    Public Class ObjectivePool(Of Chr As IReadOnlyId) : Implements IMultiObjective(Of Chr)

        ''' <summary>
        ''' The maximum number of the cached objective vectors, the oldest 
        ''' records will be removed once this capacity is reached.
        ''' </summary>
        Protected Friend maxCapacity%
        ''' <summary>
        ''' The objective vector cache pool which is indexed via the unique 
        ''' identity of the target chromosome.
        ''' </summary>
        Protected Friend ReadOnly cache As New Dictionary(Of String, Double())

        ReadOnly inner As IMultiObjective(Of Chr)
        ReadOnly cacheEnabled As Boolean

        ''' <summary>
        ''' Create a objective vector cache pool.
        ''' </summary>
        ''' <param name="objectives">The raw multi-objective evaluation model.</param>
        ''' <param name="cacheSize">
        ''' The maximum number of the cached objective vectors; a value which is 
        ''' not greater than zero will disable the cache table, and the evaluation 
        ''' will be delegated to the <paramref name="objectives"/> model directly.
        ''' </param>
        Public Sub New(objectives As IMultiObjective(Of Chr), Optional cacheSize% = 0)
            If objectives Is Nothing Then
                Throw New ArgumentNullException(NameOf(objectives))
            End If

            Me.inner = objectives
            Me.maxCapacity = std.Max(cacheSize, 0)
            Me.cacheEnabled = cacheSize > 0 AndAlso objectives.Cacheable
        End Sub

        ''' <summary>
        ''' The number of the objective functions which was defined by the 
        ''' wrapped evaluation model.
        ''' </summary>
        ''' <returns>An <see cref="Integer"/> value.</returns>
        Public ReadOnly Property Objectives As Integer Implements IMultiObjective(Of Chr).Objectives
            Get
                Return inner.Objectives
            End Get
        End Property

        ''' <summary>
        ''' Is the cache table enabled?
        ''' </summary>
        ''' <returns>A <see cref="Boolean"/> value.</returns>
        Public ReadOnly Property Cacheable As Boolean Implements IMultiObjective(Of Chr).Cacheable
            Get
                Return cacheEnabled
            End Get
        End Property

        ''' <summary>
        ''' Evaluate the objective vector of the given chromosome, the cached 
        ''' result will be returned directly when the cache table is enabled and 
        ''' the target chromosome was evaluated before.
        ''' </summary>
        ''' <param name="chromosome">The candidate chromosome which will be evaluated.</param>
        ''' <param name="parallel">Whether the evaluation should be run in parallel mode?</param>
        ''' <returns>
        ''' A new objective vector instance of the given <paramref name="chromosome"/>.
        ''' </returns>
        Public Function Evaluate(chromosome As Chr, parallel As Boolean) As Double() Implements IMultiObjective(Of Chr).Evaluate
            If Not cacheEnabled Then
                Return inner.Evaluate(chromosome, parallel)
            End If

            Dim key As String = chromosome.Identity

            If String.IsNullOrEmpty(key) Then
                Return inner.Evaluate(chromosome, parallel)
            End If

            SyncLock cache
                Dim cached As Double() = Nothing

                If cache.TryGetValue(key, cached) Then
                    Return DirectCast(cached.Clone, Double())
                End If
            End SyncLock

            Dim value As Double() = DirectCast(inner.Evaluate(chromosome, parallel).Clone, Double())

            SyncLock cache
                cache(key) = value

                If cache.Count > maxCapacity Then
                    Call trimCache()
                End If
            End SyncLock

            Return DirectCast(value.Clone, Double())
        End Function

        ''' <summary>
        ''' Remove the oldest cache records, keep only the newest 
        ''' ``maxCapacity * 0.9`` records.
        ''' </summary>
        Private Sub trimCache()
            Dim keeps% = CInt(maxCapacity * 0.9)
            Dim removes As String() = cache.Keys _
                .Take(cache.Count - keeps) _
                .ToArray

            For Each key As String In removes
                Call cache.Remove(key)
            Next
        End Sub

        ''' <summary>
        ''' Clear all of the cached objective vectors.
        ''' </summary>
        Public Sub Clear()
            SyncLock cache
                Call cache.Clear()
            End SyncLock
        End Sub
    End Class
End Namespace
