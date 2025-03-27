#Region "Microsoft.VisualBasic::1cd778f68e820a505a6d56dfd363c335, analysis\ProteinTools\ProteinMatrix\MorganFingerprint.vb"

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

    '   Total Lines: 25
    '    Code Lines: 19 (76.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (24.00%)
    '     File Size: 846 B


    ' Class MorganFingerprint
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: HashAtom, HashEdge
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.MorganFingerprint
Imports Microsoft.VisualBasic.Math.HashMaps

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

        Return hashcode Xor CULng(e.NSzie)
    End Function
End Class

