#Region "Microsoft.VisualBasic::3e3b400449f08b97aa2769f02e1d4026, ..\core\Bio.Assembly\Assembly\KEGG\Web\Map\Map.vb"

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

Imports System.Drawing
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.WebServices

    Public Class Map

        <XmlElement> Public Property Areas As Area()

        ''' <summary>
        ''' base64 image
        ''' </summary>
        ''' <returns></returns>
        <XmlText>
        Public Property PathwayImage As String

        Public Function GetImage() As Image
            Dim lines$() = PathwayImage.lTokens
            Dim base64$ = String.Join("", lines)
            Return Base64Codec.GetImage(base64)
        End Function

        Public Overrides Function ToString() As String
            Return Areas.GetJson
        End Function

        Const data$ = "<map name=""mapdata"">.+?</map>"

        Public Shared Function ParseHTML(url$) As Map
            Dim html$ = url.GET
            Dim map$ = r.Match(html, data, RegexICSng).Value
            Dim areas = map.lTokens.Skip(1).ToArray
            Dim img = r.Match(html, "<img src="".+?"" name=""pathwayimage"" usemap=""#mapdata"".+?/>", RegexICSng).Value
            Dim tmp$ = App.GetAppSysTempFile
            Dim shapes = areas _
                .Take(areas.Length - 1) _
                .Select(AddressOf Area.Parse) _
                .ToArray

            With "http://www.genome.jp/" & img.src
                Call .DownloadFile(tmp)

                img = tmp.LoadImage.ToBase64String
                img = FastaToken.SequenceLineBreak(200, img)
            End With

            Return New Map With {
                .PathwayImage = img,
                .Areas = shapes
            }
        End Function
    End Class

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
        ''' Compound, Gene, Pathway
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Type As String
            Get
                If InStr(href, "/dbget-bin/www_bget") = 1 Then
                    With IdList.First
                        If .IsPattern("[CDG]\d+") Then
                            ' compound, drug, glycan
                            Return NameOf(Compound)
                        ElseIf .IndexOf(":"c) > -1 Then
                            Return "Gene"
                        ElseIf shape = "rect" AndAlso .IndexOf(":"c) = -1 Then
                            Return NameOf(Pathway)
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

        Public ReadOnly Property IdList As String()
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
            Return Me.GetJson
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
