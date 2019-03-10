#Region "Microsoft.VisualBasic::eea9c40eba29be2d467e7a9c39c41b32, data\GO_gene-ontology\obographs\obographs\test\io\PrefixHelperTest.vb"

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

    ' 	Class PrefixHelperTest
    ' 
    ' 	    Sub: testContext, testContextHandling
    ' 
    ' /********************************************************************************/

#End Region

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
