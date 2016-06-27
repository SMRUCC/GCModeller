Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.KEGG.WebServices
Imports LANS.SystemsBiology.Assembly.MetaCyc.Schema
Imports LANS.SystemsBiology.ComponentModel.DBLinkBuilder
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG.DBGET.bGetObject

    <XmlRoot("KEGG.Glycan", Namespace:="http://www.kegg.jp/dbget-bin/www_bget?gl:glycan_id")>
    Public Class Glycan : Inherits Compound
        Implements ICompoundObject

        Public Property Composition As String
        Public Property Mass As String

        Dim _DBLinks As DBLinks

        Public Property Reactions As String()

        Const URL = "http://www.kegg.jp/dbget-bin/www_bget?gl:{0}"

        Public Overloads Shared Function Download(Id As String) As Glycan
            Return DownloadFrom(url:=String.Format(URL, Id))
        End Function

        Public Overloads Shared Function DownloadFrom(url As String) As Glycan
            Dim WebForm As New KEGG.WebServices.InternalWebFormParsers.WebForm(url)
            Dim Compound As Glycan = New Glycan With {
                .Entry = Regex.Match(WebForm.GetValue("Entry").FirstOrDefault, "[GC]\d+").Value
            }
            Compound.CommonNames = KEGG.DBGET.bGetObject.Compound.GetCommonNames(WebForm.GetValue("Name").FirstOrDefault())
            Compound.Composition = WebForm.GetValue("Composition").FirstOrDefault.Replace("<br>", "")
            Compound.Reactions = KEGG.DBGET.bGetObject.Compound.GetReactionList(WebForm.GetValue("Reaction").FirstOrDefault)
            Compound.Pathway =
                LinqAPI.Exec(Of String) <= From x As KeyValuePair
                                           In InternalWebFormParsers.WebForm.parseList(WebForm.GetValue("Pathway").FirstOrDefault, "<a href="".*/kegg-bin/show_pathway\?.+?"">.+?</a>")
                                           Select String.Format("[{0}] {1}", x.Key, x.Value)
            Compound.Module =
                LinqAPI.Exec(Of String) <= From x As KeyValuePair
                                           In InternalWebFormParsers.WebForm.parseList(WebForm.GetValue("Module").FirstOrDefault, "<a href="".*/kegg-bin/show_module\?.+?"">.+?</a>")
                                           Select String.Format("[{0}] {1}", x.Key, x.Value)
            Compound._DBLinks = KEGG.DBGET.bGetObject.Compound.GetDBLinks(WebForm.GetValue("Other DBs").FirstOrDefault)
            Compound.Mass = Val(WebForm.GetValue("Mass").FirstOrDefault)

            If Compound.CommonNames.IsNullOrEmpty Then
                Compound.CommonNames = New String() {Compound.Composition}
            End If

            Return Compound
        End Function

        Public Function ToCompound() As Compound
            Return New Compound With {
                .Entry = Entry,
                .CommonNames = CommonNames,
                .DbLinks = DbLinks,
                .Formula = Me.Composition,
                .InvolvedReactions = Reactions,
                .Module = Me.Module,
                .MolWeight = Val(Mass),
                .Pathway = Pathway
            }
        End Function
    End Class
End Namespace