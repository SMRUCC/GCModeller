#Region "Microsoft.VisualBasic::4c72472f74104c5ec205b73c7a6588eb, CLI_tools\KEGG\Tools\SystemDistribution.vb"

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

    ' Module SystemDistribution
    ' 
    '     Function: InvokeDrawing
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Imaging.Drawing2D

<Package("Pathway.System.Distribution", Publisher:="xie.guigang@gmail.com")>
Public Module SystemDistribution

    <DataFrameColumn("Margin")> Dim Margin As Integer

    <ExportAPI("Invoke.Drawing")>
    Public Function InvokeDrawing(MAT As IO.File) As Image
        Dim Organism = bGetObject.Organism.GetOrganismListFromResource
        Dim LQuery = (From row In MAT.Skip(1).AsParallel
                      Let sp As String = row.First
                      Let system As Integer() = (From s As String In row.Skip(1) Select CInt(Val(s))).ToArray
                      Select sp, system, org = Organism(sp)
                      Group By org.Phylum Into Group).ToArray

        Using device = New Size(MAT.Width * 100 + 2 * Margin, MAT.Count * 2 + 2 * Margin).CreateGDIDevice
            Dim X As Integer, Y As Integer
            Dim DrawingFont As New Font(FontFace.MicrosoftYaHei, 8)
            Dim Size As SizeF = LQuery.First.Group.First.sp.MeasureSize(device, DrawingFont)

            For Each row In LQuery
                Dim Rect As Rectangle = New Rectangle(X, Y, device.Width, Size.Height * row.Group.Count)
                Call device.Graphics.DrawRectangle(Pens.Black, Rect)

                X = Margin
                Y += 2

                For Each sp In row.Group
                    Size = device.Graphics.MeasureString(sp.sp, DrawingFont)
                    Call device.Graphics.DrawString(sp.sp, DrawingFont, Brushes.Black, New Point(X, Y))

                    X += Size.Width

                    For Each sys In sp.system
                        Dim Color As Color = If(sys = 0, Color.White, Color.Red)
                        Call device.Graphics.FillRectangle(New SolidBrush(Color), New Rectangle(New Point(X, Y), New Size(80, 2)))
                        X += 90
                    Next

                    Y += Size.Height
                    X = Margin
                Next
            Next

            Return device.ImageResource
        End Using
    End Function
End Module
