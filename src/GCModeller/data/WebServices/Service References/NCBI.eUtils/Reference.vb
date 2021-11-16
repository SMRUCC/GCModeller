#Region "Microsoft.VisualBasic::8e15e26aea56b7dc032d48ef453df4b6, data\WebServices\Service References\NCBI.eUtils\Reference.vb"

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

    '     Interface eUtilsServiceSoap
    ' 
    '         Function: run_eGquery, run_eGqueryAsync, run_eInfo, run_eInfoAsync, run_eLink
    '                   run_eLinkAsync, run_ePost, run_ePostAsync, run_eSearch, run_eSearchAsync
    '                   run_eSpell, run_eSpellAsync, run_eSummary, run_eSummaryAsync
    ' 
    '     Class eGqueryRequest
    ' 
    '         Properties: email, term, tool
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class LinkInfoType
    ' 
    '         Properties: DbTo, HtmlTag, LinkName, MenuTag, Priority
    '                     Url
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class UrlType
    ' 
    '         Properties: LNG, Value
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: RaisePropertyChanged
    ' 
    '     Enum UrlTypeLNG
    ' 
    '         DA, DE, EL, EN, ES
    '         FR, IT, IW, JA, NL
    '         NO, RU, SV, ZH
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class IdLinkSetType
    ' 
    '         Properties: Id, LinkInfo
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class IdType
    ' 
    '         Properties: HasLinkOut, HasLinkOutSpecified, HasNeighbor, HasNeighborSpecified, Value
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Enum IdTypeHasLinkOut
    ' 
    '         N, Y
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum IdTypeHasNeighbor
    ' 
    '         N, Y
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class IdCheckListType
    ' 
    '         Properties: [ERROR], Items
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class FirstCharsType
    ' 
    '         Properties: FirstChar
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class ProviderType
    ' 
    '         Properties: IconUrl, Id, Name, NameAbbr, Url
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class IconUrlType
    ' 
    '         Properties: LNG, Value
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: RaisePropertyChanged
    ' 
    '     Enum IconUrlTypeLNG
    ' 
    '         DA, DE, EL, EN, ES
    '         FR, IT, IW, JA, NL
    '         NO, RU, SV, ZH
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class ObjUrlType
    ' 
    '         Properties: Attribute, Category, IconUrl, LinkName, Provider
    '                     SubjectType, SubProvider, Url
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class IdUrlSetType
    ' 
    '         Properties: Id, Items
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class IdUrlListType
    ' 
    '         Properties: Items
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class LinkSetDbHistoryType
    ' 
    '         Properties: [ERROR], DbTo, Info, LinkName, QueryKey
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class LinkType1
    ' 
    '         Properties: Id, Score
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class LinkSetDbType
    ' 
    '         Properties: [ERROR], DbTo, Info, Link, LinkName
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class LinkSetType
    ' 
    '         Properties: [ERROR], DbFrom, IdCheckList, IdList, IdUrlList
    '                     LinkSetDb, LinkSetDbHistory, WebEnv
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class ItemType
    ' 
    '         Properties: Item, ItemContent, Name, Type
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Enum ItemTypeType
    ' 
    '         [Date], [Integer], [String], [Structure], Enumerator
    '         Flags, List, Qualifier, Unknown
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class DocSumType
    ' 
    '         Properties: Id, Item
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class WarningListType
    ' 
    '         Properties: OutputMessage, PhraseIgnored, QuotedPhraseNotFound
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class ErrorListType
    ' 
    '         Properties: FieldNotFound, PhraseNotFound
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class TermSetType
    ' 
    '         Properties: Count, Explode, Field, Term
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class TranslationType
    ' 
    '         Properties: [To], From
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class LinkType
    ' 
    '         Properties: DbTo, Description, Menu, Name
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class FieldType
    ' 
    '         Properties: Description, FullName, Hierarchy, IsDate, IsHidden
    '                     IsNumerical, IsRangable, IsTruncatable, Name, SingleToken
    '                     TermCount
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class DbInfoType
    ' 
    '         Properties: Count, DbName, Description, FieldList, LastUpdate
    '                     LinkList, MenuName
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class DbListType
    ' 
    '         Properties: Items
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class ResultItemType
    ' 
    '         Properties: Count, DbName, MenuName, Status
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class eGQueryResultType
    ' 
    '         Properties: [ERROR], ResultItem
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class Result
    ' 
    '         Properties: eGQueryResult, Term
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class run_eGqueryRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class run_eGqueryResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class eInfoRequest
    ' 
    '         Properties: db, email, tool
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class eInfoResult
    ' 
    '         Properties: [ERROR], DbInfo, DbList
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class run_eInfoRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class run_eInfoResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class eSearchRequest
    ' 
    '         Properties: datetype, db, email, field, maxdate
    '                     mindate, QueryKey, reldate, RetMax, RetStart
    '                     rettype, sort, term, tool, usehistory
    '                     WebEnv
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class eSearchResult
    ' 
    '         Properties: [ERROR], Count, ErrorList, IdList, QueryKey
    '                     QueryTranslation, RetMax, RetStart, TranslationSet, TranslationStack
    '                     WarningList, WebEnv
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class run_eSearchRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class run_eSearchResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class eSummaryRequest
    ' 
    '         Properties: db, email, id, query_key, retmax
    '                     retstart, tool, WebEnv
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class eSummaryResult
    ' 
    '         Properties: [ERROR], DocSum
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class run_eSummaryRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class run_eSummaryResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class eLinkRequest
    ' 
    '         Properties: cmd, datetype, db, dbfrom, email
    '                     id, linkname, maxdate, mindate, query_key
    '                     reldate, term, tool, WebEnv
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class eLinkResult
    ' 
    '         Properties: [ERROR], LinkSet
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class run_eLinkRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class run_eLinkResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class eSpellRequest
    ' 
    '         Properties: db, email, term, tool
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class eSpellResult
    ' 
    '         Properties: [ERROR], CorrectedQuery, Database, Query, SpelledQuery
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class SpelledQuery
    ' 
    '         Properties: Items, ItemsElementName
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Enum ItemsChoiceType
    ' 
    '         Original, Replaced
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class run_eSpellRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class run_eSpellResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class ePostRequest
    ' 
    '         Properties: db, email, id, tool, WebEnv
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class ePostResult
    ' 
    '         Properties: [ERROR], InvalidIdList, QueryKey, WebEnv
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class run_ePostRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class run_ePostResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Interface eUtilsServiceSoapChannel
    ' 
    ' 
    ' 
    '     Class eUtilsServiceSoapClient
    ' 
    '         Constructor: (+5 Overloads) Sub New
    '         Function: NCBI_eUtils_eUtilsServiceSoap_run_eGquery, NCBI_eUtils_eUtilsServiceSoap_run_eGqueryAsync, NCBI_eUtils_eUtilsServiceSoap_run_eInfo, NCBI_eUtils_eUtilsServiceSoap_run_eInfoAsync, NCBI_eUtils_eUtilsServiceSoap_run_eLink
    '                   NCBI_eUtils_eUtilsServiceSoap_run_eLinkAsync, NCBI_eUtils_eUtilsServiceSoap_run_ePost, NCBI_eUtils_eUtilsServiceSoap_run_ePostAsync, NCBI_eUtils_eUtilsServiceSoap_run_eSearch, NCBI_eUtils_eUtilsServiceSoap_run_eSearchAsync
    '                   NCBI_eUtils_eUtilsServiceSoap_run_eSpell, NCBI_eUtils_eUtilsServiceSoap_run_eSpellAsync, NCBI_eUtils_eUtilsServiceSoap_run_eSummary, NCBI_eUtils_eUtilsServiceSoap_run_eSummaryAsync, run_eGquery
    '                   run_eGqueryAsync, run_eInfo, run_eInfoAsync, run_eLink, run_eLinkAsync
    '                   run_ePost, run_ePostAsync, run_eSearch, run_eSearchAsync, run_eSpell
    '                   run_eSpellAsync, run_eSummary, run_eSummaryAsync
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


Namespace NCBI.eUtils
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.ServiceContractAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/", ConfigurationName:="NCBI.eUtils.eUtilsServiceSoap")>  _
    Public Interface eUtilsServiceSoap
        
        'CODEGEN: Generating message contract since the operation run_eGquery is neither RPC nor document wrapped.
        <System.ServiceModel.OperationContractAttribute(Action:="egquery", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function run_eGquery(ByVal request As NCBI.eUtils.run_eGqueryRequest) As NCBI.eUtils.run_eGqueryResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="egquery", ReplyAction:="*")>  _
        Function run_eGqueryAsync(ByVal request As NCBI.eUtils.run_eGqueryRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eGqueryResponse)
        
        'CODEGEN: Generating message contract since the operation run_eInfo is neither RPC nor document wrapped.
        <System.ServiceModel.OperationContractAttribute(Action:="einfo", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function run_eInfo(ByVal request As NCBI.eUtils.run_eInfoRequest) As NCBI.eUtils.run_eInfoResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="einfo", ReplyAction:="*")>  _
        Function run_eInfoAsync(ByVal request As NCBI.eUtils.run_eInfoRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eInfoResponse)
        
        'CODEGEN: Generating message contract since the operation run_eSearch is neither RPC nor document wrapped.
        <System.ServiceModel.OperationContractAttribute(Action:="esearch", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function run_eSearch(ByVal request As NCBI.eUtils.run_eSearchRequest) As NCBI.eUtils.run_eSearchResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="esearch", ReplyAction:="*")>  _
        Function run_eSearchAsync(ByVal request As NCBI.eUtils.run_eSearchRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eSearchResponse)
        
        'CODEGEN: Generating message contract since the operation run_eSummary is neither RPC nor document wrapped.
        <System.ServiceModel.OperationContractAttribute(Action:="esummary", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function run_eSummary(ByVal request As NCBI.eUtils.run_eSummaryRequest) As NCBI.eUtils.run_eSummaryResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="esummary", ReplyAction:="*")>  _
        Function run_eSummaryAsync(ByVal request As NCBI.eUtils.run_eSummaryRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eSummaryResponse)
        
        'CODEGEN: Generating message contract since the operation run_eLink is neither RPC nor document wrapped.
        <System.ServiceModel.OperationContractAttribute(Action:="elink", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function run_eLink(ByVal request As NCBI.eUtils.run_eLinkRequest) As NCBI.eUtils.run_eLinkResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="elink", ReplyAction:="*")>  _
        Function run_eLinkAsync(ByVal request As NCBI.eUtils.run_eLinkRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eLinkResponse)
        
        'CODEGEN: Generating message contract since the operation run_eSpell is neither RPC nor document wrapped.
        <System.ServiceModel.OperationContractAttribute(Action:="espell", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function run_eSpell(ByVal request As NCBI.eUtils.run_eSpellRequest) As NCBI.eUtils.run_eSpellResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="espell", ReplyAction:="*")>  _
        Function run_eSpellAsync(ByVal request As NCBI.eUtils.run_eSpellRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eSpellResponse)
        
        'CODEGEN: Generating message contract since the operation run_ePost is neither RPC nor document wrapped.
        <System.ServiceModel.OperationContractAttribute(Action:="epost", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true)>  _
        Function run_ePost(ByVal request As NCBI.eUtils.run_ePostRequest) As NCBI.eUtils.run_ePostResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="epost", ReplyAction:="*")>  _
        Function run_ePostAsync(ByVal request As NCBI.eUtils.run_ePostRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_ePostResponse)
    End Interface
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/egquery")>  _
    Partial Public Class eGqueryRequest
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private termField As String
        
        Private toolField As String
        
        Private emailField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property term() As String
            Get
                Return Me.termField
            End Get
            Set
                Me.termField = value
                Me.RaisePropertyChanged("term")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property tool() As String
            Get
                Return Me.toolField
            End Get
            Set
                Me.toolField = value
                Me.RaisePropertyChanged("tool")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property email() As String
            Get
                Return Me.emailField
            End Get
            Set
                Me.emailField = value
                Me.RaisePropertyChanged("email")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class LinkInfoType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dbToField As String
        
        Private linkNameField As String
        
        Private menuTagField As String
        
        Private htmlTagField As String
        
        Private urlField As UrlType
        
        Private priorityField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property DbTo() As String
            Get
                Return Me.dbToField
            End Get
            Set
                Me.dbToField = value
                Me.RaisePropertyChanged("DbTo")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property LinkName() As String
            Get
                Return Me.linkNameField
            End Get
            Set
                Me.linkNameField = value
                Me.RaisePropertyChanged("LinkName")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property MenuTag() As String
            Get
                Return Me.menuTagField
            End Get
            Set
                Me.menuTagField = value
                Me.RaisePropertyChanged("MenuTag")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property HtmlTag() As String
            Get
                Return Me.htmlTagField
            End Get
            Set
                Me.htmlTagField = value
                Me.RaisePropertyChanged("HtmlTag")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property Url() As UrlType
            Get
                Return Me.urlField
            End Get
            Set
                Me.urlField = value
                Me.RaisePropertyChanged("Url")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=5)>  _
        Public Property Priority() As String
            Get
                Return Me.priorityField
            End Get
            Set
                Me.priorityField = value
                Me.RaisePropertyChanged("Priority")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class UrlType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private lNGField As UrlTypeLNG
        
        Private valueField As String
        
        Public Sub New()
            MyBase.New
            Me.lNGField = UrlTypeLNG.EN
        End Sub
        
        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(UrlTypeLNG.EN)>  _
        Public Property LNG() As UrlTypeLNG
            Get
                Return Me.lNGField
            End Get
            Set
                Me.lNGField = value
                Me.RaisePropertyChanged("LNG")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlTextAttribute()>  _
        Public Property Value() As String
            Get
                Return Me.valueField
            End Get
            Set
                Me.valueField = value
                Me.RaisePropertyChanged("Value")
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
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Public Enum UrlTypeLNG
        
        '''<remarks/>
        DA
        
        '''<remarks/>
        DE
        
        '''<remarks/>
        EN
        
        '''<remarks/>
        EL
        
        '''<remarks/>
        ES
        
        '''<remarks/>
        FR
        
        '''<remarks/>
        IT
        
        '''<remarks/>
        IW
        
        '''<remarks/>
        JA
        
        '''<remarks/>
        NL
        
        '''<remarks/>
        NO
        
        '''<remarks/>
        RU
        
        '''<remarks/>
        SV
        
        '''<remarks/>
        ZH
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class IdLinkSetType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private idField As IdType
        
        Private linkInfoField() As LinkInfoType
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property Id() As IdType
            Get
                Return Me.idField
            End Get
            Set
                Me.idField = value
                Me.RaisePropertyChanged("Id")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("LinkInfo", Order:=1)>  _
        Public Property LinkInfo() As LinkInfoType()
            Get
                Return Me.linkInfoField
            End Get
            Set
                Me.linkInfoField = value
                Me.RaisePropertyChanged("LinkInfo")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class IdType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private hasLinkOutField As IdTypeHasLinkOut
        
        Private hasLinkOutFieldSpecified As Boolean
        
        Private hasNeighborField As IdTypeHasNeighbor
        
        Private hasNeighborFieldSpecified As Boolean
        
        Private valueField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()>  _
        Public Property HasLinkOut() As IdTypeHasLinkOut
            Get
                Return Me.hasLinkOutField
            End Get
            Set
                Me.hasLinkOutField = value
                Me.RaisePropertyChanged("HasLinkOut")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property HasLinkOutSpecified() As Boolean
            Get
                Return Me.hasLinkOutFieldSpecified
            End Get
            Set
                Me.hasLinkOutFieldSpecified = value
                Me.RaisePropertyChanged("HasLinkOutSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()>  _
        Public Property HasNeighbor() As IdTypeHasNeighbor
            Get
                Return Me.hasNeighborField
            End Get
            Set
                Me.hasNeighborField = value
                Me.RaisePropertyChanged("HasNeighbor")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property HasNeighborSpecified() As Boolean
            Get
                Return Me.hasNeighborFieldSpecified
            End Get
            Set
                Me.hasNeighborFieldSpecified = value
                Me.RaisePropertyChanged("HasNeighborSpecified")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlTextAttribute()>  _
        Public Property Value() As String
            Get
                Return Me.valueField
            End Get
            Set
                Me.valueField = value
                Me.RaisePropertyChanged("Value")
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
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Public Enum IdTypeHasLinkOut
        
        '''<remarks/>
        Y
        
        '''<remarks/>
        N
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Public Enum IdTypeHasNeighbor
        
        '''<remarks/>
        Y
        
        '''<remarks/>
        N
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class IdCheckListType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private itemsField() As Object
        
        Private eRRORField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("Id", GetType(IdType), Order:=0),  _
         System.Xml.Serialization.XmlElementAttribute("IdLinkSet", GetType(IdLinkSetType), Order:=0)>  _
        Public Property Items() As Object()
            Get
                Return Me.itemsField
            End Get
            Set
                Me.itemsField = value
                Me.RaisePropertyChanged("Items")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property [ERROR]() As String
            Get
                Return Me.eRRORField
            End Get
            Set
                Me.eRRORField = value
                Me.RaisePropertyChanged("ERROR")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class FirstCharsType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private firstCharField() As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("FirstChar", Order:=0)>  _
        Public Property FirstChar() As String()
            Get
                Return Me.firstCharField
            End Get
            Set
                Me.firstCharField = value
                Me.RaisePropertyChanged("FirstChar")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class ProviderType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private nameField As String
        
        Private nameAbbrField As String
        
        Private idField As IdType
        
        Private urlField As UrlType
        
        Private iconUrlField As IconUrlType
        
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
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property NameAbbr() As String
            Get
                Return Me.nameAbbrField
            End Get
            Set
                Me.nameAbbrField = value
                Me.RaisePropertyChanged("NameAbbr")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property Id() As IdType
            Get
                Return Me.idField
            End Get
            Set
                Me.idField = value
                Me.RaisePropertyChanged("Id")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property Url() As UrlType
            Get
                Return Me.urlField
            End Get
            Set
                Me.urlField = value
                Me.RaisePropertyChanged("Url")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property IconUrl() As IconUrlType
            Get
                Return Me.iconUrlField
            End Get
            Set
                Me.iconUrlField = value
                Me.RaisePropertyChanged("IconUrl")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class IconUrlType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private lNGField As IconUrlTypeLNG
        
        Private valueField As String
        
        Public Sub New()
            MyBase.New
            Me.lNGField = IconUrlTypeLNG.EN
        End Sub
        
        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(IconUrlTypeLNG.EN)>  _
        Public Property LNG() As IconUrlTypeLNG
            Get
                Return Me.lNGField
            End Get
            Set
                Me.lNGField = value
                Me.RaisePropertyChanged("LNG")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlTextAttribute()>  _
        Public Property Value() As String
            Get
                Return Me.valueField
            End Get
            Set
                Me.valueField = value
                Me.RaisePropertyChanged("Value")
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
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Public Enum IconUrlTypeLNG
        
        '''<remarks/>
        DA
        
        '''<remarks/>
        DE
        
        '''<remarks/>
        EN
        
        '''<remarks/>
        EL
        
        '''<remarks/>
        ES
        
        '''<remarks/>
        FR
        
        '''<remarks/>
        IT
        
        '''<remarks/>
        IW
        
        '''<remarks/>
        JA
        
        '''<remarks/>
        NL
        
        '''<remarks/>
        NO
        
        '''<remarks/>
        RU
        
        '''<remarks/>
        SV
        
        '''<remarks/>
        ZH
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class ObjUrlType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private urlField As UrlType
        
        Private iconUrlField As IconUrlType
        
        Private linkNameField As String
        
        Private subjectTypeField() As String
        
        Private categoryField() As String
        
        Private attributeField() As String
        
        Private providerField As ProviderType
        
        Private subProviderField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property Url() As UrlType
            Get
                Return Me.urlField
            End Get
            Set
                Me.urlField = value
                Me.RaisePropertyChanged("Url")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property IconUrl() As IconUrlType
            Get
                Return Me.iconUrlField
            End Get
            Set
                Me.iconUrlField = value
                Me.RaisePropertyChanged("IconUrl")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property LinkName() As String
            Get
                Return Me.linkNameField
            End Get
            Set
                Me.linkNameField = value
                Me.RaisePropertyChanged("LinkName")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("SubjectType", Order:=3)>  _
        Public Property SubjectType() As String()
            Get
                Return Me.subjectTypeField
            End Get
            Set
                Me.subjectTypeField = value
                Me.RaisePropertyChanged("SubjectType")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("Category", Order:=4)>  _
        Public Property Category() As String()
            Get
                Return Me.categoryField
            End Get
            Set
                Me.categoryField = value
                Me.RaisePropertyChanged("Category")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("Attribute", Order:=5)>  _
        Public Property Attribute() As String()
            Get
                Return Me.attributeField
            End Get
            Set
                Me.attributeField = value
                Me.RaisePropertyChanged("Attribute")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=6)>  _
        Public Property Provider() As ProviderType
            Get
                Return Me.providerField
            End Get
            Set
                Me.providerField = value
                Me.RaisePropertyChanged("Provider")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=7)>  _
        Public Property SubProvider() As String
            Get
                Return Me.subProviderField
            End Get
            Set
                Me.subProviderField = value
                Me.RaisePropertyChanged("SubProvider")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class IdUrlSetType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private idField As IdType
        
        Private itemsField() As Object
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property Id() As IdType
            Get
                Return Me.idField
            End Get
            Set
                Me.idField = value
                Me.RaisePropertyChanged("Id")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("Info", GetType(String), Order:=1),  _
         System.Xml.Serialization.XmlElementAttribute("ObjUrl", GetType(ObjUrlType), Order:=1)>  _
        Public Property Items() As Object()
            Get
                Return Me.itemsField
            End Get
            Set
                Me.itemsField = value
                Me.RaisePropertyChanged("Items")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class IdUrlListType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private itemsField() As Object
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("FirstChars", GetType(FirstCharsType), Order:=0),  _
         System.Xml.Serialization.XmlElementAttribute("IdUrlSet", GetType(IdUrlSetType), Order:=0)>  _
        Public Property Items() As Object()
            Get
                Return Me.itemsField
            End Get
            Set
                Me.itemsField = value
                Me.RaisePropertyChanged("Items")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class LinkSetDbHistoryType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dbToField As String
        
        Private linkNameField As String
        
        Private queryKeyField As String
        
        Private infoField As String
        
        Private eRRORField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property DbTo() As String
            Get
                Return Me.dbToField
            End Get
            Set
                Me.dbToField = value
                Me.RaisePropertyChanged("DbTo")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property LinkName() As String
            Get
                Return Me.linkNameField
            End Get
            Set
                Me.linkNameField = value
                Me.RaisePropertyChanged("LinkName")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property QueryKey() As String
            Get
                Return Me.queryKeyField
            End Get
            Set
                Me.queryKeyField = value
                Me.RaisePropertyChanged("QueryKey")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property Info() As String
            Get
                Return Me.infoField
            End Get
            Set
                Me.infoField = value
                Me.RaisePropertyChanged("Info")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property [ERROR]() As String
            Get
                Return Me.eRRORField
            End Get
            Set
                Me.eRRORField = value
                Me.RaisePropertyChanged("ERROR")
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
     System.Xml.Serialization.XmlTypeAttribute(TypeName:="LinkType", [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class LinkType1
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private idField As IdType
        
        Private scoreField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property Id() As IdType
            Get
                Return Me.idField
            End Get
            Set
                Me.idField = value
                Me.RaisePropertyChanged("Id")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property Score() As String
            Get
                Return Me.scoreField
            End Get
            Set
                Me.scoreField = value
                Me.RaisePropertyChanged("Score")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class LinkSetDbType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dbToField As String
        
        Private linkNameField As String
        
        Private linkField() As LinkType1
        
        Private infoField As String
        
        Private eRRORField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property DbTo() As String
            Get
                Return Me.dbToField
            End Get
            Set
                Me.dbToField = value
                Me.RaisePropertyChanged("DbTo")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property LinkName() As String
            Get
                Return Me.linkNameField
            End Get
            Set
                Me.linkNameField = value
                Me.RaisePropertyChanged("LinkName")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("Link", Order:=2)>  _
        Public Property Link() As LinkType1()
            Get
                Return Me.linkField
            End Get
            Set
                Me.linkField = value
                Me.RaisePropertyChanged("Link")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property Info() As String
            Get
                Return Me.infoField
            End Get
            Set
                Me.infoField = value
                Me.RaisePropertyChanged("Info")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property [ERROR]() As String
            Get
                Return Me.eRRORField
            End Get
            Set
                Me.eRRORField = value
                Me.RaisePropertyChanged("ERROR")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class LinkSetType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dbFromField As String
        
        Private idListField() As IdType
        
        Private linkSetDbField() As LinkSetDbType
        
        Private linkSetDbHistoryField() As LinkSetDbHistoryType
        
        Private webEnvField As String
        
        Private idUrlListField As IdUrlListType
        
        Private idCheckListField As IdCheckListType
        
        Private eRRORField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property DbFrom() As String
            Get
                Return Me.dbFromField
            End Get
            Set
                Me.dbFromField = value
                Me.RaisePropertyChanged("DbFrom")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(Order:=1),  _
         System.Xml.Serialization.XmlArrayItemAttribute("Id", IsNullable:=false)>  _
        Public Property IdList() As IdType()
            Get
                Return Me.idListField
            End Get
            Set
                Me.idListField = value
                Me.RaisePropertyChanged("IdList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("LinkSetDb", Order:=2)>  _
        Public Property LinkSetDb() As LinkSetDbType()
            Get
                Return Me.linkSetDbField
            End Get
            Set
                Me.linkSetDbField = value
                Me.RaisePropertyChanged("LinkSetDb")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("LinkSetDbHistory", Order:=3)>  _
        Public Property LinkSetDbHistory() As LinkSetDbHistoryType()
            Get
                Return Me.linkSetDbHistoryField
            End Get
            Set
                Me.linkSetDbHistoryField = value
                Me.RaisePropertyChanged("LinkSetDbHistory")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property WebEnv() As String
            Get
                Return Me.webEnvField
            End Get
            Set
                Me.webEnvField = value
                Me.RaisePropertyChanged("WebEnv")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=5)>  _
        Public Property IdUrlList() As IdUrlListType
            Get
                Return Me.idUrlListField
            End Get
            Set
                Me.idUrlListField = value
                Me.RaisePropertyChanged("IdUrlList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=6)>  _
        Public Property IdCheckList() As IdCheckListType
            Get
                Return Me.idCheckListField
            End Get
            Set
                Me.idCheckListField = value
                Me.RaisePropertyChanged("IdCheckList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=7)>  _
        Public Property [ERROR]() As String
            Get
                Return Me.eRRORField
            End Get
            Set
                Me.eRRORField = value
                Me.RaisePropertyChanged("ERROR")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esummary")>  _
    Partial Public Class ItemType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private itemField() As ItemType
        
        Private itemContentField As String
        
        Private nameField As String
        
        Private typeField As ItemTypeType
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("Item", Order:=0)>  _
        Public Property Item() As ItemType()
            Get
                Return Me.itemField
            End Get
            Set
                Me.itemField = value
                Me.RaisePropertyChanged("Item")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property ItemContent() As String
            Get
                Return Me.itemContentField
            End Get
            Set
                Me.itemContentField = value
                Me.RaisePropertyChanged("ItemContent")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()>  _
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
        <System.Xml.Serialization.XmlAttributeAttribute()>  _
        Public Property Type() As ItemTypeType
            Get
                Return Me.typeField
            End Get
            Set
                Me.typeField = value
                Me.RaisePropertyChanged("Type")
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
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esummary")>  _
    Public Enum ItemTypeType
        
        '''<remarks/>
        [Integer]
        
        '''<remarks/>
        [Date]
        
        '''<remarks/>
        [String]
        
        '''<remarks/>
        [Structure]
        
        '''<remarks/>
        List
        
        '''<remarks/>
        Flags
        
        '''<remarks/>
        Qualifier
        
        '''<remarks/>
        Enumerator
        
        '''<remarks/>
        Unknown
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esummary")>  _
    Partial Public Class DocSumType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private idField As String
        
        Private itemField() As ItemType
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property Id() As String
            Get
                Return Me.idField
            End Get
            Set
                Me.idField = value
                Me.RaisePropertyChanged("Id")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("Item", Order:=1)>  _
        Public Property Item() As ItemType()
            Get
                Return Me.itemField
            End Get
            Set
                Me.itemField = value
                Me.RaisePropertyChanged("Item")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esearch")>  _
    Partial Public Class WarningListType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private phraseIgnoredField() As String
        
        Private quotedPhraseNotFoundField() As String
        
        Private outputMessageField() As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("PhraseIgnored", Order:=0)>  _
        Public Property PhraseIgnored() As String()
            Get
                Return Me.phraseIgnoredField
            End Get
            Set
                Me.phraseIgnoredField = value
                Me.RaisePropertyChanged("PhraseIgnored")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("QuotedPhraseNotFound", Order:=1)>  _
        Public Property QuotedPhraseNotFound() As String()
            Get
                Return Me.quotedPhraseNotFoundField
            End Get
            Set
                Me.quotedPhraseNotFoundField = value
                Me.RaisePropertyChanged("QuotedPhraseNotFound")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("OutputMessage", Order:=2)>  _
        Public Property OutputMessage() As String()
            Get
                Return Me.outputMessageField
            End Get
            Set
                Me.outputMessageField = value
                Me.RaisePropertyChanged("OutputMessage")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esearch")>  _
    Partial Public Class ErrorListType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private phraseNotFoundField() As String
        
        Private fieldNotFoundField() As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("PhraseNotFound", Order:=0)>  _
        Public Property PhraseNotFound() As String()
            Get
                Return Me.phraseNotFoundField
            End Get
            Set
                Me.phraseNotFoundField = value
                Me.RaisePropertyChanged("PhraseNotFound")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("FieldNotFound", Order:=1)>  _
        Public Property FieldNotFound() As String()
            Get
                Return Me.fieldNotFoundField
            End Get
            Set
                Me.fieldNotFoundField = value
                Me.RaisePropertyChanged("FieldNotFound")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esearch")>  _
    Partial Public Class TermSetType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private termField As String
        
        Private fieldField As String
        
        Private countField As String
        
        Private explodeField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property Term() As String
            Get
                Return Me.termField
            End Get
            Set
                Me.termField = value
                Me.RaisePropertyChanged("Term")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property Field() As String
            Get
                Return Me.fieldField
            End Get
            Set
                Me.fieldField = value
                Me.RaisePropertyChanged("Field")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property Count() As String
            Get
                Return Me.countField
            End Get
            Set
                Me.countField = value
                Me.RaisePropertyChanged("Count")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property Explode() As String
            Get
                Return Me.explodeField
            End Get
            Set
                Me.explodeField = value
                Me.RaisePropertyChanged("Explode")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esearch")>  _
    Partial Public Class TranslationType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private fromField As String
        
        Private toField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property From() As String
            Get
                Return Me.fromField
            End Get
            Set
                Me.fromField = value
                Me.RaisePropertyChanged("From")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property [To]() As String
            Get
                Return Me.toField
            End Get
            Set
                Me.toField = value
                Me.RaisePropertyChanged("To")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/einfo")>  _
    Partial Public Class LinkType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private nameField As String
        
        Private menuField As String
        
        Private descriptionField As String
        
        Private dbToField As String
        
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
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property Menu() As String
            Get
                Return Me.menuField
            End Get
            Set
                Me.menuField = value
                Me.RaisePropertyChanged("Menu")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property Description() As String
            Get
                Return Me.descriptionField
            End Get
            Set
                Me.descriptionField = value
                Me.RaisePropertyChanged("Description")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property DbTo() As String
            Get
                Return Me.dbToField
            End Get
            Set
                Me.dbToField = value
                Me.RaisePropertyChanged("DbTo")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/einfo")>  _
    Partial Public Class FieldType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private nameField As String
        
        Private fullNameField As String
        
        Private descriptionField As String
        
        Private termCountField As String
        
        Private isDateField As String
        
        Private isNumericalField As String
        
        Private singleTokenField As String
        
        Private hierarchyField As String
        
        Private isHiddenField As String
        
        Private isRangableField As String
        
        Private isTruncatableField As String
        
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
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property FullName() As String
            Get
                Return Me.fullNameField
            End Get
            Set
                Me.fullNameField = value
                Me.RaisePropertyChanged("FullName")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property Description() As String
            Get
                Return Me.descriptionField
            End Get
            Set
                Me.descriptionField = value
                Me.RaisePropertyChanged("Description")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property TermCount() As String
            Get
                Return Me.termCountField
            End Get
            Set
                Me.termCountField = value
                Me.RaisePropertyChanged("TermCount")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property IsDate() As String
            Get
                Return Me.isDateField
            End Get
            Set
                Me.isDateField = value
                Me.RaisePropertyChanged("IsDate")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=5)>  _
        Public Property IsNumerical() As String
            Get
                Return Me.isNumericalField
            End Get
            Set
                Me.isNumericalField = value
                Me.RaisePropertyChanged("IsNumerical")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=6)>  _
        Public Property SingleToken() As String
            Get
                Return Me.singleTokenField
            End Get
            Set
                Me.singleTokenField = value
                Me.RaisePropertyChanged("SingleToken")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=7)>  _
        Public Property Hierarchy() As String
            Get
                Return Me.hierarchyField
            End Get
            Set
                Me.hierarchyField = value
                Me.RaisePropertyChanged("Hierarchy")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=8)>  _
        Public Property IsHidden() As String
            Get
                Return Me.isHiddenField
            End Get
            Set
                Me.isHiddenField = value
                Me.RaisePropertyChanged("IsHidden")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=9)>  _
        Public Property IsRangable() As String
            Get
                Return Me.isRangableField
            End Get
            Set
                Me.isRangableField = value
                Me.RaisePropertyChanged("IsRangable")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=10)>  _
        Public Property IsTruncatable() As String
            Get
                Return Me.isTruncatableField
            End Get
            Set
                Me.isTruncatableField = value
                Me.RaisePropertyChanged("IsTruncatable")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/einfo")>  _
    Partial Public Class DbInfoType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dbNameField As String
        
        Private menuNameField As String
        
        Private descriptionField As String
        
        Private countField As String
        
        Private lastUpdateField As String
        
        Private fieldListField() As FieldType
        
        Private linkListField() As LinkType
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property DbName() As String
            Get
                Return Me.dbNameField
            End Get
            Set
                Me.dbNameField = value
                Me.RaisePropertyChanged("DbName")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property MenuName() As String
            Get
                Return Me.menuNameField
            End Get
            Set
                Me.menuNameField = value
                Me.RaisePropertyChanged("MenuName")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property Description() As String
            Get
                Return Me.descriptionField
            End Get
            Set
                Me.descriptionField = value
                Me.RaisePropertyChanged("Description")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property Count() As String
            Get
                Return Me.countField
            End Get
            Set
                Me.countField = value
                Me.RaisePropertyChanged("Count")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property LastUpdate() As String
            Get
                Return Me.lastUpdateField
            End Get
            Set
                Me.lastUpdateField = value
                Me.RaisePropertyChanged("LastUpdate")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(Order:=5),  _
         System.Xml.Serialization.XmlArrayItemAttribute("Field", IsNullable:=false)>  _
        Public Property FieldList() As FieldType()
            Get
                Return Me.fieldListField
            End Get
            Set
                Me.fieldListField = value
                Me.RaisePropertyChanged("FieldList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(Order:=6),  _
         System.Xml.Serialization.XmlArrayItemAttribute("Link", IsNullable:=false)>  _
        Public Property LinkList() As LinkType()
            Get
                Return Me.linkListField
            End Get
            Set
                Me.linkListField = value
                Me.RaisePropertyChanged("LinkList")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/einfo")>  _
    Partial Public Class DbListType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private itemsField() As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("DbName", Order:=0)>  _
        Public Property Items() As String()
            Get
                Return Me.itemsField
            End Get
            Set
                Me.itemsField = value
                Me.RaisePropertyChanged("Items")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/egquery")>  _
    Partial Public Class ResultItemType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dbNameField As String
        
        Private menuNameField As String
        
        Private countField As String
        
        Private statusField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property DbName() As String
            Get
                Return Me.dbNameField
            End Get
            Set
                Me.dbNameField = value
                Me.RaisePropertyChanged("DbName")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property MenuName() As String
            Get
                Return Me.menuNameField
            End Get
            Set
                Me.menuNameField = value
                Me.RaisePropertyChanged("MenuName")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property Count() As String
            Get
                Return Me.countField
            End Get
            Set
                Me.countField = value
                Me.RaisePropertyChanged("Count")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property Status() As String
            Get
                Return Me.statusField
            End Get
            Set
                Me.statusField = value
                Me.RaisePropertyChanged("Status")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/egquery")>  _
    Partial Public Class eGQueryResultType
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private eRRORField As String
        
        Private resultItemField() As ResultItemType
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property [ERROR]() As String
            Get
                Return Me.eRRORField
            End Get
            Set
                Me.eRRORField = value
                Me.RaisePropertyChanged("ERROR")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("ResultItem", Order:=1)>  _
        Public Property ResultItem() As ResultItemType()
            Get
                Return Me.resultItemField
            End Get
            Set
                Me.resultItemField = value
                Me.RaisePropertyChanged("ResultItem")
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
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/egquery")>  _
    Partial Public Class Result
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private termField As String
        
        Private eGQueryResultField As eGQueryResultType
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property Term() As String
            Get
                Return Me.termField
            End Get
            Set
                Me.termField = value
                Me.RaisePropertyChanged("Term")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property eGQueryResult() As eGQueryResultType
            Get
                Return Me.eGQueryResultField
            End Get
            Set
                Me.eGQueryResultField = value
                Me.RaisePropertyChanged("eGQueryResult")
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
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_eGqueryRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/egquery", Order:=0)>  _
        Public eGqueryRequest As NCBI.eUtils.eGqueryRequest
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal eGqueryRequest As NCBI.eUtils.eGqueryRequest)
            MyBase.New
            Me.eGqueryRequest = eGqueryRequest
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_eGqueryResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/egquery", Order:=0)>  _
        Public Result As NCBI.eUtils.Result
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal Result As NCBI.eUtils.Result)
            MyBase.New
            Me.Result = Result
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/einfo")>  _
    Partial Public Class eInfoRequest
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dbField As String
        
        Private toolField As String
        
        Private emailField As String
        
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
        Public Property tool() As String
            Get
                Return Me.toolField
            End Get
            Set
                Me.toolField = value
                Me.RaisePropertyChanged("tool")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property email() As String
            Get
                Return Me.emailField
            End Get
            Set
                Me.emailField = value
                Me.RaisePropertyChanged("email")
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
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/einfo")>  _
    Partial Public Class eInfoResult
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private eRRORField As String
        
        Private dbListField As DbListType
        
        Private dbInfoField As DbInfoType
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property [ERROR]() As String
            Get
                Return Me.eRRORField
            End Get
            Set
                Me.eRRORField = value
                Me.RaisePropertyChanged("ERROR")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property DbList() As DbListType
            Get
                Return Me.dbListField
            End Get
            Set
                Me.dbListField = value
                Me.RaisePropertyChanged("DbList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property DbInfo() As DbInfoType
            Get
                Return Me.dbInfoField
            End Get
            Set
                Me.dbInfoField = value
                Me.RaisePropertyChanged("DbInfo")
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
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_eInfoRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/einfo", Order:=0)>  _
        Public eInfoRequest As NCBI.eUtils.eInfoRequest
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal eInfoRequest As NCBI.eUtils.eInfoRequest)
            MyBase.New
            Me.eInfoRequest = eInfoRequest
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_eInfoResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/einfo", Order:=0)>  _
        Public eInfoResult As NCBI.eUtils.eInfoResult
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal eInfoResult As NCBI.eUtils.eInfoResult)
            MyBase.New
            Me.eInfoResult = eInfoResult
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esearch")>  _
    Partial Public Class eSearchRequest
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dbField As String
        
        Private termField As String
        
        Private webEnvField As String
        
        Private queryKeyField As String
        
        Private usehistoryField As String
        
        Private toolField As String
        
        Private emailField As String
        
        Private fieldField As String
        
        Private reldateField As String
        
        Private mindateField As String
        
        Private maxdateField As String
        
        Private datetypeField As String
        
        Private retStartField As String
        
        Private retMaxField As String
        
        Private rettypeField As String
        
        Private sortField As String
        
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
        Public Property term() As String
            Get
                Return Me.termField
            End Get
            Set
                Me.termField = value
                Me.RaisePropertyChanged("term")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property WebEnv() As String
            Get
                Return Me.webEnvField
            End Get
            Set
                Me.webEnvField = value
                Me.RaisePropertyChanged("WebEnv")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property QueryKey() As String
            Get
                Return Me.queryKeyField
            End Get
            Set
                Me.queryKeyField = value
                Me.RaisePropertyChanged("QueryKey")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property usehistory() As String
            Get
                Return Me.usehistoryField
            End Get
            Set
                Me.usehistoryField = value
                Me.RaisePropertyChanged("usehistory")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=5)>  _
        Public Property tool() As String
            Get
                Return Me.toolField
            End Get
            Set
                Me.toolField = value
                Me.RaisePropertyChanged("tool")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=6)>  _
        Public Property email() As String
            Get
                Return Me.emailField
            End Get
            Set
                Me.emailField = value
                Me.RaisePropertyChanged("email")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=7)>  _
        Public Property field() As String
            Get
                Return Me.fieldField
            End Get
            Set
                Me.fieldField = value
                Me.RaisePropertyChanged("field")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=8)>  _
        Public Property reldate() As String
            Get
                Return Me.reldateField
            End Get
            Set
                Me.reldateField = value
                Me.RaisePropertyChanged("reldate")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=9)>  _
        Public Property mindate() As String
            Get
                Return Me.mindateField
            End Get
            Set
                Me.mindateField = value
                Me.RaisePropertyChanged("mindate")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=10)>  _
        Public Property maxdate() As String
            Get
                Return Me.maxdateField
            End Get
            Set
                Me.maxdateField = value
                Me.RaisePropertyChanged("maxdate")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=11)>  _
        Public Property datetype() As String
            Get
                Return Me.datetypeField
            End Get
            Set
                Me.datetypeField = value
                Me.RaisePropertyChanged("datetype")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=12)>  _
        Public Property RetStart() As String
            Get
                Return Me.retStartField
            End Get
            Set
                Me.retStartField = value
                Me.RaisePropertyChanged("RetStart")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=13)>  _
        Public Property RetMax() As String
            Get
                Return Me.retMaxField
            End Get
            Set
                Me.retMaxField = value
                Me.RaisePropertyChanged("RetMax")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=14)>  _
        Public Property rettype() As String
            Get
                Return Me.rettypeField
            End Get
            Set
                Me.rettypeField = value
                Me.RaisePropertyChanged("rettype")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=15)>  _
        Public Property sort() As String
            Get
                Return Me.sortField
            End Get
            Set
                Me.sortField = value
                Me.RaisePropertyChanged("sort")
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
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esearch")>  _
    Partial Public Class eSearchResult
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private eRRORField As String
        
        Private countField As String
        
        Private retMaxField As String
        
        Private retStartField As String
        
        Private queryKeyField As String
        
        Private webEnvField As String
        
        Private idListField() As String
        
        Private translationSetField() As TranslationType
        
        Private translationStackField() As Object
        
        Private queryTranslationField As String
        
        Private errorListField As ErrorListType
        
        Private warningListField As WarningListType
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property [ERROR]() As String
            Get
                Return Me.eRRORField
            End Get
            Set
                Me.eRRORField = value
                Me.RaisePropertyChanged("ERROR")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property Count() As String
            Get
                Return Me.countField
            End Get
            Set
                Me.countField = value
                Me.RaisePropertyChanged("Count")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property RetMax() As String
            Get
                Return Me.retMaxField
            End Get
            Set
                Me.retMaxField = value
                Me.RaisePropertyChanged("RetMax")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property RetStart() As String
            Get
                Return Me.retStartField
            End Get
            Set
                Me.retStartField = value
                Me.RaisePropertyChanged("RetStart")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property QueryKey() As String
            Get
                Return Me.queryKeyField
            End Get
            Set
                Me.queryKeyField = value
                Me.RaisePropertyChanged("QueryKey")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=5)>  _
        Public Property WebEnv() As String
            Get
                Return Me.webEnvField
            End Get
            Set
                Me.webEnvField = value
                Me.RaisePropertyChanged("WebEnv")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(Order:=6),  _
         System.Xml.Serialization.XmlArrayItemAttribute("Id", IsNullable:=false)>  _
        Public Property IdList() As String()
            Get
                Return Me.idListField
            End Get
            Set
                Me.idListField = value
                Me.RaisePropertyChanged("IdList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(Order:=7),  _
         System.Xml.Serialization.XmlArrayItemAttribute("Translation", IsNullable:=false)>  _
        Public Property TranslationSet() As TranslationType()
            Get
                Return Me.translationSetField
            End Get
            Set
                Me.translationSetField = value
                Me.RaisePropertyChanged("TranslationSet")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(Order:=8),  _
         System.Xml.Serialization.XmlArrayItemAttribute("OP", GetType(String), IsNullable:=false),  _
         System.Xml.Serialization.XmlArrayItemAttribute("TermSet", GetType(TermSetType), IsNullable:=false)>  _
        Public Property TranslationStack() As Object()
            Get
                Return Me.translationStackField
            End Get
            Set
                Me.translationStackField = value
                Me.RaisePropertyChanged("TranslationStack")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=9)>  _
        Public Property QueryTranslation() As String
            Get
                Return Me.queryTranslationField
            End Get
            Set
                Me.queryTranslationField = value
                Me.RaisePropertyChanged("QueryTranslation")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=10)>  _
        Public Property ErrorList() As ErrorListType
            Get
                Return Me.errorListField
            End Get
            Set
                Me.errorListField = value
                Me.RaisePropertyChanged("ErrorList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=11)>  _
        Public Property WarningList() As WarningListType
            Get
                Return Me.warningListField
            End Get
            Set
                Me.warningListField = value
                Me.RaisePropertyChanged("WarningList")
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
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_eSearchRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esearch", Order:=0)>  _
        Public eSearchRequest As NCBI.eUtils.eSearchRequest
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal eSearchRequest As NCBI.eUtils.eSearchRequest)
            MyBase.New
            Me.eSearchRequest = eSearchRequest
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_eSearchResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esearch", Order:=0)>  _
        Public eSearchResult As NCBI.eUtils.eSearchResult
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal eSearchResult As NCBI.eUtils.eSearchResult)
            MyBase.New
            Me.eSearchResult = eSearchResult
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esummary")>  _
    Partial Public Class eSummaryRequest
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dbField As String
        
        Private idField As String
        
        Private webEnvField As String
        
        Private query_keyField As String
        
        Private retstartField As String
        
        Private retmaxField As String
        
        Private toolField As String
        
        Private emailField As String
        
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
        Public Property id() As String
            Get
                Return Me.idField
            End Get
            Set
                Me.idField = value
                Me.RaisePropertyChanged("id")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property WebEnv() As String
            Get
                Return Me.webEnvField
            End Get
            Set
                Me.webEnvField = value
                Me.RaisePropertyChanged("WebEnv")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property query_key() As String
            Get
                Return Me.query_keyField
            End Get
            Set
                Me.query_keyField = value
                Me.RaisePropertyChanged("query_key")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property retstart() As String
            Get
                Return Me.retstartField
            End Get
            Set
                Me.retstartField = value
                Me.RaisePropertyChanged("retstart")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=5)>  _
        Public Property retmax() As String
            Get
                Return Me.retmaxField
            End Get
            Set
                Me.retmaxField = value
                Me.RaisePropertyChanged("retmax")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=6)>  _
        Public Property tool() As String
            Get
                Return Me.toolField
            End Get
            Set
                Me.toolField = value
                Me.RaisePropertyChanged("tool")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=7)>  _
        Public Property email() As String
            Get
                Return Me.emailField
            End Get
            Set
                Me.emailField = value
                Me.RaisePropertyChanged("email")
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
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esummary")>  _
    Partial Public Class eSummaryResult
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private eRRORField As String
        
        Private docSumField() As DocSumType
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property [ERROR]() As String
            Get
                Return Me.eRRORField
            End Get
            Set
                Me.eRRORField = value
                Me.RaisePropertyChanged("ERROR")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("DocSum", Order:=1)>  _
        Public Property DocSum() As DocSumType()
            Get
                Return Me.docSumField
            End Get
            Set
                Me.docSumField = value
                Me.RaisePropertyChanged("DocSum")
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
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_eSummaryRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esummary", Order:=0)>  _
        Public eSummaryRequest As NCBI.eUtils.eSummaryRequest
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal eSummaryRequest As NCBI.eUtils.eSummaryRequest)
            MyBase.New
            Me.eSummaryRequest = eSummaryRequest
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_eSummaryResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/esummary", Order:=0)>  _
        Public eSummaryResult As NCBI.eUtils.eSummaryResult
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal eSummaryResult As NCBI.eUtils.eSummaryResult)
            MyBase.New
            Me.eSummaryResult = eSummaryResult
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class eLinkRequest
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dbField As String
        
        Private idField() As String
        
        Private reldateField As String
        
        Private mindateField As String
        
        Private maxdateField As String
        
        Private datetypeField As String
        
        Private termField As String
        
        Private dbfromField As String
        
        Private linknameField As String
        
        Private webEnvField As String
        
        Private query_keyField As String
        
        Private cmdField As String
        
        Private toolField As String
        
        Private emailField As String
        
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
        <System.Xml.Serialization.XmlElementAttribute("id", Order:=1)>  _
        Public Property id() As String()
            Get
                Return Me.idField
            End Get
            Set
                Me.idField = value
                Me.RaisePropertyChanged("id")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property reldate() As String
            Get
                Return Me.reldateField
            End Get
            Set
                Me.reldateField = value
                Me.RaisePropertyChanged("reldate")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property mindate() As String
            Get
                Return Me.mindateField
            End Get
            Set
                Me.mindateField = value
                Me.RaisePropertyChanged("mindate")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property maxdate() As String
            Get
                Return Me.maxdateField
            End Get
            Set
                Me.maxdateField = value
                Me.RaisePropertyChanged("maxdate")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=5)>  _
        Public Property datetype() As String
            Get
                Return Me.datetypeField
            End Get
            Set
                Me.datetypeField = value
                Me.RaisePropertyChanged("datetype")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=6)>  _
        Public Property term() As String
            Get
                Return Me.termField
            End Get
            Set
                Me.termField = value
                Me.RaisePropertyChanged("term")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=7)>  _
        Public Property dbfrom() As String
            Get
                Return Me.dbfromField
            End Get
            Set
                Me.dbfromField = value
                Me.RaisePropertyChanged("dbfrom")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=8)>  _
        Public Property linkname() As String
            Get
                Return Me.linknameField
            End Get
            Set
                Me.linknameField = value
                Me.RaisePropertyChanged("linkname")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=9)>  _
        Public Property WebEnv() As String
            Get
                Return Me.webEnvField
            End Get
            Set
                Me.webEnvField = value
                Me.RaisePropertyChanged("WebEnv")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=10)>  _
        Public Property query_key() As String
            Get
                Return Me.query_keyField
            End Get
            Set
                Me.query_keyField = value
                Me.RaisePropertyChanged("query_key")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=11)>  _
        Public Property cmd() As String
            Get
                Return Me.cmdField
            End Get
            Set
                Me.cmdField = value
                Me.RaisePropertyChanged("cmd")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=12)>  _
        Public Property tool() As String
            Get
                Return Me.toolField
            End Get
            Set
                Me.toolField = value
                Me.RaisePropertyChanged("tool")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=13)>  _
        Public Property email() As String
            Get
                Return Me.emailField
            End Get
            Set
                Me.emailField = value
                Me.RaisePropertyChanged("email")
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
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink")>  _
    Partial Public Class eLinkResult
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private linkSetField() As LinkSetType
        
        Private eRRORField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("LinkSet", Order:=0)>  _
        Public Property LinkSet() As LinkSetType()
            Get
                Return Me.linkSetField
            End Get
            Set
                Me.linkSetField = value
                Me.RaisePropertyChanged("LinkSet")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property [ERROR]() As String
            Get
                Return Me.eRRORField
            End Get
            Set
                Me.eRRORField = value
                Me.RaisePropertyChanged("ERROR")
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
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_eLinkRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink", Order:=0)>  _
        Public eLinkRequest As NCBI.eUtils.eLinkRequest
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal eLinkRequest As NCBI.eUtils.eLinkRequest)
            MyBase.New
            Me.eLinkRequest = eLinkRequest
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_eLinkResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/elink", Order:=0)>  _
        Public eLinkResult As NCBI.eUtils.eLinkResult
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal eLinkResult As NCBI.eUtils.eLinkResult)
            MyBase.New
            Me.eLinkResult = eLinkResult
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/espell")>  _
    Partial Public Class eSpellRequest
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dbField As String
        
        Private termField As String
        
        Private toolField As String
        
        Private emailField As String
        
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
        Public Property term() As String
            Get
                Return Me.termField
            End Get
            Set
                Me.termField = value
                Me.RaisePropertyChanged("term")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property tool() As String
            Get
                Return Me.toolField
            End Get
            Set
                Me.toolField = value
                Me.RaisePropertyChanged("tool")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property email() As String
            Get
                Return Me.emailField
            End Get
            Set
                Me.emailField = value
                Me.RaisePropertyChanged("email")
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
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/espell")>  _
    Partial Public Class eSpellResult
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private databaseField As String
        
        Private queryField As String
        
        Private correctedQueryField As String
        
        Private spelledQueryField As SpelledQuery
        
        Private eRRORField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=0)>  _
        Public Property Database() As String
            Get
                Return Me.databaseField
            End Get
            Set
                Me.databaseField = value
                Me.RaisePropertyChanged("Database")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property Query() As String
            Get
                Return Me.queryField
            End Get
            Set
                Me.queryField = value
                Me.RaisePropertyChanged("Query")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property CorrectedQuery() As String
            Get
                Return Me.correctedQueryField
            End Get
            Set
                Me.correctedQueryField = value
                Me.RaisePropertyChanged("CorrectedQuery")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property SpelledQuery() As SpelledQuery
            Get
                Return Me.spelledQueryField
            End Get
            Set
                Me.spelledQueryField = value
                Me.RaisePropertyChanged("SpelledQuery")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property [ERROR]() As String
            Get
                Return Me.eRRORField
            End Get
            Set
                Me.eRRORField = value
                Me.RaisePropertyChanged("ERROR")
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
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/espell")>  _
    Partial Public Class SpelledQuery
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private itemsField() As String
        
        Private itemsElementNameField() As ItemsChoiceType
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("Original", GetType(String), Order:=0),  _
         System.Xml.Serialization.XmlElementAttribute("Replaced", GetType(String), Order:=0),  _
         System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")>  _
        Public Property Items() As String()
            Get
                Return Me.itemsField
            End Get
            Set
                Me.itemsField = value
                Me.RaisePropertyChanged("Items")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("ItemsElementName", Order:=1),  _
         System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property ItemsElementName() As ItemsChoiceType()
            Get
                Return Me.itemsElementNameField
            End Get
            Set
                Me.itemsElementNameField = value
                Me.RaisePropertyChanged("ItemsElementName")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/espell", IncludeInSchema:=false)>  _
    Public Enum ItemsChoiceType
        
        '''<remarks/>
        Original
        
        '''<remarks/>
        Replaced
    End Enum
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_eSpellRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/espell", Order:=0)>  _
        Public eSpellRequest As NCBI.eUtils.eSpellRequest
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal eSpellRequest As NCBI.eUtils.eSpellRequest)
            MyBase.New
            Me.eSpellRequest = eSpellRequest
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_eSpellResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/espell", Order:=0)>  _
        Public eSpellResult As NCBI.eUtils.eSpellResult
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal eSpellResult As NCBI.eUtils.eSpellResult)
            MyBase.New
            Me.eSpellResult = eSpellResult
        End Sub
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/epost")>  _
    Partial Public Class ePostRequest
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private dbField As String
        
        Private idField As String
        
        Private webEnvField As String
        
        Private toolField As String
        
        Private emailField As String
        
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
        Public Property id() As String
            Get
                Return Me.idField
            End Get
            Set
                Me.idField = value
                Me.RaisePropertyChanged("id")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property WebEnv() As String
            Get
                Return Me.webEnvField
            End Get
            Set
                Me.webEnvField = value
                Me.RaisePropertyChanged("WebEnv")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property tool() As String
            Get
                Return Me.toolField
            End Get
            Set
                Me.toolField = value
                Me.RaisePropertyChanged("tool")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=4)>  _
        Public Property email() As String
            Get
                Return Me.emailField
            End Get
            Set
                Me.emailField = value
                Me.RaisePropertyChanged("email")
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
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true, [Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/epost")>  _
    Partial Public Class ePostResult
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private invalidIdListField() As String
        
        Private queryKeyField As String
        
        Private webEnvField As String
        
        Private eRRORField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(Order:=0),  _
         System.Xml.Serialization.XmlArrayItemAttribute("Id", IsNullable:=false)>  _
        Public Property InvalidIdList() As String()
            Get
                Return Me.invalidIdListField
            End Get
            Set
                Me.invalidIdListField = value
                Me.RaisePropertyChanged("InvalidIdList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=1)>  _
        Public Property QueryKey() As String
            Get
                Return Me.queryKeyField
            End Get
            Set
                Me.queryKeyField = value
                Me.RaisePropertyChanged("QueryKey")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property WebEnv() As String
            Get
                Return Me.webEnvField
            End Get
            Set
                Me.webEnvField = value
                Me.RaisePropertyChanged("WebEnv")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=3)>  _
        Public Property [ERROR]() As String
            Get
                Return Me.eRRORField
            End Get
            Set
                Me.eRRORField = value
                Me.RaisePropertyChanged("ERROR")
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
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_ePostRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/epost", Order:=0)>  _
        Public ePostRequest As NCBI.eUtils.ePostRequest
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal ePostRequest As NCBI.eUtils.ePostRequest)
            MyBase.New
            Me.ePostRequest = ePostRequest
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class run_ePostResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ncbi.nlm.nih.gov/soap/eutils/epost", Order:=0)>  _
        Public ePostResult As NCBI.eUtils.ePostResult
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal ePostResult As NCBI.eUtils.ePostResult)
            MyBase.New
            Me.ePostResult = ePostResult
        End Sub
    End Class
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Public Interface eUtilsServiceSoapChannel
        Inherits NCBI.eUtils.eUtilsServiceSoap, System.ServiceModel.IClientChannel
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Partial Public Class eUtilsServiceSoapClient
        Inherits System.ServiceModel.ClientBase(Of NCBI.eUtils.eUtilsServiceSoap)
        Implements NCBI.eUtils.eUtilsServiceSoap
        
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
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_eGquery(ByVal request As NCBI.eUtils.run_eGqueryRequest) As NCBI.eUtils.run_eGqueryResponse Implements NCBI.eUtils.eUtilsServiceSoap.run_eGquery
            Return MyBase.Channel.run_eGquery(request)
        End Function
        
        Public Function run_eGquery(ByVal eGqueryRequest As NCBI.eUtils.eGqueryRequest) As NCBI.eUtils.Result
            Dim inValue As NCBI.eUtils.run_eGqueryRequest = New NCBI.eUtils.run_eGqueryRequest()
            inValue.eGqueryRequest = eGqueryRequest
            Dim retVal As NCBI.eUtils.run_eGqueryResponse = CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_eGquery(inValue)
            Return retVal.Result
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_eGqueryAsync(ByVal request As NCBI.eUtils.run_eGqueryRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eGqueryResponse) Implements NCBI.eUtils.eUtilsServiceSoap.run_eGqueryAsync
            Return MyBase.Channel.run_eGqueryAsync(request)
        End Function
        
        Public Function run_eGqueryAsync(ByVal eGqueryRequest As NCBI.eUtils.eGqueryRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eGqueryResponse)
            Dim inValue As NCBI.eUtils.run_eGqueryRequest = New NCBI.eUtils.run_eGqueryRequest()
            inValue.eGqueryRequest = eGqueryRequest
            Return CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_eGqueryAsync(inValue)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_eInfo(ByVal request As NCBI.eUtils.run_eInfoRequest) As NCBI.eUtils.run_eInfoResponse Implements NCBI.eUtils.eUtilsServiceSoap.run_eInfo
            Return MyBase.Channel.run_eInfo(request)
        End Function
        
        Public Function run_eInfo(ByVal eInfoRequest As NCBI.eUtils.eInfoRequest) As NCBI.eUtils.eInfoResult
            Dim inValue As NCBI.eUtils.run_eInfoRequest = New NCBI.eUtils.run_eInfoRequest()
            inValue.eInfoRequest = eInfoRequest
            Dim retVal As NCBI.eUtils.run_eInfoResponse = CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_eInfo(inValue)
            Return retVal.eInfoResult
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_eInfoAsync(ByVal request As NCBI.eUtils.run_eInfoRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eInfoResponse) Implements NCBI.eUtils.eUtilsServiceSoap.run_eInfoAsync
            Return MyBase.Channel.run_eInfoAsync(request)
        End Function
        
        Public Function run_eInfoAsync(ByVal eInfoRequest As NCBI.eUtils.eInfoRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eInfoResponse)
            Dim inValue As NCBI.eUtils.run_eInfoRequest = New NCBI.eUtils.run_eInfoRequest()
            inValue.eInfoRequest = eInfoRequest
            Return CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_eInfoAsync(inValue)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_eSearch(ByVal request As NCBI.eUtils.run_eSearchRequest) As NCBI.eUtils.run_eSearchResponse Implements NCBI.eUtils.eUtilsServiceSoap.run_eSearch
            Return MyBase.Channel.run_eSearch(request)
        End Function
        
        Public Function run_eSearch(ByVal eSearchRequest As NCBI.eUtils.eSearchRequest) As NCBI.eUtils.eSearchResult
            Dim inValue As NCBI.eUtils.run_eSearchRequest = New NCBI.eUtils.run_eSearchRequest()
            inValue.eSearchRequest = eSearchRequest
            Dim retVal As NCBI.eUtils.run_eSearchResponse = CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_eSearch(inValue)
            Return retVal.eSearchResult
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_eSearchAsync(ByVal request As NCBI.eUtils.run_eSearchRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eSearchResponse) Implements NCBI.eUtils.eUtilsServiceSoap.run_eSearchAsync
            Return MyBase.Channel.run_eSearchAsync(request)
        End Function
        
        Public Function run_eSearchAsync(ByVal eSearchRequest As NCBI.eUtils.eSearchRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eSearchResponse)
            Dim inValue As NCBI.eUtils.run_eSearchRequest = New NCBI.eUtils.run_eSearchRequest()
            inValue.eSearchRequest = eSearchRequest
            Return CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_eSearchAsync(inValue)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_eSummary(ByVal request As NCBI.eUtils.run_eSummaryRequest) As NCBI.eUtils.run_eSummaryResponse Implements NCBI.eUtils.eUtilsServiceSoap.run_eSummary
            Return MyBase.Channel.run_eSummary(request)
        End Function
        
        Public Function run_eSummary(ByVal eSummaryRequest As NCBI.eUtils.eSummaryRequest) As NCBI.eUtils.eSummaryResult
            Dim inValue As NCBI.eUtils.run_eSummaryRequest = New NCBI.eUtils.run_eSummaryRequest()
            inValue.eSummaryRequest = eSummaryRequest
            Dim retVal As NCBI.eUtils.run_eSummaryResponse = CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_eSummary(inValue)
            Return retVal.eSummaryResult
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_eSummaryAsync(ByVal request As NCBI.eUtils.run_eSummaryRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eSummaryResponse) Implements NCBI.eUtils.eUtilsServiceSoap.run_eSummaryAsync
            Return MyBase.Channel.run_eSummaryAsync(request)
        End Function
        
        Public Function run_eSummaryAsync(ByVal eSummaryRequest As NCBI.eUtils.eSummaryRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eSummaryResponse)
            Dim inValue As NCBI.eUtils.run_eSummaryRequest = New NCBI.eUtils.run_eSummaryRequest()
            inValue.eSummaryRequest = eSummaryRequest
            Return CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_eSummaryAsync(inValue)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_eLink(ByVal request As NCBI.eUtils.run_eLinkRequest) As NCBI.eUtils.run_eLinkResponse Implements NCBI.eUtils.eUtilsServiceSoap.run_eLink
            Return MyBase.Channel.run_eLink(request)
        End Function
        
        Public Function run_eLink(ByVal eLinkRequest As NCBI.eUtils.eLinkRequest) As NCBI.eUtils.eLinkResult
            Dim inValue As NCBI.eUtils.run_eLinkRequest = New NCBI.eUtils.run_eLinkRequest()
            inValue.eLinkRequest = eLinkRequest
            Dim retVal As NCBI.eUtils.run_eLinkResponse = CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_eLink(inValue)
            Return retVal.eLinkResult
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_eLinkAsync(ByVal request As NCBI.eUtils.run_eLinkRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eLinkResponse) Implements NCBI.eUtils.eUtilsServiceSoap.run_eLinkAsync
            Return MyBase.Channel.run_eLinkAsync(request)
        End Function
        
        Public Function run_eLinkAsync(ByVal eLinkRequest As NCBI.eUtils.eLinkRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eLinkResponse)
            Dim inValue As NCBI.eUtils.run_eLinkRequest = New NCBI.eUtils.run_eLinkRequest()
            inValue.eLinkRequest = eLinkRequest
            Return CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_eLinkAsync(inValue)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_eSpell(ByVal request As NCBI.eUtils.run_eSpellRequest) As NCBI.eUtils.run_eSpellResponse Implements NCBI.eUtils.eUtilsServiceSoap.run_eSpell
            Return MyBase.Channel.run_eSpell(request)
        End Function
        
        Public Function run_eSpell(ByVal eSpellRequest As NCBI.eUtils.eSpellRequest) As NCBI.eUtils.eSpellResult
            Dim inValue As NCBI.eUtils.run_eSpellRequest = New NCBI.eUtils.run_eSpellRequest()
            inValue.eSpellRequest = eSpellRequest
            Dim retVal As NCBI.eUtils.run_eSpellResponse = CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_eSpell(inValue)
            Return retVal.eSpellResult
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_eSpellAsync(ByVal request As NCBI.eUtils.run_eSpellRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eSpellResponse) Implements NCBI.eUtils.eUtilsServiceSoap.run_eSpellAsync
            Return MyBase.Channel.run_eSpellAsync(request)
        End Function
        
        Public Function run_eSpellAsync(ByVal eSpellRequest As NCBI.eUtils.eSpellRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_eSpellResponse)
            Dim inValue As NCBI.eUtils.run_eSpellRequest = New NCBI.eUtils.run_eSpellRequest()
            inValue.eSpellRequest = eSpellRequest
            Return CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_eSpellAsync(inValue)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_ePost(ByVal request As NCBI.eUtils.run_ePostRequest) As NCBI.eUtils.run_ePostResponse Implements NCBI.eUtils.eUtilsServiceSoap.run_ePost
            Return MyBase.Channel.run_ePost(request)
        End Function
        
        Public Function run_ePost(ByVal ePostRequest As NCBI.eUtils.ePostRequest) As NCBI.eUtils.ePostResult
            Dim inValue As NCBI.eUtils.run_ePostRequest = New NCBI.eUtils.run_ePostRequest()
            inValue.ePostRequest = ePostRequest
            Dim retVal As NCBI.eUtils.run_ePostResponse = CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_ePost(inValue)
            Return retVal.ePostResult
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function NCBI_eUtils_eUtilsServiceSoap_run_ePostAsync(ByVal request As NCBI.eUtils.run_ePostRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_ePostResponse) Implements NCBI.eUtils.eUtilsServiceSoap.run_ePostAsync
            Return MyBase.Channel.run_ePostAsync(request)
        End Function
        
        Public Function run_ePostAsync(ByVal ePostRequest As NCBI.eUtils.ePostRequest) As System.Threading.Tasks.Task(Of NCBI.eUtils.run_ePostResponse)
            Dim inValue As NCBI.eUtils.run_ePostRequest = New NCBI.eUtils.run_ePostRequest()
            inValue.ePostRequest = ePostRequest
            Return CType(Me,NCBI.eUtils.eUtilsServiceSoap).run_ePostAsync(inValue)
        End Function
    End Class
End Namespace
