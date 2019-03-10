Namespace Graphics

    Public Module REngineExtension

        <System.Runtime.CompilerServices.Extension>
        Public Sub Install(engine As REngine, device As IGraphicsDevice)
            Dim adapter = New GraphicsDeviceAdapter(device)
            AddHandler engine.Disposing, Sub(sender, e) Call adapter.Dispose()
            adapter.SetEngine(engine)
        End Sub
    End Module
End Namespace