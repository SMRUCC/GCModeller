Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Molecule

Module Extensions

    <Extension>
    Public Iterator Function variables(massTable As MassTable, complex As Protein) As IEnumerable(Of Variable)
        For Each compound In complex.compounds
            Yield massTable.variable(compound)
        Next
        For Each peptide In complex.polypeptides
            Yield massTable.variable(peptide)
        Next
    End Function
End Module
