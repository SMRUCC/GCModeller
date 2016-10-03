#Region "Microsoft.VisualBasic::1f1eca5d8bd72da8625284b9123fffe7, ..\GCModeller\data\GO_gene-ontology\obographs\obographs\io\OgJsonReader.vb"

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

Imports System.IO
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace org.geneontology.obographs.io




    Public Class OgJsonReader

        Public Shared Function readFile(fileName As String) As org.geneontology.obographs.model.Graph
            Return readFile(New FileStream(fileName, FileMode.Open))
        End Function

        Public Shared Function readFile(file As FileStream) As org.geneontology.obographs.model.Graph
            Return readInputStream(file)
        End Function

        Public Shared Function readInputStream(stream As Stream) As org.geneontology.obographs.model.Graph
            Return TryCast(stream.LoadJSONObject(GetType(org.geneontology.obographs.model.Graph)), org.geneontology.obographs.model.Graph)
        End Function

    End Class

End Namespace
