#Region "Microsoft.VisualBasic::54397e503f1e31989ff26816a690781a, analysis\ProteinTools\ProteinMatrix\MorganFingerprint.vb"

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

    '   Total Lines: 33
    '    Code Lines: 20 (60.61%)
    ' Comment Lines: 7 (21.21%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (18.18%)
    '     File Size: 1.12 KB


    ' Class MorganFingerprint
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: HashAtom, HashEdge
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.MorganFingerprint
Imports Microsoft.VisualBasic.Math.HashMaps
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure.Kmer

''' <summary>
''' help for make embedding a sequence to vector
''' </summary>
''' <remarks>
''' processing of the gene/protein sequence, not working well for 
''' the genomics complete sequence
''' </remarks>
Public Class MorganFingerprint : Inherits GraphMorganFingerprint(Of KmerNode, KmerEdge)

    Public Sub New(size As Integer)
        MyBase.New(size)
    End Sub

    Protected Overrides Function HashAtom(v As KmerNode) As ULong
        Return HashLabelKey(v.Type)
    End Function

    Protected Overrides Function HashEdge(atoms() As KmerNode, e As KmerEdge, flip As Boolean) As ULong
        Dim hashcode As ULong

        If flip Then
            hashcode = HashMap.HashCodePair(atoms(e.U).Code, atoms(e.V).Code)
        Else
            hashcode = HashMap.HashCodePair(atoms(e.V).Code, atoms(e.U).Code)
        End If

        Return hashcode Xor CULng(e.NSize)
    End Function
End Class
