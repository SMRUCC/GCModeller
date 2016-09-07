Namespace org.geneontology.obographs.io




	Public Class OgYamlReader

		Public Shared Function readFile(fileName As String) As org.geneontology.obographs.model.Graph
			Return readFile(New File(fileName))
		End Function

		Public Shared Function readFile(file As java.io.File) As org.geneontology.obographs.model.Graph
			Dim objectMapper As New com.fasterxml.jackson.databind.ObjectMapper(New com.fasterxml.jackson.dataformat.yaml.YAMLFactory)
			Return objectMapper.readValue(file, GetType(org.geneontology.obographs.model.Graph))
		End Function

		Public Shared Function readInputStream(stream As java.io.InputStream) As org.geneontology.obographs.model.Graph
			Dim objectMapper As New com.fasterxml.jackson.databind.ObjectMapper(New com.fasterxml.jackson.dataformat.yaml.YAMLFactory)
			Return objectMapper.readValue(stream, GetType(org.geneontology.obographs.model.Graph))
		End Function

	End Class

End Namespace