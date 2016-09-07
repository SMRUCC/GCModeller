Imports System
Imports System.Collections.Generic

Namespace org.geneontology.obographs.runner





	Public Class RunEngine


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private verbose As Integer? = 1

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private outpath As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private outformat As String



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private files As IList(Of String) = New List(Of String)

		Friend manager As org.semanticweb.owlapi.model.OWLOntologyManager


		Public Shared Sub Main(ParamArray args As String())
			Dim main As New RunEngine
			Dim TempJCommander As com.beust.jcommander.JCommander = New com.beust.jcommander.JCommander(main, args)
			main.run()
		End Sub

		Public Overridable Sub run()

			Dim fromOwl As New org.geneontology.obographs.owlapi.FromOwl
			Dim file As New File(files(0))
			Dim ont As org.semanticweb.owlapi.model.OWLOntology = Me.loadOWL(file)
			Dim gd As org.geneontology.obographs.model.GraphDocument = fromOwl.generateGraphDocument(ont)

			Dim doc As String
			If outformat Is Nothing OrElse outformat.Equals("json") Then
				doc = org.geneontology.obographs.io.OgJsonGenerator.render(gd)
			ElseIf outformat.Equals("yaml") Then
				doc = org.geneontology.obographs.io.OgYamlGenerator.render(gd)
			Else
				Throw New java.io.IOException("no such format " & outformat)
			End If

			If outpath Is Nothing Then
				Console.WriteLine(doc)
			Else
				org.apache.commons.io.FileUtils.writeStringToFile(New File(outpath), doc)
			End If

		End Sub

		Private Property OWLOntologyManager As org.semanticweb.owlapi.model.OWLOntologyManager
			Get
				If manager Is Nothing Then manager = org.semanticweb.owlapi.apibinding.OWLManager.createOWLOntologyManager()
				Return manager
			End Get
		End Property
		''' <param name="iri"> </param>
		''' <returns> OWL Ontology </returns>
		''' <exception cref="OWLOntologyCreationException"> </exception>
		Public Overridable Function loadOWL(iri As org.semanticweb.owlapi.model.IRI) As org.semanticweb.owlapi.model.OWLOntology
			Return OWLOntologyManager.loadOntology(iri)
		End Function

		''' <param name="file"> </param>
		''' <returns> OWL Ontology </returns>
		''' <exception cref="OWLOntologyCreationException"> </exception>
		Public Overridable Function loadOWL(file As java.io.File) As org.semanticweb.owlapi.model.OWLOntology
			Dim iri As org.semanticweb.owlapi.model.IRI = org.semanticweb.owlapi.model.IRI.create(file)
			Return OWLOntologyManager.loadOntologyFromOntologyDocument(iri)
		End Function
	End Class


End Namespace