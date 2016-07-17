Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Language
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
        End Set
    End Property

    Dim __first As AA
    Dim __chain As AA()

    Sub New(PDB As PDB)
        Dim aas As String() =
            LinqAPI.Exec(Of String) <= From AA As AminoAcid
                                       In PDB.AminoAcidSequenceData
                                       Select AA.AA_ID
                                       Distinct
        Dim AAColors = (From cl In RenderingColor.InitCOGColors(aas)
                        Select ID = cl.Key,
                            br = New Pen(New SolidBrush(cl.Value), penWidth)) _
                           .ToDictionary(Function(item) item.ID,
                                         Function(item) item.br)
        Chian = LinqAPI.Exec(Of AA) <= From x As AminoAcid
                                       In PDB.AminoAcidSequenceData
                                       Select New AA(x, AAColors)
    End Sub

    Public Sub Rotate(x As Double)
        For Each aa As AA In Chian
            aa.Point = aa.Point.RotateX(x)
        Next
    End Sub

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
        Dim pre As Point = __draw(g, __first, clientSize, vd)

#If DEBUG Then
        Call g.DrawString($"View Distance: {vd}", New Font(FontFace.SegoeUI, 12), Brushes.Red, New Point(5, 10))
#End If

        For Each p As AA In Chian.Skip(1)
            Dim cur = __draw(g, p, clientSize, vd)
            Call g.DrawLine(p.Color, cur, pre)
            pre = cur
        Next
    End Sub

    Private Function __draw(g As Graphics, aa As AA, clientSize As Size, vd As Integer) As Point
        Dim pt3D As Point3D =
              aa.Point.Project(clientSize.Width,
                               clientSize.Height,
                               256, vd)
        Dim pt As New Point(pt3D.X, pt3D.Y)
        '  pt = GraphToScreen(pt, rect)

        '   Call pt.Offset(pt)
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
End Class