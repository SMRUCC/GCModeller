Imports System.Drawing
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
                    With IDVector.First
                        If .IsPattern("[CDG]\d+") Then
                            ' compound, drug, glycan
                            Return NameOf(Compound)
                        ElseIf .IndexOf(":"c) > -1 Then
                            Return "Gene"
                        ElseIf .IsPattern("R\d+") Then
                            Return "Reaction"
                        ElseIf shape = "rect" AndAlso .IndexOf(":"c) = -1 Then
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
                                    .name = name.Name,
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