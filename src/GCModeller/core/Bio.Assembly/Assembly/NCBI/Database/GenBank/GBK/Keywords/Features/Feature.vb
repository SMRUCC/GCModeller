Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels

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
                    Return Me.gbRaw.Origin.GetFeatureSegment(Me)
                Else
                    Dim part1 As String = Me.gbRaw.Origin.GetFeatureSegment(Me)
                    Dim part2 As String = Me.gbRaw.Origin.SequenceParser.TryParse(jLoci.Left, jLoci.RegionLength)
                    If Me.Location.Complement Then
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
        Protected __innerList As New List(Of KeyValuePairObject(Of String, String))

        Public ReadOnly Property PairedValues As KeyValuePairObject(Of String, String)()
            Get
                Return __innerList.ToArray
            End Get
        End Property

        Public Function Query(Key As String) As String
            Dim LQuery = (From profileData As KeyValuePairObject(Of String, String)
                          In __innerList
                          Where String.Equals(profileData.Key, Key)
                          Select profileData.Value).FirstOrDefault
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

        Public Function ToGff() As TabularFormat.Feature
            Dim gff As New TabularFormat.Feature

            gff.Strand = Me.Location.ContiguousRegion.Strand
            gff.seqname = gbRaw.Accession.AccessionId
            gff.Right = Me.Location.Location.Right
            gff.Left = Me.Location.Location.Left
            gff.Feature = Me.KeyName
            gff.comments = Me.Query(FeatureQualifiers.note)
            gff.attributes = New Dictionary(Of String, String)
            gff.attributes.Add("gbkey", KeyName)
            gff.attributes.Add("Name", Query(FeatureQualifiers.locus_tag))

            Return gff
        End Function

        ''' <summary>
        ''' Some key would be duplicated 
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function QueryDuplicated(key As String) As String()
            Dim LQuery As String() = LinqAPI.Exec(Of String) <=
                From pairedObj As KeyValuePairObject(Of String, String)
                In __innerList
                Where String.Equals(pairedObj.Key, key)
                Select pairedObj.Value '
            Return LQuery
        End Function

        Public Overrides Function ToString() As String
            Dim sb As StringBuilder = New StringBuilder(1024)

            Call sb.AppendFormat("KeyName:='{0}'{1}", KeyName, vbCrLf)
            Call sb.AppendFormat("Located:='{0}'{1}", Location.ToString, vbCrLf)
            Call sb.AppendLine("{")
            For Each Line As KeyValuePairObject(Of String, String) In __innerList
                Call sb.AppendFormat("  {0}{1}", Line.ToString, vbCrLf)
            Next
            Call sb.AppendLine("}")

            Return sb.ToString
        End Function

        Public Shared Widening Operator CType(strData As String()) As Feature
            Dim Feature As Feature = New Feature
            Dim s As String = strData.First

            Feature.KeyName = s.Split.First
            Feature.Location = Mid(s, Len(Feature.KeyName) + 1).Trim

            For Each e As KeyValuePairObject(Of String, String) In ReadingQualifiers(strData.Skip(1).ToArray)
                Call Feature.__innerList.Add(e)
            Next

            Return Feature
        End Operator

        Protected Shared Iterator Function ReadingQualifiers(Data As String()) As IEnumerable(Of KeyValuePairObject(Of String, String))
            For Each str As String In Data
                Yield __parser(Mid(str, 2))
            Next
        End Function

        Private Shared Function __parser(s As String) As KeyValuePairObject(Of String, String)
            Dim Name As String = s.Split(CChar("=")).First
            Dim Value As String = Mid(s, Len(Name) + 2)

            If (Not String.IsNullOrEmpty(Value)) AndAlso
                (Value.First = QUOT_CHAR AndAlso Value.Last = QUOT_CHAR) Then
                Value = Mid(Value, 2, Len(Value) - 2)
            End If

            If String.Equals(Name, "translation") Then
                Value = Value.Replace(" ", String.Empty)
            End If

            Return New KeyValuePairObject(Of String, String)(Name, Value)
        End Function

        Protected Friend Sub CopyTo(ByRef InternalList As List(Of KeyValuePairObject(Of String, String)))
            InternalList = Me.PairedValues.ToList
        End Sub

        Public Function SetValue(key As FeatureQualifiers, value As String) As Feature
            Dim keyName As String = key.ToString
            Dim LQuery = (From x As KeyValuePairObject(Of String, String)
                          In Me.__innerList
                          Where String.Equals(keyName, x.Key)
                          Select x).FirstOrDefault

            If LQuery Is Nothing Then
                Call Add(keyName, value)
            Else
                LQuery.Value = value
            End If

            Return Me
        End Function

#Region "Implements IDictionary(Of String, String)"

        Public Sub Add(item As KeyValuePair(Of String, String)) Implements ICollection(Of KeyValuePair(Of String, String)).Add
            Call __innerList.Add(New KeyValuePairObject(Of String, String)(item))
        End Sub

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of String, String)).Clear
            Call __innerList.Clear()
        End Sub

        Public Function Contains(item As KeyValuePair(Of String, String)) As Boolean Implements ICollection(Of KeyValuePair(Of String, String)).Contains
            Dim LQuery = (From n In __innerList Where String.Equals(item.Key, n.Key) AndAlso String.Equals(item.Value, n.Value) Select n.Key).FirstOrDefault
            Return Not LQuery Is Nothing
        End Function

        Public Sub CopyTo(array() As KeyValuePair(Of String, String), arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of String, String)).CopyTo
            Throw New NotImplementedException
        End Sub

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of String, String)).Count
            Get
                If __innerList.IsNullOrEmpty Then
                    Return 0
                Else
                    Return __innerList.Count
                End If
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of String, String)).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function Remove(item As KeyValuePair(Of String, String)) As Boolean Implements ICollection(Of KeyValuePair(Of String, String)).Remove
            Dim LQuery = (From n In __innerList Where String.Equals(item.Key, n.Key) AndAlso String.Equals(item.Value, n.Value) Select n).ToArray
            For Each n In LQuery
                Call __innerList.Remove(n)
            Next

            Return Not LQuery.IsNullOrEmpty
        End Function

        ''' <summary>
        ''' 添加一个注释信息
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="value"></param>
        Public Sub Add(key As String, value As String) Implements IDictionary(Of String, String).Add
            Call __innerList.Add(New KeyValuePairObject(Of String, String)(key, value))
        End Sub

        Public Function ContainsKey(skey As String) As Boolean Implements IDictionary(Of String, String).ContainsKey
            Dim LQuery = (From stored In __innerList Where String.Equals(skey, stored.Key, StringComparison.OrdinalIgnoreCase) Select stored.Key).FirstOrDefault
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
                Return (From item In __innerList Select item.Key Distinct).ToArray
            End Get
        End Property

        Public Function Remove(key As String) As Boolean Implements IDictionary(Of String, String).Remove
            Dim LQuery = (From item In __innerList Where String.Equals(key, item.Key) Select item).ToArray
            For Each Item As KeyValuePairObject(Of String, String) In __innerList
                Call __innerList.Remove(Item)
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
                Return (From item In __innerList Select item.Value).ToArray
            End Get
        End Property

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, String)) Implements IEnumerable(Of KeyValuePair(Of String, String)).GetEnumerator
            For Each Item As KeyValuePairObject(Of String, String) In __innerList
                Yield New KeyValuePair(Of String, String)(Item.Key, Item.Value)
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region
    End Class
End Namespace