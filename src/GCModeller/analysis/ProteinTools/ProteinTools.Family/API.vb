#Region "Microsoft.VisualBasic::b28efa62e3624946ba3b87f81ac69ead, analysis\ProteinTools\ProteinTools.Family\API.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module API
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: __getSavePath, __trim, (+2 Overloads) FamilyAlign, (+2 Overloads) FamilyDomains, FamilyStat
    '               SaveDb, SaveRepository
    '     Class AnnotationOut
    ' 
    '         Properties: Family, LocusId, PfamString
    ' 
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.ProteinTools.Family.FileSystem
Imports SMRUCC.genomics.Data

<Package("SMART.PfamFamily",
                  Category:=APICategories.ResearchTools,
                  Description:="Protein family category using Motif Parallel Alignment method.",
                  Publisher:="xie.guigang@gcmodeller.org")>
Public Module API

    ''' <summary>
    ''' 需要兼容KEGG和自己的Regprecise数据库_(:зゝ∠)_
    ''' </summary>
    ''' <param name="KEGG"></param>
    ''' <param name="Pfam"></param>
    ''' <returns></returns>
    <ExportAPI("FamilyDomain.Dumps", Info:="Dump the family database for the further analysis.")>
    Public Function FamilyDomains(KEGG As SequenceModel.FASTA.FastaFile,
                                  Pfam As IEnumerable(Of Xfam.Pfam.PfamString.PfamString)) As FamilyPfam
        Return Family.KEGG.FamilyDomains(KEGG, Pfam)
    End Function

    <ExportAPI("FamilyDomain.Dumps", Info:="Dump the family database for the further analysis.")>
    Public Function FamilyDomains(Regprecise As Dictionary(Of String, Regprecise.FastaReaders.Regulator),
                                  Pfam As Generic.IEnumerable(Of Xfam.Pfam.PfamString.PfamString)) As FamilyPfam

        Pfam = (From x In Pfam.AsParallel Where Not x.PfamString.IsNullOrEmpty Select x).AsList

        Dim LQuery = (From x As Xfam.Pfam.PfamString.PfamString In Pfam
                      Let entry = Regprecise(x.ProteinId)
                      Let family As String = entry.KEGGFamily.Trim
                      Let describ = entry.Definition
                      Let stringPfam = PfamString.CreateObject(x)
                      Select stringPfam, family.ToLower, family, describ
                      Group By ToLower Into Group) _
                         .Select(Function(x) FileSystem.Family.CreateObject(
                            x.Group.First.family,
                            x.Group.Select(Function(xx) xx.stringPfam),
                            x.Group.Select(Function(xx) New KeyValuePair With {
                                .Key = xx.stringPfam.LocusTag,
                                .Value = xx.describ})))
        Dim FamilyDb As New FamilyPfam With {
            .Build = Now.ToString,
            .Family = LQuery
        }
        Return FamilyDb
    End Function

    <ExportAPI("Write.Repository")>
    Public Function SaveRepository(FamilyDb As FamilyPfam, Name As String) As Boolean
        Dim DbIO As New Database
        Return Not String.IsNullOrEmpty(DbIO.Add(Name, FamilyDb))
    End Function

    <ExportAPI("Write.Xml.FamilyDb")>
    Public Function SaveDb(FamilyDb As FamilyPfam, Optional saveXml As String = "") As Boolean
        Return FamilyDb.GetXml.SaveTo(__getSavePath(saveXml)).CLICode
    End Function

    Private Function __getSavePath(saveXml As String) As String
        If Not String.IsNullOrEmpty(saveXml) Then
            Return saveXml
        End If

        Return GCModeller.FileSystem.RepositoryRoot & "/PfamFamily/PfamFamily.xml"
    End Function

    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    ''' <summary>
    ''' Protein Family Classification Annotation.
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="Threshold"></param>
    ''' <param name="MP"></param>
    ''' <param name="DbName"></param>
    ''' <returns></returns>
    <ExportAPI("Family.Align", Info:="Protein Family Classification Annotation.")>
    Public Function FamilyAlign(query As IEnumerable(Of Xfam.Pfam.PfamString.PfamString),
                                Optional Threshold As Double = 0.65,
                                Optional MP As Double = 0.65,
                                Optional accept As Integer = 10,
                                Optional DbName As String = "") As AnnotationOut()
        Dim FamilyDb As FamilyPfam = New Database().GetDatabase(DbName)
        Dim result As New List(Of AnnotationOut)

        Call MethodBase.GetCurrentMethod().GetFullName.__DEBUG_ECHO
        Call $"{NameOf(Threshold)} => {Threshold}".__DEBUG_ECHO
        Call $"{NameOf(MP)}        => {MP}".__DEBUG_ECHO
        Call $"{NameOf(accept)}    => {accept}".__DEBUG_ECHO
        Call $"{NameOf(DbName)}    => {DbName}".__DEBUG_ECHO

        For Each protein In query
            Dim classes As String() = FamilyDb.Classify(protein, Threshold, MP, accept, parallel:=True)
            Dim out As New AnnotationOut With {
                .Family = classes,
                .LocusId = protein.ProteinId,
                .PfamString = protein.PfamString
            }

            If Not classes.IsNullOrEmpty Then
                Call result.Add(out)
                Call Console.WriteLine()
                Call out.__DEBUG_ECHO
            Else
                Call Console.Write(".")
            End If
        Next

        Return result.ToArray
    End Function

    ''' <summary>
    ''' 家族注释的结果
    ''' </summary>
    Public Class AnnotationOut
        ''' <summary>
        ''' 基因号
        ''' </summary>
        ''' <returns></returns>
        Public Property LocusId As String
        ''' <summary>
        ''' 家族列表
        ''' </summary>
        ''' <returns></returns>
        Public Property Family As String()
        <CollectionAttribute("Pfam-String", "+")> Public Property PfamString As String()

        Public Overrides Function ToString() As String
            Return $"{LocusId}  => {Family.JoinBy(", ")}   //{PfamString.JoinBy("+")}"
        End Function
    End Class

    <ExportAPI("Family.Align", Info:="Protein Family Classification Annotation.")>
    Public Function FamilyAlign(query As Xfam.Pfam.PfamString.PfamString,
                                Optional Threshold As Double = 0.65,
                                Optional MP As Double = 0.65,
                                Optional accept As Integer = 10) As String()
        Return FamilyAlign({query}, Threshold, MP, accept)(Scan0).Family
    End Function

    <ExportAPI("Family.Stat")>
    Public Function FamilyStat(out As IEnumerable(Of AnnotationOut)) As File
        Dim protFamily = (From prot As AnnotationOut
                          In out
                          Select prot.LocusId, Family = __trim(prot.Family)).ToArray
        Dim LQuery = (From prot In protFamily
                      Select (From fm As String
                              In prot.Family
                              Select prot.LocusId, Family = fm).ToArray).ToArray.Unlist
        Dim Groups = (From x In LQuery Select x Order By x.Family Group x By x.Family Into Group).ToArray
        Dim Csv As New File

        Call Csv.Add("Family", "NumberOfProt", "LocusId")
        For Each FamilySet In Groups
            Call Csv.Add(FamilySet.Family, CStr(FamilySet.Group.Count), FamilySet.Group.Select(Function(x) x.LocusId).JoinBy("; "))
        Next
        Call Csv.AppendLine()
        Call Csv.AppendLine()
        Call Csv.Add("ID", "NumberOfFamily", "Family")
        For Each prot In protFamily
            Call Csv.Add(prot.LocusId, CStr(prot.Family.Length), prot.Family.JoinBy("; "))
        Next

        Return Csv
    End Function

    Private Function __trim(Family As String()) As String()
        Dim LQuery = (From s As String In Family
                      Let st As String = s.Replace("-like", "")
                      Select st.Split("/"c)).ToArray.Unlist
        Return LQuery.Distinct.ToArray
    End Function
End Module
