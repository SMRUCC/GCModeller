
''' <summary>
''' Helper module for generate protein complex from the autodock vina docking result
''' </summary>
Public Class ComplexGenerator

    ReadOnly prot As PDB
    ReadOnly ligand As PDB

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="prot">the docking source protein pdb object input</param>
    ''' <param name="ligand">the docking source ligand pdb object input</param>
    Sub New(prot As PDB, ligand As PDB)
        Me.prot = prot
        Me.ligand = ligand
    End Sub

    ''' <summary>
    ''' split the vina docking result and combine with the source protein to generate the complex pdb file list
    ''' </summary>
    ''' <param name="dock_result"></param>
    ''' <returns>
    ''' a collection of the pdb content text data of the generated complex in different docking conformations
    ''' </returns>
    Public Iterator Function GetComplexList(dock_result As String) As IEnumerable(Of String)
        Dim matches As String() = dock_result _
            .Matches("MODEL\s+\d+(.*?)ENDMDL") _
            .ToArray

        For Each model As String In matches

        Next
    End Function

End Class
