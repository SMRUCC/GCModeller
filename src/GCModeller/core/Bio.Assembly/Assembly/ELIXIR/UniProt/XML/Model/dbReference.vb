Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.Uniprot.XML

    Public Class dbReference : Implements INamedValue

        ''' <summary>
        ''' 外部数据库的名称
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 对于RefSeq而言，RefSeq的编号是蛋白序列在NCBI数据库之中的编号，如果需要找到对应的核酸编号，
        ''' 则会需要通过<see cref="properties"/>列表之中的``nucleotide sequence ID``键值对来获取
        ''' </remarks>
        <XmlAttribute> Public Property type As String Implements INamedValue.Key
        ''' <summary>
        ''' 外部数据库的编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property id As String
        <XmlElement("property")>
        Public Property properties As [property]()
        Public Property molecule As molecule

        Default Public ReadOnly Property PropertyValue(name$) As String
            Get
                Return properties _
                    .SafeQuery _
                    .Where(Function([property]) [property].type = name) _
                    .FirstOrDefault _
                   ?.value
            End Get
        End Property

        Public Function hasDbReference(dbName As String) As Boolean
            Return Not properties _
                .SafeQuery _
                .Where(Function([property])
                           Return [property].type = dbName
                       End Function) _
                .FirstOrDefault Is Nothing
        End Function

        Public Overrides Function ToString() As String
            Return $"[{type}] {id}"
        End Function

    End Class

End Namespace