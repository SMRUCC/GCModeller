#Region "Microsoft.VisualBasic::f1c7d39b12544675f148403b6ee41f8a, GCModeller\core\Bio.Assembly\Assembly\DOMINE\Tables.vb"

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

    '   Total Lines: 221
    '    Code Lines: 144
    ' Comment Lines: 66
    '   Blank Lines: 11
    '     File Size: 7.10 KB


    '     Class Interaction
    ' 
    '         Properties: _3did, [ME], DIPD, Domain1, Domain2
    '                     DomainGA, DPEA, Fusion, GPE, INSITE
    '                     iPfam, KGIDDI, MetaData, PE, PP
    '                     PredictionConfidence, Pvalue, RCDP, RDFF, SameGO
    ' 
    '         Function: GetInteractionDomain, ToString
    ' 
    '     Class Pfam
    ' 
    '         Properties: DomainAcc, DomainDesc, DomainId, InterproId
    ' 
    '         Function: ToString
    ' 
    '     Class Go
    ' 
    '         Properties: GoDesc, GoTerm, Ontology
    ' 
    '         Function: ToString
    ' 
    '     Class PGMap
    ' 
    '         Properties: DomainAcc, GoTerm
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace Assembly.DOMINE.Tables

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE INTERACTION
    ''' (
    ''' Domain1 char(7),
    ''' Domain2 char(7),
    ''' iPfam boolean,
    ''' 3did boolean,
    ''' ME boolean,
    ''' RCDP boolean,
    ''' Pvalue boolean,
    ''' Fusion boolean,
    ''' DPEA boolean,
    ''' PE boolean,
    ''' GPE boolean,
    ''' DIPD boolean,
    ''' RDFF boolean,
    ''' KGIDDI boolean,
    ''' INSITE boolean,
    ''' DomainGA boolean,
    ''' PP boolean,
    ''' PredictionConfidence char(2),
    ''' SameGO boolean,
    ''' PRIMARY KEY (Domain1, Domain2),
    ''' FOREIGN KEY (Domain1) references PFAM(DomainAcc),
    ''' FOREIGN KEY (DOmain2) references PFAM(DomainAcc)
    ''' );
    ''' </remarks>
    Public Class Interaction
        <XmlAttribute> Public Property Domain1 As String
        <XmlAttribute> Public Property Domain2 As String
        <XmlAttribute> Public Property MetaData As Integer()

        <XmlAttribute> Public ReadOnly Property iPfam As Boolean
            Get
                Return CType(MetaData(0), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property _3did As Boolean
            Get
                Return CType(MetaData(1), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property [ME] As Boolean
            Get
                Return CType(MetaData(2), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property RCDP As Boolean
            Get
                Return CType(MetaData(3), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property Pvalue As Boolean
            Get
                Return CType(MetaData(4), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property Fusion As Boolean
            Get
                Return CType(MetaData(5), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property DPEA As Boolean
            Get
                Return CType(MetaData(6), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property PE As Boolean
            Get
                Return CType(MetaData(7), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property GPE As Boolean
            Get
                Return CType(MetaData(8), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property DIPD As Boolean
            Get
                Return CType(MetaData(9), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property RDFF As Boolean
            Get
                Return CType(MetaData(10), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property KGIDDI As Boolean
            Get
                Return CType(MetaData(11), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property INSITE As Boolean
            Get
                Return CType(MetaData(12), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property DomainGA As Boolean
            Get
                Return CType(MetaData(13), Boolean)
            End Get
        End Property
        <XmlAttribute> Public ReadOnly Property PP As Boolean
            Get
                Return CType(MetaData(14), Boolean)
            End Get
        End Property
        <XmlAttribute> Public Property PredictionConfidence As String
        <XmlAttribute> Public ReadOnly Property SameGO As Boolean
            Get
                Return CType(MetaData(15), Boolean)
            End Get
        End Property

        Public Function GetInteractionDomain(DomainId As String) As String
            If String.Equals(Domain1, DomainId) Then
                Return Domain2
            ElseIf String.Equals(Domain2, DomainId) Then
                Return Domain1
            Else
                Return ""
            End If
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}",
                                 Domain1,
                                 Domain2(),
                                 iPfam(),
                                 _3did(),
                                 [ME](),
                                 RCDP(),
                                 Pvalue(),
                                 Fusion(),
                                 DPEA(),
                                 PE(),
                                 GPE(),
                                 DIPD(),
                                 RDFF,
                                 KGIDDI(),
                                 INSITE(),
                                 DomainGA(),
                                 PP(),
                                 PredictionConfidence,
                                 SameGO)
        End Function
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE PFAM
    ''' (
    ''' DomainAcc char(7) PRIMARY KEY,
    ''' DomainId varchar(256),
    ''' DomainDesc varchar(256),
    ''' InterproId char(10)
    ''' );
    ''' </remarks>
    Public Class Pfam
        <XmlAttribute> Public Property DomainAcc As String
        <XmlAttribute> Public Property DomainId As String
        <XmlElement> Public Property DomainDesc As String
        <XmlAttribute> Public Property InterproId As String

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}", DomainAcc, DomainId)
        End Function
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE GO
    ''' (
    ''' GoTerm char(10) PRIMARY KEY,
    ''' Ontology varchar(256),
    ''' GoDesc varchar(256)
    ''' );
    ''' </remarks>
    Public Class Go
        <XmlAttribute> Public Property GoTerm As String
        <XmlAttribute> Public Property Ontology As String
        <XmlElement> Public Property GoDesc As String

        Public Overrides Function ToString() As String
            Return String.Format("({0}){1}: {2}", Ontology, GoTerm, GoDesc)
        End Function
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATE TABLE PGMAP
    ''' (
    ''' DomainAcc char(7),
    ''' GoTerm char(10),
    ''' PRIMARY KEY (DomainAcc, GoTerm),
    ''' FOREIGN KEY (DomainAcc) references PFAM(DomainAcc),
    ''' FOREIGN KEY (GoTerm) references GO(GoTerm)
    ''' );
    ''' </remarks>
    Public Class PGMap
        <XmlAttribute> Public Property DomainAcc As String
        <XmlAttribute> Public Property GoTerm As String

        Public Overrides Function ToString() As String
            Return String.Format("[{0}, {1}]", GoTerm, DomainAcc)
        End Function
    End Class
End Namespace
