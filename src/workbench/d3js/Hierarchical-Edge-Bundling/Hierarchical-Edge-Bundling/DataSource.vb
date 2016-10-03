#Region "Microsoft.VisualBasic::142877c70729c5da684f4cf21dbbf9e8, ..\workbench\d3js\Hierarchical-Edge-Bundling\Hierarchical-Edge-Bundling\DataSource.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Linq
Imports System.Runtime.CompilerServices

Module DataSource

    Public Function LoadAs(path As String) As FlareImports()
        Return LoadFromMaps(path)
    End Function

    Public Const fromNode As String = NameOf(fromNode)
    Public Const toNode As String = NameOf(toNode)
    Public Const Confidence As String = NameOf(Confidence)

    Public Function LoadFromMaps(path As String,
                                 Optional fromMap As String = fromNode,
                                 Optional toMap As String = toNode,
                                 Optional sizeMap As String = Confidence) As FlareImports()

        Dim maps As New Dictionary(Of String, String) From {
            {fromNode, fromMap},
            {toNode, toMap},
            {Confidence, sizeMap}
        }
        Dim interacts As IEnumerable(Of Interacts) =
            path.LoadCsv(Of Interacts)(maps:=maps)
        Dim GroupNodes = From x As Interacts
                         In interacts
                         Select x
                         Group x By x.From Into Group
        Dim __imports As FlareImports() =
            LinqAPI.Exec(Of FlareImports) <=
                From g In GroupNodes
                Let datas As Interacts() = g.Group.ToArray
                Select New FlareImports With {
                    .name = g.From.AllTrims,
                    .size = datas.Sum(Function(x) If(x.size = 0, 1, x.size)),
                    .imports = datas.ToArray(Function(x) x.To.AllTrims)
                }

        Return __imports
    End Function

    <Extension>
    Private Function AllTrims(s As String) As String
        Return "flare.vis.operator.label." &
            s.Replace(".", "_") _
             .Replace("[", "_") _
             .Replace("]", "_")
    End Function
End Module

Public Class Interacts : Implements sIdEnumerable

    <Column(fromNode)>
    Public Property From As String Implements sIdEnumerable.Identifier
    <Column(toNode)>
    Public Property [To] As String
    <Column(Confidence)>
    Public Property size As Integer

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
