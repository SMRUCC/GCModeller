#Region "Microsoft.VisualBasic::845118f1af127c926f14cf80a0c0613b, ..\GCModeller\visualize\visualizeTools\NCBIBlastResult\BlastVisualize.vb"

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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Terminal
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.ComparativeGenomics

Namespace NCBIBlastResult

    <[PackageNamespace]("NCBI.Visualization.BLAST",
                        Description:="Visualization of the blast result",
                        Category:=APICategories.ResearchTools,
                        Publisher:="xie.guigang@gmail.com")>
    Public Module BlastVisualize

        <ExportAPI("Invoke.Drawing")>
        Public Function InvokeDrawing(blast As v228, query As String) As Image
            Dim LQuery = (From x In blast.Queries.AsParallel
                          Where String.Equals(query, x.QueryName, StringComparison.OrdinalIgnoreCase)
                          Select x).FirstOrDefault
            If LQuery Is Nothing Then
                Return Nothing
            End If
            Return InvokeDrawing(LQuery)
        End Function

        <ExportAPI("Invoke.Drawing")>
        Public Function InvokeDrawing(Query As Query) As Image
            Dim Gr = New Size(Margin * 3 + Query.QueryLength, 2 * Margin + 100).CreateGDIDevice
            Dim Y As Integer = Margin / 2
            Dim rect As Rectangle = New Rectangle(New Point(Margin, Y), New Size(Query.QueryLength, 10))
            Y += rect.Height
            Dim font As New Font(FontFace.Ubuntu, 6, FontStyle.Regular)

            Call Gr.Graphics.FillRectangle(Brushes.Black, rect)

            Dim sz = Gr.Graphics.MeasureString("0", font)

            For i As Integer = 0 To Query.QueryLength Step 10
                If i Mod 50 = 0 Then
                    Dim YY As Integer = Y + 5
                    Call Gr.Graphics.DrawLine(New Pen(Color.Black, 2), New Point(i + Margin, YY), New Point(i + Margin, Y))   ' 大标尺
                    sz = Gr.Graphics.MeasureString(i, font)
                    Call Gr.Graphics.DrawString(i, font, Brushes.Black, New Point(sz.Width / 2 + i + Margin, YY))
                Else
                    Call Gr.Graphics.DrawLine(New Pen(Color.Gray, 1), New Point(i + Margin, Y + 3), New Point(i + Margin, Y)) ' 小标尺
                End If
            Next

            Call Gr.Graphics.DrawString($"({Query.QueryLength})", font, Brushes.Black, New Point(Query.QueryLength + Margin + 5, Y + 5))

            Y += (10 + sz.Height)
            font = New Font(FontFace.Ubuntu, 8, FontStyle.Regular)

            For Each hit In Query.SubjectHits
                Dim loci = hit.QueryLocation
#If DEBUG Then
                Call loci.__DEBUG_ECHO
#End If
                rect = New Rectangle(New Point(Margin + loci.Left, Y), New Size(loci.FragmentSize, 10))
                Call Gr.Graphics.FillRectangle(Brushes.Blue, rect)
                Dim x As Integer = Gr.Graphics.MeasureString(hit.Name, font).Width
                x = rect.Left + (rect.Width - x) / 2
                Call Gr.Graphics.DrawString(hit.Name, font, brush:=Brushes.Gray, point:=New PointF(x, rect.Bottom + 2))
                Y += 32
            Next

            Return Gr.ImageResource
        End Function

        <DataFrameColumn("margin")> Dim Margin As Integer = 100
        <DataFrameColumn("font.size")> Dim FontSize As Integer = 12

        ''' <summary>
        ''' 一个碱基或者一个氨基酸所对应的像素
        ''' </summary>
        ''' <remarks></remarks>
        <DataFrameColumn("convert.factor")> Dim ConvertFactor As Double = 0.1

        <ExportAPI("alignment_dump.from.blastx")>
        Public Function AlignmentTableFromBlastX(<Parameter("dir.source", "The directory which contains the blastx output data.")> source As String) As AlignmentTable
            Return ParserAPI.CreateFromBlastX(source)
        End Function

        <ExportAPI("alignment_dump.from.blastn")>
        Public Function AlignmentTableFromBlastn(<Parameter("dir.source", "The directory which contains the blastn output data.")> source As String) As AlignmentTable
            Return ParserAPI.CreateFromBlastn(source)
        End Function

        Private Function InternalShortID_s(srcFasta As FASTA.FastaToken) As String
            Dim ss As String = Regex.Match(srcFasta.Title, "(emb|gb|dbj)\|[a-z]+\d+(\.\d+)?", RegexOptions.IgnoreCase).Value

            If Not String.IsNullOrEmpty(ss) Then
                Dim p As String = Regex.Match(srcFasta.Title, "\d+-\d+").Value
                Return ss.Split(CChar("|")).Last.Split(CChar(".")).First & "_" & p.Split(CChar("-")).First & "&"
            Else
                Return srcFasta.Title
            End If
        End Function

        ''' <summary>
        ''' 对Entry信息进行简化
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("alignment_dump.def2id",
                   Info:="if the export parameter is not empty then the function will export the entry information into the target directory.")>
        Public Function ShortID(<Parameter("data.fasta")> data As FASTA.FastaFile,
                                <Parameter("dir.export",
                                           "if this dir path parameter is not empty then the function will export the entry information into the directory that this parameter specificed.")>
                                Optional EXPORT As String = "") As FASTA.FastaFile

            Dim setValue = New SetValue(Of FastaToken) <= NameOf(FastaToken.Attributes)
            Dim LQuery = (From srcFasta As FASTA.FastaToken
                          In data
                          Let ShortID_s As String = InternalShortID_s(srcFasta)
                          Select ShortID_s,
                              srcFasta.Title,
                              Fasta = setValue(srcFasta, New String() {ShortID_s})).ToArray
            Dim FastaData As New FASTA.FastaFile(From ShortIDFasta In LQuery Select ShortIDFasta.Fasta)

            If String.IsNullOrEmpty(EXPORT) Then Return FastaData
            Dim TableGrouped = (From title In LQuery Select title.ShortID_s, title.Title Group By ShortID_s Into Group).ToArray
            Dim TableDistinct = (From srcFasta In TableGrouped Select srcFasta.ShortID_s, Title = srcFasta.Group.First).ToArray

            Call TableDistinct.SaveTo(EXPORT, explicit:=False)

            EXPORT = FileIO.FileSystem.GetParentPath(EXPORT) & "/" & IO.Path.GetFileNameWithoutExtension(EXPORT) & "/"

            For Each fa In LQuery
                Call fa.Fasta.SaveTo(EXPORT & fa.ShortID_s & ".fasta")
            Next

            Return FastaData
        End Function

        <ExportAPI("export.order.gi", Info:="Export the drawing order of the species hits on the graphics.")>
        Public Function ExportTableOrderByGI(Tab As AlignmentTable) As String()
            Return Tab.ExportOrderByGI
        End Function

        ''' <summary>
        ''' 将编号信息转换为描述信息
        ''' </summary>
        ''' <param name="table"></param>
        ''' <param name="info"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("alignment_table.gi2def")>
        Public Function ApplyDescription(Table As AlignmentTable,
                                         info As IEnumerable(Of gbEntryBrief),
                                         Optional MaxLength As Integer = 0) As AlignmentTable
            Call Table.DescriptionSubstituted(info.ToArray)
            Call Table.TrimLength(MaxLength)
            Return Table
        End Function

        <ExportAPI("alignment_table.get.subject_idlist")>
        Public Function GetSubjectHitLocusID(Table As AlignmentTable) As String()
            Return Table.GetHitsEntryList
        End Function

        ''' <summary>
        ''' 将编号信息转换为描述信息
        ''' </summary>
        ''' <param name="table"></param>
        ''' <param name="info"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("alignment_table.id2def", Info:="Please notices that this function requires the target hit its subjectids property is just the locus tag value.")>
        Public Function ApplyDescription2(Table As AlignmentTable,
                                          info As IEnumerable(Of gbEntryBrief),
                                          Optional MaxLength As Integer = 0) As AlignmentTable
            Call Table.DescriptionSubstituted2(info.ToArray)
            Call Table.TrimLength(MaxLength)
            Return Table
        End Function

        <ExportAPI("read.txt.blast_result", Info:="Read the blast output table result text file which was download from the NCBI blast website.")>
        Public Function LoadResult(path As String) As AlignmentTable
            Return ParserAPI.LoadDocument(path)
        End Function

        ''' <summary>
        ''' 这个函数主要是针对blastp的结果的
        ''' </summary>
        ''' <param name="source">blast输出日志文件夹</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("aligned_table.from_blastoutput")>
        Public Function CreateTableFromBlastOutput(<Parameter("source.dir", "The directory contains the blast output result text file.")> source As String,
                                                   <Parameter("query.id")> QueryID As String,
                                                   <Parameter("list.cds.info")> CdsInfo As IEnumerable(Of GeneDumpInfo)) As AlignmentTable
            Dim ResourceEntries = (From Entry In source.LoadSourceEntryList({"*.txt"})
                                   Where InStr(Entry.Key, QueryID, CompareMethod.Text) = 1
                                   Select ID = Entry.Key,
                                       Path = Entry.Value).ToArray
            Dim LoadBlastOutput = (From Entry In ResourceEntries.AsParallel
                                   Where FileIO.FileSystem.GetFileInfo(Entry.Path).Length > 0
                                   Let Output As v228 = TryParse(Entry.Path)
                                   Select Entry.ID,
                                       Output).ToArray
            Dim ORF As Dictionary(Of String, GeneDumpInfo) = CdsInfo.ToDictionary(Function(GeneObject) GeneObject.LocusID)
            Dim ChunkBuffer As HitRecord() = (From EntryInfo In LoadBlastOutput
                                              Let hits As HitRecord() = __createHits(ORF, EntryInfo.Output)
                                              Where Not hits.IsNullOrEmpty
                                              Select hits).MatrixToVector
            Dim Table As AlignmentTable =
                New AlignmentTable With {
                    .Database = source,
                    .Program = "LocalBLAST",
                    .Query = QueryID,
                    .RID = Now.ToString,
                    .Hits = ChunkBuffer
            }
            Return Table
        End Function

        ''' <summary>
        ''' 只需要找出有hits的query，然后将位置列举出来即可
        ''' </summary>
        ''' <param name="ORF"></param>
        ''' <param name="Blastoutput"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __createHits(ORF As Dictionary(Of String, GeneDumpInfo), Blastoutput As v228) As HitRecord()
            Dim Trim = (From query As Query
                        In Blastoutput.Queries
                        Where Not query.SubjectHits.IsNullOrEmpty
                        Select Query = ORF(query.QueryName.Split(CChar("|")).First),
                            query.SubjectHits
                        Order By Query.Left Ascending).ToArray
            Dim hits As String() =
                LinqAPI.Exec(Of String) <= From x
                                           In Trim
                                           Select From hit As SubjectHit
                                                  In x.SubjectHits
                                                  Select hit.Name
            Dim SortHits As GeneDumpInfo() =
                LinqAPI.Exec(Of GeneDumpInfo) <= From id As String
                                                 In hits
                                                 Where ORF.ContainsKey(id)
                                                 Select _orf = ORF(id)
                                                 Order By _orf.Left Ascending
            Dim OrderedHits As String() = SortHits.ToArray(Function(hit) hit.LocusID)

            If OrderedHits.IsNullOrEmpty Then Return New HitRecord() {}

            Dim TrimedQuery As New List(Of KeyValuePair(Of GeneDumpInfo, Double)())

            Dim p_Direction As Integer '方向
            Dim i As Integer

            Dim TempChunk As New List(Of KeyValuePair(Of GeneDumpInfo, Double))

            ' 只获取hit的连续片段
            Do While i < Trim.Length
                Dim Query = Trim(i)
                Dim __single As Boolean = True

                For Each hit As SubjectHit In Query.SubjectHits
                    Dim pHit As Integer = Array.IndexOf(OrderedHits, hit.Name)  '获取当前的编号
                    '向后移动query
                    Dim j = i + 1
                    p_Direction = 0

                    If j = Trim.Count Then
                        Exit For
                    End If

                    For Each h In Trim(j).SubjectHits '获取hits的移动方向
                        If pHit = OrderedHits.Count - 1 Then
                            If String.Equals(h.Name, OrderedHits(pHit - 1)) Then '最后一个元素只能向前移动
                                p_Direction = -1
                                Exit For
                            Else
                                Continue For  '最后一个元素已经无法再向后移动了
                            End If
                        End If

                        If String.Equals(h.Name, OrderedHits(pHit + 1)) Then '第一个元素只能够向后移动
                            p_Direction = 1
                            Exit For
                        ElseIf pHit > 0 AndAlso String.Equals(h.Name, OrderedHits(pHit - 1)) Then '最后一个元素只能向前移动
                            p_Direction = -1
                            Exit For
                        End If
                    Next

                    'p任然为零说明不连续，则跳过本循环
                    If p_Direction = 0 Then
                        __single = True
                        Continue For
                    Else '连续的
                        __single = False
                        TempChunk += New KeyValuePair(Of GeneDumpInfo, Double)(Query.Query, hit.Score.RawScore)
                    End If

                    Do While j < Trim.Count  '按照p方向前进，直到断裂为止

                        pHit += p_Direction '按照p的方向移动
                        Dim Match As Boolean = False

                        Query = Trim(j)

                        If pHit < 0 OrElse pHit = OrderedHits.Count Then
                            GoTo CONTINUTE    '已经到尽头了，断裂了
                        End If

                        Dim currentHit = OrderedHits(pHit)

                        For Each h In Query.SubjectHits '查找匹配
                            If String.Equals(h.Name, currentHit) Then '匹配成功
                                TempChunk += New KeyValuePair(Of GeneDumpInfo, Double)(Query.Query, h.Score.RawScore)
                                Match = True
                                Exit For
                            End If
                        Next

CONTINUTE:
                        If Not Match Then '断裂了
                            Call TrimedQuery.Add(TempChunk.ToArray)
                            Call TempChunk.Clear()
                            Exit Do
                        End If

                        j += 1
                    Loop

                    i = j
                    Continue Do
                Next

                If __single Then
                    TrimedQuery += {New KeyValuePair(Of GeneDumpInfo, Double)(Query.Query, Query.SubjectHits.First.Score.RawScore)}
                End If

                i += 1
            Loop

            Return LinqAPI.Exec(Of HitRecord) <=
                From TrimedData As KeyValuePair(Of GeneDumpInfo, Double)()
                In TrimedQuery
                Let loci As IEnumerable(Of Integer) =
                    (LinqAPI.MakeList(Of Integer) <= From GeneObject
                                                     In TrimedData
                                                     Select {GeneObject.Key.Left, GeneObject.Key.Right})  '
                Select New HitRecord With {
                    .QueryID = "",
                    .SubjectIDs = Blastoutput.Database,
                    .QueryStart = loci.Min,
                    .QueryEnd = loci.Max,
                    .BitScore = TrimedData.Average(Function(x) x.Value)
                }
        End Function

        <DataFrameColumn("Gene.Drawing.Object.Height")> Dim GeneObjectDrawingHeight As Integer = 25

        <ExportAPI("Cog.Class.Assign")>
        Public Sub AssignCogClass(PTT As PTTDbLoader,
                                  <Parameter("Class.Cog.Mappings",
                                             "The excel table object which contains the gene cog value.")> Mapping As DocumentStream.File,
                                  <Parameter("Mapping.Gene.ID",
                                             "The column name of the gene id in the execel table.")> Optional GeneID As String = "GeneID",
                                  <Parameter("Mapping.COG",
                                             "The column name of the cog class value in the excel table.")> Optional COG As String = "COG")

            Dim DF As DocumentStream.DataFrame = DocumentStream.DataFrame.CreateObject(Mapping)
            Dim CogValue As Dictionary(Of String, String) =
                DF.CreateDataSource.ToDictionary(Function(x) x.Attribute(GeneID), Function(x) x(COG))

            For Each gene As GeneBrief In PTT.Values
                If CogValue.ContainsKey(gene.Synonym) Then
                    gene.COG = CogValue(gene.Synonym)
                    If String.Equals(gene.COG, "-") Then
                        gene.COG = ""
                    End If
                Else
                    gene.COG = ""
                End If
            Next
        End Sub

        Private Function __COGsBrush(queryNoColor As Boolean,
                                     refQuery As PTT,
                                     COGTextureMappings As Boolean,
                                     TextureSource As String,
                                     ResourceIDMapping As Boolean,
                                     Device As GDIPlusDeviceHandle,
                                     MaxIDLength As Integer) As ICOGsBrush

            Dim COGsColor As Dictionary(Of String, Brush) = Nothing

            If Not queryNoColor Then
                Dim CogCategory As String() = (From gene As GeneBrief
                                               In refQuery.GeneObjects
                                               Where Not String.IsNullOrEmpty(gene.COG)
                                               Select gene.COG Distinct).ToArray

                If COGTextureMappings Then    ' 使用材质映射，假若没有设置资源文件夹，则使用系统默认的材质进行绘图
                    If String.IsNullOrEmpty(TextureSource) Then
                        Dim TextureList As Image() = TextureResourceLoader.LoadInternalDefaultResource.Shuffles
                        COGsColor = RenderingColor.CategoryMapsTextures(categories:=CogCategory, textures:=TextureList)
                    Else
                        Dim TextureList As TagValue(Of Image)() =
                            LinqAPI.Exec(Of TagValue(Of Image)) <= From path As String
                                                                   In ls - l - r - wildcards("*.bmp", "*.jpg", "*.png") <= TextureSource
                                                                   Select New TagValue(Of Image)(path.BaseName, path.LoadImage)

                        If ResourceIDMapping Then
                            COGsColor = TextureList.ToDictionary(Function(obj) obj.tag.ToUpper, Function(obj) DirectCast(New TextureBrush(obj.Value), Brush))
                        Else
                            COGsColor = RenderingColor.CategoryMapsTextures(categories:=CogCategory, textures:=TextureList.ToArray(Function(obj) obj.Value))
                        End If
                    End If

                    Call COGsColor.Add("", Brushes.White)
                Else
                    COGsColor = RenderingColor.InitCOGColors(categories:=CogCategory) _
                        .ToDictionary(Function(obj) obj.Key,
                                      Function(cl) CType(New SolidBrush(cl.Value), Brush))
                    Call COGsColor.Add("", New SolidBrush(Color.Brown))
                    Call COGsColor.Remove("")
                    Call COGsColor.Add("COG_NOT_ASSIGN", Brushes.Brown)
                    Call Device.Graphics.DrawingCOGColors(COGsColor,
                                                           ref:=New Point(Margin, Device.Height - MaxIDLength * 3),
                                                           legendFont:=New Font(FontFace.Ubuntu, 8),
                                                           width:=Device.Width,
                                                           margin:=Margin)
                End If
            End If

            Return refQuery.COGsColorBrush(CustomCOGMapping:=True, COGsColor:=COGsColor)
        End Function

        ''' <summary>
        ''' 对blast结果进行可视化
        ''' </summary>
        ''' <param name="alignment"></param>
        ''' <param name="refQuery"></param>
        ''' <param name="AlignmentColorSchema">bit_scores, identities</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("invoke.drawing",
                   Info:="You can using the custom_order parameter to specific the order of the genome drawing on the visualized image. idType: 1 -> locusID; 2 -> geneName + id_number")>
        Public Function InvokeDrawing(<Parameter("align.tab", "Blast result that you can download from NCBI blast website, or you also can generates from GCModeller.")> alignment As AlignmentTable,
                                      <Parameter("query.info", "The genome brief information of the query species.")> refQuery As PTT,
                                      <Parameter("custom.orders", "The custom order of the blast hits show on the graphics.")> Optional CustomOrder As String() = Nothing,
                                      <Parameter("using.id.type.alt",
                                                 "idType: 1 -> locusID; 2 -> geneName + id_number; 3 -> only display gene name, default value is 1 (locusID)")>
                                      Optional idType As Integer = 1,
                                      <Parameter("using.arrow.type.alt")> Optional ArrowAlternativeStyle As Boolean = False,
                                      <Parameter("align.color.schema", "The alignment hit bar color.")> Optional AlignmentColorSchema As String = "bit_scores",
                                      <Parameter("fasta.nt.query", "The genome sequence fasta object data of the query.")> Optional QueryNT As FASTA.FastaToken = Nothing,
                                      <Parameter("using.id.anno.alt")> Optional AltIDAnnotation As Boolean = False,
                                      <Parameter("query.color.none")> Optional QueryNoColor As Boolean = False,
                                      <Parameter("identity.color.none")> Optional IdentityNoColor As Boolean = True,
                                      <Parameter("hit.height.equals.to.query")> Optional HitHeightEqualsToQuery As Boolean = False,
                                      <Parameter("using.cog.texture.mappings",
                                                 "If this parameter is TRUE, then the texture will be using for the cog class value instead of ORF cog color.")>
                                      Optional COGTextureMappings As Boolean = False,
                                      <Parameter("mapping.texture.source", "This value is the directory which contains the texture image " &
                                                                           "to mapping the cog, if this value is null then the default texture " &
                                                                           "resource in this programs resource package will be using.")>
                                      Optional TextureSource As String = "",
                                      <Parameter("Mapping.Texture.ID")> Optional ResourceIDMapping As Boolean = True,
                                      <Parameter("scale.factor")> Optional ScaleFactor As Double = 1.0R,
                                      <Parameter("ref.Brush")> Optional queryBrush As ICOGsBrush = Nothing) As Image

            If ScaleFactor <= 0 Then
                Call VBDebugger.Warning($"The page scale factor value ""{ScaleFactor}"" is Zero or negative, reset to normal scale_factor=1")
                ScaleFactor = 1.0R
            End If

            Dim QueryLength As Integer = refQuery.Size
            Dim spList = (From hitData As HitRecord
                          In alignment.Hits
                          Select hitData
                          Group By hitData.SubjectIDs Into Group).ToArray ' 为了保持原有的顺序，在这里不需要并行化拓展
            Dim DrawingFont As Font = New Font(FontFace.Ubuntu, FontSize)
            Dim MaxIDLength As Size = (From Species In spList Select Species.SubjectIDs Order By Len(SubjectIDs) Descending).First.MeasureString(DrawingFont, ScaleFactor, ScaleFactor)
            Dim MappingLength As Integer = QueryLength * ConvertFactor
            Dim BlockSize As New Size(100, MaxIDLength.Height + 20)
            Dim dSize As New Size((Margin * 2 + MappingLength + MaxIDLength.Width) * ScaleFactor,
                                  (If(AltIDAnnotation, ("0".MeasureString(DrawingFont, ScaleFactor, ScaleFactor).Height + 3) * (spList.Length + 5), 0) +
                                  Margin * 4 + spList.Length * (MaxIDLength.Height + 5) + 10 * (BlockSize.Height + 8)) * ScaleFactor)
            Dim Device As GDIPlusDeviceHandle = dSize.CreateGDIDevice
            '  Dim n As Integer = QueryLength / 10000 ' 这个长度是画标尺需要使用到的步移的长度
            ' Dim dl As Integer = MappingLength / n
            Dim X, Y As Integer
            Dim ColorSchema As RangeList(Of Double, TagValue(Of Color))
            Dim GetScore As Func(Of HitRecord, Double)
            Dim GetSubjectHitColorMethod As Func(Of Double, RangeList(Of Double, TagValue(Of Color)), Color) '比对结果的颜色渲染的方法

            If ScaleFactor <> 1.0R Then Call Device.Graphics.ScaleTransform(ScaleFactor, ScaleFactor)

            If String.IsNullOrEmpty(AlignmentColorSchema) OrElse
                Not String.Equals(AlignmentColorSchema, "identities", StringComparison.OrdinalIgnoreCase) Then
                ColorSchema = NCBIBlastResult.ColorSchema.BitScores
                AlignmentColorSchema = "alignment bit scores"
                GetScore = Function(item As HitRecord) item.BitScore
                GetSubjectHitColorMethod = Function(p, Colors) Colors.GetColor(p)
            Else
                If IdentityNoColor Then
                    ColorSchema = NCBIBlastResult.ColorSchema.IdentitiesNonColors
                Else
                    ColorSchema = NCBIBlastResult.ColorSchema.IdentitiesColors
                End If
                GetScore = Function(SubjectHit As HitRecord) SubjectHit.Identity
                If String.Equals(alignment.Program, "blastn", StringComparison.OrdinalIgnoreCase) Then
                    GetSubjectHitColorMethod = Function(p, Colors) Colors.GetColor(p)
                Else
                    GetSubjectHitColorMethod = Function(p, Colors) Colors.GetColor(p)
                End If
            End If

            Y = Margin
            X = Margin
            Y += BlockSize.Height + 10

            Dim IDMethod As GetDrawingID = ModelAPI.GetMethod(idType)

            If queryBrush Is Nothing Then
                queryBrush = __COGsBrush(queryNoColor:=QueryNoColor,
                                         COGTextureMappings:=COGTextureMappings,
                                         Device:=Device,
                                         MaxIDLength:=MaxIDLength.Height,
                                         refQuery:=refQuery,
                                         ResourceIDMapping:=ResourceIDMapping,
                                         TextureSource:=TextureSource)
            End If

            Dim Models As GenomeModel =
                ModelAPI.CreateObject(refQuery.GeneObjects,
                                      refQuery.Size,
                                      alignment.ToString,
                                      IDMethod,
                                      DefaultWhite:=True,
                                      COGsColor:=queryBrush)

            Dim Left As Integer = Margin,
                Height As Integer = Margin + 20
            Models.genes =
                LinqAPI.Exec(Of GeneObject) <= From Ordered_GeneObj As GeneObject
                                               In Models.genes
                                               Select Ordered_GeneObj
                                               Order By Ordered_GeneObj.Left Ascending

            Dim QueryGenomeDrawingLength As Integer = Device.Width - 2 * Margin - MaxIDLength.Width
            Dim sz As New Size(QueryGenomeDrawingLength, GeneObjectDrawingHeight - 0.4 * GeneObjectDrawingHeight)
            Dim rect As New Rectangle(New Point(Margin, Height + 0.2 * GeneObjectDrawingHeight), sz)

            Call Device.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(20, Color.LightGray)), rect)

            Dim cF As Double = (Device.Width - 2 * Margin - MaxIDLength.Width) / Models.Length
            Dim Font As Font = New Font(FontFace.Ubuntu, FontSize)

            Left += Models.First.Left * cF

            ' Dim ORFDrawingY As Integer = Height

            Dim IDConflictedRegion As Rectangle

            '绘制基本图形
            For i As Integer = 0 To Models.Count - 2
                Dim Gene As GeneObject = Models(i)
                Gene.Height = GeneObjectDrawingHeight
                Dim NextGene As GeneObject = Models(i + 1)

                If QueryNoColor Then
                    Gene.Color = Brushes.White
                End If
                Left = Gene.InvokeDrawing(Device.Graphics,
                                          New Point(Left, Height),
                                          NextLeft:=NextGene.Left,
                                          ConvertFactor:=cF,
                                          Region:=Nothing,
                                          IdGrawingPositionDown:=False,
                                          Font:=Font,
                                          AlternativeArrowStyle:=ArrowAlternativeStyle,
                                          IDConflictedRegion:=IDConflictedRegion)
            Next

            Models.Last.Height = GeneObjectDrawingHeight
            If QueryNoColor Then
                Models.Last.Color = Brushes.White
            End If
            Call Models.Last.InvokeDrawing(Device.Graphics,
                                           New Point(Left, Height),
                                           NextLeft:=Models.Length,
                                           ConvertFactor:=cF,
                                           Region:=Nothing,
                                           IdGrawingPositionDown:=False,
                                           Font:=Font,
                                           AlternativeArrowStyle:=ArrowAlternativeStyle,
                                           IDConflictedRegion:=IDConflictedRegion)

            Dim TitleDrawingFont As New Font("Microsoft YaHei", 20)
            Dim TitleFontSize = Device.Graphics.MeasureString(Models.Title, TitleDrawingFont)
            Call Device.Graphics.DrawString(Models.Title,
                                             TitleDrawingFont,
                                             Brushes.Black,
                                             New Point((Device.Width - TitleFontSize.Width) / 2, 10))

            Y += 10
            Call Device.Graphics.DrawLine(Pens.Black, X, Y, X + MappingLength, Y)

            'For i As Integer = 0 To n
            '    Call Device.Gr_Device.DrawLine(Pens.Black, X, Y, X, Y + 5)
            '    Call Device.Gr_Device.DrawString(i * 10 & "KB", DrawingFont, Brushes.Black, New Point(X, Y + 5))
            '    X += dl
            'Next

            Y += MaxIDLength.Height + 5

            Dim BlockHeight As Integer = If(HitHeightEqualsToQuery, GeneObjectDrawingHeight * 1.2, MaxIDLength.Height * 0.8)
            Dim LinePen As New Pen(New SolidBrush(Color.FromArgb(alpha:=100, baseColor:=Color.Brown)))

            If Not CustomOrder.IsNullOrEmpty Then '使用自定义的排序
                Dim ls = spList.ToList
                Dim ls2 = spList.ToList
                Dim InternalGetItem = Function(id As String) (From Gene
                                                              In ls.AsParallel
                                                              Where String.Equals(id, Gene.SubjectIDs, StringComparison.OrdinalIgnoreCase) OrElse
                                                                  InStr(Gene.SubjectIDs, id, CompareMethod.Text) > 0
                                                              Select Gene).FirstOrDefault
                Call ls2.Clear() '复制匿名对象的信息并构建一个用于排序的空的列表

                For Each ID As String In CustomOrder
                    Dim selectedItem = InternalGetItem(ID)
                    If Not selectedItem Is Nothing Then '由于是倒序的，故而将对象移动到最后一个元素即可
                        Call ls2.Add(selectedItem)
                        Call ls.Remove(selectedItem)
                    End If
                Next

                Call ls2.AddRange(ls) '添加剩余的数据
                spList = ls2.ToArray
            End If

            Dim InternalGetColor As Func(Of HitRecord, Color) =
                Function(hit) GetSubjectHitColorMethod(arg1:=GetScore(hit), arg2:=ColorSchema)
            Dim IDannos As New Dictionary(Of Integer, String)
            Dim p_ID As Integer = 1
            Dim proc As New ProgressBar("Drawing alignment hit regions...")
            Dim pp As New ProgressProvider(spList.Length)

            For Each hit In spList
                Call proc.SetProgress(pp.StepProgress)

                X = Margin
                Y += BlockHeight + 4

                Call Device.Graphics.DrawLine(LinePen, X, Y, X + MappingLength, Y)
                If AltIDAnnotation Then '在hit的开始位置的前面使用数字进行标识，然后在最下面写上编号
                    Call Device.Graphics.DrawString(p_ID, DrawingFont, Brushes.Black, x:=10, y:=Y - MaxIDLength.Height / 2)
                    Call IDannos.Add(p_ID, hit.SubjectIDs)
                    Call p_ID.MoveNext()
                Else
                    Call Device.Graphics.DrawString(hit.SubjectIDs,
                                                     DrawingFont,
                                                     Brushes.Black,
                                                     X + MappingLength + 10,
                                                     Y - MaxIDLength.Height / 2)
                End If

                For Each Segment As HitRecord In hit.Group
                    Left = Segment.QueryStart
                    Dim Right As Integer = Segment.QueryEnd

                    If Left > Right Then
                        Call Left.SwapWith(Right)
                    End If

                    Dim Loci As Point = New Point(Margin + Left * ConvertFactor, Y)
                    Dim Block As Size = New Size(ConvertFactor * (Right - Left), BlockHeight)
                    Dim hitColor As New SolidBrush(InternalGetColor(Segment))

                    Call Device.Graphics.FillRectangle(hitColor, New Rectangle(Loci, Block))
                Next
            Next

            X = Margin + 30
            Y += BlockHeight * 5
            Dim YT = Y

            Call Device.Graphics.DrawString("Color key for " & AlignmentColorSchema, DrawingFont, Brushes.Black, New Point(Margin, Y - MaxIDLength.Height * 2))

            For Each Line As TagValue(Of Color) In ColorSchema.Values
                Call Device.Graphics.FillRectangle(New SolidBrush(Line.Value), New Rectangle(New Point(X, Y), BlockSize))
                Call Device.Graphics.DrawString(Line.tag, DrawingFont, Brushes.Black, X + BlockSize.Width + 10, Y + 3)
                Y += BlockSize.Height + 5
            Next

            Y += 3 * BlockSize.Height

            Call Device.Graphics.DrawString("Window Size   =   " & GCSkew.WindowSize, DrawingFont, Brushes.Black, New Point(X, Y))
            Call Device.Graphics.DrawString("Steps                =   " & GCSkew.Steps, DrawingFont, Brushes.Black, New Point(X, Y + 10 + "0".MeasureString(DrawingFont, ScaleFactor, ScaleFactor).Height))

            Dim n As Integer

            If AltIDAnnotation Then

                n = (IDannos.First.Value.MeasureString(DrawingFont, ScaleFactor, ScaleFactor).Height + 2)
                X = Device.Width - (From ID In IDannos Select ID.Value Order By Len(Value) Descending).First.MeasureString(DrawingFont, ScaleFactor, ScaleFactor).Width * 3 - Margin
                Y = YT

                '在下面标出物种编号
                For Each ID In IDannos
                    Call Device.Graphics.DrawString(String.Format("{0}.  {1}", ID.Key, ID.Value), DrawingFont, Brushes.Black, New Point(X, Y))
                    Y += n
                Next
            End If

            If Not QueryNT Is Nothing Then
                Dim DeltaHeight As Integer = 1000
                Dim Gr As GDIPlusDeviceHandle = New Size(Device.Width, Device.Height + DeltaHeight).CreateGDIDevice
                Dim hhh As Single = DeltaHeight + TitleFontSize.Height + 30

                Call Gr.Graphics.DrawImage(Device.ImageResource, 0, DeltaHeight, Device.Width, Device.Height)
                Call Gr.Graphics.FillRectangle(Brushes.White, New Rectangle(New Point(), New Size(Device.Width, hhh)))       '覆盖掉标题
                Call GCSkew.InvokeDrawingGCContent(Gr.ImageResource, QueryNT, New Point(Margin, 0.95 * hhh), Width:=QueryGenomeDrawingLength)

                Return Gr.ImageResource
            End If

            Return Device.ImageResource
        End Function

        Private Function GetColor(score As Integer) As Color
            If score < 40 Then
                Return Color.Black
            ElseIf score >= 40 AndAlso score < 50 Then
                Return Color.Blue
            ElseIf score >= 50 AndAlso score < 80 Then
                Return Color.Green
            ElseIf score >= 80 AndAlso score < 200 Then
                Return Color.Purple
            Else
                Return Color.Red
            End If
        End Function
    End Module
End Namespace
