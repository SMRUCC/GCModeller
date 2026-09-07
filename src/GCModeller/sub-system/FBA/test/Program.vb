Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming
Imports SMRUCC.genomics.Analysis.FBA
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

Module Program
    Sub Main(args As String())
        Dim gem As VirtualCell = "N:\NVP\models\GEMs\Streptomyces botrytidirepellens NEAU-LD23.xml".LoadXml(Of VirtualCell)
        Dim metabolic As Equation() = gem.metabolismStructure.reactions.AsEnumerable.Select(Function(r) r.BuildEquation).ToArray
        Dim matrix = metabolic.BuildMatrix
        Dim resolve As LPPSolution = New LinearProgrammingEngine().Run(matrix)



        Pause()
    End Sub
End Module
