#Region "Microsoft.VisualBasic::8c4619ba94e66d77e2b576ca2d27232d, annotations\KEGG\PathwayMap.vb"

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

    ' Module PathwayMapVisualize
    ' 
    '     Function: BuildModel, DEGsPathwayMap, LocalRenderMaps
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.GCModeller.Workbench.KEGGReport
Imports gene = Microsoft.VisualBasic.Data.csv.IO.EntityObject

''' <summary>
''' 这个仅支持代谢反应过程，即<see cref="PathwayMap.KEGGReaction"/>集合不能够为空
''' </summary>
Public Module PathwayMapVisualize

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function LocalRenderMaps(idlist$(), repo$, output$, Optional scale$ = "1,1") As NamedValue(Of String)()
        Return PathwayMapRender.RenderMaps(repo, idlist, output, scale:=scale)
    End Function

    ''' <summary>
    ''' 这个仅支持代谢反应过程，即<see cref="PathwayMap.KEGGReaction"/>集合不能够为空
    ''' </summary>
    ''' <param name="ref"></param>
    ''' <param name="reaction"></param>
    ''' <param name="compounds"></param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildModel(ref As PathwayMap, reaction As IRepositoryRead(Of String, Reaction), compounds As IRepositoryRead(Of String, Compound)) As NetworkTables
        If ref.KEGGReaction.IsNullOrEmpty Then
            Return Nothing
        End If


    End Function

    ''' <summary>
    ''' KEGG Mapper – Search&amp;Color Pathway
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <param name="logFC$"></param>
    ''' <param name="logCut#"></param>
    ''' <param name="colorUP$"></param>
    ''' <param name="colorDown$"></param>
    ''' <param name="colorNormal$"></param>
    ''' <param name="KO$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function DEGsPathwayMap(genes As IEnumerable(Of gene),
                                   Optional logFC$ = "logFC",
                                   Optional logCut# = 1.0R,
                                   Optional colorUP$ = "red",
                                   Optional colorDown$ = "blue",
                                   Optional colorNormal$ = "green",
                                   Optional KO$ = "KO",
                                   Optional DEP As Boolean = False) As String

        Dim out As New List(Of String)
        Dim up = 1
        Dim down = -1

        If DEP Then
            up = Math.Log(1.5, 2)
            down = -up
        End If

        For Each gene As gene In genes
            Dim logValue = gene(logFC).ParseNumeric
            Dim bgColor$
            Dim KEGG As String = Strings.Trim(gene(KO))

            If String.IsNullOrEmpty(KEGG) Then
                Continue For
            End If

            If logValue >= up Then
                bgColor = colorUP
            ElseIf logValue <= down Then
                bgColor = colorDown
            Else
                bgColor = colorNormal
            End If

            For Each tag As String In KEGG.Split(";"c).Select(AddressOf Strings.Trim)
                out += $"{tag} {bgColor},black"
            Next
        Next

        Return out.JoinBy(ASCII.LF)
    End Function
End Module
