#Region "Microsoft.VisualBasic::df9cb19e8f9b66f80cf13179588cfe03, R#\phenotype_kit\WGCNA.vb"

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


    ' Code Statistics:

    '   Total Lines: 49
    '    Code Lines: 43
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.91 KB


    ' Module WGCNA
    ' 
    '     Function: applyModuleColors, readModules, readWeightMatrix
    ' 
    ' 
    ' /********************************************************************************/

#End Region

#If netcore5 = 0 Then
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.Network
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports any = Microsoft.VisualBasic.Scripting

<Package("WGCNA")>
Module WGCNA

    <ExportAPI("read.modules")>
    Public Function readModules(file As String, Optional prefix$ = Nothing) As Object
        Return WGCNAModules _
            .LoadModules(file) _
            .ToDictionary(Function(g)
                              Return If(prefix Is Nothing, g.nodeName, prefix & g.nodeName)
                          End Function,
                          Function(g)
                              Return CObj(g.nodesPresent)
                          End Function) _
            .DoCall(Function(mods)
                        Return New list With {
                            .slots = mods
                        }
                    End Function)
    End Function

    <ExportAPI("read.weightMatrix")>
    Public Function readWeightMatrix(file As String, Optional threshold As Double = 0, Optional prefix$ = Nothing) As WGCNAWeight
        Return FastImports(path:=file, threshold:=threshold, prefix:=prefix)
    End Function

    <ExportAPI("applyModuleColors")>
    Public Function applyModuleColors(g As NetworkGraph, modules As list) As Object
        For Each geneId As String In modules.getNames
            If Not g.GetElementByID(geneId) Is Nothing Then
                g.GetElementByID(geneId).data.color = any.ToString(modules(geneId)).GetBrush
            End If
        Next

        Return g
    End Function
End Module

#End If
