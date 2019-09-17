#Region "Microsoft.VisualBasic::c7e2d24a4778e47d2aac9cdc49ce8feb, GO_gene-ontology\GeneOntology\Files\Obo\OntologyRelations.vb"

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

    '     Class OntologyRelations
    ' 
    '         Properties: is_a
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace OBO

    Public Class OntologyRelations

        ''' <summary>
        ''' ```
        ''' is_a: GO:0048311 ! mitochondrion distribution
        ''' ```
        ''' </summary>
        ''' <returns></returns>
        Public Property is_a As NamedValue(Of String)()

        Sub New(term As Term)
            is_a = term.is_a.Select(Function(s) s.GetTagValue(" ! ", trim:=True)).ToArray
        End Sub
    End Class
End Namespace
