Imports SMRUCC.genomics.Molecule.SMILES.Graph

''' <summary>
''' demo 使用的分子用例定义。
''' </summary>
Friend Class MoleculeCase

    ''' <summary>用例名称（同时作为输出文件名前缀）。</summary>
    Public Property Name As String
    ''' <summary>结构式字符串。</summary>
    Public Property Text As String
    ''' <summary>显式指定的格式；<see cref="MoleculeFormat.Unknown"/> 表示自动识别。</summary>
    Public Property Format As MoleculeFormat = MoleculeFormat.Unknown
    ''' <summary>期望的分子式。</summary>
    Public Property Formula As String
    ''' <summary>期望的重原子数。</summary>
    Public Property Atoms As Integer
    ''' <summary>期望的键数。</summary>
    Public Property Bonds As Integer
    ''' <summary>期望的环数（最小环基大小）。</summary>
    Public Property Rings As Integer
    ''' <summary>是否输出结构图像。</summary>
    Public Property Render As Boolean = True

End Class

''' <summary>
''' demo 用例表：SMILES 与 InChI 各若干条，覆盖链状、环状、稠环、带电、多组分与手性分子。
''' </summary>
Friend Module TestCases

    ''' <summary>
    ''' 返回全部用例。
    ''' </summary>
    Public Function All() As List(Of MoleculeCase)
        Dim cases As New List(Of MoleculeCase)

        Call add(cases, "ethanol", "CCO", "C2H6O", 3, 2, 0)
        Call add(cases, "benzene", "c1ccccc1", "C6H6", 6, 6, 1)
        Call add(cases, "acetic-acid", "CC(=O)O", "C2H4O2", 4, 3, 0)
        Call add(cases, "cyclohexane", "C1CCCCC1", "C6H12", 6, 6, 1)
        Call add(cases, "phenol", "Oc1ccccc1", "C6H6O", 7, 7, 1)
        Call add(cases, "naphthalene", "c1ccc2ccccc2c1", "C10H8", 10, 11, 2)
        Call add(cases, "caffeine", "Cn1cnc2c1c(=O)n(C)c(=O)n2C", "C8H10N4O2", 14, 15, 2)
        Call add(cases, "alanine", "N[C@@H](C)C(=O)O", "C3H7NO2", 6, 5, 0)
        Call add(cases, "glucose", "OC[C@H]1OC(O)[C@H](O)[C@@H](O)[C@@H]1O", "C6H12O6", 12, 12, 1)
        Call add(cases, "sodium-chloride", "[Na+].[Cl-]", "ClNa", 2, 0, 0)

        Call addInChI(cases, "inchi-methane", "InChI=1S/CH4/h1H4", "CH4", 1, 0, 0)
        Call addInChI(cases, "inchi-ethanol", "InChI=1S/C2H6O/c1-2-3/h3H,2H2,1H3", "C2H6O", 3, 2, 0)
        Call addInChI(cases, "inchi-benzene", "InChI=1S/C6H6/c1-2-4-6-5-3-1/h1-6H", "C6H6", 6, 6, 1)
        Call addInChI(cases, "inchi-acetic-acid", "InChI=1S/C2H4O2/c1-2(3)4/h1H3,(H,3,4)", "C2H4O2", 4, 3, 0)
        Call addInChI(cases, "inchi-water", "InChI=1S/H2O/h1H2", "H2O", 1, 0, 0)
        Call addInChI(cases, "inchi-alanine",
                      "InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)",
                      "C3H7NO2", 6, 5, 0)

        Return cases
    End Function

    Private Sub add(store As List(Of MoleculeCase), name As String, smiles As String,
                    formula As String, atoms As Integer, bonds As Integer, rings As Integer)

        store.Add(New MoleculeCase With {
            .Name = name,
            .Text = smiles,
            .Format = MoleculeFormat.Smiles,
            .Formula = formula,
            .Atoms = atoms,
            .Bonds = bonds,
            .Rings = rings
        })
    End Sub

    Private Sub addInChI(store As List(Of MoleculeCase), name As String, inchi As String,
                         formula As String, atoms As Integer, bonds As Integer, rings As Integer)

        store.Add(New MoleculeCase With {
            .Name = name,
            .Text = inchi,
            .Format = MoleculeFormat.InChI,
            .Formula = formula,
            .Atoms = atoms,
            .Bonds = bonds,
            .Rings = rings
        })
    End Sub

End Module
