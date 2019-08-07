#Region "Microsoft.VisualBasic::a0eabe67b3eeebf2ab8b34996ac44bb9, localblast\LocalBLAST\Web\Alignment\AlignmentTable.vb"

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
    '         Properties: Database, Hits, Iteration, Program, Query
    '                     RID
    ' 
    '         Function: __substituted2, DescriptionSubstituted, DescriptionSubstituted2, GenericEnumerator, GetEnumerator
    '                   (+2 Overloads) Save, substituted, ToString
    ' 
    '         Sub: TrimLength
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports

Namespace NCBIBlastResult.WebBlast

    Public Class AlignmentTable : Implements Enumeration(Of HitRecord)

        Implements ISaveHandle

        <XmlAttribute>
        Public Property Program As String
        Public Property Iteration As Integer
        ''' <summary>
        ''' The query fasta file name or NCBI sequence accession id
        ''' </summary>
        ''' <returns></returns>
        Public Property Query As String
        ''' <summary>
        ''' NCBI blast task id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property RID As String
        ''' <summary>
        ''' Selected NCBI blast sequence database name
        ''' </summary>
        ''' <returns></returns>
        Public Property Database As String
        ''' <summary>
        ''' The alignment result
        ''' </summary>
        ''' <returns></returns>
        Public Property Hits As HitRecord()

        Public Overrides Function ToString() As String
            Return $"[{RID}]  {Program} -query {Query} -database {Database}  // {Hits.Length} hits found."
        End Function

        ''' <summary>
        ''' 按照GI编号进行替换
        ''' </summary>
        ''' <param name="Info"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DescriptionSubstituted(Info As gbEntryBrief()) As Integer
            Dim giTable = Info.ToDictionary(Function(item) item.GI)
            Dim LQuery = (From entry As HitRecord In Hits Select substituted(entry, giTable)).ToArray
            Hits = LQuery
            Return Hits.Length
        End Function

        Private Shared Function substituted(hitEntry As HitRecord, giTable As Dictionary(Of String, gbEntryBrief)) As HitRecord
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
        Public Function Save(Path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Return Me.GetXml.SaveTo(Path, encoding)
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of HitRecord) Implements Enumeration(Of HitRecord).GenericEnumerator
            For Each hit As HitRecord In Hits
                Yield hit
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of HitRecord).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class
End Namespace
