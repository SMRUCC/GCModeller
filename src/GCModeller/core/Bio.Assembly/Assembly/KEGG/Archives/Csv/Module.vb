Imports System.Data.Linq.Mapping
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG.Archives.Csv

    Public Class [Module] : Inherits ComponentModel.PathwayBrief
        Implements IKeyValuePairObject(Of String, String())

        Public Overrides Property EntryId As String Implements IKeyValuePairObject(Of String, String()).Identifier
            Get
                Return MyBase.EntryId
            End Get
            Set(value As String)
                MyBase.EntryId = value
            End Set
        End Property

        <Column(Name:="Num.Genes")> Public ReadOnly Property NumberGenes As Integer
            Get
                Return PathwayGenes.Length
            End Get
        End Property

        Public Property PathwayGenes As String() Implements IKeyValuePairObject(Of String, String()).Value

        Public Overrides Function GetPathwayGenes() As String()
            Return PathwayGenes
        End Function

#Region "Brite Infomation"

        ''' <summary>
        ''' A
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="br.Type")> Public Property Type As String
        ''' <summary>
        ''' B
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="br.Class")>
        Public Property [Class] As String
        ''' <summary>
        ''' C
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="br.Category")>
        Public Property Category As String
#End Region

        Public Property Name As String
        Public Property Reactions As String()

        Public Overrides ReadOnly Property BriteId As String
            Get
                Return EntryId.Split("_"c).Last
            End Get
        End Property

        Public Shared Function GenerateObject(XmlModel As KEGG.DBGET.bGetObject.Module) As [Module]
            Dim ReactionIdlist As String()

            If XmlModel.Reaction.IsNullOrEmpty Then
                ReactionIdlist = New String() {}
            Else
                ReactionIdlist =
                    LinqAPI.Exec(Of String) <= From rxn As KeyValuePair
                                               In XmlModel.Reaction
                                               Select rxn.Key
                                               Order By Key Ascending
            End If

            Return New [Module] With {
                .EntryId = XmlModel.EntryId,
                .Description = XmlModel.Description,
                .PathwayGenes = XmlModel.GetPathwayGenes,
                .Name = XmlModel.Name,
                .Reactions = ReactionIdlist
            }
        End Function

        ''' <summary>
        ''' 导出XML文件之中的数据至Csv文件之中
        ''' </summary>
        ''' <typeparam name="TSource"></typeparam>
        ''' <param name="Data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function [Imports](Of TSource As IEnumerable(Of KEGG.DBGET.bGetObject.Module))(Data As TSource) As [Module]()
            Dim mods = (From kmod As KEGG.DBGET.bGetObject.Module
                        In Data
                        Select GenerateObject(kmod)).ToArray
            Dim defBr = KEGG.DBGET.BriteHEntry.Module.GetDictionary

            For Each kmod As [Module] In mods
                Dim brMod = defBr(kmod.BriteId)
                kmod.Type = brMod.Class
                kmod.Class = brMod.Category
                kmod.Category = brMod.SubCategory
            Next

            Return mods
        End Function
    End Class
End Namespace