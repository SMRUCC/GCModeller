Imports System.Text

Namespace SequenceModel.Polypeptides.SecondaryStructure

    Public Class AminoAcid

        Public ReadOnly Property AminoAcid As Polypeptides.AminoAcid

        Protected Friend _MaskAlphaHelix As Boolean
        Protected Friend _MaskBetaSheet_ As Boolean
        Protected Friend _MastBetaTurn__ As Boolean

        ''' <summary>
        ''' 当前的这个氨基酸分子所处的二级结构特征
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property [StructureType] As ChouFasman.SecondaryStructures

        ''' <summary>
        ''' 使用一个字符用来表示的二级结构特征
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property StructureChar As Char
            Get
                Return StructureTypesToChar(Me.StructureType).First
            End Get
        End Property

        Sub New(Type As Polypeptides.AminoAcid)
            _AminoAcid = Type
        End Sub

        Protected Friend ReadOnly Property HelixSheetOverlap As Boolean
            Get
                Return _MaskAlphaHelix AndAlso _MaskBetaSheet_
            End Get
        End Property

        Protected Friend ReadOnly Property HelixTurnOverlap As Boolean
            Get
                Return _MaskAlphaHelix AndAlso _MastBetaTurn__
            End Get
        End Property

        Protected Friend ReadOnly Property SheetTurnOverlap As Boolean
            Get
                Return _MaskBetaSheet_ AndAlso _MastBetaTurn__
            End Get
        End Property

        Protected Friend ReadOnly Property Coil As Boolean
            Get
                Return (_MaskAlphaHelix AndAlso _MaskBetaSheet_ AndAlso _MastBetaTurn__) OrElse
                    (Not _MaskAlphaHelix AndAlso Not _MaskBetaSheet_ AndAlso Not _MastBetaTurn__)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0};  [alpha-helix:={1},  beta_sheet:={2},  beta_turn:={3}]", _AminoAcid.ToString, _MaskAlphaHelix, _MaskBetaSheet_, _MastBetaTurn__)
        End Function
    End Class
End Namespace