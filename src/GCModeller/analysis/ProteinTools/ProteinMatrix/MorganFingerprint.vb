#Region "Microsoft.VisualBasic::3574b806e0ed31b1a649679422142dc9, analysis\ProteinTools\ProteinMatrix\MorganFingerprint.vb"

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

    '   Total Lines: 26
    '    Code Lines: 20 (76.92%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (23.08%)
    '     File Size: 908 B


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

Public Class MorganFingerprint : Inherits GraphMorganFingerprint(Of KmerNode, KmerEdge)

    Public Sub New(size As Integer)
        MyBase.New(size)
    End Sub

    Protected Overrides Function HashAtom(v As KmerNode) As Integer
        Return KMerGraph.HashKMer(v.Type)
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
