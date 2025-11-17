#Region "Microsoft.VisualBasic::fa63c6a601da0f7fd099bff4006cfa5b, visualize\ChromosomeMap\ChromesomeMapAPI.vb"

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

    '   Total Lines: 432
    '    Code Lines: 350 (81.02%)
    ' Comment Lines: 37 (8.56%)
    '    - Xml Docs: 97.30%
    ' 
    '   Blank Lines: 45 (10.42%)
    '     File Size: 19.59 KB


    ' Module ChromesomeMapAPI
    ' 
    '     Function: __getRandomColor, AddLociSites, AddMotifSites, AddMutationData, ApplyCogColorProfile
    '               (+2 Overloads) CreateDevice, DescribTest, ExportColorInformation, FromGenbankDIR, FromGenes
    '               FromPTT, FromPttObject, get_Converted, GetDefaultConfiguration, InvokeDrawing
    '               LoadConfig, READ_PlasmidData, SaveImage, WriteGeneFasta
    '     Class __setRNAColorInvoke
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __setColor, __setColorBrush
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework.Extensions
Imports Microsoft.VisualBasic.Data.Framework.IO.Properties
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.ListExtensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize.ChromosomeMap.Configuration
Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

''' <summary>
''' This module contains the required API function for create the chromosomes map of a specific bacteria genome.
''' </summary>
<Package("Data.Visualization.Chromosome_Map",
                        Description:="You can using this tool to create the map of the whole bacteria genome.",
                        Publisher:="xie.guigang@gcmodeller.org",
                        Url:="http://gcmodeller.org")>
Public Module ChromesomeMapAPI

    <ExportAPI("DescribTest")>
    Public Function DescribTest(genome As PTT, config As Config, Optional sites As Integer = 500) As ChromesomeDrawingModel
        Dim rand As New Random(5 * (Now.ToBinary Mod 23))
        Dim nSites = (From i As Integer In sites.Sequence.AsParallel
                      Let lociLeft As Integer = genome.Size * rand.NextDouble
                      Let loci = New NucleotideLocation(lociLeft, lociLeft + 1, lociLeft Mod 2 = 0)
                      Select relateds = genome.GetRelatedGenes(loci), loci).ToArray
        Dim TSSs = (From obj In nSites.AsParallel
                    Select New TSSs With {
                        .Left = obj.loci.left,
                        .Comments = obj.relateds.Select(Function(x) x.ToString).JoinBy(vbCrLf),
                        .Right = obj.loci.right,
                        .Strand = obj.loci.Strand,
                        .SiteName = obj.GetHashCode,
                        .Synonym = obj.loci.GetHashCode}).ToArray
        Dim model As ChromesomeDrawingModel = ChromesomeMapAPI.FromPTT(genome, config)
        model.TSSs = TSSs
        Return model
    End Function

    <ExportAPI("Add.MutationSites")>
    Public Function AddMutationData(Model As ChromesomeDrawingModel,
                                    <Parameter("Sites", "The mutation sites which contains the mutation type and the location on the genome nt sequence.")>
                                    SitesData As Generic.IEnumerable(Of MultationPointData)) As ChromesomeDrawingModel
        Model.MutationDatas = SitesData
        Return Model
    End Function

    <ExportAPI("Gene2Fasta", Info:="Convert the gene annotation data into a fasta sequence.")>
    Public Function WriteGeneFasta(<Parameter("Genes.Anno")> data As IEnumerable(Of PlasmidAnnotation)) As FastaFile
        Dim falist As IEnumerable(Of FastaSeq) =
            From gene As PlasmidAnnotation
            In data
            Where Not String.IsNullOrEmpty(gene.GeneNA)
            Select New FastaSeq With {
                .Headers = New String() {gene.ORF_ID},
                .SequenceData = gene.GeneNA
            }

        Return New FastaFile(falist)
    End Function

    <ExportAPI("Read.Csv.PlasmidAnno", Info:="Reads the plasmid annotation data from a csv document.")>
    Public Function READ_PlasmidData(path As String) As PlasmidAnnotation()
        Return path.LoadCsv(Of PlasmidAnnotation)(False).ToArray
    End Function

    <ExportAPI("PTT.From.Plasmid",
               Info:="Generates the PTT genome data from the plasmid annotation data. NT.Src parameter is the file path of the original genome fasta source sequence.")>
    Public Function get_Converted(Annotations As IEnumerable(Of PlasmidAnnotation),
                                  <Parameter("NT.Src", "The nt fasta sequence of the plasmid whole genome.")> src As String) As PTTDbLoader
        Dim LoaderObject = PTTDbLoader.CreateObject(Annotations, FastaSeq.Load(src))
        Dim AnnoDict = Annotations.ToDictionary(Function(ann) ann.ORF_ID)

        For Each gene As GeneBrief In LoaderObject.Values
            gene.Gene = AnnoDict(gene.Synonym).Gene_name
        Next

        Return LoaderObject
    End Function

    <ExportAPI("Add.Motif_Sites")>
    Public Function AddMotifSites(model As ChromesomeDrawingModel, data As IEnumerable(Of SMRUCC.genomics.ComponentModel.Loci.Loci)) As ChromesomeDrawingModel
        Dim LQuery = (From loci As SMRUCC.genomics.ComponentModel.Loci.Loci
                      In data.AsParallel
                      Select site = New DrawingModels.MotifSite With {
                            .Left = loci.Left,
                            .Regulators = New String() {},
                            .Right = loci.Right,
                            .MotifName = loci.TagData,
                            .SiteName = loci.TagData,
                            .Comments = loci.TagData,
                            .Color = Color.Black}
                      Order By site.Left Ascending).ToArray
        model.MotifSites = model.MotifSites.Join(LQuery).ToArray
        Return model
    End Function

    <ExportAPI("Add.Loci_Sites")>
    Public Function AddLociSites(model As ChromesomeDrawingModel, data As IEnumerable(Of NucleotideModels.SegmentObject)) As ChromesomeDrawingModel
        Dim Locis = LinqAPI.Exec(Of DrawingModels.Loci) <=
            From site As NucleotideModels.SegmentObject
            In data
            Select New DrawingModels.Loci With {
                .SiteName = site.Title,
                .SequenceData = site.SequenceData,
                .Right = site.right,
                .Left = site.left,
                .Color = Color.Black
            }
        model.Loci = Locis
        Return model
    End Function

    <ExportAPI("Device.Invoke_Drawing")>
    Public Function InvokeDrawing(<Parameter("Gr.Device")> Device As DrawingDevice,
                                  <Parameter("Chr.Gr.Model", "The drawing object which was represents the bacteria genome chromosomes.")>
                                  Model As ChromesomeDrawingModel) As GraphicsData()
        Return Device.InvokeDrawing(Model)
    End Function

    ''' <summary>
    ''' 按照COG分类来赋值COG颜色的
    ''' </summary>
    ''' <param name="Model"></param>
    ''' <param name="MyvaCOG"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("ColorProfiles.Apply.From.COGs")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ApplyCogColorProfile(Model As ChromesomeDrawingModel,
                                         <Parameter("COG.Myva")>
                                         MyvaCOG As IEnumerable(Of ICOGCatalog),
                                         Optional alpha% = 230) As ChromesomeDrawingModel
        Return MyvaCOG.ToArray.ApplyingCOGCategoryColor(Model, alpha)
    End Function

    ''' <summary>
    ''' 使用这个函数进行绘图设备的配置参数的读取操作
    ''' </summary>
    ''' <param name="file"></param>  
    ''' <returns></returns>
    ''' 
    <ExportAPI("Read.TXT.Drawing_Config")>
    Public Function LoadConfig(File As String) As Config
        Return File.LoadConfiguration(Of Config)(AddressOf GetDefaultConfiguration)
    End Function

    <ExportAPI("Device.DefaultConfiguration")>
    Public Function GetDefaultConfiguration(<Parameter("Path.Save")> SaveFile As String) As Config
        Dim df As Config = Config.DefaultValue
        Call df.ToConfigDoc.SaveTo(SaveFile)
        Return df
    End Function

    ''' <summary>
    ''' 请注意，在宽度上面是4倍的Margin
    ''' </summary>
    ''' <param name="Config"></param>
    ''' <returns></returns>
    <ExportAPI("Device.From.Configuration")>
    <Extension>
    Public Function CreateDevice(Config As Config) As DrawingDevice
        Return Config.FromConfig()
    End Function

    <ExportAPI("Device.Open")>
    Public Function CreateDevice(args As CommandLine) As DrawingDevice
        Dim width As String = args("-width")
        Dim height As String = args("-height")

        If String.IsNullOrEmpty(height) Then
            Return New DrawingDevice(Width:=Val(width))
        Else
            Return New DrawingDevice(Val(width), Val(height))
        End If
    End Function

    ''' <summary>
    ''' Image formats can be one of the value: ``jpg,bmp,emf,exif,gif,png,wmf,tiff``
    ''' </summary>
    ''' <param name="res"><see cref="DrawingDevice.InvokeDrawing(ChromesomeDrawingModel)"/>函数所生成的绘图图形输出资源数据</param>
    ''' <param name="EXPORT">将要进行数据保存的文件夹</param>
    ''' <param name="Format">Value variant in ``jpg,bmp,emf,exif,gif,png,wmf,tiff``</param>
    ''' <returns></returns>
    <ExportAPI("Resource.Save")>
    <Extension>
    Public Function SaveImage(<Parameter("Resource")> res As GraphicsData(),
                              <Parameter("DIR.EXPORT")> EXPORT$,
                              <Parameter("Image.Format", "Value variant in jpg,bmp,emf,exif,gif,png,wmf,tiff")>
                              Optional format As ImageFormats = ImageFormats.Png) As Integer

        Dim i As i32 = 0

        Call EXPORT.MakeDir

        For Each Bitmap As GraphicsData In res
            Call Bitmap.Save($"{EXPORT}/ChromosomeMap [{++i}].{format.Description}")
        Next

        Return i
    End Function

    <ExportAPI("Export.MapInfo")>
    Public Function ExportColorInformation(device As DrawingDevice, model As ChromesomeDrawingModel) As Image
        Return device.ExportColorProfiles(model)
    End Function

    <ExportAPI("DrawingModel.From.PTT")>
    Public Function FromGenbankDIR(<Parameter("DIR.PTT")> PTT_DIR As String, conf As Config) As ChromesomeDrawingModel
        Dim PTTFile As String = PTT_DIR.TheFile("*.ptt")
#Region ""
        Dim genes = LinqAPI.MakeList(Of SegmentObject) <=
            From gene As GeneBrief
            In PTT.Load(PTTFile).GeneObjects
            Select New SegmentObject With {
                .Color = New SolidBrush(Color.Black),
                .Product = gene.Product,
                .LocusTag = gene.Synonym,
                .CommonName = gene.Gene,
                .Left = Math.Min(gene.Location.left, gene.Location.right),
                .Right = Math.Max(gene.Location.right, gene.Location.left),
                .Direction = gene.Location.Strand
            }
#End Region
        PTTFile = PTT_DIR.TheFile("*.rnt")

        Dim configs As Configuration.DataReader = conf.ToConfigurationModel
        Dim SetRNAColor As New __setRNAColorInvoke(configs)
        Dim RNA = LinqAPI.Exec(Of SegmentObject) <=
            From gene As GeneBrief
            In PTT.Load(PTTFile).GeneObjects
            Select New SegmentObject With {
                .Color = SetRNAColor.__setColorBrush(gene.Product),
                .Product = gene.Product,
                .LocusTag = gene.Synonym,
                .CommonName = gene.Gene,
                .Left = Math.Min(gene.Location.left, gene.Location.right),
                .Right = Math.Max(gene.Location.right, gene.Location.left),
                .Direction = gene.Location.Strand
            }

        genes += RNA

        With Rpt.Load(Of ChromesomeDrawingModel)(PTT_DIR.TheFile("*.rpt"))
            .GeneObjects = genes _
                .OrderBy(Function(gene) gene.Left) _
                .ToArray
            .Configuration = configs
            .MutationDatas = New MultationPointData() {}

            Return DirectCast(.ByRef, ChromesomeDrawingModel)
        End With
    End Function

    <ExportAPI("DrawingModel.From.PTT", Info:="Creates a basically simple drawing model object from the PTT file data.")>
    Public Function FromPTT(PTT As PTT, Optional conf As Config = Nothing) As ChromesomeDrawingModel
        With FromGenes(PTT, conf Or Config.DefaultValue, PTT.Size)
            .CDSCount = PTT.NumOfProducts
            Return .ByRef
        End With
    End Function

    ReadOnly brown As [Default](Of String) = NameOf(Color.Brown)

    ''' <summary>
    ''' 通常使用这个方法从PTT构件之中生成部分基因组的绘制模型数据
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <param name="conf"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 在这个函数之中所生成的绘图模型的基因模型之中还没有颜色画刷值
    ''' </remarks>
    <ExportAPI("DrawingModel.From.PTT.Elements")>
    Public Function FromGenes(<Parameter("PTT.Genes")>
                              genes As IEnumerable(Of GeneBrief),
                              conf As Config,
                              <Parameter("Ranges", "The nt length of the gene objects contains in the region.")>
                              rangeLen As Integer) As ChromesomeDrawingModel
#Region "Create gene models"
        Dim defaultColor As Color = (conf.NoneCogColor Or brown).TranslateColor
        Dim geneModels = LinqAPI.Exec(Of SegmentObject) <=
                                                          _
            From gene As GeneBrief
            In genes
            Let position As Location = gene.Location.Normalization
            Select gm = New SegmentObject With {
                .Color = New SolidBrush(defaultColor),
                .Product = gene.Product,
                .LocusTag = gene.Synonym,
                .CommonName = gene.Gene,
                .Left = position.left,
                .Right = position.right,
                .Direction = gene.Location.Strand
            }
            Order By gm.Left Ascending
#End Region
        Dim model As New ChromesomeDrawingModel With {
            .CDSCount = geneModels.Count,
            .Size = rangeLen,
            .NumberOfGenes = geneModels.Count,
            .ProteinCount = geneModels.Count,
            .PseudoCDSCount = 0,
            .PseudoGeneCount = 0,
            .RNACount = 0,
            .GeneObjects = geneModels,
            .Configuration = conf.ToConfigurationModel,
            .MutationDatas = New MultationPointData() {}
        }

        Return model
    End Function

    Private Class __setRNAColorInvoke

        Dim tRnaColor, DefaultRNAColor, rRNAColor As Color
        Dim _tRnaColor, _DefaultRNAColor, _rRNAColor As Brush

        Sub New(configurations As Configuration.DataReader)
            Me.tRnaColor = configurations.tRNAColor
            Me.DefaultRNAColor = configurations.DefaultRNAColor
            Me.rRNAColor = configurations.ribosomalRNAColor
            Me._rRNAColor = New SolidBrush(rRNAColor)
            Me._DefaultRNAColor = New SolidBrush(DefaultRNAColor)
            Me._tRnaColor = New SolidBrush(tRnaColor)
        End Sub

        Public Function __setColor(Product As String) As Color
            If String.IsNullOrEmpty(Product) Then
                Return DefaultRNAColor
            End If

            If InStr(Product, "ribosomal RNA") > 0 Then
                Return rRNAColor
            ElseIf InStr(Product, "tRNA") > 1 Then
                Return tRnaColor
            Else
                Return DefaultRNAColor
            End If
        End Function

        Public Function __setColorBrush(Product As String) As Brush
            If String.IsNullOrEmpty(Product) Then
                Return _DefaultRNAColor
            End If

            If InStr(Product, "ribosomal RNA") > 0 Then
                Return _rRNAColor
            ElseIf InStr(Product, "tRNA") > 1 Then
                Return _tRnaColor
            Else
                Return _DefaultRNAColor
            End If
        End Function
    End Class

    <ExportAPI("DrawingModel.From.PTT")>
    Public Function FromPttObject(<Parameter("Bacterial.Genome", "Using the gene object model data that define in the database to construct the basically bacterial genome skeleton.")>
                                  genome As PTTDbLoader, conf As Config) As ChromesomeDrawingModel
        Dim defaultColor As Color = (conf.NoneCogColor Or brown).TranslateColor
        Dim GeneObjects As SegmentObject() = (From gene As GeneBrief
                           In genome.Values.AsParallel
                                              Select New SegmentObject With {
                               .Color = New SolidBrush(defaultColor),
                               .Product = gene.Product,
                               .LocusTag = gene.Synonym,
                               .CommonName = gene.Gene,
                               .Left = Math.Min(gene.Location.left, gene.Location.right),
                               .Right = Math.Max(gene.Location.right, gene.Location.left),
                               .Direction = gene.Location.Strand
                               }).AsList

        Dim configurations = conf.ToConfigurationModel
        Dim Model As ChromesomeDrawingModel = genome.RptGenomeBrief.CopyTo(Of ChromesomeDrawingModel)()
        Model.GeneObjects = (From GeneObject As SegmentObject
                                 In GeneObjects
                             Select GeneObject
                             Order By GeneObject.Left Ascending).ToArray
        Model.Configuration = configurations
        Model.MutationDatas = New MultationPointData() {}

        Return Model
    End Function
End Module
