#Region "Microsoft.VisualBasic::22463d90ad37262a84f4cb15213cbf39, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\Features\Feature.vb"

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


    ' Code Statistics:

    '   Total Lines: 337
    '    Code Lines: 237
    ' Comment Lines: 46
    '   Blank Lines: 54
    '     File Size: 13.86 KB


    '     Class Feature
    ' 
    '         Properties: Count, IsReadOnly, KeyName, Keys, Location
    '                     PairedValues, SequenceData, Values
    ' 
    '         Function: attributes, Contains, ContainsKey, doParser, GetEnumerator
    '                   GetEnumerator1, (+2 Overloads) Query, QueryDuplicated, ReadingQualifiers, (+2 Overloads) Remove
    '                   SetValue, ToString, TryGetValue
    ' 
    '         Sub: (+2 Overloads) Add, Clear, (+2 Overloads) CopyTo
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    ''' <summary>
    ''' A sequence feature site on the genome DNA sequence.(基因组序列上面的特性区域片段)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Feature : Inherits IgbComponent
        Implements IDictionary(Of String, String)

        ''' <summary>
        ''' 第6至第20列的小字段的字段名
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property KeyName As String
        ''' <summary>
        ''' The location of this feature site on the DNA chain.(本特性位点在DNA链上面的位置)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlElement> Public Property Location As Location

        ''' <summary>
        ''' nt sequence of this feature site.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SequenceData As String
            Get
                Dim jLoci = Location.JoinLocation

                If jLoci Is Nothing Then
                    Return Me.gb.Origin.GetFeatureSegment(Me)
                Else
                    Dim part1 As String = gb.Origin.GetFeatureSegment(Me)
                    Dim part2 As String = gb.Origin.CutSequenceBylength(
                        jLoci.Left,
                        jLoci.RegionLength).SequenceData

                    If Location.Complement Then
                        part2 = NucleicAcid.Complement(part2)
                    End If

                    Return part1 & part2
                End If
            End Get
        End Property

        ''' <summary>
        ''' 请注意在值中有拼接断行所产生的空格，在导出CDS序列的时候，请注意消除该空格
        ''' </summary>
        ''' <remarks>这里不能够使用字典来进行储存数据，因为字典对大小写敏感并且这里面会有重复的Key出现</remarks>
        Protected innerList As New List(Of NamedValue(Of String))

        Public ReadOnly Property PairedValues As NamedValue(Of String)()
            Get
                Return innerList.ToArray
            End Get
        End Property

        Public Function Query(key$) As String
            Dim LQuery = LinqAPI.DefaultFirst(Of String) <=
 _
                From profileData As NamedValue(Of String)
                In innerList
                Where String.Equals(profileData.Name, key)
                Select profileData.Value

            Return LQuery
        End Function

        ''' <summary>
        ''' Get feature describ value string by key
        ''' </summary>
        ''' <param name="Key"></param>
        ''' <returns></returns>
        Public Function Query(Key As FeatureQualifiers) As String
            Dim sKey As String = Key.ToString
            Return Query(sKey)
        End Function

        Public Function attributes() As Dictionary(Of String, String)
            Dim attrs As New List(Of (key$, val$))

            attrs += ("gbKey", KeyName)
            attrs += ("name", Query(FeatureQualifiers.locus_tag))
            attrs += ("gene", Query(FeatureQualifiers.gene))
            attrs += ("protein_id", Query(FeatureQualifiers.protein_id))
            attrs += ("product", Query(FeatureQualifiers.product))
            attrs += ("phenotype", Query(FeatureQualifiers.phenotype))
            attrs += ("pseudo", Query(FeatureQualifiers.pseudo))
            attrs += ("locus_tag", Query(FeatureQualifiers.locus_tag))
            attrs += ("transl_table", Query(FeatureQualifiers.transl_table))
            attrs += ("type", Query(FeatureQualifiers.type))
            attrs += ("Dbxref", Query(FeatureQualifiers.db_xref))
            attrs += ("EC_number", Query(FeatureQualifiers.EC_number))
            attrs += ("function", Query(FeatureQualifiers.function))
            attrs += ("Note", Query(FeatureQualifiers.note))

            Return attrs _
                .Where(Function(a)
                           Return Not a.val.StringEmpty
                       End Function) _
                .ToDictionary(Function(t) t.key,
                              Function(t)
                                  Return t.val
                              End Function)
        End Function

        ''' <summary>
        ''' Some key would be duplicated 
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function QueryDuplicated(key As String) As String()
            Dim LQuery As String() = LinqAPI.Exec(Of String) <=
                From pairedObj As NamedValue(Of String)
                In innerList
                Where String.Equals(pairedObj.Name, key)
                Select pairedObj.Value '

            Return LQuery
        End Function

        Public Overrides Function ToString() As String
            Dim sb As StringBuilder = New StringBuilder(1024)

            Call sb.AppendFormat("KeyName:='{0}'{1}", KeyName, vbCrLf)
            Call sb.AppendFormat("Located:='{0}'{1}", Location.ToString, vbCrLf)
            Call sb.AppendLine("{")
            For Each Line As NamedValue(Of String) In innerList
                Call sb.AppendFormat("  {0}{1}", Line.ToString, vbCrLf)
            Next
            Call sb.AppendLine("}")

            Return sb.ToString
        End Function

        Public Shared Widening Operator CType(strData As String()) As Feature
            Dim s As String = strData.First
            Dim Feature As New Feature With {
                .KeyName = s.Split.First,
                .Location = Mid(s, Len(.KeyName) + 1).Trim
            }

            For Each e As NamedValue(Of String) In ReadingQualifiers(strData.Skip(1).ToArray)
                Call Feature.innerList.Add(e)
            Next

            Return Feature
        End Operator

        Protected Shared Iterator Function ReadingQualifiers(data As String()) As IEnumerable(Of NamedValue(Of String))
            For Each str As String In data
                Yield doParser(Mid(str, 2))
            Next
        End Function

        Private Shared Function doParser(s As String) As NamedValue(Of String)
            Dim value = s.GetTagValue("=")
            Dim str$ = value.Value

            If (Not String.IsNullOrEmpty(str)) AndAlso
                str.First = ASCII.Quot AndAlso
                str.Last = ASCII.Quot Then
                str = Mid(str, 2, Len(str) - 2)
            End If

            If String.Equals(value.Name, "translation") Then
                str = str.Replace(" ", String.Empty)
            End If

            Return New NamedValue(Of String) With {
                .Name = value.Name,
                .Value = str
            }
        End Function

        Protected Friend Sub CopyTo(ByRef InternalList As List(Of NamedValue(Of String)))
            InternalList = New List(Of NamedValue(Of String))(PairedValues)
        End Sub

        Public Function SetValue(key As FeatureQualifiers, value As String) As Feature
            Dim keyName As String = key.ToString

            For i As Integer = 0 To innerList.Count - 1
                If keyName = innerList(i).Name Then
                    innerList(i) = New NamedValue(Of String) With {
                        .Name = keyName,
                        .Value = value
                    }

                    Return Me
                End If
            Next

            ' 没有查找到，则添加新纪录
            Call Add(keyName, value)

            Return Me
        End Function

#Region "Implements IDictionary(Of String, String)"

        Public Sub Add(item As KeyValuePair(Of String, String)) Implements ICollection(Of KeyValuePair(Of String, String)).Add
            Call innerList.Add(New NamedValue(Of String)(item.Key, item.Value))
        End Sub

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of String, String)).Clear
            Call innerList.Clear()
        End Sub

        Public Function Contains(item As KeyValuePair(Of String, String)) As Boolean Implements ICollection(Of KeyValuePair(Of String, String)).Contains
            Dim LQuery = (From n In innerList Where String.Equals(item.Key, n.Name) AndAlso String.Equals(item.Value, n.Value) Select n.Name).FirstOrDefault
            Return Not LQuery Is Nothing
        End Function

        Public Sub CopyTo(array() As KeyValuePair(Of String, String), arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of String, String)).CopyTo
            Throw New NotImplementedException
        End Sub

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of String, String)).Count
            Get
                If innerList.IsNullOrEmpty Then
                    Return 0
                Else
                    Return innerList.Count
                End If
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of String, String)).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function Remove(item As KeyValuePair(Of String, String)) As Boolean Implements ICollection(Of KeyValuePair(Of String, String)).Remove
            Dim LQuery = (From n In innerList Where String.Equals(item.Key, n.Name) AndAlso String.Equals(item.Value, n.Value) Select n).ToArray
            For Each n In LQuery
                Call innerList.Remove(n)
            Next

            Return Not LQuery.IsNullOrEmpty
        End Function

        ''' <summary>
        ''' 添加一个注释信息
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="value"></param>
        Public Sub Add(key As String, value As String) Implements IDictionary(Of String, String).Add
            Call innerList.Add(New NamedValue(Of String)(key, value))
        End Sub

        Public Function ContainsKey(skey As String) As Boolean Implements IDictionary(Of String, String).ContainsKey
            Dim LQuery = (From stored In innerList Where String.Equals(skey, stored.Name, StringComparison.OrdinalIgnoreCase) Select stored.Name).FirstOrDefault
            Return Not LQuery Is Nothing
        End Function

        ''' <summary>
        ''' 对于已经存在的数据，本方法会覆盖原有的数据，假若不存在，或者目标对象有多个值，则会进行添加
        ''' </summary>
        ''' <param name="key"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public Property Item(key As String) As String Implements IDictionary(Of String, String).Item
            Get
                Return Query(key)
            End Get
            Set(value As String)
                If ContainsKey(key) Then
                    Dim data = Me.QueryDuplicated(key)
                    If data.Length > 1 Then
                        Call Me.Add(New KeyValuePair(Of String, String)(key, value))
                    Else '只有一个值，则进行替换
                        Call Me.Remove(New KeyValuePair(Of String, String)(key, value:=data.First))
                    End If
                Else
                    Call Me.Add(New KeyValuePair(Of String, String)(key, value))
                End If
            End Set
        End Property

        Public ReadOnly Property Keys As ICollection(Of String) Implements IDictionary(Of String, String).Keys
            Get
                Return (From item In innerList Select item.Name Distinct).ToArray
            End Get
        End Property

        Public Function Remove(key As String) As Boolean Implements IDictionary(Of String, String).Remove
            Dim LQuery = (From item In innerList Where String.Equals(key, item.Name) Select item).ToArray
            For Each Item As NamedValue(Of String) In innerList
                Call innerList.Remove(Item)
            Next

            Return Not LQuery.IsNullOrEmpty
        End Function

        Public Function TryGetValue(key As String, ByRef value As String) As Boolean Implements IDictionary(Of String, String).TryGetValue
            Dim s As String = Query(key)
            value = s

            If String.IsNullOrEmpty(s) Then
                Return False
            Else
                Return True
            End If
        End Function

        Public ReadOnly Property Values As ICollection(Of String) Implements IDictionary(Of String, String).Values
            Get
                Return (From item In innerList Select item.Value).ToArray
            End Get
        End Property

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, String)) Implements IEnumerable(Of KeyValuePair(Of String, String)).GetEnumerator
            For Each Item As NamedValue(Of String) In innerList
                Yield New KeyValuePair(Of String, String)(Item.Name, Item.Value)
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region
    End Class
End Namespace
