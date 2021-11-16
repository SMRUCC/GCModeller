#Region "Microsoft.VisualBasic::38203f8d14d5503b838f6271e7fad95e, data\WebServices\Service References\NCBI\Reference.vb"

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

    '     Interface PUGSoap
    ' 
    '         Function: AssayDownload, AssayDownloadAsync, Download, DownloadAsync, GetAssayColumnDescription
    '                   GetAssayColumnDescriptionAsync, GetAssayColumnDescriptions, GetAssayColumnDescriptionsAsync, GetAssayDescription, GetAssayDescriptionAsync
    '                   GetDownloadUrl, GetDownloadUrlAsync, GetEntrezKey, GetEntrezKeyAsync, GetEntrezUrl
    '                   GetEntrezUrlAsync, GetIDList, GetIDListAsync, GetListItemsCount, GetListItemsCountAsync
    '                   GetOperationStatus, GetOperationStatusAsync, GetStandardizedCID, GetStandardizedCIDAsync, GetStandardizedStructure
    '                   GetStandardizedStructureAsync, GetStandardizedStructureBase64, GetStandardizedStructureBase64Async, GetStatusMessage, GetStatusMessageAsync
    '                   IdentitySearch, IdentitySearchAsync, IDExchange, IDExchangeAsync, InputAssay
    '                   InputAssayAsync, InputEntrez, InputEntrezAsync, InputList, InputListAsync
    '                   InputListString, InputListStringAsync, InputListText, InputListTextAsync, InputStructure
    '                   InputStructureAsync, InputStructureBase64, InputStructureBase64Async, MFSearch, MFSearchAsync
    '                   ScoreMatrix, ScoreMatrixAsync, SimilaritySearch2D, SimilaritySearch2DAsync, Standardize
    '                   StandardizeAsync, SubstructureSearch, SubstructureSearchAsync, SuperstructureSearch, SuperstructureSearchAsync
    ' 
    '     Enum AssayFormatType
    ' 
    '         eAssayFormat_ASN_Binary, eAssayFormat_ASN_Text, eAssayFormat_CSV, eAssayFormat_XML
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum CompressType
    ' 
    '         eCompress_BZip2, eCompress_GZip, eCompress_None
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum FormatType
    ' 
    '         eFormat_ASNB, eFormat_ASNT, eFormat_Image, eFormat_InChI, eFormat_SDF
    '         eFormat_SMILES, eFormat_Thumbnail, eFormat_XML
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class DataBlobType
    ' 
    '         Properties: BlobFormat, BlobFormatSpecified, Data, eCompress, eCompressSpecified
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Enum BlobFormatType
    ' 
    '         eBlobFormat_ASNB, eBlobFormat_ASNT, eBlobFormat_CSV, eBlobFormat_HTML, eBlobFormat_Other
    '         eBlobFormat_PNG, eBlobFormat_SDF, eBlobFormat_Text, eBlobFormat_Unspecified, eBlobFormat_XML
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class StructureSearchOptions
    ' 
    '         Properties: ChainsMatchRings, ChainsMatchRingsSpecified, eStereo, eStereoSpecified, MatchCharges
    '                     MatchChargesSpecified, MatchIsotopes, MatchIsotopesSpecified, MatchTautomers, MatchTautomersSpecified
    '                     RingsNotEmbedded, RingsNotEmbeddedSpecified, SingeDoubleBondsMatch, SingeDoubleBondsMatchSpecified, StripHydrogen
    '                     StripHydrogenSpecified, ToWebEnv
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Enum StereoType
    ' 
    '         eStereo_Exact, eStereo_Ignore, eStereo_NonConflicting, eStereo_Relative
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class SimilaritySearchOptions
    ' 
    '         Properties: threshold, ToWebEnv
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class MFSearchOptions
    ' 
    '         Properties: AllowOtherElements, ToWebEnv
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class LimitsType
    ' 
    '         Properties: ListKey, maxRecords, maxRecordsSpecified, seconds, secondsSpecified
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class IdentitySearchOptions
    ' 
    '         Properties: eIdentity, ToWebEnv
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Enum IdentityType
    ' 
    '         eIdentity_AnyTautomer, eIdentity_SameConnectivity, eIdentity_SameIsotope, eIdentity_SameIsotopeNonconflictStereo, eIdentity_SameNonconflictStereo
    '         eIdentity_SameStereo, eIdentity_SameStereoIsotope
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class EntrezKey
    ' 
    '         Properties: db, key, webenv
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class AssayTargetType
    ' 
    '         Properties: gi, Name
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class AssayDescriptionType
    ' 
    '         Properties: CIDCountActive, CIDCountActiveSpecified, CIDCountAll, CIDCountAllSpecified, CIDCountInactive
    '                     CIDCountInactiveSpecified, CIDCountInconclusive, CIDCountInconclusiveSpecified, CIDCountProbe, CIDCountProbeSpecified
    '                     CIDCountUnspecified, CIDCountUnspecifiedSpecified, Comment, Description, HasScore
    '                     LastDataChange, LastDataChangeSpecified, Method, Name, NumberOfTIDs
    '                     Protocol, Revision, RevisionSpecified, SIDCountActive, SIDCountActiveSpecified
    '                     SIDCountAll, SIDCountAllSpecified, SIDCountInactive, SIDCountInactiveSpecified, SIDCountInconclusive
    '                     SIDCountInconclusiveSpecified, SIDCountProbe, SIDCountProbeSpecified, SIDCountUnspecified, SIDCountUnspecifiedSpecified
    '                     Targets, Version, VersionSpecified
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class TestedConcentrationType
    ' 
    '         Properties: Concentration, Unit
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class ColumnDescriptionType
    ' 
    '         Properties: ActiveConcentration, ActiveConcentrationSpecified, Description, Heading, Name
    '                     TestedConcentration, TID, TIDSpecified, Type, Unit
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Enum HeadingType
    ' 
    '         outcome, score, TID
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class DownloadRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class DownloadResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class GetAssayColumnDescriptionsRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class GetAssayColumnDescriptionsResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class GetAssayDescriptionRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class GetAssayDescriptionResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Enum StatusType
    ' 
    '         eStatus_DataError, eStatus_HitLimit, eStatus_InputError, eStatus_Queued, eStatus_Running
    '         eStatus_ServerError, eStatus_Stopped, eStatus_Success, eStatus_TimeLimit, eStatus_Unknown
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class GetStandardizedStructureBase64Request
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class GetStandardizedStructureBase64Response
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Enum IDOperationType
    ' 
    '         eIDOperation_Same, eIDOperation_SameConnectivity, eIDOperation_SameIsotope, eIDOperation_SameParent, eIDOperation_SameParentConnectivity
    '         eIDOperation_SameParentIsotope, eIDOperation_SameParentStereo, eIDOperation_SameStereo, eIDOperation_Similar2D, eIDOperation_Similar3D
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum PCIDType
    ' 
    '         eID_AID, eID_CID, eID_ConformerID, eID_InChI, eID_InChIKey
    '         eID_SID, eID_SourceID, eID_TID
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum IDOutputFormatType
    ' 
    '         eIDOutputFormat_Entrez, eIDOutputFormat_FileList, eIDOutputFormat_FilePair
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class IDExchangeRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class IDExchangeResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Enum AssayColumnsType
    ' 
    '         eAssayColumns_Complete, eAssayColumns_Concise, eAssayColumns_TIDs
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum AssayOutcomeFilterType
    ' 
    '         eAssayOutcome_Active, eAssayOutcome_All, eAssayOutcome_Inactive, eAssayOutcome_Inconclusive, eAssayOutcome_Unspecified
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class InputListStringRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class InputListStringResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class InputStructureBase64Request
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class InputStructureBase64Response
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Enum ScoreTypeType
    ' 
    '         eScoreType_FeatureOpt3D, eScoreType_ShapeOpt3D, eScoreType_Sim2DSubs
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum MatrixFormatType
    ' 
    '         eMatrixFormat_CSV, eMatrixFormat_IdIdScore
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class StandardizeRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class StandardizeResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Interface PUGSoapChannel
    ' 
    ' 
    ' 
    '     Class PUGSoapClient
    ' 
    '         Constructor: (+5 Overloads) Sub New
    ' 
    '         Function: AssayDownload, AssayDownloadAsync, Download, DownloadAsync, GetAssayColumnDescription
    '                   GetAssayColumnDescriptionAsync, GetAssayColumnDescriptions, GetAssayColumnDescriptionsAsync, GetAssayDescription, GetAssayDescriptionAsync
    '                   GetDownloadUrl, GetDownloadUrlAsync, GetEntrezKey, GetEntrezKeyAsync, GetEntrezUrl
    '                   GetEntrezUrlAsync, GetIDList, GetIDListAsync, GetListItemsCount, GetListItemsCountAsync
    '                   GetOperationStatus, GetOperationStatusAsync, GetStandardizedCID, GetStandardizedCIDAsync, GetStandardizedStructure
    '                   GetStandardizedStructureAsync, GetStandardizedStructureBase64, GetStandardizedStructureBase64Async, GetStatusMessage, GetStatusMessageAsync
    '                   IdentitySearch, IdentitySearchAsync, IDExchange, IDExchangeAsync, InputAssay
    '                   InputAssayAsync, InputEntrez, InputEntrezAsync, InputList, InputListAsync
    '                   InputListString, InputListStringAsync, InputListText, InputListTextAsync, InputStructure
    '                   InputStructureAsync, InputStructureBase64, InputStructureBase64Async, MFSearch, MFSearchAsync
    '                   NCBI_PUGSoap_Download, NCBI_PUGSoap_GetAssayColumnDescriptions, NCBI_PUGSoap_GetAssayColumnDescriptionsAsync, NCBI_PUGSoap_GetAssayDescription, NCBI_PUGSoap_GetStandardizedStructureBase64
    '                   NCBI_PUGSoap_GetStandardizedStructureBase64Async, NCBI_PUGSoap_IDExchange, NCBI_PUGSoap_InputListString, NCBI_PUGSoap_InputListStringAsync, NCBI_PUGSoap_InputStructureBase64
    '                   NCBI_PUGSoap_InputStructureBase64Async, NCBI_PUGSoap_Standardize, ScoreMatrix, ScoreMatrixAsync, SimilaritySearch2D
    '                   SimilaritySearch2DAsync, StandardizeAsync, SubstructureSearch, SubstructureSearchAsync, SuperstructureSearch
    '                   SuperstructureSearchAsync
    ' 
    '         Sub: Standardize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace NCBI
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.ServiceContractAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", ConfigurationName:="NCBI.PUGSoap")>  _
    Public Interface PUGSoap
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/AssayDownload", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function AssayDownload(ByVal AssayKey As String, ByVal AssayFormat As NCBI.AssayFormatType, ByVal eCompress As NCBI.CompressType) As <System.ServiceModel.MessageParameterAttribute(Name:="DownloadKey")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/AssayDownload", ReplyAction:="*")>  _
        Function AssayDownloadAsync(ByVal AssayKey As String, ByVal AssayFormat As NCBI.AssayFormatType, ByVal eCompress As NCBI.CompressType) As <System.ServiceModel.MessageParameterAttribute(Name:="DownloadKey")> System.Threading.Tasks.Task(Of String)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/Download", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function Download(ByVal request As NCBI.DownloadRequest) As NCBI.DownloadResponse
        
        'CODEGEN: Generating message contract since the operation has multiple return values.
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/Download", ReplyAction:="*")>  _
        Function DownloadAsync(ByVal request As NCBI.DownloadRequest) As System.Threading.Tasks.Task(Of NCBI.DownloadResponse)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetAssayColumnDescription", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function GetAssayColumnDescription(ByVal AID As Integer, ByVal Heading As NCBI.HeadingType, ByVal TID As Integer) As <System.ServiceModel.MessageParameterAttribute(Name:="ColumnDescription")> NCBI.ColumnDescriptionType
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetAssayColumnDescription", ReplyAction:="*")>  _
        Function GetAssayColumnDescriptionAsync(ByVal AID As Integer, ByVal Heading As NCBI.HeadingType, ByVal TID As Integer) As <System.ServiceModel.MessageParameterAttribute(Name:="ColumnDescription")> System.Threading.Tasks.Task(Of NCBI.ColumnDescriptionType)
        
        'CODEGEN: Parameter 'ColumnDescription' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetAssayColumnDescriptions", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function GetAssayColumnDescriptions(ByVal request As NCBI.GetAssayColumnDescriptionsRequest) As <System.ServiceModel.MessageParameterAttribute(Name:="ColumnDescription")> NCBI.GetAssayColumnDescriptionsResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetAssayColumnDescriptions", ReplyAction:="*")>  _
        Function GetAssayColumnDescriptionsAsync(ByVal request As NCBI.GetAssayColumnDescriptionsRequest) As System.Threading.Tasks.Task(Of NCBI.GetAssayColumnDescriptionsResponse)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetAssayDescription", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function GetAssayDescription(ByVal request As NCBI.GetAssayDescriptionRequest) As NCBI.GetAssayDescriptionResponse
        
        'CODEGEN: Generating message contract since the operation has multiple return values.
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetAssayDescription", ReplyAction:="*")>  _
        Function GetAssayDescriptionAsync(ByVal request As NCBI.GetAssayDescriptionRequest) As System.Threading.Tasks.Task(Of NCBI.GetAssayDescriptionResponse)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetDownloadUrl", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function GetDownloadUrl(ByVal DownloadKey As String) As <System.ServiceModel.MessageParameterAttribute(Name:="url")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetDownloadUrl", ReplyAction:="*")>  _
        Function GetDownloadUrlAsync(ByVal DownloadKey As String) As <System.ServiceModel.MessageParameterAttribute(Name:="url")> System.Threading.Tasks.Task(Of String)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetEntrezKey", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function GetEntrezKey(ByVal ListKey As String) As <System.ServiceModel.MessageParameterAttribute(Name:="EntrezKey")> NCBI.EntrezKey
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetEntrezKey", ReplyAction:="*")>  _
        Function GetEntrezKeyAsync(ByVal ListKey As String) As <System.ServiceModel.MessageParameterAttribute(Name:="EntrezKey")> System.Threading.Tasks.Task(Of NCBI.EntrezKey)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetEntrezUrl", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function GetEntrezUrl(ByVal EntrezKey As NCBI.EntrezKey) As <System.ServiceModel.MessageParameterAttribute(Name:="url")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetEntrezUrl", ReplyAction:="*")>  _
        Function GetEntrezUrlAsync(ByVal EntrezKey As NCBI.EntrezKey) As <System.ServiceModel.MessageParameterAttribute(Name:="url")> System.Threading.Tasks.Task(Of String)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetIDList", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function GetIDList(ByVal ListKey As String, ByVal Start As Integer, ByVal Count As Integer) As <System.ServiceModel.MessageParameterAttribute(Name:="IDList")> Integer()
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetIDList", ReplyAction:="*")>  _
        Function GetIDListAsync(ByVal ListKey As String, ByVal Start As Integer, ByVal Count As Integer) As <System.ServiceModel.MessageParameterAttribute(Name:="IDList")> System.Threading.Tasks.Task(Of Integer())
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetListItemsCount", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function GetListItemsCount(ByVal ListKey As String) As <System.ServiceModel.MessageParameterAttribute(Name:="count")> Integer
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetListItemsCount", ReplyAction:="*")>  _
        Function GetListItemsCountAsync(ByVal ListKey As String) As <System.ServiceModel.MessageParameterAttribute(Name:="count")> System.Threading.Tasks.Task(Of Integer)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetOperationStatus", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function GetOperationStatus(ByVal AnyKey As String) As <System.ServiceModel.MessageParameterAttribute(Name:="status")> NCBI.StatusType
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetOperationStatus", ReplyAction:="*")>  _
        Function GetOperationStatusAsync(ByVal AnyKey As String) As <System.ServiceModel.MessageParameterAttribute(Name:="status")> System.Threading.Tasks.Task(Of NCBI.StatusType)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetStandardizedCID", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function GetStandardizedCID(ByVal StrKey As String) As <System.ServiceModel.MessageParameterAttribute(Name:="CID")> Integer
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetStandardizedCID", ReplyAction:="*")>  _
        Function GetStandardizedCIDAsync(ByVal StrKey As String) As <System.ServiceModel.MessageParameterAttribute(Name:="CID")> System.Threading.Tasks.Task(Of Integer)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetStandardizedStructure", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function GetStandardizedStructure(ByVal StrKey As String, ByVal format As NCBI.FormatType) As <System.ServiceModel.MessageParameterAttribute(Name:="structure")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetStandardizedStructure", ReplyAction:="*")>  _
        Function GetStandardizedStructureAsync(ByVal StrKey As String, ByVal format As NCBI.FormatType) As <System.ServiceModel.MessageParameterAttribute(Name:="structure")> System.Threading.Tasks.Task(Of String)
        
        'CODEGEN: Parameter 'structure' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetStandardizedStructureBase64", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function GetStandardizedStructureBase64(ByVal request As NCBI.GetStandardizedStructureBase64Request) As <System.ServiceModel.MessageParameterAttribute(Name:="structure")> NCBI.GetStandardizedStructureBase64Response
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetStandardizedStructureBase64", ReplyAction:="*")>  _
        Function GetStandardizedStructureBase64Async(ByVal request As NCBI.GetStandardizedStructureBase64Request) As System.Threading.Tasks.Task(Of NCBI.GetStandardizedStructureBase64Response)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetStatusMessage", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function GetStatusMessage(ByVal AnyKey As String) As <System.ServiceModel.MessageParameterAttribute(Name:="message")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/GetStatusMessage", ReplyAction:="*")>  _
        Function GetStatusMessageAsync(ByVal AnyKey As String) As <System.ServiceModel.MessageParameterAttribute(Name:="message")> System.Threading.Tasks.Task(Of String)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/IdentitySearch", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function IdentitySearch(ByVal StrKey As String, ByVal idOptions As NCBI.IdentitySearchOptions, ByVal limits As NCBI.LimitsType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/IdentitySearch", ReplyAction:="*")>  _
        Function IdentitySearchAsync(ByVal StrKey As String, ByVal idOptions As NCBI.IdentitySearchOptions, ByVal limits As NCBI.LimitsType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> System.Threading.Tasks.Task(Of String)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/IDExchange", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function IDExchange(ByVal request As NCBI.IDExchangeRequest) As NCBI.IDExchangeResponse
        
        'CODEGEN: Generating message contract since the operation has multiple return values.
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/IDExchange", ReplyAction:="*")>  _
        Function IDExchangeAsync(ByVal request As NCBI.IDExchangeRequest) As System.Threading.Tasks.Task(Of NCBI.IDExchangeResponse)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputAssay", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function InputAssay(ByVal AID As Integer, ByVal Columns As NCBI.AssayColumnsType, ByVal ListKeyTIDs As String, ByVal ListKeySCIDs As String, ByVal OutcomeFilter As NCBI.AssayOutcomeFilterType) As <System.ServiceModel.MessageParameterAttribute(Name:="AssayKey")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputAssay", ReplyAction:="*")>  _
        Function InputAssayAsync(ByVal AID As Integer, ByVal Columns As NCBI.AssayColumnsType, ByVal ListKeyTIDs As String, ByVal ListKeySCIDs As String, ByVal OutcomeFilter As NCBI.AssayOutcomeFilterType) As <System.ServiceModel.MessageParameterAttribute(Name:="AssayKey")> System.Threading.Tasks.Task(Of String)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputEntrez", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function InputEntrez(ByVal EntrezKey As NCBI.EntrezKey) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputEntrez", ReplyAction:="*")>  _
        Function InputEntrezAsync(ByVal EntrezKey As NCBI.EntrezKey) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> System.Threading.Tasks.Task(Of String)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputList", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function InputList(ByVal ids() As Integer, ByVal idType As NCBI.PCIDType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputList", ReplyAction:="*")>  _
        Function InputListAsync(ByVal ids() As Integer, ByVal idType As NCBI.PCIDType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> System.Threading.Tasks.Task(Of String)
        
        'CODEGEN: Parameter 'strids' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlArrayItemAttribute'.
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputListString", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function InputListString(ByVal request As NCBI.InputListStringRequest) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> NCBI.InputListStringResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputListString", ReplyAction:="*")>  _
        Function InputListStringAsync(ByVal request As NCBI.InputListStringRequest) As System.Threading.Tasks.Task(Of NCBI.InputListStringResponse)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputListText", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function InputListText(ByVal ids As String, ByVal idType As NCBI.PCIDType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputListText", ReplyAction:="*")>  _
        Function InputListTextAsync(ByVal ids As String, ByVal idType As NCBI.PCIDType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> System.Threading.Tasks.Task(Of String)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputStructure", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function InputStructure(ByVal [structure] As String, ByVal format As NCBI.FormatType) As <System.ServiceModel.MessageParameterAttribute(Name:="StrKey")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputStructure", ReplyAction:="*")>  _
        Function InputStructureAsync(ByVal [structure] As String, ByVal format As NCBI.FormatType) As <System.ServiceModel.MessageParameterAttribute(Name:="StrKey")> System.Threading.Tasks.Task(Of String)
        
        'CODEGEN: Parameter 'structure' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputStructureBase64", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function InputStructureBase64(ByVal request As NCBI.InputStructureBase64Request) As <System.ServiceModel.MessageParameterAttribute(Name:="StrKey")> NCBI.InputStructureBase64Response
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/InputStructureBase64", ReplyAction:="*")>  _
        Function InputStructureBase64Async(ByVal request As NCBI.InputStructureBase64Request) As System.Threading.Tasks.Task(Of NCBI.InputStructureBase64Response)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/MFSearch", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function MFSearch(ByVal MF As String, ByVal mfOptions As NCBI.MFSearchOptions, ByVal limits As NCBI.LimitsType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/MFSearch", ReplyAction:="*")>  _
        Function MFSearchAsync(ByVal MF As String, ByVal mfOptions As NCBI.MFSearchOptions, ByVal limits As NCBI.LimitsType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> System.Threading.Tasks.Task(Of String)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/ScoreMatrix", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function ScoreMatrix(ByVal ListKey As String, ByVal SecondaryListKey As String, ByVal ScoreType As NCBI.ScoreTypeType, ByVal MatrixFormat As NCBI.MatrixFormatType, ByVal eCompress As NCBI.CompressType, ByVal N3DConformers As Integer, ByVal No3DParent As Boolean) As <System.ServiceModel.MessageParameterAttribute(Name:="DownloadKey")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/ScoreMatrix", ReplyAction:="*")>  _
        Function ScoreMatrixAsync(ByVal ListKey As String, ByVal SecondaryListKey As String, ByVal ScoreType As NCBI.ScoreTypeType, ByVal MatrixFormat As NCBI.MatrixFormatType, ByVal eCompress As NCBI.CompressType, ByVal N3DConformers As Integer, ByVal No3DParent As Boolean) As <System.ServiceModel.MessageParameterAttribute(Name:="DownloadKey")> System.Threading.Tasks.Task(Of String)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/SimilaritySearch2D", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function SimilaritySearch2D(ByVal StrKey As String, ByVal simOptions As NCBI.SimilaritySearchOptions, ByVal limits As NCBI.LimitsType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/SimilaritySearch2D", ReplyAction:="*")>  _
        Function SimilaritySearch2DAsync(ByVal StrKey As String, ByVal simOptions As NCBI.SimilaritySearchOptions, ByVal limits As NCBI.LimitsType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> System.Threading.Tasks.Task(Of String)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/Standardize", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function Standardize(ByVal request As NCBI.StandardizeRequest) As NCBI.StandardizeResponse
        
        'CODEGEN: Generating message contract since the operation has multiple return values.
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/Standardize", ReplyAction:="*")>  _
        Function StandardizeAsync(ByVal request As NCBI.StandardizeRequest) As System.Threading.Tasks.Task(Of NCBI.StandardizeResponse)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/SubstructureSearch", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function SubstructureSearch(ByVal StrKey As String, ByVal ssOptions As NCBI.StructureSearchOptions, ByVal limits As NCBI.LimitsType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/SubstructureSearch", ReplyAction:="*")>  _
        Function SubstructureSearchAsync(ByVal StrKey As String, ByVal ssOptions As NCBI.StructureSearchOptions, ByVal limits As NCBI.LimitsType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> System.Threading.Tasks.Task(Of String)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/SuperstructureSearch", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function SuperstructureSearch(ByVal StrKey As String, ByVal ssOptions As NCBI.StructureSearchOptions, ByVal limits As NCBI.LimitsType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://pubchem.ncbi.nlm.nih.gov/SuperstructureSearch", ReplyAction:="*")>  _
        Function SuperstructureSearchAsync(ByVal StrKey As String, ByVal ssOptions As NCBI.StructureSearchOptions, ByVal limits As NCBI.LimitsType) As <System.ServiceModel.MessageParameterAttribute(Name:="ListKey")> System.Threading.Tasks.Task(Of String)
    End Interface
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum AssayFormatType
        
        '''<remarks/>
        eAssayFormat_XML
        
        '''<remarks/>
        eAssayFormat_ASN_Text
        
        '''<remarks/>
        eAssayFormat_ASN_Binary
        
        '''<remarks/>
        eAssayFormat_CSV
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum CompressType
        
        '''<remarks/>
        eCompress_None
        
        '''<remarks/>
        eCompress_GZip
        
        '''<remarks/>
        eCompress_BZip2
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum FormatType
        
        '''<remarks/>
        eFormat_ASNB
        
        '''<remarks/>
        eFormat_ASNT
        
        '''<remarks/>
        eFormat_XML
        
        '''<remarks/>
        eFormat_SDF
        
        '''<remarks/>
        eFormat_SMILES
        
        '''<remarks/>
        eFormat_InChI
        
        '''<remarks/>
        eFormat_Image
        
        '''<remarks/>
        eFormat_Thumbnail
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Partial Public Class DataBlobType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dataField() As Byte
        
        Private blobFormatField As BlobFormatType
        
        Private blobFormatFieldSpecified As Boolean
        
        Private eCompressField As CompressType
        
        Private eCompressFieldSpecified As Boolean
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary", Order:=0)>  _
        Public Property Data() As Byte()
            Get
                Return Me.dataField
            End Get
            Set
                Me.dataField = value
                Me.RaisePropertyChanged("Data")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property BlobFormat() As BlobFormatType
            Get
                Return Me.blobFormatField
            End Get
            Set
                Me.blobFormatField = value
                Me.RaisePropertyChanged("BlobFormat")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property BlobFormatSpecified() As Boolean
            Get
                Return Me.blobFormatFieldSpecified
            End Get
            Set
                Me.blobFormatFieldSpecified = value
                Me.RaisePropertyChanged("BlobFormatSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property eCompress() As CompressType
            Get
                Return Me.eCompressField
            End Get
            Set
                Me.eCompressField = value
                Me.RaisePropertyChanged("eCompress")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property eCompressSpecified() As Boolean
            Get
                Return Me.eCompressFieldSpecified
            End Get
            Set
                Me.eCompressFieldSpecified = value
                Me.RaisePropertyChanged("eCompressSpecified")
            End Set
        End Property
        
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        
        Protected Sub RaisePropertyChanged(ByVal propertyName As String)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum BlobFormatType
        
        '''<remarks/>
        eBlobFormat_Unspecified
        
        '''<remarks/>
        eBlobFormat_ASNB
        
        '''<remarks/>
        eBlobFormat_ASNT
        
        '''<remarks/>
        eBlobFormat_XML
        
        '''<remarks/>
        eBlobFormat_SDF
        
        '''<remarks/>
        eBlobFormat_CSV
        
        '''<remarks/>
        eBlobFormat_Text
        
        '''<remarks/>
        eBlobFormat_HTML
        
        '''<remarks/>
        eBlobFormat_PNG
        
        '''<remarks/>
        eBlobFormat_Other
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Partial Public Class StructureSearchOptions
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private matchIsotopesField As Boolean
        
        Private matchIsotopesFieldSpecified As Boolean
        
        Private matchChargesField As Boolean
        
        Private matchChargesFieldSpecified As Boolean
        
        Private matchTautomersField As Boolean
        
        Private matchTautomersFieldSpecified As Boolean
        
        Private ringsNotEmbeddedField As Boolean
        
        Private ringsNotEmbeddedFieldSpecified As Boolean
        
        Private singeDoubleBondsMatchField As Boolean
        
        Private singeDoubleBondsMatchFieldSpecified As Boolean
        
        Private chainsMatchRingsField As Boolean
        
        Private chainsMatchRingsFieldSpecified As Boolean
        
        Private stripHydrogenField As Boolean
        
        Private stripHydrogenFieldSpecified As Boolean
        
        Private eStereoField As StereoType
        
        Private eStereoFieldSpecified As Boolean
        
        Private toWebEnvField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property MatchIsotopes() As Boolean
            Get
                Return Me.matchIsotopesField
            End Get
            Set
                Me.matchIsotopesField = value
                Me.RaisePropertyChanged("MatchIsotopes")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property MatchIsotopesSpecified() As Boolean
            Get
                Return Me.matchIsotopesFieldSpecified
            End Get
            Set
                Me.matchIsotopesFieldSpecified = value
                Me.RaisePropertyChanged("MatchIsotopesSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property MatchCharges() As Boolean
            Get
                Return Me.matchChargesField
            End Get
            Set
                Me.matchChargesField = value
                Me.RaisePropertyChanged("MatchCharges")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property MatchChargesSpecified() As Boolean
            Get
                Return Me.matchChargesFieldSpecified
            End Get
            Set
                Me.matchChargesFieldSpecified = value
                Me.RaisePropertyChanged("MatchChargesSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property MatchTautomers() As Boolean
            Get
                Return Me.matchTautomersField
            End Get
            Set
                Me.matchTautomersField = value
                Me.RaisePropertyChanged("MatchTautomers")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property MatchTautomersSpecified() As Boolean
            Get
                Return Me.matchTautomersFieldSpecified
            End Get
            Set
                Me.matchTautomersFieldSpecified = value
                Me.RaisePropertyChanged("MatchTautomersSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property RingsNotEmbedded() As Boolean
            Get
                Return Me.ringsNotEmbeddedField
            End Get
            Set
                Me.ringsNotEmbeddedField = value
                Me.RaisePropertyChanged("RingsNotEmbedded")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property RingsNotEmbeddedSpecified() As Boolean
            Get
                Return Me.ringsNotEmbeddedFieldSpecified
            End Get
            Set
                Me.ringsNotEmbeddedFieldSpecified = value
                Me.RaisePropertyChanged("RingsNotEmbeddedSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property SingeDoubleBondsMatch() As Boolean
            Get
                Return Me.singeDoubleBondsMatchField
            End Get
            Set
                Me.singeDoubleBondsMatchField = value
                Me.RaisePropertyChanged("SingeDoubleBondsMatch")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property SingeDoubleBondsMatchSpecified() As Boolean
            Get
                Return Me.singeDoubleBondsMatchFieldSpecified
            End Get
            Set
                Me.singeDoubleBondsMatchFieldSpecified = value
                Me.RaisePropertyChanged("SingeDoubleBondsMatchSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=5)>  _
        Public Property ChainsMatchRings() As Boolean
            Get
                Return Me.chainsMatchRingsField
            End Get
            Set
                Me.chainsMatchRingsField = value
                Me.RaisePropertyChanged("ChainsMatchRings")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property ChainsMatchRingsSpecified() As Boolean
            Get
                Return Me.chainsMatchRingsFieldSpecified
            End Get
            Set
                Me.chainsMatchRingsFieldSpecified = value
                Me.RaisePropertyChanged("ChainsMatchRingsSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=6)>  _
        Public Property StripHydrogen() As Boolean
            Get
                Return Me.stripHydrogenField
            End Get
            Set
                Me.stripHydrogenField = value
                Me.RaisePropertyChanged("StripHydrogen")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property StripHydrogenSpecified() As Boolean
            Get
                Return Me.stripHydrogenFieldSpecified
            End Get
            Set
                Me.stripHydrogenFieldSpecified = value
                Me.RaisePropertyChanged("StripHydrogenSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=7)>  _
        Public Property eStereo() As StereoType
            Get
                Return Me.eStereoField
            End Get
            Set
                Me.eStereoField = value
                Me.RaisePropertyChanged("eStereo")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property eStereoSpecified() As Boolean
            Get
                Return Me.eStereoFieldSpecified
            End Get
            Set
                Me.eStereoFieldSpecified = value
                Me.RaisePropertyChanged("eStereoSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=8)>  _
        Public Property ToWebEnv() As String
            Get
                Return Me.toWebEnvField
            End Get
            Set
                Me.toWebEnvField = value
                Me.RaisePropertyChanged("ToWebEnv")
            End Set
        End Property
        
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        
        Protected Sub RaisePropertyChanged(ByVal propertyName As String)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum StereoType
        
        '''<remarks/>
        eStereo_Ignore
        
        '''<remarks/>
        eStereo_Exact
        
        '''<remarks/>
        eStereo_Relative
        
        '''<remarks/>
        eStereo_NonConflicting
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Partial Public Class SimilaritySearchOptions
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private thresholdField As Integer
        
        Private toWebEnvField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property threshold() As Integer
            Get
                Return Me.thresholdField
            End Get
            Set
                Me.thresholdField = value
                Me.RaisePropertyChanged("threshold")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property ToWebEnv() As String
            Get
                Return Me.toWebEnvField
            End Get
            Set
                Me.toWebEnvField = value
                Me.RaisePropertyChanged("ToWebEnv")
            End Set
        End Property
        
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        
        Protected Sub RaisePropertyChanged(ByVal propertyName As String)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Partial Public Class MFSearchOptions
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private allowOtherElementsField As Boolean
        
        Private toWebEnvField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property AllowOtherElements() As Boolean
            Get
                Return Me.allowOtherElementsField
            End Get
            Set
                Me.allowOtherElementsField = value
                Me.RaisePropertyChanged("AllowOtherElements")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property ToWebEnv() As String
            Get
                Return Me.toWebEnvField
            End Get
            Set
                Me.toWebEnvField = value
                Me.RaisePropertyChanged("ToWebEnv")
            End Set
        End Property
        
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        
        Protected Sub RaisePropertyChanged(ByVal propertyName As String)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Partial Public Class LimitsType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private secondsField As Integer
        
        Private secondsFieldSpecified As Boolean
        
        Private maxRecordsField As Integer
        
        Private maxRecordsFieldSpecified As Boolean
        
        Private listKeyField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property seconds() As Integer
            Get
                Return Me.secondsField
            End Get
            Set
                Me.secondsField = value
                Me.RaisePropertyChanged("seconds")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property secondsSpecified() As Boolean
            Get
                Return Me.secondsFieldSpecified
            End Get
            Set
                Me.secondsFieldSpecified = value
                Me.RaisePropertyChanged("secondsSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property maxRecords() As Integer
            Get
                Return Me.maxRecordsField
            End Get
            Set
                Me.maxRecordsField = value
                Me.RaisePropertyChanged("maxRecords")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property maxRecordsSpecified() As Boolean
            Get
                Return Me.maxRecordsFieldSpecified
            End Get
            Set
                Me.maxRecordsFieldSpecified = value
                Me.RaisePropertyChanged("maxRecordsSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property ListKey() As String
            Get
                Return Me.listKeyField
            End Get
            Set
                Me.listKeyField = value
                Me.RaisePropertyChanged("ListKey")
            End Set
        End Property
        
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        
        Protected Sub RaisePropertyChanged(ByVal propertyName As String)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Partial Public Class IdentitySearchOptions
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private eIdentityField As IdentityType
        
        Private toWebEnvField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property eIdentity() As IdentityType
            Get
                Return Me.eIdentityField
            End Get
            Set
                Me.eIdentityField = value
                Me.RaisePropertyChanged("eIdentity")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property ToWebEnv() As String
            Get
                Return Me.toWebEnvField
            End Get
            Set
                Me.toWebEnvField = value
                Me.RaisePropertyChanged("ToWebEnv")
            End Set
        End Property
        
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        
        Protected Sub RaisePropertyChanged(ByVal propertyName As String)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum IdentityType
        
        '''<remarks/>
        eIdentity_SameConnectivity
        
        '''<remarks/>
        eIdentity_AnyTautomer
        
        '''<remarks/>
        eIdentity_SameStereo
        
        '''<remarks/>
        eIdentity_SameIsotope
        
        '''<remarks/>
        eIdentity_SameStereoIsotope
        
        '''<remarks/>
        eIdentity_SameNonconflictStereo
        
        '''<remarks/>
        eIdentity_SameIsotopeNonconflictStereo
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Partial Public Class EntrezKey
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dbField As String
        
        Private keyField As String
        
        Private webenvField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property db() As String
            Get
                Return Me.dbField
            End Get
            Set
                Me.dbField = value
                Me.RaisePropertyChanged("db")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property key() As String
            Get
                Return Me.keyField
            End Get
            Set
                Me.keyField = value
                Me.RaisePropertyChanged("key")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property webenv() As String
            Get
                Return Me.webenvField
            End Get
            Set
                Me.webenvField = value
                Me.RaisePropertyChanged("webenv")
            End Set
        End Property
        
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        
        Protected Sub RaisePropertyChanged(ByVal propertyName As String)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Partial Public Class AssayTargetType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private giField As Integer
        
        Private nameField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property gi() As Integer
            Get
                Return Me.giField
            End Get
            Set
                Me.giField = value
                Me.RaisePropertyChanged("gi")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property Name() As String
            Get
                Return Me.nameField
            End Get
            Set
                Me.nameField = value
                Me.RaisePropertyChanged("Name")
            End Set
        End Property
        
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        
        Protected Sub RaisePropertyChanged(ByVal propertyName As String)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Partial Public Class AssayDescriptionType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private nameField As String
        
        Private descriptionField() As String
        
        Private protocolField() As String
        
        Private commentField() As String
        
        Private numberOfTIDsField As Integer
        
        Private hasScoreField As Boolean
        
        Private methodField As String
        
        Private targetsField() As AssayTargetType
        
        Private versionField As Integer
        
        Private versionFieldSpecified As Boolean
        
        Private revisionField As Integer
        
        Private revisionFieldSpecified As Boolean
        
        Private lastDataChangeField As Integer
        
        Private lastDataChangeFieldSpecified As Boolean
        
        Private sIDCountAllField As Integer
        
        Private sIDCountAllFieldSpecified As Boolean
        
        Private sIDCountActiveField As Integer
        
        Private sIDCountActiveFieldSpecified As Boolean
        
        Private sIDCountInactiveField As Integer
        
        Private sIDCountInactiveFieldSpecified As Boolean
        
        Private sIDCountInconclusiveField As Integer
        
        Private sIDCountInconclusiveFieldSpecified As Boolean
        
        Private sIDCountUnspecifiedField As Integer
        
        Private sIDCountUnspecifiedFieldSpecified As Boolean
        
        Private sIDCountProbeField As Integer
        
        Private sIDCountProbeFieldSpecified As Boolean
        
        Private cIDCountAllField As Integer
        
        Private cIDCountAllFieldSpecified As Boolean
        
        Private cIDCountActiveField As Integer
        
        Private cIDCountActiveFieldSpecified As Boolean
        
        Private cIDCountInactiveField As Integer
        
        Private cIDCountInactiveFieldSpecified As Boolean
        
        Private cIDCountInconclusiveField As Integer
        
        Private cIDCountInconclusiveFieldSpecified As Boolean
        
        Private cIDCountUnspecifiedField As Integer
        
        Private cIDCountUnspecifiedFieldSpecified As Boolean
        
        Private cIDCountProbeField As Integer
        
        Private cIDCountProbeFieldSpecified As Boolean
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property Name() As String
            Get
                Return Me.nameField
            End Get
            Set
                Me.nameField = value
                Me.RaisePropertyChanged("Name")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(Order:=1),  _
         System.Xml.Serialization.XmlArrayItemAttribute(IsNullable:=false)>  _
        Public Property Description() As String()
            Get
                Return Me.descriptionField
            End Get
            Set
                Me.descriptionField = value
                Me.RaisePropertyChanged("Description")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(Order:=2),  _
         System.Xml.Serialization.XmlArrayItemAttribute(IsNullable:=false)>  _
        Public Property Protocol() As String()
            Get
                Return Me.protocolField
            End Get
            Set
                Me.protocolField = value
                Me.RaisePropertyChanged("Protocol")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(Order:=3),  _
         System.Xml.Serialization.XmlArrayItemAttribute(IsNullable:=false)>  _
        Public Property Comment() As String()
            Get
                Return Me.commentField
            End Get
            Set
                Me.commentField = value
                Me.RaisePropertyChanged("Comment")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property NumberOfTIDs() As Integer
            Get
                Return Me.numberOfTIDsField
            End Get
            Set
                Me.numberOfTIDsField = value
                Me.RaisePropertyChanged("NumberOfTIDs")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=5)>  _
        Public Property HasScore() As Boolean
            Get
                Return Me.hasScoreField
            End Get
            Set
                Me.hasScoreField = value
                Me.RaisePropertyChanged("HasScore")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=6)>  _
        Public Property Method() As String
            Get
                Return Me.methodField
            End Get
            Set
                Me.methodField = value
                Me.RaisePropertyChanged("Method")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(Order:=7),  _
         System.Xml.Serialization.XmlArrayItemAttribute("Target", IsNullable:=false)>  _
        Public Property Targets() As AssayTargetType()
            Get
                Return Me.targetsField
            End Get
            Set
                Me.targetsField = value
                Me.RaisePropertyChanged("Targets")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=8)>  _
        Public Property Version() As Integer
            Get
                Return Me.versionField
            End Get
            Set
                Me.versionField = value
                Me.RaisePropertyChanged("Version")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property VersionSpecified() As Boolean
            Get
                Return Me.versionFieldSpecified
            End Get
            Set
                Me.versionFieldSpecified = value
                Me.RaisePropertyChanged("VersionSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=9)>  _
        Public Property Revision() As Integer
            Get
                Return Me.revisionField
            End Get
            Set
                Me.revisionField = value
                Me.RaisePropertyChanged("Revision")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property RevisionSpecified() As Boolean
            Get
                Return Me.revisionFieldSpecified
            End Get
            Set
                Me.revisionFieldSpecified = value
                Me.RaisePropertyChanged("RevisionSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=10)>  _
        Public Property LastDataChange() As Integer
            Get
                Return Me.lastDataChangeField
            End Get
            Set
                Me.lastDataChangeField = value
                Me.RaisePropertyChanged("LastDataChange")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property LastDataChangeSpecified() As Boolean
            Get
                Return Me.lastDataChangeFieldSpecified
            End Get
            Set
                Me.lastDataChangeFieldSpecified = value
                Me.RaisePropertyChanged("LastDataChangeSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=11)>  _
        Public Property SIDCountAll() As Integer
            Get
                Return Me.sIDCountAllField
            End Get
            Set
                Me.sIDCountAllField = value
                Me.RaisePropertyChanged("SIDCountAll")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property SIDCountAllSpecified() As Boolean
            Get
                Return Me.sIDCountAllFieldSpecified
            End Get
            Set
                Me.sIDCountAllFieldSpecified = value
                Me.RaisePropertyChanged("SIDCountAllSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=12)>  _
        Public Property SIDCountActive() As Integer
            Get
                Return Me.sIDCountActiveField
            End Get
            Set
                Me.sIDCountActiveField = value
                Me.RaisePropertyChanged("SIDCountActive")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property SIDCountActiveSpecified() As Boolean
            Get
                Return Me.sIDCountActiveFieldSpecified
            End Get
            Set
                Me.sIDCountActiveFieldSpecified = value
                Me.RaisePropertyChanged("SIDCountActiveSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=13)>  _
        Public Property SIDCountInactive() As Integer
            Get
                Return Me.sIDCountInactiveField
            End Get
            Set
                Me.sIDCountInactiveField = value
                Me.RaisePropertyChanged("SIDCountInactive")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property SIDCountInactiveSpecified() As Boolean
            Get
                Return Me.sIDCountInactiveFieldSpecified
            End Get
            Set
                Me.sIDCountInactiveFieldSpecified = value
                Me.RaisePropertyChanged("SIDCountInactiveSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=14)>  _
        Public Property SIDCountInconclusive() As Integer
            Get
                Return Me.sIDCountInconclusiveField
            End Get
            Set
                Me.sIDCountInconclusiveField = value
                Me.RaisePropertyChanged("SIDCountInconclusive")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property SIDCountInconclusiveSpecified() As Boolean
            Get
                Return Me.sIDCountInconclusiveFieldSpecified
            End Get
            Set
                Me.sIDCountInconclusiveFieldSpecified = value
                Me.RaisePropertyChanged("SIDCountInconclusiveSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=15)>  _
        Public Property SIDCountUnspecified() As Integer
            Get
                Return Me.sIDCountUnspecifiedField
            End Get
            Set
                Me.sIDCountUnspecifiedField = value
                Me.RaisePropertyChanged("SIDCountUnspecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property SIDCountUnspecifiedSpecified() As Boolean
            Get
                Return Me.sIDCountUnspecifiedFieldSpecified
            End Get
            Set
                Me.sIDCountUnspecifiedFieldSpecified = value
                Me.RaisePropertyChanged("SIDCountUnspecifiedSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=16)>  _
        Public Property SIDCountProbe() As Integer
            Get
                Return Me.sIDCountProbeField
            End Get
            Set
                Me.sIDCountProbeField = value
                Me.RaisePropertyChanged("SIDCountProbe")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property SIDCountProbeSpecified() As Boolean
            Get
                Return Me.sIDCountProbeFieldSpecified
            End Get
            Set
                Me.sIDCountProbeFieldSpecified = value
                Me.RaisePropertyChanged("SIDCountProbeSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=17)>  _
        Public Property CIDCountAll() As Integer
            Get
                Return Me.cIDCountAllField
            End Get
            Set
                Me.cIDCountAllField = value
                Me.RaisePropertyChanged("CIDCountAll")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property CIDCountAllSpecified() As Boolean
            Get
                Return Me.cIDCountAllFieldSpecified
            End Get
            Set
                Me.cIDCountAllFieldSpecified = value
                Me.RaisePropertyChanged("CIDCountAllSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=18)>  _
        Public Property CIDCountActive() As Integer
            Get
                Return Me.cIDCountActiveField
            End Get
            Set
                Me.cIDCountActiveField = value
                Me.RaisePropertyChanged("CIDCountActive")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property CIDCountActiveSpecified() As Boolean
            Get
                Return Me.cIDCountActiveFieldSpecified
            End Get
            Set
                Me.cIDCountActiveFieldSpecified = value
                Me.RaisePropertyChanged("CIDCountActiveSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=19)>  _
        Public Property CIDCountInactive() As Integer
            Get
                Return Me.cIDCountInactiveField
            End Get
            Set
                Me.cIDCountInactiveField = value
                Me.RaisePropertyChanged("CIDCountInactive")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property CIDCountInactiveSpecified() As Boolean
            Get
                Return Me.cIDCountInactiveFieldSpecified
            End Get
            Set
                Me.cIDCountInactiveFieldSpecified = value
                Me.RaisePropertyChanged("CIDCountInactiveSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=20)>  _
        Public Property CIDCountInconclusive() As Integer
            Get
                Return Me.cIDCountInconclusiveField
            End Get
            Set
                Me.cIDCountInconclusiveField = value
                Me.RaisePropertyChanged("CIDCountInconclusive")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property CIDCountInconclusiveSpecified() As Boolean
            Get
                Return Me.cIDCountInconclusiveFieldSpecified
            End Get
            Set
                Me.cIDCountInconclusiveFieldSpecified = value
                Me.RaisePropertyChanged("CIDCountInconclusiveSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=21)>  _
        Public Property CIDCountUnspecified() As Integer
            Get
                Return Me.cIDCountUnspecifiedField
            End Get
            Set
                Me.cIDCountUnspecifiedField = value
                Me.RaisePropertyChanged("CIDCountUnspecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property CIDCountUnspecifiedSpecified() As Boolean
            Get
                Return Me.cIDCountUnspecifiedFieldSpecified
            End Get
            Set
                Me.cIDCountUnspecifiedFieldSpecified = value
                Me.RaisePropertyChanged("CIDCountUnspecifiedSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=22)>  _
        Public Property CIDCountProbe() As Integer
            Get
                Return Me.cIDCountProbeField
            End Get
            Set
                Me.cIDCountProbeField = value
                Me.RaisePropertyChanged("CIDCountProbe")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property CIDCountProbeSpecified() As Boolean
            Get
                Return Me.cIDCountProbeFieldSpecified
            End Get
            Set
                Me.cIDCountProbeFieldSpecified = value
                Me.RaisePropertyChanged("CIDCountProbeSpecified")
            End Set
        End Property
        
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        
        Protected Sub RaisePropertyChanged(ByVal propertyName As String)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Partial Public Class TestedConcentrationType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private concentrationField As Double
        
        Private unitField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property Concentration() As Double
            Get
                Return Me.concentrationField
            End Get
            Set
                Me.concentrationField = value
                Me.RaisePropertyChanged("Concentration")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property Unit() As String
            Get
                Return Me.unitField
            End Get
            Set
                Me.unitField = value
                Me.RaisePropertyChanged("Unit")
            End Set
        End Property
        
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        
        Protected Sub RaisePropertyChanged(ByVal propertyName As String)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Partial Public Class ColumnDescriptionType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private headingField As HeadingType
        
        Private tIDField As Integer
        
        Private tIDFieldSpecified As Boolean
        
        Private nameField As String
        
        Private descriptionField() As String
        
        Private typeField As String
        
        Private unitField As String
        
        Private testedConcentrationField As TestedConcentrationType
        
        Private activeConcentrationField As Boolean
        
        Private activeConcentrationFieldSpecified As Boolean
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property Heading() As HeadingType
            Get
                Return Me.headingField
            End Get
            Set
                Me.headingField = value
                Me.RaisePropertyChanged("Heading")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property TID() As Integer
            Get
                Return Me.tIDField
            End Get
            Set
                Me.tIDField = value
                Me.RaisePropertyChanged("TID")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property TIDSpecified() As Boolean
            Get
                Return Me.tIDFieldSpecified
            End Get
            Set
                Me.tIDFieldSpecified = value
                Me.RaisePropertyChanged("TIDSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property Name() As String
            Get
                Return Me.nameField
            End Get
            Set
                Me.nameField = value
                Me.RaisePropertyChanged("Name")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(Order:=3),  _
         System.Xml.Serialization.XmlArrayItemAttribute(IsNullable:=false)>  _
        Public Property Description() As String()
            Get
                Return Me.descriptionField
            End Get
            Set
                Me.descriptionField = value
                Me.RaisePropertyChanged("Description")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property Type() As String
            Get
                Return Me.typeField
            End Get
            Set
                Me.typeField = value
                Me.RaisePropertyChanged("Type")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=5)>  _
        Public Property Unit() As String
            Get
                Return Me.unitField
            End Get
            Set
                Me.unitField = value
                Me.RaisePropertyChanged("Unit")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=6)>  _
        Public Property TestedConcentration() As TestedConcentrationType
            Get
                Return Me.testedConcentrationField
            End Get
            Set
                Me.testedConcentrationField = value
                Me.RaisePropertyChanged("TestedConcentration")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=7)>  _
        Public Property ActiveConcentration() As Boolean
            Get
                Return Me.activeConcentrationField
            End Get
            Set
                Me.activeConcentrationField = value
                Me.RaisePropertyChanged("ActiveConcentration")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property ActiveConcentrationSpecified() As Boolean
            Get
                Return Me.activeConcentrationFieldSpecified
            End Get
            Set
                Me.activeConcentrationFieldSpecified = value
                Me.RaisePropertyChanged("ActiveConcentrationSpecified")
            End Set
        End Property
        
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        
        Protected Sub RaisePropertyChanged(ByVal propertyName As String)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum HeadingType
        
        '''<remarks/>
        TID
        
        '''<remarks/>
        outcome
        
        '''<remarks/>
        score
    End Enum
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="Download", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class DownloadRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0)>  _
        Public ListKey As String
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=1)>  _
        Public eFormat As NCBI.FormatType
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=2)>  _
        Public eCompress As NCBI.CompressType
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=3)>  _
        Public Use3D As Boolean
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=4)>  _
        Public N3DConformers As Integer
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=5)>  _
        Public SynchronousSingleRecord As Boolean
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal ListKey As String, ByVal eFormat As NCBI.FormatType, ByVal eCompress As NCBI.CompressType, ByVal Use3D As Boolean, ByVal N3DConformers As Integer, ByVal SynchronousSingleRecord As Boolean)
            MyBase.New
            Me.ListKey = ListKey
            Me.eFormat = eFormat
            Me.eCompress = eCompress
            Me.Use3D = Use3D
            Me.N3DConformers = N3DConformers
            Me.SynchronousSingleRecord = SynchronousSingleRecord
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="DownloadResponse", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class DownloadResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0)>  _
        Public DownloadKey As String
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=1)>  _
        Public DataBlob As NCBI.DataBlobType
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal DownloadKey As String, ByVal DataBlob As NCBI.DataBlobType)
            MyBase.New
            Me.DownloadKey = DownloadKey
            Me.DataBlob = DataBlob
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="GetAssayColumnDescriptions", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class GetAssayColumnDescriptionsRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0)>  _
        Public AID As Integer
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal AID As Integer)
            MyBase.New
            Me.AID = AID
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="GetAssayColumnDescriptionsResponse", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class GetAssayColumnDescriptionsResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0),  _
         System.Xml.Serialization.XmlElementAttribute("ColumnDescription")>  _
        Public ColumnDescription() As NCBI.ColumnDescriptionType
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal ColumnDescription() As NCBI.ColumnDescriptionType)
            MyBase.New
            Me.ColumnDescription = ColumnDescription
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="GetAssayDescription", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class GetAssayDescriptionRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0)>  _
        Public AID As Integer
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=1)>  _
        Public GetVersion As Boolean
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=2)>  _
        Public GetCounts As Boolean
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=3)>  _
        Public GetFullDataBlob As Boolean
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=4)>  _
        Public eFormat As NCBI.FormatType
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal AID As Integer, ByVal GetVersion As Boolean, ByVal GetCounts As Boolean, ByVal GetFullDataBlob As Boolean, ByVal eFormat As NCBI.FormatType)
            MyBase.New
            Me.AID = AID
            Me.GetVersion = GetVersion
            Me.GetCounts = GetCounts
            Me.GetFullDataBlob = GetFullDataBlob
            Me.eFormat = eFormat
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="GetAssayDescriptionResponse", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class GetAssayDescriptionResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0)>  _
        Public AssayDescription As NCBI.AssayDescriptionType
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=1)>  _
        Public DataBlob As NCBI.DataBlobType
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal AssayDescription As NCBI.AssayDescriptionType, ByVal DataBlob As NCBI.DataBlobType)
            MyBase.New
            Me.AssayDescription = AssayDescription
            Me.DataBlob = DataBlob
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum StatusType
        
        '''<remarks/>
        eStatus_Unknown
        
        '''<remarks/>
        eStatus_Success
        
        '''<remarks/>
        eStatus_ServerError
        
        '''<remarks/>
        eStatus_HitLimit
        
        '''<remarks/>
        eStatus_TimeLimit
        
        '''<remarks/>
        eStatus_InputError
        
        '''<remarks/>
        eStatus_DataError
        
        '''<remarks/>
        eStatus_Stopped
        
        '''<remarks/>
        eStatus_Running
        
        '''<remarks/>
        eStatus_Queued
    End Enum
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="GetStandardizedStructureBase64", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class GetStandardizedStructureBase64Request
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0)>  _
        Public StrKey As String
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=1)>  _
        Public format As NCBI.FormatType
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal StrKey As String, ByVal format As NCBI.FormatType)
            MyBase.New
            Me.StrKey = StrKey
            Me.format = format
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="GetStandardizedStructureBase64Response", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class GetStandardizedStructureBase64Response
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0),  _
         System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>  _
        Public [structure]() As Byte
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal [structure]() As Byte)
            MyBase.New
            Me.[structure] = [structure]
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum IDOperationType
        
        '''<remarks/>
        eIDOperation_Same
        
        '''<remarks/>
        eIDOperation_SameStereo
        
        '''<remarks/>
        eIDOperation_SameIsotope
        
        '''<remarks/>
        eIDOperation_SameConnectivity
        
        '''<remarks/>
        eIDOperation_SameParent
        
        '''<remarks/>
        eIDOperation_SameParentStereo
        
        '''<remarks/>
        eIDOperation_SameParentIsotope
        
        '''<remarks/>
        eIDOperation_SameParentConnectivity
        
        '''<remarks/>
        eIDOperation_Similar2D
        
        '''<remarks/>
        eIDOperation_Similar3D
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum PCIDType
        
        '''<remarks/>
        eID_CID
        
        '''<remarks/>
        eID_SID
        
        '''<remarks/>
        eID_AID
        
        '''<remarks/>
        eID_TID
        
        '''<remarks/>
        eID_ConformerID
        
        '''<remarks/>
        eID_SourceID
        
        '''<remarks/>
        eID_InChI
        
        '''<remarks/>
        eID_InChIKey
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum IDOutputFormatType
        
        '''<remarks/>
        eIDOutputFormat_Entrez
        
        '''<remarks/>
        eIDOutputFormat_FileList
        
        '''<remarks/>
        eIDOutputFormat_FilePair
    End Enum
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="IDExchange", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class IDExchangeRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0)>  _
        Public InputListKey As String
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=1)>  _
        Public Operation As NCBI.IDOperationType
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=2)>  _
        Public OutputType As NCBI.PCIDType
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=3)>  _
        Public OutputSourceName As String
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=4)>  _
        Public OutputFormat As NCBI.IDOutputFormatType
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=5)>  _
        Public ToWebEnv As String
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=6)>  _
        Public eCompress As NCBI.CompressType
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal InputListKey As String, ByVal Operation As NCBI.IDOperationType, ByVal OutputType As NCBI.PCIDType, ByVal OutputSourceName As String, ByVal OutputFormat As NCBI.IDOutputFormatType, ByVal ToWebEnv As String, ByVal eCompress As NCBI.CompressType)
            MyBase.New
            Me.InputListKey = InputListKey
            Me.Operation = Operation
            Me.OutputType = OutputType
            Me.OutputSourceName = OutputSourceName
            Me.OutputFormat = OutputFormat
            Me.ToWebEnv = ToWebEnv
            Me.eCompress = eCompress
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="IDExchangeResponse", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class IDExchangeResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0)>  _
        Public ListKey As String
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=1)>  _
        Public DownloadKey As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal ListKey As String, ByVal DownloadKey As String)
            MyBase.New
            Me.ListKey = ListKey
            Me.DownloadKey = DownloadKey
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum AssayColumnsType
        
        '''<remarks/>
        eAssayColumns_Complete
        
        '''<remarks/>
        eAssayColumns_Concise
        
        '''<remarks/>
        eAssayColumns_TIDs
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum AssayOutcomeFilterType
        
        '''<remarks/>
        eAssayOutcome_All
        
        '''<remarks/>
        eAssayOutcome_Inactive
        
        '''<remarks/>
        eAssayOutcome_Active
        
        '''<remarks/>
        eAssayOutcome_Inconclusive
        
        '''<remarks/>
        eAssayOutcome_Unspecified
    End Enum
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="InputListString", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class InputListStringRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0),  _
         System.Xml.Serialization.XmlArrayItemAttribute(IsNullable:=false)>  _
        Public strids() As String
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=1)>  _
        Public idType As NCBI.PCIDType
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=2)>  _
        Public SourceName As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal strids() As String, ByVal idType As NCBI.PCIDType, ByVal SourceName As String)
            MyBase.New
            Me.strids = strids
            Me.idType = idType
            Me.SourceName = SourceName
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="InputListStringResponse", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class InputListStringResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0)>  _
        Public ListKey As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal ListKey As String)
            MyBase.New
            Me.ListKey = ListKey
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="InputStructureBase64", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class InputStructureBase64Request
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0),  _
         System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>  _
        Public [structure]() As Byte
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=1)>  _
        Public format As NCBI.FormatType
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal [structure]() As Byte, ByVal format As NCBI.FormatType)
            MyBase.New
            Me.[structure] = [structure]
            Me.format = format
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="InputStructureBase64Response", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class InputStructureBase64Response
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0)>  _
        Public StrKey As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal StrKey As String)
            MyBase.New
            Me.StrKey = StrKey
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum ScoreTypeType
        
        '''<remarks/>
        eScoreType_Sim2DSubs
        
        '''<remarks/>
        eScoreType_ShapeOpt3D
        
        '''<remarks/>
        eScoreType_FeatureOpt3D
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/")>  _
    Public Enum MatrixFormatType
        
        '''<remarks/>
        eMatrixFormat_CSV
        
        '''<remarks/>
        eMatrixFormat_IdIdScore
    End Enum
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="Standardize", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class StandardizeRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0)>  _
        Public StrKey As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal StrKey As String)
            MyBase.New
            Me.StrKey = StrKey
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="StandardizeResponse", WrapperNamespace:="http://pubchem.ncbi.nlm.nih.gov/", IsWrapped:=true)>  _
    Partial Public Class StandardizeResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://pubchem.ncbi.nlm.nih.gov/", Order:=0)>  _
        Public StrKey As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal StrKey As String)
            MyBase.New
            Me.StrKey = StrKey
        End Sub
    End Class
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Public Interface PUGSoapChannel
        Inherits NCBI.PUGSoap, System.ServiceModel.IClientChannel
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Partial Public Class PUGSoapClient
        Inherits System.ServiceModel.ClientBase(Of NCBI.PUGSoap)
        Implements NCBI.PUGSoap
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String)
            MyBase.New(endpointConfigurationName)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As String)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal binding As System.ServiceModel.Channels.Binding, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(binding, remoteAddress)
        End Sub
        
        Public Function AssayDownload(ByVal AssayKey As String, ByVal AssayFormat As NCBI.AssayFormatType, ByVal eCompress As NCBI.CompressType) As String Implements NCBI.PUGSoap.AssayDownload
            Return MyBase.Channel.AssayDownload(AssayKey, AssayFormat, eCompress)
        End Function
        
        Public Function AssayDownloadAsync(ByVal AssayKey As String, ByVal AssayFormat As NCBI.AssayFormatType, ByVal eCompress As NCBI.CompressType) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.AssayDownloadAsync
            Return MyBase.Channel.AssayDownloadAsync(AssayKey, AssayFormat, eCompress)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_PUGSoap_Download(ByVal request As NCBI.DownloadRequest) As NCBI.DownloadResponse Implements NCBI.PUGSoap.Download
            Return MyBase.Channel.Download(request)
        End Function
        
        Public Function Download(ByVal ListKey As String, ByVal eFormat As NCBI.FormatType, ByVal eCompress As NCBI.CompressType, ByVal Use3D As Boolean, ByVal N3DConformers As Integer, ByVal SynchronousSingleRecord As Boolean, <System.Runtime.InteropServices.OutAttribute()> ByRef DataBlob As NCBI.DataBlobType) As String
            Dim inValue As NCBI.DownloadRequest = New NCBI.DownloadRequest()
            inValue.ListKey = ListKey
            inValue.eFormat = eFormat
            inValue.eCompress = eCompress
            inValue.Use3D = Use3D
            inValue.N3DConformers = N3DConformers
            inValue.SynchronousSingleRecord = SynchronousSingleRecord
            Dim retVal As NCBI.DownloadResponse = CType(Me,NCBI.PUGSoap).Download(inValue)
            DataBlob = retVal.DataBlob
            Return retVal.DownloadKey
        End Function
        
        Public Function DownloadAsync(ByVal request As NCBI.DownloadRequest) As System.Threading.Tasks.Task(Of NCBI.DownloadResponse) Implements NCBI.PUGSoap.DownloadAsync
            Return MyBase.Channel.DownloadAsync(request)
        End Function
        
        Public Function GetAssayColumnDescription(ByVal AID As Integer, ByVal Heading As NCBI.HeadingType, ByVal TID As Integer) As NCBI.ColumnDescriptionType Implements NCBI.PUGSoap.GetAssayColumnDescription
            Return MyBase.Channel.GetAssayColumnDescription(AID, Heading, TID)
        End Function
        
        Public Function GetAssayColumnDescriptionAsync(ByVal AID As Integer, ByVal Heading As NCBI.HeadingType, ByVal TID As Integer) As System.Threading.Tasks.Task(Of NCBI.ColumnDescriptionType) Implements NCBI.PUGSoap.GetAssayColumnDescriptionAsync
            Return MyBase.Channel.GetAssayColumnDescriptionAsync(AID, Heading, TID)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_PUGSoap_GetAssayColumnDescriptions(ByVal request As NCBI.GetAssayColumnDescriptionsRequest) As NCBI.GetAssayColumnDescriptionsResponse Implements NCBI.PUGSoap.GetAssayColumnDescriptions
            Return MyBase.Channel.GetAssayColumnDescriptions(request)
        End Function
        
        Public Function GetAssayColumnDescriptions(ByVal AID As Integer) As NCBI.ColumnDescriptionType()
            Dim inValue As NCBI.GetAssayColumnDescriptionsRequest = New NCBI.GetAssayColumnDescriptionsRequest()
            inValue.AID = AID
            Dim retVal As NCBI.GetAssayColumnDescriptionsResponse = CType(Me,NCBI.PUGSoap).GetAssayColumnDescriptions(inValue)
            Return retVal.ColumnDescription
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_PUGSoap_GetAssayColumnDescriptionsAsync(ByVal request As NCBI.GetAssayColumnDescriptionsRequest) As System.Threading.Tasks.Task(Of NCBI.GetAssayColumnDescriptionsResponse) Implements NCBI.PUGSoap.GetAssayColumnDescriptionsAsync
            Return MyBase.Channel.GetAssayColumnDescriptionsAsync(request)
        End Function
        
        Public Function GetAssayColumnDescriptionsAsync(ByVal AID As Integer) As System.Threading.Tasks.Task(Of NCBI.GetAssayColumnDescriptionsResponse)
            Dim inValue As NCBI.GetAssayColumnDescriptionsRequest = New NCBI.GetAssayColumnDescriptionsRequest()
            inValue.AID = AID
            Return CType(Me,NCBI.PUGSoap).GetAssayColumnDescriptionsAsync(inValue)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_PUGSoap_GetAssayDescription(ByVal request As NCBI.GetAssayDescriptionRequest) As NCBI.GetAssayDescriptionResponse Implements NCBI.PUGSoap.GetAssayDescription
            Return MyBase.Channel.GetAssayDescription(request)
        End Function
        
        Public Function GetAssayDescription(ByVal AID As Integer, ByVal GetVersion As Boolean, ByVal GetCounts As Boolean, ByVal GetFullDataBlob As Boolean, ByVal eFormat As NCBI.FormatType, <System.Runtime.InteropServices.OutAttribute()> ByRef DataBlob As NCBI.DataBlobType) As NCBI.AssayDescriptionType
            Dim inValue As NCBI.GetAssayDescriptionRequest = New NCBI.GetAssayDescriptionRequest()
            inValue.AID = AID
            inValue.GetVersion = GetVersion
            inValue.GetCounts = GetCounts
            inValue.GetFullDataBlob = GetFullDataBlob
            inValue.eFormat = eFormat
            Dim retVal As NCBI.GetAssayDescriptionResponse = CType(Me,NCBI.PUGSoap).GetAssayDescription(inValue)
            DataBlob = retVal.DataBlob
            Return retVal.AssayDescription
        End Function
        
        Public Function GetAssayDescriptionAsync(ByVal request As NCBI.GetAssayDescriptionRequest) As System.Threading.Tasks.Task(Of NCBI.GetAssayDescriptionResponse) Implements NCBI.PUGSoap.GetAssayDescriptionAsync
            Return MyBase.Channel.GetAssayDescriptionAsync(request)
        End Function
        
        Public Function GetDownloadUrl(ByVal DownloadKey As String) As String Implements NCBI.PUGSoap.GetDownloadUrl
            Return MyBase.Channel.GetDownloadUrl(DownloadKey)
        End Function
        
        Public Function GetDownloadUrlAsync(ByVal DownloadKey As String) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.GetDownloadUrlAsync
            Return MyBase.Channel.GetDownloadUrlAsync(DownloadKey)
        End Function
        
        Public Function GetEntrezKey(ByVal ListKey As String) As NCBI.EntrezKey Implements NCBI.PUGSoap.GetEntrezKey
            Return MyBase.Channel.GetEntrezKey(ListKey)
        End Function
        
        Public Function GetEntrezKeyAsync(ByVal ListKey As String) As System.Threading.Tasks.Task(Of NCBI.EntrezKey) Implements NCBI.PUGSoap.GetEntrezKeyAsync
            Return MyBase.Channel.GetEntrezKeyAsync(ListKey)
        End Function
        
        Public Function GetEntrezUrl(ByVal EntrezKey As NCBI.EntrezKey) As String Implements NCBI.PUGSoap.GetEntrezUrl
            Return MyBase.Channel.GetEntrezUrl(EntrezKey)
        End Function
        
        Public Function GetEntrezUrlAsync(ByVal EntrezKey As NCBI.EntrezKey) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.GetEntrezUrlAsync
            Return MyBase.Channel.GetEntrezUrlAsync(EntrezKey)
        End Function
        
        Public Function GetIDList(ByVal ListKey As String, ByVal Start As Integer, ByVal Count As Integer) As Integer() Implements NCBI.PUGSoap.GetIDList
            Return MyBase.Channel.GetIDList(ListKey, Start, Count)
        End Function
        
        Public Function GetIDListAsync(ByVal ListKey As String, ByVal Start As Integer, ByVal Count As Integer) As System.Threading.Tasks.Task(Of Integer()) Implements NCBI.PUGSoap.GetIDListAsync
            Return MyBase.Channel.GetIDListAsync(ListKey, Start, Count)
        End Function
        
        Public Function GetListItemsCount(ByVal ListKey As String) As Integer Implements NCBI.PUGSoap.GetListItemsCount
            Return MyBase.Channel.GetListItemsCount(ListKey)
        End Function
        
        Public Function GetListItemsCountAsync(ByVal ListKey As String) As System.Threading.Tasks.Task(Of Integer) Implements NCBI.PUGSoap.GetListItemsCountAsync
            Return MyBase.Channel.GetListItemsCountAsync(ListKey)
        End Function
        
        Public Function GetOperationStatus(ByVal AnyKey As String) As NCBI.StatusType Implements NCBI.PUGSoap.GetOperationStatus
            Return MyBase.Channel.GetOperationStatus(AnyKey)
        End Function
        
        Public Function GetOperationStatusAsync(ByVal AnyKey As String) As System.Threading.Tasks.Task(Of NCBI.StatusType) Implements NCBI.PUGSoap.GetOperationStatusAsync
            Return MyBase.Channel.GetOperationStatusAsync(AnyKey)
        End Function
        
        Public Function GetStandardizedCID(ByVal StrKey As String) As Integer Implements NCBI.PUGSoap.GetStandardizedCID
            Return MyBase.Channel.GetStandardizedCID(StrKey)
        End Function
        
        Public Function GetStandardizedCIDAsync(ByVal StrKey As String) As System.Threading.Tasks.Task(Of Integer) Implements NCBI.PUGSoap.GetStandardizedCIDAsync
            Return MyBase.Channel.GetStandardizedCIDAsync(StrKey)
        End Function
        
        Public Function GetStandardizedStructure(ByVal StrKey As String, ByVal format As NCBI.FormatType) As String Implements NCBI.PUGSoap.GetStandardizedStructure
            Return MyBase.Channel.GetStandardizedStructure(StrKey, format)
        End Function
        
        Public Function GetStandardizedStructureAsync(ByVal StrKey As String, ByVal format As NCBI.FormatType) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.GetStandardizedStructureAsync
            Return MyBase.Channel.GetStandardizedStructureAsync(StrKey, format)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_PUGSoap_GetStandardizedStructureBase64(ByVal request As NCBI.GetStandardizedStructureBase64Request) As NCBI.GetStandardizedStructureBase64Response Implements NCBI.PUGSoap.GetStandardizedStructureBase64
            Return MyBase.Channel.GetStandardizedStructureBase64(request)
        End Function
        
        Public Function GetStandardizedStructureBase64(ByVal StrKey As String, ByVal format As NCBI.FormatType) As Byte()
            Dim inValue As NCBI.GetStandardizedStructureBase64Request = New NCBI.GetStandardizedStructureBase64Request()
            inValue.StrKey = StrKey
            inValue.format = format
            Dim retVal As NCBI.GetStandardizedStructureBase64Response = CType(Me,NCBI.PUGSoap).GetStandardizedStructureBase64(inValue)
            Return retVal.[structure]
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_PUGSoap_GetStandardizedStructureBase64Async(ByVal request As NCBI.GetStandardizedStructureBase64Request) As System.Threading.Tasks.Task(Of NCBI.GetStandardizedStructureBase64Response) Implements NCBI.PUGSoap.GetStandardizedStructureBase64Async
            Return MyBase.Channel.GetStandardizedStructureBase64Async(request)
        End Function
        
        Public Function GetStandardizedStructureBase64Async(ByVal StrKey As String, ByVal format As NCBI.FormatType) As System.Threading.Tasks.Task(Of NCBI.GetStandardizedStructureBase64Response)
            Dim inValue As NCBI.GetStandardizedStructureBase64Request = New NCBI.GetStandardizedStructureBase64Request()
            inValue.StrKey = StrKey
            inValue.format = format
            Return CType(Me,NCBI.PUGSoap).GetStandardizedStructureBase64Async(inValue)
        End Function
        
        Public Function GetStatusMessage(ByVal AnyKey As String) As String Implements NCBI.PUGSoap.GetStatusMessage
            Return MyBase.Channel.GetStatusMessage(AnyKey)
        End Function
        
        Public Function GetStatusMessageAsync(ByVal AnyKey As String) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.GetStatusMessageAsync
            Return MyBase.Channel.GetStatusMessageAsync(AnyKey)
        End Function
        
        Public Function IdentitySearch(ByVal StrKey As String, ByVal idOptions As NCBI.IdentitySearchOptions, ByVal limits As NCBI.LimitsType) As String Implements NCBI.PUGSoap.IdentitySearch
            Return MyBase.Channel.IdentitySearch(StrKey, idOptions, limits)
        End Function
        
        Public Function IdentitySearchAsync(ByVal StrKey As String, ByVal idOptions As NCBI.IdentitySearchOptions, ByVal limits As NCBI.LimitsType) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.IdentitySearchAsync
            Return MyBase.Channel.IdentitySearchAsync(StrKey, idOptions, limits)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_PUGSoap_IDExchange(ByVal request As NCBI.IDExchangeRequest) As NCBI.IDExchangeResponse Implements NCBI.PUGSoap.IDExchange
            Return MyBase.Channel.IDExchange(request)
        End Function
        
        Public Function IDExchange(ByVal InputListKey As String, ByVal Operation As NCBI.IDOperationType, ByVal OutputType As NCBI.PCIDType, ByVal OutputSourceName As String, ByVal OutputFormat As NCBI.IDOutputFormatType, ByVal ToWebEnv As String, ByVal eCompress As NCBI.CompressType, <System.Runtime.InteropServices.OutAttribute()> ByRef DownloadKey As String) As String
            Dim inValue As NCBI.IDExchangeRequest = New NCBI.IDExchangeRequest()
            inValue.InputListKey = InputListKey
            inValue.Operation = Operation
            inValue.OutputType = OutputType
            inValue.OutputSourceName = OutputSourceName
            inValue.OutputFormat = OutputFormat
            inValue.ToWebEnv = ToWebEnv
            inValue.eCompress = eCompress
            Dim retVal As NCBI.IDExchangeResponse = CType(Me,NCBI.PUGSoap).IDExchange(inValue)
            DownloadKey = retVal.DownloadKey
            Return retVal.ListKey
        End Function
        
        Public Function IDExchangeAsync(ByVal request As NCBI.IDExchangeRequest) As System.Threading.Tasks.Task(Of NCBI.IDExchangeResponse) Implements NCBI.PUGSoap.IDExchangeAsync
            Return MyBase.Channel.IDExchangeAsync(request)
        End Function
        
        Public Function InputAssay(ByVal AID As Integer, ByVal Columns As NCBI.AssayColumnsType, ByVal ListKeyTIDs As String, ByVal ListKeySCIDs As String, ByVal OutcomeFilter As NCBI.AssayOutcomeFilterType) As String Implements NCBI.PUGSoap.InputAssay
            Return MyBase.Channel.InputAssay(AID, Columns, ListKeyTIDs, ListKeySCIDs, OutcomeFilter)
        End Function
        
        Public Function InputAssayAsync(ByVal AID As Integer, ByVal Columns As NCBI.AssayColumnsType, ByVal ListKeyTIDs As String, ByVal ListKeySCIDs As String, ByVal OutcomeFilter As NCBI.AssayOutcomeFilterType) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.InputAssayAsync
            Return MyBase.Channel.InputAssayAsync(AID, Columns, ListKeyTIDs, ListKeySCIDs, OutcomeFilter)
        End Function
        
        Public Function InputEntrez(ByVal EntrezKey As NCBI.EntrezKey) As String Implements NCBI.PUGSoap.InputEntrez
            Return MyBase.Channel.InputEntrez(EntrezKey)
        End Function
        
        Public Function InputEntrezAsync(ByVal EntrezKey As NCBI.EntrezKey) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.InputEntrezAsync
            Return MyBase.Channel.InputEntrezAsync(EntrezKey)
        End Function
        
        Public Function InputList(ByVal ids() As Integer, ByVal idType As NCBI.PCIDType) As String Implements NCBI.PUGSoap.InputList
            Return MyBase.Channel.InputList(ids, idType)
        End Function
        
        Public Function InputListAsync(ByVal ids() As Integer, ByVal idType As NCBI.PCIDType) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.InputListAsync
            Return MyBase.Channel.InputListAsync(ids, idType)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_PUGSoap_InputListString(ByVal request As NCBI.InputListStringRequest) As NCBI.InputListStringResponse Implements NCBI.PUGSoap.InputListString
            Return MyBase.Channel.InputListString(request)
        End Function
        
        Public Function InputListString(ByVal strids() As String, ByVal idType As NCBI.PCIDType, ByVal SourceName As String) As String
            Dim inValue As NCBI.InputListStringRequest = New NCBI.InputListStringRequest()
            inValue.strids = strids
            inValue.idType = idType
            inValue.SourceName = SourceName
            Dim retVal As NCBI.InputListStringResponse = CType(Me,NCBI.PUGSoap).InputListString(inValue)
            Return retVal.ListKey
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_PUGSoap_InputListStringAsync(ByVal request As NCBI.InputListStringRequest) As System.Threading.Tasks.Task(Of NCBI.InputListStringResponse) Implements NCBI.PUGSoap.InputListStringAsync
            Return MyBase.Channel.InputListStringAsync(request)
        End Function
        
        Public Function InputListStringAsync(ByVal strids() As String, ByVal idType As NCBI.PCIDType, ByVal SourceName As String) As System.Threading.Tasks.Task(Of NCBI.InputListStringResponse)
            Dim inValue As NCBI.InputListStringRequest = New NCBI.InputListStringRequest()
            inValue.strids = strids
            inValue.idType = idType
            inValue.SourceName = SourceName
            Return CType(Me,NCBI.PUGSoap).InputListStringAsync(inValue)
        End Function
        
        Public Function InputListText(ByVal ids As String, ByVal idType As NCBI.PCIDType) As String Implements NCBI.PUGSoap.InputListText
            Return MyBase.Channel.InputListText(ids, idType)
        End Function
        
        Public Function InputListTextAsync(ByVal ids As String, ByVal idType As NCBI.PCIDType) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.InputListTextAsync
            Return MyBase.Channel.InputListTextAsync(ids, idType)
        End Function
        
        Public Function InputStructure(ByVal [structure] As String, ByVal format As NCBI.FormatType) As String Implements NCBI.PUGSoap.InputStructure
            Return MyBase.Channel.InputStructure([structure], format)
        End Function
        
        Public Function InputStructureAsync(ByVal [structure] As String, ByVal format As NCBI.FormatType) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.InputStructureAsync
            Return MyBase.Channel.InputStructureAsync([structure], format)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_PUGSoap_InputStructureBase64(ByVal request As NCBI.InputStructureBase64Request) As NCBI.InputStructureBase64Response Implements NCBI.PUGSoap.InputStructureBase64
            Return MyBase.Channel.InputStructureBase64(request)
        End Function
        
        Public Function InputStructureBase64(ByVal [structure]() As Byte, ByVal format As NCBI.FormatType) As String
            Dim inValue As NCBI.InputStructureBase64Request = New NCBI.InputStructureBase64Request()
            inValue.[structure] = [structure]
            inValue.format = format
            Dim retVal As NCBI.InputStructureBase64Response = CType(Me,NCBI.PUGSoap).InputStructureBase64(inValue)
            Return retVal.StrKey
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_PUGSoap_InputStructureBase64Async(ByVal request As NCBI.InputStructureBase64Request) As System.Threading.Tasks.Task(Of NCBI.InputStructureBase64Response) Implements NCBI.PUGSoap.InputStructureBase64Async
            Return MyBase.Channel.InputStructureBase64Async(request)
        End Function
        
        Public Function InputStructureBase64Async(ByVal [structure]() As Byte, ByVal format As NCBI.FormatType) As System.Threading.Tasks.Task(Of NCBI.InputStructureBase64Response)
            Dim inValue As NCBI.InputStructureBase64Request = New NCBI.InputStructureBase64Request()
            inValue.[structure] = [structure]
            inValue.format = format
            Return CType(Me,NCBI.PUGSoap).InputStructureBase64Async(inValue)
        End Function
        
        Public Function MFSearch(ByVal MF As String, ByVal mfOptions As NCBI.MFSearchOptions, ByVal limits As NCBI.LimitsType) As String Implements NCBI.PUGSoap.MFSearch
            Return MyBase.Channel.MFSearch(MF, mfOptions, limits)
        End Function
        
        Public Function MFSearchAsync(ByVal MF As String, ByVal mfOptions As NCBI.MFSearchOptions, ByVal limits As NCBI.LimitsType) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.MFSearchAsync
            Return MyBase.Channel.MFSearchAsync(MF, mfOptions, limits)
        End Function
        
        Public Function ScoreMatrix(ByVal ListKey As String, ByVal SecondaryListKey As String, ByVal ScoreType As NCBI.ScoreTypeType, ByVal MatrixFormat As NCBI.MatrixFormatType, ByVal eCompress As NCBI.CompressType, ByVal N3DConformers As Integer, ByVal No3DParent As Boolean) As String Implements NCBI.PUGSoap.ScoreMatrix
            Return MyBase.Channel.ScoreMatrix(ListKey, SecondaryListKey, ScoreType, MatrixFormat, eCompress, N3DConformers, No3DParent)
        End Function
        
        Public Function ScoreMatrixAsync(ByVal ListKey As String, ByVal SecondaryListKey As String, ByVal ScoreType As NCBI.ScoreTypeType, ByVal MatrixFormat As NCBI.MatrixFormatType, ByVal eCompress As NCBI.CompressType, ByVal N3DConformers As Integer, ByVal No3DParent As Boolean) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.ScoreMatrixAsync
            Return MyBase.Channel.ScoreMatrixAsync(ListKey, SecondaryListKey, ScoreType, MatrixFormat, eCompress, N3DConformers, No3DParent)
        End Function
        
        Public Function SimilaritySearch2D(ByVal StrKey As String, ByVal simOptions As NCBI.SimilaritySearchOptions, ByVal limits As NCBI.LimitsType) As String Implements NCBI.PUGSoap.SimilaritySearch2D
            Return MyBase.Channel.SimilaritySearch2D(StrKey, simOptions, limits)
        End Function
        
        Public Function SimilaritySearch2DAsync(ByVal StrKey As String, ByVal simOptions As NCBI.SimilaritySearchOptions, ByVal limits As NCBI.LimitsType) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.SimilaritySearch2DAsync
            Return MyBase.Channel.SimilaritySearch2DAsync(StrKey, simOptions, limits)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_PUGSoap_Standardize(ByVal request As NCBI.StandardizeRequest) As NCBI.StandardizeResponse Implements NCBI.PUGSoap.Standardize
            Return MyBase.Channel.Standardize(request)
        End Function
        
        Public Sub Standardize(ByRef StrKey As String)
            Dim inValue As NCBI.StandardizeRequest = New NCBI.StandardizeRequest()
            inValue.StrKey = StrKey
            Dim retVal As NCBI.StandardizeResponse = CType(Me,NCBI.PUGSoap).Standardize(inValue)
            StrKey = retVal.StrKey
        End Sub
        
        Public Function StandardizeAsync(ByVal request As NCBI.StandardizeRequest) As System.Threading.Tasks.Task(Of NCBI.StandardizeResponse) Implements NCBI.PUGSoap.StandardizeAsync
            Return MyBase.Channel.StandardizeAsync(request)
        End Function
        
        Public Function SubstructureSearch(ByVal StrKey As String, ByVal ssOptions As NCBI.StructureSearchOptions, ByVal limits As NCBI.LimitsType) As String Implements NCBI.PUGSoap.SubstructureSearch
            Return MyBase.Channel.SubstructureSearch(StrKey, ssOptions, limits)
        End Function
        
        Public Function SubstructureSearchAsync(ByVal StrKey As String, ByVal ssOptions As NCBI.StructureSearchOptions, ByVal limits As NCBI.LimitsType) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.SubstructureSearchAsync
            Return MyBase.Channel.SubstructureSearchAsync(StrKey, ssOptions, limits)
        End Function
        
        Public Function SuperstructureSearch(ByVal StrKey As String, ByVal ssOptions As NCBI.StructureSearchOptions, ByVal limits As NCBI.LimitsType) As String Implements NCBI.PUGSoap.SuperstructureSearch
            Return MyBase.Channel.SuperstructureSearch(StrKey, ssOptions, limits)
        End Function
        
        Public Function SuperstructureSearchAsync(ByVal StrKey As String, ByVal ssOptions As NCBI.StructureSearchOptions, ByVal limits As NCBI.LimitsType) As System.Threading.Tasks.Task(Of String) Implements NCBI.PUGSoap.SuperstructureSearchAsync
            Return MyBase.Channel.SuperstructureSearchAsync(StrKey, ssOptions, limits)
        End Function
    End Class
End Namespace
