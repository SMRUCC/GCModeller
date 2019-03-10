#Region "Microsoft.VisualBasic::65dbaaed772b8f1def17903f1223a22d, data\GO_gene-ontology\obographs\obographs\io\OgYamlReader.vb"

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

    ' 	Class OgYamlReader
    ' 
    ' 	    Function: (+2 Overloads) readFile, readInputStream
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
