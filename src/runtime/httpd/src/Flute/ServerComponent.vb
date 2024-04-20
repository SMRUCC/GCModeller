Imports Flute.Http.Configurations

Public MustInherit Class ServerComponent

    Protected ReadOnly settings As Configuration

    Sub New(settings As Configuration)
        Me.settings = settings
    End Sub

End Class
