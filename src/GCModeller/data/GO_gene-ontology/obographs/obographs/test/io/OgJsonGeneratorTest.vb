#Region "Microsoft.VisualBasic::f8d6357eccca44b248ddcf067dbf1a96, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\test\io\OgJsonGeneratorTest.vb"

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

Imports org.junit.Assert

Namespace org.geneontology.obographs.io




	Public Class OgJsonGeneratorTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			Dim d As org.geneontology.obographs.model.GraphDocument = org.geneontology.obographs.model.GraphDocumentTest.build()

			Dim s As String = OgJsonGenerator.render(d)
			org.apache.commons.io.FileUtils.writeStringToFile(New File("target/simple-example.json"), s)
		End Sub



	End Class

End Namespace
