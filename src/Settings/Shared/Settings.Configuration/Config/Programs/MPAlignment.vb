Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Parallel.Linq

Namespace Settings.Programs

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
        Private Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean Implements IProfile.Save
            Return False
        End Function
#End Region

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return New Settings(Of MPAlignment)(Me).View
        End Function

        Public Shared Function GetValue(source As Settings.File) As MPAlignment
            If source Is Nothing Then
                Return New MPAlignment
            ElseIf source.MPAlignment Is Nothing Then
                Return New MPAlignment
            End If

            Dim param As MPAlignment = source.MPAlignment
            If param.ParserTimeOut <= 0 Then
                param.ParserTimeOut = 300
            End If
            If param.ParserThreads <= 0 Then
                param.ParserThreads = LQuerySchedule.CPU_NUMBER
            End If
            If param.Evalue <= 0 Then
                param.Evalue = 10 ^ -5
            End If
            If param.Coverage >= 1 OrElse param.Coverage <= 0R Then
                param.Coverage = 0.85
            End If
            If param.Identities >= 1 OrElse param.Identities <= 0R Then
                param.Identities = 0.3
            End If
            If param.Offset >= 1 OrElse param.Offset <= 0R Then
                param.Offset = 0.1
            End If
            If param.FamilyAccept <= 0 Then
                param.FamilyAccept = 50
            End If

            Return param
        End Function
    End Class
End Namespace