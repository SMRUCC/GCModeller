#Region "Microsoft.VisualBasic::1eac4cb293051c97df508fd1c8efab68, circoskit\Model.vb"

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

    ' Module Model
    ' 
    '     Function: SetIdeogramRadius
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Circos
Imports SMRUCC.genomics.Visualize.Circos.Configurations

<Package("circos.model", Category:=APICategories.UtilityTools)>
Module Model

    ''' <summary>
    ''' Invoke set the radius value of the ideogram circle.
    ''' </summary>
    ''' <param name="circos"></param>
    ''' <param name="r"></param>
    ''' <returns></returns>
    <ExportAPI("Set.Ideogram.Radius")>
    Public Function SetIdeogramRadius(circos As Circos, r As Double) As Circos
        Dim idg As Ideogram = circos.GetIdeogram
        Call CircosAPI.SetIdeogramRadius(idg, r)
        Return circos
    End Function
End Module
