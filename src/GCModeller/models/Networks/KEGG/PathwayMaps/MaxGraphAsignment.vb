#Region "Microsoft.VisualBasic::41621a1a9795e02d59173ade870a067a, GCModeller\models\Networks\KEGG\PathwayMaps\MaxGraphAsignment.vb"

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

    '   Total Lines: 23
    '    Code Lines: 12
    ' Comment Lines: 4
    '   Blank Lines: 7
    '     File Size: 622 B


    '     Class MaxGraphAsignment
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: AssignMapClass
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace PathwayMaps

    ''' <summary>
    ''' assign of the pathway category to compounds/KO by 
    ''' max graph evaluation
    ''' </summary>
    Public Class MaxGraphAsignment

        ReadOnly maps As Map()

        Sub New(maps As IEnumerable(Of Map))
            Me.maps = maps.ToArray
        End Sub

        Public Function AssignMapClass(compounds As IEnumerable(Of String), KO As IEnumerable(Of String)) As IEnumerable(Of NamedValue(Of String))

        End Function

    End Class
End Namespace
