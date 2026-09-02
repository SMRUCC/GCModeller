Imports SMRUCC.genomics.Data.RCSB.PDB.Structures

Namespace Core


    ''' <summary>IPoseObjective 适配器：绑定配体/受体/扭转树</summary>
    Public Class DockObjective
        Implements IPoseObjective

        Private ReadOnly _baseCoords(,) As Double        ' 配体初始坐标 (n,3)
        Private ReadOnly _ligAtoms As List(Of VinaAtom)      ' 配体原子（类型/半径在 atom 上）
        Private ReadOnly _scorer As VinaScorer
        Private ReadOnly _axes() As Int32
        Private ReadOnly _branches As List(Of List(Of Int32))
        Private ReadOnly _intraI() As Int32
        Private ReadOnly _intraJ() As Int32
        Private ReadOnly _workPos() As Double
        Private ReadOnly _rigidCenter(2) As Double

        Public Sub New(baseCoords(,) As Double, ligAtoms As List(Of VinaAtom), scorer As VinaScorer,
                       axes() As Int32, branches As List(Of List(Of Int32)),
                       intraI() As Int32, intraJ() As Int32)
            _baseCoords = baseCoords
            _ligAtoms = ligAtoms
            _scorer = scorer
            _axes = axes
            _branches = branches
            _intraI = intraI
            _intraJ = intraJ
            _workPos = New Double(3 * ligAtoms.Count - 1) {}
        End Sub

        Public ReadOnly Property NumTorsions As Int32 Implements IPoseObjective.NumTorsions
            Get
                Return _branches.Count
            End Get
        End Property

        Public Function Evaluate(trans() As Double, rotvec() As Double, torsions() As Double,
                                 grads() As Double, rigidCenter() As Double) As Double Implements IPoseObjective.Evaluate
            PoseOps.ApplyPose(_baseCoords, trans, rotvec, _axes, _branches, torsions,
                              _workPos, rigidCenter(0), rigidCenter(1), rigidCenter(2))
            For i = 0 To _ligAtoms.Count - 1
                _ligAtoms(i).X = _workPos(3 * i)
                _ligAtoms(i).Y = _workPos(3 * i + 1)
                _ligAtoms(i).Z = _workPos(3 * i + 2)
            Next
            Return _scorer.Evaluate(_ligAtoms, rigidCenter, _intraI, _intraJ, _axes, _branches, grads)
        End Function

        ''' <summary>物化当前姿态坐标（x,y,z 平铺）</summary>
        Public Function MaterializeCoords(trans() As Double, rotvec() As Double, torsions() As Double) As Double()
            Dim rc(2) As Double
            PoseOps.ApplyPose(_baseCoords, trans, rotvec, _axes, _branches, torsions,
                              _workPos, rc(0), rc(1), rc(2))
            Return CType(_workPos.Clone(), Double())
        End Function

    End Class
End Namespace