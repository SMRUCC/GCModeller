#Region "Microsoft.VisualBasic::3dc8d3a132699ee01d3b16f5af0a1c19, analysis\ProteinTools\ProteinMatrix\FamilyCluster\ProteinFamily.vb"

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

    '   Total Lines: 44
    '    Code Lines: 18 (40.91%)
    ' Comment Lines: 18 (40.91%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (18.18%)
    '     File Size: 1.53 KB


    '     Class ProteinFamily
    ' 
    '         Properties: familyId, members, memberSequences, msa, reference
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace FamilyCluster

    ''' <summary>
    ''' a single protein family discovered by the unsupervised clustering pipeline
    ''' </summary>
    Public Class ProteinFamily

        ''' <summary>
        ''' the integer family id assigned by the Louvain community detection step
        ''' </summary>
        Public Property familyId As Integer

        ''' <summary>
        ''' the titles of every member protein sequence
        ''' </summary>
        Public Property members As String()

        ''' <summary>
        ''' the member protein sequences (title + sequence data)
        ''' </summary>
        Public Property memberSequences As FastaSeq()

        ''' <summary>
        ''' the selected reference sequence: the member with the fewest edits in the MSA
        ''' </summary>
        Public Property reference As FastaSeq

        ''' <summary>
        ''' the multiple sequence alignment of the family (may be nothing if the family has a single member)
        ''' </summary>
        Public Property msa As MSAOutput

        Public Overrides Function ToString() As String
            If reference Is Nothing Then
                Return $"family_{familyId} ({members.Length} members)"
            Else
                Return $"family_{familyId} ({members.Length} members, ref={reference.Title})"
            End If
        End Function
    End Class
End Namespace

