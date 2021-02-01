#Region "Microsoft.VisualBasic::6402eb025b428af7b97ae51cae538316, analysis\RNA-Seq\WGCNA\Network\CExprMods.vb"

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

    '     Class CExprMods
    ' 
    '         Properties: altName, nodeName, nodesPresent
    ' 
    '         Function: CreateObject, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace Network

    ''' <summary>
    ''' CytoscapeNodes
    ''' </summary>
    ''' <remarks>
    ''' nodeName	altName	nodeAttr[nodesPresent, ]
    ''' </remarks>
    Public Class CExprMods

        Public Property nodeName As String
        Public Property altName As String

        <Column("nodeAttr[nodesPresent, ]")>
        Public Property nodesPresent As String

        Public Overrides Function ToString() As String
            Return $"{nodeName} @{nodesPresent}"
        End Function

        Friend Shared Function CreateObject(record As String) As CExprMods
            Dim tokens As String() = Strings.Split(record, vbTab)

            Return New CExprMods With {
                .nodeName = tokens(Scan0),
                .altName = tokens(1),
                .nodesPresent = tokens(2)
            }
        End Function
    End Class
End Namespace
