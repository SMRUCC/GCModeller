#Region "Microsoft.VisualBasic::ba054ac5ee54b966bc51950449599ca6, Shared\Settings.Configuration\Config\Programs\IDE.vb"

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

'     Class IDE
' 
' 
'         Enum Languages
' 
' 
' 
' 
'         Class StartPageF
' 
'             Properties: CloseAfterProjectLoad, IDE, Session, ShowOnStartUp, StartPage
' 
'             Function: (+2 Overloads) [Default]
' 
'         Class IDEConfig
' 
'             Properties: Language, Location, Size
' 
'             Function: [Default]
'             Class PointF
' 
'                 Properties: Left, Top
' 
'             Class SizeF
' 
'                 Properties: Height, Width
' 
' 
' 
'         Class SessionF
' 
'             Properties: LoadLastSessionAfterStartUp, ProjectFile
' 
'             Function: [Default]
' 
'  
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Settings

Namespace Settings.Programs

    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class IDE

        ''' <summary>
        ''' Enum the mainly used language.
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum Languages
            ''' <summary>
            ''' Following the system default language.
            ''' </summary>
            ''' <remarks></remarks>
            <Description("System")> System

            ''' <summary>
            ''' Language in Chinese simplify.
            ''' (简体中文) 
            ''' </summary>
            ''' <remarks></remarks>
            <Description("zh-CN")> ZhCN

            ''' <summary>
            ''' Language in English.
            ''' (英语语言)
            ''' </summary>
            ''' <remarks></remarks>
            <Description("en-US")> EnUS

            ''' <summary>
            ''' Language in french.
            ''' (法语语言)
            ''' </summary>
            ''' <remarks></remarks>
            <Description("fr-FR")> FrFR
        End Enum


        <ProfileNodeItem> Public Property StartPage As Settings.Programs.IDE.StartPageF
        <ProfileNodeItem> Public Property IDE As Settings.Programs.IDE.IDEConfig
        <ProfileNodeItem> Public Property Session As Settings.Programs.IDE.SessionF

        ''' <summary>
        ''' save the <see cref="IDE"/> window size and location configuration data
        ''' and then apply to the window at startup?
        ''' </summary>
        ''' <returns></returns>
        Public Property RememberWindowStatus As Boolean

        Public Shared Function [Default]() As Settings.Programs.IDE
            Return New IDE With {
                .RememberWindowStatus = True,
                .StartPage = Settings.Programs.IDE.StartPageF.Default,
                .IDE = Settings.Programs.IDE.IDEConfig.Default,
                .Session = Settings.Programs.IDE.SessionF.Default
            }
        End Function

        <ClassInterface(ClassInterfaceType.AutoDual)>
        <ComVisible(True)>
        Public Class StartPageF
            <ProfileItem> <XmlElement> Public Property CloseAfterProjectLoad As Boolean
            <ProfileItem> <XmlElement> Public Property ShowOnStartUp As Boolean

            Public Shared Function [Default]() As StartPageF
                Return New StartPageF With {.ShowOnStartUp = True, .CloseAfterProjectLoad = True}
            End Function
        End Class

        <ClassInterface(ClassInterfaceType.AutoDual)>
        <ComVisible(True)>
        Public Class IDEConfig
            <ProfileItem> <XmlElement> Public Property Location As PointF
            <ProfileItem> <XmlElement> Public Property Size As SizeF
            <ProfileItem> <XmlElement> Public Property Language As Languages

            Public Shared Function [Default]() As IDEConfig
                Dim config As IDEConfig = New IDE.IDEConfig With {
                    .Size = New SizeF With {
                        .Width = 800,
                        .Height = 600
                    },
                    .Location = New PointF With {
                        .Left = 0,'My.Computer.Screen.Bounds.Width * 0.3,
                        .Top = 0'My.Computer.Screen.Bounds.Height * 0.4
                    },
                    .Language = Languages.System
                }

                Return config
            End Function

            <ClassInterface(ClassInterfaceType.AutoDual)>
            <ComVisible(True)>
            Public Class PointF
                <XmlAttribute> Public Property Left As Double
                <XmlAttribute> Public Property Top As Double
            End Class

            <ClassInterface(ClassInterfaceType.AutoDual)>
            <ComVisible(True)>
            Public Class SizeF
                <XmlAttribute> Public Property Width As Integer
                <XmlAttribute> Public Property Height As Integer
            End Class
        End Class

        <ClassInterface(ClassInterfaceType.AutoDual)>
        <ComVisible(True)>
        Public Class SessionF
            <ProfileItem> <XmlElement> Public Property ProjectFile As String
            <ProfileItem> <XmlAttribute> Public Property LoadLastSessionAfterStartUp As Boolean = False

            Public Shared Function [Default]() As SessionF
                Return New SessionF With {
                    .LoadLastSessionAfterStartUp = False,
                    .ProjectFile = $"{App.HOME}/Projs/Default.gcproject.xml"
                }
            End Function
        End Class
    End Class
End Namespace
