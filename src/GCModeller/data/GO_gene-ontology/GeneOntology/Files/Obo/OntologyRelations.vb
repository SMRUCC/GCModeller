#Region "Microsoft.VisualBasic::0715cd7600221c853b3484c447764288, ..\GCModeller\data\GO_gene-ontology\GeneOntology\Files\Obo\OntologyRelations.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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
            is_a = term.is_a.ToArray(Function(s) s.GetTagValue(" ! ", trim:=True))
        End Sub
    End Class
End Namespace
