#Region "Microsoft.VisualBasic::799d2a29501e22cd4324b9a08d1abefd, engine\IO\GCTabular\CsvTabularData\Storage\XmlModel.vb"

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

    '     Structure ResourceCollection
    ' 
    '         Properties: Resources
    ' 
    '         Function: ToDictionary, TryGetValue
    ' 
    '     Class CellSystemXmlModel
    ' 
    '         Properties: FilePath, OperonCounts, ResourceCollection, ResourceMapper
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: CreateObject, get_ParentDirectory, Internal_getA_ResourceLinks, LoadXml, Save
    '                   SaveOrCopy
    ' 
    '         Sub: Copy, Internal_MapA_ResourceLinks, WriteREADME
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.LDM

Namespace FileStream.XmlFormat

    Public Structure ResourceCollection

        ''' <summary>
        ''' 每一个节点都是属于不同的类型分类的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Resources As ResourceNode()

        Public Function ToDictionary() As Dictionary(Of String, ResourceNode)
            Return Resources.ToDictionary(Function(item) item.TYPE_ID)
        End Function

        Public Function TryGetValue(resourceId As String) As HrefLink
            Dim hreflink = (From node In Resources.AsParallel
                            Let q = (From link In node.InternalHrefLinks
                                     Where String.Equals(link.ResourceId, resourceId)
                                     Select link).FirstOrDefault
                            Where Not q Is Nothing
                            Select node,
                              link = q).FirstOrDefault
            If hreflink Is Nothing Then
                Return Nothing
            Else
                Return hreflink.link.set_Category(hreflink.node)
            End If
        End Function
    End Structure

    ''' <summary>
    ''' 本计算模型中的所构建的细胞中的基本系统：代谢组和转录组，请注意，对于本对象的属性中的路径对象，当在编译器阶段的时候为一个绝对路径，但是当执行了保存动作之后，都将变为相对路径
    ''' </summary>
    ''' <remarks></remarks>
    <XmlRoot(ElementName:="GCModeller.CSV_TabularEntry",
        IsNullable:=True,
        Namespace:="http://code.google.com/p/genome-in-code/virtualcell_model/GCMarkupLanguage/csv_tabular")>
    Public Class CellSystemXmlModel : Inherits ModelBaseType
        Implements IFileReference

        ''' <summary>
        ''' XML模型文件之中的资源连接数据都是存储在这个属性之中的，当加载的时候，就会通过本属性来讲值赋值给其他的属性
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("ResourceCollection", Namespace:="http://code.google.com/p/genome-in-code/gcml/resource_collection")>
        Public Property ResourceCollection As ResourceCollection
            Get
                If _InternalResourceCollection.IsNullOrEmpty Then
                    _InternalResourceCollection = Internal_getA_ResourceLinks().ToDictionary
                End If
                Return New ResourceCollection With {.Resources = _InternalResourceCollection.Values.ToArray}
            End Get
            Set(value As ResourceCollection)
                Call Internal_MapA_ResourceLinks(value)
            End Set
        End Property

        <XmlIgnore> Public Property ResourceMapper As ResourceMapper
        <XmlAttribute("DoorOperon.Counts")> Public Property OperonCounts As Integer

        Dim _InternalResourceCollection As Dictionary(Of ResourceNode)

        Default Public ReadOnly Property get_ResourceCollectionItem(type As String) As ResourceNode
            Get
                Return _InternalResourceCollection(type)
            End Get
        End Property

        Public Property FilePath As String Implements IFileReference.FilePath

        Sub New()
            ResourceMapper = New ResourceMapper
        End Sub

        Private Function Internal_getA_ResourceLinks() As FileStream.XmlFormat.ResourceNode()
            Dim Bac_AnnotationDat As ResourceNode = New ResourceNode With {.TYPE_ID = ResourceNode.TYPE_ID_BACTERIA_ANNOTATION_DATA_CHUNK,
                                                                           .Comment = NodeDocument.DataAnnotationComment,
                                                                           .ResourceCategory = DIR_ANNOTIATIONS,
                                                                           .InternalHrefLinks = {
                                                                               ResourceMapper.KEGGCompiledXmlModel,
                                                                               ResourceMapper.CrossTalkAnnotations,
                                                                               ResourceMapper.ChipData,
                                                                               ResourceMapper.DoorOperons,
                                                                               ResourceMapper.EffectorMapping,
                                                                               ResourceMapper.EnzymeMapping,
                                                                               ResourceMapper.GenomeAnnotiation,
                                                                               ResourceMapper.MisT2,
                                                                               ResourceMapper.STrPModel}}
            Dim ProgrammingGenome As ResourceNode = New ResourceNode With {.TYPE_ID = ResourceNode.TYPE_ID_BACTERIA_GENOME_PROGRAMMING_INFORMATION,
                                                                           .Comment = NodeDocument.ProgrammingDataComment,
                                                                           .ResourceCategory = DIR_GENOME_PROGRAMMING_INFORMATION,
                                                                           .InternalHrefLinks = {
                                                                               ResourceMapper.RibosomeAssembly,
                                                                               ResourceMapper.RNAPolymerase,
                                                                               ResourceMapper.ExpressionKinetics,
                                                                               ResourceMapper.Motifs,
                                                                               ResourceMapper.Regulators,
                                                                               ResourceMapper.Transcript,
                                                                               ResourceMapper.TranscriptionModel}}
            Dim CellPhenotypeData As ResourceNode = New ResourceNode With {.TYPE_ID = ResourceNode.TYPE_ID_BACTERIA_CELL_PHENOTYPE_DESCRIPTION,
                                                                           .Comment = NodeDocument.PhenotypeDataComment,
                                                                           .ResourceCategory = DIR_CELL_PHENOTYPE,
                                                                           .InternalHrefLinks = {
                                                                               ResourceMapper.Enzymes,
                                                                               ResourceMapper.MetabolismModel,
                                                                               ResourceMapper.MetabolitesModel,
                                                                               ResourceMapper.Pathway,
                                                                               ResourceMapper.ProteinAssembly,
                                                                               ResourceMapper.Proteins,
                                                                               ResourceMapper.TransmembraneTransportation}}
            Dim SignalTransductin As ResourceNode = New ResourceNode With {.TYPE_ID = ResourceNode.TYPE_ID_BACTERIA_SIGNAL_TRANSDUCTION_NETWORK,
                                                                           .ResourceCategory = DIR_SIGNAL_TRANSDUCTION_NETWORK,
                                                                           .Comment = NodeDocument.SignalTransductionNetwork,
                                                                           .InternalHrefLinks = {
                                                                               ResourceMapper.CheBMethylesterase,
                                                                               ResourceMapper.CheBPhosphate,
                                                                               ResourceMapper.ChemotaxisProfile,
                                                                               ResourceMapper.CheRMethyltransferase,
                                                                               ResourceMapper.OCSSensing,
                                                                               ResourceMapper.CrossTalk,
                                                                               ResourceMapper.HkAutoPhosphorus}}
            Dim EnvironmentVariab As ResourceNode = New ResourceNode With {.TYPE_ID = ResourceNode.TYPE_ID_BACTERIA_EXPERINMENT_ENVIRONMENT,
                                                                           .ResourceCategory = DIR_EXPERIMENTS_DATA,
                                                                           .Comment = NodeDocument.EnvironmentnComments,
                                                                           .InternalHrefLinks = {
                                                                               ResourceMapper.CultivationMediums,
                                                                               ResourceMapper.ObjectiveFunctionModel,
                                                                               ResourceMapper.ConstraintMetabolites,
                                                                               ResourceMapper.SystemVariables}}

            Return {Bac_AnnotationDat, ProgrammingGenome, CellPhenotypeData, SignalTransductin, EnvironmentVariab}.ToArray
        End Function

        Private Sub Internal_MapA_ResourceLinks(data As ResourceCollection)
            ResourceMapper = New ResourceMapper(data)
            _InternalResourceCollection = data.Resources.ToDictionary
        End Sub

        Public Overloads Shared Widening Operator CType(sPath As String) As FileStream.XmlFormat.CellSystemXmlModel
            Dim CellSystem = sPath.LoadXml(Of FileStream.XmlFormat.CellSystemXmlModel)()
            Return CellSystem
        End Operator

        ''' <summary>
        ''' 请使用本方法来加载资源数据
        ''' </summary>
        ''' <param name="sPath"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadXml(sPath As String) As FileStream.XmlFormat.CellSystemXmlModel
            Return CType(sPath, FileStream.XmlFormat.CellSystemXmlModel)
        End Function

        Public Overloads Shared Function CreateObject() As CellSystemXmlModel
            Dim Xml As New CellSystemXmlModel
            Xml._InternalResourceCollection = Xml.Internal_getA_ResourceLinks.ToDictionary
            Return Xml
        End Function

        Public Function get_ParentDirectory() As String
            Return FileIO.FileSystem.GetParentPath(Nothing)
        End Function

        Public Const DIR_ANNOTIATIONS As String = "./DATA_ANNOTATIONS/"
        Public Const DIR_DATAMODELS As String = "./DATA_MODELS/"
        Public Const DIR_SIGNAL_TRANSDUCTION_NETWORK As String = DIR_DATAMODELS & "/SIGNAL_TRANSDUCTION_NETWORK/"
        Public Const DIR_GENOME_PROGRAMMING_INFORMATION As String = DIR_DATAMODELS & "/GENOME_PROGRAMMING_INFORMATION/"
        Public Const DIR_CELL_PHENOTYPE As String = DIR_DATAMODELS & "/CELL_PHENOTYPE_DATA/"
        Public Const DIR_EXPERIMENTS_DATA As String = "./EXPERIMENT_DATA/"

        Private Overloads Sub Copy(ByRef href As HrefLink, DIR As String)
            Dim Path As String = href.GetFullPath(Me.get_ParentDirectory)

            If Not FileIO.FileSystem.FileExists(Path) Then
                Call Console.WriteLine("[ERROR] Object data file ""{0}"" is not exists on your filesystem!", Path)
                Return
            End If

            Path = String.Format("{0}/{1}", DIR, FileIO.FileSystem.GetName(Path))

            If FileIO.FileSystem.FileExists(Path) Then
                Call Console.WriteLine("Object data in file ""{0}"" was overrided!", FileIO.FileSystem.GetFileInfo(Path).FullName)
                Call FileIO.FileSystem.DeleteFile(Path)
            End If
            Call FileIO.FileSystem.CopyFile(href.Value, Path) : href.Value = Path
        End Sub

        ''' <summary>
        ''' 这个方法仅仅是保存当前的这个xml文件对象
        ''' </summary>
        ''' <param name="FilePath"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Save(Optional FilePath As String = "") As Boolean
            If Me.ModelProperty Is Nothing Then
                Me.ModelProperty = New Framework.Kernel_Driver.LDM.Property
            End If

            Call WriteREADME()

            Return Me.GetXml.SaveTo(FilePath)
        End Function

        ''' <summary>
        ''' 由于本文件仅仅是一个资源的连接文件，故而在保存数据的时候，是不知道所要进行保存的数据的具体格式的，
        ''' 故而在本方法之中，仅仅是根据<see cref="CellSystemXmlModel.ResourceMapper"></see>之中的连接指针
        ''' 将所指向的目标资源对象复制到<paramref name="FilePath"></paramref>的文件目录之中。参数
        ''' <paramref name="FilePath"></paramref>仅仅适用于表示主XML连接文件的文件路径，故而当该参数的父文件夹
        ''' 与当前的模型文件所处的父文件夹一致的时候，仅仅会保存XML主文件，当不同的时候，会进行资源文件的复制操作
        ''' </summary>
        ''' <param name="FilePath"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function SaveOrCopy(FilePath As String) As Boolean
            Dim SavedDir As String = FileIO.FileSystem.GetParentPath(FilePath)
            Dim Current As String = Me.get_ParentDirectory

            If String.Equals(SavedDir, Current) Then
                '假若是相同的父文件夹，则仅仅是保存主XML文件，即当前的这个对象
                GoTo WRITE_RESOURCE
            Else
                '以相对路径保存表格文件
                Call FileIO.FileSystem.CreateDirectory(SavedDir & DIR_ANNOTIATIONS)
                Call FileIO.FileSystem.CreateDirectory(SavedDir & DIR_DATAMODELS)

                My.Computer.FileSystem.CurrentDirectory = SavedDir
            End If


            On Error Resume Next

            Dim ResourceEntry As ResourceNode = Me.get_ResourceCollectionItem(ResourceNode.TYPE_ID_BACTERIA_ANNOTATION_DATA_CHUNK)

            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.ChipData)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.CheBMethylesterase)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.CheBPhosphate)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.ChemotaxisProfile)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.CheRMethyltransferase)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.ConstraintMetabolites)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.CrossTalk)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.CultivationMediums)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.DoorOperons)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.EffectorMapping)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.EnzymeMapping)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.Enzymes)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.ExpressionKinetics)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.GenomeAnnotiation)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.HkAutoPhosphorus)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.MetabolismModel)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.MetabolitesModel)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.MisT2)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.Motifs)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.ObjectiveFunctionModel)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.OCSSensing)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.Pathway)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.ProteinAssembly)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.Proteins)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.Regulators)
            Call ResourceEntry.CopyFile(SavedDir, Current, Me.ResourceMapper.CrossTalkAnnotations)

WRITE_RESOURCE:
            If Me.ModelProperty Is Nothing Then
                Me.ModelProperty = New Framework.Kernel_Driver.LDM.Property
            End If

            Call WriteREADME()

            Return Me.GetXml.SaveTo(FilePath)
        End Function

        Protected Friend Sub WriteREADME()
            Dim README As String = String.Format(FileStream.README.README.Value, MyBase.ModelProperty.Title, MyBase.ModelProperty.SpecieId)

            Call Console.WriteLine(README)
            Call FileIO.FileSystem.WriteAllText("./README.txt", README, append:=False)
        End Sub
    End Class
End Namespace
