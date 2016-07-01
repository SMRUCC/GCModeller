#Region "Microsoft.VisualBasic::f20b9e20a905ae6e515ce3d7fea04b19, ..\GCModeller\visualize\visualizeTools\ComparativeGenomics\ShellScriptAPI.vb"

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

Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.GCModeller.DataVisualization

Namespace ComparativeGenomics

    <[Namespace]("data.visualization.Comparative_Genomics")>
    Module ShellScriptAPI

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
                    Dim G1 = GBFF.File.Read(File1)
                    Dim File2 As String = String.Format("{0}/{1}.gbk", gbDIR, FileTokens.Last)
                    If Not FileIO.FileSystem.FileExists(File2) Then
                        File2 = String.Format("{0}/{1}.1.gbk", gbDIR, FileTokens.Last)
                    End If
                    Dim G2 = GBFF.File.Read(File2)
                    Dim Model = ModelFromGBK(G1, G2)
                    Call LinkFromBesthit(Besthits, Model)
                    Dim res As Image = New DrawingDevice().InvokeDrawing(Model)
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
            Return GBFF.File.Read(path)
        End Function

        <ExportAPI("invoke.drawing")>
        Public Function InvokeDrawing(Model As DrawingModel) As Image
            Return New DrawingDevice().InvokeDrawing(Model)
        End Function

        <ExportAPI("model.from_gbk")>
        Public Function ModelFromGBK(GBk1 As GBFF.File, GBk2 As GBFF.File) As DrawingModel
            Return ModelFromPTT(GbkffExportToPTT(GBk1), GbkffExportToPTT(GBk2))
        End Function

        <ExportAPI("model.from_ptt")>
        Public Function ModelFromPTT(a As TabularFormat.PTT, b As TabularFormat.PTT) As DrawingModel
            Dim COGs As String() = New List(Of String)(From gene As GeneBrief
                                                       In a.GeneObjects
                                                       Select gene.COG) + From gene As GeneBrief
                                                                          In b.GeneObjects
                                                                          Select gene.COG
            Dim colours As Dictionary(Of String, Brush) =
                RenderingColor.InitCOGColors(COGs) _
               .ToDictionary(Function(cl) cl.Key,
                             Function(x) DirectCast(New SolidBrush(x.Value), Brush))
            Return New DrawingModel With {
                .Genome1 = ModelAPI.CreateObject(a, colours),
                .Genome2 = ModelAPI.CreateObject(b, colours)
            }
        End Function

        <ExportAPI("model.add_links_from_besthit")>
        Public Function LinkFromBesthit(besthit As IEnumerable(Of BestHit), ByRef model As DrawingModel) As DrawingModel
            model.Links = LinqAPI.Exec(Of GeneLink) <= From hit As BestHit
                                                       In besthit
                                                       Where Not String.Equals(hit.HitName, IBlastOutput.HITS_NOT_FOUND)
                                                       Select New GeneLink With {
                                                           .genome1 = hit.QueryName,
                                                           .genome2 = hit.HitName
                                                       }
            Return model
        End Function
    End Module
End Namespace
