#Region "Microsoft.VisualBasic::39fe09d001e15023dcc4d71fb6ecf714, data\RCSB PDB\PDBQt\ComplexGenerator.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 84
    '    Code Lines: 55 (65.48%)
    ' Comment Lines: 15 (17.86%)
    '    - Xml Docs: 93.33%
    ' 
    '   Blank Lines: 14 (16.67%)
    '     File Size: 3.15 KB


    ' Class ComplexGenerator
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateComplexList
    ' 
    ' /********************************************************************************/

#End Region


Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords.Het
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords.HETATM

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
    Public Iterator Function CreateComplexList(dock_result As String, prot_pdbqt As String) As IEnumerable(Of String)
        Dim matches As String() = dock_result _
            .Matches("MODEL\s+\d+(.*?)ENDMDL") _
            .ToArray
        Dim line_val As String

        For Each model As String In matches
            Dim pdb As New StringBuilder
            Dim hetatoms As Atom = Nothing

            For Each line_str As String In model _
                .LineTokens _
                .Where(Function(line) line.StartsWith("HETATM"))

                line_val = line_str.GetTagValue(" ", trim:=False)
                HETATM.Append(hetatoms, line_val)
            Next

            Call pdb.AppendLine(header.ToPdbString)
            Call pdb.AppendLine(author.ToPDBAuthorFieldText)

            For Each key As String In hetatoms.HetAtoms.Keys
                Dim atoms As HETATMRecord() = hetatoms.HetAtoms(key)
                Dim het As New HETRecord With {
                    .ResidueType = key.Substring(0, 3),
                    .ChainID = "A",
                    .AtomCount = atoms.Length,
                    .SequenceNumber = atoms(Scan0).ResidueSequenceNumber
                }

                Call pdb.AppendLine(het.ToPdbHETLine)
                Call pdb.AppendLine(ligand.ToPDBText)
            Next
            For Each key As String In hetatoms.HetAtoms.Keys
                For Each atom As HETATMRecord In hetatoms.HetAtoms(key)
                    atom.ChainID = "A"
                    pdb.AppendLine(atom.ToString)
                Next
            Next

            Call pdb.AppendLine(prot_pdbqt)

            Yield pdb.ToString
        Next
    End Function

End Class

