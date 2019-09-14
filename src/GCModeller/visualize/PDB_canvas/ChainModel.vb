#Region "Microsoft.VisualBasic::753e2422647e25bb08a92f50058cbba0, visualize\PDB_canvas\ChainModel.vb"

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

    ' Class ChainModel
    ' 
    '     Properties: Chian
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: __draw, GraphToScreen, ToString
    ' 
    '     Sub: Rotate, RotateX, RotateY, UpdateGraph
    ' 
    ' Class AA
    ' 
    '     Properties: Color, Point
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Drawing3D.Math3D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Data.RCSB.PDB

''' <summary>
''' 多肽链的绘图模型
''' </summary>
Public Class ChainModel

    Public Property Chian As AA()
        Get
            Return __chain
        End Get
        Set(value As AA())
            __first = value(Scan0)
            __chain = value
            __central = value.Select(Function(x) x.Point).Center
        End Set
    End Property

    Dim __first As AA
    Dim __chain As AA()
    Dim __central As Point3D

    Sub New(PDB As PDB)
        Dim aas As String() =
            LinqAPI.Exec(Of String) <= From AA As AminoAcid
                                       In PDB.AminoAcidSequenceData
                                       Select AA.AA_ID
                                       Distinct
        Dim AAColors = (From cl
                        In RenderingColor.InitCOGColors(aas)
                        Select ID = cl.Key,
                            br = New Pen(New SolidBrush(cl.Value), penWidth)) _
                           .ToDictionary(Function(item) item.ID,
                                         Function(item) item.br)
        Chian = LinqAPI.Exec(Of AA) <= From x As AminoAcid
                                       In PDB.AminoAcidSequenceData
                                       Select New AA(x, AAColors)
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Sub RotateX(x As Double)
        '    For Each aa As AA In __chain
        '        aa.Point = aa.Point.RotateX(x)
        '    Next
        _rotate = New Point3D(x, _rotate.Y, _rotate.Z)
    End Sub

    Public Sub RotateY(y As Double)
        '    For Each aa As AA In __chain
        '        aa.Point = aa.Point.RotateY(y)
        '    Next
        _rotate = New Point3D(_rotate.X, y, _rotate.Z)
    End Sub

    Public Sub Rotate(a As Double)
        '    For Each aa As AA In __chain
        '        aa.Point = aa.Point.RotateX(a)
        '        aa.Point = aa.Point.RotateY(a)
        '        aa.Point = aa.Point.RotateZ(a)
        '    Next
        _rotate = New Point3D(a, a, a)
    End Sub

    Dim _rotate As New Point3D

    Public Shared Function GraphToScreen(iPos As Point, rect As Rectangle) As Point
        Dim x As Integer = CInt(Math.Truncate(iPos.X + (CSng(rect.Right - rect.Left) / 2.0F)))
        Dim y As Integer = CInt(Math.Truncate(iPos.Y + (CSng(rect.Bottom - rect.Top) / 2.0F)))
        Return New Point(x, y)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="clientSize"></param>
    ''' <param name="vd">View distance</param>
    Public Sub UpdateGraph(ByRef g As Graphics, clientSize As Size, vd As Integer)
        Dim pre As Point = __draw(g, __first, clientSize, vd, _rotate)

#Const DEBUG = 1

#If DEBUG Then
        Call g.DrawString($"View Distance: {vd}", New Font(FontFace.SegoeUI, 12), Brushes.Red, New Point(5, 10))
#End If

        For Each p As AA In Chian.Skip(1)
            Dim cur = __draw(g, p, clientSize, vd, _rotate)
            Call g.DrawLine(p.Color, cur, pre)
            pre = cur
        Next
    End Sub

    Private Function __draw(g As Graphics, aa As AA, client As Size, vd As Integer, rotate As Point3D) As Point
        Dim pt3D As Point3D = aa.Point _
            .RotateX(_rotate.X).RotateY(_rotate.Y).RotateZ(_rotate.Z) _
            .Project(client.Width, client.Height, 256, vd)

        Dim pt As New Point(pt3D.X, pt3D.Y)
        Call g.FillPie(aa.Color.Brush, New Rectangle(pt, New Size(10, 10)), 0, 360)

        Return pt
    End Function
End Class

''' <summary>
''' Amino acid.
''' </summary>
Public Class AA

    Public Property Point As Point3D
    Public Property Color As Pen

    Sub New(aa As AminoAcid, AAColors As Dictionary(Of String, Pen))
        Dim ap As Keywords.Point3D = aa.Carbon.Location

        Color = AAColors(aa.AA_ID)
        Point = New Point3D(ap.X, ap.Y, ap.Z)
    End Sub

    Public Overrides Function ToString() As String
        Return Point.GetJson
    End Function
End Class
