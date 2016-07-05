#Region "Microsoft.VisualBasic::c1be60d20cb9b86e9a1db5199781cb89, ..\GCModeller\CLI_tools\S.M.A.R.T\DrawingPDB.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.RCSB.PDB
Imports SMRUCC.genomics.Visualize

<[PackageNamespace]("Drawing.Protein.3D.PDB",
                    Description:="Visualize the protein 3D structure from the PDB file.",
                    Publisher:="amethyst.asuka@gcmodeller.org", Url:="http://gcmodeller.org")>
Public Module DrawingPDB

    <DataFrameColumn("X.Rotation")> Public XRotation As Double = 60
    <DataFrameColumn("Scale.Factor")> Public ScaleFactor As Double = 20
    <DataFrameColumn("Pen.Width")> Public penWidth As Integer = 10

    <ExportAPI("Drawing.Invoke", Info:="Drawing a protein structure from its pdb data.")>
    Public Function MolDrawing(PDB As PDB, Optional hideAtoms As Boolean = True, Optional DisplayAAID As Boolean = True) As Image
        Dim Device As GDIPlusDeviceHandle = (New Size(3000, 3000)).CreateGDIDevice
        Dim offset As Point = Device.Center
        Dim AASequence = PDB.AminoAcidSequenceData
        Dim PreAA = AASequence.First
        Dim PrePoint As Point
        Dim aas As String() = (From AA In AASequence Select AA.AA_ID Distinct).ToArray
        Dim AAColors = (From cl In RenderingColor.InitCOGColors(aas)
                        Select ID = cl.Key,
                            br = New Pen(New SolidBrush(cl.Value), penWidth)) _
                           .ToDictionary(Function(item) item.ID,
                                         Function(item) item.br)
        Dim AAFont As New Font(FontFace.MicrosoftYaHei, 10)

        Call __drawingOfAA(PreAA, PrePoint, offset, Device, DisplayAAID, AAFont, hideAtoms)

        For Each Point As AminoAcid In AASequence
            Dim pt2d As Point

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

    Private Sub __drawingOfAA(AA As AminoAcid, ByRef pt2d As Point, offset As Point, Device As GDIPlusDeviceHandle, DisplayAAID As Boolean, AAFont As Font, hideAtoms As Boolean)
        Dim Carbon As Keywords.AtomUnit = AA.Carbon
        Dim pt3d As Drawing3D.Point3D = New Drawing3D.Point3D(Carbon.Location.X * ScaleFactor, Carbon.Location.Y * ScaleFactor, Carbon.Location.Z * ScaleFactor)
        pt2d = pt3d.SpaceToGrid(xRotate:=XRotation, offset:=offset)
        Call Device.Graphics.FillPie(Brushes.Black, New Rectangle(pt2d, New Size(penWidth, penWidth)), 0, 360)

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

