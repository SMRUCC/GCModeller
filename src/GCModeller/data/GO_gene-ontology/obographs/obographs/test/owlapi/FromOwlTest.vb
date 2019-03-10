#Region "Microsoft.VisualBasic::be2a7c8934e554e5d1e573af39c46083, data\GO_gene-ontology\obographs\obographs\test\owlapi\FromOwlTest.vb"

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

    ' 	Class FromOwlTest
    ' 
    ' 	    Sub: export, test
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System
Imports System.Collections.Generic
Imports org.junit.Assert

Namespace org.geneontology.obographs.owlapi




	Public Class FromOwlTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()

			Dim exts As String() = {"obo","owl"}
			Dim dir As New File("src/test/resources")
			Dim files As ICollection(Of File) = org.apache.commons.io.FileUtils.listFiles(dir, exts, True)

			For Each file As File In files
				Console.WriteLine("Converting: " & file)
				'System.out.println(file.toPath());
				'System.out.println(file.toString());

				Dim m As org.semanticweb.owlapi.model.OWLOntologyManager = org.semanticweb.owlapi.apibinding.OWLManager.createOWLOntologyManager()
				Dim ontology As org.semanticweb.owlapi.model.OWLOntology = m.loadOntologyFromOntologyDocument(file)

				Dim ___fromOwl As New FromOwl
				Dim gd As org.geneontology.obographs.model.GraphDocument = ___fromOwl.generateGraphDocument(ontology)

				Dim fn As java.nio.file.Path = file.toPath().FileName
				Dim jsonStr As String = org.geneontology.obographs.io.OgJsonGenerator.render(gd)
				export(jsonStr, fn, ".json")
				Dim yamlStr As String = org.geneontology.obographs.io.OgYamlGenerator.render(gd)
				export(yamlStr, fn, ".yaml")
				'String ofn = fn.toString().replace(".obo", ".json").replace(".owl", ".json");
				'FileUtils.writeStringToFile(new File("examples/"+ofn), s);

			Next file
		End Sub

		Private Sub export(ByVal s As String, ByVal fn As java.nio.file.Path, ByVal suffix As String)
			Dim ofn As String = fn.ToString().Replace(".obo", suffix).Replace(".owl", suffix)
			org.apache.commons.io.FileUtils.writeStringToFile(New File("examples/" & ofn), s)

		End Sub

	End Class

End Namespace
