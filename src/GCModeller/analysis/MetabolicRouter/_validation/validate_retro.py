# ============================================================================
# validate_retro.py — 逆合成路径搜索核心的 Python 镜像验证
# 待验证（将 1:1 转录到 VB）：
#   1. SMILES 子集解析（支链/环闭合/电价）+ Morgan EC 规范键不变性
#   2. 隐式氢/价态模型
#   3. SMARTS 子集子图匹配（回溯）
#   4. SMIRKS 式规则应用：原子类映射、断键/成键、元素覆盖、碎片化、价态校验
#   5. 内置规则库：柠檬酸 1 步 → OAA+乙酸；苏氨酸 2 步 → 乙醛+草酸；苹果酸 1 步 → OAA
# ============================================================================
import re
from itertools import combinations

VALENCE = {"C": 4, "N": 3, "O": 2, "S": 6, "P": 5, "F": 1, "Cl": 1, "Br": 1, "I": 1}

def valence_of(el, charge):
    v = VALENCE[el]
    if el == "N":
        if charge > 0: v = 4
        elif charge < 0: v = 2
    elif el == "O":
        if charge > 0: v = 3
        elif charge < 0: v = 1
    elif el == "C" and charge != 0:
        v = 3
    return v

# ---------------- 分子图 ----------------
class Mol:
    def __init__(self):
        self.el = []          # 元素
        self.charge = []      # 电荷
        self.bonds = []       # (a, b, order)
    def n(self): return len(self.el)
    def neighbors(self, a):
        out = []
        for (x, y, o) in self.bonds:
            if x == a: out.append((y, o))
            elif y == a: out.append((x, o))
        return out
    def degree(self, a): return len(self.neighbors(a))
    def bond_order(self, a, b):
        for (x, y, o) in self.bonds:
            if (x, y) in ((a, b), (b, a)): return o
        return 0
    def implicit_h(self, a):
        used = sum(o for (_, o) in self.neighbors(a))
        return max(0, valence_of(self.el[a], self.charge[a]) - used - self.explicit_h[a])
    def copy(self):
        m = Mol()
        m.el = self.el[:]; m.charge = self.charge[:]
        m.bonds = self.bonds[:]; m.explicit_h = self.explicit_h[:]
        return m

# ---------------- SMILES 子集解析 ----------------
ELEMENTS = "Cl|Br|Si|C|N|O|S|P|F|I|B|H"

# 重新实现：解析时直接记录 explicit_h（上面的重放太丑）
def parse_smiles2(s):
    m = Mol()
    m.explicit_h = []
    stack = []
    prev = -1
    pending_order = 1
    ring_open = {}
    i = 0
    n = len(s)
    while i < n:
        c = s[i]
        if c == "(":
            stack.append(prev); i += 1
        elif c == ")":
            prev = stack.pop(); i += 1
        elif c in "-=#":
            pending_order = {"-": 1, "=": 2, "#": 3}[c]; i += 1
        elif c.isdigit():
            d = c; i += 1
            if d in ring_open:
                (ra, ro) = ring_open.pop(d)
                order = pending_order if pending_order > 1 else ro
                m.bonds.append((ra, prev, order)); pending_order = 1
            else:
                ring_open[d] = (prev, pending_order if pending_order > 1 else 1)
                pending_order = 1
        elif c == "[":
            j = s.index("]", i)
            body = s[i+1:j]
            mm = re.match(r"^([A-Z][a-z]?)(H(\d)?)?([+-]\d*|[+-])?", body)
            el = mm.group(1)
            eh = int(mm.group(3)) if mm.group(3) else (1 if mm.group(2) else 0)
            ch = mm.group(4)
            charge = 0
            if ch:
                if ch == "+": charge = 1
                elif ch == "-": charge = -1
                else: charge = int(ch[:-1]) if len(ch) > 1 else (1 if ch[0] == "+" else -1)
            idx = m.n()
            m.el.append(el); m.charge.append(charge); m.explicit_h.append(eh)
            if prev >= 0: m.bonds.append((prev, idx, pending_order))
            pending_order = 1
            prev = idx
            i = j + 1
        else:
            mm = re.match(ELEMENTS, s[i:])
            el = mm.group(0)
            idx = m.n()
            m.el.append(el); m.charge.append(0); m.explicit_h.append(0)
            if prev >= 0: m.bonds.append((prev, idx, pending_order))
            pending_order = 1
            prev = idx
            i += len(el)
    # 有效 H = explicit + implicit（bracket H 直接计入总 H）
    return m

def total_h(m, a):
    return m.explicit_h[a] + m.implicit_h(a)

def check_valence(m):
    """价态校验：Σ键级+显式H ≤ 价态（返回违规原子列表）"""
    bad = []
    for a in range(m.n()):
        used = sum(o for (_, o) in m.neighbors(a)) + m.explicit_h[a]
        if used > valence_of(m.el[a], m.charge[a]):
            bad.append(a)
    return bad

# ---------------- Morgan EC 规范键 ----------------
def morgan_ranks(m, rounds=8):
    """Morgan/EC 迭代精化：不变量 = (初始不变量, 邻居标签+键级多重集)"""
    labels = [(m.el[a], m.charge[a], m.explicit_h[a], m.degree(a),
               tuple(sorted(o for (_, o) in m.neighbors(a)))) for a in range(m.n())]
    prev_count = len(set(labels))
    for _ in range(rounds):
        new = [(labels[a], tuple(sorted((labels[b], o) for (b, o) in m.neighbors(a))))
               for a in range(m.n())]
        uniq = {}
        for x in sorted(set(new)):
            uniq[x] = len(uniq)
        labels = [uniq[x] for x in new]
        if len(set(labels)) == prev_count:
            break
        prev_count = len(set(labels))
    return labels

def mol_key(m):
    """图指纹：原子不变量 + 键三元组（Morgan 秩编号）——同分异构体不同、原子重排不变"""
    ranks = morgan_ranks(m)
    # 秩 → 紧凑编号
    uniq = {}
    for r in sorted(set(ranks)):
        uniq[r] = len(uniq)
    rk = [uniq[r] for r in ranks]
    atoms = sorted((m.el[a], m.charge[a], m.explicit_h[a], rk[a]) for a in range(m.n()))
    bonds = sorted((min(rk[a], rk[b]), max(rk[a], rk[b]), o) for (a, b, o) in m.bonds)
    return repr((atoms, bonds))

def components(m):
    """连通分量划分"""
    seen = [False] * m.n()
    comps = []
    for a in range(m.n()):
        if seen[a]: continue
        comp = []
        stack = [a]
        seen[a] = True
        while stack:
            x = stack.pop()
            comp.append(x)
            for (b, o) in m.neighbors(x):
                if not seen[b]:
                    seen[b] = True
                    stack.append(b)
        comps.append(sorted(comp))
    return comps

# ---------------- SMARTS 子集模式 ----------------
class PatAtom:
    __slots__ = ("el", "charge", "h", "deg", "cls")
    def __init__(self, el=None, charge=None, h=None, deg=None, cls=-1):
        self.el = el; self.charge = charge; self.h = h; self.deg = deg; self.cls = cls

class Pattern:
    def __init__(self):
        self.atoms = []
        self.bonds = []      # (a, b, order or 0=any)
        self.no_oh = []      # 每原子：!O 否定（无单键 OH 邻居）

def parse_pattern(s):
    """SMARTS 子集: [C] [CH2] [C+] [OH1] [CD3] [O-] [N:3]，键 - = # ~，支链 ()"""
    p = Pattern()
    stack = []
    prev = -1
    pending = 0      # 0 = any
    i = 0
    n = len(s)
    while i < n:
        c = s[i]
        if c == "(":
            stack.append(prev); i += 1
        elif c == ")":
            prev = stack.pop(); i += 1
        elif c in "-=#~":
            pending = {"-": 1, "=": 2, "#": 3, "~": 0}[c]; i += 1
        elif c == ".":
            prev = -1; i += 1      # 多组分：下一原子不与前一原子成键
        elif c == "[":
            j = s.index("]", i)
            body = s[i+1:j]
            cls = -1
            if ":" in body:
                body, clsS = body.rsplit(":", 1)
                cls = int(clsS)
            # 旗标循环解析：H\d / D\d / !O 任意顺序
            mm = re.match(r"^([A-Z][a-z]?)", body)
            if not mm: raise ValueError(f"模式原子解析失败: {body}")
            el = mm.group(1)
            rest = body[mm.end():]
            h = None; deg = None; no_oh = False
            while True:
                mh = re.match(r"^H(\d)", rest)
                md = re.match(r"^D(\d)", rest)
                mo = re.match(r"^!O", rest)
                if mh:
                    h = int(mh.group(1)); rest = rest[mh.end():]
                elif md:
                    deg = int(md.group(1)); rest = rest[md.end():]
                elif mo:
                    no_oh = True; rest = rest[mo.end():]
                else:
                    break
            ch = rest
            charge = None
            if ch:
                if ch == "+": charge = 1
                elif ch == "-": charge = -1
                else: charge = int(ch[:-1]) if len(ch) > 1 else (1 if ch[0] == "+" else -1)
            p.atoms.append(PatAtom(el, charge, h, deg, cls))
            p.no_oh.append(no_oh)
            idx = len(p.atoms) - 1
            if prev >= 0:
                p.bonds.append((prev, idx, pending))
            pending = 0
            prev = idx
            i = j + 1
        else:
            raise ValueError(f"模式解析失败 @ {i}: {s}")
    return p

def match_pattern(m, pat, limit=100):
    """子图单射匹配（回溯）。返回 [(patAtom→molAtom dict)]"""
    p = pat
    if not pat.atoms: return []
    # 匹配类 = 有键的类（孤立组分不参与匹配）；无键模式 → 全部类
    bonded_cls = set()
    for (x, y, o) in pat.bonds:
        bonded_cls.add(pat.atoms[x].cls)
        bonded_cls.add(pat.atoms[y].cls)
    if not bonded_cls:
        bonded_cls = {a.cls for a in pat.atoms}
    match_idx = [i for i in range(len(pat.atoms)) if pat.atoms[i].cls in bonded_cls]
    if not match_idx: return []
    # 模式原子按连通性排序（DFS，仅匹配子图）
    adj = {}
    for (x, y, o) in pat.bonds:
        if pat.atoms[x].cls in bonded_cls and pat.atoms[y].cls in bonded_cls:
            adj.setdefault(x, []).append(y)
            adj.setdefault(y, []).append(x)
    order = []
    seen = set()
    def dfs(a):
        if a in seen: return
        seen.add(a); order.append(a)
        for b in adj.get(a, []):
            dfs(b)
    for a in match_idx:
        dfs(a)
    for a in match_idx:
        if a not in seen: dfs(a)
    # 候选
    pa_idx_no_oh = {i: pat.no_oh[i] for i in range(len(pat.atoms))}

    def has_oh_neighbor(ma):
        for (b, o) in m.neighbors(ma):
            if o == 1 and m.el[b] == "O" and total_h(m, b) >= 1:
                return True
        return False

    def ok_atom(ma, pai):
        pa = pat.atoms[pai]
        if pa.el and m.el[ma] != pa.el: return False
        if pa.charge is not None and m.charge[ma] != pa.charge: return False
        if pa.h is not None and total_h(m, ma) != pa.h: return False
        if pa.deg is not None and m.degree(ma) != pa.deg: return False
        if pa_idx_no_oh[pai] and has_oh_neighbor(ma): return False
        return True

    cand = {}
    for pai in order:
        cand[pai] = [ma for ma in range(m.n()) if ok_atom(ma, pai)]
    results = []
    used = set()
    assign = {}
    def bt(k):
        if len(results) >= limit: return
        if k == len(order):
            results.append(dict(assign))
            return
        pa = order[k]
        for ma in cand[pa]:
            if ma in used: continue
            # 键约束：与已指派的模式邻居（仅匹配子图内的键）
            good = True
            for (x, y, o) in p.bonds:
                if y == pa and x in assign:
                    bo = m.bond_order(assign[x], ma)
                    if o == 0:
                        if bo == 0: good = False; break
                    else:
                        if bo != o: good = False; break
                elif x == pa and y in assign:
                    bo = m.bond_order(assign[y], ma)
                    if o == 0:
                        if bo == 0: good = False; break
                    else:
                        if bo != o: good = False; break
            if not good: continue
            used.add(ma); assign[pa] = ma
            bt(k + 1)
            used.discard(ma); del assign[pa]
    bt(0)
    # 返回 {类号 → 分子原子}（SMIRKS 原子映射货币）
    cls_results = []
    seen_cls = set()
    for mp in results:
        cm = {}
        okc = True
        for pai, ma in mp.items():
            c = pat.atoms[pai].cls
            if c < 0 or c in cm:
                okc = False
                break
            cm[c] = ma
        if okc:
            key = frozenset(cm.items())
            if key not in seen_cls:
                seen_cls.add(key)
                cls_results.append(cm)
    return cls_results

# ---------------- 规则应用（SMIRKS 式）----------------
class Rule:
    def __init__(self, rid, name, reactant, product, dg, tier, reversible=True):
        self.id = rid; self.name = name
        self.reactant = parse_pattern(reactant)
        self.product = parse_pattern(product)
        self.dg = dg; self.tier = tier; self.reversible = reversible

CURRENCY_SMILES = {"O": "H2O", "O=C=O": "CO2", "N": "NH3", "OP(=O)(O)O": "Pi", "S": "CoA-SH"}

def apply_rule(m, pat_match_side, pat_other_side, limit=50):
    """把 match 侧模式匹配到 m（仅有键类），按 other 侧拓扑变换。
    类语义：两侧共有类 → 原子存活（元素/电荷可覆盖）；
    match 侧有键、other 侧孤立或缺失 → 键删除（原子成独立碎片=共产物）；
    other 侧有键、match 侧缺失 → 创建原子。
    返回 [(fragments:[Mol], mapped:[(cls, molAtom)])]。价态违规 → 丢弃。"""
    matches = match_pattern(m, pat_match_side, limit)
    out = []
    # other 侧拓扑
    other_bonded = set()
    for (x, y, o) in pat_other_side.bonds:
        other_bonded.add(pat_other_side.atoms[x].cls)
        other_bonded.add(pat_other_side.atoms[y].cls)
    other_atom = {a.cls: a for a in pat_other_side.atoms}
    other_bonds = {}
    for (x, y, o) in pat_other_side.bonds:
        cx, cy = pat_other_side.atoms[x].cls, pat_other_side.atoms[y].cls
        other_bonds[(min(cx, cy), max(cx, cy))] = o
    match_bonds = {}
    for (x, y, o) in pat_match_side.bonds:
        cx, cy = pat_match_side.atoms[x].cls, pat_match_side.atoms[y].cls
        match_bonds[(min(cx, cy), max(cx, cy))] = o
    for mp in matches:
        res = m.copy()
        mapped = []
        surv = {}
        # 1) 存活原子：匹配类（元素/电荷按 other 侧覆盖）
        for cls, ma in mp.items():
            surv[cls] = ma
            pa = other_atom.get(cls)
            if pa is not None:
                if pa.el:
                    res.el[ma] = pa.el
                if pa.charge is not None:
                    res.charge[ma] = pa.charge
                # H 约束在 other 侧为模板信息（隐式氢自动重算），不直接设置
            mapped.append((cls, ma))
        # 2) 创建原子：other 侧有键但未匹配的类
        for cls in other_bonded:
            if cls in surv:
                continue
            pa = other_atom[cls]
            idx = res.n()
            res.el.append(pa.el)
            res.charge.append(pa.charge if pa.charge is not None else 0)
            res.explicit_h.append(0)
            surv[cls] = idx
            mapped.append((cls, idx))
        # 3) 键处理：所有 (surv 类对)
        allcls = sorted(surv.keys())
        for i in range(len(allcls)):
            for j in range(i + 1, len(allcls)):
                ca, cb = allcls[i], allcls[j]
                key = (min(ca, cb), max(ca, cb))
                a, b = surv[ca], surv[cb]
                in_other = key in other_bonds
                in_match = key in match_bonds
                if in_other:
                    order = other_bonds[key]
                    if order == 0:
                        order = 1
                    bo = res.bond_order(a, b)
                    if bo == 0:
                        res.bonds.append((a, b, order))
                    elif bo != order:
                        res.bonds = [(x, y, order if (x, y) in ((a, b), (b, a)) else o2)
                                     for (x, y, o2) in res.bonds]
                elif in_match:
                    # other 侧无此键且 match 侧有 → 断键
                    res.bonds = [(x, y, o2) for (x, y, o2) in res.bonds
                                 if (x, y) not in ((a, b), (b, a))]
                # 两者皆无 → 环境键，保留
        # 4) 价态校验
        if check_valence(res):
            continue
        # 5) 碎片化
        frags = components(res)
        frag_mols = []
        for comp in frags:
            fm = Mol()
            fm.explicit_h = []
            remap = {}
            for a in comp:
                remap[a] = fm.n()
                fm.el.append(res.el[a]); fm.charge.append(res.charge[a])
                fm.explicit_h.append(res.explicit_h[a])
            for (x, y, o) in res.bonds:
                if x in remap and y in remap:
                    fm.bonds.append((remap[x], remap[y], o))
            frag_mols.append(fm)
        out.append((frag_mols, mapped))
    return out

def rule_applications(m, rule):
    """两个方向的应用：forward = match reactant→product；reverse = match product→reactant"""
    fwd = apply_rule(m, rule.reactant, rule.product)
    rev = apply_rule(m, rule.product, rule.reactant)
    return [("forward", fwd), ("reverse", rev)]

def smiles_of(m):
    """输出简化 SMILES（确定性 DFS，按 Morgan 秩）——仅用于展示/调试"""
    ranks = morgan_ranks(m)
    if m.n() == 0: return ""
    start = min(range(m.n()), key=lambda a: (ranks[a], a))
    visited = set()
    parts = []
    def emit(a, prev):
        visited.add(a)
        s = m.el[a]
        if m.charge[a]:
            s += ("+" if m.charge[a] > 0 else "-")
            if abs(m.charge[a]) > 1: s += str(abs(m.charge[a]))
        nbs = sorted(m.neighbors(a), key=lambda t: (ranks[t[0]], t[0]))
        branches = [x for x in nbs if x[0] != prev and x[0] in visited]
        cont = [x for x in nbs if x[0] != prev and x[0] not in visited]
        for (b, o) in branches[:-1] if cont else branches:
            bo = {1: "", 2: "=", 3: "#"}[o]
            parts.append("(" + bo + mol_atom_str(m, b) + ")")
        first = True
        for (b, o) in cont:
            bo = {1: "", 2: "=", 3: "#"}[o]
            if first and prev >= 0:
                parts.append(bo + mol_atom_str(m, b))
            else:
                parts.append(bo + mol_atom_str(m, b))
            emit(b, a)
            first = False
        for (b, o) in cont:
            pass
    def mol_atom_str(mm, a):
        return mm.el[a]
    # 简化：直接输出遍历序列
    parts2 = []
    def dfs2(a, prev):
        visited.add(a)
        parts2.append(m.el[a])
        nbs = sorted(m.neighbors(a), key=lambda t: (ranks[t[0]], t[0]))
        for i2, (b, o) in enumerate(nbs):
            if b == prev: continue
            bo = {1: "", 2: "=", 3: "#"}[o]
            if b in visited:
                parts2.append("(" + bo + m.el[b] + ")")
            else:
                parts2.append(bo)
                dfs2(b, a)
    dfs2(start, -1)
    return "".join(parts2)

# ---------------- 内置规则库 ----------------
RULES = [
    Rule("R001", "醇脱氢酶（氧化/还原）", "[C:1]-[OH1:2]", "[C:1]=[O:2]", 18, 1),
    Rule("R002", "转氨酶（酮→胺/胺→酮）", "[CD3H0!O:1]=[O:2]", "[CD3H1!O:1]-[N:3].[O:2]", 10, 1),
    Rule("R003", "醛缩酶（β-羟羰基裂解）", "[C:1](-[OH1:2])-[C:3]-[C:4]=[O:5]",
         "[C:1]=[O:2].[C:3]-[C:4]=[O:5]", -15, 1),
    Rule("R004", "脱羧酶", "[C:1]-[C:2](=[O:3])-[OH1:4]", "[C:1].[O:3]=[C:2]=[O:4]", -28, 1),
    Rule("R005", "水合酶（C=C 水合/脱水）", "[C:1]=[C:2]", "[C:1](-[O:3])-[C:2]", -12, 1),
    Rule("R006", "激酶（磷酸化）", "[OH1:1]", "[O:1]-[P:2](=[O:3])-[O-:4]", 25, 1),
    Rule("R007", "酯酶（水解/酯化）", "[C:1](=[O:2])-[O:3]", "[C:1](=[O:2])-[O:5].[O:3]", -20, 1),
    Rule("R008", "硫解酶（Claisen 裂解）", "[C:1](=[O:2])-[CH2:3]-[C:4](=[O:5])",
         "[C:1](=[O:2])-[S:6].[C:3]-[C:4]=[O:5]", -25, 2),
    Rule("R009", "酮-烯醇互变异构酶", "[C:1]-[C:2]=[O:3]", "[C:1]=[C:2]-[OH1:3]", -2, 2),
]

# ---------------- 测试 ----------------
def test_smiles_and_key():
    print("=== 1. SMILES 解析 + 指纹不变性 ===")
    m1 = parse_smiles2("OC(=O)CC(O)C(=O)O")     # 苹果酸
    m2 = parse_smiles2("O=C(O)C(O)CC(O)=O")    # 同一分子不同写法？验证结构相同
    # 更严格：重排原子顺序的等价写法
    m3 = parse_smiles2("C(C(=O)O)C(O)C(=O)O")
    k1, k3 = mol_key(m1), mol_key(m3)
    print(f"  苹果酸原子数={m1.n()} 键数={len(m1.bonds)}")
    ok = (k1 == k3)
    # 乙醇隐式氢
    e = parse_smiles2("CCO")
    h = [total_h(e, a) for a in range(3)]
    print(f"  乙醇各原子 H = {h}（期望 [3,2,1]）")
    ok = ok and h == [3, 2, 1]
    # 环闭合：苯（Kekulé）C1=CC=CC=C1
    b = parse_smiles2("C1=CC=CC=C1")
    border = sum(o for (_, _, o) in b.bonds)
    print(f"  苯（Kekulé）原子={b.n()} 键={len(b.bonds)} 键级和={border}（期望 6/6/9）")
    ok = ok and b.n() == 6 and len(b.bonds) == 6 and border == 9
    # 电荷：铵 [NH4+]
    am = parse_smiles2("[NH4+]")
    print(f"  铵: el={am.el[0]} charge={am.charge[0]} H={total_h(am,0)}（期望 4）")
    ok = ok and am.charge[0] == 1 and total_h(am, 0) == 4
    # 价态校验：乙烷正常，C 五键违规
    ok_mol = parse_smiles2("CC(=O)O")
    print(f"  乙酸价态违规原子 = {check_valence(ok_mol)}（期望 []）")
    ok = ok and not check_valence(ok_mol)
    print(f"  指纹不变性+解析: {'OK' if ok else 'FAIL'}")
    return 0 if ok else 1

def test_match():
    print("=== 2. 子图匹配 ===")
    propanol = parse_smiles2("CCCO")
    eth_pattern = parse_pattern("[C:1]-[OH1:2]")
    matches = match_pattern(propanol, eth_pattern)
    print(f"  [C]-[OH1] 在丙醇中匹配数 = {len(matches)}（期望 1）")
    ok = len(matches) == 1
    propane = parse_smiles2("CCC")
    ok = ok and len(match_pattern(propane, eth_pattern)) == 0
    keto = parse_pattern("[CD3:1]=[O:2]")
    pyr = parse_smiles2("CC(=O)C(=O)O")
    mset = match_pattern(pyr, keto)
    print(f"  [CD3]=[O] 在丙酮酸匹配 = {len(mset)}（期望 2：酮 C 与羧基 C 均为 D3）")
    ok = ok and len(mset) == 2
    # 带 !O 否定的转氨酶模式：仅酮 C
    keto2 = parse_pattern("[CD3H0!O:1]=[O:2]")
    mset2 = match_pattern(pyr, keto2)
    print(f"  [CD3H0!O]=[O] 在丙酮酸匹配 = {len(mset2)}（期望 1：仅酮 C）")
    ok = ok and len(mset2) == 1
    # 羧酸不匹配醇
    acetic = parse_smiles2("CC(=O)O")
    n_raw = len(match_pattern(acetic, eth_pattern))
    print(f"  [C]-[OH1] 在乙酸原匹配 = {n_raw}（羧基 C-OH 也匹配，应用层价态排除）")
    ok = ok and n_raw == 1
    n_app = len(apply_rule(acetic, parse_pattern("[C:1]-[OH1:2]"), parse_pattern("[C:1]=[O:2]")))
    print(f"  乙酸氧化应用数 = {n_app}（价态校验拒绝，期望 0）")
    ok = ok and n_app == 0
    print(f"  匹配正负例: {'OK' if ok else 'FAIL'}")
    return 0 if ok else 1

def test_rules():
    print("=== 3. 规则应用 ===")
    ok = True
    # 乙醇氧化 → 乙醛
    eth = parse_smiles2("CCO")
    apps = apply_rule(eth, RULES[0].reactant, RULES[0].product)
    frags = apps[0][0] if apps else []
    keys = sorted(mol_key(f) for f in frags)
    acetaldehyde = mol_key(parse_smiles2("CC=O"))
    print(f"  乙醇氧化产物键 = {[len(f.bonds) for f in frags]}  含乙醛 = {acetaldehyde in keys}")
    ok = ok and acetaldehyde in keys
    # 苹果酸氧化 → 草酰乙酸（价态自动排除羧基匹配）
    mal = parse_smiles2("OC(=O)CC(O)C(=O)O")
    apps = apply_rule(mal, RULES[0].reactant, RULES[0].product)
    keys = sorted(mol_key(f) for f in apps[0][0]) if apps else []
    oaa = mol_key(parse_smiles2("OC(=O)CC(=O)C(=O)O"))
    print(f"  苹果酸→OAA: 应用数={len(apps)}  含 OAA = {oaa in keys}")
    ok = ok and len(apps) == 1 and oaa in keys
    # 柠檬酸醛缩裂解 → OAA + 乙酸
    cit = parse_smiles2("OC(=O)CC(O)(CC(=O)O)C(=O)O")
    apps = apply_rule(cit, RULES[2].reactant, RULES[2].product)
    keys = sorted(mol_key(f) for f in apps[0][0]) if apps else []
    acet = mol_key(parse_smiles2("CC(=O)O"))
    print(f"  柠檬酸裂解: 应用数={len(apps)}（两个对称 CH2 臂）含 OAA = {oaa in keys}  含乙酸 = {acet in keys}")
    ok = ok and len(apps) == 2 and oaa in keys and acet in keys
    # 苏氨酸转氨（reverse）→ 2-氧代-3-羟基丁酸 + NH3
    thr = parse_smiles2("CC(O)C(N)C(=O)O")
    apps = apply_rule(thr, RULES[1].product, RULES[1].reactant)
    keys = sorted(mol_key(f) for f in apps[0][0]) if apps else []
    oxobut = mol_key(parse_smiles2("CC(O)C(=O)C(=O)O"))
    ammonia = mol_key(parse_smiles2("N"))
    print(f"  苏氨酸转氨: 应用数={len(apps)}  含 2-氧-3-羟丁酸 = {oxobut in keys}  含 NH3 = {ammonia in keys}")
    ok = ok and len(apps) == 1 and oxobut in keys and ammonia in keys
    # 2-氧-3-羟丁酸醛缩 → 乙醛 + 草酸
    apps2 = apply_rule(parse_smiles2("CC(O)C(=O)C(=O)O"), RULES[2].reactant, RULES[2].product)
    keys2 = sorted(mol_key(f) for f in apps2[0][0]) if apps2 else []
    glyox = mol_key(parse_smiles2("OC(=O)C=O"))
    print(f"  醛缩裂解: 含乙醛 = {acetaldehyde in keys2}  含乙醛酸 = {glyox in keys2}")
    ok = ok and acetaldehyde in keys2 and glyox in keys2
    # 延胡索酸水合（forward）→ 苹果酸
    fum = parse_smiles2("OC(=O)C=CC(=O)O")
    apps3 = apply_rule(fum, RULES[4].reactant, RULES[4].product)
    keys3 = sorted(mol_key(f) for f in apps3[0][0]) if apps3 else []
    print(f"  延胡索酸水合: 含苹果酸 = {mol_key(mal) in keys3}")
    ok = ok and mol_key(mal) in keys3
    # 苹果酸脱水（reverse）→ 延胡索酸 + H2O
    apps4 = apply_rule(mal, RULES[4].product, RULES[4].reactant)
    keys4 = [mol_key(f) for app in apps4 for f in app[0]]
    water = mol_key(parse_smiles2("O"))
    print(f"  苹果酸脱水: 应用数={len(apps4)} 含延胡索酸 = {mol_key(fum) in keys4}  含 H2O = {water in keys4}")
    ok = ok and mol_key(fum) in keys4 and water in keys4
    # 脱羧：草酰乙酸 → 丙酮酸 + CO2
    apps5 = apply_rule(parse_smiles2("OC(=O)C(=O)CC(=O)O"), RULES[3].reactant, RULES[3].product)
    pyr = mol_key(parse_smiles2("CC(=O)C(=O)O"))
    co2 = mol_key(parse_smiles2("O=C=O"))
    for ai, (frags, mapped) in enumerate(apps5):
        print(f"    OAA 脱羧应用{ai}: 碎片SMILES骨架 = {[smiles_of(f) for f in frags]} 映射={mapped}")
        for f in frags:
            print(f"      碎片: 原子={[f.el[a] for a in range(f.n())]} 键={f.bonds}")
    keys5 = [mol_key(f) for app in apps5 for f in app[0]]
    print(f"  OAA 脱羧: 应用数={len(apps5)} 含丙酮酸 = {pyr in keys5}  含 CO2 = {co2 in keys5}")
    ok = ok and len(apps5) >= 1 and pyr in keys5 and co2 in keys5
    print(f"  规则应用: {'OK' if ok else 'FAIL'}")
    return 0 if ok else 1

# ---------------- 束搜索 ----------------
CURRENCY_SMILES = ["O", "O=C=O", "N", "S"]
CURRENCY_KEYS = set()

def init_currency():
    for s in CURRENCY_SMILES:
        CURRENCY_KEYS.add(mol_key(parse_smiles2(s)))

def beam_search(target_smiles, sink_smiles, rules, beam_width=50, max_depth=6):
    """状态 = 待汇化合物集合；扩展 = 单化合物 × 规则 × 双向；
    循环消除（used 集合）、状态去重、束剪枝 [operon... readme §3]"""
    if not CURRENCY_KEYS:
        init_currency()
    sink_keys = {mol_key(parse_smiles2(s)): s for s in sink_smiles}
    target = parse_smiles2(target_smiles)
    tkey = mol_key(target)
    if tkey in sink_keys:
        return []
    st0 = {"pending": [(tkey, target)], "steps": [], "used": frozenset([tkey])}
    frontier = [st0]
    completed = []
    for depth in range(max_depth):
        next_states = []
        for st in frontier:
            for (ckey, cmol) in st["pending"]:
                for rule in rules:
                    for orient, (mp_, op_) in (("forward", (rule.reactant, rule.product)),
                                               ("reverse", (rule.product, rule.reactant))):
                        try:
                            apps = apply_rule(cmol, mp_, op_)
                        except Exception:
                            continue
                        for frags, mapped in apps:
                            precursors = []
                            coproducts = []
                            for f in frags:
                                fk = mol_key(f)
                                if fk in CURRENCY_KEYS or fk == ckey:
                                    coproducts.append(fk)
                                else:
                                    precursors.append((fk, f))
                            # 循环消除：前体已在祖先集
                            if any(pk in st["used"] for pk, _ in precursors):
                                continue
                            newused = set(st["used"])
                            pend = []
                            for (pk, pm) in precursors:
                                newused.add(pk)
                                if pk not in sink_keys:
                                    pend.append((pk, pm))
                            rest = [(k, m) for (k, m) in st["pending"] if k != ckey]
                            ns = {"pending": rest + pend,
                                  "steps": st["steps"] + [{
                                      "rule": rule.id, "name": rule.name, "orient": orient,
                                      "substrate": ckey,
                                      "precursors": [k for k, _ in precursors],
                                      "coproducts": len(coproducts),
                                      "dg": rule.dg * (1 if orient == "forward" else -1),
                                      "tier": rule.tier}],
                                  "used": frozenset(newused)}
                            if not ns["pending"]:
                                completed.append(ns)
                            else:
                                next_states.append(ns)
        # 状态去重 + 束剪枝（确定性排序）
        seen = set()
        dedup = []
        for st in sorted(next_states, key=lambda s: (len(s["pending"]),
                                                     sum(m.n() for _, m in s["pending"]),
                                                     s["steps"][-1]["dg"])):
            sk = tuple(sorted(k for k, _ in st["pending"]))
            if sk in seen:
                continue
            seen.add(sk)
            dedup.append(st)
        frontier = dedup[:beam_width]
        if not frontier:
            break
    return completed

def test_search():
    print("=== 4. 束搜索端到端 ===")
    ok = True
    sink = ["CC(=O)C(=O)O",      # 丙酮酸
            "OC(=O)C(=O)CC(=O)O",  # 草酰乙酸
            "CC(=O)O",             # 乙酸
            "CC=O",                # 乙醛
            "NCC(=O)O",            # 甘氨酸
            "OC(=O)C=O",           # 乙醛酸
            "O", "O=C=O", "N",
            "OC(=O)C(=O)O"]        # 草酸
    # 柠檬酸：1 步（醛缩裂解 → OAA + 乙酸）
    paths = beam_search("OC(=O)CC(O)(CC(=O)O)C(=O)O", sink, RULES, beam_width=20, max_depth=3)
    r1 = [p for p in paths if len(p["steps"]) == 1 and p["steps"][0]["rule"] == "R003"]
    print(f"  柠檬酸: 路径数={len(paths)} 1步醛缩路径={len(r1)}")
    ok = ok and len(r1) >= 1
    # 苏氨酸：1 步醛缩（甘氨酸+乙醛）与 2 步（转氨→醛缩）
    paths2 = beam_search("CC(O)C(N)C(=O)O", sink, RULES, beam_width=30, max_depth=4)
    r_direct = [p for p in paths2 if len(p["steps"]) == 1]
    r_two = [p for p in paths2 if len(p["steps"]) == 2]
    print(f"  苏氨酸: 总路径={len(paths2)} 1步={len(r_direct)}（醛缩 R003）2步={len(r_two)}")
    ok = ok and any(p["steps"][0]["rule"] == "R003" for p in r_direct)
    ok = ok and any(p["steps"][0]["rule"] == "R002" and p["steps"][1]["rule"] == "R003"
                    for p in r_two)
    # 苹果酸：1 步氧化 → OAA
    paths3 = beam_search("OC(=O)CC(O)C(=O)O", sink, RULES, beam_width=20, max_depth=3)
    r3 = [p for p in paths3 if len(p["steps"]) == 1 and p["steps"][0]["rule"] == "R001"]
    print(f"  苹果酸: 路径数={len(paths3)} 1步氧化路径={len(r3)}")
    ok = ok and len(r3) >= 1
    # 循环消除：路径内无重复化合物
    cyc_ok = True
    for p in paths2:
        seen = set()
        for s in p["steps"]:
            if s["substrate"] in seen:
                cyc_ok = False
            seen.add(s["substrate"])
    print(f"  循环消除: {'OK' if cyc_ok else 'FAIL'}")
    ok = ok and cyc_ok
    print(f"  束搜索: {'OK' if ok else 'FAIL'}")
    return 0 if ok else 1

if __name__ == "__main__":
    total = 0
    total += test_smiles_and_key()
    total += test_match()
    total += test_rules()
    total += test_search()
    print(f"\n总计: {'ALL PASS' if total == 0 else f'{total} FAILS'}")
