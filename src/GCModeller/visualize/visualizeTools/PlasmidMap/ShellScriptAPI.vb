#Region "Microsoft.VisualBasic::50be803fc8accc9ebd2f1b29c83f19ab, ..\visualize\visualizeTools\PlasmidMap\ShellScriptAPI.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace PlasmidMap

    <Package("Data.Visualization.Plasmid_Map", Description:="Data visualization module for the bacteria plasmid object.", Category:=APICategories.UtilityTools)>
    Public Module ShellScriptAPI

#Const DEBUG = 1

#If DEBUG Then
        <ExportAPI("test_debug()", Info:="Just for debugging...")>
        Public Function TestDEBUG() As Boolean
            Dim model As New PlasmidMap.PlasmidMapDrawingModel With {
                .GeneObjects = {New DrawingModels.SegmentObject With {
                .LocusTag = "TEST_1",
                .Direction = 0,
                .CommonName = "TEST_Annotations_TEXT",
                .Left = 100,
                .Right = 200,
                .GenomeLength = 600,
                .Color = Color.Black}}}
            Call New DrawingDevice().InvokeDrawing(model).Save(My.Computer.FileSystem.SpecialDirectories.Temp & "/Test.bmp")
            Return True
        End Function
#End If
    End Module
End Namespace
