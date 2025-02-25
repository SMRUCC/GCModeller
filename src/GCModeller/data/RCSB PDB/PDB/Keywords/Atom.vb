#Region "Microsoft.VisualBasic::5daaf0eeb37e4f37dd2edd277ce4b29b, data\RCSB PDB\PDB\Keywords\Atom.vb"

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

    '     Class Atom
    ' 
    '         Properties: Atoms, Keyword
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetEnumerator, GetEnumerator1
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Keywords

    Public Class Atom : Inherits Keyword
        Implements IEnumerable(Of AtomUnit)

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Atoms = (From item In itemDatas.AsParallel Select AtomUnit.InternalParser(item.Value, InternalIndex:=item.Key)).ToArray
        End Sub

        Public Property Atoms As AtomUnit()

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_ATOM
            End Get
        End Property

        Public Iterator Function GetEnumerator() As IEnumerator(Of AtomUnit) Implements IEnumerable(Of AtomUnit).GetEnumerator
            For Each Atom As AtomUnit In Me.Atoms
                Yield Atom
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
