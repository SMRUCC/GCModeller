Imports System.Text
Imports System.Runtime.CompilerServices

Namespace Builder

    Public Module ModelExport

        <Extension> Public Function ExportModel(Model As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Dim Csv As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Csv.AppendLine({"Id", "Name", "Equation", "Associate-Genes"})
            For i As Integer = 0 To Model.Metabolism.MetabolismNetwork.Count - 1
                Dim Reaction = Model.Metabolism.MetabolismNetwork(i)

                Call Csv.AppendLine({Reaction.Identifier, Reaction.Name, Reaction.Equation, GetAssociatedGenes(Model, Reaction)})
            Next

            Return Csv
        End Function

        <Extension> Public Function GetAssociatedGenes(Model As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel, Reaction As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction) As String
            'Dim Query = From Gene In Model.BacteriaGenome.Genes.AsParallel Where Gene._MetabolismNetwork.IndexOf(Reaction.Handle) > -1 Let Id = Gene.CommonName Select Id Order By Id Ascending  '
            'Dim GeneList = Query.ToArray

            'If GeneList.Count = 0 Then
            '    Return ""
            'End If

            'Dim sBuilder As StringBuilder = New StringBuilder(1024)
            'For Each Id In GeneList
            '    Call sBuilder.AppendFormat("{0}, ", Id)
            'Next
            'Call sBuilder.Remove(sBuilder.Length - 2, 2)

            'Return sBuilder.ToString
            Throw New NotImplementedException
        End Function
    End Module
End Namespace
