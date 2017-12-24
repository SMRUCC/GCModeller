#Region "Microsoft.VisualBasic::f28696259b69c41c3c410b1ef2c3a0e6, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\Map\Area.vb"

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

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

Namespace Assembly.KEGG.WebServices

    Public Class Area

        <XmlAttribute> Public Property shape As String
        ''' <summary>
        ''' 位置坐标信息
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property coords As String
        <XmlElement> Public Property href As String
        <XmlElement> Public Property title As String

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
                If InStr(href, "/dbget-bin/www_bget") = 1 Then
                    With IDVector
                        If .First.IsPattern("[CDG]\d+") Then
                            ' compound, drug, glycan
                            Return NameOf(Compound)
                        ElseIf (shape = "rect" AndAlso .Any(Function(id) id.IsPattern("K\d+"))) OrElse .First.IndexOf(":"c) > -1 Then
                            Return "Gene"
                        ElseIf .First.IsPattern("R\d+") Then
                            Return "Reaction"
                        ElseIf shape = "rect" AndAlso .First.IndexOf(":"c) = -1 Then
                            Return NameOf(Pathway)
                        ElseIf shape = "poly" Then
                            Return "Reaction"
                        Else
                            Throw New NotImplementedException(Me.GetXml)
                        End If
                    End With
                ElseIf InStr(href, "/kegg-bin/show_pathway") = 1 Then
                    Return NameOf(Pathway)
                Else
                    Throw New NotImplementedException(Me.GetXml)
                End If
            End Get
        End Property

        Public ReadOnly Property IDVector As String()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return href.Split("?"c).Last.Split("+"c)
            End Get
        End Property

        Public ReadOnly Property Names As NamedValue(Of String)()
            Get
                Dim t = title _
                    .Split(","c) _
                    .Select(AddressOf Trim) _
                    .Select(Function(s)
                                Dim name = s.GetTagValue(" ")
                                Return New NamedValue(Of String) With {
                                    .Name = name.Name,
                                    .Value = name.Value.GetStackValue("(", ")")
                                }
                            End Function) _
                    .ToArray

                Return t
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
