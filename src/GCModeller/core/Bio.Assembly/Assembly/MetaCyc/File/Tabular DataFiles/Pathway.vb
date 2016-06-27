Imports System.Text
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.DataTabular
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.TabularDataFiles

    ''' <summary>
    ''' 代谢途径对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Pathway

        ''' <summary>
        ''' 这个途径对象在PGDB数据库中的唯一标识符
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property UniqueId As String
        ''' <summary>
        ''' 途径的名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property DisplayingName As String

        ''' <summary>
        ''' 与其他的数据库的对象连接关系
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlArray("GENE-Association")>
        Public Property GeneCollection As List(Of GeneLink)

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder(256)

            sBuilder.Append("(Pathway) " & UniqueId)
            sBuilder.AppendLine("; [" & DisplayingName & "]")
            sBuilder.AppendLine("GENES associated:")
            For Each link As GeneLink In GeneCollection
                sBuilder.AppendLine(link.ToString)
            Next

            Return sBuilder.ToString
        End Function

        Public Shared Widening Operator CType(e As String()) As Pathway
            Dim newPwy As New Pathway

            With newPwy
                .UniqueId = e(0)
                .DisplayingName = e(1)
                .GeneCollection = New List(Of GeneLink)

                Dim n As Integer = 1 + (e.Length - 2) / 2
                For i As Integer = 2 To n
                    .GeneCollection += New GeneLink With {
                        .GENEId = e(i),
                        .CGSCId = e(i + n - 1)
                    }
                    If String.IsNullOrEmpty(e(i + 1)) Then
                        Exit For
                    End If
                Next
            End With

            Return newPwy
        End Operator
    End Class

End Namespace