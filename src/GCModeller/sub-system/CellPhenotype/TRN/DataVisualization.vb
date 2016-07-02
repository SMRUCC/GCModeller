#Region "Microsoft.VisualBasic::ca07261670ca80e87df09c9e48174396, ..\GCModeller\sub-system\CellPhenotype\TRN\DataVisualization.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataVisualization.Network.FileStream
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT

<[PackageNamespace]("Cellphenotype.DynamicsNetwork", Publisher:="xie.guigang@gcmodeller.org")>
Module DataVisualization

    Public Class Edge : Inherits NetworkEdge
        Public Property InteractValue As Double
    End Class

    <ExportAPI("Export.Dynamics", Info:="Export a dynamics cell system network from thje simulation data.")>
    Public Function ExportDynamics(Of T As NetworkEdge)(data As IEnumerable(Of ExprSamples),
                                                        Network As T(),
                                                        <Parameter("Read.Index")>
                                                        ReadIndex As Integer) As Edge()

        Dim dict = data.ToDictionary(Function(obj) obj.locusId,
                                     Function(obj) obj.Values)
        Dim LQuery = (From node As T In Network
                      Let [from] = dict(node.FromNode)(ReadIndex)
                      Let [to] = dict(node.ToNode)(ReadIndex)
                      Where from <> 0 AndAlso [to] <> 0
                      Select New Edge With {
                          .FromNode = node.FromNode,
                          .ToNode = node.ToNode,
                          .InteractionType = node.InteractionType,
                          .Confidence = node.Confidence,
                          .InteractValue = Math.Min([from], [to]) * node.Confidence}).ToArray
        Return LQuery
    End Function

    <ExportAPI("Read.Csv.SimulationResult")>
    Public Function LoadResult(path As String) As ExprSamples()
        Dim Csv As DocumentStream.File = DocumentStream.File.Load(path)
        Return MatrixAPI.ToSamples(Csv, True)
    End Function

    <ExportAPI("Write.Csv.DynamicsNetwork")>
    Public Function WriteDynamics(data As IEnumerable(Of Edge), <Parameter("Path.Save")> SaveTo As String) As Boolean
        Return data.SaveTo(SaveTo, False)
    End Function
End Module

