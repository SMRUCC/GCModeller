Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Extensions
Imports SMRUCC.genomics.Assembly

Namespace Builder

    Public Class EnzymeActivityRegulator : Inherits IBuilder

        Sub New(MetaCyc As MetaCyc.File.FileSystem.DatabaseLoadder, Model As SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel)
            Call MyBase.New(MetaCyc, Model)
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            Return MyBase.Model
        End Function
    End Class
End Namespace