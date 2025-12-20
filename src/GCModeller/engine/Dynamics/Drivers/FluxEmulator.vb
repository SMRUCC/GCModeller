#Region "Microsoft.VisualBasic::47278ae3f42433813f372adddb4aa97c, engine\Dynamics\Drivers\FluxEmulator.vb"

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

'   Total Lines: 108
'    Code Lines: 77 (71.30%)
' Comment Lines: 10 (9.26%)
'    - Xml Docs: 60.00%
' 
'   Blank Lines: 21 (19.44%)
'     File Size: 3.79 KB


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
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports std = System.Math

Namespace Engine

    ''' <summary>
    ''' a container of <see cref="Vessel"/> as simulation core.
    ''' </summary>
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
        Dim forwardSnapshot As SnapshotDriver
        Dim reverseSnapshot As SnapshotDriver

        Sub New(Optional core As Vessel = Nothing,
                Optional maxTime As Integer = 50,
                Optional resolution As Integer = 10000,
                Optional showProgress As Boolean = True,
                Optional n_threads As Integer = 8,
                Optional debug As Boolean = False)

            Me.showProgress = showProgress
            Me.maxTime = maxTime
            Me.resolution = resolution

            If Not core Is Nothing Then
                Me.core = core
            Else
                Me.core = New Vessel(n_threads, is_debug:=debug)
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

        Public Function AttachRegulationDriver(forward As SnapshotDriver, reverse As SnapshotDriver) As FluxEmulator
            forwardSnapshot = forward
            reverseSnapshot = reverse
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
            Dim tick As RunSlavePipeline.SetProgressEventHandler
            Dim process As Tqdm.ProgressBar = Nothing

            If showProgress AndAlso App.EnableTqdm Then
                process = New Tqdm.ProgressBar(total:=resolution, useColor:=True)
                tick = AddressOf process.Progress
            Else
                ' i is the current iteration number
                tick = Sub(i, message)
                           Call $"{(i / resolution * 100).ToString("F2")}% {message}".info
                       End Sub
            End If

            Call loopInternal(tick)

            If Not process Is Nothing Then
                Call process.Finish()
            End If

            Call "run experiment finished!".info

            Return 0
        End Function

        Protected Overridable Sub loopInternal(tick As RunSlavePipeline.SetProgressEventHandler)
            Dim engine As SolverIterator = core.ContainerIterator(maxTime, resolution)
            Dim flux As New FluxAggregater(core)
            Dim iterations As Integer = resolution
            Dim summary As New StringBuilder
            Dim fluxNames As Dictionary(Of String, String) = flux.GetFluxNames

            For i As Integer = 0 To iterations - 1
                ' run internal engine iteration
                Call engine.Tick()

                ' clip mass values, keeps positive
                For Each factor As Factor In core.m_massIndex.Values
                    If factor.Value < 0 Then
                        Call factor.reset(0)
                    End If
                Next

                Dim fluxData As Dictionary(Of String, Double) = flux.getFlux
                Dim abs As Double() = fluxData.Values _
                    .Select(Function(xi) std.Abs(xi)) _
                    .ToArray
                Dim max As Integer = which.Max(abs)
                Dim maxKey As String = fluxData.Keys(which.Max(abs))

                ' and then populate result data snapshot
                Call massSnapshotDriver(i, core.getMassValues)
                Call fluxSnapshotDriver(i, fluxData)

                summary.Clear()
                summary.AppendFormat("total_loads: {0} ", abs.Sum.ToString("F3"))
                summary.AppendFormat("mean_loads: {0} ", abs.Average.ToString("F3"))
                summary.AppendFormat("max_loads_flux: {0}={1}", If(fluxNames.TryGetValue(maxKey), maxKey), fluxData(maxKey).ToString("F4"))

                With flux.getRegulations
                    Call forwardSnapshot(i, .forward)
                    Call reverseSnapshot(i, .reverse)
                End With

                Call tick(i, summary.ToString)
            Next
        End Sub
    End Class
End Namespace
