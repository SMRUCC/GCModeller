#Region "Microsoft.VisualBasic::9d52e720ae019f239b7e287e85d894da, visualize\SyntenyVisual\ComparativeGenomics\ShellScriptAPI.vb"

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

    '     Module ShellScriptAPI
    ' 
    '         Function: BatchDrawing, InvokeDrawing, LinkFromBesthit, LinkFromBlastnMaps, LogScore
    '                   ModelFromGBK, ModelFromGFF, ModelFromPTT, ReadGBK, SyntenyTuple
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Framework.Extensions
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput

#If NET48 Then
Imports Brush = System.Drawing.Brush
Imports SolidBrush = System.Drawing.SolidBrush
Imports Brushes = System.Drawing.Brushes
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

Namespace ComparativeGenomics

    <[Namespace]("data.visualization.Comparative_Genomics")>
    Public Module ShellScriptAPI

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="DIR">besthitde的批量导出的文件夹</param>
        ''' <param name="gbDIR">gbk文件夹</param>
        ''' <param name="EXPORT">绘图数据的导出文件夹</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("fastview.besthits")>
        Public Function BatchDrawing(DIR As String, gbDIR As String, EXPORT As String) As Boolean
            For Each df As String In ls - l - wildcards("*.csv") <= DIR
                Try
                    Dim FileTokens = Strings.Split(FileIO.FileSystem.GetFileInfo(df).Name.Replace(".txt.besthit.csv", "").Replace(".fasta", ""), "__vs_")
                    Dim Besthits = df.LoadCsv(Of BestHit)(False)
                    Dim File1 As String = String.Format("{0}/{1}.gbk", gbDIR, FileTokens.First)
                    If Not FileIO.FileSystem.FileExists(File1) Then
                        File1 = String.Format("{0}/{1}.1.gbk", gbDIR, FileTokens.First)
                    End If
                    Dim G1 = GBFF.File.Load(File1)
                    Dim File2 As String = String.Format("{0}/{1}.gbk", gbDIR, FileTokens.Last)
                    If Not FileIO.FileSystem.FileExists(File2) Then
                        File2 = String.Format("{0}/{1}.1.gbk", gbDIR, FileTokens.Last)
                    End If
                    Dim G2 = GBFF.File.Load(File2)
                    Dim Model = ModelFromGBK(G1, G2)
                    Call LinkFromBesthit(Besthits, Model)
                    Dim res As GraphicsData = New DrawingDevice().Plot(Model)
                    Call res.Save(EXPORT & "/" & FileIO.FileSystem.GetFileInfo(df).Name & ".bmp")
                Catch ex As Exception
                    ex = New Exception(df.ToFileURL, ex)
                    Call App.LogException(ex)
                    Call VBDebugger.PrintException(df.ToFileURL)
                End Try
            Next

            Return True
        End Function

        <ExportAPI("read.gbk")>
        Public Function ReadGBK(path As String) As GBFF.File
            Return GBFF.File.Load(path)
        End Function

        <ExportAPI("invoke.drawing")>
        Public Function InvokeDrawing(Model As DrawingModel) As GraphicsData
            Return New DrawingDevice().Plot(Model)
        End Function

        <ExportAPI("model.from_gbk")>
        Public Function ModelFromGBK(GBk1 As GBFF.File, GBk2 As GBFF.File) As DrawingModel
            Return ModelFromPTT(GBk1.GbffToPTT(ORF:=True), GBk2.GbffToPTT(ORF:=True))
        End Function

        <ExportAPI("model.from_ptt")>
        Public Function ModelFromPTT(a As PTT, b As PTT) As DrawingModel
            Dim COGs As String() = a.GeneObjects.COGs.AsList + From gene As GeneBrief
                                                               In b.GeneObjects
                                                               Select gene.COG
            Dim colours As Dictionary(Of String, Brush) = RenderingColor _
                .InitCOGColors(COGs) _
                .ToDictionary(Function(cl) cl.Key,
                              Function(x) DirectCast(New SolidBrush(x.Value), Brush))

            Return New DrawingModel With {
                .Genome1 = ModelAPI.CreateObject(a, colours),
                .Genome2 = ModelAPI.CreateObject(b, colours)
            }
        End Function

        <ExportAPI("model.from_gff")>
        Public Function ModelFromGFF(a As GFFTable, b As GFFTable) As DrawingModel
            Dim genomeA As GenomeModel = GenomeModel.FromGffTable(a)
            Dim genomeB As GenomeModel = GenomeModel.FromGffTable(b)

            Return New DrawingModel With {
                .Genome1 = genomeA,
                .Genome2 = genomeB
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function SyntenyTuple(compares As (a As GFFTable, b As GFFTable)) As DrawingModel
            Return ModelFromGFF(compares.a, compares.b)
        End Function

        <ExportAPI("model.add_links_from_besthit")>
        Public Function LinkFromBesthit(besthit As IEnumerable(Of BestHit), ByRef model As DrawingModel) As DrawingModel
            model.Links = LinqAPI.Exec(Of GeneLink) _
                () <= From hit As BestHit
                      In besthit
                      Where Not String.Equals(hit.HitName, IBlastOutput.HITS_NOT_FOUND)
                      Select New GeneLink With {
                          .genome1 = hit.QueryName,
                          .genome2 = hit.HitName
                      }

            Return model
        End Function

        <Extension>
        Public Function LogScore(Of T)(data As IEnumerable(Of T), getScore As Func(Of T, Double)) As Vector
            Dim raw = data _
                .Select(Function(x)
                            Dim score = getScore(x)

                            If score = 0R Then
                                Return Double.MinValue
                            Else
                                Return -Math.Log(score, 10)
                            End If
                        End Function) _
                .ToArray
            Dim max = raw.Max + 1

            For i As Integer = 0 To raw.Length - 1
                If raw(i) <= Double.MinValue + 10 Then
                    raw(i) = max
                End If
            Next

            Return raw
        End Function

        <Extension>
        Public Function LinkFromBlastnMaps(model As DrawingModel,
                                           maps As IEnumerable(Of BlastnMapping),
                                           Optional grepOp As TextGrepScriptEngine = Nothing,
                                           Optional ribbonColors$ = "Set1:c6") As DrawingModel

            Dim grep As TextGrepMethod = (grepOp Or TextGrepScriptEngine.DoNothing).PipelinePointer
            Dim colors As Color() = Designer.GetColors(ribbonColors, 30)
            Dim mapsVector = maps.ToArray
            Dim scores As Vector = mapsVector.Select(Function(d) d.identitiesValue).AsVector  ' maps.LogScore(Function(m) m.Evalue)
            Dim colorIndex As New DoubleRange(scores)
            Dim indexRange As New DoubleRange(0, colors.Length - 1)

            model.Links = mapsVector _
                .Select(Function(m, i)
                            Dim score# = scores(i)
                            Dim index% = colorIndex.ScaleMapping(score, indexRange)

                            Return New GeneLink With {
                                .genome1 = grep(m.ReadQuery),
                                .genome2 = grep(m.Reference),
                                .Score = score,
                                .Color = colors(index)
                            }
                        End Function) _
                .ToArray
            model.RibbonScoreColors = (colorIndex, colors)

            Return model
        End Function
    End Module
End Namespace
