Imports System
Imports System.Collections
Imports System.Collections.Generic

Namespace org.geneontology.obographs.io




	''' <summary>
	''' Provides convenience methods for working with JSON LD contexts and prefixes
	''' 
	''' Taken from ROBOT
	''' @author <a href="mailto:james@overton.ca">James A. Overton</a>
	''' </summary>
	Public Class PrefixHelper
		''' <summary>
		''' Logger.
		''' </summary>
		Private Shared ReadOnly logger As org.slf4j.Logger = org.slf4j.LoggerFactory.getLogger(GetType(PrefixHelper))


		''' <summary>
		''' Path to default context as a resource.
		''' </summary>
		Private Shared defaultContextPath As String = "/obo_context.jsonld"

		''' <summary>
		''' Store the current JSON-LD context.
		''' </summary>
		Private context As New com.github.jsonldjava.core.Context

		''' <summary>
		''' Create a new IOHelper with the default prefixes.
		''' </summary>
		Public Sub New()
			Try
				Context = DefaultContext
			Catch e As java.io.IOException
				logger.warn("Could not load default prefixes.")
				logger.warn(e.Message)
			End Try
		End Sub

		''' <summary>
		''' Create a new IOHelper with or without the default prefixes.
		''' </summary>
		''' <param name="defaults"> false if defaults should not be used </param>
		Public Sub New(defaults As Boolean)
			Try
				If defaults Then
					Context = DefaultContext
				Else
					setContext()
				End If
			Catch e As java.io.IOException
				logger.warn("Could not load default prefixes.")
				logger.warn(e.Message)
			End Try
		End Sub

		''' <summary>
		''' Create a new IOHelper with the specified prefixes.
		''' </summary>
		''' <param name="map"> the prefixes to use </param>
		Public Sub New(map As IDictionary(Of String, Object))
			Context = map
		End Sub

		''' <summary>
		''' Create a new IOHelper with prefixes from a file path.
		''' </summary>
		''' <param name="path"> to a JSON-LD file with a @context </param>
		Public Sub New(path As String)
			Try
				Dim jsonString As String = org.apache.commons.io.FileUtils.readFileToString(New File(path))
				Context = jsonString
			Catch e As java.io.IOException
				logger.warn("Could not load prefixes from " & path)
				logger.warn(e.Message)
			End Try
		End Sub

		''' <summary>
		''' Create a new IOHelper with prefixes from a file.
		''' </summary>
		''' <param name="file"> a JSON-LD file with a @context </param>
		Public Sub New(file As java.io.File)
			Try
				Dim jsonString As String = org.apache.commons.io.FileUtils.readFileToString(file)
				Context = jsonString
			Catch e As java.io.IOException
				logger.warn("Could not load prefixes from " & file)
				logger.warn(e.Message)
			End Try
		End Sub

		''' <summary>
		''' Try to guess the location of the catalog.xml file.
		''' Looks in the directory of the given ontology file for a catalog file.
		''' </summary>
		''' <param name="ontologyFile"> the </param>
		''' <returns> the guessed catalog File; may not exist! </returns>
		Public Overridable Function guessCatalogFile(ontologyFile As java.io.File) As java.io.File
			Dim path As String = ontologyFile.Parent
			Dim catalogPath As String = "catalog-v001.xml"
			If path IsNot Nothing Then catalogPath = path & "/catalog-v001.xml"
			Return New File(catalogPath)
		End Function






		''' <summary>
		''' Given a term string, use the current prefixes to create an IRI.
		''' </summary>
		''' <param name="term"> the term to convert to an IRI </param>
		''' <returns> the new IRI </returns>
		Public Overridable Function createIRI(term As String) As org.semanticweb.owlapi.model.IRI
			If term Is Nothing Then Return Nothing

			Try
				' This is stupid, because better methods aren't public.
				' We create a new JSON map and add one entry
				' with the term as the key and some string as the value.
				' Then we run the JsonLdApi to expand the JSON map
				' in the current context, and just grab the first key.
				' If everything worked, that key will be our expanded iri.
				Dim jsonMap As IDictionary(Of String, Object) = New Dictionary(Of String, Object)
				jsonMap(term) = "ignore this string"
				Dim expanded As Object = (New com.github.jsonldjava.core.JsonLdApi).expand(context, jsonMap)
				Dim result As String = CType(expanded, IDictionary(Of String, Object)).Keys.GetEnumerator().next()
				If result IsNot Nothing Then Return org.semanticweb.owlapi.model.IRI.create(result)
			Catch e As Exception
				logger.warn("Could not create IRI for {}", term)
				logger.warn(e.Message)
			End Try
			Return Nothing
		End Function





		''' <summary>
		''' Load a map of prefixes from the "@context" of a JSON-LD string.
		''' </summary>
		''' <param name="jsonString"> the JSON-LD string </param>
		''' <returns> a map from prefix name strings to prefix IRI strings </returns>
		''' <exception cref="IOException"> on any problem </exception>
		Public Shared Function parseContext(jsonString As String) As com.github.jsonldjava.core.Context
			Try
				Dim jsonObject As Object = com.github.jsonldjava.utils.JsonUtils.fromString(jsonString)
				If Not(TypeOf jsonObject Is IDictionary) Then Return Nothing
				Dim jsonMap As IDictionary(Of String, Object) = CType(jsonObject, IDictionary(Of String, Object))
				If Not jsonMap.ContainsKey("@context") Then Return Nothing
				Dim jsonContext As Object = jsonMap("@context")
				Return (New com.github.jsonldjava.core.Context).parse(jsonContext)
			Catch e As Exception
				Throw New java.io.IOException(e)
			End Try
		End Function

		''' <summary>
		''' Get a copy of the default context.
		''' </summary>
		''' <returns> a copy of the current context </returns>
		''' <exception cref="IOException"> if default context file cannot be read </exception>
		Public Overridable Property DefaultContext As com.github.jsonldjava.core.Context
			Get
				Dim stream As java.io.InputStream = GetType(PrefixHelper).getResourceAsStream(defaultContextPath)
				Dim jsonString As String = org.apache.commons.io.IOUtils.ToString(stream)
				Return parseContext(jsonString)
			End Get
		End Property

		''' <summary>
		''' Get a copy of the current context.
		''' </summary>
		''' <returns> a copy of the current context </returns>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getContext() As com.github.jsonldjava.core.Context 'JavaToDotNetTempPropertyGetContext
		Public Overridable Property Context As com.github.jsonldjava.core.Context
			Get
				Return Me.context.clone()
			End Get
			Set(context As com.github.jsonldjava.core.Context)
		End Property

		''' <summary>
		''' Set an empty context.
		''' </summary>
		Public Overridable Sub setContext()
			Me.context = New com.github.jsonldjava.core.Context
		End Sub

			If context Is Nothing Then
				setContext()
			Else
				Me.context = context
			End If
		End Sub

		''' <summary>
		''' Set the current JSON-LD context to the given context.
		''' </summary>
		''' <param name="jsonString"> the new JSON-LD context as a JSON string </param>
		Public Overridable Property Context As String
			Set(jsonString As String)
				Try
					Me.context = parseContext(jsonString)
				Catch e As Exception
					logger.warn("Could not set context from JSON")
					logger.warn(e.Message)
				End Try
			End Set
		End Property

		''' <summary>
		''' Set the current JSON-LD context to the given map.
		''' </summary>
		''' <param name="map"> a map of strings for the new JSON-LD context </param>
		Public Overridable Property Context As IDictionary(Of String, Object)
			Set(map As IDictionary(Of String, Object))
				Try
					Me.context = (New com.github.jsonldjava.core.Context).parse(map)
				Catch e As Exception
					logger.warn("Could not set context {}", map)
					logger.warn(e.Message)
				End Try
			End Set
		End Property

		''' <summary>
		''' Make an OWLAPI DefaultPrefixManager from a map of prefixes.
		''' </summary>
		''' <param name="prefixes"> a map from prefix name strings to prefix IRI strings </param>
		''' <returns> a new DefaultPrefixManager </returns>
		Public Shared Function makePrefixManager(prefixes As IDictionary(Of String, String)) As org.semanticweb.owlapi.util.DefaultPrefixManager
			Dim pm As New org.semanticweb.owlapi.util.DefaultPrefixManager
			For Each entry As KeyValuePair(Of String, String) In prefixes
				pm.setPrefix(entry.Key & ":", entry.Value)
			Next entry
			Return pm
		End Function

		''' <summary>
		''' Get a prefix manager with the current prefixes.
		''' </summary>
		''' <returns> a new DefaultPrefixManager </returns>
		Public Overridable Property PrefixManager As org.semanticweb.owlapi.util.DefaultPrefixManager
			Get
				Return makePrefixManager(context.getPrefixes(False))
			End Get
		End Property

		''' <summary>
		''' Add a prefix mapping as a single string "foo: http://example.com#".
		''' </summary>
		''' <param name="combined"> both prefix and target </param>
		''' <exception cref="IllegalArgumentException"> on malformed input </exception>
		Public Overridable Sub addPrefix(combined As String)
			Dim results As String() = combined.Split(":", 2)
			If results.Length < 2 Then Throw New System.ArgumentException("Invalid prefix string: " & combined)
			addPrefix(results(0), results(1))
		End Sub

		''' <summary>
		''' Add a prefix mapping to the current JSON-LD context,
		''' as a prefix string and target string.
		''' Rebuilds the context.
		''' </summary>
		''' <param name="prefix"> the short prefix to add; should not include ":" </param>
		''' <param name="target"> the IRI string that is the target of the prefix </param>
		Public Overridable Sub addPrefix(prefix As String, target As String)
			Try
				context.put(prefix.Trim(), target.Trim())
				context.remove("@base")
				Context = CType(context, IDictionary(Of String, Object))
			Catch e As Exception
				logger.warn("Could not load add prefix ""{}"" ""{}""", prefix, target)
				logger.warn(e.Message)
			End Try
		End Sub

		''' <summary>
		''' Get a copy of the current prefix map.
		''' </summary>
		''' <returns> a copy of the current prefix map </returns>
		Public Overridable Property Prefixes As IDictionary(Of String, String)
			Get
				Return Me.context.getPrefixes(False)
			End Get
			Set(map As IDictionary(Of String, Object))
				Context = map
			End Set
		End Property


		''' <summary>
		''' Return the current prefixes as a JSON-LD string.
		''' </summary>
		''' <returns> the current prefixes as a JSON-LD string </returns>
		''' <exception cref="IOException"> on any error </exception>
		Public Overridable Property ContextString As String
			Get
				Try
					Dim compact As Object = com.github.jsonldjava.core.JsonLdProcessor.compact(com.github.jsonldjava.utils.JsonUtils.fromString("{}"), context.getPrefixes(False), New com.github.jsonldjava.core.JsonLdOptions)
					Return com.github.jsonldjava.utils.JsonUtils.toPrettyString(compact)
				Catch e As Exception
					Throw New java.io.IOException("JSON-LD could not be generated", e)
				End Try
			End Get
		End Property

		''' <summary>
		''' Write the current context as a JSON-LD file.
		''' </summary>
		''' <param name="path"> the path to write the context </param>
		''' <exception cref="IOException"> on any error </exception>
		Public Overridable Sub saveContext(path As String)
			saveContext(New File(path))
		End Sub

		''' <summary>
		''' Write the current context as a JSON-LD file.
		''' </summary>
		''' <param name="file"> the file to write the context </param>
		''' <exception cref="IOException"> on any error </exception>
		Public Overridable Sub saveContext(file As java.io.File)
			Dim writer As New java.io.FileWriter(file)
			writer.write(ContextString)
			writer.close()
		End Sub





	End Class

End Namespace