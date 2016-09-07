Imports System

Namespace org.geneontology.obographs.io




	Public Class OgSchemaGenerator

		Public Shared Function makeSchema(c As Type) As String

			Dim m As New com.fasterxml.jackson.databind.ObjectMapper
			Dim visitor As New com.fasterxml.jackson.module.jsonSchema.factories.SchemaFactoryWrapper
			m.acceptJsonFormatVisitor(m.constructType(c), visitor)
			Dim jsonSchema As com.fasterxml.jackson.module.jsonSchema.JsonSchema = visitor.finalSchema()
			Dim s As String = m.writerWithDefaultPrettyPrinter().writeValueAsString(jsonSchema)
			Return s
		End Function

	End Class

End Namespace