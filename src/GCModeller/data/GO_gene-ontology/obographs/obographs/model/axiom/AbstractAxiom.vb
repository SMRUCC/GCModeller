#Region "Microsoft.VisualBasic::c1d379c52efe169179987d90c3839c29, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\model\axiom\AbstractAxiom.vb"

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

Namespace org.geneontology.obographs.model.axiom




	Public MustInherit Class AbstractAxiom
		Implements Axiom

		Protected Friend Sub New(builder As org.geneontology.obographs.model.axiom.EquivalentNodesSet.Builder)
			meta = builder.meta
		End Sub

		Protected Friend ReadOnly meta As org.geneontology.obographs.model.Meta





		''' <returns> the meta </returns>
		Public Overridable Property Meta As org.geneontology.obographs.model.Meta Implements Axiom.getMeta
			Get
				Return meta
			End Get
		End Property


		Public Class Builder

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___meta As org.geneontology.obographs.model.Meta

			Public Overridable Function meta(___meta As org.geneontology.obographs.model.Meta) As org.geneontology.obographs.model.axiom.EquivalentNodesSet.Builder
				Me.___meta = ___meta
				Return Me
			End Function


		End Class

	End Class

End Namespace
