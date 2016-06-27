Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.KEGG.WebServices
Imports LANS.SystemsBiology.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.KEGG.DBGET.ReferenceMap

    <XmlType("KEGG-RefRxnDef", Namespace:="http://code.google.com/p/genome-in-code/kegg/reference_reaction")>
    Public Class ReferenceReaction : Inherits bGetObject.Reaction

        ''' <summary>
        ''' 酶分子的直系同源的参考序列
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SSDBs As KeyValuePairObject(Of String, QueryEntry())()

        Const ENTRY_PATTERN As String = "<a href=""/dbget-bin/www_bget\?ko:K\d+"

        Public Overloads Shared Function Download(Entry As WebServices.ListEntry) As ReferenceMap.ReferenceReaction
            Dim WebForm As New WebForm(Url:=Entry.Url)

            If WebForm.Count = 0 Then
                Return Nothing
            End If

            Dim refReaction = __webFormParser(Of ReferenceMap.ReferenceReaction)(WebForm)
            Dim sValue As String = WebForm("Orthology").FirstOrDefault

            If Not String.IsNullOrEmpty(sValue) Then
                Dim OrthologyEntries = (From m As Match
                                        In Regex.Matches(sValue, ENTRY_PATTERN, RegexOptions.IgnoreCase)
                                        Select m.Value.Split(CChar(":")).Last).ToArray
                Dim GeneList = (From EntryID As String
                                In OrthologyEntries
                                Select New KeyValuePairObject(Of String, QueryEntry()) With {
                                    .Key = EntryID,
                                    .Value = DBGET.bGetObject.SSDB.API.HandleDownload(KO_ID:=EntryID)}).ToArray
                refReaction.SSDBs = GeneList
            End If

            Return refReaction
        End Function
    End Class
End Namespace