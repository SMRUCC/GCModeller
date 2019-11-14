#Region "Microsoft.VisualBasic::077411a5ab237d5ea29c178fde600554, engine\IO\GCTabular\CsvTabularData\XmlLoader.vb"

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

    '     Class XmlresxLoader
    ' 
    '         Properties: CellSystemModel, CheBMethylesterase, CheBPhosphate, ChemotaxisSensing, CheRMethyltransferase
    '                     ConstraintMetabolites, CrossTalk, CrossTalksAnnotation, CultivationMediums, DoorOperon
    '                     EffectorMapping, EnzymeMapping, Enzymes, ExpressionKinetics, GenomeAnnotiation
    '                     HkAutoPhosphorus, KEGG_Pathways, MetabolismModel, MetabolitesModel, MisT2
    '                     ModelParentDIR, Motifs, ObjectiveFunctionModel, OCSSensing, Pathway
    '                     ProteinAssembly, Proteins, Regulators, RibosomeAssembly, RNAPolymerase
    '                     StringInteractions, STrPModel, SystemVariables, TranscriptionModel, Transcripts
    '                     TransmembraneTransportation
    ' 
    '         Constructor: (+3 Overloads) Sub New
    ' 
    '         Function: CreateObject, Decryption, Encryption, LoadCsvData, LoadData
    '                   SaveCsvData, SaveData, SaveTo, ToString
    ' 
    '         Sub: Copy, InternalLoadResourceData, SetExportDirectory
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.MiST2
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.SabiorkKineticLaws.TabularDump
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Components
Imports SMRUCC.genomics.Model.Network.STRING.Models
Imports SMRUCC.genomics.Model.Network.STRING.TCS

Namespace FileStream.IO

    ''' <summary>
    ''' The GCModeller cellular network xml model component resource entry loader.(GCModeller虚拟细胞计算模型的资源加载器) 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class XmlresxLoader

        ''' <summary>
        ''' 资源管理器的内部入口点的挂载对象
        ''' </summary>
        ''' <remarks></remarks>
        Dim _InternalCellSystemResourceManager As FileStream.XmlFormat.CellSystemXmlModel
        ''' <summary>
        ''' 资源数据的总入口
        ''' </summary>
        ''' <remarks></remarks>
        Dim _InternalModelParentDirRoot As String

        Public Property MetabolitesModel As Dictionary(Of FileStream.Metabolite)
        Public Property MetabolismModel As List(Of FileStream.MetabolismFlux)
        Public Property TranscriptionModel As List(Of FileStream.TranscriptUnit)
        Public Property ObjectiveFunctionModel As List(Of FileStream.ObjectiveFunction)
        Public Property GenomeAnnotiation As FileStream.GeneObject()
        Public Property Transcripts As List(Of FileStream.Transcript)
        Public Property Proteins As List(Of FileStream.Protein)
        Public Property Enzymes As List(Of EnzymeCatalystKineticLaw)
        Public Property SystemVariables As List(Of KeyValuePair)
        Public Property STrPModel As Network
        Public Property ConstraintMetabolites As List(Of ConstraintMetaboliteMap)
        Public Property ProteinAssembly As Dictionary(Of String, FileStream.ProteinAssembly)
        Public Property RibosomeAssembly As List(Of FileStream.ProteinAssembly)
        Public Property RNAPolymerase As List(Of FileStream.ProteinAssembly)

        Public Property CultivationMediums As List(Of I_SubstrateRefx)
        Public Property EffectorMapping As List(Of MetaCyc.Schema.EffectorMap)
        Public Property EnzymeMapping As List(Of Mapping.EnzymeGeneMap)
        Public Property Pathway As List(Of FileStream.Pathway)
        Public Property TransmembraneTransportation As List(Of FileStream.MetabolismFlux)
        Public Property ExpressionKinetics As List(Of FileStream.ExpressionKinetics)
        Public Property DoorOperon As DOOR.CsvModel.Operon()

        Public Property Motifs As List(Of FileStream.MotifSite)
        Public Property Regulators As List(Of FileStream.Regulator)

        Public Property MisT2 As MiST2
        Public Property StringInteractions As SimpleCsv.Network
        Public Property CrossTalksAnnotation As CrossTalks()

        ''' <summary>
        ''' 编译好的KEGG代谢网络模型
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property KEGG_Pathways As Archives.Xml.XmlModel

        ''' <summary>
        ''' [MCP][CH3] -> MCP + -CH3  Enzyme:[CheB][PI]
        ''' 
        ''' Protein L-glutamate O(5)-methyl ester + H(2)O = protein L-glutamate + methanol
        ''' C00132
        '''
        ''' METOH
        '''
        ''' </summary>
        ''' <remarks></remarks>
        Public Property CheBMethylesterase As FileStream.MetabolismFlux()
        ''' <summary>
        ''' MCP + -CH3 -> [MCP][CH3]   Enzyme:CheR
        ''' S-adenosyl-L-methionine
        ''' S-ADENOSYLMETHIONINE
        ''' C00019
        ''' 
        ''' S-ADENOSYLMETHIONINE                              ADENOSYL-HOMO-CYS
        ''' S-adenosyl-L-methionine + protein L-glutamate = S-adenosyl-L-homocysteine + protein L-glutamate methyl ester.
        ''' </summary>
        ''' <remarks></remarks>
        Public Property CheRMethyltransferase As FileStream.MetabolismFlux()
        ''' <summary>
        ''' CheB + [ChA][PI] -> [CheB][PI] + CheA
        ''' </summary>
        ''' <remarks></remarks>
        Public Property CheBPhosphate As FileStream.MetabolismFlux()
        ''' <summary>
        ''' [MCP][CH3] + Inducer &lt;--&gt; [MCP][CH3][Inducer]
        ''' </summary>
        ''' <remarks></remarks>
        Public Property ChemotaxisSensing As List(Of SensorInducers)
        ''' <summary>
        ''' CheAHK + ATP -> [CheAHK][PI] + ADP   Enzyme: [MCP][CH3][Inducer]
        ''' </summary>
        ''' <remarks></remarks>
        Public Property HkAutoPhosphorus As List(Of SensorInducers)
        ''' <summary>
        ''' [CheAHK][PI] + RR -> [RR][PI] + CheAHK
        ''' [CheAHK][PI] + OCS -> CheAHK + [OCS][PI]
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Property CrossTalk As FileStream.MetabolismFlux()
        Public Property OCSSensing As FileStream.MetabolismFlux()

        Public ReadOnly Property ModelParentDIR As String
            Get
                Return _InternalModelParentDirRoot
            End Get
        End Property

#Region "Object Constructors"

        ''' <summary>
        ''' Create a new and empty gcml csvx resource loader object.(创建一个全新的空的资源加载器Loader对象)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateObject() As XmlresxLoader
            Dim resLoader As XmlresxLoader = New XmlresxLoader With {
                ._InternalCellSystemResourceManager = FileStream.XmlFormat.CellSystemXmlModel.CreateObject,
                ._InternalModelParentDirRoot = My.Computer.FileSystem.SpecialDirectories.Temp
            }
            Return resLoader
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="CellSystemPath"><see cref=" FileStream.XmlFormat.CellSystemXmlModel">Cell system model xml</see> file path.</param>
        ''' <remarks></remarks>
        Sub New(CellSystemPath As String)
            Me._InternalCellSystemResourceManager = FileStream.XmlFormat.CellSystemXmlModel.LoadXml(CellSystemPath)
            Call InternalLoadResourceData()
        End Sub

        Sub New(CellSystem As FileStream.XmlFormat.CellSystemXmlModel)
            Me._InternalCellSystemResourceManager = CellSystem
            Call InternalLoadResourceData()
        End Sub

        Protected Sub New()
        End Sub
#End Region

        Public Overrides Function ToString() As String
            Return CellSystemModel.ToString
        End Function

        ''' <summary>
        ''' 模型本身+最后一行的MD5校验码
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Encryption() As String
            Dim XML As String = Me.GetXml
            Dim XMLMD5 As String = SecurityString.GetMd5Hash(XML)
            Dim EncryptedData = New SecurityString.SHA256("gcmodeller", "12345678")
            Dim ChunkBuffer As StringBuilder = New StringBuilder(1024 * 1024)
            Dim strTemp As String = EncryptedData.EncryptData(XML)
            For i As Integer = 0 To strTemp.Length - 1 Step 1024
                If strTemp.Length - i < 1024 Then
                    Call ChunkBuffer.AppendLine(Mid(strTemp, i))
                    Exit For
                End If
                Call ChunkBuffer.AppendLine(Mid(strTemp, i, 1024))
            Next
            Call ChunkBuffer.AppendLine(XMLMD5)

            Return ChunkBuffer.ToString
        End Function

        Public Shared Function Decryption(Data As String) As XmlresxLoader
            Dim DecryptedData As SecurityString.SHA256 = New SecurityString.SHA256("gcmodeller", "12345678")
            Dim Tokens = Strings.Split(Data, vbCrLf)
            Dim MD5 As String = Tokens.Last
            Dim sBuilder As StringBuilder = New StringBuilder(1024 * 1024)
            For i As Integer = 0 To Tokens.Count - 2
                Call sBuilder.Append(Tokens(i))
            Next

            Dim XML = DecryptedData.DecryptString(sBuilder.ToString)
            If SecurityString.VerifyMd5Hash(XML, MD5) Then
                Return XML.LoadFromXml(Of XmlresxLoader)()
            Else
                Throw New Exception("Model data was corrupted!")
            End If
        End Function

        ''' <summary>
        ''' 加载Xml文件之中的属性所指向的Excel资源文件之中的数据至网络模型之中
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub InternalLoadResourceData()
            If String.IsNullOrEmpty(Me._InternalCellSystemResourceManager.FilePath) Then
                Me._InternalModelParentDirRoot = My.Computer.FileSystem.SpecialDirectories.Temp & "\_________________temp_compiler/"
            Else
                Me._InternalModelParentDirRoot = FileIO.FileSystem.GetParentPath(_InternalCellSystemResourceManager.FilePath)
            End If

            Dim PreWork As String = My.Computer.FileSystem.CurrentDirectory

            My.Computer.FileSystem.CurrentDirectory = ModelParentDIR

            Call Console.WriteLine("Start to load data from model reference...." & vbCrLf)

            Dim Metabolites As FileStream.Metabolite() = Nothing

            Call Me.CellSystemModel.ResourceMapper.MetabolitesModel.LoadResource(Metabolites)
            Me.MetabolitesModel = Metabolites.ToDictionary

            Call Me.CellSystemModel.ResourceMapper.GenomeAnnotiation.LoadResource(Me.GenomeAnnotiation)
            Call Me.CellSystemModel.ResourceMapper.Transcript.LoadResource(Me.Transcripts)
            Call Me.CellSystemModel.ResourceMapper.TranscriptionModel.LoadResource(Me.TranscriptionModel)
            Call Me.CellSystemModel.ResourceMapper.MetabolismModel.LoadResource(Me.MetabolismModel)
            Call Me.CellSystemModel.ResourceMapper.Proteins.LoadResource(Me.Proteins)
            Call Me.CellSystemModel.ResourceMapper.Enzymes.LoadResource(Me.Enzymes)
            Call Me.CellSystemModel.ResourceMapper.ConstraintMetabolites.LoadResource(Me.ConstraintMetabolites)

            Dim PC As FileStream.ProteinAssembly() = Nothing
            Call Me.CellSystemModel.ResourceMapper.ProteinAssembly.LoadResource(PC)
            Me.ProteinAssembly = PC.ToDictionary(Function(x) x.ProteinComplexes)

            Call Me.CellSystemModel.ResourceMapper.CultivationMediums.LoadResource(Me.CultivationMediums)
            Call Me.CellSystemModel.ResourceMapper.ObjectiveFunctionModel.LoadResource(Me.ObjectiveFunctionModel)
            Call Me.CellSystemModel.ResourceMapper.EffectorMapping.LoadResource(Me.EffectorMapping)
            Call Me.CellSystemModel.ResourceMapper.EnzymeMapping.LoadResource(Me.EnzymeMapping)
            Call Me.CellSystemModel.ResourceMapper.Pathway.LoadResource(Me.Pathway)
            Call Me.CellSystemModel.ResourceMapper.TransmembraneTransportation.LoadResource(Me.TransmembraneTransportation)
            Call Me.CellSystemModel.ResourceMapper.RibosomeAssembly.LoadResource(Me.RibosomeAssembly)
            Call Me.CellSystemModel.ResourceMapper.RNAPolymerase.LoadResource(Me.RNAPolymerase)
            Call Me.CellSystemModel.ResourceMapper.ExpressionKinetics.LoadResource(Me.ExpressionKinetics)
            Call Me.CellSystemModel.ResourceMapper.DoorOperons.LoadResource(Me.DoorOperon)
            Call Me.CellSystemModel.ResourceMapper.SystemVariables.LoadResource(Me.SystemVariables)
            Call Me.CellSystemModel.ResourceMapper.ChemotaxisProfile.LoadResource(Me.ChemotaxisSensing)
            Call Me.CellSystemModel.ResourceMapper.CheBMethylesterase.LoadResource(Me.CheBMethylesterase)
            Call Me.CellSystemModel.ResourceMapper.CheBPhosphate.LoadResource(Me.CheBPhosphate)
            Call Me.CellSystemModel.ResourceMapper.CheRMethyltransferase.LoadResource(Me.CheRMethyltransferase)
            Call Me.CellSystemModel.ResourceMapper.CrossTalk.LoadResource(Me.CrossTalk)
            Call Me.CellSystemModel.ResourceMapper.HkAutoPhosphorus.LoadResource(Me.HkAutoPhosphorus)
            Call Me.CellSystemModel.ResourceMapper.OCSSensing.LoadResource(Me.OCSSensing)
            Call Me.CellSystemModel.ResourceMapper.Motifs.LoadResource(Me.Motifs)
            Call Me.CellSystemModel.ResourceMapper.Regulators.LoadResource(Me.Regulators)
            Call Me.CellSystemModel.ResourceMapper.CrossTalkAnnotations.LoadResource(Me.CrossTalksAnnotation)

            Call Me.CellSystemModel.ResourceMapper.STrPModel.LoadResource(Me.STrPModel)
            Call Me.CellSystemModel.ResourceMapper.MisT2.LoadResource(Me.MisT2)
            Call Me.CellSystemModel.ResourceMapper.KEGGCompiledXmlModel.LoadResource(Me.KEGG_Pathways)

            Call Console.WriteLine("Data load done!")

            My.Computer.FileSystem.CurrentDirectory = PreWork
        End Sub

        Private Function LoadData(Of T As Class)(Href As Href) As T
            If Not Href Is Nothing Then
                Dim path = Href.GetFullPath(ModelParentDIR)
                Return If(FileIO.FileSystem.FileExists(path), path.LoadXml(Of T), Nothing)
            Else
                Return Nothing
            End If
        End Function

        Private Function LoadCsvData(Of T As Class)(Href As Href) As List(Of T)
            If Href Is Nothing Then
                Call Console.WriteLine("Href of type {0} is null....", GetType(T).FullName)
                Return New List(Of T)
            End If

            Dim Path = Href.GetFullPath(ModelParentDIR)
            If FileIO.FileSystem.FileExists(Path) Then
                Return Path.LoadCsv(Of T)(False)
            Else
                Call Console.WriteLine("Data file {0} of type {1} is not exists on the filesystem!", Path, GetType(T).FullName)
                Return New List(Of T)
            End If
        End Function

        Public ReadOnly Property CellSystemModel As FileStream.XmlFormat.CellSystemXmlModel
            Get
                Return _InternalCellSystemResourceManager
            End Get
        End Property

        ''' <summary>
        ''' 设置模型数据 <seealso cref="XmlresxLoader.CellSystemModel"></seealso> 的导出路径
        ''' </summary>
        ''' <param name="dirPath"></param>
        ''' <remarks></remarks>
        Public Sub SetExportDirectory(dirPath As String)
            Me._InternalModelParentDirRoot = dirPath
        End Sub

        Public Function SaveTo(File As String) As Boolean
            If String.IsNullOrEmpty(File) Then
                If String.IsNullOrEmpty(_InternalCellSystemResourceManager.FilePath) Then
                    File = My.Computer.FileSystem.SpecialDirectories.Temp & "/____temp.xml"
                Else
                    File = _InternalCellSystemResourceManager.FilePath
                End If
            End If

            Dim PreWork As String = My.Computer.FileSystem.CurrentDirectory

            Me._InternalModelParentDirRoot = FileIO.FileSystem.GetParentPath(File)
            Call FileIO.FileSystem.CreateDirectory(Me.ModelParentDIR)
            Call FlushMemory()
            My.Computer.FileSystem.CurrentDirectory = Me._InternalModelParentDirRoot
            Call Console.WriteLine(vbCrLf & "Save model data to {0}" & vbCrLf, ModelParentDIR)

            Dim ResourceCollection = Me.CellSystemModel.get_ResourceCollectionItem(FileStream.XmlFormat.ResourceNode.TYPE_ID_BACTERIA_CELL_PHENOTYPE_DESCRIPTION) '保存细胞表型相关的数据

            Dim Metabolites = (From item In Me.MetabolitesModel Select item.Value Order By Value.Identifier Ascending).ToArray

            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.MetabolitesModel, data:=Metabolites)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.Proteins, data:=Me.Proteins)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.Enzymes, data:=Me.Enzymes)
            Call ResourceCollection.WriteResource(hreflink:=Me.CellSystemModel.ResourceMapper.ProteinAssembly, data:=Me.ProteinAssembly)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.Pathway, data:=Me.Pathway)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.TransmembraneTransportation, data:=Me.TransmembraneTransportation)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.MetabolismModel, data:=Me.MetabolismModel)


            ResourceCollection = Me.CellSystemModel.get_ResourceCollectionItem(FileStream.XmlFormat.ResourceNode.TYPE_ID_BACTERIA_GENOME_PROGRAMMING_INFORMATION)    '保存基因组信息

            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.TranscriptionModel, data:=Me.TranscriptionModel)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.Transcript, data:=Me.Transcripts)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.ExpressionKinetics, data:=Me.ExpressionKinetics)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.RibosomeAssembly, data:=RibosomeAssembly)
            Call ResourceCollection.WriteResource(hrefLink:=Me._InternalCellSystemResourceManager.ResourceMapper.RNAPolymerase, data:=RNAPolymerase)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.Motifs, data:=Motifs)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.Regulators, data:=Regulators)


            ResourceCollection = Me.CellSystemModel.get_ResourceCollectionItem(FileStream.XmlFormat.ResourceNode.TYPE_ID_BACTERIA_ANNOTATION_DATA_CHUNK)      '保存注释信息
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.GenomeAnnotiation, data:=Me.GenomeAnnotiation)
            Call ResourceCollection.WriteResource(hrefLink:=Me._InternalCellSystemResourceManager.ResourceMapper.DoorOperons, data:=DoorOperon)
            Call ResourceCollection.WriteResource(hreflink:=Me.CellSystemModel.ResourceMapper.STrPModel, data:=STrPModel)
            Call ResourceCollection.WriteResource(hreflink:=Me.CellSystemModel.ResourceMapper.MisT2, data:=Me.MisT2)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.CrossTalkAnnotations, data:=Me.CrossTalksAnnotation)
            Call ResourceCollection.WriteResource(hreflink:=Me.CellSystemModel.ResourceMapper.KEGGCompiledXmlModel, data:=Me.KEGG_Pathways)

            '由于在数据库之间存在差异，所以无法自动判断，需要用户手动填酶分子的值
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.EnzymeMapping, data:=EnzymeMapping)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.EffectorMapping, data:=EffectorMapping)

            ResourceCollection = Me.CellSystemModel.get_ResourceCollectionItem(FileStream.XmlFormat.ResourceNode.TYPE_ID_BACTERIA_SIGNAL_TRANSDUCTION_NETWORK)   '保存信号转到网络数据

            Call ResourceCollection.WriteResource(hrefLink:=Me._InternalCellSystemResourceManager.ResourceMapper.ChemotaxisProfile, data:=Me.ChemotaxisSensing)
            Call ResourceCollection.WriteResource(hrefLink:=Me._InternalCellSystemResourceManager.ResourceMapper.CheBMethylesterase, data:=Me.CheBMethylesterase)
            Call ResourceCollection.WriteResource(hrefLink:=Me._InternalCellSystemResourceManager.ResourceMapper.CheBPhosphate, data:=Me.CheBPhosphate)
            Call ResourceCollection.WriteResource(hrefLink:=Me._InternalCellSystemResourceManager.ResourceMapper.CheRMethyltransferase, data:=Me.CheRMethyltransferase)
            Call ResourceCollection.WriteResource(hrefLink:=Me._InternalCellSystemResourceManager.ResourceMapper.CrossTalk, data:=Me.CrossTalk)
            Call ResourceCollection.WriteResource(hrefLink:=Me._InternalCellSystemResourceManager.ResourceMapper.HkAutoPhosphorus, data:=Me.HkAutoPhosphorus)
            Call ResourceCollection.WriteResource(hrefLink:=Me._InternalCellSystemResourceManager.ResourceMapper.OCSSensing, data:=Me.OCSSensing)


            ResourceCollection = CellSystemModel.get_ResourceCollectionItem(FileStream.XmlFormat.ResourceNode.TYPE_ID_BACTERIA_EXPERINMENT_ENVIRONMENT)  '保存虚拟细胞的实验数据

            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.ObjectiveFunctionModel, data:=New FileStream.ObjectiveFunction() {})
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.SystemVariables, data:=Me.SystemVariables)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.CultivationMediums, data:=Me.CultivationMediums)
            Call ResourceCollection.WriteResource(hrefLink:=Me.CellSystemModel.ResourceMapper.ConstraintMetabolites, data:=Me.ConstraintMetabolites)


            Call Me.CellSystemModel.Save(File)

            My.Computer.FileSystem.CurrentDirectory = PreWork

            Return True
        End Function

        Private Sub Copy(ByRef href As Href, Dir As String)
            If href Is Nothing Then
                Return
            End If

            Dim Path As String = href.GetFullPath(Me._InternalModelParentDirRoot)

            If Not FileIO.FileSystem.FileExists(Path) Then
                Call Console.WriteLine("[ERROR] Object data file ""{0}"" is not exists on your filesystem!", Path)
                Return
            End If

            Path = String.Format("{0}/{1}", Dir, FileIO.FileSystem.GetName(Path))

            If FileIO.FileSystem.FileExists(Path) Then
                Call Console.WriteLine("Object data in file ""{0}"" was overrided!", FileIO.FileSystem.GetFileInfo(Path).FullName)
                Call FileIO.FileSystem.DeleteFile(Path)
            End If
            Call FileIO.FileSystem.CopyFile(href.Value, Path) : href.Value = Path
        End Sub

        Private Function SaveData(Of T As Class)(File As String, obj As T) As Href
            If FileIO.FileSystem.FileExists(File) Then
                Call FileIO.FileSystem.DeleteFile(File, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
            End If
            Call obj.GetXml.SaveTo(File)

            Return New Href With {
                .Value = File
            }
        End Function

        Private Function SaveCsvData(Of T As Class)(File As String, data As IEnumerable(Of T)) As Href
            If FileIO.FileSystem.FileExists(File) Then
                Call FileIO.FileSystem.DeleteFile(File, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
            End If

            On Error Resume Next

            If data.Empty Then
                Call Console.WriteLine("Model Data is NULL!  ""{0}""", File)
            End If

            Call data.SaveTo(File, False)
            Return New Href With {
                .Value = File
            }
        End Function
    End Class
End Namespace
