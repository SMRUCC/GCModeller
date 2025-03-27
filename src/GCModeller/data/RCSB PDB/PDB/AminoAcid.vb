#Region "Microsoft.VisualBasic::0cc1fb652a5fb43544df946fbed61097, data\RCSB PDB\PDB\AminoAcid.vb"

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

    '   Total Lines: 39
    '    Code Lines: 26 (66.67%)
    ' Comment Lines: 10 (25.64%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (7.69%)
    '     File Size: 1.33 KB


    ' Class AminoAcid
    ' 
    '     Properties: AA_ID, Atoms, Carbon, Index
    ' 
    '     Function: SequenceGenerator
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 氨基酸残基
''' </summary>
''' <remarks></remarks>
Public Class AminoAcid

    Public Property Index As Integer
    Public Property AA_ID As String
    Public Property Atoms As Keywords.AtomUnit()

    ''' <summary>
    ''' 中心的碳原子
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Carbon As Keywords.AtomUnit
        Get
            Dim CLQuery = (From Atom In Atoms Where String.Equals(Atom.Atom, "C") Select Atom).FirstOrDefault
            If CLQuery Is Nothing Then
                Return Atoms.First
            Else
                Return CLQuery
            End If
        End Get
    End Property

    Public Shared Function SequenceGenerator(Atoms As Keywords.Atom) As AminoAcid()
        Dim Resource = (From Atom In Atoms Select Atom Group Atom By Atom.AA_IDX Into Group).ToArray
        Dim LQuery = (From item In Resource
                      Select AA = New AminoAcid With {
                          .Index = item.AA_IDX,
                          .AA_ID = item.Group.First.AA_ID,
                          .Atoms = item.Group.ToArray
                      }
                      Order By AA.Index Ascending).ToArray
        Return LQuery
    End Function
End Class
