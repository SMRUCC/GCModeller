#Region "Microsoft.VisualBasic::340e30587615f319cf66eb28f1c619dd, GCModeller\engine\Dynamics\Drivers\FluxEmulator.vb"

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

    '   Total Lines: 106
    '    Code Lines: 79
    ' Comment Lines: 6
    '   Blank Lines: 21
    '     File Size: 3.80 KB


    '     Class FluxEmulator
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: AttatchFluxDriver, AttatchMassDriver, getCore, getMass, Run
    ' 
    '         Sub: loopInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Namespace Engine

    Public Class FluxEmulator : Implements ITaskDriver

        ''' <summary>
        ''' The biological flux simulator engine core module
        ''' </summary>
        Protected core As Vessel
        Protected showProgress As Boolean = True
        Protected resolution As Integer
        Protected maxTime As Integer

        Dim massSnapshotDriver As SnapshotDriver
        Dim fluxSnapshotDriver As SnapshotDriver

        Sub New(Optional core As Vessel = Nothing,
                Optional maxTime As Integer = 50,
                Optional resolution As Integer = 10000,
                Optional showProgress As Boolean = True)

            Me.showProgress = showProgress
            Me.maxTime = maxTime
            Me.resolution = resolution

            If Not core Is Nothing Then
                Me.core = core
            Else
                Me.core = New Vessel
            End If
        End Sub

        Public Function AttatchMassDriver(driver As SnapshotDriver) As FluxEmulator
            massSnapshotDriver = driver
            Return Me
        End Function

        Public Function AttatchFluxDriver(driver As SnapshotDriver) As FluxEmulator
            fluxSnapshotDriver = driver
            Return Me
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function getCore() As Vessel
            Return core
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function getMass(names As IEnumerable(Of String)) As IEnumerable(Of Factor)
            Return names.Select(AddressOf core.m_massIndex.TryGetValue)
        End Function

        Public Overridable Function Run() As Integer Implements ITaskDriver.Run
            Dim tick As Action(Of Integer)
            Dim process As ProgressBar = Nothing
            Dim progress As ProgressProvider = Nothing

            If showProgress Then
                process = New ProgressBar("Running simulator...")
                progress = New ProgressProvider(process, resolution)

                tick = Sub(i)
                           Call ($"iteration: {i + 1}; ETA: {progress.ETA().FormatTime}") _
                               .DoCall(Sub(msg)
                                           Call process.SetProgress(progress.StepProgress, msg)
                                       End Sub)
                       End Sub
            Else
                tick = Sub()
                           ' do nothing
                       End Sub
            End If

            Call loopInternal(tick)

            If Not process Is Nothing Then
                Call process.Dispose()
            End If

            Return 0
        End Function

        Protected Overridable Sub loopInternal(tick As Action(Of Integer))
            Dim engine As SolverIterator = core.ContainerIterator(maxTime, resolution)
            Dim flux As New FluxAggregater(core)
            Dim iterations As Integer = resolution

            For i As Integer = 0 To iterations - 1
                ' run internal engine iteration
                Call engine.Tick()

                ' and then populate result data snapshot
                Call massSnapshotDriver(i, core.getMassValues)
                Call fluxSnapshotDriver(i, flux.getFlux)

                Call tick(i)
            Next
        End Sub
    End Class
End Namespace
