Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords

Public Module PdbQt

    <Extension>
    Public Function GetLigandReference(pdb As PDB) As NamedValue(Of Het.HETRecord)
        Dim model = pdb.AtomStructures.FirstOrDefault

        If model Is Nothing Then
            Return Nothing
        End If

        Dim hetatoms = model.HetAtoms
        Dim name As String = hetatoms.Keys.First
        Dim atoms = hetatoms(name)
        Dim ref As New Het.HETRecord With {
            .AtomCount = atoms.Length,
            .ChainID = atoms.First.ChainID,
            .ResidueType = atoms.First.ResidueName,
            .SequenceNumber = atoms.First.ResidueSequenceNumber
        }

        Return New NamedValue(Of Het.HETRecord)(name, ref)
    End Function
End Module
