Namespace EngineSystem.RuntimeObjects

    ''' <summary>
    ''' I'm just kidding
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class RuntimeObject
        Implements IRuntimeObject

        Public ReadOnly Property Guid As Long Implements IRuntimeObject.Guid

        Sub New()
            _Guid = _GuidGenerator
            _GuidGenerator += 1
        End Sub

        Private Shared _GuidGenerator As Long = Long.MinValue
    End Class

    Public Interface IRuntimeObject
        ReadOnly Property Guid As Long
    End Interface
End Namespace