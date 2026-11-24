Namespace PDBExport

    ''' <summary>
    ''' SMILES/InChI 分子图导出为 PDB 结构时的选项。
    ''' </summary>
    Public Class PdbExportOptions

        ''' <summary>
        ''' 残基名（PDB 列 18-20，最多 3 个字符）。配体通常使用 <c>UNL</c>/<c>LIG</c>/<c>MOL</c>。
        ''' </summary>
        Public Property ResName As String = "UNL"

        ''' <summary>
        ''' 链标识符（PDB 列 22）。
        ''' </summary>
        Public Property ChainID As String = "A"

        ''' <summary>
        ''' 残基序号（PDB 列 23-26）。
        ''' </summary>
        Public Property ResSeq As Integer = 1

        ''' <summary>
        ''' 是否以 <c>HETATM</c> 记录输出；<c>False</c> 时使用 <c>ATOM</c> 记录。
        ''' </summary>
        Public Property UseHetatm As Boolean = True

        ''' <summary>
        ''' 是否保留氢原子。为 <c>False</c> 时会先移除氢原子再导出。
        ''' </summary>
        Public Property IncludeHydrogens As Boolean = True

        ''' <summary>
        ''' 分子图缺少 3D 坐标时，是否自动在副本上执行 <see cref="Graph.MoleculeGraph.Embed3D"/>。
        ''' </summary>
        Public Property Ensure3D As Boolean = True

        ''' <summary>
        ''' 是否输出 <c>CONECT</c> 连接记录。
        ''' </summary>
        Public Property WriteConect As Boolean = True

        ''' <summary>
        ''' 是否输出 <c>REMARK</c> 注释（结构式原文与键级表）。
        ''' </summary>
        Public Property WriteRemarks As Boolean = True

        ''' <summary>
        ''' 结构标题；为空时使用分子式。
        ''' </summary>
        Public Property Title As String

        ''' <summary>
        ''' 是否把坐标平移使几何中心位于原点（3D 嵌入本身已经居中，通常无需再动）。
        ''' </summary>
        Public Property CenterAtOrigin As Boolean = False

        ''' <summary>
        ''' 创建一个使用默认值的选项对象。
        ''' </summary>
        Public Sub New()
        End Sub

    End Class

End Namespace
