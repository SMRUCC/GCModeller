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