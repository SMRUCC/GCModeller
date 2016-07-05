#Region "Microsoft.VisualBasic::52f9da1abdda35fb887cc2dfe2dffbfc, ..\interops\visualize\Cytoscape\Cytoscape\Cli\Cytoscape\CLI\MetaCyc.vb"

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

Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.NetworkModel
Imports LANS.SystemsBiology.Assembly.SBML.Level2
Imports Microsoft.VisualBasic.CommandLine.Reflection

Partial Module CLI

    <ExportAPI("/Net.rFBA", Usage:="/Net.rFBA /in <metacyc.sbml> /fba.out <flux.Csv> [/out <outDIR>]")>
    Public Function net_rFBA(args As CommandLine.CommandLine) As Integer
        Dim inSBML As String = args("/in")
        Dim fbaResult As String = args("/fba.out")
        Dim outDIR As String = args.GetValue("/out", inSBML.TrimFileExt & "-" & fbaResult.GetJustFileName & "/")
        Dim net = SBMLrFBA.CreateNetwork(XmlFile.Load(inSBML), SBMLrFBA.LoadFBAResult(fbaResult))
        Return net.Save(outDIR, Encodings.ASCII.GetEncodings).CLICode
    End Function
End Module

