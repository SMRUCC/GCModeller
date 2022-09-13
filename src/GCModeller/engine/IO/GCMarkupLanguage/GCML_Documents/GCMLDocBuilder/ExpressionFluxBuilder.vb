#Region "Microsoft.VisualBasic::ae15b862fdca5d222ba6653875bab882, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\GCMLDocBuilder\ExpressionFluxBuilder.vb"

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

    '   Total Lines: 126
    '    Code Lines: 70
    ' Comment Lines: 39
    '   Blank Lines: 17
    '     File Size: 7.27 KB


    '     Class ExpressionFluxBuilder
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateGenes, CreateTranscripts, CreateTransUnits, GetAllUnmodifiedProduct, Invoke
    '                   Link, TakesMetabolites
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports Microsoft.VisualBasic.Extensions
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME

Namespace Builder

    ''' <summary>
    ''' 表达流对象构建器，重建出目标模型的基因组、转录组
    ''' </summary>
    ''' <remarks>生成模型文件中的基因、转录单元和转录组分这三张表</remarks>
    Public Class ExpressionFluxBuilder : Inherits IBuilder

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <param name="Model">在模型对象之中的代谢组必须是已经构建好了的</param>
        ''' <remarks></remarks>
        Sub New(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As BacterialModel)
            MyBase.New(MetaCyc, Model)
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            printf("Start to compile the gene object in the metacyc.")
            Model.BacteriaGenome.Genes = CreateGenes(MetaCyc)
            printf("Start to compile the transcript object in the metacyc.")
            Model.BacteriaGenome.Transcripts = CreateTranscripts(MetaCyc, Model)
            printf("Start to compile transunit object in the metacyc.")
            Model.BacteriaGenome.TransUnits = CreateTransUnits(MetaCyc, Model)

            Return Model
        End Function

        ''' <summary>
        ''' 根据MetaCyc数据库模型生成转录单元对象列表
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function CreateTransUnits(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As BacterialModel) _
            As List(Of GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit)

            Dim TransUnits As GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit() = (From TransUnit As MetaCyc.File.DataFiles.Slots.TransUnit
                                                           In MetaCyc.GetTransUnits.AsParallel
                                                                                              Select GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit.CreateObject(TransUnit)).ToArray

            printf("[INFO] function generate_transcript_units() create %s tu models.", TransUnits.Count)
            printf("[INFO] link the transcript unit object with its gene cluster...")

            Dim LQuery = (From TU As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit
                          In TransUnits.AsParallel
                          Let TU2 As GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit = TU.Link(Model.BacteriaGenome.Genes)
                          Select TU2
                          Order By TU2.Identifier Ascending).AsList
            Return LQuery
        End Function

        Private Shared Function CreateGenes(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder) As GeneObject()
            Dim LQuery = From e As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Gene
                         In MetaCyc.GetGenes.AsParallel
                         Let G = GeneObject.CastTo(e)
                         Select G Order By G.Identifier Ascending   '
            Return LQuery.ToArray
        End Function

        ''' <summary>
        ''' 创建RNA分子对象，然后添加进入代谢组对象之中
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <param name="Model"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function CreateTranscripts(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As BacterialModel) As GCML_Documents.XmlElements.Bacterial_GENOME.Transcript()
            Dim List As List(Of GCML_Documents.XmlElements.Bacterial_GENOME.Transcript) = New List(Of GCML_Documents.XmlElements.Bacterial_GENOME.Transcript)
            Dim UnmodifiedProteins = GetAllUnmodifiedProduct(MetaCyc, Model)
            Dim LQuery = (From Gene In Model.BacteriaGenome.Genes Select Link(Gene, GCML_Documents.XmlElements.Bacterial_GENOME.Transcript.CreateObject(MetaCyc, Gene, Model), List)).ToArray   '

            '将构造出来的mRNA分子添加进入代谢组里面并添加句柄
            Dim n As Long = Model.Metabolism.Metabolites.Count
            Dim m As Long = (From tr In List Select tr.GenerateVector(MetaCyc)).ToArray.Sum
            Model.Metabolism.Metabolites.AddRange((From transcript In List Select transcript.CreateMetabolite).ToArray)

            Return List.ToArray
        End Function

        ''' <summary>
        ''' 将一个基因对象与相应的转录产物想联系起来
        ''' </summary>
        ''' <param name="Gene"></param>
        ''' <param name="Transcripts"></param>
        ''' <param name="List"></param>
        ''' <returns></returns>
        ''' <remarks>!!!请注意这里！！！</remarks>
        Private Shared Function Link(Gene As GeneObject, Transcripts As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript(), List As List(Of GCML_Documents.XmlElements.Bacterial_GENOME.Transcript)) As Integer
            Dim Products As List(Of String) = New List(Of String)
            If Not Transcripts.IsNullOrEmpty Then
                Dim Transcript = Transcripts.First
                List.Add(Transcript)
            End If

            Return 0
        End Function

        ''' <summary>
        ''' 获取所有未经过化学修饰的蛋白质多肽链单体对象
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <param name="Model"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetAllUnmodifiedProduct(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As BacterialModel) As GCML_Documents.XmlElements.Metabolism.Metabolite()
            Dim LQuery = From Protein As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Protein
                         In MetaCyc.GetProteins.AsParallel
                         Where Protein.IsPolypeptide AndAlso Not Protein.IsModifiedProtein AndAlso Not String.IsNullOrEmpty(Protein.Gene)
                         Select Protein.Identifier  '筛选条件为目标蛋白质对象为多肽链、不是化学修饰形式，并且基因号不为空
            Dim Proteins = LQuery.ToArray
            Return TakesMetabolites(UniqueIDCollection:=Proteins, Model:=Model)
        End Function

        Private Shared Function TakesMetabolites(UniqueIDCollection As Generic.IEnumerable(Of String), Model As BacterialModel) As GCML_Documents.XmlElements.Metabolism.Metabolite()
            Dim LQuery = From UniqueID As String In UniqueIDCollection Select Model.Metabolism.Metabolites.GetItem(UniqueID) '
            Return LQuery.ToArray
        End Function
    End Class
End Namespace
