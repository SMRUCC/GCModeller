#Region "Microsoft.VisualBasic::fa65318de212320d007ba27336f21080, engine\AutoCAD\GeneticComponents\UniProtExtensions.vb"

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

    ' Module UniProtExtensions
    ' 
    '     Function: CreateDump
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.SequenceModel.Polypeptides

''' <summary>
''' Create genetic components from Uniprot database.
''' </summary>
Public Module UniProtExtensions

    <Extension>
    Public Iterator Function CreateDump(uniprot As IEnumerable(Of entry)) As IEnumerable(Of GeneticNode)
        For Each protein As entry In uniprot.Where(Function(g) Not g.sequence Is Nothing)
            Dim KO As dbReference = protein.KO

            If KO Is Nothing Then
                Continue For
            End If

            Dim ntAccess As [property] = protein.Xrefs.TryGetValue("RefSeq") _
                ?.FirstOrDefault _
                ?.properties _
                 .FirstOrDefault(Function(p) p.type = "nucleotide sequence ID")

            Yield New GeneticNode With {
                .KO = KO.id,
                .[Function] = protein.proteinFullName,
                .ID = protein.accessions.First,
                .Sequence = Polypeptide _
                    .ConstructVector(protein.ProteinSequence) _
                    .ToArray,
                .GO = protein.Xrefs _
                    .TryGetValue("GO", [default]:={}) _
                    .Select(Function(g) g.id) _
                    .ToArray,
                .Xref = .ID,
                .Accession = ntAccess?.value,
                .Nt = {}
            }
        Next
    End Function
End Module
