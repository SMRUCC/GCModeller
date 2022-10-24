#Region "Microsoft.VisualBasic::e6ec5dabd60855ca1c761eecdde3f743, Shared\Settings.Configuration\Config\Programs\MPAlignment.vb"

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

'     Class MPAlignment
' 
'         Properties: Coverage, Evalue, FamilyAccept, FilePath, Identities
'                     MimeType, Offset, ParserThreads, ParserTimeOut
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: GetValue, Save, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Parallel.Linq

Namespace Settings.Programs

    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class MPAlignment : Implements IProfile

        <XmlAttribute> <ProfileItem("Mpl.Parser.TimeOut")> Public Property ParserTimeOut As Integer = 300
        <XmlAttribute> <ProfileItem("Mpl.Parser.Threads")> Public Property ParserThreads As Integer = 12
        <XmlAttribute>
        <ProfileItem("Mpl.Evalue", "E-value for the NCBI localblast on the domain data and domain parser.")>
        Public Property Evalue As Double = 10 ^ -5
        <XmlAttribute> <ProfileItem("Mpl.Coverage")> Public Property Coverage As Double = 0.85
        <XmlAttribute> <ProfileItem("Mpl.Identities")> Public Property Identities As Double = 0.3
        <XmlAttribute> <ProfileItem("Mpl.Offset")> Public Property Offset As Double = 0.1
        <XmlAttribute> <ProfileItem("Mpl.FamilyAccept")> Public Property FamilyAccept As Integer = 10

#Region "Interface implements"

        Private Property FilePath As String Implements IProfile.FilePath

        Public ReadOnly Property MimeType As ContentType() Implements Microsoft.VisualBasic.ComponentModel.IFileReference.MimeType
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Private Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean Implements IProfile.Save
            Return False
        End Function
#End Region

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return New Settings(Of MPAlignment)(Me).View
        End Function

        'Public Shared Function GetValue(source As Settings.File) As MPAlignment
        '    If source Is Nothing Then
        '        Return New MPAlignment
        '    ElseIf source.MPAlignment Is Nothing Then
        '        Return New MPAlignment
        '    End If

        '    Dim param As MPAlignment = source.MPAlignment
        '    If param.ParserTimeOut <= 0 Then
        '        param.ParserTimeOut = 300
        '    End If
        '    If param.ParserThreads <= 0 Then
        '        param.ParserThreads = LQuerySchedule.CPU_NUMBER
        '    End If
        '    If param.Evalue <= 0 Then
        '        param.Evalue = 10 ^ -5
        '    End If
        '    If param.Coverage >= 1 OrElse param.Coverage <= 0R Then
        '        param.Coverage = 0.85
        '    End If
        '    If param.Identities >= 1 OrElse param.Identities <= 0R Then
        '        param.Identities = 0.3
        '    End If
        '    If param.Offset >= 1 OrElse param.Offset <= 0R Then
        '        param.Offset = 0.1
        '    End If
        '    If param.FamilyAccept <= 0 Then
        '        param.FamilyAccept = 50
        '    End If

        '    Return param
        'End Function
    End Class
End Namespace
