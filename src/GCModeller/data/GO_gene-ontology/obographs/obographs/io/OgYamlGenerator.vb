Namespace org.geneontology.obographs.io


	Public Class OgYamlGenerator

		Public Shared Function render(obj As Object) As String
			Dim mapper As New com.fasterxml.jackson.databind.ObjectMapper(New com.fasterxml.jackson.dataformat.yaml.YAMLFactory)
			mapper.SerializationInclusion = com.fasterxml.jackson.annotation.JsonInclude.Include.NON_NULL
			Dim writer As com.fasterxml.jackson.databind.ObjectWriter = mapper.writerWithDefaultPrettyPrinter()
			Return writer.writeValueAsString(obj)
		End Function

	End Class

End Namespace