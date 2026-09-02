' ============================================================================
' ResidueTemplates.vb — 20 标准氨基酸模板（原子→Vina 类型 + 连接表）
' ----------------------------------------------------------------------------
' 蛋白原子类型按 PDB 原子名直接查表（比几何推断可靠）：
'   主链 N → N（给体型），CA/CB/... → C，C=O 的 O → OA；
'   芳香环 C → A（PHE/TYR/TRP/HIS 的环位）；
'   酸性 OH/羧基 O → OA；碱性 N（NZ, NH1, NH2, NE, ND1...）→ N；
'   MET.SD → S，CYS.SG → S。
' 连接表供 PEOE 电荷计算；键级：C=O 双键，芳环 1.5，其余单键。
' ============================================================================

Imports SMRUCC.genomics.Data.RCSB.PDB.Structures

Namespace Core

    Public Module ResidueTemplates

        ''' <summary>模板 DSL：残基名 | 原子定义(名:元素:类型) 用逗号分隔 | 键(名-名[=ord]) 用逗号分隔</summary>
        Private ReadOnly Templates() As String = {
            "GLY|N:N:N,CA:C:C,C:C:C,O:OA:OA|N-CA,CA-C,C=O",
            "ALA|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C|N-CA,CA-C,C=O,CA-CB",
            "SER|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,OG:O:OA|N-CA,CA-C,C=O,CA-CB,CB-OG",
            "CYS|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,SG:S:S|N-CA,CA-C,C=O,CA-CB,CB-SG",
            "THR|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,OG1:O:OA,CG2:C:C|N-CA,CA-C,C=O,CA-CB,CB-OG1,CB-CG2",
            "VAL|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG1:C:C,CG2:C:C|N-CA,CA-C,C=O,CA-CB,CB-CG1,CB-CG2",
            "LEU|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG:C:C,CD1:C:C,CD2:C:C|N-CA,CA-C,C=O,CA-CB,CB-CG,CG-CD1,CG-CD2",
            "ILE|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG1:C:C,CG2:C:C,CD1:C:C|N-CA,CA-C,C=O,CA-CB,CB-CG1,CB-CG2,CG1-CD1",
            "PRO|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG:C:C,CD:C:C|N-CA,CA-C,C=O,CA-CB,CB-CG,CG-CD,CD-N",
            "ASP|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG:C:C,OD1:O:OA,OD2:O:OA|N-CA,CA-C,C=O,CA-CB,CB-CG,CG=OD1,CG-OD2",
            "GLU|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG:C:C,CD:C:C,OE1:O:OA,OE2:O:OA|N-CA,CA-C,C=O,CA-CB,CB-CG,CG-CD,CD=OE1,CD-OE2",
            "ASN|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG:C:C,OD1:O:OA,ND2:N:N|N-CA,CA-C,C=O,CA-CB,CB-CG,CG=OD1,CG-ND2",
            "GLN|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG:C:C,CD:C:C,OE1:O:OA,NE2:N:N|N-CA,CA-C,C=O,CA-CB,CB-CG,CG-CD,CD=OE1,CD-NE2",
            "MET|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG:C:C,SD:S:S,CE:C:C|N-CA,CA-C,C=O,CA-CB,CB-CG,CG-SD,SD-CE",
            "PHE|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG:A:A,CD1:A:A,CD2:A:A,CE1:A:A,CE2:A:A,CZ:A:A|N-CA,CA-C,C=O,CA-CB,CB-CG,CG-CD1,CG-CD2,CD1-CE1,CD2-CE2,CE1-CZ,CE2-CZ",
            "TYR|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG:A:A,CD1:A:A,CD2:A:A,CE1:A:A,CE2:A:A,CZ:A:A,OH:O:OA|N-CA,CA-C,C=O,CA-CB,CB-CG,CG-CD1,CG-CD2,CD1-CE1,CD2-CE2,CE1-CZ,CE2-CZ,CZ-OH",
            "TRP|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG:A:A,CD1:A:A,CD2:A:A,NE1:N:N,CE2:A:A,CE3:A:A,CZ2:A:A,CZ3:A:A,CH2:A:A|N-CA,CA-C,C=O,CA-CB,CB-CG,CG-CD1,CG-CD2,CD1-NE1,NE1-CE2,CD2-CE2,CD2-CE3,CE3-CZ3,CZ3-CH2,CH2-CZ2,CZ2-CE2",
            "HIS|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG:A:A,ND1:N:NA,CD2:A:A,CE1:A:A,NE2:N:NA|N-CA,CA-C,C=O,CA-CB,CB-CG,CG-ND1,CG-CD2,ND1-CE1,CE1-NE2,NE2-CD2",
            "LYS|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG:C:C,CD:C:C,CE:C:C,NZ:N:N|N-CA,CA-C,C=O,CA-CB,CB-CG,CG-CD,CD-CE,CE-NZ",
            "ARG|N:N:N,CA:C:C,C:C:C,O:OA:OA,CB:C:C,CG:C:C,CD:C:C,NE:N:N,CZ:C:C,NH1:N:N,NH2:N:N|N-CA,CA-C,C=O,CA-CB,CB-CG,CG-CD,CD-NE,NE-CZ,CZ-NH1,CZ-NH2"}

        Private ReadOnly _typeMap As Dictionary(Of String, Dictionary(Of String, Int32)) = BuildTypeMap()
        Private ReadOnly _bondMap As Dictionary(Of String, List(Of Bond)) = BuildBondMap()

        Private Function BuildTypeMap() As Dictionary(Of String, Dictionary(Of String, Int32))
            Dim result As New Dictionary(Of String, Dictionary(Of String, Int32))(StringComparer.Ordinal)
            For Each tpl In Templates
                Dim parts = tpl.Split("|"c)
                Dim resName = parts(0)
                Dim atomMap As New Dictionary(Of String, Int32)(StringComparer.Ordinal)
                For Each def In parts(1).Split(","c)
                    Dim f = def.Split(":"c)
                    atomMap(f(0)) = TypeFromName(f(2))
                Next
                result(resName) = atomMap
            Next
            Return result
        End Function

        Private Function BuildBondMap() As Dictionary(Of String, List(Of Bond))
            ' 返回残基内 原子名→(原子名, 键级) 列表；此处存原始字符串由 TryGetBonds 解析
            Return New Dictionary(Of String, List(Of Bond))()
        End Function

        Private ReadOnly _bondSpecs As Dictionary(Of String, String()) = BuildBondSpecs()

        Private Function BuildBondSpecs() As Dictionary(Of String, String())
            Dim result As New Dictionary(Of String, String())(StringComparer.Ordinal)
            For Each tpl In Templates
                Dim parts = tpl.Split("|"c)
                result(parts(0)) = parts(2).Split(","c)
            Next
            Return result
        End Function

        Private Function TypeFromName(name As String) As Int32
            Select Case name
                Case "C" : Return VinaAtomTypes.TC
                Case "A" : Return VinaAtomTypes.TA
                Case "N" : Return VinaAtomTypes.TN
                Case "NA" : Return VinaAtomTypes.TNA
                Case "OA" : Return VinaAtomTypes.TOA
                Case "S" : Return VinaAtomTypes.TS
                Case "SA" : Return VinaAtomTypes.TSA
                Case "P" : Return VinaAtomTypes.TP
                Case "F" : Return VinaAtomTypes.TF
                Case "Cl" : Return VinaAtomTypes.TCl
                Case "Br" : Return VinaAtomTypes.TBr
                Case "I" : Return VinaAtomTypes.TI
                Case Else : Return VinaAtomTypes.TMetal
            End Select
        End Function

        Public Function TryGetType(resName As String, atomName As String, element As String, ByRef vinaType As Int32) As Boolean
            Dim atomMap As Dictionary(Of String, Int32) = Nothing
            If _typeMap.TryGetValue(resName, atomMap) Then
                If atomMap.TryGetValue(atomName, vinaType) Then Return True
            End If
            ' 非标准残基名（如修饰氨基酸）：主链名兜底
            Select Case atomName
                Case "N" : vinaType = VinaAtomTypes.TN : Return element = "N"
                Case "CA", "C", "O" : Return False
            End Select
            Return False
        End Function

        ''' <summary>残基内连接表（原子名键对与键级）；找不到残基返回 False</summary>
        Public Function TryGetBondSpecs(resName As String, ByRef specs() As String) As Boolean
            Return _bondSpecs.TryGetValue(resName, specs)
        End Function

        Public Function GetStandardResidueNames() As IEnumerable(Of String)
            Return _typeMap.Keys
        End Function

        ''' <summary>残基形式电荷（生理 pH）：ASP/GLU −1，LYS/ARG +1，HIS 0，其余 0</summary>
        Public Function FormalCharge(resName As String) As Integer
            Select Case resName
                Case "ASP", "GLU" : Return -1
                Case "LYS", "ARG" : Return 1
                Case Else : Return 0
            End Select
        End Function

    End Module

End Namespace
