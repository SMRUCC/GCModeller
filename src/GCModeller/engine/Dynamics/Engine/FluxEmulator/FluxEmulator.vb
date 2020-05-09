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

        ''' <summary>
        ''' Data snapshot of current iteration.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property snapshot As (mass As Dictionary(Of String, Double), flux As Dictionary(Of String, Double))

        Sub New(maxTime As Integer, Optional resolution As Integer = 10000, Optional showProgress As Boolean = True)
            Me.showProgress = showProgress
            Me.maxTime = maxTime
            Me.resolution = resolution
        End Sub

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
                engine.Tick()
                ' and then populate result data snapshot
                _snapshot = (mass.GetMassValues, flux.getFlux)

                tick(i)
            Next
        End Sub
    End Class
End Namespace