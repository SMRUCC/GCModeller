#Region "Microsoft.VisualBasic::a87c414f1cbb36fba312a9e73b70f7b4, ..\GCModeller\data\GO_gene-ontology\GeneOntology\DAG\Term.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace DAG

    Public Class Term : Implements INamedValue

        Public Property xref As NamedValue(Of String)()
        ''' <summary>
        ''' 当前的这个term是子节点，则这个属性内的所有的节点都是这个节点的父节点
        ''' </summary>
        ''' <returns></returns>
        Public Property is_a As is_a()
        Public Property synonym As synonym()
        Public Property relationship As Relationship()
        Public Property id As String Implements INamedValue.Key
        Public Property [namespace] As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
