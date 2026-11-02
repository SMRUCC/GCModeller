Imports Chem = SMRUCC.genomics.ComponentModel.Chemical

Namespace Graph

    ''' <summary>
    ''' 单个元素的物理化学常量集合。
    ''' </summary>
    Public Class ElementInfo

        ''' <summary>元素符号（标准大小写）。</summary>
        Public Property Symbol As String
        ''' <summary>共价半径，单位埃（Å）。</summary>
        Public Property CovalentRadius As Double = 0.77
        ''' <summary>范德华半径，单位埃（Å）。</summary>
        Public Property VdwRadius As Double = 1.7
        ''' <summary>CPK 配色（<c>RRGGBB</c> 十六进制，不带前缀 <c>#</c>）。</summary>
        Public Property CpkColor As String = "909090"
        ''' <summary>常见价态列表（升序），用于隐式氢补全与键级还原。</summary>
        Public Property Valences As Integer() = {4}
        ''' <summary>相对原子质量。</summary>
        Public Property AtomicMass As Double

        ''' <summary>
        ''' 该元素在给定形式电荷下的目标价态：从 <see cref="Valences"/> 中选出使
        ''' <c>价态 − 形式电荷</c> 最接近电荷中性需求的值。
        ''' </summary>
        ''' <param name="charge">形式电荷。</param>
        ''' <returns>该电荷状态下的目标价态。</returns>
        Public Function ValenceOf(charge As Integer) As Integer
            If Valences Is Nothing OrElse Valences.Length = 0 Then Return 0

            Dim target As Integer = Valences(0) + charge

            For i = 0 To Valences.Length - 1
                If Valences(i) >= target Then Return Valences(i)
            Next

            Return Valences(Valences.Length - 1)
        End Function

        ''' <summary>
        ''' 与另一个元素成键时的默认键长（两个共价半径之和）。
        ''' </summary>
        Public Function BondLengthTo(other As ElementInfo) As Double
            Return If(other Is Nothing, CovalentRadius * 2, CovalentRadius + other.CovalentRadius)
        End Function
    End Class

    ''' <summary>
    ''' 元素周期表：共价半径、范德华半径、CPK 配色、常见价态与原子量。
    ''' </summary>
    ''' <remarks>
    ''' 本表只收录到 Bi 为止的常见元素，其余元素回退到碳的默认值。
    ''' 原子量优先复用 <c>Bio.Assembly</c> 的 <c>PeriodicTable</c> 字典，取不到时使用内置值。
    ''' </remarks>
    Public Module ElementTable

        Const DEFAULT_RADIUS As Double = 0.77
        Const DEFAULT_VDW As Double = 1.7
        Const DEFAULT_COLOR As String = "909090"

        ''' <summary>
        ''' 元素符号 → 元素常量。查不到时返回碳的常量。
        ''' </summary>
        ''' <param name="symbol">元素符号，大小写不敏感。</param>
        Public Function GetElement(symbol As String) As ElementInfo
            If String.IsNullOrEmpty(symbol) Then Return GetElement("C")

            Dim key As String = If(symbol = "*", "C", symbol)
            Dim info As ElementInfo = Nothing

            If table.TryGetValue(key, info) Then Return info

            key = key.Substring(0, 1).ToUpper & key.Substring(1).ToLower

            If table.TryGetValue(key, info) Then Return info

            Return New ElementInfo With {
                .Symbol = key,
                .CovalentRadius = DEFAULT_RADIUS,
                .VdwRadius = DEFAULT_VDW,
                .CpkColor = DEFAULT_COLOR,
                .Valences = New Integer() {4},
                .AtomicMass = 0
            }
        End Function

        ''' <summary>
        ''' 共价半径（Å）。
        ''' </summary>
        ''' <param name="symbol">元素符号。</param>
        Public Function CovalentRadius(symbol As String) As Double
            Return GetElement(symbol).CovalentRadius
        End Function

        ''' <summary>
        ''' 范德华半径（Å）。
        ''' </summary>
        ''' <param name="symbol">元素符号。</param>
        Public Function VdwRadius(symbol As String) As Double
            Return GetElement(symbol).VdwRadius
        End Function

        ''' <summary>
        ''' CPK 配色的十六进制字符串（<c>RRGGBB</c>）。
        ''' </summary>
        ''' <param name="symbol">元素符号。</param>
        Public Function CpkColor(symbol As String) As String
            Return GetElement(symbol).CpkColor
        End Function

        ''' <summary>
        ''' 相对原子质量；未知元素返回 <c>0</c>。
        ''' </summary>
        ''' <param name="symbol">元素符号。</param>
        Public Function AtomicMass(symbol As String) As Double
            Dim m As Double = 0

            Try
                If Chem.PeriodicTable.PeriodicTable.TryGetValue(symbol, m) Then Return m
            Catch ex As Exception
                Call App.LogException(ex)
            End Try

            m = GetElement(symbol).AtomicMass

            If m > 0 Then Return m
            If symbol = "D" Then Return 2.014
            If symbol = "T" Then Return 3.016

            Return 0
        End Function

        ''' <summary>
        ''' 两个成键原子之间的默认键长（Å），取共价半径之和。
        ''' </summary>
        ''' <param name="a">第一个元素符号。</param>
        ''' <param name="b">第二个元素符号。</param>
        Public Function DefaultBondLength(a As String, b As String) As Double
            Return GetElement(a).BondLengthTo(GetElement(b))
        End Function

        ''' <summary>
        ''' 该元素是否在有机子集内（SMILES 中可以不带方括号书写的元素）。
        ''' </summary>
        ''' <param name="symbol">元素符号。</param>
        Public Function IsOrganicSubset(symbol As String) As Boolean
            Select Case symbol
                Case "B", "C", "N", "O", "P", "S", "F", "Cl", "Br", "I"
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private ReadOnly table As Dictionary(Of String, ElementInfo) = buildTable()

        Private Function buildTable() As Dictionary(Of String, ElementInfo)
            Dim data As New Dictionary(Of String, ElementInfo)(StringComparer.OrdinalIgnoreCase)

            Call add(data, "H", 0.31, 1.2, "FFFFFF", 1.008, {1})
            Call add(data, "He", 0.28, 1.4, "D9FFFF", 4.003, {0})
            Call add(data, "Li", 1.28, 1.82, "CC80FF", 6.941, {1})
            Call add(data, "Be", 0.96, 1.53, "C2FF00", 9.012, {2})
            Call add(data, "B", 0.84, 1.92, "FFA0FF", 10.81, {3})
            Call add(data, "C", 0.76, 1.7, "909090", 12.011, {4})
            Call add(data, "N", 0.71, 1.55, "3050F8", 14.007, {3, 5})
            Call add(data, "O", 0.66, 1.52, "FF0D0D", 15.999, {2})
            Call add(data, "F", 0.57, 1.47, "90E050", 18.998, {1})
            Call add(data, "Ne", 0.58, 1.54, "B3E3F5", 20.18, {0})
            Call add(data, "Na", 1.66, 2.27, "AB5CF2", 22.99, {1})
            Call add(data, "Mg", 1.41, 1.73, "8AFF00", 24.305, {2})
            Call add(data, "Al", 1.21, 1.84, "BFA6A6", 26.982, {3})
            Call add(data, "Si", 1.11, 2.1, "F0C8A0", 28.085, {4})
            Call add(data, "P", 1.07, 1.8, "FF8000", 30.974, {3, 5})
            Call add(data, "S", 1.05, 1.8, "FFFF30", 32.06, {2, 4, 6})
            Call add(data, "Cl", 1.02, 1.75, "1FF01F", 35.45, {1, 3, 5, 7})
            Call add(data, "Ar", 1.06, 1.88, "80D1E3", 39.948, {0})
            Call add(data, "K", 2.03, 2.75, "8F40D4", 39.098, {1})
            Call add(data, "Ca", 1.76, 2.31, "3DFF00", 40.078, {2})
            Call add(data, "Ti", 1.6, 2.0, "BFC2C7", 47.867, {4})
            Call add(data, "Cr", 1.39, 2.0, "8A99C7", 51.996, {3})
            Call add(data, "Mn", 1.39, 2.0, "9C7AC7", 54.938, {2})
            Call add(data, "Fe", 1.32, 2.0, "E06633", 55.845, {2, 3})
            Call add(data, "Co", 1.26, 2.0, "F090A0", 58.933, {2})
            Call add(data, "Ni", 1.24, 2.0, "50D050", 58.693, {2})
            Call add(data, "Cu", 1.32, 2.0, "C88033", 63.546, {2})
            Call add(data, "Zn", 1.22, 1.39, "7D80B0", 65.38, {2})
            Call add(data, "Ga", 1.22, 1.87, "C28F8F", 69.723, {3})
            Call add(data, "Ge", 1.2, 2.11, "668F8F", 72.63, {4})
            Call add(data, "As", 1.19, 1.85, "BD80E3", 74.922, {3, 5})
            Call add(data, "Se", 1.2, 1.9, "FFA100", 78.971, {2, 4, 6})
            Call add(data, "Br", 1.2, 1.85, "A62929", 79.904, {1, 3, 5})
            Call add(data, "Kr", 1.16, 2.02, "5CB8D1", 83.798, {0})
            Call add(data, "Rb", 2.2, 3.03, "702EB0", 85.468, {1})
            Call add(data, "Sr", 1.95, 2.49, "00FF00", 87.62, {2})
            Call add(data, "Zr", 1.75, 2.0, "94E0E0", 91.224, {4})
            Call add(data, "Mo", 1.54, 2.0, "54B5B5", 95.95, {6})
            Call add(data, "Ag", 1.45, 1.72, "C0C0C0", 107.87, {1})
            Call add(data, "Cd", 1.44, 1.58, "FFD98F", 112.41, {2})
            Call add(data, "In", 1.42, 1.93, "A67573", 114.82, {3})
            Call add(data, "Sn", 1.39, 2.17, "668080", 118.71, {4})
            Call add(data, "Sb", 1.39, 2.06, "9E63B5", 121.76, {3, 5})
            Call add(data, "Te", 1.38, 2.06, "D47A00", 127.6, {2, 4, 6})
            Call add(data, "I", 1.39, 1.98, "940094", 126.9, {1, 3, 5, 7})
            Call add(data, "Xe", 1.4, 2.16, "429EB0", 131.29, {0})
            Call add(data, "Cs", 2.44, 3.43, "57178F", 132.91, {1})
            Call add(data, "Ba", 2.15, 2.68, "00C900", 137.33, {2})
            Call add(data, "Pt", 1.36, 1.75, "D0D0E0", 195.08, {2, 4})
            Call add(data, "Au", 1.36, 1.66, "FFD123", 196.97, {1, 3})
            Call add(data, "Hg", 1.32, 1.55, "B8B8D0", 200.59, {2})
            Call add(data, "Pb", 1.46, 2.02, "575961", 207.2, {2, 4})
            Call add(data, "Bi", 1.48, 2.07, "9E4FB5", 208.98, {3, 5})

            Return data
        End Function

        Private Sub add(store As Dictionary(Of String, ElementInfo),
                        symbol As String,
                        covalent As Double,
                        vdw As Double,
                        color As String,
                        mass As Double,
                        valences As Integer())

            store(symbol) = New ElementInfo With {
                .Symbol = symbol,
                .CovalentRadius = covalent,
                .VdwRadius = vdw,
                .CpkColor = color,
                .AtomicMass = mass,
                .Valences = valences
            }
        End Sub

    End Module

End Namespace
