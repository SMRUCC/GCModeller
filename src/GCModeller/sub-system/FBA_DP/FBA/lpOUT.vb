#Region "Microsoft.VisualBasic::fc086b28e695379587354a8de335ba2f, ..\GCModeller\sub-system\FBA_DP\FBA\lpOUT.vb"

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

Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Linq
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.AnalysisTools.ModelSolvers.FBA.FBA_OUTPUT

''' <summary>
''' 线性规划的最优解的输出
''' </summary>
Public Class lpOUT
    Public Property Objective As String
    Public Property FluxsDistribution As String()

    Public Function CreateDataFile(lpSolveRModel As lpSolveRModel) As TabularOUT()
        Printf("Generating the EXCEL file...")

        Dim LQuery = FluxsDistribution.ToArray(
            Function(id, i) New TabularOUT With {
                .Flux = Val(id),
                .Rxn = lpSolveRModel.fluxColumns(i)})
        Return LQuery
    End Function
End Class
