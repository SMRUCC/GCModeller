#Region "Microsoft.VisualBasic::ae2b4b634f7bae8959134d4e11abf392, LocalBLAST\Web\AlignmentTable.vb"

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

    '     Class AlignmentTable
    ' 
    '         Properties: Database, FilePath, Hits, Program, Query
    '                     RID
    ' 
    '         Function: __substituted, __substituted2, DescriptionSubstituted, DescriptionSubstituted2, ExportOrderByGI
    '                   GetHitsEntryList, Save, ToString
    ' 
    '         Sub: TrimLength
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports

Namespace NCBIBlastResult

    Public Class AlignmentTable : Inherits ITextFile
        Implements ISaveHandle

        <XmlAttribute>
        Public Property Program As String
        Public Property Query As String
        <XmlAttribute>
        Public Property RID As String
        Public Property Database As String
        Public Property Hits As HitRecord()

        <XmlIgnore> <ScriptIgnore> <Ignored>
        Public Shadows Property FilePath As String
            Get
                Return MyBase.FilePath
            End Get
            Set(value As String)
                MyBase.FilePath = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return $"[{RID}]  {Program} -query {Query} -database {Database}  // {Hits.Length} hits found."
        End Function

        Const LOCUS_ID As String = "(emb|gb|dbj)\|[a-z]+\d+"

        Public Function GetHitsEntryList() As String()
            Dim LQuery As String() =
                LinqAPI.Exec(Of String) <= From hit As HitRecord
                                           In Me.Hits
                                           Let hitID As String =
                                               Regex.Match(hit.SubjectIDs, LOCUS_ID, RegexICSng).Value
                                           Where Not String.IsNullOrEmpty(hitID)
                                           Select hitID.Split(CChar("|")).Last
                                           Distinct
            Return LQuery
        End Function

        ''' <summary>
        ''' 按照GI编号进行替换
        ''' </summary>
        ''' <param name="Info"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DescriptionSubstituted(Info As gbEntryBrief()) As Integer
            Dim giTable = Info.ToDictionary(Function(item) item.GI)
            Dim LQuery = (From entry As HitRecord In Hits Select __substituted(entry, giTable)).ToArray
            Hits = LQuery
            Return Hits.Length
        End Function

        Private Shared Function __substituted(hitEntry As HitRecord, giTable As Dictionary(Of String, gbEntryBrief)) As HitRecord
            Dim entry = LinqAPI.DefaultFirst(Of gbEntryBrief) <=
 _
                From id As String
                In hitEntry.GI
                Where giTable.ContainsKey(id)
                Select giTable(id)

            If Not entry Is Nothing Then
                hitEntry.SubjectIDs = String.Format("gi|{0}|{1}", entry.GI, entry.Definition)
            End If

            Return hitEntry
        End Function

        Public Sub TrimLength(MaxLength As Integer)
            Dim avgLength As Integer = (From hit As HitRecord
                                        In Hits
                                        Select Len(hit.SubjectIDs)).Average * 1.3

            If avgLength > MaxLength AndAlso MaxLength > 0 Then
                avgLength = MaxLength
            End If

            For Each hit In Hits
                If Len(hit.SubjectIDs) > avgLength Then
                    hit.SubjectIDs = Mid(hit.SubjectIDs, 1, avgLength)
                End If
            Next
        End Sub

        ''' <summary>
        ''' 按照基因组编号进行替换
        ''' </summary>
        ''' <param name="Info"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DescriptionSubstituted2(Info As gbEntryBrief()) As Integer
            Dim GiDict As Dictionary(Of gbEntryBrief) = Info.ToDictionary
            Dim LQuery As HitRecord() = (From hitEntry As HitRecord In Hits Select __substituted2(hitEntry, GiDict)).ToArray
            Hits = LQuery
            Return Hits.Length
        End Function

        Private Shared Function __substituted2(hitEntry As HitRecord, GiDict As Dictionary(Of gbEntryBrief)) As HitRecord
            If GiDict.ContainsKey(hitEntry.SubjectIDs) Then
                Dim GetEntry As gbEntryBrief = GiDict(hitEntry.SubjectIDs)
                hitEntry.SubjectIDs = String.Format("gi|{0}|{1}", GetEntry.GI, GetEntry.Definition)
            End If
            Return hitEntry
        End Function

        ''' <summary>
        ''' Save as XML
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        Public Overrides Function Save(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean Implements ISaveHandle.Save
            Return Me.GetXml.SaveTo(Path, encoding)
        End Function

        ''' <summary>
        ''' 导出绘制的顺序
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>这里不能够使用并行拓展</remarks>
        Public Function ExportOrderByGI() As String()
            Dim LQuery As String() = (From hit As HitRecord
                                      In Hits
                                      Select hit.GI.FirstOrDefault
                                      Distinct).ToArray
            Return LQuery
        End Function
    End Class
End Namespace
