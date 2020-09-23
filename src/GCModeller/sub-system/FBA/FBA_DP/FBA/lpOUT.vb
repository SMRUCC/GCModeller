#Region "Microsoft.VisualBasic::72b45e5d773852327dd22551eebee397, sub-system\FBA\FBA_DP\FBA\lpOUT.vb"

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

    ' Class lpOUT
    ' 
    '     Properties: FluxsDistribution, Objective
    ' 
    '     Function: CreateDataFile
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports SMRUCC.genomics.Analysis.FBA_DP.FBA_OUTPUT

''' <summary>
''' 线性规划的最优解的输出
''' </summary>
Public Class lpOUT
    Public Property Objective As String
    Public Property FluxsDistribution As String()

    Public Function CreateDataFile(lpSolveRModel As lpSolveRModel) As TabularOUT()
        printf("Generating the EXCEL file...")

        Dim LQuery = FluxsDistribution.Select(
            Function(id, i) New TabularOUT With {
                .Flux = Val(id),
                .Rxn = lpSolveRModel.fluxColumns(i)})
        Return LQuery
    End Function
End Class
