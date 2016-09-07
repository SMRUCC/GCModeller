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