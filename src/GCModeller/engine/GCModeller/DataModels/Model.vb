Namespace DataModels

    Public Class Model : Inherits LANS.SystemsBiology.Assembly.Xml.Model

        ''' <summary>
        ''' 模拟计算的运行时间
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Xml.Serialization.XmlAttribute> Public Property Time As Long

        Public Overrides Sub Save(Optional File As String = "")
            Dim Xml As String = GNU.Linux.VisualBasic.Compatibility.Stdio.GetXml(Of Model)(Me)
            Call FileIO.FileSystem.WriteAllText(File, Xml, append:=False)
        End Sub
    End Class
End Namespace