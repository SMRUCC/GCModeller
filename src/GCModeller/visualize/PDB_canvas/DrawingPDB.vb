#Region "Microsoft.VisualBasic::ff44d1b698684d2817635185d2f2896c, visualize\PDB_canvas\DrawingPDB.vb"

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

    ' Module DrawingPDB
    ' 
    '     Function: MolDrawing
    ' 
    '     Sub: __drawingOfAA
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing3D.Math3D
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.RCSB.PDB

''' <summary>
''' Visualize the protein 3D structure from the PDB file.
''' </summary>
<Package("Drawing.Protein.3D.PDB",
                    Description:="Visualize the protein 3D structure from the PDB file.",
                    Publisher:="amethyst.asuka@gcmodeller.org", Url:="http://gcmodeller.org")>
Public Module DrawingPDB

    <DataFrameColumn("X.Rotation")> Public XRotation As Double = 60
    <DataFrameColumn("Scale.Factor")> Public ScaleFactor As Double = 20
    <DataFrameColumn("Pen.Width")> Public penWidth As Integer = 10

    ''' <summary>
    ''' Drawing a protein structure from its pdb data.
    ''' </summary>
    ''' <param name="PDB"></param>
    ''' <param name="hideAtoms"></param>
    ''' <param name="DisplayAAID"></param>
    ''' <returns></returns>
    <ExportAPI("Drawing.Invoke", Info:="Drawing a protein structure from its pdb data.")>
    <Extension>
    Public Function MolDrawing(PDB As PDB, Optional hideAtoms As Boolean = True, Optional DisplayAAID As Boolean = True) As Image
        Dim Device As Graphics2D = (New Size(3000, 3000)).CreateGDIDevice
        Dim offset As Point = Device.Center
        Dim AASequence As AminoAcid() = PDB.AminoAcidSequenceData
        Dim PreAA As AminoAcid = AASequence.First
        Dim PrePoint As PointF
        Dim aas As String() = (From AA In AASequence Select AA.AA_ID Distinct).ToArray
        Dim AAColors = (From cl In RenderingColor.InitCOGColors(aas)
                        Select ID = cl.Key,
                            br = New Pen(New SolidBrush(cl.Value), penWidth)) _
                           .ToDictionary(Function(item) item.ID,
                                         Function(item) item.br)
        Dim AAFont As New Font(FontFace.MicrosoftYaHei, 10)
        Dim pt2d As PointF

        Call __drawingOfAA(PreAA, PrePoint, offset, Device, DisplayAAID, AAFont, hideAtoms) ' 绘制第一个碳原子

        For Each Point As AminoAcid In AASequence


            Call __drawingOfAA(Point, pt2d, offset, Device, DisplayAAID, AAFont, hideAtoms)
            Call Device.Graphics.DrawLine(AAColors(Point.AA_ID), pt2d, PrePoint)

            PrePoint = pt2d
        Next

        Dim Max = PDB.MaxSpace
        Dim Min = PDB.MinSpace

        Call Device.Graphics.DrawLine(Pens.Black, New Drawing3D.Point3D(Max.X * ScaleFactor, 0, 0).SpaceToGrid(XRotation, offset), New Drawing3D.Point3D(Min.Y * ScaleFactor, 0, 0).SpaceToGrid(XRotation, offset)) 'X
        Call Device.Graphics.DrawLine(Pens.Black, New Drawing3D.Point3D(0, Max.Y * ScaleFactor, 0).SpaceToGrid(XRotation, offset), New Drawing3D.Point3D(0, Min.Y * ScaleFactor, 0).SpaceToGrid(XRotation, offset)) 'Y
        Call Device.Graphics.DrawLine(Pens.Black, New Drawing3D.Point3D(0, 0, Max.Z * ScaleFactor).SpaceToGrid(XRotation, offset), New Drawing3D.Point3D(0, 0, Min.Z * ScaleFactor).SpaceToGrid(XRotation, offset)) 'Z

        Return Device.ImageResource
    End Function

    <Extension>
    Private Sub __drawingOfAA(AA As AminoAcid, ByRef pt2d As PointF, offset As Point, Device As Graphics2D, DisplayAAID As Boolean, AAFont As Font, hideAtoms As Boolean)
        Dim Carbon As Keywords.AtomUnit = AA.Carbon
        Dim pt3d As New Drawing3D.Point3D(Carbon.Location.X * ScaleFactor, Carbon.Location.Y * ScaleFactor, Carbon.Location.Z * ScaleFactor)
        pt2d = pt3d.SpaceToGrid(xRotate:=XRotation, offset:=offset)
        Call Device.Graphics.FillEllipse(Brushes.Black, New RectangleF(pt2d, New Size(penWidth, penWidth)))

        If DisplayAAID Then
            Call Device.Graphics.DrawString(AA.AA_ID, AAFont, Brushes.Gray, pt2d)
        End If

        If hideAtoms Then Return

        For Each Atom In AA.Atoms
            Dim pt = New Drawing3D.Point3D(Atom.Location.X * ScaleFactor, Atom.Location.Y * ScaleFactor, Atom.Location.Z * ScaleFactor)
            Dim pt22d = pt.SpaceToGrid(XRotation, offset)
            Call Device.Graphics.DrawLine(Pens.Gray, pt22d, pt2d)
        Next
    End Sub
End Module
