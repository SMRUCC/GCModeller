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