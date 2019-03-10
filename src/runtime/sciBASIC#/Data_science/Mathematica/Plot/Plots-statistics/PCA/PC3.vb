﻿#Region "Microsoft.VisualBasic::378758b8a1b483c70dec6394042b204f, Data_science\Mathematica\Plot\Plots-statistics\PCA\PC3.vb"

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

    '     Module PCAPlot
    ' 
    '         Function: PC3
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports PCA_analysis = Microsoft.VisualBasic.Math.LinearAlgebra.PCA

Namespace PCA

    Partial Module PCAPlot

        ''' <summary>
        ''' 将目标数据通过PCA的方法降维到三维，然后绘制空间散点图
        ''' </summary>
        ''' <param name="input"></param>
        ''' <param name="sampleGroup%"></param>
        ''' <param name="labels$"></param>
        ''' <param name="size$"></param>
        ''' <param name="colorSchema$"></param>
        ''' <returns></returns>
        <Extension> Public Function PC3(input As GeneralMatrix,
                                        sampleGroup%,
                                        Optional labels$() = Nothing,
                                        Optional size$ = "2000,1800",
                                        Optional colorSchema$ = "Set1:c8") As GraphicsData

            Throw New NotImplementedException
        End Function
    End Module
End Namespace
