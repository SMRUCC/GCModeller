#Region "Microsoft.VisualBasic::72c1b18fd00c17a24d3ea3ce1dbda093, visualize\Cytoscape\CLI_tool\CLI\MetaCyc.vb"

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

    ' Module CLI
    ' 
    '     Function: net_rFBA
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Model.SBML.Level2
Imports SMRUCC.genomics.Visualize.Cytoscape.NetworkModel

Partial Module CLI

    <ExportAPI("/Net.rFBA",
               Usage:="/Net.rFBA /in <metacyc.sbml> /fba.out <flux.Csv> [/out <outDIR>]")>
    <Group(CLIGrouping.MetaCyc)>
    Public Function net_rFBA(args As CommandLine) As Integer
        Dim inSBML As String = args("/in")
        Dim fbaResult As String = args("/fba.out")
        Dim outDIR As String = args.GetValue("/out", inSBML.TrimSuffix & "-" & fbaResult.BaseName & "/")
        Dim net = SBMLrFBA.CreateNetwork(XmlFile.Load(inSBML), SBMLrFBA.LoadFBAResult(fbaResult))
        Return net.Save(outDIR, Encodings.ASCII.CodePage).CLICode
    End Function
End Module
