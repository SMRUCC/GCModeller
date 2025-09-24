
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords.Het

''' <summary>
''' Helper module for generate protein complex from the autodock vina docking result
''' </summary>
Public Class ComplexGenerator

    ReadOnly author As New Author With {.Name = {"GCMODELLER"}}
    ReadOnly header As Header
    ReadOnly ligand As HetName

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="prot_id">the docking source protein pdb object input</param>
    ''' <param name="ligand">the docking source ligand pdb object input</param>
    Sub New(prot_id As String, ligand As String)
        Me.header = New Header With {
            .[Date] = "1-JAN-99",
            .pdbID = prot_id,
            .Title = "AUTODOCK VINA PROTEIN DOCK COMPLEX"
        }
        Me.ligand = New HetName(New NamedValue(Of String)("CPD", ligand))
    End Sub

    ''' <summary>
    ''' split the vina docking result and combine with the source protein to generate the complex pdb file list
    ''' </summary>
    ''' <param name="dock_result"></param>
    ''' <returns>
    ''' a collection of the pdb content text data of the generated complex in different docking conformations
    ''' </returns>
    Public Iterator Function GetComplexList(dock_result As String, prot_pdbqt As String) As IEnumerable(Of String)
        Dim matches As String() = dock_result _
            .Matches("MODEL\s+\d+(.*?)ENDMDL") _
            .ToArray

        For Each model As String In matches
            Dim pdb As New StringBuilder
            Dim het As New HETRecord With {
                .ResidueType = "CPD",
                .ChainID = "A",
                .AtomCount = 0,
                .SequenceNumber = 0
            }

            Call pdb.AppendLine(header.ToPdbString)
            Call pdb.AppendLine(author.ToPDBAuthorFieldText)
            Call pdb.AppendLine(het.ToPdbHETLine)
            Call pdb.AppendLine(ligand.ToPDBText)
            Call pdb.AppendLine(model)
            Call pdb.AppendLine(prot_pdbqt)

            Yield pdb.ToString
        Next
    End Function

End Class
