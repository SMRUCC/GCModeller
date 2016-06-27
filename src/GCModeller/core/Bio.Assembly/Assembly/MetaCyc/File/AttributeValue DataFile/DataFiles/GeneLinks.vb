Imports System.Text
Imports System.Xml.Serialization

Namespace Assembly.MetaCyc.File.DataFiles.DataTabular

    Public Class GeneLinks

        <XmlArray("Links")> Public Property Objects As GeneLink()

        Public Shared Widening Operator CType(Path As String) As GeneLinks
            Dim File As String() = Nothing
            Dim prop As [Property] = Nothing
            Call FileReader.TabularParser(Path, prop, File, "").Assertion(Logging.MSG_TYPES.WRN)
            Dim LinkSource As IEnumerable(Of GeneLink) = From s As String
                                                         In File.AsParallel
                                                         Select CType(s, GeneLink)
            Return New GeneLinks With {
                .Objects = LinkSource.ToArray
            }
        End Operator
    End Class

    ''' <summary>
    ''' 在不同的数据库之间交换数据所需要的对象连接映射，即由PGDB中的Unique映射至通用基因号的关系对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure GeneLink

        <XmlAttribute> Dim GENEId As String
        <XmlAttribute> Dim CGSCId As String
        <XmlAttribute> Dim UniProtId As String
        <XmlAttribute> Dim GeneName As String

        Public Shared Widening Operator CType(s As String) As GeneLink
            Dim GeneLink As GeneLink = New GeneLink
            Dim Data As String() = s.Split(CChar(vbTab))

            GeneLink.GENEId = Data(0)
            GeneLink.CGSCId = Data(1)
            GeneLink.UniProtId = Data(2)
            GeneLink.GeneName = Data(3)

            Return GeneLink
        End Operator

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder(512)

            Append(GENEId, sBuilder)
            Append(CGSCId, sBuilder)
            Append(UniProtId, sBuilder)
            Append(GeneName, sBuilder)

            Return sBuilder.ToString
        End Function

        Friend Sub Append(e As String, ByRef sbr As StringBuilder)
            If String.IsNullOrEmpty(e) Then
                sbr.Append("NULL, ")
            Else
                sbr.Append(String.Format("{0}, ", e))
            End If
        End Sub
    End Structure
End Namespace