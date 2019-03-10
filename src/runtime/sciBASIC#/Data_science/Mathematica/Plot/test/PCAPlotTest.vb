﻿#Region "Microsoft.VisualBasic::be03e60390020ba457dc55a19a31f51f, Data_science\Mathematica\Plot\test\PCAPlotTest.vb"

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

    ' Module PCAPlotTest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics.PCA
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math.Matrix
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File

Module PCAPlotTest

    Sub Main()
        Dim data As GeneralMatrix = csv.Load("D:\GCModeller\src\runtime\sciBASIC#\Data_science\algorithms\PCA\flower.csv").AsMatrix

        Call PCAPlot.PC2(data, 8).AsGDIImage.SaveAs("D:\GCModeller\src\runtime\sciBASIC#\Data_science\algorithms\PCA\flower.PCA2.png")
    End Sub
End Module
