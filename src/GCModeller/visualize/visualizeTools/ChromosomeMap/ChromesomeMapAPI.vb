#Region "Microsoft.VisualBasic::446e75b1c70e75fb9374f732b1fc2662, ..\GCModeller\visualize\visualizeTools\ChromosomeMap\ChromesomeMapAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Drawing
Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.Java.IO.Properties.Reflector
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels

Namespace ChromosomeMap

    ''' <summary>
    ''' This module contains the required API function for create the chromosomes map of a specific bacteria genome.
    ''' </summary>
    <[PackageNamespace]("Data.Visualization.Chromosome_Map",
                        Description:="You can using this tool to create the map of the whole bacteria genome.",
                        Publisher:="xie.guigang@gcmodeller.org",
                        Url:="http://gcmodeller.org")>
    Public Module ChromesomeMapAPI

        <ExportAPI("DescribTest")>
        Public Function DescribTest(genome As PTT, config As Configurations, Optional sites As Integer = 500) As ChromesomeDrawingModel
            Dim rand As New Random(5 * (Now.ToBinary Mod 23))
            Dim nSites = (From i As Integer In sites.Sequence.AsParallel
                          Let lociLeft As Integer = genome.Size * rand.NextDouble
                          Let loci = New SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation(lociLeft, lociLeft + 1, lociLeft Mod 2 = 0)
                          Select relateds = genome.GetRelatedGenes(loci), loci).ToArray
            Dim TSSs = (From obj In nSites.AsParallel
                        Select New DrawingModels.TSSs With {
                            .Left = obj.loci.Left,
                            .Comments = obj.relateds.ToArray(Function(x) x.ToString).JoinBy(vbCrLf),
                            .Right = obj.loci.Right,
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
        Public Function WriteGeneFasta(<Parameter("Genes.Anno")> data As Generic.IEnumerable(Of PlasmidAnnotation)) As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
            Return (From Gene As PlasmidAnnotation In data
                    Where Not String.IsNullOrEmpty(Gene.GeneNA)
                    Select New SMRUCC.genomics.SequenceModel.FASTA.FastaToken With {
                        .Attributes = New String() {Gene.ORF_ID},
                        .SequenceData = Gene.GeneNA}).ToArray
        End Function

        <ExportAPI("Read.Csv.PlasmidAnno", Info:="Reads the plasmid annotation data from a csv document.")>
        Public Function READ_PlasmidData(path As String) As PlasmidAnnotation()
            Return path.LoadCsv(Of PlasmidAnnotation)(False).ToArray
        End Function

        <ExportAPI("PTT.From.Plasmid", Info:="Generates the PTT genome data from the plasmid annotation data. NT.Src parameter is the file path of the original genome fasta source sequence.")>
        Public Function get_Converted(Annotations As Generic.IEnumerable(Of PlasmidAnnotation),
                                      <Parameter("NT.Src", "The nt fasta sequence of the plasmid whole genome.")> src As String) As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTTDbLoader
            Dim LoaderObject = SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTTDbLoader.CreateObject(Annotations, SMRUCC.genomics.SequenceModel.FASTA.FastaToken.Load(src))
            Dim AnnoDict = Annotations.ToDictionary(Function(item) item.ORF_ID)
            For Each Gene As GeneBrief In LoaderObject.Values
                Gene.Gene = AnnoDict(Gene.Synonym).Gene_name
            Next
            Return LoaderObject
        End Function

        <ExportAPI("MyvaCOG.Export.From.PTT",
                   Info:="If the ptt data file containing the COG information of the genes, then you can using this function to export the COG information from the ptt file using for the downstream visualization.")>
        Public Function TryExportMyva(data As PTTDbLoader) As RpsBLAST.MyvaCOG()
            Return data.ExportCOGProfiles(Of RpsBLAST.MyvaCOG)()
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="data">可以使用<see cref="GenomeMotifFootPrints.GroupMotifs"></see>方法来合并一些重复的motif数据</param>
        ''' <param name="onlyRegulations">如果为真，则仅会将有调控因子的位点进行转换，如果为假，则所有的位点都会被绘制出来</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Add.Motif_Sites", Info:="Trim.Regulation: if TRUE, then only the motif site which has the regulator will be drawing on the map, else all of the motif site will be drawing on the maps.")>
        Public Function AddMotifSites(model As ChromesomeDrawingModel,
                                      data As Generic.IEnumerable(Of PredictedRegulationFootprint),
                                      <Parameter("Trim.Regulation", "If the parameter is TRUE, then only the site data with regulation data that will be drawn.")>
                                      Optional onlyRegulations As Boolean = True) As ChromesomeDrawingModel

            If onlyRegulations Then
                data = (From footprint As PredictedRegulationFootprint
                        In data.AsParallel
                        Where Not String.IsNullOrEmpty(footprint.Regulator)'s.IsNullOrEmpty
                        Select footprint).ToArray
            End If

            model = __addMotifSites(model, data:=(From footprint As PredictedRegulationFootprint
                                                  In data.AsParallel
                                                  Select New KeyValuePair(Of String(), PredictedRegulationFootprint)({footprint.Regulator}, footprint)).ToArray)
            Return model
        End Function

        ''' <summary>
        ''' 将TSS位点以Motif位点的形式添加到绘图模型之上
        ''' </summary>
        ''' <param name="Model"></param>
        ''' <param name="Sites"></param>
        ''' <returns></returns>
        <ExportAPI("Add.TSSs")>
        Public Function AddTSSs(Model As ChromesomeDrawingModel, <Parameter("TSSs.Sites")> Sites As IEnumerable(Of Transcript)) As ChromesomeDrawingModel
            Dim TSSs = (From obj As Transcript In Sites.AsParallel
                        Select New DrawingModels.TSSs With {
                            .Left = obj.TSSs,
                            .Comments = obj.Position,
                            .Right = obj.TSSs + (CInt(obj.MappingLocation.Strand) * 10),
                            .Strand = obj.MappingLocation.Strand,
                            .SiteName = obj.TSS_ID,
                            .Synonym = obj.Synonym}).ToArray
            Model.TSSs = TSSs
            Return Model
        End Function

        <ExportAPI("Read.Csv.TSSs")>
        Public Function LoadTSSs(path As String) As Transcript()
            Return path.LoadCsv(Of Transcript)(False).ToArray
        End Function

        <ExportAPI("Add.Motif_Sites")>
        Public Function AddMotifSites(model As ChromesomeDrawingModel,
                                      data As Generic.IEnumerable(Of SMRUCC.genomics.ComponentModel.Loci.Loci)) As ChromesomeDrawingModel
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
        Public Function AddLociSites(model As ChromesomeDrawingModel,
                                     data As Generic.IEnumerable(Of SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentObject)) As ChromesomeDrawingModel
            Dim Locis = (From site As SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentObject
                         In data
                         Select New DrawingModels.Loci With {
                             .SiteName = site.Title,
                             .SequenceData = site.SequenceData,
                             .Right = site.Right,
                             .Left = site.Left,
                             .Color = Color.Black}).ToArray
            model.Loci = Locis
            Return model
        End Function

        Private Function __addMotifSites(Of T As VirtualFootprints)(
                                            model As ChromesomeDrawingModel,
                                            data As IEnumerable(Of KeyValuePair(Of String(), T))) As ChromesomeDrawingModel

            Dim ColorProfiles As New ColorProfiles((From item In data Select VirtualFootprintAPI.FamilyFromId(item.Value) Distinct).ToArray, DefaultColor:=Color.Black)
            Dim LQuery = (From siteEntry As KeyValuePair(Of String(), T) In data.AsParallel
                          Let footprint = siteEntry.Value
                          Let sf As String = VirtualFootprintAPI.FamilyFromId(footprint)
                          Select New MotifSite With {
                              .MotifName = sf,
                              .Regulators = If(siteEntry.Key.IsNullOrEmpty, New String() {}, siteEntry.Key),
                              .Color = ColorProfiles(sf),
                              .Left = footprint.Starts, .Right = footprint.Ends,
                              .Strand = footprint.Strand.First,
                              .Comments = If(siteEntry.Key.IsNullOrEmpty, "", String.Join(", ", siteEntry.Key)),
                              .SiteName = footprint.MotifId}).ToArray
            Call model.MotifSiteColorProfile.InvokeSet(ColorProfiles.ColorProfiles)
            model.MotifSites = LQuery
            Return model
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="data">可以使用<see cref="GenomeMotifFootPrints.GroupMotifs"></see>方法来合并一些重复的motif数据</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("add.motif_sites",
                   Info:="only_regulations: if TRUE, then only the motif site which has the regulator will be drawing on the map, else all of the motif site will be drawing on the maps.")>
        Public Function AddMotifSites(model As ChromesomeDrawingModel,
                                      <Parameter("Data.VirtualFootprint")>
                                      data As IEnumerable(Of VirtualFootprints)) As ChromesomeDrawingModel

            Dim DataSource = (From footprint As VirtualFootprints
                              In data.AsParallel
                              Select New KeyValuePair(Of String(), VirtualFootprints)(Nothing, footprint)).ToArray
            Return __addMotifSites(Of VirtualFootprints)(model, DataSource)
        End Function

        <ExportAPI("Device.Invoke_Drawing")>
        Public Function InvokeDrawing(<Parameter("Gr.Device")> Device As DrawingDevice,
                                      <Parameter("Chr.Gr.Model", "The drawing object which was represents the bacteria genome chromosomes.")>
                                      Model As ChromesomeDrawingModel) As KeyValuePair(Of Imaging.ImageFormat, Bitmap())
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
        Public Function ApplyCogColorProfile(Model As ChromesomeDrawingModel,
                                             <Parameter("COG.Myva")>
                                             MyvaCOG As IEnumerable(Of RpsBLAST.MyvaCOG)) As ChromesomeDrawingModel
            Return ApplyingCOGCategoryColor(MyvaCOG.ToArray, Model)
        End Function

        ''' <summary>
        ''' 使用这个函数进行绘图设备的配置参数的读取操作
        ''' </summary>
        ''' <param name="file"></param>  
        ''' <returns></returns>
        ''' 
        <ExportAPI("Read.TXT.Drawing_Config")>
        Public Function LoadConfig(File As String) As Configurations
            Return File.LoadConfiguration(Of Configurations)(AddressOf GetDefaultConfiguration)
        End Function

        <ExportAPI("Device.DefaultConfiguration")>
        Public Function GetDefaultConfiguration(<Parameter("Path.Save")> SaveFile As String) As Configurations
            Dim df = Configurations.DefaultValue
            Call df.ToConfigDoc.SaveTo(SaveFile)
            Return df
        End Function

        ''' <summary>
        ''' 请注意，在宽度上面是4倍的Margin
        ''' </summary>
        ''' <param name="Config"></param>
        ''' <returns></returns>
        <ExportAPI("Device.From.Configuration")>
        Public Function CreateDevice(Config As Configurations) As DrawingDevice
            Return ConfigurationCommon.FromConfig(Config)
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
        ''' Image formats can be one of the value: jpg,bmp,emf,exif,gif,png,wmf,tiff
        ''' </summary>
        ''' <param name="res"><see cref="DrawingDevice.InvokeDrawing(ChromesomeDrawingModel)"/>函数所生成的绘图图形输出资源数据</param>
        ''' <param name="Export">将要进行数据保存的文件夹</param>
        ''' <param name="Format">Value variant in jpg,bmp,emf,exif,gif,png,wmf,tiff</param>
        ''' <returns></returns>
        <ExportAPI("Resource.Save")>
        Public Function SaveImage(<Parameter("Resource")> res As KeyValuePair(Of Imaging.ImageFormat, Bitmap()),
                                  <Parameter("Dir.Export")> Export As String,
                                  <Parameter("Image.Format", "Value variant in jpg,bmp,emf,exif,gif,png,wmf,tiff")>
                                  Optional Format As String = "") As Integer
            Dim i As Integer = 0
            Dim imageFormat As Imaging.ImageFormat =
                If(String.IsNullOrEmpty(Format), Nothing, GetSaveImageFormat(Format))

            Call FileIO.FileSystem.CreateDirectory(Export)

            If imageFormat Is Nothing Then
                imageFormat = res.Key  ' 默认使用配置文件之中所设定的格式
            End If

            For Each Bitmap As Bitmap In res.Value
                Call Bitmap.Save($"{Export}/ChromosomeMap_Drawing_data.resources__{i.MoveNext}.bmp", imageFormat)
            Next
            Return i
        End Function

        <ExportAPI("Export.MapInfo")>
        Public Function ExportColorInformation(device As DrawingDevice, model As ChromesomeDrawingModel) As Image
            Return device.ExportColorProfiles(model)
        End Function

        <ExportAPI("DrawingModel.From.PTT")>
        Public Function FromPttDir(<Parameter("DIR.PTT")> PTT_DIR As String, conf As Configurations) As ChromesomeDrawingModel
            Dim PTTFile As String = FileIO.FileSystem.GetFiles(PTT_DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.ptt").First
#Region ""
            Dim GeneObjects = (From Gene As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief
                               In SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.Load(PTTFile).GeneObjects
                               Select New SegmentObject With {
                                   .Color = New SolidBrush(Color.Black),
                                   .Product = Gene.Product,
                                   .LocusTag = Gene.Synonym,
                                   .CommonName = Gene.Gene,
                                   .Left = Global.System.Math.Min(Gene.Location.Left, Gene.Location.Right),
                                   .Right = Global.System.Math.Max(Gene.Location.Right, Gene.Location.Left),
                                   .Direction = Gene.Location.Strand}).ToList
#End Region
            PTTFile = FileIO.FileSystem.GetFiles(PTT_DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.rnt").First

            Dim configurations As Conf = conf.ToConfigurationModel
            Dim SetRNAColor As New __setRNAColorInvoke(configurations)
            Dim Rna = (From GeneObject In PTT.Load(PTTFile).GeneObjects
                       Select New SegmentObject With {
                           .Color = SetRNAColor.__setColorBrush(GeneObject.Product),
                           .Product = GeneObject.Product,
                           .LocusTag = GeneObject.Synonym,
                           .CommonName = GeneObject.Gene,
                           .Left = Global.System.Math.Min(GeneObject.Location.Left, GeneObject.Location.Right),
                           .Right = Global.System.Math.Max(GeneObject.Location.Right, GeneObject.Location.Left),
                           .Direction = GeneObject.Location.Strand
                           }).ToArray
            Call GeneObjects.AddRange(Rna)

            Dim Model As ChromesomeDrawingModel = Rpt.Load(Of ChromesomeDrawingModel)(
                FileIO.FileSystem.GetFiles(PTT_DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.rpt").First)
            Model.GeneObjects = (From Gene In GeneObjects
                                 Select Gene
                                 Order By Gene.Left Ascending).ToArray
            Model.DrawingConfigurations = configurations
            Model.MutationDatas = New MultationPointData() {}

            Return Model
        End Function

        <ExportAPI("DrawingModel.From.PTT", Info:="Creates a basically simple drawing model object from the PTT file data.")>
        Public Function FromPTT(PTT As PTT, conf As Configurations) As ChromesomeDrawingModel
            Dim Model As ChromesomeDrawingModel = FromPttElements(PTT, conf, PTT.Size)
            Model.CDSCount = PTT.NumOfProducts
            Return Model
        End Function

        ''' <summary>
        ''' 通常使用这个方法从PTT构件之中生成部分基因组的绘制模型数据
        ''' </summary>
        ''' <param name="PTTGeneObjects"></param>
        ''' <param name="conf"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("DrawingModel.From.PTT.Elements")>
        Public Function FromPttElements(<Parameter("PTT.Genes")>
                                        PTTGeneObjects As IEnumerable(Of GeneBrief),
                                        conf As Configurations,
                                        <Parameter("Ranges", "The nt length of the gene objects contains in the region.")>
                                        RangeLength As Integer) As ChromesomeDrawingModel
#Region ""
            Dim GeneObjects = (From GeneObject As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief
                               In PTTGeneObjects
                               Select New SegmentObject With {
                                   .Color = New SolidBrush(Color.Black),
                                   .Product = GeneObject.Product,
                                   .LocusTag = GeneObject.Synonym,
                                   .CommonName = GeneObject.Gene,
                                   .Left = Global.System.Math.Min(GeneObject.Location.Left, GeneObject.Location.Right),
                                   .Right = Global.System.Math.Max(GeneObject.Location.Right, GeneObject.Location.Left),
                                   .Direction = GeneObject.Location.Strand}).ToList
#End Region
            Dim Model = New ChromesomeDrawingModel With {
                .CDSCount = GeneObjects.Count,
                .Size = RangeLength,
                .NumberOfGenes = GeneObjects.Count,
                .ProteinCount = GeneObjects.Count,
                .PseudoCDSCount = 0,
                .PseudoGeneCount = 0,
                .RNACount = 0
            }
            Model.GeneObjects = (From Gene As SegmentObject
                                 In GeneObjects
                                 Select Gene
                                 Order By Gene.Left Ascending).ToArray
            Model.DrawingConfigurations = conf.ToConfigurationModel
            Model.MutationDatas = New MultationPointData() {}

            Return Model
        End Function

        Private Class __setRNAColorInvoke

            Dim tRnaColor, DefaultRNAColor, rRNAColor As Color
            Dim _tRnaColor, _DefaultRNAColor, _rRNAColor As Brush

            Sub New(configurations As Conf)
                Me.tRnaColor = configurations.tRNAColor
                Me.DefaultRNAColor = configurations.DefaultRNAColor
                Me.rRNAColor = configurations.ribosomalRNAColor
                Me._rRNAColor = New SolidBrush(rRNAColor)
                Me._DefaultRNAColor = New SolidBrush(DefaultRNAColor)
                Me._tRnaColor = New SolidBrush(tRnaColor)
            End Sub

            Public Function __setColor(Product As String) As System.Drawing.Color
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

            Public Function __setColorBrush(Product As String) As System.Drawing.Brush
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
                                      BacterialGenomics As PTTDbLoader, conf As Configurations) As ChromesomeDrawingModel
            Dim GeneObjects = (From GeneObject As GeneBrief
                               In BacterialGenomics.Values.AsParallel
                               Select New SegmentObject With {
                                   .Color = New SolidBrush(Color.Black),
                                   .Product = GeneObject.Product,
                                   .LocusTag = GeneObject.Synonym,
                                   .CommonName = GeneObject.Gene,
                                   .Left = Global.System.Math.Min(GeneObject.Location.Left, GeneObject.Location.Right),
                                   .Right = Global.System.Math.Max(GeneObject.Location.Right, GeneObject.Location.Left),
                                   .Direction = GeneObject.Location.Strand
                                   }).ToList

            Dim configurations = conf.ToConfigurationModel
            Dim Model As ChromesomeDrawingModel = BacterialGenomics.RptGenomeBrief.CopyTo(Of ChromesomeDrawingModel)()
            Model.GeneObjects = (From GeneObject As SegmentObject
                                     In GeneObjects
                                 Select GeneObject
                                 Order By GeneObject.Left Ascending).ToArray
            Model.DrawingConfigurations = configurations
            Model.MutationDatas = New MultationPointData() {}

            Return Model
        End Function

        Private Function __getRandomColor() As Color
            Dim r = Rnd() * 255
            Dim g = Rnd() * 255
            Dim b = Rnd() * 255
            Return Color.FromArgb(r, g, b)
        End Function

        Public Function GetTestMutationData() As MultationPointData()
            Return New DrawingModels.MultationPointData() {
                New DrawingModels.MultationPointData With {
                    .Direction = 1,
                    .Left = 100,
                    .Right = 200,
                    .MutationType = MultationPointData.MutationTypes.DeleteMutation},
                New DrawingModels.MultationPointData With {
                    .Direction = -1,
                    .Left = 3000,
                    .Right = 4000,
                    .MutationType = MultationPointData.MutationTypes.IntegrationMutant
                }
            }
        End Function
    End Module
End Namespace
