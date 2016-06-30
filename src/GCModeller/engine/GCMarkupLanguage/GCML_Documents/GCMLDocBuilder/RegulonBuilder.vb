Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Terminal.stdio

Namespace Builder

    Public Class RegulonBuilder : Inherits IBuilder

        Dim Data As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject()

        Sub New(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel, DataPackage As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, Optional FirstLineTitle As Boolean = True)
            MyBase.New(MetaCyc, Model)
            If FirstLineTitle Then
                Data = DataPackage.Skip(1)
            Else
                Data = DataPackage.ToArray
            End If
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            Dim LQuery = (From row In Data Select GCML_Documents.XmlElements.Bacterial_GENOME.Regulon.Create(row, MyBase.Model)).ToArray
            Model.BacteriaGenome.Regulons = LQuery.ToArray
            Return MyBase.Model
        End Function
    End Class
End Namespace