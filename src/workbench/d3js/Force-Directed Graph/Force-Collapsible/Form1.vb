#Region "Microsoft.VisualBasic::7b81cc64e3eeec307644473e612360a7, ..\workbench\d3js\Force-Directed Graph\Force-Collapsible\Form1.vb"

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

Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call NetworkGenerator.LoadJson("G:\1.13.RegPrecise_network\Cellular Phenotypes\KEGG_Modules-SimpleModsNET").SaveTo("F:\GCModeller.Workbench\d3js\Force-Directed Graph\test.json")

        Call NetworkGenerator.LoadJson("G:\4.15\MEME\footprints\xcb-modules.TestFootprints2,2.csv").SaveTo("G:\4.15\MEME\footprints\xcb-modules.TestFootprints2,2.json")
        Call NetworkGenerator.LoadJson("G:\4.15\MEME\footprints\xcb-pathways.TestFootprints2,2.csv").SaveTo("G:\4.15\MEME\footprints\xcb-pathways.TestFootprints2,2.json")

    End Sub
End Class

