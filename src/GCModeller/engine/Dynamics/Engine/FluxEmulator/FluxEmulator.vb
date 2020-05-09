Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Namespace Engine

    Public Class FluxEmulator : Implements ITaskDriver

        ''' <summary>
        ''' A snapshot of the compounds mass
        ''' </summary>
        Protected mass As MassTable
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
            Return mass.GetByKey(names)
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
                Call massSnapshotDriver(i, mass.GetMassValues)
                Call fluxSnapshotDriver(i, flux.getFlux)

                Call tick(i)
            Next
        End Sub
    End Class
End Namespace