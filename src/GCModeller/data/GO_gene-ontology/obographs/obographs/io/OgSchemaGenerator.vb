#Region "Microsoft.VisualBasic::86861e08700b68eb5c753baf72c1984b, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\io\OgSchemaGenerator.vb"

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
