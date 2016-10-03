#Region "Microsoft.VisualBasic::7aad1c844b42a874628e169ba3df550d, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\model\GraphDocument.vb"

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

Imports System.Collections.Generic

Namespace org.geneontology.obographs.model

    ' Basic Obo Graph Model
    ' 
    ' Corresponds to the subset of OWL consisting of SubClassOf axioms between named classes
    ' and either named classes or simple existential restrictions
    ' 
    ' @author cjm
    ' 

    ''' <summary>
    ''' Holds a collection of graphs, plus document-level metadata
    ''' 
    ''' @author cjm
    ''' 
    ''' </summary>
    Public Class GraphDocument

		Private Sub New(builder As Builder)
			meta = builder.meta
			graphs = builder.graphs
			context = builder.context
		End Sub

		Private ReadOnly graphs As IList(Of Graph)
		Private ReadOnly meta As Meta

		''' <summary>
		''' The JSON-LD context for this document. This needs to be an unstructured
		''' Object, since it could be either a list or a map. We don't want to store
		''' it here as a Context because we want to roundtrip it the way it is written
		''' in the document.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private ReadOnly context As Object


		''' <returns> the graphs </returns>
		Public Overridable Property Graphs As IList(Of Graph)
			Get
				Return graphs
			End Get
		End Property



		''' <returns> the meta </returns>
		Public Overridable Property Meta As Meta
			Get
				Return meta
			End Get
		End Property






		Public Class Builder

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___meta As Meta
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___graphs As IList(Of Graph)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ___context As Object



			Public Overridable Function meta(___meta As Meta) As Builder
				Me.___meta = ___meta
				Return Me
			End Function
			Public Overridable Function graphs(___graphs As IList(Of Graph)) As Builder
				Me.___graphs = ___graphs
				Return Me
			End Function
			Public Overridable Function context(___context As Object) As Builder
				Me.___context = ___context
				Return Me
			End Function

			Public Overridable Function build() As GraphDocument
				Return New GraphDocument(Me)
			End Function

		End Class

	End Class

End Namespace
