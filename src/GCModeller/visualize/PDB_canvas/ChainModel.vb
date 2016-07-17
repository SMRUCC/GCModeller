Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports SMRUCC.genomics.Data.RCSB.PDB

''' <summary>
''' 多肽链的绘图模型
''' </summary>
Public Class ChainModel

    Public Property Chian As AA()

    Public Sub UpdateGraph(ByRef g As Graphics)

    End Sub
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