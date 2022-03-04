Imports System.Runtime.CompilerServices

Public NotInheritable Class ElementFactory

    Private Sub New()
    End Sub

    Shared ReadOnly registry As New Dictionary(Of Type, Func(Of ValueString, Object))

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub Register(Of T)(factory As Func(Of ValueString, Object))
        registry(GetType(T)) = factory
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function CastTo(value As ValueString, template As Type) As Object
        Return registry(template)(value)
    End Function
End Class
