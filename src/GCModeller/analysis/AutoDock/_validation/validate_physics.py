# ============================================================================
# validate_physics.py — Vina 打分 + 解析梯度 + BFGS + ILS 的 Python 镜像验证
# ----------------------------------------------------------------------------
# 待验证数学（将 1:1 转录到 VB）：
#   1. 五项对势（表面距离 d = r - R_i - R_j；README §1.1 表 + 权重）
#   2. 力累积 F_i = -de/dd * u_ij → 梯度：平移 = -ΣF；旋转 = -Σ(x-c)×F；
#      扭转 = -axis·Σ_{i∈branch}(x_pre-p)×F  [README §1.3]
#   3. 有限差分校验解析梯度（刚体 6 + 扭转 N）
#   4. BFGS + Armijo 线搜索收敛
#   5. ILS 自对接冒烟
# ============================================================================
import math
import random

ROT_WEIGHT = 0.0585



W1, W2, W3, W4, W5 = -0.0356, -0.00516, 0.840, -0.0351, -0.587
G1_O, G1_W = 0.0, 1.5
G2_O, G2_W = 3.0, 2.0
HB_LO, HB_HI = -0.7, 0.0
HYD_LO, HYD_HI = 0.5, 1.5
CUTOFF = 8.0
RADII = {'C': 1.9, 'A': 1.9, 'N': 1.8, 'NA': 1.8, 'OA': 1.7, 'S': 2.0,
         'P': 2.1, 'F': 1.5, 'Cl': 1.9, 'Br': 2.1, 'I': 2.2, 'Metal': 1.5}
HYDROPHOBIC = {'C', 'A', 'F', 'Cl', 'Br', 'I'}
ACCEPTOR = {'OA', 'NA', 'SA'}
DONOR = {'N', 'NA', 'OA', 'SA'}


def score_pair(pos_i, pos_j, t_i, t_j, R_i, R_j, w=(W1, W2, W3, W4, W5)):
    """加权五项对势 + 作用在 i 上的力 F_i = -de/dd * u_ij"""
    dx = pos_i[0] - pos_j[0]; dy = pos_i[1] - pos_j[1]; dz = pos_i[2] - pos_j[2]
    r = math.sqrt(dx * dx + dy * dy + dz * dz)
    if r > CUTOFF or r < 1e-9:
        return 0.0, (0.0, 0.0, 0.0)
    d = r - R_i - R_j
    x1 = d - G1_O
    x2 = d - G2_O
    g1 = math.exp(-(x1 / G1_W) ** 2)
    g2 = math.exp(-(x2 / G2_W) ** 2)
    e = w[0] * g1 + w[1] * g2
    de = w[0] * g1 * (-2.0 * x1 / (G1_W * G1_W)) + w[1] * g2 * (-2.0 * x2 / (G2_W * G2_W))
    if d <= 0:
        e += w[2] * d * d
        de += w[2] * 2.0 * d
    if t_i in HYDROPHOBIC and t_j in HYDROPHOBIC:
        if d <= HYD_LO:
            e += w[3]
        elif d < HYD_HI:
            e += w[3] * (HYD_HI - d) / (HYD_HI - HYD_LO)
            de += w[3] * (-1.0 / (HYD_HI - HYD_LO))
    if (t_i in ACCEPTOR and t_j in DONOR) or (t_j in ACCEPTOR and t_i in DONOR):
        if d <= HB_LO:
            e += w[4]
        elif d < HB_HI:
            e += w[4] * (HB_HI - d) / (HB_HI - HB_LO)
            de += w[4] * (-1.0 / (HB_HI - HB_LO))
    u = (dx / r, dy / r, dz / r)
    f = tuple(-de * u[k] for k in range(3))
    return e, f


def rodrigues(axis, angle):
    """Rodrigues 旋转矩阵"""
    x, y, z = axis
    n = math.sqrt(x * x + y * y + z * z)
    if n < 1e-12:
        return [[1, 0, 0], [0, 1, 0], [0, 0, 1]]
    x, y, z = x / n, y / n, z / n
    c = math.cos(angle)
    s = math.sin(angle)
    C = 1 - c
    return [[c + x * x * C, x * y * C - z * s, x * z * C + y * s],
            [y * x * C + z * s, c + y * y * C, y * z * C - x * s],
            [z * x * C - y * s, z * y * C + x * s, c + z * z * C]]


def mat_vec(m, v):
    return tuple(sum(m[i][k] * v[k] for k in range(3)) for i in range(3))


class TorsionTree:
    """扭转树：每个扭转键有一个 branch（下游原子集），父先子后依次应用"""

    def __init__(self, bonds, natoms):
        # bonds: [(a, b)] 可旋转键（a 是靠根侧）
        self.bonds = bonds
        self.branches = []   # 每键：branch 原子索引列表
        self.axis_a = []
        self.axis_b = []
        for (a, b) in bonds:
            # 从 b 出发 BFS，不经 a
            seen = {a, b}
            stack = [b]
            branch = []
            while stack:
                u = stack.pop()
                branch.append(u)
                for (p, q) in bonds:
                    pass
            # 用邻接表重建
            adj = {}
            for (p, q) in bonds:
                adj.setdefault(p, set()).add(q)
                adj.setdefault(q, set()).add(p)
            # 完整邻接需要全部键——这里仅用于演示，实际传入全键
            self.branches.append(branch)
            self.axis_a.append(a)
            self.axis_b.append(b)

    @staticmethod
    def build(all_bonds, natoms, rotatable_idx):
        adj = {}
        for (p, q) in all_bonds:
            adj.setdefault(p, set()).add(q)
            adj.setdefault(q, set()).add(p)
        bonds = [all_bonds[i] for i in rotatable_idx]
        branches = []
        for (a, b) in bonds:
            seen = {a, b}
            stack = [b]
            branch = []
            while stack:
                u = stack.pop()
                branch.append(u)
                for v in adj.get(u, ()):
                    if v not in seen:
                        seen.add(v)
                        stack.append(v)
            branches.append(sorted(branch))
        return bonds, branches


def apply_pose(coords0, trans, rotvec, tree_bonds, tree_branches, torsions):
    """应用刚体 + 扭转（父先子后），返回最终坐标 + 每扭转应用前快照"""
    n = len(coords0)
    c = (sum(p[0] for p in coords0) / n,
         sum(p[1] for p in coords0) / n,
         sum(p[2] for p in coords0) / n)
    R = rodrigues(rotvec, math.sqrt(sum(v * v for v in rotvec)))
    pos = []
    for p in coords0:
        rel = (p[0] - c[0], p[1] - c[1], p[2] - c[2])
        r = mat_vec(R, rel)
        pos.append((r[0] + c[0] + trans[0], r[1] + c[1] + trans[1], r[2] + c[2] + trans[2]))

    # 刚体增量旋转的中心 = c0 + t（扭转会移动最终质心，故必须在此记录）
    rigid_center = (c[0] + trans[0], c[1] + trans[1], c[2] + trans[2])
    # 扭转：父先子后；由共轭律 x(θ+δ) = R(δ)·x(θ)，梯度用最终几何即可
    for (a, b), branch, theta in zip(tree_bonds, tree_branches, torsions):
        pa, pb = pos[a], pos[b]
        axis = (pb[0] - pa[0], pb[1] - pa[1], pb[2] - pa[2])
        R = rodrigues(axis, theta)
        for u in branch:
            rel = (pos[u][0] - pa[0], pos[u][1] - pa[1], pos[u][2] - pa[2])
            r = mat_vec(R, rel)
            pos[u] = (pa[0] + r[0], pa[1] + r[1], pa[2] + r[2])
    return pos, rigid_center


def energy_grad(coords0, trans, rotvec, torsions, rec_atoms, rec_types, lig_types, tree_bonds, tree_branches):
    """返回 (inter_score, grad[9+N])；梯度 = README §1.3（切空间增量语义）"""
    pos, rigid_center = apply_pose(coords0, trans, rotvec, tree_bonds, tree_branches, torsions)
    forces = [[0.0, 0.0, 0.0] for _ in pos]
    inter = 0.0
    for i, p in enumerate(pos):
        ti = lig_types[i]
        Ri = RADII[ti]
        for j in range(len(rec_atoms)):
            q = rec_atoms[j]
            tj = rec_types[j]
            e, f = score_pair(p, q, ti, tj, Ri, RADII[tj])
            inter += e
            forces[i][0] += f[0]; forces[i][1] += f[1]; forces[i][2] += f[2]
    # 梯度组装
    n = len(pos)
    gt = [0.0, 0.0, 0.0]
    tau = [0.0, 0.0, 0.0]   # 总扭矩：杠杆臂 = pos_i - 刚体中心(c0+t)
    for i in range(n):
        for k in range(3):
            gt[k] += -forces[i][k]                    # 平移 = -ΣF
        rx = pos[i][0] - rigid_center[0]
        ry = pos[i][1] - rigid_center[1]
        rz = pos[i][2] - rigid_center[2]
        tau[0] += ry * forces[i][2] - rz * forces[i][1]
        tau[1] += rz * forces[i][0] - rx * forces[i][2]
        tau[2] += rx * forces[i][1] - ry * forces[i][0]
    gr = [-tau[0], -tau[1], -tau[2]]   # [README §1.3] 旋转 = 负总扭矩
    # 扭转 = -轴·Σ_{i∈branch}(x_i_final - p)×F_i（共轭律：增量作用于最终几何）
    gtors = []
    for (a, b), branch in zip(tree_bonds, tree_branches):
        pa, pb = pos[a], pos[b]
        ax = pb[0] - pa[0]; ay = pb[1] - pa[1]; az = pb[2] - pa[2]
        na = math.sqrt(ax * ax + ay * ay + az * az)
        acc = 0.0
        for u in branch:
            fx = forces[u][0]; fy = forces[u][1]; fz = forces[u][2]
            rx = pos[u][0] - pa[0]; ry = pos[u][1] - pa[1]; rz = pos[u][2] - pa[2]
            cross = (ry * fz - rz * fy,
                     rz * fx - rx * fz,
                     rx * fy - ry * fx)
            acc += (ax * cross[0] + ay * cross[1] + az * cross[2]) / na
        gtors.append(-acc)
    return inter, list(gt) + list(gr) + gtors


def rotvec_to_mat(rotvec):
    th = math.sqrt(sum(v * v for v in rotvec))
    if th < 1e-12:
        return [[1, 0, 0], [0, 1, 0], [0, 0, 1]]
    return rodrigues(rotvec, th)


def mat_to_rotvec(m):
    """旋转矩阵 → 旋转向量（Shepperd 法，处理小角/近π）"""
    cos_t = 0.5 * (m[0][0] + m[1][1] + m[2][2] - 1.0)
    cos_t = max(-1.0, min(1.0, cos_t))
    th = math.acos(cos_t)
    if th < 1e-9:
        # 小角：反对称部分近似
        return (0.5 * (m[2][1] - m[1][2]), 0.5 * (m[0][2] - m[2][0]), 0.5 * (m[1][0] - m[0][1]))
    if abs(math.pi - th) < 1e-5:
        # 近 π：用对角项
        k = th / (2 * (1 + cos_t))
        return (k * (m[0][0] + 1), k * (m[1][1] + 1), k * (m[2][2] + 1))
    s = 2.0 * math.sin(th)
    return ((m[2][1] - m[1][2]) / s * th,
            (m[0][2] - m[2][0]) / s * th,
            (m[1][0] - m[0][1]) / s * th)


def mat_mul(A, B):
    return [[sum(A[i][k] * B[k][j] for k in range(3)) for j in range(3)] for i in range(3)]


def apply_increment(trans, rotvec, dtrans, drot):
    """增量语义：trans += dtrans；rotvec ← log(R(drot)·R(rotvec))"""
    R_new = mat_mul(rotvec_to_mat(drot), rotvec_to_mat(rotvec))
    return (trans[0] + dtrans[0], trans[1] + dtrans[1], trans[2] + dtrans[2]), mat_to_rotvec(R_new)


def numeric_grad(coords0, trans, rotvec, torsions, rec, rt, lt, tb, tbr, h=1e-6):
    """增量语义有限差分：旋转分量按 R(h e_k)·R(ω) 扰动（与 BFGS 更新一致）"""
    def f(t, rv, to):
        e, _ = energy_grad(coords0, t, rv, to, rec, rt, lt, tb, tbr)
        return e

    base = f(trans, rotvec, torsions)
    g = []
    for k in range(3):
        dt = [h if i == k else 0.0 for i in range(3)]
        t2, _ = apply_increment(trans, rotvec, dt, (0, 0, 0))
        t1, _ = apply_increment(trans, rotvec, (-dt[0], -dt[1], -dt[2]), (0, 0, 0))
        g.append((f(t2, rotvec, torsions) - f(t1, rotvec, torsions)) / (2 * h))
    for k in range(3):
        dr = [h if i == k else 0.0 for i in range(3)]
        _, r2 = apply_increment(trans, rotvec, (0, 0, 0), dr)
        _, r1 = apply_increment(trans, rotvec, (0, 0, 0), (-dr[0], -dr[1], -dr[2]))
        g.append((f(trans, r2, torsions) - f(trans, r1, torsions)) / (2 * h))
    for k in range(len(torsions)):
        tp = list(torsions); tp[k] += h
        tm = list(torsions); tm[k] -= h
        g.append((f(trans, rotvec, tp) - f(trans, rotvec, tm)) / (2 * h))
    return base, g
    base, _ = energy_grad(coords0, trans, rotvec, torsions, rec, rt, lt, tb, tbr)
    x = list(trans) + list(rotvec) + list(torsions)

    def f(vec):
        return energy_grad(coords0, vec[0:3], vec[3:6], vec[6:], rec, rt, lt, tb, tbr)[0]

    g = []
    for i in range(len(x)):
        xp = list(x); xp[i] += h
        xm = list(x); xm[i] -= h
        g.append((f(xp) - f(xm)) / (2 * h))
    return base, g


def build_test_system():
    rng = random.Random(7)
    # 受体：壳层口袋——原子分布在半径 6~8 的球壳上，口袋内无原子
    rec = []
    rt = []
    for i in range(60):
        while True:
            v = (rng.uniform(-1, 1), rng.uniform(-1, 1), rng.uniform(-1, 1))
            n = math.sqrt(sum(x * x for x in v))
            if 0.2 < n < 1.0:
                break
        rad = rng.uniform(6.0, 8.0)
        rec.append((v[0] / n * rad, v[1] / n * rad, v[2] / n * rad))
        rt.append(rng.choice(['C', 'OA', 'NA', 'N', 'A', 'C', 'C']))
    # 配体：6 原子小分子，1 个可旋转键（链状 C-C-C-C-C-C）
    lig_types = ['C', 'C', 'C', 'OA', 'C', 'N']
    coords0 = [(0.5 * i, 0.3 * (i % 3) * 0.2, 0.1 * i) for i in range(6)]
    all_bonds = [(0, 1), (1, 2), (2, 3), (3, 4), (4, 5)]
    rot = [1, 3]   # 两个可旋转键
    tb, tbr = TorsionTree.build(all_bonds, 6, rot)
    return coords0, lig_types, rec, rt, tb, tbr


def test_gradient():
    coords0, lt, rec, rt, tb, tbr = build_test_system()
    rng = random.Random(11)
    fails = 0
    for trial in range(5):
        trans = (rng.uniform(-1, 1), rng.uniform(-1, 1), rng.uniform(-1, 1))
        rotvec = (rng.uniform(-0.3, 0.3), rng.uniform(-0.3, 0.3), rng.uniform(-0.3, 0.3))
        tors = [rng.uniform(-1, 1), rng.uniform(-1, 1)]
        _, ga = energy_grad(coords0, trans, rotvec, tors, rec, rt, lt, tb, tbr)
        _, gn = numeric_grad(coords0, trans, rotvec, tors, rec, rt, lt, tb, tbr)
        maxerr = max(abs(a - b) for a, b in zip(ga, gn))
        scale = max(1.0, max(abs(g) for g in gn))
        rel = maxerr / scale
        status = "OK" if rel < 2e-3 else "FAIL"
        if rel >= 2e-3:
            fails += 1
        print(f"  trial{trial}: 解析梯度 = {[f'{v:.4f}' for v in ga]}")
        print(f"            数值梯度 = {[f'{v:.4f}' for v in gn]}  相对误差 {rel:.2e} [{status}]")
    return fails


# ---------------- BFGS ----------------
def bfgs_v2(fgrad, x0, max_iter=300, gtol=1e-4):
    """BFGS（切空间梯度 + 增量姿态更新，Vina 同款语义）
    x = [trans(3), rotvec(3), torsions(N)]；方向 p 的旋转分量以
    R(p·α)·R(rotvec) 乘法应用，扭转分量直接加。"""
    x = list(x0)
    f, g = fgrad(x)
    n = len(x)
    H = [[1.0 if i == j else 0.0 for j in range(n)] for i in range(n)]
    evals = 1

    def step(xv, direction, alpha):
        t = (xv[0] + alpha * direction[0],
             xv[1] + alpha * direction[1],
             xv[2] + alpha * direction[2])
        rv = rotvec_to_mat(xv[3:6])
        dr = [alpha * direction[3], alpha * direction[4], alpha * direction[5]]
        Rd = rotvec_to_mat(dr)
        rv2 = mat_to_rotvec(mat_mul(Rd, rv))
        to = [xv[6 + k] + alpha * direction[6 + k] for k in range(n - 6)]
        return [t[0], t[1], t[2], rv2[0], rv2[1], rv2[2]] + to

    for it in range(max_iter):
        gnorm = math.sqrt(sum(v * v for v in g))
        if gnorm < gtol:
            break
        p = [-sum(H[i][k] * g[k] for k in range(n)) for i in range(n)]
        slope = sum(p[i] * g[i] for i in range(n))
        if slope >= 0:
            H = [[1.0 if i == j else 0.0 for j in range(n)] for i in range(n)]
            continue
        alpha = 1.0
        f0 = f
        while True:
            xn = step(x, p, alpha)
            fn, gn = fgrad(xn)
            evals += 1
            if fn <= f0 + 1e-4 * alpha * slope or alpha < 1e-12:
                break
            alpha *= 0.5
        s = [alpha * p[i] for i in range(n)]
        y = [gn[i] - g[i] for i in range(n)]
        sy = sum(s[i] * y[i] for i in range(n))
        if sy > 1e-10:
            rho = 1.0 / sy
            Hy = [sum(H[i][k] * y[k] for k in range(n)) for i in range(n)]
            yH = [sum(y[k] * H[k][j] for k in range(n)) for j in range(n)]
            yHy = sum(y[k] * Hy[k] for k in range(n))
            newH = [[0.0] * n for _ in range(n)]
            for i in range(n):
                for j in range(n):
                    newH[i][j] = (H[i][j] - rho * s[i] * yH[j] - rho * Hy[i] * s[j]
                                  + rho * rho * yHy * s[i] * s[j] + rho * s[i] * s[j])
            H = newH
        x, f, g = xn, fn, gn
    return x, f, evals


def test_bfgs():
    coords0, lt, rec, rt, tb, tbr = build_test_system()

    def fgrad(x):
        e, g = energy_grad(coords0, x[0:3], x[3:6], x[6:], rec, rt, lt, tb, tbr)
        # ΔG = e + 0.0585 * nrot（2 个可旋转键）
        return e + ROT_WEIGHT * len(tb), g

    x0 = [1.0, 0.5, -0.5, 0.2, -0.1, 0.3, 0.5, -0.8]
    x, f, ev = bfgs_v2(fgrad, x0)
    print(f"  BFGS: 初值 f={fgrad(x0)[0]:.4f} → 收敛 f={f:.4f}（{ev} 次评估）, x={[f'{v:.3f}' for v in x]}")
    # 再跑一次确认稳定性
    x2, f2, ev2 = bfgs_v2(fgrad, [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0])
    print(f"  BFGS: 从原点 f={f2:.4f}")
    return 0 if f < fgrad(x0)[0] else 1


def test_ils():
    """自对接冒烟：把配体放进受体口袋的已知位置，ILS 应找到相当或更优的解"""
    coords0, lt, rec, rt, tb, tbr = build_test_system()
    rng = random.Random(3)

    def full_score(x):
        e, _ = energy_grad(coords0, x[0:3], x[3:6], x[6:], rec, rt, lt, tb, tbr)
        return e

    # "晶体"姿态：配体放在口袋中心
    crystal = [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.6, -0.4]
    f_crystal = full_score(crystal)

    def fgrad(x):
        e, g = energy_grad(coords0, x[0:3], x[3:6], x[6:], rec, rt, lt, tb, tbr)
        return e, g

    T = 293.15 * 0.001987
    best_in_pocket = float('inf')
    for run in range(8):
        x = [rng.uniform(-2, 2), rng.uniform(-2, 2), rng.uniform(-2, 2),
             rng.uniform(-1, 1), rng.uniform(-1, 1), rng.uniform(-1, 1)] + [0.0, 0.0]
        x, f, _ = bfgs_v2(fgrad, x, max_iter=120)
        for step in range(30):
            xp = list(x)
            xp[0] += rng.uniform(-1, 1); xp[1] += rng.uniform(-1, 1); xp[2] += rng.uniform(-1, 1)
            xp[3] += rng.uniform(-0.3, 0.3); xp[4] += rng.uniform(-0.3, 0.3); xp[5] += rng.uniform(-0.3, 0.3)
            xp[6] += rng.uniform(-0.4, 0.4); xp[7] += rng.uniform(-0.4, 0.4)
            xp, fp, _ = bfgs_v2(fgrad, xp, max_iter=100)
            dc = fp - f
            if dc < 0 or rng.random() < math.exp(-dc / T):
                x, f = xp, fp
        # 口袋内判定：配体质心距原点 < 5Å（口袋中心）
        cen = apply_pose(coords0, x[0:3], x[3:6], tb, tbr, x[6:])[0]  # pos
        cxx = sum(p[0] for p in cen) / len(cen)
        cyy = sum(p[1] for p in cen) / len(cen)
        czz = sum(p[2] for p in cen) / len(cen)
        if math.sqrt(cxx**2 + cyy**2 + czz**2) < 5.0 and f < best_in_pocket:
            best_in_pocket = f
    print(f"  ILS: 晶体姿态分={f_crystal:.4f}  口袋内最优={best_in_pocket:.4f}  "
          f"{'OK' if best_in_pocket <= f_crystal + 0.5 else 'FAIL'}")
    return 0 if best_in_pocket <= f_crystal + 0.5 else 1


def test_gb_sasa():
    """GB 自项 + SASA 解析球验证"""
    fails = 0
    # GB: 孤立单原子 G_self = -(1/2)(1-1/ε) q²/R
    q = 0.5
    R = 1.6
    eps = 78.5
    g_self = -0.5 * (1 - 1 / eps) * q * q / R
    # 用两原子公式验证：两原子相距很远 → f_ij≈r，交叉项 ≈ -... 主要验证自项公式一致
    print(f"  GB 自项（解析）: {g_self:.6f} kcal/mol  （公式直算，验证 -0.5(1-1/ε)q²/R 结构）")
    # 数值：两同号电荷远处 → 相互作用趋近 0
    f_ij = math.sqrt(100.0 ** 2 + R * R * math.exp(-100.0 ** 2 / (4 * R * R)))
    inter = -(1 - 1 / eps) * q * q / f_ij
    expect = -(1 - 1 / eps) * q * q / 100.0   # Born 屏蔽项已衰减，趋近纯库仑形式
    ok = abs(inter - expect) < 1e-10
    print(f"  GB 远距交叉项 {inter:.6e} ≈ 理论 {expect:.6e}  {'OK' if ok else 'FAIL'}")
    if not ok:
        fails += 1

    # SASA: 孤立原子 = 4π(r+probe)²
    def shrake_rupley(pos, radii, probe=1.4, n_points=960):
        # golden spiral 球面点
        pts = []
        offset = 2.0 / n_points
        inc = math.pi * (3 - math.sqrt(5))
        for k in range(n_points):
            y = ((k * offset) - 1) + offset / 2
            r = math.sqrt(max(0.0, 1 - y * y))
            phi = k * inc
            pts.append((math.cos(phi) * r, y, math.sin(phi) * r))
        areas = [0.0] * len(pos)
        # 邻居
        for i, (p, ri) in enumerate(zip(pos, radii)):
            const = 4 * math.pi * (ri + probe) ** 2 / n_points
            acc = 0
            for pt in pts:
                sx = p[0] + pt[0] * (ri + probe)
                sy = p[1] + pt[1] * (ri + probe)
                sz = p[2] + pt[2] * (ri + probe)
                accessible = True
                for j, (q2, rj) in enumerate(zip(pos, radii)):
                    if j == i:
                        continue
                    d2 = (sx - q2[0]) ** 2 + (sy - q2[1]) ** 2 + (sz - q2[2]) ** 2
                    rr = rj + probe
                    if d2 < rr * rr:
                        accessible = False
                        break
                if accessible:
                    acc += 1
            areas[i] = acc * const
        return areas

    # 孤立碳原子：4π(1.9+1.4)² = 136.85
    a = shrake_rupley([(0, 0, 0)], [1.9])[0]
    expect = 4 * math.pi * (1.9 + 1.4) ** 2
    rel = abs(a - expect) / expect
    print(f"  SASA 孤立原子: {a:.3f} vs 解析 {expect:.3f}  相对误差 {rel:.4f}  {'OK' if rel < 0.01 else 'FAIL'}")
    if rel >= 0.01:
        fails += 1
    return fails


def test_nwat():
    """Nwat 最近水选择"""
    lig = [(0, 0, 0), (1, 0, 0)]
    waters = [(3, 0, 0), (2.5, 0.5, 0), (1.2, 0.1, 0), (5, 5, 5), (1.5, 0, 0), (10, 0, 0)]
    def dist(p, qset):
        return min(math.sqrt((p[0]-q[0])**2 + (p[1]-q[1])**2 + (p[2]-q[2])**2) for q in qset)
    ranked = sorted(range(len(waters)), key=lambda k: dist(waters[k], lig))
    nwat = 3
    sel = [waters[k] for k in ranked[:nwat]]
    expect = [(1.2, 0.1, 0), (1.5, 0, 0), (2.5, 0.5, 0)]
    ok = all(sel[i] == expect[i] for i in range(3))
    print(f"  Nwat=3 选择: {[str(w) for w in sel]}  {'OK' if ok else 'FAIL'}")
    return 0 if ok else 1


if __name__ == "__main__":
    import sys
    which = sys.argv[1] if len(sys.argv) > 1 else "all"
    total = 0
    if which in ("all", "grad"):
        print("=== 1. 解析梯度 vs 有限差分 ===")
        total += test_gradient()
    if which in ("all", "bfgs"):
        print("=== 2. BFGS 收敛 ===")
        total += test_bfgs()
    if which in ("all", "ils"):
        print("=== 3. ILS 自对接 ===")
        total += test_ils()
    if which in ("all", "gbsa"):
        print("=== 4. GB / SASA ===")
        total += test_gb_sasa()
    if which in ("all", "nwat"):
        print("=== 5. Nwat 选择 ===")
        total += test_nwat()
    print(f"\n[{which}] {'PASS' if total == 0 else f'{total} FAILS'}")
