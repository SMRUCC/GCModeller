#Region "Microsoft.VisualBasic::f67cebe99b1cb58888f33072cd066e18, foundation\PSICQUIC\psidev\XML\XmlCommon.vb"

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

    '   Total Lines: 107
    '    Code Lines: 36
    ' Comment Lines: 57
    '   Blank Lines: 14
    '     File Size: 4.11 KB


    '     Class DataItem
    ' 
    '         Properties: id
    ' 
    '         Function: ToString
    ' 
    '     Class Names
    ' 
    '         Properties: [alias], fullName, shortLabel
    ' 
    '         Function: ToString
    ' 
    '     Class Xref
    ' 
    '         Properties: primaryRef, secondaryRef
    ' 
    '         Function: ToString
    ' 
    '     Class Reference
    ' 
    '         Properties: db, dbAc, id, refType, refTypeAc
    '                     secondary, version
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace XML

    Public MustInherit Class DataItem

        <XmlAttribute> Public Overridable Property id As Integer

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Class

    ''' <summary>
    ''' Names for an object.
    ''' </summary>
    Public Class Names

        ''' <summary>
        ''' A short string, suitable to remember the object. Can be e.g. a gene name, the first author of a paper, etc.
        ''' </summary>
        ''' <returns></returns>
        Public Property shortLabel As String
        ''' <summary>
        ''' A full, detailed name or description of the object. Can be e.g. the full title of a publication, or the scientific name of a species.
        ''' </summary>
        ''' <returns></returns>
        Public Property fullName As String
        Public Property [alias] As String

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}", shortLabel, fullName)
        End Function
    End Class

    ''' <summary>
    ''' Crossreference to an external database. Crossreferences to literature databases, e.g. PubMed, should not be put into
    ''' this Structure, but into the bibRef element where possible.
    ''' </summary>
    Public Class Xref

        ''' <summary>
        ''' Primary reference to an external database.
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property primaryRef As Reference
        ''' <summary>
        ''' Further external objects describing the object.
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property secondaryRef As Reference()

        Public Overrides Function ToString() As String
            Return primaryRef.ToString
        End Function
    End Class

    ''' <summary>
    ''' Refers to a unique object in an external database.
    ''' </summary>
    Public Class Reference : Inherits DataItem

        ''' <summary>
        ''' Primary identifier of the object in the external database, e.g. UniProt accession number.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Overrides Property id As Integer

        ''' <summary>
        ''' Name of the external database. Taken from the controlled vocabulary of databases.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property db As String
        ''' <summary>
        ''' Accession number of the database in the database CV. This element is controlled by the PSI-MI controlled
        ''' vocabulary "database citation", root term id MI0444.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property dbAc As String
        ''' <summary>
        ''' Reference type, e.g. "identity" if this reference referes to an identical object in the external database,
        ''' Or "see-also" for additional information. Controlled by CV.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property refType As String
        ''' <summary>
        ''' Reference type accession number from the CV of reference types. This element is controlled by the PSI-MI
        ''' controlled vocabulary "xref type", root term id MI:0353.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property refTypeAc As String
        ''' <summary>
        ''' Secondary identifier of the object in the external database, e.g. UniProt ID.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property secondary As String
        ''' <summary>
        ''' The version number of the object in the external database.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property version As String

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", db, id)
        End Function
    End Class
End Namespace
