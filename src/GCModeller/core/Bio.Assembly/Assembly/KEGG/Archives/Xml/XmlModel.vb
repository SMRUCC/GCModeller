Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.KEGG.Archives.Xml.Nodes
Imports LANS.SystemsBiology.Assembly.KEGG.DBGET
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Assembly.KEGG.Archives.Xml

    ''' <summary>
    ''' The compile data of the target bacteria species genome database in the KEGG.(KEGG数据库之中的关于目标研究的微生物菌株的KEGG数据库之中的所有的信息的编译的集合)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <XmlType("KEGG.CellMaps")>
    Public Class XmlModel : Implements ISaveHandle

        Dim __reactions As bGetObject.Reaction()
        Dim _metaHash As Dictionary(Of String, bGetObject.Reaction)

        ''' <summary>
        ''' The reaction information which can be annotated in this bacteria species.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlArray("Reactions")>
        Public Property Metabolome As bGetObject.Reaction()
            Get
                Return __reactions
            End Get
            Set(value As bGetObject.Reaction())
                __reactions = value

                If __reactions.IsNullOrEmpty Then
                    _metaHash = New Dictionary(Of String, bGetObject.Reaction)
                Else
                    _metaHash = (From x As bGetObject.Reaction
                                 In __reactions
                                 Select x
                                 Group x By x.Entry Into Group) _
                                      .ToDictionary(Function(x) x.Entry, 
                                                    Function(x) x.Group.First)
                End If
            End Set
        End Property

        <XmlArray("Modules")>
        Public Property Modules As bGetObject.Module()
        <XmlArray("CellPhenotypes")>
        Public Property Pathways As PwyBriteFunc()
        <XmlArray("EC.Maps")>
        Public Property EC_Mappings As EC_Mapping()
            Get
                Return _maps
            End Get
            Set(value As EC_Mapping())
                _maps = value

                If _maps.IsNullOrEmpty Then
                    _mapsHash = New Dictionary(Of EC_Mapping)
                Else
                    _mapsHash = value.ToDictionary
                End If
            End Set
        End Property

        ''' <summary>
        ''' The kegg species brief code.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute("sp.Code")> Public Property spCode As String

        Dim _mapsHash As Dictionary(Of EC_Mapping)
        Dim _maps As EC_Mapping()

        Public Function GetReaction(rxn As String) As bGetObject.Reaction
            If _metaHash.ContainsKey(rxn) Then
                Return _metaHash(rxn)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 获取得到某一个基因所映射得到的酶促反应过程
        ''' </summary>
        ''' <param name="locus">基因号</param>
        ''' <returns></returns>
        Public Function GetMaps(locus As String) As EC_Mapping
            If _mapsHash.ContainsKey(locus) Then
                Return _mapsHash(locus)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Gets the pathway information data from this species genome compile data.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAllPathways() As bGetObject.Pathway()
            Dim LQuery = Pathways.ToArray(Function(x) x.Pathways).MatrixToVector
            Return LQuery
        End Function

        Public Function Save(Optional Path As String = "", Optional encoding As Text.Encoding = Nothing) As Boolean Implements ISaveHandle.Save
            If String.IsNullOrEmpty(Path) Then
                Throw New Exception("Path is empty!")
            End If

            Return Me.GetXml.SaveTo(Path, encoding)
        End Function

        Public Function Save(Optional Path As String = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(Path, encoding.GetEncodings)
        End Function
    End Class
End Namespace