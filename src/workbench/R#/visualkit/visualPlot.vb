#Region "Microsoft.VisualBasic::79a539985f9dd52d121d7b7d090bce1b, R#\visualkit\visualPlot.vb"

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

' Module visualPlot
' 
'     Function: KEGGCategoryProfilePlots
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Visualize.CatalogProfiling
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("visualkit.plots")>
Module visualPlot

    ''' <summary>
    ''' Do plot of the given kegg pathway profiles data
    ''' </summary>
    ''' <param name="profiles"></param>
    ''' <param name="title"></param>
    ''' <param name="axisTitle"></param>
    ''' <param name="size"></param>
    ''' <param name="tick"></param>
    ''' <param name="colors"></param>
    ''' <param name="displays"></param>
    ''' <returns></returns>
    <ExportAPI("kegg.category_profiles.plot")>
    <RApiReturn(GetType(GraphicsData))>
    Public Function KEGGCategoryProfilePlots(profiles As Object,
                                             Optional title$ = "KEGG Orthology Profiling",
                                             Optional axisTitle$ = "Number Of Proteins",
                                             <RRawVectorArgument>
                                             Optional size As Object = "2300,2000",
                                             Optional tick# = -1,
                                             <RRawVectorArgument>
                                             Optional colors As Object = "#E41A1C,#377EB8,#4DAF4A,#984EA3,#FF7F00,#CECE00",
                                             Optional displays% = 10,
                                             Optional env As Environment = Nothing) As Object

        Dim profile As Dictionary(Of String, NamedValue(Of Double)())

        If TypeOf profiles Is Dictionary(Of String, Integer) Then
            profile = DirectCast(profiles, Dictionary(Of String, Integer)) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return CDbl(a.Value)
                              End Function) _
                .doKeggProfiles(displays)
        ElseIf TypeOf profiles Is Dictionary(Of String, NamedValue(Of Double)()) Then
            profile = DirectCast(profiles, Dictionary(Of String, NamedValue(Of Double)()))
        ElseIf TypeOf profiles Is Dictionary(Of String, Double) Then
            profile = DirectCast(profiles, Dictionary(Of String, Double)).doKeggProfiles(displays)
        ElseIf TypeOf profiles Is list Then
            profile = DirectCast(profiles, list).slots _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return CDbl(REnv.asVector(Of Double)(a.Value).GetValue(Scan0))
                              End Function) _
                .doKeggProfiles(displays)
        Else
            Return Internal.debug.stop("invalid data type for plot kegg category profile plot!", env)
        End If

        Return profile.ProfilesPlot(title,
            size:=InteropArgumentHelper.getSize(size),
            tick:=tick,
            axisTitle:=axisTitle,
            labelRightAlignment:=False,
            valueFormat:="F0",
            colorSchema:=colors
        )
    End Function

    <Extension>
    Private Function doKeggProfiles(profiles As Dictionary(Of String, Double), displays%) As Dictionary(Of String, NamedValue(Of Double)())
        Return profiles _
            .KEGGCategoryProfiles _
            .Where(Function(cls) Not cls.Value.IsNullOrEmpty) _
            .ToDictionary(Function(p) p.Key,
                          Function(group)
                              Return group.Value _
                                  .OrderByDescending(Function(t) t.Value) _
                                  .Take(displays) _
                                  .ToArray
                          End Function)
    End Function

End Module

