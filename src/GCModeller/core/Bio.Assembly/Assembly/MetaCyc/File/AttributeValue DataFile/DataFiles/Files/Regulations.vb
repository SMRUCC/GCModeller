Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' This class describes most forms of protein, RNA or activity regulation.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Regulations : Inherits DataFile(Of Slots.Regulation)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "ACCESSORY-PROTEINS", "ANTI-ANTITERM-END-POS",
                    "ANTI-ANTITERM-START-POS", "ANTITERMINATOR-END-POS", "ANTITERMINATOR-START-POS",
                    "ASSOCIATED-BINDING-SITE", "ASSOCIATED-RNASE", "COMMENT", "COMMENT-INTERNAL",
                    "CREDITS", "DATA-SOURCE", "DOCUMENTATION", "DOWNSTREAM-GENES-ONLY?", "GROWTH-CONDITIONS",
                    "HIDE-SLOT?", "INSTANCE-NAME-TEMPLATE", "KI", "MECHANISM", "MEMBER-SORT-FN",
                    "MODE", "PAUSE-END-POS", "PAUSE-START-POS", "PHYSIOLOGICALLY-RELEVANT?",
                    "REGULATED-ENTITY", "REGULATOR", "SYNONYMS", "CITATIONS"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function

        ''' <summary>
        ''' 查找出某一个指定UniqueId编号值的Regulation集合
        ''' </summary>
        ''' <param name="Regulator">Regulator的UniqueID属性值</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetRegulationsByRegulator(Regulator As String) As Slots.Regulation()
            Dim LQuery = From Regulation In Me.AsParallel Where String.Equals(Regulation.Regulator, Regulator) Select Regulation '
            Return LQuery.ToArray
        End Function

        Public Function GetMechanism() As String()
            Dim TlQuery As String() = (From e As Slots.Regulation
                                       In Me.Values
                                       Let s As String = e.Mechanism
                                       Where Len(s) > 0
                                       Select s
                                       Distinct
                                       Order By s Ascending).ToArray
            Return TlQuery
        End Function

        Public Shared Shadows Narrowing Operator CType(e As Regulations) As Slots.Regulation()
            Return e.Values.ToArray
        End Operator
    End Class
End Namespace