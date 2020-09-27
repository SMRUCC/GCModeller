#Region "Microsoft.VisualBasic::0d5fcab66bf36112ce28487403de8e5a, visualize\DataVisualizationTools\NCBIBlastResult\BlastVisualize.vb"

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

    '     Module BlastVisualize
    ' 
    '         Function: __COGsBrush, __createHits, AlignmentTableFromBlastn, AlignmentTableFromBlastX, ApplyDescription
    '                   ApplyDescription2, CreateTableFromBlastOutput, ExportTableOrderByGI, GetColor, GetSubjectHitLocusID
    '                   InternalShortID_s, (+2 Overloads) InvokeDrawing, LoadResult, PlotMap, ShortID
    ' 
    '         Sub: AssignCogClass
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.SyntenyVisualize.ComparativeGenomics
Imports VBLinq = Microsoft.VisualBasic.Linq

Namespace NCBIBlastResult

    ''' <summary>
    ''' 对blastn和blastx的比对结果进行可视化操作
    ''' </summary>
    <Package("NCBI.Visualization.BLAST",
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
        <Extension> Public Function InvokeDrawing(query As Query) As Image
            Dim g = New Size(Margin * 3 + query.QueryLength, 2 * Margin + 100).CreateGDIDevice
            Dim Y As Integer = Margin / 2
            Dim rect As New Rectangle(New Point(Margin, Y), New Size(query.QueryLength, 10))
            Y += rect.Height
            Dim font As New Font(FontFace.Ubuntu, 6, FontStyle.Regular)

            Call g.Graphics.FillRectangle(Brushes.Black, rect)

            Dim sz = g.Graphics.MeasureString("0", font)

            For i As Integer = 0 To query.QueryLength Step 10
                If i Mod 50 = 0 Then
                    Dim YY As Integer = Y + 5
                    Call g.Graphics.DrawLine(New Pen(Color.Black, 2), New Point(i + Margin, YY), New Point(i + Margin, Y))   ' 大标尺
                    sz = g.Graphics.MeasureString(i, font)
                    Call g.Graphics.DrawString(i, font, Brushes.Black, New Point(sz.Width / 2 + i + Margin, YY))
                Else
                    Call g.Graphics.DrawLine(New Pen(Color.Gray, 1), New Point(i + Margin, Y + 3), New Point(i + Margin, Y)) ' 小标尺
                End If
            Next

            Call g.Graphics.DrawString($"({query.QueryLength})", font, Brushes.Black, New Point(query.QueryLength + Margin + 5, Y + 5))

            Y += (10 + sz.Height)
            font = New Font(FontFace.Ubuntu, 8, FontStyle.Regular)

            For Each hit In query.SubjectHits
                Dim loci = hit.QueryLocation
#If DEBUG Then
                Call loci.__DEBUG_ECHO
#End If
                rect = New Rectangle(New Point(Margin + loci.left, Y), New Size(loci.FragmentSize, 10))
                Call g.Graphics.FillRectangle(Brushes.Blue, rect)
                Dim x As Integer = g.Graphics.MeasureString(hit.Name, font).Width
                x = rect.Left + (rect.Width - x) / 2
                Call g.Graphics.DrawString(hit.Name, font, brush:=Brushes.Gray, point:=New PointF(x, rect.Bottom + 2))
                Y += 32
            Next

            Return g.ImageResource
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
            Return AlignmentTableParser.CreateFromBlastX(source)
        End Function

        <ExportAPI("alignment_dump.from.blastn")>
        Public Function AlignmentTableFromBlastn(<Parameter("dir.source", "The directory which contains the blastn output data.")> source As String) As AlignmentTable
            Return AlignmentTableParser.CreateFromBlastn(source)
        End Function

        Private Function InternalShortID_s(srcFasta As FASTA.FastaSeq) As String
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
        Public Function ShortID(<Parameter("data.fasta")> data As FastaFile,
                                <Parameter("dir.export",
                                           "if this dir path parameter is not empty then the function will export the entry information into the directory that this parameter specificed.")>
                                Optional EXPORT$ = "") As FastaFile

            Dim LQuery = (From fa As FastaSeq
                          In data
                          Let ShortID_s As String = InternalShortID_s(fa)
                          Select ShortID_s,
                              fa.Title,
                              Fasta = fa).ToArray

            Const attrs$ = NameOf(FastaSeq.Headers)

            With LQuery
                VBLinq _
                    .DATA(.Select(Function(x) x.Fasta)) _
                    .Evaluate(attrs) = .Select(Function(x) {x.ShortID_s}) _
                                       .ToArray
            End With

            Dim fastaFile As New FastaFile(From ShortIDFasta In LQuery Select ShortIDFasta.Fasta)

            If String.IsNullOrEmpty(EXPORT) Then
                Return fastaFile
            End If

            Dim TableGrouped = (From title In LQuery Select title.ShortID_s, title.Title Group By ShortID_s Into Group).ToArray
            Dim TableDistinct = (From fa In TableGrouped Select fa.ShortID_s, Title = fa.Group.First).ToArray

            Call TableDistinct.SaveTo(EXPORT, strict:=False)

            EXPORT = EXPORT.ParentPath & "/" & BaseName(EXPORT) & "/"

            For Each fa In LQuery
                Call fa.Fasta.SaveTo(EXPORT & fa.ShortID_s & ".fasta")
            Next

            Return fastaFile
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
                                         Optional maxLength% = 0) As AlignmentTable
            Call Table.DescriptionSubstituted(info.ToArray)
            Call Table.TrimLength(maxLength)
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
            Return AlignmentTableParser.LoadTable(path)
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
                                                   <Parameter("list.cds.info")> CdsInfo As IEnumerable(Of GeneTable)) As AlignmentTable
            Dim ResourceEntries = (From Entry
                                   In source.LoadSourceEntryList({"*.txt"})
                                   Where InStr(Entry.Key, QueryID, CompareMethod.Text) = 1
                                   Select ID = Entry.Key,
                                       Path = Entry.Value).ToArray
            Dim LoadBlastOutput = (From Entry
                                   In ResourceEntries.AsParallel
                                   Where FileIO.FileSystem.GetFileInfo(Entry.Path).Length > 0
                                   Let Output As v228 = TryParse(Entry.Path)
                                   Select Entry.ID,
                                       Output).ToArray
            Dim ORF As Dictionary(Of String, GeneTable) =
                CdsInfo.ToDictionary(Function(g)
                                         Return g.locus_id
                                     End Function)
            Dim ChunkBuffer As HitRecord() = LinqAPI.Exec(Of HitRecord) <=
 _
                From EntryInfo
                In LoadBlastOutput
                Let hits As HitRecord() = __createHits(ORF, EntryInfo.Output)
                Where Not hits.IsNullOrEmpty
                Select hits

            Dim Table As New AlignmentTable With {
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
        Private Function __createHits(ORF As Dictionary(Of String, GeneTable), Blastoutput As v228) As HitRecord()
            Dim Trim = (From query As Query
                        In Blastoutput.Queries
                        Where Not query.SubjectHits.IsNullOrEmpty
                        Select Query = ORF(query.QueryName.Split(CChar("|")).First),
                            query.SubjectHits
                        Order By Query.left Ascending).ToArray
            Dim hits As String() =
                LinqAPI.Exec(Of String) <= From x
                                           In Trim
                                           Select From hit As SubjectHit
                                                  In x.SubjectHits
                                                  Select hit.Name
            Dim SortHits As GeneTable() =
                LinqAPI.Exec(Of GeneTable) <= From id As String
                                                 In hits
                                              Where ORF.ContainsKey(id)
                                              Select _orf = ORF(id)
                                              Order By _orf.left Ascending
            Dim OrderedHits As String() = SortHits.Select(Function(hit) hit.locus_id)

            If OrderedHits.IsNullOrEmpty Then Return New HitRecord() {}

            Dim trimedQuery As New List(Of KeyValuePair(Of GeneTable, Double)())

            Dim p_Direction As Integer '方向
            Dim i As Integer

            Dim TempChunk As New List(Of KeyValuePair(Of GeneTable, Double))

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
                        TempChunk += New KeyValuePair(Of GeneTable, Double)(Query.Query, hit.Score.RawScore)
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
                                TempChunk += New KeyValuePair(Of GeneTable, Double)(Query.Query, h.Score.RawScore)
                                Match = True
                                Exit For
                            End If
                        Next

CONTINUTE:
                        If Not Match Then '断裂了
                            Call trimedQuery.Add(TempChunk.ToArray)
                            Call TempChunk.Clear()
                            Exit Do
                        End If

                        j += 1
                    Loop

                    i = j
                    Continue Do
                Next

                If __single Then
                    Dim score# = Query.SubjectHits.First.Score.RawScore

                    trimedQuery += {
                        New KeyValuePair(Of GeneTable, Double)(Query.Query, score)
                    }
                End If

                i += 1
            Loop

            Return LinqAPI.Exec(Of HitRecord) <=
                From trimedData As KeyValuePair(Of GeneTable, Double)()
                In trimedQuery
                Let loci As IEnumerable(Of Integer) =
                    (LinqAPI.MakeList(Of Integer) <= From GeneObject
                                                     In trimedData
                                                     Select {GeneObject.Key.left, GeneObject.Key.right})  '
                Select New HitRecord With {
                    .QueryID = "",
                    .SubjectIDs = Blastoutput.Database,
                    .QueryStart = loci.Min,
                    .QueryEnd = loci.Max,
                    .BitScore = trimedData.Average(Function(x) x.Value)
                }
        End Function

        <DataFrameColumn("Gene.Drawing.Object.Height")> Dim GeneObjectDrawingHeight As Integer = 25

        <ExportAPI("Cog.Class.Assign")>
        Public Sub AssignCogClass(PTT As PTTDbLoader,
                                  <Parameter("Class.Cog.Mappings",
                                             "The excel table object which contains the gene cog value.")> Mapping As IO.File,
                                  <Parameter("Mapping.Gene.ID",
                                             "The column name of the gene id in the execel table.")> Optional GeneID As String = "GeneID",
                                  <Parameter("Mapping.COG",
                                             "The column name of the cog class value in the excel table.")> Optional COG As String = "COG")

            Dim DF As IO.DataFrame = IO.DataFrame.CreateObject(Mapping)
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

        ''' <summary>
        ''' 这个函数使用的是<see cref="GeneBrief.COG"/>属性来获取分类值
        ''' </summary>
        ''' <param name="queryNoColor"></param>
        ''' <param name="refQuery"></param>
        ''' <param name="COGTextureMappings"></param>
        ''' <param name="TextureSource$"></param>
        ''' <param name="ResourceIDMapping"></param>
        ''' <param name="g"></param>
        ''' <param name="MaxIDLength%"></param>
        ''' <returns></returns>
        Private Function __COGsBrush(queryNoColor As Boolean,
                                     refQuery As PTT,
                                     COGTextureMappings As Boolean,
                                     TextureSource$,
                                     ResourceIDMapping As Boolean,
                                     g As Graphics2D,
                                     MaxIDLength%) As ICOGsBrush

            Dim COGsColor As Dictionary(Of String, Brush) = Nothing

            If Not queryNoColor Then
                Dim COGs$() = LinqAPI.Exec(Of String) <=
 _
                    From gene As GeneBrief
                    In refQuery.GeneObjects
                    Where Not String.IsNullOrEmpty(gene.COG)
                    Select gene.COG
                    Distinct

                If COGTextureMappings Then    ' 使用材质映射，假若没有设置资源文件夹，则使用系统默认的材质进行绘图
                    If String.IsNullOrEmpty(TextureSource) Then
                        Dim TextureList As Image() = TextureResourceLoader.LoadInternalDefaultResource.Shuffles
                        COGsColor = RenderingColor.CategoryMapsTextures(categories:=COGs, textures:=TextureList)
                    Else
                        Dim TextureList = LinqAPI.Exec(Of NamedValue(Of Image)) <=
 _
                            From path As String
                            In ls - l - r - {"*.bmp", "*.jpg", "*.png"} <= TextureSource
                            Select New NamedValue(Of Image) With {
                                .Name = path.BaseName,
                                .Value = path.LoadImage
                            }

                        If ResourceIDMapping Then
                            COGsColor = TextureList.ToDictionary(
                                Function(obj) obj.Name.ToUpper,
                                Function(obj) DirectCast(New TextureBrush(obj.Value), Brush))
                        Else
                            COGsColor = RenderingColor.CategoryMapsTextures(
                                categories:=COGs,
                                textures:=TextureList.Select(Function(obj) obj.Value))
                        End If
                    End If

                    Call COGsColor.Add("", Brushes.White)
                Else
                    COGsColor = RenderingColor.InitCOGColors(categories:=COGs) _
                        .ToDictionary(Function(obj) obj.Key,
                                      Function(cl) CType(New SolidBrush(cl.Value), Brush))
                    Call COGsColor.Add("", New SolidBrush(Color.Brown))
                    Call g.DrawingCOGColors(
                        COGsColor,
                        ref:=New Point(Margin, g.Height - MaxIDLength * 3),
                        legendFont:=New Font(FontFace.Ubuntu, 8),
                        width:=g.Width,
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
        ''' <param name="queryBrush">query基因组上面的基因的颜色画刷的来源，默认是使用内部的COG颜色</param>
        ''' <param name="QueryNoColor">如果这个参数为真，那么query的基因箭头矩形对象将不会有任何颜色</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Map.drawing",
                   Info:="You can using the custom_order parameter to specific the order of the genome drawing on the visualized image. idType: 1 -> locusID; 2 -> geneName + id_number")>
        Public Function PlotMap(<Parameter("align.tab", "Blast result that you can download from NCBI blast website, or you also can generates from GCModeller.")> alignment As AlignmentTable,
                                <Parameter("query.info", "The genome brief information of the query species.")> refQuery As PTT,
                                <Parameter("custom.orders", "The custom order of the blast hits show on the graphics.")> Optional CustomOrder$() = Nothing,
                                <Parameter("using.id.type.alt", "idType: 1 -> locusID; 2 -> geneName + id_number; 3 -> only display gene name, default value is 1 (locusID)")> Optional idType% = 1,
                                <Parameter("using.arrow.type.alt")> Optional ArrowAlternativeStyle As Boolean = False,
                                <Parameter("align.color.schema", "The alignment hit bar color.")> Optional AlignmentColorSchema$ = "bit_scores",
                                <Parameter("fasta.nt.query", "The genome sequence fasta object data of the query.")> Optional QueryNT As FastaSeq = Nothing,
                                <Parameter("using.id.anno.alt")> Optional AltIDAnnotation As Boolean = False,
                                <Parameter("query.color.none")> Optional QueryNoColor As Boolean = False,
                                <Parameter("identity.color.none")> Optional IdentityNoColor As Boolean = True,
                                <Parameter("hit.height.equals.to.query")> Optional HitHeightEqualsToQuery As Boolean = False,
                                <Parameter("using.cog.texture.mappings", "If this parameter is TRUE, then the texture will be using for the cog class value instead of ORF cog color.")> Optional COGTextureMappings As Boolean = False,
                                <Parameter("mapping.texture.source", "This value is the directory which contains the texture image " &
                                                                     "to mapping the cog, if this value is null then the default texture " &
                                                                     "resource in this programs resource package will be using.")> Optional TextureSource$ = "",
                                <Parameter("Mapping.Texture.ID")> Optional ResourceIDMapping As Boolean = True,
                                <Parameter("scale.factor")> Optional ScaleFactor As Double = 1.0R,
                                <Parameter("ref.Brush")> Optional queryBrush As ICOGsBrush = Nothing, Optional margin% = 200) As Image

            If ScaleFactor <= 0 Then
                Call VBDebugger.Warning($"The page scale factor value ""{ScaleFactor}"" is Zero or negative, reset to normal scale_factor=1")
                ScaleFactor = 1.0R
            End If

            Dim queryLength As Integer = refQuery.Size
            Dim spList = (From hitData As HitRecord
                          In alignment.Hits
                          Select hitData
                          Group By hitData.SubjectIDs Into Group).ToArray ' 为了保持原有的顺序，在这里不需要并行化拓展
            Dim drawingFont As New Font(FontFace.Ubuntu, FontSize)
            Dim MaxIDLength As SizeF = spList _
                .MaxLengthString(Function(sp) sp.SubjectIDs) _
                .MeasureSize(New Size(1, 1).CreateGDIDevice, drawingFont, (ScaleFactor, ScaleFactor))
            Dim MappingLength As Integer = queryLength * ConvertFactor
            Dim BlockSize As New Size(100, MaxIDLength.Height + 20)
            Dim dSize As New Size With {
                .Width = (margin * 2 + MappingLength + MaxIDLength.Width) * ScaleFactor,
                .Height = (If(AltIDAnnotation,
                    ("0".MeasureSize(New Size(1, 1).CreateGDIDevice, drawingFont, (ScaleFactor, ScaleFactor)).Height + 3) * (spList.Length + 5), 0) + margin + spList.Length * (MaxIDLength.Height + 5) + 10 * (BlockSize.Height + 8)) * ScaleFactor
            }
            Dim X, Y As Integer
            Dim ColorSchema As RangeList(Of Double, NamedValue(Of Color))
            Dim getScore As Func(Of HitRecord, Double)
            Dim getSubjectHitColor As Func(Of Double, RangeList(Of Double, NamedValue(Of Color)), Color) '比对结果的颜色渲染的方法

            If Not AlignmentColorSchema.TextEquals("identities") Then
                ColorSchema = NCBIBlastResult.ColorSchema.BitScores
                AlignmentColorSchema = "alignment bit scores"
                getScore = Function(item As HitRecord) item.BitScore
                getSubjectHitColor = Function(p, Colors) Colors.GetColor(p)
            Else
                If IdentityNoColor Then
                    ColorSchema = NCBIBlastResult.ColorSchema.IdentitiesMonoChrome
                Else
                    ColorSchema = NCBIBlastResult.ColorSchema.IdentitiesChromatic
                End If

                getScore = Function(subjectHit As HitRecord) subjectHit.Identity

                If String.Equals(alignment.Program, "blastn", StringComparison.OrdinalIgnoreCase) Then
                    getSubjectHitColor = Function(p, Colors) Colors.GetColor(p)
                Else
                    getSubjectHitColor = Function(p, Colors) Colors.GetColor(p)
                End If
            End If

            Y = margin
            X = margin
            Y += BlockSize.Height + 10

            Dim IDMethod As GetDrawingID = ModelAPI.GetMethod(idType)

            Using device As Graphics2D = dSize.CreateGDIDevice

                If ScaleFactor <> 1.0R Then
                    Call device.ScaleTransform(ScaleFactor, ScaleFactor)
                End If

                If queryBrush Is Nothing Then

                    ' 绘制COG分类的颜色legend
                    queryBrush = __COGsBrush(
                        queryNoColor:=QueryNoColor,
                        COGTextureMappings:=COGTextureMappings,
                        g:=device,
                        MaxIDLength:=MaxIDLength.Height,
                        refQuery:=refQuery,
                        ResourceIDMapping:=ResourceIDMapping,
                        TextureSource:=TextureSource)
                End If

                Dim models As GenomeModel = ModelAPI.CreateSyntenyGenome(
                    refQuery.GeneObjects,
                    refQuery.Size,
                    alignment.ToString,
                    IDMethod,
                    defaultWhite:=False,
                    COGsColor:=queryBrush)

                Dim Left As Integer = margin, Height As Integer = margin + 20

                With models
                    .genes = LinqAPI.Exec(Of GeneObject) <=
 _
                        From ordered_geneObj As GeneObject
                        In .genes
                        Select ordered_geneObj
                        Order By ordered_geneObj.Left Ascending

                End With

                Dim QueryGenomeDrawingLength As Integer = device.Width - 2 * margin - MaxIDLength.Width
                Dim sz As New Size(QueryGenomeDrawingLength, GeneObjectDrawingHeight - 0.4 * GeneObjectDrawingHeight)
                Dim rect As New Rectangle(New Point(margin, Height + 0.2 * GeneObjectDrawingHeight), sz)

                Call device.FillRectangle(New SolidBrush(Color.FromArgb(20, Color.LightGray)), rect)

                Dim cfactor As Double = (device.Width - 2 * margin - MaxIDLength.Width) / models.Length
                Dim Font As Font = New Font(FontFace.Ubuntu, FontSize)

                Left += models.First.Left * cfactor

                Dim IDConflictedRegion As MapLabelLayout

                ' 绘制query对象的基因组的基因map的基本图形
                For i As Integer = 0 To models.genes.Length - 2
                    Dim gene As GeneObject = models(i)
                    Dim next_gene As GeneObject = models(i + 1)

                    With gene

                        .Height = GeneObjectDrawingHeight

                        If QueryNoColor Then
                            .Color = Brushes.White
                        End If

                        Left = .InvokeDrawing(
                            device.Graphics,
                            New Point(Left, Height), NextLeft:=next_gene.Left, scaleFactor:=cfactor,
                            arrowRect:=Nothing,
                            IdDrawPositionDown:=False,
                            Font:=Font,
                            AlternativeArrowStyle:=ArrowAlternativeStyle,
                            overlapLayout:=IDConflictedRegion, drawConflictLine:=True)

                    End With
                Next

                models.Last.Height = GeneObjectDrawingHeight
                If QueryNoColor Then
                    models.Last.Color = Brushes.White
                End If
                Call models.Last.InvokeDrawing(
                    device.Graphics,
                    New Point(Left, Height), NextLeft:=models.Length, scaleFactor:=cfactor,
                    arrowRect:=Nothing,
                    IdDrawPositionDown:=False,
                    Font:=Font,
                    AlternativeArrowStyle:=ArrowAlternativeStyle,
                    overlapLayout:=IDConflictedRegion, drawConflictLine:=True)

                Dim titleDrawingFont As New Font("Microsoft YaHei", 20)
                Dim titleFontSize = device.MeasureString(models.Title, titleDrawingFont)

                Call device.DrawString(
                    models.Title, titleDrawingFont,
                    Brushes.Black,
                    New Point((device.Width - titleFontSize.Width) / 2, 10))

                Y += 10
                Call device.DrawLine(Pens.Black, X, Y, X + MappingLength, Y)

                Y += MaxIDLength.Height + 5

                Dim BlockHeight As Integer = If(
                    HitHeightEqualsToQuery,
                    GeneObjectDrawingHeight * 1.2,
                    MaxIDLength.Height * 0.8)
                Dim LinePen As New Pen(New SolidBrush(Color.FromArgb(alpha:=100, baseColor:=Color.Brown)))

                If Not CustomOrder.IsNullOrEmpty Then '使用自定义的排序
                    spList = spList _
                        .ReorderByKeys(Function(g) g.SubjectIDs, CustomOrder) _
                        .ToArray
                End If

                Dim internalGetColor = Function(hit As HitRecord) getSubjectHitColor(arg1:=getScore(hit), arg2:=ColorSchema)
                Dim IDannos As New Dictionary(Of Integer, String)

                Using proc As New ProgressBar("Drawing alignment hit regions...", 1, CLS:=True)
                    Dim pp As New ProgressProvider(proc, spList.Length)
                    Dim p_ID As Integer = 1

                    For Each hit In spList
                        Call proc.SetProgress(pp.StepProgress, details:=hit.SubjectIDs)

                        X = margin
                        Y += BlockHeight + 4

                        Call device.DrawLine(LinePen, X, Y, X + MappingLength, Y)
                        If AltIDAnnotation Then '在hit的开始位置的前面使用数字进行标识，然后在最下面写上编号
                            Call device.DrawString(p_ID, drawingFont, Brushes.Black, x:=10, y:=Y - MaxIDLength.Height / 2)
                            Call IDannos.Add(p_ID, hit.SubjectIDs)

                            p_ID += 1
                        Else
                            Call device.DrawString(
                                hit.SubjectIDs, drawingFont,
                                Brushes.Black,
                                X + MappingLength + 10,
                                Y - MaxIDLength.Height / 2)
                        End If

                        For Each Segment As HitRecord In hit.Group
                            Left = Segment.QueryStart
                            Dim Right As Integer = Segment.QueryEnd

                            If Left > Right Then
                                Call Left.Swap(Right)
                            End If

                            Dim Loci As Point = New Point(margin + Left * ConvertFactor, Y)
                            Dim Block As Size = New Size(ConvertFactor * (Right - Left), BlockHeight)
                            Dim hitColor As New SolidBrush(internalGetColor(Segment))

                            Call device.FillRectangle(hitColor, New Rectangle(Loci, Block))
                        Next
                    Next
                End Using

                X = margin + 30
                Y += BlockHeight * 10
                Dim YT = Y

                Call device.DrawString("Color key for " & AlignmentColorSchema, drawingFont, Brushes.Black, New Point(margin, Y - MaxIDLength.Height * 2))

                For Each Line As NamedValue(Of Color) In ColorSchema.Values
                    Call device.FillRectangle(New SolidBrush(Line.Value), New Rectangle(New Point(X, Y), BlockSize))
                    Call device.DrawString(Line.Name, drawingFont, Brushes.Black, X + BlockSize.Width + 10, Y + 3)
                    Y += BlockSize.Height + 5
                Next

                Y += 3 * BlockSize.Height

                If Not QueryNT Is Nothing Then
                    Call device.DrawString("Window Size   =   " & GCSkew.WindowSize, drawingFont, Brushes.Black, New Point(X, Y))
                    Call device.DrawString("Steps         =   " & GCSkew.Steps, drawingFont, Brushes.Black, New Point(X, Y + 10 + "0".MeasureSize(device, drawingFont, (ScaleFactor, ScaleFactor)).Height))
                End If

                Dim n As Integer

                If AltIDAnnotation Then

                    n = (IDannos.First.Value.MeasureSize(device, drawingFont, (ScaleFactor, ScaleFactor)).Height + 2)
                    X = device.Width - IDannos.MaxLengthString(Function(k) k.Value).MeasureSize(device, drawingFont, (ScaleFactor, ScaleFactor)).Width * 3 - margin
                    Y = YT

                    '在下面标出物种编号
                    For Each ID In IDannos
                        Call device.DrawString(String.Format("{0}.  {1}", ID.Key, ID.Value), drawingFont, Brushes.Black, New Point(X, Y))
                        Y += n
                    Next
                End If

                If Not QueryNT Is Nothing Then
                    Dim DeltaHeight% = 500

                    ' 如果目标基因组序列存在的话，还会在顶端绘制GCSkew的histgram图表
                    Using g As Graphics2D = New Size(device.Width, device.Height + DeltaHeight).CreateGDIDevice
                        Dim hhh As Single = DeltaHeight + titleFontSize.Height + 30

                        Call g.DrawImage(device.ImageResource, 0, DeltaHeight, device.Width, device.Height)
                        Call g.FillRectangle(Brushes.White, New Rectangle(New Point(), New Size(device.Width, hhh)))  ' 覆盖掉标题

                        ' 由于在绘图函数之中克隆了原来的图像，所以这里返回函数的结果，否则直接返回gdi设备之中的图形任然会缺失掉histogram图形的
                        Return GCSkew.InvokeDrawingGCContent(
                            g.ImageResource,
                            QueryNT,
                            New Point(margin, 0.95 * hhh),
                            Width:=QueryGenomeDrawingLength)
                    End Using
                End If

                Return device.ImageResource
            End Using
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
