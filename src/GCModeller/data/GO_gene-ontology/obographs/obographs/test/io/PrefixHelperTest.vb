Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports org.junit.Assert

Namespace org.geneontology.obographs.io




	Public Class PrefixHelperTest

		''' <summary>
		''' Test getting the default context.
		''' </summary>
		''' <exception cref="IOException"> on file problem </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub testContext()
			Dim ioh As New PrefixHelper
			Dim context As com.github.jsonldjava.core.Context = ioh.Context

			assertEquals("Check GO prefix", "http://purl.obolibrary.org/obo/GO_", context.getPrefixes(False).get("GO"))
		End Sub

		''' <summary>
		''' Test fancier JSON-LD contexts.
		''' </summary>
		''' <exception cref="IOException"> on file problem </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub testContextHandling()
			Dim json As String = "{" & vbLf & "  ""@context"" : {" & vbLf & "    ""foo"" : ""http: + " ""bar"" : 'example.com#\",\n"
				" + vbLf + " ""id"": ""http: & "      ""@type"": ""@id""" & vbLf & "    }" & vbLf & "  }" & vbLf & "}" 'example.com#\",\n"

			Dim ioh As New PrefixHelper
			Dim context As com.github.jsonldjava.core.Context = ioh.parseContext(json)
			ioh.Context = context

			Dim expected As IDictionary(Of String, String) = New Dictionary(Of String, String)
			expected("foo") = "http://example.com#"
			expected("bar") = "http://example.com#"
			assertEquals("Check JSON prefixes", expected, ioh.Prefixes)

		''' <summary>
		''' Test prefix maps.
		''' </summary>
		''' <exception cref="IOException"> on file problem </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public void testPrefixHandling() throws java.io.IOException
			Dim ioh As New PrefixHelper(False)
			Dim expected As IDictionary(Of String, String) = New Dictionary(Of String, String)
			assertEquals("Check no prefixes", expected, ioh.Prefixes)

			ioh.addPrefix("foo", "http://example.com#")
			expected("foo") = "http://example.com#"
			assertEquals("Check foo prefix", expected, ioh.Prefixes)

			Dim json As String = "{" & vbLf & "  ""@context"" : {" & vbLf & "    ""foo"" : ""http: + " 'example.com#\"\n"
		" + vbLf + "
		End Sub
		Friend "; assertEquals("Check JSON-LD", json, ioh.getContextString()); ioh.addPrefix("bar: http://example.com#"); expected.put("bar", "http://example.com#"); assertEquals("Check no prefixes", expected, ioh.getPrefixes()); } @Test public void testPrefixManager() throws IOException { PrefixHelper ioh = new PrefixHelper(); DefaultPrefixManager pm = ioh.getPrefixManager(); assertEquals("Check GO CURIE", "http://purl.obolibrary.org/obo/GO_12345", pm.getIRI("GO:12345").toString()); } }
		''' <summary>
		''' Test the default prefix manager.
		''' </summary>
		''' <exception cref="IOException"> on file problem </exception>

End Namespace