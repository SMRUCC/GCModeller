' ============================================================================
' VinaAtom.vb — MiniDock 对基础库原子模型的对接语义扩展
' ----------------------------------------------------------------------------
' 通用物理化学字段（坐标/元素/电荷/残基链信息）由基础库
' SMRUCC.genomics.Data.RCSB.PDB.Structures.Atom 提供，此处只承载 AutoDock Vina
' 打分特有的字段，避免在基础库里混入对接领域语义。
'
' VinaMolecule 是 Molecule(Of VinaAtom) 的具名子类：泛型容器 + 具名子类让
' MolBuilder / Charges / VinaScoring / MmGbsa 直接拿到强类型的 VinaAtom，
' 不需要在调用点写任何向下转型。
' ============================================================================

Imports SMRUCC.genomics.Data.RCSB.PDB.Structures

Namespace Core

    ''' <summary>Vina 打分所需的原子扩展字段（重原子模型）</summary>
    Public Class VinaAtom : Inherits Atom

        ''' <summary>Vina 原子类型编码（见 VinaAtomTypes）</summary>
        Public Property VinaType As Int32

        ''' <summary>Amber 简化 LJ ε（kcal/mol）</summary>
        Public Property LjEps As Double

        ''' <summary>Amber 简化 LJ 最小距离半径 R*（Å）</summary>
        Public Property LjRmin As Double

        ''' <summary>该原子是否来自受体侧</summary>
        Public Property FromReceptor As Boolean = True

    End Class

    ''' <summary>
    ''' MiniDock 使用的分子拓扑容器：等价于 Molecule(Of VinaAtom)，具名以便签名书写。
    ''' </summary>
    Public Class VinaMolecule : Inherits Molecule(Of VinaAtom)
    End Class

End Namespace
