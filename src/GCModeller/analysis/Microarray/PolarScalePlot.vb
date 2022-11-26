#Region "Microsoft.VisualBasic::ec9614aee34c9b39000a623c9a3af585, GCModeller\analysis\Microarray\PolarScalePlot.vb"

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

    '   Total Lines: 28
    '    Code Lines: 12
    ' Comment Lines: 11
    '   Blank Lines: 5
    '     File Size: 960 B


    ' Module PolarScalePlot
    ' 
    '     Function: Plot
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Module PolarScalePlot

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="sampleinfo">
    ''' 每一个实验分组就是一个极坐标
    ''' </param>
    ''' <param name="size$"></param>
    ''' <param name="padding$"></param>
    ''' <param name="bg$"></param>
    ''' <returns></returns>
    Public Function Plot(matrix As Matrix, sampleinfo As SampleInfo(),
                         Optional size$ = "3000,2700",
                         Optional padding$ = g.DefaultPadding,
                         Optional bg$ = "white") As GraphicsData

        Dim polarAxis = sampleinfo.GroupBy(Function(sample) sample.sample_info).ToArray

    End Function

End Module
