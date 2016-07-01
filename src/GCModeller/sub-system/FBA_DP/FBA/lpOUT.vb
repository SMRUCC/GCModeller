Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Linq
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.AnalysisTools.ModelSolvers.FBA.FBA_OUTPUT

''' <summary>
''' 线性规划的最优解的输出
''' </summary>
Public Class lpOUT
    Public Property Objective As String
    Public Property FluxsDistribution As String()

    Public Function CreateDataFile(lpSolveRModel As lpSolveRModel) As TabularOUT()
        Printf("Generating the EXCEL file...")

        Dim LQuery = FluxsDistribution.ToArray(
            Function(id, i) New TabularOUT With {
                .Flux = Val(id),
                .Rxn = lpSolveRModel.fluxColumns(i)})
        Return LQuery
    End Function
End Class