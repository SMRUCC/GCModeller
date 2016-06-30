Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Extensions
Imports LANS.SystemsBiology.Assembly

Namespace Builder

    Public Class EnzymeActivityRegulator : Inherits IBuilder

        Sub New(MetaCyc As MetaCyc.File.FileSystem.DatabaseLoadder, Model As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel)
            Call MyBase.New(MetaCyc, Model)
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            Return MyBase.Model
        End Function
    End Class
End Namespace