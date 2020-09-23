#Region "Microsoft.VisualBasic::c50f3e44f16c9e4e0d57d88f16a3735d, data\WebServices\Service References\WSDbfetchDoclitServerServices\Reference.vb"

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

    '     Class DbfParamsException
    ' 
    ' 
    ' 
    '     Class DbfException
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class StyleInfo
    ' 
    '         Properties: mimeType, name
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class FormatInfo
    ' 
    '         Properties: aliases, dataTerms, name, styleInfoList, syntaxTerms
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class ExampleIdentifiersInfo
    ' 
    '         Properties: accessionList, entryVersionList, idList, nameList, sequenceVersionList
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class DataResourceInfo
    ' 
    '         Properties: href, name
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class DatabaseInfo
    ' 
    '         Properties: aliasList, databaseTerms, dataResourceInfoList, defaultFormat, description
    '                     displayName, exampleIdentifiers, formatInfoList, href, name
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Class DbfNoEntryFoundException
    ' 
    ' 
    ' 
    '     Class DbfConnException
    ' 
    ' 
    ' 
    '     Class InputException
    ' 
    '         Sub: RaisePropertyChanged
    ' 
    '     Interface WSDBFetchServer
    ' 
    '         Function: fetchBatch, fetchBatchAsync, fetchData, fetchDataAsync, getDatabaseInfo
    '                   getDatabaseInfoAsync, getDatabaseInfoList, getDatabaseInfoListAsync, getDbFormats, getDbFormatsAsync
    '                   getFormatInfo, getFormatInfoAsync, getFormatStyles, getFormatStylesAsync, getStyleInfo
    '                   getStyleInfoAsync, getSupportedDBs, getSupportedDBsAsync, getSupportedFormats, getSupportedFormatsAsync
    '                   getSupportedStyles, getSupportedStylesAsync
    ' 
    '     Class getSupportedDBsRequest
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class getSupportedDBsResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class getSupportedFormatsRequest
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class getSupportedFormatsResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class getSupportedStylesRequest
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class getSupportedStylesResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class getDatabaseInfoListRequest
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class getDatabaseInfoListResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class getDbFormatsRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class getDbFormatsResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class getFormatStylesRequest
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class getFormatStylesResponse
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Interface WSDBFetchServerChannel
    ' 
    ' 
    ' 
    '     Class WSDBFetchServerClient
    ' 
    '         Constructor: (+5 Overloads) Sub New
    '         Function: fetchBatch, fetchBatchAsync, fetchData, fetchDataAsync, getDatabaseInfo
    '                   getDatabaseInfoAsync, getDatabaseInfoList, getDatabaseInfoListAsync, getDbFormats, getDbFormatsAsync
    '                   getFormatInfo, getFormatInfoAsync, getFormatStyles, getFormatStylesAsync, getStyleInfo
    '                   getStyleInfoAsync, getSupportedDBs, getSupportedDBsAsync, getSupportedFormats, getSupportedFormatsAsync
    '                   getSupportedStyles, getSupportedStylesAsync, WSDbfetchDoclitServerServices_WSDBFetchServer_getDatabaseInfoList, WSDbfetchDoclitServerServices_WSDBFetchServer_getDatabaseInfoListAsync, WSDbfetchDoclitServerServices_WSDBFetchServer_getDbFormats
    '                   WSDbfetchDoclitServerServices_WSDBFetchServer_getDbFormatsAsync, WSDbfetchDoclitServerServices_WSDBFetchServer_getFormatStyles, WSDbfetchDoclitServerServices_WSDBFetchServer_getFormatStylesAsync, WSDbfetchDoclitServerServices_WSDBFetchServer_getSupportedDBs, WSDbfetchDoclitServerServices_WSDBFetchServer_getSupportedDBsAsync
    '                   WSDbfetchDoclitServerServices_WSDBFetchServer_getSupportedFormats, WSDbfetchDoclitServerServices_WSDBFetchServer_getSupportedFormatsAsync, WSDbfetchDoclitServerServices_WSDBFetchServer_getSupportedStyles, WSDbfetchDoclitServerServices_WSDBFetchServer_getSupportedStylesAsync
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


Namespace WSDbfetchDoclitServerServices
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit")>  _
    Partial Public Class DbfParamsException
        Inherits DbfException
    End Class
    
    '''<remarks/>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(DbfNoEntryFoundException)),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(DbfConnException)),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(DbfParamsException)),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit")>  _
    Partial Public Class DbfException
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit")>  _
    Partial Public Class StyleInfo
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private mimeTypeField As String
        
        Private nameField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(IsNullable:=true, Order:=0)>  _
        Public Property mimeType() As String
            Get
                Return Me.mimeTypeField
            End Get
            Set
                Me.mimeTypeField = value
                Me.RaisePropertyChanged("mimeType")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(IsNullable:=true, Order:=1)>  _
        Public Property name() As String
            Get
                Return Me.nameField
            End Get
            Set
                Me.nameField = value
                Me.RaisePropertyChanged("name")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit")>  _
    Partial Public Class FormatInfo
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private aliasesField() As String
        
        Private dataTermsField() As String
        
        Private nameField As String
        
        Private styleInfoListField() As StyleInfo
        
        Private syntaxTermsField() As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(IsNullable:=true, Order:=0),  _
         System.Xml.Serialization.XmlArrayItemAttribute("alias", IsNullable:=false)>  _
        Public Property aliases() As String()
            Get
                Return Me.aliasesField
            End Get
            Set
                Me.aliasesField = value
                Me.RaisePropertyChanged("aliases")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(IsNullable:=true, Order:=1),  _
         System.Xml.Serialization.XmlArrayItemAttribute("dataTerm", IsNullable:=false)>  _
        Public Property dataTerms() As String()
            Get
                Return Me.dataTermsField
            End Get
            Set
                Me.dataTermsField = value
                Me.RaisePropertyChanged("dataTerms")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Order:=2)>  _
        Public Property name() As String
            Get
                Return Me.nameField
            End Get
            Set
                Me.nameField = value
                Me.RaisePropertyChanged("name")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(IsNullable:=true, Order:=3),  _
         System.Xml.Serialization.XmlArrayItemAttribute("styleInfo", IsNullable:=false)>  _
        Public Property styleInfoList() As StyleInfo()
            Get
                Return Me.styleInfoListField
            End Get
            Set
                Me.styleInfoListField = value
                Me.RaisePropertyChanged("styleInfoList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(IsNullable:=true, Order:=4),  _
         System.Xml.Serialization.XmlArrayItemAttribute("syntaxTerm", IsNullable:=false)>  _
        Public Property syntaxTerms() As String()
            Get
                Return Me.syntaxTermsField
            End Get
            Set
                Me.syntaxTermsField = value
                Me.RaisePropertyChanged("syntaxTerms")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit")>  _
    Partial Public Class ExampleIdentifiersInfo
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private accessionListField() As String
        
        Private entryVersionListField() As String
        
        Private idListField() As String
        
        Private nameListField() As String
        
        Private sequenceVersionListField() As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(IsNullable:=true, Order:=0),  _
         System.Xml.Serialization.XmlArrayItemAttribute("accession", IsNullable:=false)>  _
        Public Property accessionList() As String()
            Get
                Return Me.accessionListField
            End Get
            Set
                Me.accessionListField = value
                Me.RaisePropertyChanged("accessionList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(IsNullable:=true, Order:=1),  _
         System.Xml.Serialization.XmlArrayItemAttribute("entryVersion", IsNullable:=false)>  _
        Public Property entryVersionList() As String()
            Get
                Return Me.entryVersionListField
            End Get
            Set
                Me.entryVersionListField = value
                Me.RaisePropertyChanged("entryVersionList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(IsNullable:=true, Order:=2),  _
         System.Xml.Serialization.XmlArrayItemAttribute("id", IsNullable:=false)>  _
        Public Property idList() As String()
            Get
                Return Me.idListField
            End Get
            Set
                Me.idListField = value
                Me.RaisePropertyChanged("idList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(IsNullable:=true, Order:=3),  _
         System.Xml.Serialization.XmlArrayItemAttribute("name", IsNullable:=false)>  _
        Public Property nameList() As String()
            Get
                Return Me.nameListField
            End Get
            Set
                Me.nameListField = value
                Me.RaisePropertyChanged("nameList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(IsNullable:=true, Order:=4),  _
         System.Xml.Serialization.XmlArrayItemAttribute("sequenceVersion", IsNullable:=false)>  _
        Public Property sequenceVersionList() As String()
            Get
                Return Me.sequenceVersionListField
            End Get
            Set
                Me.sequenceVersionListField = value
                Me.RaisePropertyChanged("sequenceVersionList")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit")>  _
    Partial Public Class DataResourceInfo
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private hrefField As String
        
        Private nameField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(IsNullable:=true, Order:=0)>  _
        Public Property href() As String
            Get
                Return Me.hrefField
            End Get
            Set
                Me.hrefField = value
                Me.RaisePropertyChanged("href")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(IsNullable:=true, Order:=1)>  _
        Public Property name() As String
            Get
                Return Me.nameField
            End Get
            Set
                Me.nameField = value
                Me.RaisePropertyChanged("name")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit")>  _
    Partial Public Class DatabaseInfo
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Private aliasListField() As String
        
        Private databaseTermsField() As String
        
        Private dataResourceInfoListField() As DataResourceInfo
        
        Private defaultFormatField As String
        
        Private descriptionField As String
        
        Private displayNameField As String
        
        Private exampleIdentifiersField As ExampleIdentifiersInfo
        
        Private formatInfoListField() As FormatInfo
        
        Private hrefField As String
        
        Private nameField As String
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(IsNullable:=true, Order:=0),  _
         System.Xml.Serialization.XmlArrayItemAttribute("alias", IsNullable:=false)>  _
        Public Property aliasList() As String()
            Get
                Return Me.aliasListField
            End Get
            Set
                Me.aliasListField = value
                Me.RaisePropertyChanged("aliasList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(IsNullable:=true, Order:=1),  _
         System.Xml.Serialization.XmlArrayItemAttribute("databaseTerm", IsNullable:=false)>  _
        Public Property databaseTerms() As String()
            Get
                Return Me.databaseTermsField
            End Get
            Set
                Me.databaseTermsField = value
                Me.RaisePropertyChanged("databaseTerms")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(IsNullable:=true, Order:=2),  _
         System.Xml.Serialization.XmlArrayItemAttribute("dataResourceInfo", IsNullable:=false)>  _
        Public Property dataResourceInfoList() As DataResourceInfo()
            Get
                Return Me.dataResourceInfoListField
            End Get
            Set
                Me.dataResourceInfoListField = value
                Me.RaisePropertyChanged("dataResourceInfoList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(IsNullable:=true, Order:=3)>  _
        Public Property defaultFormat() As String
            Get
                Return Me.defaultFormatField
            End Get
            Set
                Me.defaultFormatField = value
                Me.RaisePropertyChanged("defaultFormat")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(IsNullable:=true, Order:=4)>  _
        Public Property description() As String
            Get
                Return Me.descriptionField
            End Get
            Set
                Me.descriptionField = value
                Me.RaisePropertyChanged("description")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(IsNullable:=true, Order:=5)>  _
        Public Property displayName() As String
            Get
                Return Me.displayNameField
            End Get
            Set
                Me.displayNameField = value
                Me.RaisePropertyChanged("displayName")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(IsNullable:=true, Order:=6)>  _
        Public Property exampleIdentifiers() As ExampleIdentifiersInfo
            Get
                Return Me.exampleIdentifiersField
            End Get
            Set
                Me.exampleIdentifiersField = value
                Me.RaisePropertyChanged("exampleIdentifiers")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute(IsNullable:=true, Order:=7),  _
         System.Xml.Serialization.XmlArrayItemAttribute("formatInfo", IsNullable:=false)>  _
        Public Property formatInfoList() As FormatInfo()
            Get
                Return Me.formatInfoListField
            End Get
            Set
                Me.formatInfoListField = value
                Me.RaisePropertyChanged("formatInfoList")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(IsNullable:=true, Order:=8)>  _
        Public Property href() As String
            Get
                Return Me.hrefField
            End Get
            Set
                Me.hrefField = value
                Me.RaisePropertyChanged("href")
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(IsNullable:=true, Order:=9)>  _
        Public Property name() As String
            Get
                Return Me.nameField
            End Get
            Set
                Me.nameField = value
                Me.RaisePropertyChanged("name")
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit")>  _
    Partial Public Class DbfNoEntryFoundException
        Inherits DbfException
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit")>  _
    Partial Public Class DbfConnException
        Inherits DbfException
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit")>  _
    Partial Public Class InputException
        Inherits Object
        Implements System.ComponentModel.INotifyPropertyChanged
        
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        
        Protected Sub RaisePropertyChanged(ByVal propertyName As String)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub
    End Class
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.ServiceContractAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", ConfigurationName:="WSDbfetchDoclitServerServices.WSDBFetchServer")>  _
    Public Interface WSDBFetchServer
        
        'CODEGEN: Parameter 'getSupportedDBsReturn' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(DbfException))>  _
        Function getSupportedDBs(ByVal request As WSDbfetchDoclitServerServices.getSupportedDBsRequest) As <System.ServiceModel.MessageParameterAttribute(Name:="getSupportedDBsReturn")> WSDbfetchDoclitServerServices.getSupportedDBsResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*")>  _
        Function getSupportedDBsAsync(ByVal request As WSDbfetchDoclitServerServices.getSupportedDBsRequest) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getSupportedDBsResponse)
        
        'CODEGEN: Parameter 'getSupportedFormatsReturn' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(DbfException))>  _
        Function getSupportedFormats(ByVal request As WSDbfetchDoclitServerServices.getSupportedFormatsRequest) As <System.ServiceModel.MessageParameterAttribute(Name:="getSupportedFormatsReturn")> WSDbfetchDoclitServerServices.getSupportedFormatsResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*")>  _
        Function getSupportedFormatsAsync(ByVal request As WSDbfetchDoclitServerServices.getSupportedFormatsRequest) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getSupportedFormatsResponse)
        
        'CODEGEN: Parameter 'getSupportedStylesReturn' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(DbfException))>  _
        Function getSupportedStyles(ByVal request As WSDbfetchDoclitServerServices.getSupportedStylesRequest) As <System.ServiceModel.MessageParameterAttribute(Name:="getSupportedStylesReturn")> WSDbfetchDoclitServerServices.getSupportedStylesResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*")>  _
        Function getSupportedStylesAsync(ByVal request As WSDbfetchDoclitServerServices.getSupportedStylesRequest) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getSupportedStylesResponse)
        
        'CODEGEN: Parameter 'getDatabaseInfoListReturn' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(DbfException))>  _
        Function getDatabaseInfoList(ByVal request As WSDbfetchDoclitServerServices.getDatabaseInfoListRequest) As <System.ServiceModel.MessageParameterAttribute(Name:="getDatabaseInfoListReturn")> WSDbfetchDoclitServerServices.getDatabaseInfoListResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*")>  _
        Function getDatabaseInfoListAsync(ByVal request As WSDbfetchDoclitServerServices.getDatabaseInfoListRequest) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getDatabaseInfoListResponse)
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.DbfParamsException), Action:="", Name:="fault"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(DbfException))>  _
        Function getDatabaseInfo(ByVal db As String) As <System.ServiceModel.MessageParameterAttribute(Name:="getDatabaseInfoReturn")> WSDbfetchDoclitServerServices.DatabaseInfo
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*")>  _
        Function getDatabaseInfoAsync(ByVal db As String) As <System.ServiceModel.MessageParameterAttribute(Name:="getDatabaseInfoReturn")> System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.DatabaseInfo)
        
        'CODEGEN: Parameter 'getDbFormatsReturn' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.DbfParamsException), Action:="", Name:="fault"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(DbfException))>  _
        Function getDbFormats(ByVal request As WSDbfetchDoclitServerServices.getDbFormatsRequest) As <System.ServiceModel.MessageParameterAttribute(Name:="getDbFormatsReturn")> WSDbfetchDoclitServerServices.getDbFormatsResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*")>  _
        Function getDbFormatsAsync(ByVal request As WSDbfetchDoclitServerServices.getDbFormatsRequest) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getDbFormatsResponse)
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.DbfParamsException), Action:="", Name:="fault"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(DbfException))>  _
        Function getFormatInfo(ByVal db As String, ByVal format As String) As <System.ServiceModel.MessageParameterAttribute(Name:="getFormatInfoReturn")> WSDbfetchDoclitServerServices.FormatInfo
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*")>  _
        Function getFormatInfoAsync(ByVal db As String, ByVal format As String) As <System.ServiceModel.MessageParameterAttribute(Name:="getFormatInfoReturn")> System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.FormatInfo)
        
        'CODEGEN: Parameter 'getFormatStylesReturn' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.DbfParamsException), Action:="", Name:="fault"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(DbfException))>  _
        Function getFormatStyles(ByVal request As WSDbfetchDoclitServerServices.getFormatStylesRequest) As <System.ServiceModel.MessageParameterAttribute(Name:="getFormatStylesReturn")> WSDbfetchDoclitServerServices.getFormatStylesResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*")>  _
        Function getFormatStylesAsync(ByVal request As WSDbfetchDoclitServerServices.getFormatStylesRequest) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getFormatStylesResponse)
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.DbfParamsException), Action:="", Name:="fault"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(DbfException))>  _
        Function getStyleInfo(ByVal db As String, ByVal format As String, ByVal style As String) As <System.ServiceModel.MessageParameterAttribute(Name:="getStyleInfoReturn")> WSDbfetchDoclitServerServices.StyleInfo
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*")>  _
        Function getStyleInfoAsync(ByVal db As String, ByVal format As String, ByVal style As String) As <System.ServiceModel.MessageParameterAttribute(Name:="getStyleInfoReturn")> System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.StyleInfo)
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.DbfConnException), Action:="", Name:="fault1"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.DbfException), Action:="", Name:="fault3"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.DbfParamsException), Action:="", Name:="fault"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.DbfNoEntryFoundException), Action:="", Name:="fault2"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.InputException), Action:="", Name:="fault4"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(DbfException))>  _
        Function fetchData(ByVal query As String, ByVal format As String, ByVal style As String) As <System.ServiceModel.MessageParameterAttribute(Name:="fetchDataReturn")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*")>  _
        Function fetchDataAsync(ByVal query As String, ByVal format As String, ByVal style As String) As <System.ServiceModel.MessageParameterAttribute(Name:="fetchDataReturn")> System.Threading.Tasks.Task(Of String)
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.DbfConnException), Action:="", Name:="fault1"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.DbfException), Action:="", Name:="fault3"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.DbfParamsException), Action:="", Name:="fault"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.DbfNoEntryFoundException), Action:="", Name:="fault2"),  _
         System.ServiceModel.FaultContractAttribute(GetType(WSDbfetchDoclitServerServices.InputException), Action:="", Name:="fault4"),  _
         System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults:=true),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(DbfException))>  _
        Function fetchBatch(ByVal db As String, ByVal ids As String, ByVal format As String, ByVal style As String) As <System.ServiceModel.MessageParameterAttribute(Name:="fetchBatchReturn")> String
        
        <System.ServiceModel.OperationContractAttribute(Action:="", ReplyAction:="*")>  _
        Function fetchBatchAsync(ByVal db As String, ByVal ids As String, ByVal format As String, ByVal style As String) As <System.ServiceModel.MessageParameterAttribute(Name:="fetchBatchReturn")> System.Threading.Tasks.Task(Of String)
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="getSupportedDBs", WrapperNamespace:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", IsWrapped:=true)>  _
    Partial Public Class getSupportedDBsRequest
        
        Public Sub New()
            MyBase.New
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="getSupportedDBsResponse", WrapperNamespace:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", IsWrapped:=true)>  _
    Partial Public Class getSupportedDBsResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", Order:=0),  _
         System.Xml.Serialization.XmlElementAttribute("getSupportedDBsReturn")>  _
        Public getSupportedDBsReturn() As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal getSupportedDBsReturn() As String)
            MyBase.New
            Me.getSupportedDBsReturn = getSupportedDBsReturn
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="getSupportedFormats", WrapperNamespace:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", IsWrapped:=true)>  _
    Partial Public Class getSupportedFormatsRequest
        
        Public Sub New()
            MyBase.New
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="getSupportedFormatsResponse", WrapperNamespace:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", IsWrapped:=true)>  _
    Partial Public Class getSupportedFormatsResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", Order:=0),  _
         System.Xml.Serialization.XmlElementAttribute("getSupportedFormatsReturn")>  _
        Public getSupportedFormatsReturn() As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal getSupportedFormatsReturn() As String)
            MyBase.New
            Me.getSupportedFormatsReturn = getSupportedFormatsReturn
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="getSupportedStyles", WrapperNamespace:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", IsWrapped:=true)>  _
    Partial Public Class getSupportedStylesRequest
        
        Public Sub New()
            MyBase.New
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="getSupportedStylesResponse", WrapperNamespace:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", IsWrapped:=true)>  _
    Partial Public Class getSupportedStylesResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", Order:=0),  _
         System.Xml.Serialization.XmlElementAttribute("getSupportedStylesReturn")>  _
        Public getSupportedStylesReturn() As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal getSupportedStylesReturn() As String)
            MyBase.New
            Me.getSupportedStylesReturn = getSupportedStylesReturn
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="getDatabaseInfoList", WrapperNamespace:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", IsWrapped:=true)>  _
    Partial Public Class getDatabaseInfoListRequest
        
        Public Sub New()
            MyBase.New
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="getDatabaseInfoListResponse", WrapperNamespace:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", IsWrapped:=true)>  _
    Partial Public Class getDatabaseInfoListResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", Order:=0),  _
         System.Xml.Serialization.XmlElementAttribute("getDatabaseInfoListReturn")>  _
        Public getDatabaseInfoListReturn() As WSDbfetchDoclitServerServices.DatabaseInfo
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal getDatabaseInfoListReturn() As WSDbfetchDoclitServerServices.DatabaseInfo)
            MyBase.New
            Me.getDatabaseInfoListReturn = getDatabaseInfoListReturn
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="getDbFormats", WrapperNamespace:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", IsWrapped:=true)>  _
    Partial Public Class getDbFormatsRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", Order:=0)>  _
        Public db As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal db As String)
            MyBase.New
            Me.db = db
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="getDbFormatsResponse", WrapperNamespace:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", IsWrapped:=true)>  _
    Partial Public Class getDbFormatsResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", Order:=0),  _
         System.Xml.Serialization.XmlElementAttribute("getDbFormatsReturn")>  _
        Public getDbFormatsReturn() As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal getDbFormatsReturn() As String)
            MyBase.New
            Me.getDbFormatsReturn = getDbFormatsReturn
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="getFormatStyles", WrapperNamespace:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", IsWrapped:=true)>  _
    Partial Public Class getFormatStylesRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", Order:=0)>  _
        Public db As String
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", Order:=1)>  _
        Public format As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal db As String, ByVal format As String)
            MyBase.New
            Me.db = db
            Me.format = format
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(WrapperName:="getFormatStylesResponse", WrapperNamespace:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", IsWrapped:=true)>  _
    Partial Public Class getFormatStylesResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute([Namespace]:="http://www.ebi.ac.uk/ws/services/WSDbfetchDoclit", Order:=0),  _
         System.Xml.Serialization.XmlElementAttribute("getFormatStylesReturn")>  _
        Public getFormatStylesReturn() As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal getFormatStylesReturn() As String)
            MyBase.New
            Me.getFormatStylesReturn = getFormatStylesReturn
        End Sub
    End Class
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Public Interface WSDBFetchServerChannel
        Inherits WSDbfetchDoclitServerServices.WSDBFetchServer, System.ServiceModel.IClientChannel
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Partial Public Class WSDBFetchServerClient
        Inherits System.ServiceModel.ClientBase(Of WSDbfetchDoclitServerServices.WSDBFetchServer)
        Implements WSDbfetchDoclitServerServices.WSDBFetchServer
        
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
        Function WSDbfetchDoclitServerServices_WSDBFetchServer_getSupportedDBs(ByVal request As WSDbfetchDoclitServerServices.getSupportedDBsRequest) As WSDbfetchDoclitServerServices.getSupportedDBsResponse Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getSupportedDBs
            Return MyBase.Channel.getSupportedDBs(request)
        End Function
        
        Public Function getSupportedDBs() As String()
            Dim inValue As WSDbfetchDoclitServerServices.getSupportedDBsRequest = New WSDbfetchDoclitServerServices.getSupportedDBsRequest()
            Dim retVal As WSDbfetchDoclitServerServices.getSupportedDBsResponse = CType(Me,WSDbfetchDoclitServerServices.WSDBFetchServer).getSupportedDBs(inValue)
            Return retVal.getSupportedDBsReturn
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function WSDbfetchDoclitServerServices_WSDBFetchServer_getSupportedDBsAsync(ByVal request As WSDbfetchDoclitServerServices.getSupportedDBsRequest) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getSupportedDBsResponse) Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getSupportedDBsAsync
            Return MyBase.Channel.getSupportedDBsAsync(request)
        End Function
        
        Public Function getSupportedDBsAsync() As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getSupportedDBsResponse)
            Dim inValue As WSDbfetchDoclitServerServices.getSupportedDBsRequest = New WSDbfetchDoclitServerServices.getSupportedDBsRequest()
            Return CType(Me,WSDbfetchDoclitServerServices.WSDBFetchServer).getSupportedDBsAsync(inValue)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function WSDbfetchDoclitServerServices_WSDBFetchServer_getSupportedFormats(ByVal request As WSDbfetchDoclitServerServices.getSupportedFormatsRequest) As WSDbfetchDoclitServerServices.getSupportedFormatsResponse Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getSupportedFormats
            Return MyBase.Channel.getSupportedFormats(request)
        End Function
        
        Public Function getSupportedFormats() As String()
            Dim inValue As WSDbfetchDoclitServerServices.getSupportedFormatsRequest = New WSDbfetchDoclitServerServices.getSupportedFormatsRequest()
            Dim retVal As WSDbfetchDoclitServerServices.getSupportedFormatsResponse = CType(Me,WSDbfetchDoclitServerServices.WSDBFetchServer).getSupportedFormats(inValue)
            Return retVal.getSupportedFormatsReturn
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function WSDbfetchDoclitServerServices_WSDBFetchServer_getSupportedFormatsAsync(ByVal request As WSDbfetchDoclitServerServices.getSupportedFormatsRequest) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getSupportedFormatsResponse) Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getSupportedFormatsAsync
            Return MyBase.Channel.getSupportedFormatsAsync(request)
        End Function
        
        Public Function getSupportedFormatsAsync() As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getSupportedFormatsResponse)
            Dim inValue As WSDbfetchDoclitServerServices.getSupportedFormatsRequest = New WSDbfetchDoclitServerServices.getSupportedFormatsRequest()
            Return CType(Me,WSDbfetchDoclitServerServices.WSDBFetchServer).getSupportedFormatsAsync(inValue)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function WSDbfetchDoclitServerServices_WSDBFetchServer_getSupportedStyles(ByVal request As WSDbfetchDoclitServerServices.getSupportedStylesRequest) As WSDbfetchDoclitServerServices.getSupportedStylesResponse Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getSupportedStyles
            Return MyBase.Channel.getSupportedStyles(request)
        End Function
        
        Public Function getSupportedStyles() As String()
            Dim inValue As WSDbfetchDoclitServerServices.getSupportedStylesRequest = New WSDbfetchDoclitServerServices.getSupportedStylesRequest()
            Dim retVal As WSDbfetchDoclitServerServices.getSupportedStylesResponse = CType(Me,WSDbfetchDoclitServerServices.WSDBFetchServer).getSupportedStyles(inValue)
            Return retVal.getSupportedStylesReturn
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function WSDbfetchDoclitServerServices_WSDBFetchServer_getSupportedStylesAsync(ByVal request As WSDbfetchDoclitServerServices.getSupportedStylesRequest) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getSupportedStylesResponse) Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getSupportedStylesAsync
            Return MyBase.Channel.getSupportedStylesAsync(request)
        End Function
        
        Public Function getSupportedStylesAsync() As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getSupportedStylesResponse)
            Dim inValue As WSDbfetchDoclitServerServices.getSupportedStylesRequest = New WSDbfetchDoclitServerServices.getSupportedStylesRequest()
            Return CType(Me,WSDbfetchDoclitServerServices.WSDBFetchServer).getSupportedStylesAsync(inValue)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function WSDbfetchDoclitServerServices_WSDBFetchServer_getDatabaseInfoList(ByVal request As WSDbfetchDoclitServerServices.getDatabaseInfoListRequest) As WSDbfetchDoclitServerServices.getDatabaseInfoListResponse Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getDatabaseInfoList
            Return MyBase.Channel.getDatabaseInfoList(request)
        End Function
        
        Public Function getDatabaseInfoList() As WSDbfetchDoclitServerServices.DatabaseInfo()
            Dim inValue As WSDbfetchDoclitServerServices.getDatabaseInfoListRequest = New WSDbfetchDoclitServerServices.getDatabaseInfoListRequest()
            Dim retVal As WSDbfetchDoclitServerServices.getDatabaseInfoListResponse = CType(Me,WSDbfetchDoclitServerServices.WSDBFetchServer).getDatabaseInfoList(inValue)
            Return retVal.getDatabaseInfoListReturn
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function WSDbfetchDoclitServerServices_WSDBFetchServer_getDatabaseInfoListAsync(ByVal request As WSDbfetchDoclitServerServices.getDatabaseInfoListRequest) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getDatabaseInfoListResponse) Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getDatabaseInfoListAsync
            Return MyBase.Channel.getDatabaseInfoListAsync(request)
        End Function
        
        Public Function getDatabaseInfoListAsync() As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getDatabaseInfoListResponse)
            Dim inValue As WSDbfetchDoclitServerServices.getDatabaseInfoListRequest = New WSDbfetchDoclitServerServices.getDatabaseInfoListRequest()
            Return CType(Me,WSDbfetchDoclitServerServices.WSDBFetchServer).getDatabaseInfoListAsync(inValue)
        End Function
        
        Public Function getDatabaseInfo(ByVal db As String) As WSDbfetchDoclitServerServices.DatabaseInfo Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getDatabaseInfo
            Return MyBase.Channel.getDatabaseInfo(db)
        End Function
        
        Public Function getDatabaseInfoAsync(ByVal db As String) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.DatabaseInfo) Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getDatabaseInfoAsync
            Return MyBase.Channel.getDatabaseInfoAsync(db)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function WSDbfetchDoclitServerServices_WSDBFetchServer_getDbFormats(ByVal request As WSDbfetchDoclitServerServices.getDbFormatsRequest) As WSDbfetchDoclitServerServices.getDbFormatsResponse Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getDbFormats
            Return MyBase.Channel.getDbFormats(request)
        End Function
        
        Public Function getDbFormats(ByVal db As String) As String()
            Dim inValue As WSDbfetchDoclitServerServices.getDbFormatsRequest = New WSDbfetchDoclitServerServices.getDbFormatsRequest()
            inValue.db = db
            Dim retVal As WSDbfetchDoclitServerServices.getDbFormatsResponse = CType(Me,WSDbfetchDoclitServerServices.WSDBFetchServer).getDbFormats(inValue)
            Return retVal.getDbFormatsReturn
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function WSDbfetchDoclitServerServices_WSDBFetchServer_getDbFormatsAsync(ByVal request As WSDbfetchDoclitServerServices.getDbFormatsRequest) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getDbFormatsResponse) Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getDbFormatsAsync
            Return MyBase.Channel.getDbFormatsAsync(request)
        End Function
        
        Public Function getDbFormatsAsync(ByVal db As String) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getDbFormatsResponse)
            Dim inValue As WSDbfetchDoclitServerServices.getDbFormatsRequest = New WSDbfetchDoclitServerServices.getDbFormatsRequest()
            inValue.db = db
            Return CType(Me,WSDbfetchDoclitServerServices.WSDBFetchServer).getDbFormatsAsync(inValue)
        End Function
        
        Public Function getFormatInfo(ByVal db As String, ByVal format As String) As WSDbfetchDoclitServerServices.FormatInfo Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getFormatInfo
            Return MyBase.Channel.getFormatInfo(db, format)
        End Function
        
        Public Function getFormatInfoAsync(ByVal db As String, ByVal format As String) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.FormatInfo) Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getFormatInfoAsync
            Return MyBase.Channel.getFormatInfoAsync(db, format)
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function WSDbfetchDoclitServerServices_WSDBFetchServer_getFormatStyles(ByVal request As WSDbfetchDoclitServerServices.getFormatStylesRequest) As WSDbfetchDoclitServerServices.getFormatStylesResponse Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getFormatStyles
            Return MyBase.Channel.getFormatStyles(request)
        End Function
        
        Public Function getFormatStyles(ByVal db As String, ByVal format As String) As String()
            Dim inValue As WSDbfetchDoclitServerServices.getFormatStylesRequest = New WSDbfetchDoclitServerServices.getFormatStylesRequest()
            inValue.db = db
            inValue.format = format
            Dim retVal As WSDbfetchDoclitServerServices.getFormatStylesResponse = CType(Me,WSDbfetchDoclitServerServices.WSDBFetchServer).getFormatStyles(inValue)
            Return retVal.getFormatStylesReturn
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function WSDbfetchDoclitServerServices_WSDBFetchServer_getFormatStylesAsync(ByVal request As WSDbfetchDoclitServerServices.getFormatStylesRequest) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getFormatStylesResponse) Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getFormatStylesAsync
            Return MyBase.Channel.getFormatStylesAsync(request)
        End Function
        
        Public Function getFormatStylesAsync(ByVal db As String, ByVal format As String) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.getFormatStylesResponse)
            Dim inValue As WSDbfetchDoclitServerServices.getFormatStylesRequest = New WSDbfetchDoclitServerServices.getFormatStylesRequest()
            inValue.db = db
            inValue.format = format
            Return CType(Me,WSDbfetchDoclitServerServices.WSDBFetchServer).getFormatStylesAsync(inValue)
        End Function
        
        Public Function getStyleInfo(ByVal db As String, ByVal format As String, ByVal style As String) As WSDbfetchDoclitServerServices.StyleInfo Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getStyleInfo
            Return MyBase.Channel.getStyleInfo(db, format, style)
        End Function
        
        Public Function getStyleInfoAsync(ByVal db As String, ByVal format As String, ByVal style As String) As System.Threading.Tasks.Task(Of WSDbfetchDoclitServerServices.StyleInfo) Implements WSDbfetchDoclitServerServices.WSDBFetchServer.getStyleInfoAsync
            Return MyBase.Channel.getStyleInfoAsync(db, format, style)
        End Function
        
        Public Function fetchData(ByVal query As String, ByVal format As String, ByVal style As String) As String Implements WSDbfetchDoclitServerServices.WSDBFetchServer.fetchData
            Return MyBase.Channel.fetchData(query, format, style)
        End Function
        
        Public Function fetchDataAsync(ByVal query As String, ByVal format As String, ByVal style As String) As System.Threading.Tasks.Task(Of String) Implements WSDbfetchDoclitServerServices.WSDBFetchServer.fetchDataAsync
            Return MyBase.Channel.fetchDataAsync(query, format, style)
        End Function
        
        Public Function fetchBatch(ByVal db As String, ByVal ids As String, ByVal format As String, ByVal style As String) As String Implements WSDbfetchDoclitServerServices.WSDBFetchServer.fetchBatch
            Return MyBase.Channel.fetchBatch(db, ids, format, style)
        End Function
        
        Public Function fetchBatchAsync(ByVal db As String, ByVal ids As String, ByVal format As String, ByVal style As String) As System.Threading.Tasks.Task(Of String) Implements WSDbfetchDoclitServerServices.WSDBFetchServer.fetchBatchAsync
            Return MyBase.Channel.fetchBatchAsync(db, ids, format, style)
        End Function
    End Class
End Namespace

