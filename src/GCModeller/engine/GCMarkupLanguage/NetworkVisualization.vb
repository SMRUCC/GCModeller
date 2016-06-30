Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<PackageNamespace("Visual.Network")>
Public Module NetworkVisualization

    <ExportAPI("Metabolism.Network")>
    Public Function MetabolismNetwork(Model As BacterialModel) As String
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        For Each rxn In Model.Metabolism.MetabolismNetwork
            For Each sb In rxn.Reactants
                Call sBuilder.AppendLine(String.Format("{0}   consume {1}", rxn.Identifier, sb.Identifier))
            Next
            For Each sb In rxn.Products
                Call sBuilder.AppendLine(String.Format("{0}   produce {1}", rxn.Identifier, sb.Identifier))
            Next
            If rxn.Enzymes.IsNullOrEmpty Then
                Continue For
            End If
            For Each enz In rxn.Enzymes
                Call sBuilder.AppendLine(String.Format("{0}   catalyze    {1}", enz, rxn.Identifier))
            Next
        Next

        Return sBuilder.ToString
    End Function

    <ExportAPI("Expression.Network")>
    Public Function ExpressionNetwork(Model As BacterialModel) As String
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        For Each Gene In Model.BacteriaGenome.Genes
            '     Call sBuilder.AppendLine(String.Format("{0}   express {1}", Gene.UniqueId, Gene.TranslateProtein.UniqueId))
        Next

        Return sBuilder.ToString
    End Function

    <ExportAPI("Protein.Assembly")>
    Public Function ProteinAssemblies(Model As BacterialModel) As String
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        For Each assm In Model.ProteinAssemblies
            If assm.Products.Count = 1 Then
                For Each sb In assm.Reactants
                    Call sBuilder.AppendLine(String.Format("{0}   assemble    {1}", sb.Identifier, assm.Products.First))
                Next
            Else
                For Each sb In assm.Reactants
                    Call sBuilder.AppendLine(String.Format("{0}   consume {1}", assm.Identifier, sb.Identifier))
                Next
                For Each sb In assm.Products
                    Call sBuilder.AppendLine(String.Format("{0}   produce {1}", assm.Identifier, sb.Identifier))
                Next
            End If

            If assm.Enzymes.IsNullOrEmpty Then
                Continue For
            End If
            For Each enz In assm.Enzymes
                Call sBuilder.AppendLine(String.Format("{0}   catalyze    {1}", enz, assm.Identifier))
            Next
        Next

        Return sBuilder.ToString
    End Function

    <ExportAPI("Network.Doc")>
    Public Function GetNetwork(Model As BacterialModel) As String
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        Call sBuilder.AppendLine(NetworkVisualization.MetabolismNetwork(Model))
        Call sBuilder.AppendLine(NetworkVisualization.ExpressionNetwork(Model))
        Call sBuilder.AppendLine(NetworkVisualization.ProteinAssemblies(Model))

        Return sBuilder.ToString
    End Function
End Module
