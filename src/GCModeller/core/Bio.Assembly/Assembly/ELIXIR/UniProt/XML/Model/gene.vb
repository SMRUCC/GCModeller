Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.Uniprot.XML

    ''' <summary>
    ''' Describes a gene.
    ''' Equivalent to the flat file GN-line.
    ''' </summary>
    Public Class gene

        ''' <summary>
        ''' Describes different types of gene designations.
        ''' Equivalent to the flat file GN-line.
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("name")> Public Property names As value()
            Get
                Return table.Values _
                    .IteratesALL _
                    .ToArray
            End Get
            Set(value As value())
                If value.IsNullOrEmpty Then
                    table = New Dictionary(Of String, value())
                Else
                    ' 会有多种重复的类型
                    table = value _
                        .GroupBy(Function(name) name.type) _
                        .ToDictionary(Function(n) n.Key,
                                      Function(g)
                                          Return g.ToArray
                                      End Function)
                End If
            End Set
        End Property

        Dim table As Dictionary(Of String, value())

        Default Public ReadOnly Property IDs(type$) As String()
            Get
                If table.ContainsKey(type) Then
                    Return table(type).ValueArray
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Function HaveKey(type$) As Boolean
            Return table.ContainsKey(type)
        End Function

        ''' <summary>
        ''' (primary) 基因名称
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Primary As String()
            Get
                If table.ContainsKey("primary") Then
                    Return table("primary").ValueArray
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' (ORF) 基因编号
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ORF As String()
            Get
                ' ORF 和 locus编号的含义是一样的

                If table.ContainsKey("ORF") Then
                    Return table("ORF").ValueArray
                ElseIf table.ContainsKey("ordered locus") Then
                    Return table("ordered locus").ValueArray
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace