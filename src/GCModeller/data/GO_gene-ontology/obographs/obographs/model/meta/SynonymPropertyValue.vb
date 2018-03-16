#Region "Microsoft.VisualBasic::9a6b765aab5864aeb789ab9c7234a88d, data\GO_gene-ontology\obographs\obographs\model\meta\SynonymPropertyValue.vb"

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

    ' 	Class SynonymPropertyValue
    ' 
    ' 
    ' 		Enum SCOPES
    ' 
    ' 		    			BROAD, 			EXACT, 			NARROW, 			RELATED
    ' 
    ' 
    ' 
    ' 		Enum PREDS
    ' 
    ' 		    			hasBroadSynonym, 			hasExactSynonym, 			hasNarrowSynonym, 			hasRelatedSynonym
    ' 
    '  
    ' 
    ' 
    ' 
    ' 		Class Builder
    ' 
    ' 		    Properties: Exact, Types
    ' 
    ' 		    Constructor: (+1 Overloads) Sub New
    ' 		    Function: addType, build, scope, val, xrefs
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

Namespace org.geneontology.obographs.model.meta





	''' <summary>
	''' A <seealso cref="PropertyValue"/> that represents a an alternative term for a node
	''' 
	''' @author cjm
	''' 
	''' </summary>

	Public Class SynonymPropertyValue
		Inherits AbstractPropertyValue
		Implements PropertyValue

		''' <summary>
		''' OBO-style synonym scopes
		''' 
		''' @author cjm
		''' 
		''' </summary>
		Public Enum SCOPES
			EXACT
			NARROW
			BROAD
			RELATED
		End Enum

		''' <summary>
		''' properties from oboInOwl vocabulary that represent scopes
		''' 
		''' @author cjm
		''' 
		''' </summary>
		Public Enum PREDS
			hasExactSynonym
			hasNarrowSynonym
			hasBroadSynonym
			hasRelatedSynonym
		End Enum

		Private Sub New(builder As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder)
			MyBase.New(builder)
		End Sub

		''' <returns> true is scope equals EXACT -- convenience predicate </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property Exact As Boolean
			Get
				Return Pred.Equals(PREDS.hasExactSynonym.ToString())
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property Types As IList(Of String)
			Get
				If Meta IsNot Nothing Then Return Meta.Subsets
				Return New List(Of )
			End Get
		End Property


		Public Class Builder
			Inherits AbstractPropertyValue.Builder

			Public Overrides Function val(___val As String) As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder
				Return CType(MyBase.val(___val), org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder)
			End Function

			Public Overrides Function xrefs(___xrefs As IList(Of String)) As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder
				Return CType(MyBase.xrefs(___xrefs), org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder)
			End Function

			Public Overridable Function addType(type As String) As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder
				' TODO: decide on pattern for nested builders
				MyBase.meta((New org.geneontology.obographs.model.Meta.Builder).subsets(java.util.Collections.singletonList(type)).build())
				Return Me
			End Function

			Public Overridable Function scope(___scope As SCOPES) As org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder
				Dim pred As PREDS = PREDS.hasRelatedSynonym
				Select Case ___scope
				Case SCOPES.EXACT
					pred = PREDS.hasExactSynonym
				Case SCOPES.RELATED
					pred = PREDS.hasRelatedSynonym
				Case SCOPES.BROAD
					pred = PREDS.hasBroadSynonym
				Case SCOPES.NARROW
					pred = PREDS.hasNarrowSynonym

				End Select
				MyBase.pred(pred.ToString())
				Return Me

			End Function

			Public Overridable Function build() As SynonymPropertyValue
				Return New SynonymPropertyValue(Me)
			End Function
		End Class

	End Class

End Namespace
