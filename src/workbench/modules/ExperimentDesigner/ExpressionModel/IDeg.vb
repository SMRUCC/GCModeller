#Region "Microsoft.VisualBasic::604211e1740ebb171e1a90d78a320032, modules\ExperimentDesigner\Ideg.vb"

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

    '   Total Lines: 14
    '    Code Lines: 5 (35.71%)
    ' Comment Lines: 7 (50.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 2 (14.29%)
    '     File Size: 329 B


    ' Interface IDeg
    ' 
    '     Properties: label, log2FC, pvalue
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' a simple abstract model for the different expression gene object
''' </summary>
Public Interface IDeg

    ''' <summary>
    ''' the gene unique id
    ''' </summary>
    ''' <returns></returns>
    Property label As String
    Property log2FC As Double
    Property pvalue As Double

End Interface
