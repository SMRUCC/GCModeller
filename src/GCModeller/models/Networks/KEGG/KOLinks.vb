#Region "Microsoft.VisualBasic::c88d8ee2d236b77e90f5f1215b85891e, ..\GCModeller\models\Networks\KEGG\KOLinks.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Imports Microsoft.VisualBasic.Text.Xml.Models
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB

Public Class KOLinks

    Public Property entry As String
    Public Property name As String
    Public Property definition As String
    Public Property pathways As NamedValue()
    Public Property reactions As String()

    Public Shared Iterator Function Build(ko00001$) As IEnumerable(Of KOLinks)
        For Each path As String In ls - l - r - "*.XML" <= ko00001
            Dim xml As Orthology = path.LoadXml(Of Orthology)

            If xml.Pathway.IsNullOrEmpty Then
                Continue For
            End If

            Dim reactions$() = xml.xref _
                .Terms _
                .Where(Function(l) l.name = "RN") _
                .Select(Function(x) x.Comment) _
                .ToArray
            Dim pathways As NamedValue() = xml.Pathway _
                .Select(Function(x)
                            Return New NamedValue(x.name, x.text.TrimNewLine().Trim)
                        End Function) _
                .ToArray

            Yield New KOLinks With {
                .definition = xml.Definition,
                .entry = xml.Entry,
                .name = xml.Name,
                .pathways = pathways,
                .reactions = reactions
            }
        Next
    End Function
End Class
