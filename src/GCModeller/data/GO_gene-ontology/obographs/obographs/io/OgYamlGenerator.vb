#Region "Microsoft.VisualBasic::dbad86e6b75af0f333e7c6a88aacbedd, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\io\OgYamlGenerator.vb"

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
