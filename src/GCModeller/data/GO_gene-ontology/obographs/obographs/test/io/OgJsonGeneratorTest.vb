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