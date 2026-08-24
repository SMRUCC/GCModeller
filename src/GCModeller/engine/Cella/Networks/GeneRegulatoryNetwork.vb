#Region "Microsoft.VisualBasic::d39bf2113e5863ab07ebaabab130d2a8, engine\Cella\Networks\GeneRegulatoryNetwork.vb"

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


    ' Code Statistics:

    '   Total Lines: 29
    '    Code Lines: 19 (65.52%)
    ' Comment Lines: 3 (10.34%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (24.14%)
    '     File Size: 1013 B


    ' Class GeneRegulatoryNetwork
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetStats
    ' 
    '     Sub: RunStep
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.DBN

''' <summary>
''' 采用动态贝叶斯网络所构建的基因表达调控网络系统
''' </summary>
Public Class GeneRegulatoryNetwork : Inherits SubNetwork

    ReadOnly GRN As DynamicBayesianNetwork

    Sub New(cell As VirtualCella, network As IEnumerable(Of RegulatoryLink), Optional config As DBNConfig = Nothing)
        Call MyBase.New(cell)

        GRN = New DynamicBayesianNetwork(If(config, New DBNConfig))
        GRN.BuildFromTopology(network)
    End Sub

    Public Overrides Sub RunStep()
        Dim metabolites = cell.metabolic.GetStats
        Dim proteins = cell.translation.GetStats
        Dim statsNext = GRN.PredictNextState(metabolites, proteins)
        Dim transcriptionRates = statsNext.RNAAbundanceChanges

    End Sub

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Throw New NotImplementedException()
    End Function
End Class

