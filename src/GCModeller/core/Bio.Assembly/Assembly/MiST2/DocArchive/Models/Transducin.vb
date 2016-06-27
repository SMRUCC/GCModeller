Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.ProteinModel
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.MiST2

    ''' <summary>
    ''' Signal Transduction Protein
    ''' </summary>
    ''' <remarks></remarks>
    '''
    Public Class Transducin : Inherits Protein
        Implements sIdEnumerable

        <XmlElement> Public Property Inputs As String()
        <XmlElement> Public Property Outputs As String()
        ''' <summary>
        ''' Image file url
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property ImageUrl As String
        <XmlAttribute> Public Property GeneName As String
        <XmlAttribute> Public Property [Class] As String

        Public Overrides Function ToString() As String
            Return Identifier
        End Function

        ''' <summary>
        ''' 从DomainImage字符串之中解析出结构域数据，并返回自身
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' http://mistdb.com/arch.php?l=646;seg(115|136)+seg(242|254)+seg(517|521)+coil(266|286)+PAS_4(145|244)+HisKA(284|350)+HK_CA:3(353|508)+RR(8|127)+RR(530|646)+
        ''' </remarks>
        Public Function GetDomainArchitecture() As Protein
            Dim strTemp As String = Mid(ImageUrl, InStr(ImageUrl, ";") + 1)
            Dim Domains As String() = strTemp.Split(CChar("+"))
            Dim LQuery = (From str As String
                          In Domains.Take(Domains.Count - 1)
                          Let dm = __convert(str)
                          Select dm
                          Order By dm.Position.Left Ascending).ToArray

            MyBase.Domains = LQuery

            Return Me
        End Function

        Private Function __convert(pfam As String) As DomainObject
            Dim p As Integer = InStr(pfam, "(")
            Dim TempTokens As String() = (From m As Match
                                          In Regex.Matches(Mid(pfam, p), "\d+")
                                          Select m.Value).ToArray
            Dim Domain As DomainObject =
                New DomainObject With {
                    .Identifier = Mid(pfam, 1, p - 1)
            }
            Domain.CommonName = Domain.Identifier
            Domain.Position = New ComponentModel.Loci.Location With {
                .Left = Convert.ToInt64(TempTokens(0)),
                .Right = Convert.ToInt64(TempTokens(1))
            }
            Return Domain
        End Function
    End Class
End Namespace