#Region "Microsoft.VisualBasic::bc581ea1215ab9a26ea031127ab816a1, analysis\RNA-Seq\WGCNA\Network\CExprMods.vb"

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

'   Total Lines: 33
'    Code Lines: 20 (60.61%)
' Comment Lines: 6 (18.18%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 7 (21.21%)
'     File Size: 971 B


'     Class CExprMods
' 
'         Properties: altName, nodeName, nodesPresent
' 
'         Function: CreateObject, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection

Namespace Network

    ''' <summary>
    ''' CytoscapeNodes
    ''' </summary>
    ''' <remarks>
    ''' nodeName \t altName \t nodeAttr[nodesPresent, ]
    ''' </remarks>
    Public Class CExprMods : Implements INamedValue

        Public Property nodeName As String Implements INamedValue.Key
        Public Property altName As String

        ''' <summary>
        ''' cluster module of current feature node
        ''' </summary>
        ''' <returns>
        ''' usually be a color name, such as "blue", "turquoise", "brown" and so on.
        ''' </returns>
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
