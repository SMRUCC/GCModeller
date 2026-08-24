#Region "Microsoft.VisualBasic::542a5d2c5991ce13c42531366283a40c, analysis\Metagenome\Metagenome\Tools\gast\TaxonomyTree\TaxonomySort.vb"

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

    '   Total Lines: 32
    '    Code Lines: 25 (78.12%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (21.88%)
    '     File Size: 852 B


    '     Class TaxonomySort
    ' 
    '         Properties: score, tax_id, taxonomy
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace gast

    Public Class TaxonomySort

        Public Property tax_id As String
        Public Property taxonomy As Metagenomics.Taxonomy
            Get
                Return _tax
            End Get
            Set(value As Metagenomics.Taxonomy)
                _tax = value
                _list = value.CreateTable.Value
            End Set
        End Property

        Public Property score As Double

        Dim _tax As Metagenomics.Taxonomy
        Dim _list As Dictionary(Of String, String)

        Default Public ReadOnly Property taxname(rank As String) As String
            Get
                Return _list(rank)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return taxonomy.ToString
        End Function

    End Class
End Namespace
