#Region "Microsoft.VisualBasic::608439e402ed438fa4c7670e1f818497, data\RCSB PDB\PDB\Keywords\Atom.vb"

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

    '   Total Lines: 53
    '    Code Lines: 37 (69.81%)
    ' Comment Lines: 7 (13.21%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (16.98%)
    '     File Size: 1.81 KB


    '     Class Atom
    ' 
    '         Properties: AminoAcidSequenceData, Atoms, Keyword, ModelId
    ' 
    '         Function: Append, GetEnumerator, GetEnumerator1
    ' 
    '         Sub: Flush
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Keywords

    ''' <summary>
    ''' structure data model
    ''' </summary>
    Public Class Atom : Inherits Keyword
        Implements IEnumerable(Of AtomUnit)

        Public Property Atoms As AtomUnit()
        ''' <summary>
        ''' the model id
        ''' </summary>
        ''' <returns></returns>
        Public Property ModelId As String

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_ATOM
            End Get
        End Property

        Public ReadOnly Property AminoAcidSequenceData As AminoAcid()
            Get
                Return AminoAcid.SequenceGenerator(Me)
            End Get
        End Property

        Dim cache As New List(Of (key As Integer, value As String))

        Friend Shared Function Append(ByRef atoms As Atom, str As String) As Atom
            If atoms Is Nothing Then
                atoms = New Atom
            End If
            Dim index = str.GetTagValue(" ", trim:=True)
            atoms.cache.Add((CInt(Val(index.Name)), index.Value))
            Return atoms
        End Function

        Friend Overrides Sub Flush()
            Atoms = (From item In cache.AsParallel Let aa = AtomUnit.InternalParser(item.value, InternalIndex:=item.key) Where Not aa Is Nothing Select aa).ToArray
        End Sub

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
