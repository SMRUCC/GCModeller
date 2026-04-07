#Region "Microsoft.VisualBasic::335d6fbdc52037d6be89b0dd35a70949, engine\IO\GCMarkupLanguage\v2\Xml\Genomics\protein.vb"

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

    '   Total Lines: 52
    '    Code Lines: 40 (76.92%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (23.08%)
    '     File Size: 1.85 KB


    '     Class protein
    ' 
    '         Properties: cellular_location, ligand, name, note, peptide_chains
    '                     protein_id
    ' 
    '         Function: ProteinRoutine
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Namespace v2

    Public Class protein

        <XmlAttribute> Public Property protein_id As String

        <XmlAttribute> Public Property name As String

        <XmlElement> Public Property ligand As String()
        <XmlElement> Public Property peptide_chains As String()

        <XmlAttribute> Public Property cellular_location As String

        Public Property note As String

        Public Shared Iterator Function ProteinRoutine(list As protein(), protein_id As String, visited As Index(Of String)) As IEnumerable(Of protein)
            If Not protein_id Like visited Then
                Dim direct As protein() = list _
                    .SafeQuery _
                    .AsParallel _
                    .Where(Function(prot) prot.protein_id = protein_id) _
                    .ToArray
                Dim complex As protein() = list _
                    .SafeQuery _
                    .AsParallel _
                    .Where(Function(prot)
                               Return (Not prot.peptide_chains.IsNullOrEmpty) AndAlso
                                   prot.peptide_chains.Contains(protein_id)
                           End Function) _
                    .ToArray

                Call visited.Add(protein_id)

                For Each prot As protein In direct
                    Yield prot
                Next
                For Each prot As protein In complex
                    Yield prot

                    For Each find As protein In ProteinRoutine(list, prot.protein_id, visited)
                        Yield find
                    Next
                Next
            End If
        End Function

    End Class
End Namespace
