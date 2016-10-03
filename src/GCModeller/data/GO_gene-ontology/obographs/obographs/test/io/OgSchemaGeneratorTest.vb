#Region "Microsoft.VisualBasic::d53567ef64e2a9e2a861908bd5d90764, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\test\io\OgSchemaGeneratorTest.vb"

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

Imports System

Namespace org.geneontology.obographs.io



	Public Class OgSchemaGeneratorTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub test()
			'System.out.println(s);
			writeSchema(GetType(org.geneontology.obographs.model.GraphDocument), "obographs-schema.json")
			writeSchema(GetType(org.geneontology.obographs.model.Graph), "subschemas/obographs-graph-schema.json")
			writeSchema(GetType(org.geneontology.obographs.model.Meta), "subschemas/obographs-meta-schema.json")
		End Sub

		Protected Friend Overridable Sub writeSchema(ByVal c As Type, ByVal fn As String)
			Dim s As String = org.geneontology.obographs.io.OgSchemaGenerator.makeSchema(c)
			org.apache.commons.io.FileUtils.writeStringToFile(New File("target/" & fn), s)
			org.apache.commons.io.FileUtils.writeStringToFile(New File("schema/" & fn), s)
		End Sub

	End Class

End Namespace
