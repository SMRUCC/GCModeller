Imports System.Collections.Generic

Namespace org.geneontology.obographs.owlapi

    ' Mapping OGs to OWL and back
    ' 
    ' Currently only one direction implemented
    ' 
    ' <br/>
    ' 
    ' See <a href="https://github.com/geneontology/obographs/blob/master/README-owlmapping.md">Spec</a>
    ' 
    ' @author cjm


    ''' <summary>
    ''' Implements OWL to OG translation
    ''' ===
    ''' 
    ''' See <a href="https://github.com/geneontology/obographs/blob/master/README-owlmapping.md">OWL Mapping spec</a>
    ''' 
    ''' <br/>
    ''' Status: _currently incomplete_
    ''' <br/>
    ''' </summary>
    ''' <seealso cref= "[SPEC](https://github.com/geneontology/obographs/blob/master/README-owlmapping.md)"
    ''' 
    ''' TODO:
    '''  * Generate Meta objects
    '''  * Synonyms
    ''' 
    ''' @author cjm
    '''  </seealso>
    Public Class FromOwl

		Public Const SUBCLASS_OF As String = "is_a"

		Private prefixHelper As org.geneontology.obographs.io.PrefixHelper
		Private context As com.github.jsonldjava.core.Context

		''' 
		Public Sub New()
			prefixHelper = New org.geneontology.obographs.io.PrefixHelper
			context = prefixHelper.Context
		End Sub

		''' <param name="baseOntology"> </param>
		''' <returns> GraphDocument where each graph is an ontology in the ontology closure </returns>
		''' <seealso cref= <a href="https://github.com/geneontology/obographs/blob/master/README-owlmapping.md">OWL Mapping spec</a> </seealso>
		Public Overridable Function generateGraphDocument(baseOntology As org.semanticweb.owlapi.model.OWLOntology) As org.geneontology.obographs.model.GraphDocument
			Dim graphs As IList(Of org.geneontology.obographs.model.Graph) = New List(Of org.geneontology.obographs.model.Graph)
			For Each ont As org.semanticweb.owlapi.model.OWLOntology In baseOntology.ImportsClosure
				graphs.Add(generateGraph(ont))
			Next ont
			Return (New org.geneontology.obographs.model.GraphDocument.Builder).graphs(graphs).build()
		End Function

		''' <param name="ontology"> </param>
		''' <returns> Graph generated from ontology </returns>
		Public Overridable Function generateGraph(ontology As org.semanticweb.owlapi.model.OWLOntology) As org.geneontology.obographs.model.Graph

			Dim synonymVocabulary As New SynonymVocabulary

			Dim edges As IList(Of org.geneontology.obographs.model.Edge) = New List(Of org.geneontology.obographs.model.Edge)
			Dim nodes As IList(Of org.geneontology.obographs.model.Node) = New List(Of org.geneontology.obographs.model.Node)
			Dim ensets As IList(Of org.geneontology.obographs.model.axiom.EquivalentNodesSet) = New List(Of org.geneontology.obographs.model.axiom.EquivalentNodesSet)
			Dim ldas As IList(Of org.geneontology.obographs.model.axiom.LogicalDefinitionAxiom) = New List(Of org.geneontology.obographs.model.axiom.LogicalDefinitionAxiom)
			Dim nodeIds As java.util.Set(Of String) = New HashSet(Of String)
			Dim nodeTypeMap As IDictionary(Of String, org.geneontology.obographs.model.Node.RDFTYPES) = New Dictionary(Of String, org.geneontology.obographs.model.Node.RDFTYPES)
			Dim nodeLabelMap As IDictionary(Of String, String) = New Dictionary(Of String, String)

			' Each node can be built from multiple axioms; use a builder for each nodeId
			Dim nodeMetaBuilderMap As IDictionary(Of String, org.geneontology.obographs.model.Meta.Builder) = New Dictionary(Of String, org.geneontology.obographs.model.Meta.Builder)


			Dim untranslatedAxioms As java.util.Set(Of org.semanticweb.owlapi.model.OWLAxiom) = New HashSet(Of org.semanticweb.owlapi.model.OWLAxiom)

			' iterate over all axioms and push to relevant builders
			For Each ax As org.semanticweb.owlapi.model.OWLAxiom In ontology.Axioms

				Dim meta As org.geneontology.obographs.model.Meta = getAnnotations(ax)

				If TypeOf ax Is org.semanticweb.owlapi.model.OWLDeclarationAxiom Then
					Dim dax As org.semanticweb.owlapi.model.OWLDeclarationAxiom = (CType(ax, org.semanticweb.owlapi.model.OWLDeclarationAxiom))
					Dim e As org.semanticweb.owlapi.model.OWLEntity = dax.Entity
					If TypeOf e Is org.semanticweb.owlapi.model.OWLClass Then setNodeType(getClassId(CType(e, org.semanticweb.owlapi.model.OWLClass)), org.geneontology.obographs.model.Node.RDFTYPES.CLASS, nodeTypeMap)
				ElseIf TypeOf ax Is org.semanticweb.owlapi.model.OWLLogicalAxiom Then

					If TypeOf ax Is org.semanticweb.owlapi.model.OWLSubClassOfAxiom Then
						' SUBCLASS

						Dim sca As org.semanticweb.owlapi.model.OWLSubClassOfAxiom = CType(ax, org.semanticweb.owlapi.model.OWLSubClassOfAxiom)
						Dim subc As org.semanticweb.owlapi.model.OWLClassExpression = sca.SubClass
						Dim supc As org.semanticweb.owlapi.model.OWLClassExpression = sca.SuperClass
						If subc.Anonymous Then
							untranslatedAxioms.add(sca)
						Else
							Dim subj As String = getClassId(CType(subc, org.semanticweb.owlapi.model.OWLClass))
							setNodeType(subj, org.geneontology.obographs.model.Node.RDFTYPES.CLASS, nodeTypeMap)

							If supc.Anonymous Then
								Dim r As org.geneontology.obographs.model.axiom.ExistentialRestrictionExpression = getRestriction(supc)
								edges.Add(getEdge(subj, r.PropertyId, r.FillerId))
							Else
								edges.Add(getEdge(subj, SUBCLASS_OF, getClassId(CType(supc, org.semanticweb.owlapi.model.OWLClass))))

							End If
						End If
					ElseIf TypeOf ax Is org.semanticweb.owlapi.model.OWLEquivalentClassesAxiom Then
						' EQUIVALENT

						Dim eca As org.semanticweb.owlapi.model.OWLEquivalentClassesAxiom = CType(ax, org.semanticweb.owlapi.model.OWLEquivalentClassesAxiom)
						Dim xs As IList(Of org.semanticweb.owlapi.model.OWLClassExpression) = eca.ClassExpressionsAsList
						Dim anonXs As IList(Of org.semanticweb.owlapi.model.OWLClassExpression) = xs.stream().filter(x -> x.Anonymous).collect(java.util.stream.Collectors.toList())
						Dim namedXs As IList(Of org.semanticweb.owlapi.model.OWLClassExpression) = xs.stream().filter(x -> (Not x.Anonymous)).collect(java.util.stream.Collectors.toList())
						Dim xClassIds As java.util.Set(Of String) = namedXs.stream().map(x -> getClassId(CType(x, org.semanticweb.owlapi.model.OWLClass))).collect(java.util.stream.Collectors.toSet())
						If anonXs.Count = 0 Then
							' EquivalentNodesSet

							' all classes in equivalence axiom are named
							' TODO: merge pairwise assertions into a clique
							Dim enset As org.geneontology.obographs.model.axiom.EquivalentNodesSet = (New org.geneontology.obographs.model.axiom.EquivalentNodesSet.Builder).nodeIds(xClassIds).build()
							ensets.Add(enset)
						Else
							If anonXs.Count = 1 AndAlso namedXs.Count = 1 Then

								Dim anonX As org.semanticweb.owlapi.model.OWLClassExpression = anonXs(0)
								If TypeOf anonX Is org.semanticweb.owlapi.model.OWLObjectIntersectionOf Then
									' LDA

									Dim ixs As java.util.Set(Of org.semanticweb.owlapi.model.OWLClassExpression) = CType(anonX, org.semanticweb.owlapi.model.OWLObjectIntersectionOf).Operands

									Dim genusClassIds As IList(Of String) = New List(Of String)
									Dim restrs As IList(Of org.geneontology.obographs.model.axiom.ExistentialRestrictionExpression) = New List(Of org.geneontology.obographs.model.axiom.ExistentialRestrictionExpression)
									Dim isLDA As Boolean = True
									For Each ix As org.semanticweb.owlapi.model.OWLClassExpression In ixs
										If Not ix.Anonymous Then
											genusClassIds.Add(getClassId(CType(ix, org.semanticweb.owlapi.model.OWLClass)))
										ElseIf TypeOf ix Is org.semanticweb.owlapi.model.OWLObjectSomeValuesFrom Then
											restrs.Add(getRestriction(ix))
										Else
											isLDA = False
											Exit For
										End If

									Next ix

									If isLDA Then
										Dim lda As org.geneontology.obographs.model.axiom.LogicalDefinitionAxiom = (New org.geneontology.obographs.model.axiom.LogicalDefinitionAxiom.Builder).definedClassId(getClassId(CType(namedXs(0), org.semanticweb.owlapi.model.OWLClass))).genusIds(genusClassIds).restrictions(restrs).build()
										ldas.Add(lda)
									End If
								End If
							Else

							End If
						End If
					Else
						untranslatedAxioms.add(ax)
					End If
				Else
					' NON-LOGICAL AXIOMS
					If TypeOf ax Is org.semanticweb.owlapi.model.OWLAnnotationAssertionAxiom Then
						Dim aaa As org.semanticweb.owlapi.model.OWLAnnotationAssertionAxiom = CType(ax, org.semanticweb.owlapi.model.OWLAnnotationAssertionAxiom)
						Dim p As org.semanticweb.owlapi.model.OWLAnnotationProperty = aaa.Property
						Dim s As org.semanticweb.owlapi.model.OWLAnnotationSubject = aaa.Subject
						If TypeOf s Is org.semanticweb.owlapi.model.IRI Then

							Dim subj As String = getNodeId(CType(s, org.semanticweb.owlapi.model.IRI))

							Dim v As org.semanticweb.owlapi.model.OWLAnnotationValue = aaa.Value
							Dim lv As String = Nothing
							If TypeOf v Is org.semanticweb.owlapi.model.OWLLiteral Then lv = CType(v, org.semanticweb.owlapi.model.OWLLiteral).Literal
							Dim pIRI As org.semanticweb.owlapi.model.IRI = p.IRI
							If p.Label Then
								If lv IsNot Nothing Then
									nodeIds.add(subj)
									nodeLabelMap(subj) = lv
								End If
							ElseIf isDefinitionProperty(pIRI) Then
								If lv IsNot Nothing Then
									Dim def As org.geneontology.obographs.model.meta.DefinitionPropertyValue = (New org.geneontology.obographs.model.meta.DefinitionPropertyValue.Builder).val(lv).xrefs(meta.XrefsValues).build()

									Dim nb As org.geneontology.obographs.model.Meta.Builder = put(nodeMetaBuilderMap, subj)
									nb.definition(def)
									nodeIds.add(subj)
								End If

							ElseIf isHasXrefProperty(pIRI) Then
								If lv IsNot Nothing Then
									Dim xref As org.geneontology.obographs.model.meta.XrefPropertyValue = (New org.geneontology.obographs.model.meta.XrefPropertyValue.Builder).val(lv).build()

									Dim nb As org.geneontology.obographs.model.Meta.Builder = put(nodeMetaBuilderMap, subj)
									nb.addXref(xref)
									nodeIds.add(subj)
								End If

							ElseIf isInSubsetProperty(pIRI) Then


									Dim nb As org.geneontology.obographs.model.Meta.Builder = put(nodeMetaBuilderMap, subj)
									nb.addSubset(v.ToString())
									nodeIds.add(subj)


							ElseIf synonymVocabulary.contains(pIRI.ToString()) Then
								Dim scope As org.geneontology.obographs.model.meta.SynonymPropertyValue.SCOPES = synonymVocabulary.get(pIRI.ToString())
								If lv IsNot Nothing Then
									Dim syn As org.geneontology.obographs.model.meta.SynonymPropertyValue = (New org.geneontology.obographs.model.meta.SynonymPropertyValue.Builder).scope(scope).val(lv).xrefs(meta.XrefsValues).build()
									Dim nb As org.geneontology.obographs.model.Meta.Builder = put(nodeMetaBuilderMap, subj)
									nb.addSynonym(syn)
									nodeIds.add(subj)
								End If
							Else
								untranslatedAxioms.add(aaa)
							End If

						Else
							' subject is anonymous
							untranslatedAxioms.add(aaa)
						End If

					End If
				End If
			Next ax

			For Each n As String In nodeIds
				Dim nb As (New org.geneontology.obographs.model.Node.Builder).id(n).label(nodeLabelMap(n))
				If nodeMetaBuilderMap.ContainsKey(n) Then
					Dim meta As org.geneontology.obographs.model.Meta = nodeMetaBuilderMap(n).build()
					nb.meta(meta)
				End If
				If nodeTypeMap.ContainsKey(n) Then nb.type(nodeTypeMap(n))
				nodes.Add(nb.build())
			Next n
			Dim gid As String = Nothing
			Dim version As String = Nothing
			Dim ontId As org.semanticweb.owlapi.model.OWLOntologyID = ontology.OntologyID
			If ontId IsNot Nothing Then gid = getNodeId(ontId.OntologyIRI.orNull())

			Dim meta As org.geneontology.obographs.model.Meta = getAnnotations(ontology.GetCustomAttributes(True))
			Return (New org.geneontology.obographs.model.Graph.Builder).id(gid).meta(meta).nodes(nodes).edges(edges).equivalentNodesSet(ensets).logicalDefinitionAxioms(ldas).build()
		End Function


		Private Sub setNodeType(id As String, t As org.geneontology.obographs.model.Node.RDFTYPES, nodeTypeMap As IDictionary(Of String, org.geneontology.obographs.model.Node.RDFTYPES))
			nodeTypeMap(id) = t
		End Sub

		Private Function put(nodeMetaBuilderMap As IDictionary(Of String, org.geneontology.obographs.model.Meta.Builder), id As String) As org.geneontology.obographs.model.Meta.Builder
			If Not nodeMetaBuilderMap.ContainsKey(id) Then nodeMetaBuilderMap(id) = New org.geneontology.obographs.model.Meta.Builder
			Return nodeMetaBuilderMap(id)
		End Function

		''' <summary>
		''' Translate all axiom annotations into a Meta object
		''' </summary>
		''' <param name="ax">
		''' @return </param>
		Private Function getAnnotations(ax As org.semanticweb.owlapi.model.OWLAxiom) As org.geneontology.obographs.model.Meta
			Return (getAnnotations(ax.GetCustomAttributes(True)))
		End Function
		Private Function getAnnotations(anns As java.util.Set(Of org.semanticweb.owlapi.model.OWLAnnotation)) As org.geneontology.obographs.model.Meta
			Dim xrefs As IList(Of org.geneontology.obographs.model.meta.XrefPropertyValue) = New List(Of org.geneontology.obographs.model.meta.XrefPropertyValue)
			Dim bpvs As IList(Of org.geneontology.obographs.model.meta.BasicPropertyValue) = New List(Of org.geneontology.obographs.model.meta.BasicPropertyValue)
			Dim inSubsets As IList(Of String) = New List(Of String)
			For Each ann As org.semanticweb.owlapi.model.OWLAnnotation In anns
				Dim p As org.semanticweb.owlapi.model.OWLAnnotationProperty = ann.Property
				Dim v As org.semanticweb.owlapi.model.OWLAnnotationValue = ann.Value
				Dim val As String = If(TypeOf v Is org.semanticweb.owlapi.model.IRI, CType(v, org.semanticweb.owlapi.model.IRI).ToString(), CType(v, org.semanticweb.owlapi.model.OWLLiteral).Literal)
				If isHasXrefProperty(p.IRI) Then
					xrefs.Add((New org.geneontology.obographs.model.meta.XrefPropertyValue.Builder).val(val).build())
				ElseIf isInSubsetProperty(p.IRI) Then
					inSubsets.Add(New String(val))
				ElseIf isHasSynonymTypeProperty(p.IRI) Then
					inSubsets.Add(New String(val))
				Else
					bpvs.Add((New org.geneontology.obographs.model.meta.BasicPropertyValue.Builder).pred(getPropertyId(p)).val(val).build())
				End If
			Next ann
			Return (New org.geneontology.obographs.model.Meta.Builder).basicPropertyValues(bpvs).subsets(inSubsets).xrefs(xrefs).build()
		End Function



		Private Function getEdge(subj As String, pred As String, obj As String) As org.geneontology.obographs.model.Edge
			Return (New org.geneontology.obographs.model.Edge.Builder).sub(subj).pred(pred).obj(obj).build()
		End Function

		Private Function getRestriction(x As org.semanticweb.owlapi.model.OWLClassExpression) As org.geneontology.obographs.model.axiom.ExistentialRestrictionExpression
			If TypeOf x Is org.semanticweb.owlapi.model.OWLObjectSomeValuesFrom Then
				Dim r As org.semanticweb.owlapi.model.OWLObjectSomeValuesFrom = CType(x, org.semanticweb.owlapi.model.OWLObjectSomeValuesFrom)
				Dim p As org.semanticweb.owlapi.model.OWLPropertyExpression = r.Property
				Dim f As org.semanticweb.owlapi.model.OWLClassExpression = r.Filler
				If TypeOf p Is org.semanticweb.owlapi.model.OWLObjectProperty AndAlso (Not f.Anonymous) Then Return (New org.geneontology.obographs.model.axiom.ExistentialRestrictionExpression.Builder).propertyId(getPropertyId(CType(p, org.semanticweb.owlapi.model.OWLObjectProperty))).fillerId(getClassId(CType(f, org.semanticweb.owlapi.model.OWLClass))).build()
			End If
			Return Nothing
		End Function

		'    private String shortenIRI(IRI iri) {
		'        prefixHelper
		'    }

		Private Function getPropertyId(p As org.semanticweb.owlapi.model.OWLObjectProperty) As String
			Return p.IRI.ToString()
		End Function
		Private Function getPropertyId(p As org.semanticweb.owlapi.model.OWLAnnotationProperty) As String
			Return p.IRI.ToString()
		End Function
		Private Function getClassId(c As org.semanticweb.owlapi.model.OWLClass) As String
			Return c.IRI.ToString()
		End Function

		Private Function getNodeId(s As org.semanticweb.owlapi.model.IRI) As String
			Return s.ToString()
		End Function

		Public Overridable Function isDefinitionProperty(iri As org.semanticweb.owlapi.model.IRI) As Boolean
			Return iri.ToString().Equals("http://purl.obolibrary.org/obo/IAO_0000115")
		End Function

		Public Overridable Function isHasXrefProperty(iri As org.semanticweb.owlapi.model.IRI) As Boolean
			Return iri.ToString().Equals("http://www.geneontology.org/formats/oboInOwl#hasDbXref")
		End Function

		Public Overridable Function isInSubsetProperty(iri As org.semanticweb.owlapi.model.IRI) As Boolean
			Return iri.ToString().Equals("http://www.geneontology.org/formats/oboInOwl#inSubset")
		End Function

		Public Overridable Function isHasSynonymTypeProperty(iri As org.semanticweb.owlapi.model.IRI) As Boolean
			Return iri.ToString().Equals("http://www.geneontology.org/formats/oboInOwl#hasSynonymType")
		End Function
	End Class

End Namespace