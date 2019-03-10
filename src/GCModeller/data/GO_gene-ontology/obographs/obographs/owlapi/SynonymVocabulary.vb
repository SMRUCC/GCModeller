#Region "Microsoft.VisualBasic::1f95e67781966ecceffb3d45d5931715, data\GO_gene-ontology\obographs\obographs\owlapi\SynonymVocabulary.vb"

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

    ' 	Class SynonymVocabulary
    ' 
    ' 	    Properties: IriToScopeMap
    ' 
    ' 	    Constructor: (+1 Overloads) Sub New
    ' 
    ' 	    Function: [get], contains
    ' 
    ' 	    Sub: [set], setDefaults
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

Namespace org.geneontology.obographs.owlapi



	Public Class SynonymVocabulary

		Friend iriToScopeMap As IDictionary(Of String, org.geneontology.obographs.model.meta.SynonymPropertyValue.SCOPES) = New Dictionary(Of String, org.geneontology.obographs.model.meta.SynonymPropertyValue.SCOPES)


		Public Sub New()
			MyBase.New()
			setDefaults()
		End Sub

		Public Overridable Sub setDefaults()
			[set]("http://www.geneontology.org/formats/oboInOwl#hasExactSynonym", org.geneontology.obographs.model.meta.SynonymPropertyValue.SCOPES.EXACT)
			[set]("http://www.geneontology.org/formats/oboInOwl#hasRelatedSynonym", org.geneontology.obographs.model.meta.SynonymPropertyValue.SCOPES.RELATED)
			[set]("http://www.geneontology.org/formats/oboInOwl#hasNarrowSynonym", org.geneontology.obographs.model.meta.SynonymPropertyValue.SCOPES.NARROW)
			[set]("http://www.geneontology.org/formats/oboInOwl#hasBroadSynonym", org.geneontology.obographs.model.meta.SynonymPropertyValue.SCOPES.BROAD)
		End Sub

		''' <returns> the iriToScopeMap </returns>
		Public Overridable Property IriToScopeMap As IDictionary(Of String, org.geneontology.obographs.model.meta.SynonymPropertyValue.SCOPES)
			Get
				Return iriToScopeMap
			End Get
		End Property

		Public Overridable Sub [set](iri As String, scope As org.geneontology.obographs.model.meta.SynonymPropertyValue.SCOPES)
			iriToScopeMap(iri) = scope
		End Sub
		Public Overridable Function [get](iri As String) As org.geneontology.obographs.model.meta.SynonymPropertyValue.SCOPES
			Return iriToScopeMap(iri)
		End Function
		Public Overridable Function contains(iri As String) As Boolean
			Return iriToScopeMap.ContainsKey(iri)
		End Function

	End Class

End Namespace
