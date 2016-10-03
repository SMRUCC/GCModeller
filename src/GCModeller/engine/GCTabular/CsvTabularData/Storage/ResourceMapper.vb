#Region "Microsoft.VisualBasic::954982d3267cd002435de8c0ef163e7f, ..\GCModeller\engine\GCTabular\CsvTabularData\Storage\ResourceMapper.vb"

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

Imports System.Xml.Serialization

Namespace FileStream.XmlFormat

    Public Class ResourceMapper

#Region "String Resources"
        Public Const RES_ID_MisT2 As String = "RES_ID_MisT2"
        Public Const RES_ID_MetabolitesModel As String = "RES_ID_MetabolitesModel"
        Public Const RES_ID_MetabolismModel As String = "RES_ID_MetabolismModel"
        Public Const RES_ID_TranscriptionModel As String = "RES_ID_TranscriptionModel"
        Public Const RES_ID_ObjectiveFunctionModel As String = "RES_ID_ObjectiveFunctionModel"
        Public Const RES_ID_GenomeAnnotiation As String = "RES_ID_GenomeAnnotiation"
        Public Const RES_ID_Transcript As String = "RES_ID_Transcript"
        Public Const RES_ID_Proteins As String = "RES_ID_Proteins"
        Public Const RES_ID_Enzymes As String = "RES_ID_Enzymes"
        Public Const RES_ID_SystemVariables As String = "RES_ID_SystemVariables"
        Public Const RES_ID_STrPModel As String = "RES_ID_STrPModel"
        Public Const RES_ID_Motifs As String = "RES_ID_Motifs"
        Public Const RES_ID_Regulators As String = "RES_ID_Regulators"
        Public Const RES_ID_ConstraintMetabolites As String = "RES_ID_ConstraintMetabolites"
        Public Const RES_ID_ProteinAssembly As String = "RES_ID_ProteinAssembly"
        Public Const RES_ID_CultivationMediums As String = "RES_ID_CultivationMediums"
        Public Const RES_ID_ChemotaxisProfile As String = "RES_ID_ChemotaxisProfile"
        Public Const RES_ID_EffectorMapping As String = "RES_ID_EffectorMapping"
        Public Const RES_ID_EnzymeMapping As String = "RES_ID_EnzymeMapping"
        Public Const RES_ID_Pathway As String = "RES_ID_Pathway"
        Public Const RES_ID_RibosomeAssembly As String = "RES_ID_RibosomeAssembly"
        Public Const RES_ID_RNAPolymerase As String = "RES_ID_RNAPolymerase"
        Public Const RES_ID_TransmembraneTransportation As String = "RES_ID_TransmembraneTransportation"
        Public Const RES_ID_ExpressionKinetics As String = "RES_ID_ExpressionKinetics"
        Public Const RES_ID_DoorOperons As String = "RES_ID_DoorOperons"
        Public Const RES_ID_ChipData As String = "RES_ID_ChipData"
        Public Const RES_ID_CheBMethylesterase As String = "RES_ID_CheBMethylesterase"
        Public Const RES_ID_CheRMethyltransferase As String = "RES_ID_CheRMethyltransferase"
        Public Const RES_ID_CheBPhosphate As String = "RES_ID_CheBPhosphate"
        Public Const RES_ID_HkAutoPhosphorus As String = "RES_ID_HkAutoPhosphorus"
        Public Const RES_ID_CrossTalk As String = "RES_ID_CrossTalk"
        Public Const RES_ID_OCSSensing As String = "RES_ID_OCSSensing"

        Public Const RES_FILE_MisT2 As String = "resource_MisT2.csv"
        Public Const RES_FILE_MetabolitesModel As String = "resource_MetabolitesModel.csv"
        Public Const RES_FILE_MetabolismModel As String = "resource_MetabolismModel.csv"
        Public Const RES_FILE_TranscriptionModel As String = "resource_TranscriptionModel.csv"
        Public Const RES_FILE_ObjectiveFunctionModel As String = "resource_ObjectiveFunctionModel.csv"
        Public Const RES_FILE_GenomeAnnotiation As String = "resource_GenomeAnnotiation.csv"
        Public Const RES_FILE_Transcript As String = "resource_Transcript.csv"
        Public Const RES_FILE_Proteins As String = "resource_Proteins.csv"
        Public Const RES_FILE_Enzymes As String = "resource_Enzymes.csv"
        Public Const RES_FILE_SystemVariables As String = "resource_SystemVariables.csv"
        Public Const RES_FILE_STrPModel As String = "resource_STrPModel.csv"
        Public Const RES_FILE_Motifs As String = "resource_Motifs.csv"
        Public Const RES_FILE_Regulators As String = "resource_Regulators.csv"
        Public Const RES_FILE_ConstraintMetabolites As String = "resource_ConstraintMetabolites.csv"
        Public Const RES_FILE_ProteinAssembly As String = "resource_ProteinAssembly.csv"
        Public Const RES_FILE_CultivationMediums As String = "resource_CultivationMediums.csv"
        Public Const RES_FILE_ChemotaxisProfile As String = "resource_ChemotaxisProfile.csv"
        Public Const RES_FILE_EffectorMapping As String = "resource_EffectorMapping.csv"
        Public Const RES_FILE_EnzymeMapping As String = "resource_EnzymeMapping.csv"
        Public Const RES_FILE_Pathway As String = "resource_Pathway.csv"
        Public Const RES_FILE_RibosomeAssembly As String = "resource_RibosomeAssembly.csv"
        Public Const RES_FILE_RNAPolymerase As String = "resource_RNAPolymerase.csv"
        Public Const RES_FILE_TransmembraneTransportation As String = "resource_TransmembraneTransportation.csv"
        Public Const RES_FILE_ExpressionKinetics As String = "resource_ExpressionKinetics.csv"
        Public Const RES_FILE_DoorOperons As String = "resource_DoorOperons.csv"
        Public Const RES_FILE_ChipData As String = "resource_ChipData.csv"
        Public Const RES_FILE_CheBMethylesterase As String = "resource_CheBMethylesterase.csv"
        Public Const RES_FILE_CheRMethyltransferase As String = "resource_CheRMethyltransferase.csv"
        Public Const RES_FILE_CheBPhosphate As String = "resource_CheBPhosphate.csv"
        Public Const RES_FILE_HkAutoPhosphorus As String = "resource_HkAutoPhosphorus.csv"
        Public Const RES_FILE_CrossTalk As String = "resource_CrossTalk.csv"
        Public Const RES_FILE_OCSSensing As String = "resource_OCSSensing.csv"
#End Region

#Region "Resource Link Mappings.(资源连接映射属性)"
#Region "Internal Storage"
        Dim _MisT2 As HrefLink
        Dim _MetabolitesModel As HrefLink
        Dim _MetabolismModel As HrefLink
        Dim _TranscriptionModel As HrefLink
        Dim _ObjectiveFunctionModel As HrefLink
        Dim _GenomeAnnotiation As HrefLink
        Dim _Transcript As HrefLink
        Dim _Proteins As HrefLink
        Dim _Enzymes As HrefLink
        Dim _SystemVariables As HrefLink
        Dim _STrPModel As HrefLink
        Dim _Motifs As HrefLink
        Dim _Regulators As HrefLink
        Dim _ConstraintMetabolites As HrefLink
        Dim _ProteinAssembly As HrefLink
        Dim _CultivationMediums As HrefLink
        Dim _ChemotaxisProfile As HrefLink
        Dim _EffectorMapping As HrefLink
        Dim _EnzymeMapping As HrefLink
        Dim _Pathway As HrefLink
        Dim _RibosomeAssembly As HrefLink
        Dim _RNAPolymerase As HrefLink
        Dim _TransmembraneTransportation As HrefLink
        Dim _ExpressionKinetics As HrefLink
        Dim _DoorOperons As HrefLink
        Dim _ChipData As HrefLink
        Dim _CheBMethylesterase As HrefLink
        Dim _CheRMethyltransferase As HrefLink
        Dim _CheBPhosphate As HrefLink
        Dim _HkAutoPhosphorus As HrefLink
        Dim _CrossTalk As HrefLink
        Dim _OCSSensing As HrefLink
#End Region

        Dim _KEGGCompiledXmlModel As HrefLink

        Public Const RES_ID_KEGG_MODEL As String = "RES_ID_KEGG_MODEL"
        Public Const RES_FILE_KEGG_MODEL As String = "resource_KeggCompiled.csv"

        Public Property KEGGCompiledXmlModel As HrefLink
            Get
                Call InternalCheckNull(argv:=_KEGGCompiledXmlModel, comments:="", resourceId:=RES_ID_KEGG_MODEL, savefile:=RES_FILE_KEGG_MODEL)
                Return _KEGGCompiledXmlModel
            End Get
            Set(value As HrefLink)
                _KEGGCompiledXmlModel = value
            End Set
        End Property

        <XmlIgnore> Public Property MisT2 As HrefLink
            Get
                Call InternalCheckNull(argv:=_MisT2, resourceId:=RES_ID_MisT2, savefile:=ResourceMapper.RES_FILE_MisT2, comments:=<MisT2>The Mist2 database file for the bacteria signal transduction network.</MisT2>)
                Return _MisT2
            End Get
            Set(value As HrefLink)
                _MisT2 = value
            End Set
        End Property

        ''' <summary>
        ''' <see cref="Metabolite"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore> Public Property MetabolitesModel As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._MetabolitesModel, resourceId:=ResourceMapper.RES_ID_MetabolitesModel, savefile:=ResourceMapper.RES_FILE_MetabolitesModel, comments:="")
                Return _MetabolitesModel
            End Get
            Set(value As HrefLink)
                _MetabolitesModel = value
            End Set
        End Property

        ''' <summary>
        ''' <see cref="MetabolismFlux"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore> Public Property MetabolismModel As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._MetabolismModel, resourceId:=ResourceMapper.RES_ID_MetabolismModel, savefile:=ResourceMapper.RES_FILE_MetabolismModel, comments:="")
                Return Me._MetabolismModel
            End Get
            Set(value As HrefLink)
                Me._MetabolismModel = value
            End Set
        End Property

        ''' <summary>
        ''' <see cref="TranscriptUnit"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore> Public Property TranscriptionModel As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._TranscriptionModel, resourceId:=ResourceMapper.RES_ID_TranscriptionModel, savefile:=ResourceMapper.RES_FILE_TranscriptionModel, comments:="")
                Return Me._TranscriptionModel
            End Get
            Set(value As HrefLink)
                Me._TranscriptionModel = value
            End Set
        End Property

        ''' <summary>
        ''' <see cref="ObjectiveFunction"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore> Public Property ObjectiveFunctionModel As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._ObjectiveFunctionModel, resourceId:=ResourceMapper.RES_ID_ObjectiveFunctionModel, savefile:=ResourceMapper.RES_FILE_ObjectiveFunctionModel, comments:="")
                Return Me._ObjectiveFunctionModel
            End Get
            Set(value As HrefLink)
                Me._ObjectiveFunctionModel = value
            End Set
        End Property
        ''' <summary>
        ''' <see cref="GeneObject"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore> Public Property GenomeAnnotiation As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._GenomeAnnotiation, resourceId:=ResourceMapper.RES_ID_GenomeAnnotiation, savefile:=ResourceMapper.RES_FILE_GenomeAnnotiation, comments:="")
                Return Me._GenomeAnnotiation
            End Get
            Set(value As HrefLink)
                Me._GenomeAnnotiation = value
            End Set
        End Property

        ''' <summary>
        ''' <see cref="gcTabular.FileStream.Transcript"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore> Public Property Transcript As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._Transcript, resourceId:=ResourceMapper.RES_ID_Transcript, savefile:=ResourceMapper.RES_FILE_Transcript, comments:="")
                Return Me._Transcript
            End Get
            Set(value As HrefLink)
                Me._Transcript = value
            End Set
        End Property

        <XmlIgnore> Public Property Proteins As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._Proteins, resourceId:=ResourceMapper.RES_ID_Proteins, savefile:=ResourceMapper.RES_FILE_Proteins, comments:="")
                Return Me._Proteins
            End Get
            Set(value As HrefLink)
                Me._Proteins = value
            End Set
        End Property

        <XmlIgnore> Public Property Enzymes As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._Enzymes, resourceId:=ResourceMapper.RES_ID_Enzymes, savefile:=ResourceMapper.RES_FILE_Enzymes, comments:="")
                Return Me._Enzymes
            End Get
            Set(value As HrefLink)
                Me._Enzymes = value
            End Set
        End Property

        <XmlIgnore> Public Property SystemVariables As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._SystemVariables, resourceId:=ResourceMapper.RES_ID_SystemVariables, savefile:=ResourceMapper.RES_FILE_SystemVariables, comments:="")
                Return Me._SystemVariables
            End Get
            Set(value As HrefLink)
                Me._SystemVariables = value
            End Set
        End Property

        <XmlIgnore> Public Property STrPModel As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._STrPModel, resourceId:=ResourceMapper.RES_ID_STrPModel, savefile:=ResourceMapper.RES_FILE_STrPModel, comments:="")
                Return Me._STrPModel
            End Get
            Set(value As HrefLink)
                Me._STrPModel = value
            End Set
        End Property

        <XmlIgnore> Public Property Motifs As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._Motifs, resourceId:=ResourceMapper.RES_ID_Motifs, savefile:=ResourceMapper.RES_FILE_Motifs, comments:="")
                Return Me._Motifs
            End Get
            Set(value As HrefLink)
                Me._Motifs = value
            End Set
        End Property
        <XmlIgnore> Public Property Regulators As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._Regulators, resourceId:=ResourceMapper.RES_ID_Regulators, savefile:=ResourceMapper.RES_FILE_Regulators, comments:="")
                Return Me._Regulators
            End Get
            Set(value As HrefLink)
                Me._Regulators = value
            End Set
        End Property

        <XmlIgnore> Public Property ConstraintMetabolites As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._ConstraintMetabolites, resourceId:=ResourceMapper.RES_ID_ConstraintMetabolites, savefile:=ResourceMapper.RES_FILE_ConstraintMetabolites, comments:="")
                Return Me._ConstraintMetabolites
            End Get
            Set(value As HrefLink)
                Me._ConstraintMetabolites = value
            End Set
        End Property
        <XmlIgnore> Public Property ProteinAssembly As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._ProteinAssembly, resourceId:=ResourceMapper.RES_ID_ProteinAssembly, savefile:=ResourceMapper.RES_FILE_ProteinAssembly, comments:="")
                Return Me._ProteinAssembly
            End Get
            Set(value As HrefLink)
                Me._ProteinAssembly = value
            End Set
        End Property
        <XmlIgnore> Public Property CultivationMediums As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._CultivationMediums, resourceId:=ResourceMapper.RES_ID_CultivationMediums, savefile:=ResourceMapper.RES_FILE_CultivationMediums, comments:="")
                Return Me._CultivationMediums
            End Get
            Set(value As HrefLink)
                Me._CultivationMediums = value
            End Set
        End Property
        <XmlIgnore> Public Property ChemotaxisProfile As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._ChemotaxisProfile, resourceId:=ResourceMapper.RES_ID_ChemotaxisProfile, savefile:=ResourceMapper.RES_FILE_ChemotaxisProfile, comments:="")
                Return Me._ChemotaxisProfile
            End Get
            Set(value As HrefLink)
                Me._ChemotaxisProfile = value
            End Set
        End Property
        <XmlIgnore> Public Property EffectorMapping As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._EffectorMapping, resourceId:=ResourceMapper.RES_ID_EffectorMapping, savefile:=ResourceMapper.RES_FILE_EffectorMapping, comments:="")
                Return Me._EffectorMapping
            End Get
            Set(value As HrefLink)
                Me._EffectorMapping = value
            End Set
        End Property
        <XmlIgnore> Public Property EnzymeMapping As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._EnzymeMapping, resourceId:=ResourceMapper.RES_ID_EnzymeMapping, savefile:=ResourceMapper.RES_FILE_EnzymeMapping, comments:="")
                Return Me._EnzymeMapping
            End Get
            Set(value As HrefLink)
                Me._EnzymeMapping = value
            End Set
        End Property
        <XmlIgnore> Public Property Pathway As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._Pathway, resourceId:=ResourceMapper.RES_ID_Pathway, savefile:=ResourceMapper.RES_FILE_Pathway, comments:="")
                Return Me._Pathway
            End Get
            Set(value As HrefLink)
                Me._Pathway = value
            End Set
        End Property

        <XmlIgnore> Public Property RibosomeAssembly As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._RibosomeAssembly, resourceId:=ResourceMapper.RES_ID_RibosomeAssembly, savefile:=ResourceMapper.RES_FILE_RibosomeAssembly, comments:="")
                Return Me._RibosomeAssembly
            End Get
            Set(value As HrefLink)
                Me._RibosomeAssembly = value
            End Set
        End Property
        <XmlIgnore> Public Property RNAPolymerase As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._RNAPolymerase, resourceId:=ResourceMapper.RES_ID_RNAPolymerase, savefile:=ResourceMapper.RES_FILE_RNAPolymerase, comments:="")
                Return Me._RNAPolymerase
            End Get
            Set(value As HrefLink)
                Me._RNAPolymerase = value
            End Set
        End Property

        <XmlIgnore> Public Property TransmembraneTransportation As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._TransmembraneTransportation, resourceId:=ResourceMapper.RES_ID_TransmembraneTransportation, savefile:=ResourceMapper.RES_FILE_TransmembraneTransportation, comments:="")
                Return Me._TransmembraneTransportation
            End Get
            Set(value As HrefLink)
                Me._TransmembraneTransportation = value
            End Set
        End Property
        <XmlIgnore> Public Property ExpressionKinetics As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._ExpressionKinetics, resourceId:=ResourceMapper.RES_ID_ExpressionKinetics, savefile:=ResourceMapper.RES_FILE_ExpressionKinetics, comments:="")
                Return Me._ExpressionKinetics
            End Get
            Set(value As HrefLink)
                Me._ExpressionKinetics = value
            End Set
        End Property
        <XmlIgnore> Public Property DoorOperons As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._DoorOperons, resourceId:=ResourceMapper.RES_ID_DoorOperons, savefile:=ResourceMapper.RES_FILE_DoorOperons, comments:="")
                Return Me._DoorOperons
            End Get
            Set(value As HrefLink)
                Me._DoorOperons = value
            End Set
        End Property

        <XmlIgnore> Public Property ChipData As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._ChipData, resourceId:=ResourceMapper.RES_ID_ChipData, savefile:=ResourceMapper.RES_FILE_ChipData, comments:="")
                Return Me._ChipData
            End Get
            Set(value As HrefLink)
                Me._ChipData = value
            End Set
        End Property

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
        <XmlIgnore> Public Property CheBMethylesterase As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._CheBMethylesterase, resourceId:=ResourceMapper.RES_ID_CheBMethylesterase, savefile:=ResourceMapper.RES_FILE_CheBMethylesterase, comments:="")
                Return Me._CheBMethylesterase
            End Get
            Set(value As HrefLink)
                Me._CheBMethylesterase = value
            End Set
        End Property
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
        <XmlIgnore> Public Property CheRMethyltransferase As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._CheRMethyltransferase, resourceId:=ResourceMapper.RES_ID_CheRMethyltransferase, savefile:=ResourceMapper.RES_FILE_CheRMethyltransferase, comments:="")
                Return Me._CheRMethyltransferase
            End Get
            Set(value As HrefLink)
                Me._CheRMethyltransferase = value
            End Set
        End Property
        ''' <summary>
        ''' CheB + [ChA][PI] -> [CheB][PI] + CheA
        ''' </summary>
        ''' <remarks></remarks>
        <XmlIgnore> Public Property CheBPhosphate As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._CheBPhosphate, resourceId:=ResourceMapper.RES_ID_CheBPhosphate, savefile:=ResourceMapper.RES_FILE_CheBPhosphate, comments:="")
                Return Me._CheBPhosphate
            End Get
            Set(value As HrefLink)
                Me._CheBPhosphate = value
            End Set
        End Property

        ''' <summary>
        ''' CheAHK + ATP -> [CheAHK][PI] + ADP   Enzyme: [MCP][CH3][Inducer]
        ''' </summary>
        ''' <remarks></remarks>
        <XmlIgnore> Public Property HkAutoPhosphorus As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._HkAutoPhosphorus, resourceId:=ResourceMapper.RES_ID_HkAutoPhosphorus, savefile:=ResourceMapper.RES_FILE_HkAutoPhosphorus, comments:="")
                Return Me._HkAutoPhosphorus
            End Get
            Set(value As HrefLink)
                Me._HkAutoPhosphorus = value
            End Set
        End Property

        Dim _CrossTalkAnnotations As HrefLink

        Public Const RES_ID_CrossTalk_Annotations As String = "RES_ID_CrossTalk_Annotations"
        Public Const RES_FILE_CrossTalk_Annotations As String = "resource_CrossTalk_Annotations.csv"

        <XmlIgnore> Public Property CrossTalkAnnotations As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._CrossTalkAnnotations, resourceId:=ResourceMapper.RES_ID_CrossTalk_Annotations, comments:="", savefile:=ResourceMapper.RES_FILE_CrossTalk_Annotations)
                Return Me._CrossTalkAnnotations
            End Get
            Set(value As HrefLink)
                Me._CrossTalkAnnotations = value
            End Set
        End Property

        ''' <summary>
        ''' [CheAHK][PI] + RR -> [RR][PI] + CheAHK
        ''' [CheAHK][PI] + OCS -> CheAHK + [OCS][PI]
        '''
        ''' </summary>
        ''' <remarks></remarks>
        <XmlIgnore> Public Property CrossTalk As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._CrossTalk, resourceId:=ResourceMapper.RES_ID_CrossTalk, savefile:=ResourceMapper.RES_FILE_CrossTalk, comments:="")
                Return Me._CrossTalk
            End Get
            Set(value As HrefLink)
                Me._CrossTalk = value
            End Set
        End Property
        <XmlIgnore> Public Property OCSSensing As HrefLink
            Get
                Call InternalCheckNull(argv:=Me._OCSSensing, resourceId:=ResourceMapper.RES_ID_OCSSensing, savefile:=ResourceMapper.RES_FILE_OCSSensing, comments:="")
                Return Me._OCSSensing
            End Get
            Set(value As HrefLink)
                Me._OCSSensing = value
            End Set
        End Property

#End Region

        Private Shared Function InternalCheckNull(ByRef argv As HrefLink, resourceId As String, comments As String, savefile As String) As HrefLink
            If argv Is Nothing Then
                argv = New HrefLink With {.ResourceId = resourceId, .Annotations = comments, .Value = savefile}
            End If
            Return argv
        End Function

        Sub New(ResourceCollection As ResourceCollection)
            Me.CheBMethylesterase = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_CheBMethylesterase)
            Me.CheBPhosphate = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_CheBPhosphate)
            Me.ChemotaxisProfile = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_ChemotaxisProfile)
            Me.CheRMethyltransferase = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_CheRMethyltransferase)
            Me.ChipData = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_ChipData)
            Me.ConstraintMetabolites = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_ConstraintMetabolites)
            Me.CrossTalk = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_CrossTalk)
            Me.CultivationMediums = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_CultivationMediums)
            Me.DoorOperons = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_DoorOperons)
            Me.EffectorMapping = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_EffectorMapping)
            Me.EnzymeMapping = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_EnzymeMapping)
            Me.Enzymes = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_Enzymes)
            Me.ExpressionKinetics = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_ExpressionKinetics)
            Me.GenomeAnnotiation = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_GenomeAnnotiation)
            Me.HkAutoPhosphorus = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_HkAutoPhosphorus)
            Me.MetabolismModel = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_MetabolismModel)
            Me.MetabolitesModel = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_MetabolitesModel)
            Me.MisT2 = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_MisT2)
            Me.Motifs = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_Motifs)
            Me.ObjectiveFunctionModel = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_ObjectiveFunctionModel)
            Me.OCSSensing = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_OCSSensing)
            Me.Pathway = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_Pathway)
            Me.ProteinAssembly = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_ProteinAssembly)
            Me.Proteins = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_Proteins)
            Me.Regulators = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_Regulators)
            Me.RibosomeAssembly = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_RibosomeAssembly)
            Me.RNAPolymerase = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_RNAPolymerase)
            Me.STrPModel = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_STrPModel)
            Me.SystemVariables = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_SystemVariables)
            Me.Transcript = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_Transcript)
            Me.TranscriptionModel = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_TranscriptionModel)
            Me.TransmembraneTransportation = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_TransmembraneTransportation)
            Me.CrossTalkAnnotations = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_CrossTalk_Annotations)
            Me.KEGGCompiledXmlModel = ResourceCollection.TryGetValue(ResourceMapper.RES_ID_KEGG_MODEL)
        End Sub

        Sub New()
        End Sub
    End Class

End Namespace
