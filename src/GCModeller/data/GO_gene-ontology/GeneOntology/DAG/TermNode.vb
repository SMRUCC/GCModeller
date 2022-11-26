#Region "Microsoft.VisualBasic::ebb44c932e84e259bd34aed71b37b3d8, GCModeller\data\GO_gene-ontology\GeneOntology\DAG\TermNode.vb"

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

    '   Total Lines: 34
    '    Code Lines: 18
    ' Comment Lines: 11
    '   Blank Lines: 5
    '     File Size: 1.20 KB


    '     Class TermNode
    ' 
    '         Properties: [namespace], GO_term, id, is_a, relationship
    '                     synonym, xref
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Namespace DAG

    ''' <summary>
    ''' The DAG node of a specific GO <see cref="Term"/>
    ''' </summary>
    Public Class TermNode : Implements INamedValue

        Public Property xref As NamedValue(Of String)()
        ''' <summary>
        ''' 当前的这个term是子节点，则这个属性内的所有的节点都是这个节点的父节点
        ''' </summary>
        ''' <returns></returns>
        Public Property is_a As is_a()
        Public Property synonym As synonym()
        Public Property relationship As Relationship()

        ''' <summary>
        ''' <see cref="Term.id"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property id As String Implements INamedValue.Key
        Public Property [namespace] As String
        Public Property GO_term As Term

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
