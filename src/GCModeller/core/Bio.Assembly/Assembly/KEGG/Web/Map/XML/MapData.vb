#Region "Microsoft.VisualBasic::03c59e3f334c268c52b8d4e559a4bd2d, core\Bio.Assembly\Assembly\KEGG\Web\Map\XML\MapData.vb"

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
    '    Code Lines: 15 (51.72%)
    ' Comment Lines: 8 (27.59%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (20.69%)
    '     File Size: 883 B


    '     Class MapData
    ' 
    '         Properties: mapdata, module_mapdata
    ' 
    '         Function: PopulateAll, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.WebServices.XML

    Public Class MapData

        ''' <summary>
        ''' the pathway map data
        ''' </summary>
        ''' <returns></returns>
        Public Property mapdata As Area()
        ''' <summary>
        ''' the module network data
        ''' </summary>
        ''' <returns></returns>
        Public Property module_mapdata As Area()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function PopulateAll() As IEnumerable(Of Area)
            Return mapdata.JoinIterates(module_mapdata)
        End Function

        Public Overrides Function ToString() As String
            Return $"{mapdata.TryCount} mapdata and {module_mapdata.TryCount} module mapdata"
        End Function

    End Class
End Namespace
