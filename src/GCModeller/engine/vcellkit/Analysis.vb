﻿#Region "Microsoft.VisualBasic::f048cf1e2ce14196c58c4cd2dbe952f4, engine\vcellkit\Analysis.vb"

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

    '   Total Lines: 113
    '    Code Lines: 79 (69.91%)
    ' Comment Lines: 24 (21.24%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (8.85%)
    '     File Size: 4.33 KB


    ' Module Analysis
    ' 
    '     Function: CompoundNames, UnionSnapshot, vcellGraph
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' GCModeller virtual cell analysis toolkit.
''' </summary>
<Package("analysis")>
Public Module Analysis

    ''' <summary>
    ''' union of the profile snapshot list to a matrix dataset.
    ''' </summary>
    ''' <param name="result"></param>
    ''' <param name="setName"></param>
    ''' <param name="trim">
    ''' Will delete all of the metabolites row that have no changes
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("union.matrix")>
    Public Function UnionSnapshot(result$, Optional setName$ = "mass\metabolome.json", Optional trim As Boolean = True) As DataSet()
        Dim metabolites As New List(Of DataSet)
        Dim data As Dictionary(Of String, Double)

        For Each dir As String In ls - l - lsDIR <= result
            Dim name = dir.BaseName
            Dim file$ = $"{dir}/{setName}"

            If Not file.FileExists() Then
                For Each folder As String In ls - l - lsDIR <= dir
                    name = folder.BaseName
                    data = $"{folder}/{setName}".LoadJsonFile(Of Dictionary(Of String, Double))
                    metabolites += New DataSet With {
                        .ID = name,
                        .Properties = data
                    }
                Next
            Else
                data = file.LoadJsonFile(Of Dictionary(Of String, Double))
                metabolites += New DataSet With {
                    .ID = name,
                    .Properties = data
                }
            End If
        Next

        If trim Then
            Return metabolites _
                .Transpose _
                .Where(Function(r)
                           Dim first As Double = r.Properties.Values.First
                           Dim testAllEquals As Boolean = r _
                              .Properties _
                              .Values _
                              .Any(Function(x) x <> first)

                           Return testAllEquals
                       End Function) _
                .ToArray
        Else
            Return metabolites.Transpose
        End If
    End Function

    ''' <summary>
    ''' set compound names for the kegg metabolites
    ''' </summary>
    ''' <param name="metabolites"></param>
    ''' <param name="names"></param>
    ''' <returns></returns>
    <ExportAPI("compound.names")>
    Public Function CompoundNames(metabolites As DataSet(), names As Dictionary(Of String, String)) As DataSet()
        If Not names.IsNullOrEmpty Then
            For Each compound As DataSet In metabolites
                If names.ContainsKey(compound.ID) Then
                    compound.ID = names(compound.ID)
                End If
            Next
        End If

        Return metabolites
    End Function

    ''' <summary>
    ''' Export the cellular graph data from the virtual cell simulation engine
    ''' </summary>
    ''' <param name="vcell"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("vcell.graph")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function vcellGraph(vcell As Object, Optional env As Environment = Nothing) As Object
        If vcell Is Nothing Then
            Return Nothing
        ElseIf TypeOf vcell Is Engine Then
            vcell = DirectCast(vcell, Engine).getCore
        ElseIf Not TypeOf vcell Is Vessel Then
            Return RInternal.debug.stop($"invalid model type: {vcell.GetType.FullName}!", env)
        End If

        Return DirectCast(vcell, Vessel).ToGraph
    End Function
End Module
