Imports System.Runtime.CompilerServices

Namespace Engine

    Public Class DebuggerView

        ReadOnly engine As Engine

        Public ReadOnly Property mass As MassTable
            Get
                Return engine.mass
            End Get
        End Property

        Public ReadOnly Property dataStorageDriver As IOmicsDataAdapter
            Get
                Return engine.dataStorageDriver
            End Get
        End Property

#Region "Debug views"

        Public ReadOnly Property viewTranscriptome As Dictionary(Of String, Double)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return mass _
                    .GetByKey(dataStorageDriver.mass.transcriptome) _
                    .ToDictionary(Function(mass) mass.ID,
                                  Function(mass)
                                      Return mass.Value
                                  End Function)
            End Get
        End Property

        Public ReadOnly Property viewProteome As Dictionary(Of String, Double)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return mass _
                    .GetByKey(dataStorageDriver.mass.proteome) _
                    .ToDictionary(Function(mass) mass.ID,
                                  Function(mass)
                                      Return mass.Value
                                  End Function)
            End Get
        End Property

        Public ReadOnly Property viewMetabolome As Dictionary(Of String, Double)
            Get
                Return mass _
                    .GetByKey(dataStorageDriver.mass.metabolome) _
                    .ToDictionary(Function(mass) mass.ID,
                                  Function(mass)
                                      Return mass.Value
                                  End Function)
            End Get
        End Property

#End Region

        Sub New(engine As Engine)
            Me.engine = engine
        End Sub
    End Class
End Namespace