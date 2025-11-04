#Region "Microsoft.VisualBasic::6c60147894c43fd8d9695866753a753d, visualize\SyntenyVisual\ComparativeGenomics\MultipleAlignment\ComparativeAlignment.vb"

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

'     Module ComparativeAlignment
' 
'         Function: __internalFilter, __invokeDrawing, BuildModel, BuildMultipleAlignmentModel, CreateColor
'                   InvokeDrawing, TCSVisualization
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.Extensions
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ListExtensions
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.ParallelTask
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.SyntenyVisualize.ComparativeGenomics

#If NET48 Then
Imports Brush = System.Drawing.Brush
Imports SolidBrush = System.Drawing.SolidBrush
Imports Brushes = System.Drawing.Brushes
Imports FontStyle = System.Drawing.FontStyle
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
#Else
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
#End If

Namespace ComparativeAlignment

    <[Namespace]("Comparative.alignment")>
    Module ComparativeAlignment

        <DataFrameColumn("margins")> Dim Margin As Integer = 150
        <DataFrameColumn("gene.drawing.height")> Dim GeneHeight As Integer = 150

        <ExportAPI("color.argb")>
        Public Function CreateColor(R As Integer, G As Integer, B As Integer, Optional alpha As Integer = 120) As Color
            Return Color.FromArgb(alpha, R, G, B)
        End Function

        Private Function __invokeDrawing(Models As GenomeModel,
                                           Device As IGraphics,
                                           Length As Integer,
                                           MaxLengthTitleSize As SizeF,
                                           Height As Integer,
                                           TitleDrawingFont As Font,
                                           Font As Font,
                                           Type2Arrow As Boolean, ColorOverrides As String) As Dictionary(Of String, Rectangle) '绘制基本的ORF信息

            Dim GeneObjectDrawingRegions = New Dictionary(Of String, Rectangle)
            Dim ConvertFactor As Double = Length / Models.Length
            Dim RegionLeft = 2 * Margin + MaxLengthTitleSize.Width

            Models.genes = (From obj As GeneObject In Models.genes Select obj Order By obj.Left Ascending).ToArray

            Call Device.FillRectangle(New SolidBrush(Color.FromArgb(200, Color.Black)),
                                            New Rectangle(New Point(RegionLeft, Height + 0.5 * GeneHeight), New Size(Length, 10)))

            RegionLeft += Models.First.Left * ConvertFactor

            Dim rtvlRegion As Rectangle
            Dim oC As Brush = Nothing
            Dim cO As Boolean = False

            If Not String.IsNullOrEmpty(ColorOverrides) Then
                oC = New SolidBrush(ColorOverrides.ToColor)
                cO = True
            End If

            Dim IDConflictedRegion As MapLabelLayout

            For i As Integer = 0 To Models.Count - 2   '绘制基本图形
                Dim GeneObjectModel As ComparativeGenomics.GeneObject = Models(i)
                Dim NextGeneObject = Models(i + 1)

                GeneObjectModel.Height = GeneHeight
                If cO Then
                    GeneObjectModel.Color = oC
                End If

                RegionLeft = GeneObjectModel.InvokeDrawing(Device, New Point(RegionLeft, Height),
                                                       NextLeft:=NextGeneObject.Left,
                                                       scaleFactor:=ConvertFactor,
                                                       arrowRect:=rtvlRegion,
                                                       IdDrawPositionDown:=True,
                                                       Font:=Font,
                                                       AlternativeArrowStyle:=Type2Arrow, overlapLayout:=IDConflictedRegion)

                Call GeneObjectDrawingRegions.Add(GeneObjectModel.locus_tag, rtvlRegion)
            Next

            Dim LastModel = Models.Last
            LastModel.Height = GeneHeight
            If cO Then
                LastModel.Color = oC
            End If

            Call LastModel.InvokeDrawing(Device, New Point(RegionLeft, Height), NextLeft:=Models.Length,
                                     scaleFactor:=ConvertFactor,
                                     arrowRect:=rtvlRegion, IdDrawPositionDown:=True,
                                     Font:=Font, AlternativeArrowStyle:=Type2Arrow,
                                     overlapLayout:=IDConflictedRegion)
            Call GeneObjectDrawingRegions.Add(Models.Last.locus_tag, rtvlRegion)
            Call Device.DrawString(Models.Title, TitleDrawingFont, Brushes.Black, New Point(Margin, Height))

            Return GeneObjectDrawingRegions
        End Function

        <DataFrameColumn("convert.factor")> Dim InternalConvertFactor As Double = 0.0015
        <DataFrameColumn("font.size")> Dim FontSize As Integer = 20

        Private Class PlotRegions : Inherits Plot

            ReadOnly Model As DrawingModel
            Dim DefaultColor As Color = Nothing,
                Type2Arrow As Boolean = True,
                QueryMiddle As Boolean = False,
                color_overrides As String = ""

            ReadOnly MaxLengthTitleSize As SizeF

            Public Sub New(Model As DrawingModel, theme As Theme, MaxLengthTitleSize As SizeF,
                           Optional DefaultColor As Color = Nothing,
                           Optional Type2Arrow As Boolean = True,
                           Optional QueryMiddle As Boolean = False,
                           Optional color_overrides As String = "")

                MyBase.New(theme)

                Me.MaxLengthTitleSize = MaxLengthTitleSize
                Me.Model = Model
                Me.DefaultColor = DefaultColor
                Me.Type2Arrow = Type2Arrow
                Me.QueryMiddle = QueryMiddle
                Me.color_overrides = color_overrides
            End Sub

            Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
                Dim TitleDrawingFont As New Font("Microsoft YaHei", 20)
                Dim Font As New Font(FontFace.MicrosoftYaHei, FontSize)
                Dim Height As Integer = Margin
                Dim Length As Integer = g.Width - 3 * Margin - MaxLengthTitleSize.Width + 20  '基因组的绘制区域的长度已经被固定下来了

                '    Call GCSkew.InvokeDrawing(Device.ImageResource, Model.NT, New Point(MaxLengthTitleSize.Width + 2 * Margin, Height + 3 * Margin), New Size(Length, 3 * Margin)) '绘制GC偏移曲线

                If QueryMiddle Then
                    Height = (g.Height - Margin) / 2
                Else
                    Height += 3 * Margin + GeneHeight
                End If

                Dim RegionData1 = __invokeDrawing(Model.Query, g, Length, MaxLengthTitleSize, Height, TitleDrawingFont, Font, Type2Arrow, color_overrides)
                Dim RegionData2 As Dictionary(Of String, Rectangle)

                Dim Links As List(Of GeneLink) = (From itemfff In (From ItemLinks In Model.links Select ItemLinks Group By ItemLinks.genome1 Into Group).ToArray Let id = (From nnn In itemfff.Group Select nnn.genome2).ToArray Let idComb = Comb(Of String).CreateCompleteObjectPairs(id) Select (From nnnnn In idComb Select (From jjjjjj In nnnnn Select New ComparativeGenomics.GeneLink With {.genome1 = jjjjjj.Item1, .genome2 = jjjjjj.Item2}).ToArray).ToArray).ToArray.Unlist.Unlist
                Call Links.AddRange(Model.links)

                If DefaultColor = Nothing Then
                    DefaultColor = Color.FromArgb(180, Color.SandyBrown)
                End If

                Dim Up As Boolean = False
                Dim dHeight As Integer = 2.5 * GeneHeight  '相邻的两个基因组之间的绘图的间隔距离
                Dim Height1 As Integer = Height - dHeight
                Dim Height2 As Integer = Height + dHeight
                Dim LastRegion1 As Dictionary(Of String, Rectangle) = RegionData1
                Dim LastRegion2 As Dictionary(Of String, Rectangle) = RegionData1

                For Each hit In Model.aligns

                    If Up Then
                        Height = Height1
                        Height1 -= dHeight
                    Else
                        Height = Height2
                        Height2 += dHeight
                    End If

                    If QueryMiddle Then Up = Not Up '位置进行翻转

                    RegionData2 = __invokeDrawing(hit, g, Length, MaxLengthTitleSize, Height, TitleDrawingFont, Font, Type2Arrow, color_overrides)

                    Dim GNModel___1 = Model.Query.ToDictionary(Function(Gene) Gene.locus_tag), GNModel___2 = Model.aligns.First.ToDictionary(Function(Gene) Gene.locus_tag)

                    On Error Resume Next

                    If Not Up Then
                        RegionData1 = LastRegion1
                    Else
                        RegionData1 = LastRegion2
                    End If

                    '绘制连接信息
                    For Each Link As ComparativeGenomics.GeneLink In Links
                        Dim Region1 As Rectangle

                        If RegionData1.ContainsKey(Link.genome1) Then
                            Region1 = RegionData1(Link.genome1)
                        Else
                            Continue For
                        End If

                        Dim Region2 As Rectangle
                        If RegionData2.ContainsKey(Link.genome2) Then
                            Region2 = RegionData2(Link.genome2)
                        Else
                            Continue For
                        End If

                        Dim LinkdrModel = New GraphicsPath

                        Dim p1, p2, p3, p4 As Point
                        p1 = New Point(Region1.Location.X, Region1.Location.Y + Region1.Height + 3)
                        p2 = New Point(Region1.Right, Region1.Top + Region1.Height + 3)

                        If GNModel___1(Link.genome1).Direction < 0 Then
                            Call p1.Swap(p2)
                        End If

                        p3 = New Point(Region2.Right, Region2.Top - 3)
                        p4 = New Point(Region2.Location.X, Region2.Location.Y - 3)

                        If GNModel___2(Link.genome2).Direction < 0 Then
                            Call p3.Swap(p4)
                        End If

                        Call LinkdrModel.AddLine(p1, p2)
                        Call LinkdrModel.AddLine(p2, p3)
                        Call LinkdrModel.AddLine(p3, p4)
                        Call LinkdrModel.AddLine(p4, p1)
                        Call g.FillPath(New SolidBrush(If(Link.Color = Nothing, DefaultColor, Link.Color)), LinkdrModel)
                    Next

                    If Not Up Then
                        LastRegion1 = RegionData2
                    Else
                        LastRegion2 = RegionData2
                    End If
                Next

                If String.IsNullOrEmpty(color_overrides) Then
                    Call g.DrawingCOGColors(Model.COGColors, New Point(Margin, g.Height - Margin), Font, g.Width, Margin)
                End If
            End Sub
        End Class

        ''' <summary>
        ''' If the parameter color_overrides is not null then all of the gene 
        ''' color will overrides the cog color as the parameter specific 
        ''' color.
        ''' 
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="defaultColor"></param>
        ''' <param name="type2Arrow"></param>
        ''' <returns></returns>
        ''' <remarks>(绘制对比对图)</remarks>
        Public Function InvokeDrawing(Model As DrawingModel,
                                      Optional DefaultColor As Color = Nothing,
                                      Optional Type2Arrow As Boolean = True,
                                      Optional QueryMiddle As Boolean = False,
                                      Optional color_overrides As String = "") As GraphicsData

            Dim TitleDrawingFont As New Font("Microsoft YaHei", 20)
            Dim maxLenTitle As String = (From str As String In New String()() {New String() {Model.Query.Title}, (From mm In Model.aligns Select mm.Title).ToArray}.Unlist
                                         Select str
                                         Order By Len(str) Descending).First
            Dim MaxLengthTitleSize As SizeF = DriverLoad.MeasureTextSize(maxLenTitle, TitleDrawingFont) '得到最长的标题字符串作为基本的绘制长度的标准
            Dim plotSize As New Size(Margin * 10 + Model.Query.Length * InternalConvertFactor + MaxLengthTitleSize.Width * 2, 5 * Margin + Model.aligns.Count * (GeneHeight + 400))
            Dim theme As New Theme With {
                .mainCSS = New CSSFont(TitleDrawingFont, Brushes.Black).CSSValue,
                .padding = New Padding(Margin).ToString
            }
            Dim app As New PlotRegions(Model, theme, MaxLengthTitleSize, DefaultColor, Type2Arrow, QueryMiddle, color_overrides)

            Return app.Plot($"{plotSize.Width},{plotSize.Height}")
        End Function

        ''' <summary>
        ''' source is for the bbh besthit output directory, query  is the query genome id or query fasta filename, 
        ''' ppt is a directory which stores the ptt file of the bbh genomes, query_nt is using for the gcshew 
        ''' calculation visualize.
        ''' </summary>
        ''' <param name="source">bbh结果的文件夹</param>
        ''' <param name="query">query的编号</param>
        ''' <param name="PTT">存放多个物种的ptt文件的文件夹</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function BuildMultipleAlignmentModel(Source As String,
                                                Query As String,
                                                PTT As String,
                                                Subject_Fasta As String,
                                                Query_nt As FASTA.FastaSeq,
                                                Query_anno As IEnumerable(Of GeneTable)) As DrawingModel

            Dim bbhFiles = (From item In (From path In Source.LoadSourceEntryList({"*.csv"}).AsParallel
                                          Select ID = path.Key, pathValue = path.Value,
                               logEntry = VennDataBuilder.LogNameParser(path.Value),
                               BBH = path.Value.LoadCsv(Of BestHit)(False)).ToArray
                            Select item).ToArray
            If FileIO.FileSystem.FileExists(Query) Then
                Query = BaseName(Query)
            End If
            Dim hitsID = (From path As String In FileIO.FileSystem.GetFiles(Subject_Fasta, FileIO.SearchOption.SearchTopLevelOnly, "*.txt", "*.fasta", "*.fsa")
                          Select (From fsa In FASTA.FastaFile.Read(path) Select fsa.Headers.First).ToArray).ToVector
            Dim bir = (From item In bbhFiles Where String.Equals(Query, item.logEntry.HitName, StringComparison.OrdinalIgnoreCase) Select item).ToArray
            Dim queryBBH = (From item In bbhFiles Where String.Equals(Query, item.logEntry.QueryName, StringComparison.OrdinalIgnoreCase) Select item).ToArray
            '创建BBH数据，生成 gene link
            Dim BBH_DATA = (From bbhFile In queryBBH
                            Let bidirEquals = (From item In bir Where bbhFile.logEntry.BiDirEquals(item.logEntry) Select item).First
                            Select bbhFile.logEntry.BBH_ID, s2s = bbhFile.logEntry.BBH_ID,
                               BBH_Value = BBHParser.GetBBHTop(bbhFile.BBH.ToArray, bidirEquals.BBH.ToArray)).ToArray
            Dim Links As Orthology() = LinqAPI.Exec(Of Orthology) <= From item In BBH_DATA
                                                                     Select From pair As BiDirectionalBesthit
                                                                            In item.BBH_Value
                                                                            Where Not String.IsNullOrEmpty(pair.HitName)
                                                                            Select New Orthology With {
                                                                                .annotation = item.BBH_ID,
                                                                                .genome1 = pair.QueryName,
                                                                                .genome2 = pair.HitName,
                                                                                .spPairs = item.s2s
                                                                            }
            Dim loadPTT = (From PathEntry In PTT.LoadSourceEntryList({"*.ptt"}).AsParallel
                           Let PttData = TabularFormat.PTT.Load(PathEntry.Value)
                           Select ID = PathEntry.Key, Length = PttData.Size,
                              Title = PttData.Title,
                              GeneObjects = (From g As GeneBrief
                                             In PttData.GeneObjects
                                             Where Array.IndexOf(hitsID, g.Synonym) > -1
                                             Select g).ToArray).ToArray
            Dim categories As String() =
                LinqAPI.Exec(Of String) <= From gene
                                           In loadPTT
                                           Select From g As GeneBrief
                                                  In gene.GeneObjects
                                                  Let COG As String = Regex.Match(g.COG, "COG\d+", RegexOptions.IgnoreCase).Value
                                                  Where Not String.IsNullOrEmpty(COG)
                                                  Select COG
            Dim COGColors As Dictionary(Of String, Brush) =
                GenerateColorProfiles(categories.Distinct, False) _
                .ToDictionary(Function(x) x.Key,
                              Function(x) DirectCast(New SolidBrush(x.Value), Brush))
            Dim COGsBrush As ICOGsBrush = loadPTT.Select(Function(x) x.GeneObjects) _
                                                 .IteratesALL _
                                                 .COGsColorBrush(, COGColors)
            Dim LQuery As GenomeModel() =
                loadPTT.Select(Function(x)
                                   Return ComparativeGenomics.ModelAPI.CreateSyntenyGenome(x.GeneObjects,
                                                                  x.Length,
                                                                  x.Title,
                                                                  getId:=Function(g) g.Synonym,
                                                                  COGsColor:=COGsBrush)
                               End Function) ' 将PTT文件之中的所有数据都转换为模型数据
            Dim QueryCOGs As Dictionary(Of String, Brush) = Nothing
            Dim QueryModel As GenomeModel = ComparativeGenomics.ModelAPI.CreateObject(Query_anno.ToArray, Query_nt, COGsColor:=QueryCOGs)
            Dim QueryIDList As String() = queryBBH.First.BBH.Select(Function(x) x.QueryName)

            For Each Color As KeyValuePair(Of String, Brush) In QueryCOGs
                If Not COGColors.ContainsKey(Color.Key) Then
                    Call COGColors.Add(Color.Key, Color.Value)
                End If
            Next

            QueryModel.genes = (From item In QueryModel.genes Where Array.IndexOf(QueryIDList, item.locus_tag) > -1 Select item).ToArray
            Dim ql = (From item In QueryModel.genes Select New Integer() {item.Left, item.Right}).ToArray.Unlist
            QueryModel.Length = ql.Max - ql.Min

            Dim Min As Double = ql.Min

            For Each GeneObject In QueryModel.genes
                GeneObject.Left = GeneObject.Left - Min + 100
                GeneObject.Right = GeneObject.Right - Min + 100
            Next

            Dim LinkWith = Function(Id__1 As String, id___2 As String) As Boolean
                               Dim LinksLQuery = (From ItemLink In Links Where ItemLink.Equals(Id__1, id___2) Select ItemLink).ToArray
                               Return Not LinksLQuery.IsNullOrEmpty
                           End Function

            '对query按照从小到大进行排序，然后找出可能的空缺，这个排序操作是对对齐所必需的
            QueryModel.genes = (From item In QueryModel.genes Order By item.Left Ascending).ToArray

            Dim Models = (From DrawingModel In LQuery Select DrawingModel Order By DrawingModel.genes.Count Descending).ToArray '绘图的时候按照比对上的数目的多少来进行排序
            Dim MaxGenomeLength = (From DrawingModel As ComparativeGenomics.GenomeModel In Models
                                   Let LociList As List(Of Integer) = (From GeneObject In DrawingModel.genes Select New Integer() {GeneObject.Left, GeneObject.Right}).ToArray.Unlist
                                   Select LociList.Max - LociList.Min).ToArray.Max

            QueryModel.Length = Math.Max(QueryModel.Length, MaxGenomeLength)
            MaxGenomeLength = QueryModel.Length
            Models = (From DrawingModel In Models Select __internalFilter(DrawingModel, QueryModel, LinkWith, MaxGenomeLength)).ToArray

            Dim Reader As New NucleicAcid(Query_nt)
            Query_nt.SequenceData = Reader.ReadSegment(ql.Min, QueryModel.Length)
            Query_nt.Headers = New String() {Query_nt.Headers.First, ql.Min.ToString, ql.Max.ToString}

            Return New DrawingModel With {
                .nt = Query_nt,
                .links = Links,
                .aligns = Models,
                .Query = QueryModel,
                .COGColors = COGColors
            }
        End Function

        Private Function __internalFilter(subject As ComparativeGenomics.GenomeModel,
                                      QueryModel As ComparativeGenomics.GenomeModel,
                                      LinkWith As Func(Of String, String, Boolean),
                                      maxLen As Integer) As ComparativeGenomics.GenomeModel

            Dim SubjectLength = (From GeneObject In subject.genes Select New Integer() {GeneObject.Left, GeneObject.Right}).ToArray.Unlist
            subject.Length = QueryModel.Length '定长

            '按照最佳比对结果找出空缺的长度
            Dim IndexList As List(Of Integer) = New List(Of Integer)
            For Each GeneObject In QueryModel.genes '按照位置从小到大进行排序了的
                Dim sfs = (From SubjectGene In subject.genes Where LinkWith(SubjectGene.locus_tag, GeneObject.locus_tag) Select SubjectGene).ToArray
                If Not sfs.IsNullOrEmpty Then
                    Call IndexList.Add(Array.IndexOf(subject.genes, sfs.First))
                End If
            Next

            Dim Direction As Integer() = (From n In IndexList.CreateSlideWindows(2) Select n.Items.First - n.Items.Last).ToArray

            If (From nnn In Direction Where nnn > 0 Select nnn).ToArray.Count >= (subject.genes.Count - 1) / 2 OrElse
                (Direction.Count = 1 AndAlso (subject.genes.First.Direction = -1 OrElse subject.genes.Last.Direction = -1)) Then

                '反向的   '假设所有的基因都是挨在一起的
                '则尝试将所有基因进行反向映射

                Dim Mmmmmax = SubjectLength.Max

                For Each SubjectGeneObject In subject.genes

                    SubjectGeneObject.Direction = If(SubjectGeneObject.Direction = 1, -1, 1)

                    Dim SwapedLeft As Integer = Mmmmmax - SubjectGeneObject.Right
                    Dim SwapedRight As Integer = Mmmmmax - SubjectGeneObject.Left

                    SubjectGeneObject.Left = SwapedLeft
                    SubjectGeneObject.Right = SwapedRight
                Next

                GoTo POSITIONNING
            End If

            Dim Mmmmin As Integer = SubjectLength.Min

            For Each GeneObject In subject.genes
                GeneObject.Right = GeneObject.Right - Mmmmin + 100
                GeneObject.Left = GeneObject.Left - Mmmmin + 100
            Next

            '到这里位置，完成了方向的修正
            '开始计算位置的补偿
POSITIONNING:

            ' 由于关系是来自于直系同源，所以位置的排布 是一样的

            subject.genes = (From SubjectGeneObject In subject.genes Select SubjectGeneObject Order By SubjectGeneObject.Left Ascending).ToArray

            '按照最佳比对结果找出空缺的长度
            Dim idx As Integer, ii As Integer
            For ii = 0 To QueryModel.genes.Count - 1  '按照位置从小到大进行排序了的
                Dim itffffem = QueryModel.genes(ii)
                Dim sfs = (From itemffff In subject.genes Where LinkWith(itemffff.locus_tag, itffffem.locus_tag) Select itemffff).ToArray
                If Not sfs.IsNullOrEmpty Then
                    idx = Array.IndexOf(subject.genes, sfs.First)
                    Exit For
                End If
            Next
            'idx为修正方向之后的第一个与query的第一个所匹配的基因
            '假若subject的偏移大于query则对query之中的每一个基因进行偏移补偿，反之则是对subject
            Dim delta = subject.genes(idx).Left - QueryModel.genes(ii).Left

            If delta > 0 Then '对query进行位置补偿

                For Each GeneObject As ComparativeGenomics.GeneObject In QueryModel.genes
                    GeneObject.Left += delta
                    GeneObject.Right += delta
                Next

                QueryModel.Length += delta

            Else

                delta *= -1
                delta += QueryModel.genes.First.Left

                For Each gene In subject.genes
                    gene.Left += delta
                    gene.Right += delta
                Next

                subject.Length += delta

            End If

            subject.SegmentOffset = SubjectLength.Min + delta

            Return subject
        End Function

        ''' <summary>
        ''' 对双组分系统进行颜色赋值，其他的基因不变
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("visualize.schema.tcs")>
        Public Function TCSVisualization(model As DrawingModel, Optional HK_color As Color = Nothing, Optional RR_color As Color = Nothing, Optional hhk_color As Color = Nothing) As DrawingModel
            If HK_color = Nothing OrElse HK_color.IsEmpty Then
                HK_color = Color.Cyan
            End If

            If RR_color = Nothing OrElse RR_color.IsEmpty Then
                RR_color = Color.Purple
            End If

            If hhk_color = Nothing OrElse hhk_color.IsEmpty Then
                hhk_color = Color.Red
            End If

            Dim HKBr As Brush = New SolidBrush(HK_color)
            Dim RRBr As Brush = New SolidBrush(RR_color)
            Dim HHKBr As Brush = New SolidBrush(hhk_color)


            For Each Genome In {model.aligns, New ComparativeGenomics.GenomeModel() {model.Query}}.Unlist
                For Each Gene In Genome.genes
                    If InStr(Gene.geneName, "hybrid", CompareMethod.Text) > 0 Then
                        Gene.Color = HHKBr
                    ElseIf InStr(Gene.geneName, "kinase", CompareMethod.Text) > 0 OrElse InStr(Gene.geneName, "sensor", CompareMethod.Text) > 0 OrElse InStr(Gene.geneName, "signal", CompareMethod.Text) > 0 Then
                        Gene.Color = HKBr   'hk
                    ElseIf InStr(Gene.geneName, "regulat", CompareMethod.Text) > 0 OrElse InStr(Gene.geneName, "response", CompareMethod.Text) > 0 Then
                        Gene.Color = RRBr  'rr
                    End If
                Next
            Next

            Return model
        End Function

        ''' <summary>
        ''' Build the comparative drawing model from the ncbi ptt source.
        ''' (数据框之中的每一行数据都表示同源基因，列表示为基因组) 
        ''' </summary>
        ''' <param name="DF">生成GeneLink数据</param>
        ''' <param name="ColumnList">假若本参数值为空，则默认取出所有的数据</param>
        ''' <param name="PttSource">请注意，这个值的顺序是与数据框之中的列是一一对应的</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BuildModel(DF As DataFrameResolver,
                               <Parameter("List.Paths.Ptt", "The source file list of the ptt data of the target drawing genomes.")> PttSource As IEnumerable(Of String),
                               <Parameter("List.ID", "The column id headers in the data frame csv data file.")> Optional ColumnList As IEnumerable(Of String) = Nothing,
                               <Parameter("Query.ID", "The column query id in the data frame.")> Optional Query As String = "",
                               <Parameter("Offset.Ignored")> Optional IgnoreOffset As Boolean = False,
                               <Parameter("Using.List.Id.As.Name")> Optional UsingColumnHeadersAsName As Boolean = False) As DrawingModel

            If ColumnList Is Nothing OrElse Not ColumnList.Any Then
                ColumnList = DF.HeadTitles
            End If

            If String.IsNullOrEmpty(Query) Then
                Query = ColumnList.First
            Else
                If Regex.Match(Query, "\d+").Value.Equals(Query) Then
                    Query = ColumnList.ElementAtOrDefault(CInt(Val(Query)))
                End If
            End If

            Dim Orders As Integer() = DF.GetOrdinalSchema(ColumnList.ToArray)
            Dim Model As New DrawingModel
            Dim Links As New List(Of Orthology)

            For i As Integer = 0 To Orders.Count - 1
                If Orders(i) = -1 Then
                    Throw New Exception(String.Format("[DEBUG] Could not found the column index ""{0}"".", ColumnList(i)))
                End If
            Next

            Dim PreGeneCombs As String() = Nothing

            Do While DF.Read
                Dim Genes = (From p As Integer In Orders Let s As String = DF.GetValue(p) Where Not String.IsNullOrEmpty(s) Select s).ToArray
REPEAT:         Dim Combs As List(Of Tuple(Of String, String)) = Comb(Of String).CreateObject(Genes).CombList.Unlist
                Combs = (From cb As Tuple(Of String, String)
                     In Combs
                         Where Not (cb.Item1.First = "/"c OrElse cb.Item2.First = "/"c) Select cb).AsList
                Links += From cb As Tuple(Of String, String)
                         In Combs
                         Select New Orthology With {
                             .genome1 = cb.Item1,
                             .genome2 = cb.Item2
                         }
                Genes = (From ID As String In Genes Where Not ID.First = "/"c Select ID).ToArray
                If Genes.Count = 1 And Combs.IsNullOrEmpty Then
                    Genes = {Genes, PreGeneCombs}.ToVector
                    GoTo REPEAT
                End If
                PreGeneCombs = Genes
            Loop

            Model.links = Links.ToArray

            Dim Qp As Integer = Array.IndexOf(ColumnList, Query)
            Dim queryPttPath$ = PttSource(Qp)
            Dim QueryPtt = TabularFormat.PTT.Load(queryPttPath)
            Dim SubjectsPtt = (From i As Integer
                               In PttSource.Sequence
                               Let path As String = PttSource(i), S_ID As String = ColumnList(i)
                               Where Not String.Equals(path, queryPttPath)
                               Select path,
                                   S_ID,
                                   Ptt = TabularFormat.PTT.Load(path)).ToArray

            Model.Query = ComparativeGenomics.ModelAPI.CreateObject(QueryPtt, COGsColor:=Nothing)
            Model.aligns = (From Genome In SubjectsPtt Select ComparativeGenomics.ModelAPI.CreateObject(Genome.Ptt, COGsColor:=Nothing)).ToArray

            If UsingColumnHeadersAsName Then
                Dim TempList = ColumnList.AsList
                Dim setValue = New SetValue(Of GenomeModel) <= NameOf(GenomeModel.Title)

                Call TempList.RemoveAt(Qp)
                Model.Query.Title = ColumnList(Qp)
                Model.aligns =
                    LinqAPI.Exec(Of GenomeModel) <= From i As Integer
                                                    In Model.aligns.Sequence
                                                    Let Genome = SubjectsPtt(i)
                                                    Let Subject = Model.aligns(i)
                                                    Select setValue(Subject, TempList(i))
            End If

            Dim HitsID As String() = (From goLink In Model.links Select New String() {goLink.genome1, goLink.genome2}).ToArray.Unlist.Distinct.ToArray
            Dim LinkWith = Function(Id__1 As String, id___2 As String) As Boolean
                               Dim LinksLQuery = (From goLink In Links Where goLink.Equals(Id__1, id___2) Select goLink).ToArray
                               Return Not LinksLQuery.IsNullOrEmpty
                           End Function

            Model.Query.genes = (From gene As GeneObject
                                      In Model.Query.genes
                                 Where Array.IndexOf(HitsID, gene.locus_tag) > -1
                                 Select gene).ToArray
            Dim QueryModel = Model.Query
            Dim QueryLength = (From GeneObject As ComparativeGenomics.GeneObject
                           In QueryModel.genes
                               Select New Integer() {GeneObject.Left, GeneObject.Right}).ToArray.Unlist
            QueryModel.Length = QueryLength.Max - QueryLength.Min

            Dim Min As Double = QueryLength.Min
            Dim setGenesValue = New SetValue(Of GenomeModel) <= NameOf(GenomeModel.genes)

            Model.aligns = LinqAPI.Exec(Of GenomeModel) <=
                From genome As GenomeModel
                In Model.aligns.AsParallel
                Let values = (From gene As GeneObject
                              In genome.genes
                              Where Array.IndexOf(HitsID, gene.locus_tag) > -1
                              Select gene).ToArray
                Select setGenesValue(genome, values)

            QueryLength = (From GenomeDrawingModel As ComparativeGenomics.GenomeModel
                       In Model.aligns
                           Let Loci As List(Of Integer) = (From Gene As ComparativeGenomics.GeneObject
                                                       In GenomeDrawingModel.genes
                                                           Select New Integer() {Gene.Left, Gene.Right}).ToArray.Unlist
                           Select Loci.Max - Loci.Min).AsList

            Model.Query.Length = Math.Max(Model.Query.Length, QueryLength.Max)
            Model.Query.SegmentOffset = Min

            For Each GeneObject As ComparativeGenomics.GeneObject In QueryModel.genes
                GeneObject.Left = GeneObject.Left - Min + 100
                GeneObject.Right = GeneObject.Right - Min + 100
            Next

            Model.aligns = (From Genome In Model.aligns.AsParallel Select __internalFilter(Genome, Model.Query, LinkWith, maxLen:=Model.Query.Length)).ToArray

            If IgnoreOffset Then
                QueryLength = (From item In QueryModel.genes Select New Integer() {item.Left, item.Right}).ToArray.Unlist
                QueryModel.Length = QueryLength.Max - QueryLength.Min
                Min = QueryLength.Min

                Model.Query.SegmentOffset = Min

                For Each GeneObject In QueryModel.genes
                    GeneObject.Left = GeneObject.Left - Min + 100
                    GeneObject.Right = GeneObject.Right - Min + 100
                Next
            End If

            Return Model
        End Function
    End Module
End Namespace
