#Region "Microsoft.VisualBasic::4dc3d972c9f572ae835f8b45e7b9a541, GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\Map\XML\Area.vb"

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

    '   Total Lines: 160
    '    Code Lines: 121
    ' Comment Lines: 23
    '   Blank Lines: 16
    '     File Size: 6.45 KB


    '     Class Area
    ' 
    '         Properties: [class], coords, data_coords, data_id, entry
    '                     href, IDVector, moduleId, Names, Rectangle
    '                     refid, shape, title, Type
    ' 
    '         Function: Parse, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Namespace Assembly.KEGG.WebServices

    <XmlType("area")> Public Class Area

        <XmlAttribute> Public Property data_id As String

        ''' <summary>
        ''' + rect
        ''' + poly
        ''' + circle
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property shape As String
        ''' <summary>
        ''' 位置坐标信息
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property coords As String
        <XmlAttribute> Public Property data_coords As String
        <XmlAttribute> Public Property [class] As String
        <XmlAttribute> Public Property href As String
        <XmlAttribute> Public Property entry As String
        <XmlAttribute> Public Property refid As String
        <XmlAttribute> Public Property moduleId As String

        <XmlText>
        Public Property title As String

        ''' <summary>
        ''' 如果是方块的话，则这个属性代表图上面的方块的参数，包括位置和大小
        ''' 如果是圆的话，则这个属性的中心点为圆心，宽度的一半为半径
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Rectangle As RectangleF
            Get
                Dim t#() = coords _
                    .Split(","c) _
                    .Select(AddressOf Val) _
                    .ToArray
                Dim pt As New PointF(t(0), t(1))

                If t.Length = 3 Then
                    ' 中心点(x, y), r
                    Dim r# = t(2)
                    pt = New PointF(pt.X - r / 2, pt.Y - r / 2)
                    Return New RectangleF(pt, New SizeF(r, r))
                ElseIf t.Length = 4 Then
                    Dim size As New SizeF(t(2) - pt.X, t(3) - pt.Y)
                    Return New RectangleF(pt, size)
                Else
                    Throw New NotImplementedException(coords)
                End If
            End Get
        End Property

        ''' <summary>
        ''' Compound, Gene, Pathway, Reaction
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Type As String
            Get
                If InStr(href, "/dbget-bin/www_bget") + InStr(href, "/entry") > 0 Then
                    With IDVector
                        If .First.IsPattern("[CDG]\d+") OrElse .First.IsPattern("DG\d+") Then
                            ' compound, drug, glycan
                            Return NameOf(Compound)
                        ElseIf (shape = "rect" AndAlso .Any(Function(id) id.IsPattern("K\d+"))) OrElse .First.IndexOf(":"c) > -1 Then
                            Return "Gene"
                        ElseIf .First.IsPattern("R\d+") Then
                            Return "Reaction"
                        ElseIf shape = "rect" AndAlso .First.IndexOf(":"c) = -1 Then
                            Return NameOf(BriteHEntry.Pathway)
                        ElseIf shape = "poly" Then
                            Return "Reaction"
                        Else
                            Return "unknown"
                        End If
                    End With
                ElseIf InStr(href, "/kegg-bin/show_pathway") + InStr(href, "/pathway/") > 0 Then
                    Return NameOf(BriteHEntry.Pathway)
                ElseIf InStr(href, "search_htext?htext=") > 0 Then
                    ' 查看这张pathway的分类信息
                    ' 不进行绘制
                    Return "null"
                ElseIf Not title.StringEmpty AndAlso title.StartsWith("PATH:") Then
                    Return NameOf(BriteHEntry.Pathway)
                Else
                    Return "unknown"
                End If
            End Get
        End Property

        Public ReadOnly Property IDVector As String()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Strings.Replace(href, "/entry/", "") _
                    .Replace("/pathway/", "") _
                    .Split("?"c) _
                    .Last _
                    .Split("+"c)
            End Get
        End Property

        Public ReadOnly Property Names As NamedValue(Of String)()
            Get
                Dim terms As NamedValue(Of String)()

                If title.StringEmpty Then
                    Return {}
                Else
                    ' the delimiter between the tokens is seperated via
                    ' a command,blankspace combination
                    ' due to the reason of compound name may contains the
                    ' comma symbol, so we use the string split function at
                    ' here for avoid the name parser error
                    terms = title _
                        .StringSplit(", ") _
                        .Select(AddressOf Trim) _
                        .Select(Function(s)
                                    Dim nameTerm = s.GetTagValue(" ")
                                    Dim name$ = nameTerm.Value.GetStackValue("(", ")")

                                    Return New NamedValue(Of String) With {
                                        .Name = nameTerm.Name,
                                        .Value = name
                                    }
                                End Function) _
                        .ToArray
                End If

                Return terms
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[{shape}] {IDVector.GetJson}"
        End Function

        Public Shared Function Parse(line$) As Area
            Dim attrs As Dictionary(Of NamedValue(Of String)) = line _
                .TagAttributes _
                .ToDictionary
            Dim getValue = Function(key$)
                               Return attrs.TryGetValue(key).Value
                           End Function

            Return New Area With {
                .coords = getValue(NameOf(coords)),
                .href = getValue(NameOf(href)),
                .shape = getValue(NameOf(shape)),
                .title = getValue(NameOf(title))
            }
        End Function
    End Class
End Namespace
