Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Data.RCSB.PDB

''' <summary>
''' 多肽链的绘图模型
''' </summary>
Public Class ChainModel

    Public Property Chian As AA()

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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="clientSize"></param>
    ''' <param name="vd">View distance</param>
    Public Sub UpdateGraph(ByRef g As Graphics, clientSize As Size, vd As Integer)

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