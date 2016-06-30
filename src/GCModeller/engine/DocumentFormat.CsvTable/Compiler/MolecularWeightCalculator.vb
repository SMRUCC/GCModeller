Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Namespace Compiler.Components

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MolecularWeightCalculator

        Public Sub CalculateK(ModelLoader As FileStream.IO.XmlresxLoader)
            Dim MetabolismModels = ModelLoader.MetabolismModel
            Dim Metabolites = ModelLoader.MetabolitesModel

            For i As Integer = 0 To MetabolismModels.Count - 1
                Dim FluxModel = MetabolismModels(i)
                Dim LEFT = (From item In FluxModel._Internal_compilerLeft Select Metabolites(item.Value).MolWeight).ToArray
                If (From n In LEFT Where n = 0.0R Select 1).ToArray.Count > 0 Then
                    Continue For
                End If
                Dim RIGHT = (From item In FluxModel._Internal_compilerRight Select Metabolites(item.Value).MolWeight).ToArray
                If (From n In RIGHT Where n = 0.0R Select 1).ToArray.Count > 0 Then
                    Continue For
                End If

                Dim nLEFT = LEFT.Average, nRIGHT = RIGHT.Average
                FluxModel.p_Dynamics_K_1 = nLEFT / nRIGHT
                FluxModel.p_Dynamics_K_2 = nRIGHT / nLEFT
            Next
        End Sub
    End Class
End Namespace