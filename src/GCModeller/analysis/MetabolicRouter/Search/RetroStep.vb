Imports SMRUCC.genomics.Analysis.RetroPath.Chem

Namespace Search

    Public Class RetroStep

        Public RuleId As String
        Public RuleName As String
        ''' <summary>
        ''' forward / reverse（规则定义方向）
        ''' </summary>
        Public Orientation As String
        ''' <summary>
        ''' 被分解化合物
        ''' </summary>
        Public SubstrateKey As String
        Public SubstrateMol As Molecule
        ''' <summary>
        ''' (key, mol)
        ''' </summary>
        Public Precursors As New List(Of (key As String, mol As Molecule))()
        Public CoproductCount As Int32
        Public DeltaG As Double
        Public EnzymeTier As Int32
        Public AtomMap As New List(Of Tuple(Of Int32, Int32))()

    End Class
End Namespace