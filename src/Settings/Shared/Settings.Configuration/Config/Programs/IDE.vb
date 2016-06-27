Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports System.Drawing

Namespace Settings.Programs

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

        Public Shared Function [Default]() As Settings.Programs.IDE
            Dim DefaultProfile As Programs.IDE = New IDE

            DefaultProfile.StartPage = Settings.Programs.IDE.StartPageF.Default
            DefaultProfile.IDE = Settings.Programs.IDE.IDEConfig.Default
            DefaultProfile.Session = Settings.Programs.IDE.SessionF.Default

            Return DefaultProfile
        End Function

        Public Class StartPageF
            <ProfileItem> <XmlElement> Public Property CloseAfterProjectLoad As Boolean
            <ProfileItem> <XmlElement> Public Property ShowOnStartUp As Boolean

            Public Shared Function [Default]() As StartPageF
                Return New StartPageF With {.ShowOnStartUp = True, .CloseAfterProjectLoad = True}
            End Function
        End Class

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
                        .Left = My.Computer.Screen.Bounds.Width * 0.3,
                        .Top = My.Computer.Screen.Bounds.Height * 0.4
                    },
                    .Language = Languages.System
                }

                Return config
            End Function

            Public Class PointF
                <XmlAttribute> Public Property Left As Double
                <XmlAttribute> Public Property Top As Double
            End Class

            Public Class SizeF
                <XmlAttribute> Public Property Width As Integer
                <XmlAttribute> Public Property Height As Integer
            End Class
        End Class

        Public Class SessionF
            <ProfileItem> <XmlElement> Public Property ProjectFile As String
            <ProfileItem> <XmlAttribute> Public Property LoadLastSessionAfterStartUp As Boolean = False

            Public Shared Function [Default]() As SessionF
                Return New SessionF With {
                    .LoadLastSessionAfterStartUp = False,
                    .ProjectFile = My.Application.Info.DirectoryPath & "/Projs/Default.gcproject.xml"
                }
            End Function
        End Class
    End Class
End Namespace