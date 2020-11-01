Imports System.Runtime.CompilerServices

Namespace Graphics
    Public Module REngineExtension
        <Extension()>
        Public Sub Install(ByVal engine As REngine, ByVal device As IGraphicsDevice)
            Dim adapter = New GraphicsDeviceAdapter(device)
            AddHandler engine.Disposing, Sub(sender, e) adapter.Dispose()
            adapter.SetEngine(engine)
        End Sub
    End Module
End Namespace
