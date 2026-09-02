' ============================================================================
' DockResult.vb — 结构化对接结果对象（JSON DTO，System.Text.Json 序列化）
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization

Namespace MiniDock.Model

    Public Class DockReport

        <JsonPropertyName("program")>
        Public Property Program As String

        <JsonPropertyName("version")>
        Public Property Version As String

        <JsonPropertyName("mode")>
        Public Property Mode As String          ' ligand | protein-protein

        <JsonPropertyName("parameters")>
        Public Property Parameters As DockParameters

        <JsonPropertyName("results")>
        Public Property Results As List(Of LigandResult)

    End Class

    Public Class DockParameters

        <JsonPropertyName("exhaustiveness")>
        Public Property Exhaustiveness As Integer

        <JsonPropertyName("steps_per_run")>
        Public Property StepsPerRun As Integer

        <JsonPropertyName("num_modes")>
        Public Property NumModes As Integer

        <JsonPropertyName("min_rmsd")>
        Public Property MinRmsd As Double

        <JsonPropertyName("box_center")>
        Public Property BoxCenter As Double()

        <JsonPropertyName("box_half_size")>
        Public Property BoxHalfSize As Double

        <JsonPropertyName("temperature_metropolis")>
        Public Property TemperatureMetropolis As Double

        <JsonPropertyName("weights")>
        Public Property Weights As Double()

        <JsonPropertyName("mmgbsa")>
        Public Property Mmgbsa As Boolean

        <JsonPropertyName("nwat")>
        Public Property Nwat As Integer

        <JsonPropertyName("receptor_atoms")>
        Public Property ReceptorAtoms As Integer

        <JsonPropertyName("seed")>
        Public Property Seed As Integer

    End Class

    Public Class LigandResult

        <JsonPropertyName("id")>
        Public Property Id As String

        <JsonPropertyName("num_atoms")>
        Public Property NumAtoms As Integer

        <JsonPropertyName("num_rotatable_bonds")>
        Public Property NumRotatableBonds As Integer

        <JsonPropertyName("poses")>
        Public Property Poses As List(Of Pose)

    End Class

    Public Class Pose

        <JsonPropertyName("rank")>
        Public Property Rank As Integer

        <JsonPropertyName("vina_score")>
        Public Property VinaScore As Double          ' ΔG = 0.0585·N_rot + c_inter

        <JsonPropertyName("intermolecular")>
        Public Property Intermolecular As Double

        <JsonPropertyName("intramolecular")>
        Public Property Intramolecular As Double

        <JsonPropertyName("num_torsions")>
        Public Property NumTorsions As Integer

        <JsonPropertyName("mmgbsa")>
        Public Property Mmgbsa As MmGbsaResultDto

        <JsonPropertyName("atoms")>
        Public Property Atoms As List(Of PoseAtom)

    End Class

    Public Class PoseAtom

        <JsonPropertyName("element")>
        Public Property Element As String

        <JsonPropertyName("x")>
        Public Property X As Double

        <JsonPropertyName("y")>
        Public Property Y As Double

        <JsonPropertyName("z")>
        Public Property Z As Double

        <JsonPropertyName("chain")>
        Public Property Chain As String

        <JsonPropertyName("res_name")>
        Public Property ResName As String

        <JsonPropertyName("res_seq")>
        Public Property ResSeq As Integer

        <JsonPropertyName("atom_name")>
        Public Property AtomName As String

    End Class

    Public Class MmGbsaResultDto

        <JsonPropertyName("delta_g")>
        Public Property DeltaG As Double

        <JsonPropertyName("vdw")>
        Public Property Vdw As Double

        <JsonPropertyName("elec")>
        Public Property Elec As Double

        <JsonPropertyName("gb_polar")>
        Public Property GbPolar As Double

        <JsonPropertyName("sasa_nonpolar")>
        Public Property SasNonpolar As Double

        <JsonPropertyName("nwat")>
        Public Property Nwat As Integer

    End Class

    ''' <summary>mmgbsa 子命令报告</summary>
    Public Class MmGbsaReport

        <JsonPropertyName("program")>
        Public Property Program As String

        <JsonPropertyName("version")>
        Public Property Version As String

        <JsonPropertyName("mode")>
        Public Property Mode As String

        <JsonPropertyName("nwat")>
        Public Property Nwat As Integer

        <JsonPropertyName("frames")>
        Public Property Frames As List(Of MmGbsaFrame)

    End Class

    Public Class MmGbsaFrame

        <JsonPropertyName("model")>
        Public Property Model As Integer

        <JsonPropertyName("delta_g")>
        Public Property DeltaG As Double

        <JsonPropertyName("vdw")>
        Public Property Vdw As Double

        <JsonPropertyName("elec")>
        Public Property Elec As Double

        <JsonPropertyName("gb_polar")>
        Public Property GbPolar As Double

        <JsonPropertyName("sasa_nonpolar")>
        Public Property SasNonpolar As Double

        <JsonPropertyName("nwat_selected")>
        Public Property NwatSelected As Integer

        <JsonPropertyName("receptor_atoms")>
        Public Property ReceptorAtoms As Integer

        <JsonPropertyName("ligand_atoms")>
        Public Property LigandAtoms As Integer

    End Class

End Namespace
