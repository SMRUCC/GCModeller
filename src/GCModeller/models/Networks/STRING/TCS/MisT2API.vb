#Region "Microsoft.VisualBasic::2c24c01ba364981dc31b88b5367e3a36, STRING\TCS\MisT2API.vb"

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

    ' Module MisT2API
    ' 
    '     Function: (+2 Overloads) AssemblySignalTransductionNetwork, LoadRegpreciseRegulators, LoadStringNetwork, ReadEffectorMap, SaveNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.Data.STRING.SimpleCsv
Imports SMRUCC.genomics.Model.Network.STRING.Models
Imports STRING_netGraph = SMRUCC.genomics.Model.Network.STRING.Models.Network

<Package("MiST2.STrP_Network", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gmail.com")>
Public Module MisT2API

    <ExportAPI("Assemble.STrP",
                   Info:="Assemble the signal transduction network from string-db protein interaction data and MiST2 annotations.")>
    Public Function AssemblySignalTransductionNetwork(stringDB As IEnumerable(Of PitrNode),
                                                          MiST2 As String,
                                                          Regulators As IEnumerable(Of RegpreciseMPBBH)) As STRING_netGraph
        Dim Assembler As New Assembler(stringDB, MiST2, Regulators)
        Return Assembler.CompileAssembly()
    End Function

    <ExportAPI("Read.Mappings.Effector", Info:="Load the effector mapping data from the database.")>
    Public Function ReadEffectorMap(Path As String) As EffectorMap()
        Return Path.LoadCsv(Of EffectorMap)(False).ToArray
    End Function

    <ExportAPI("Assemble.STrP_With_MetaCyc", Info:="Build the signal transduction network with MetaCyc compound as the effector mappings.")>
    Public Function AssemblySignalTransductionNetwork(stringDB As PitrNode(),
                                                          MiST2 As String,
                                                          Regulators As RegpreciseMPBBH(),
                                                          Mapping As EffectorMap()) As STRING_netGraph
        Dim Assembler As New Assembler(stringDB, MiST2, Regulators)
        Return Assembler.CompileAssembly(Mapping)
    End Function

    <ExportAPI("Write.Xml.STrP", Info:="Save the signal transduction network model file.")>
    Public Function SaveNetwork(Network As STRING_netGraph, <Parameter("Save.Path")> file As String) As Boolean
        Return Network.GetXml.SaveTo(file)
    End Function

    <ExportAPI("String-Db.Network.Load", Info:="Load the string-db interaction network model from database.")>
    Public Function LoadStringNetwork(Path As String) As PitrNode()
        Return Path.LoadXml(Of SimpleCsv.Network).Nodes
    End Function

    <ExportAPI("Load.RegpreciseRegulator", Info:="Load the regulators which was mapped from the regprecise database.")>
    Public Function LoadRegpreciseRegulators(<Parameter("Map.Csv", "The bbh map data from the annotated bacterial genome and the regprecise database.")>
                                                 Csv As String) As RegpreciseMPBBH()
        Return Csv.LoadCsv(Of RegpreciseMPBBH)(False).ToArray
    End Function
End Module
