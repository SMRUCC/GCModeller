#Region "Microsoft.VisualBasic::a4ddc7d34de270036a6694db2842c04d, engine\IO\GCTabular\DataVisualization\CytoscapeAPI.vb"

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

    '     Module CytoscapeAPI
    ' 
    '         Function: CreateNetwork, (+2 Overloads) CreateObject, GetInteractions, SaveNetwork, SaveResult
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat
Imports Microsoft.VisualBasic.Text

Namespace DataVisualization

    <Package("Cytoscape.GCModeller.NetVisual")>
    Public Module CytoscapeAPI

        <ExportAPI("cytoscape_generator.create")>
        Public Function CreateObject(Model As CellSystemXmlModel) As CytoscapeGenerator
            Return New CytoscapeGenerator(Model)
        End Function

        <ExportAPI("cytoscape_generator.create_from_loader")>
        Public Function CreateObject(Loader As XmlresxLoader) As CytoscapeGenerator
            Return New CytoscapeGenerator(Loader)
        End Function

        <ExportAPI("create_network")>
        Public Function CreateNetwork([operator] As CytoscapeGenerator) As NetModel
            Dim Interactions As DataVisualization.Interactions() = Nothing, NodeAttributes As DataVisualization.NodeAttributes() = Nothing
            Call [operator].CreateNetworkFile(Interactions, NodeAttributes)
            Return New NetModel(Interactions, NodeAttributes)
        End Function

        <ExportAPI("get.network")>
        Public Function GetInteractions(network As NetModel) As Interactions()
            Return network.Edges
        End Function

        <ExportAPI("write.csv.network")>
        Public Function SaveNetwork(network As NetModel, outDIR As String) As Boolean
            Return network.Save(outDIR, Encodings.ASCII.CodePage)
        End Function

        <ExportAPI("save.paths")>
        Public Function SaveResult(path As KeyValuePair(Of Integer, Interactions())(), saveto As String) As Boolean
            Dim CsvData As IO.File = New IO.File

            For Each line In path
                CsvData += New String() {String.Format("Path contains {0} nodes", line.Key)}

                For Each item As Interactions In line.Value
                    CsvData += New String() {item.FromNode, item.ToNode, item.Interaction}
                Next
                CsvData += New String() {"</>"}
            Next

            Return CsvData.Save(saveto, False)
        End Function
    End Module
End Namespace
