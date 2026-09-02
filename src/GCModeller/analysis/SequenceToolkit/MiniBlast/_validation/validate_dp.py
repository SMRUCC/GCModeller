# ============================================================================
# validate_dp.py v2 — X-drop 延伸算法验证（修正版）
#   1. 无 gap 延伸：双侧合并语义，参照 = 包含种子的对角线最大段（暴力）
#   2. gapped DP：每格记录 H/E/F 三个状态各自的回溯方向，状态机回溯
#      - traceback 重算得分必须 == DP 报告得分
#      - 双端合并 vs 全局 Smith-Waterman
# ============================================================================
import random

B62 = {}
rows = """A  R  N  D  C  Q  E  G  H  I  L  K  M  F  P  S  T  W  Y  V
A  4 -1 -2 -2  0 -1 -1  0 -2 -1 -1 -1 -1 -2 -1  1  0 -3 -2  0
R -1  5  0 -2 -3  1  0 -2  0 -3 -2  2 -1 -3 -2 -1 -1 -3 -2 -3
N -2  0  6  1 -3  0  0  0  1 -3 -3  0 -2 -3 -2  1  0 -4 -2 -3
D -2 -2  1  6 -3  0  2 -1 -1 -3 -4 -1 -3 -3 -1  0 -1 -4 -3 -3
C  0 -3 -3 -3  9 -3 -4 -3 -3 -1 -1 -3 -1 -2 -3 -1 -1 -2 -2 -1
Q -1  1  0  0 -3  5  2 -2  0 -3 -2  1  0 -3 -1  0 -1 -2 -1 -2
E -1  0  0  2 -4  2  5 -2  0 -3 -3  1 -2 -3 -1  0 -1 -3 -2 -2
G  0 -2  0 -1 -3 -2 -2  6 -2 -4 -4 -2 -3 -3 -2  0 -2 -2 -3 -3
H -2  0  1 -1 -3  0  0 -2  8 -3 -3 -1 -2 -1 -2 -1 -2 -2  2 -3
I -1 -3 -3 -3 -1 -3 -3 -4 -3  4  2 -3  1  0 -3 -2 -1 -3 -1  3
L -1 -2 -3 -4 -1 -2 -3 -4 -3  2  4 -2  2  0 -3 -2 -1 -2 -1  1
K -1  2  0 -1 -3  1  1 -2 -1 -3 -2  5 -1 -3 -1  0 -1 -3 -2 -2
M -1 -1 -2 -3 -1  0 -2 -3 -2  1  2 -1  5  0 -2 -1 -1 -1 -1  1
F -2 -3 -3 -3 -2 -3 -3 -3 -1  0  0 -3  0  6 -4 -2 -2  1  3 -1
P -1 -2 -2 -1 -3 -1 -1 -2 -2 -3 -3 -1 -2 -4  7 -1 -1 -4 -3 -2
S  1 -1  1  0 -1  0  0  0 -1 -2 -2  0 -1 -2 -1  4  1 -3 -2 -2
T  0 -1  0 -1 -1 -1 -1 -2 -2 -1 -1 -1 -1 -2 -1  1  5 -2 -2  0
W -3 -3 -4 -4 -2 -2 -3 -2 -2 -3 -2 -3 -1  1 -4 -3 -2 11  2 -3
Y -2 -2 -2 -3 -2 -1 -2 -3  2 -1 -1 -2 -1  3 -3 -2 -2  2  7 -1
V  0 -3 -3 -3 -1 -2 -2 -3 -3  3  1 -2  1 -1 -2 -2  0 -3 -1  4"""
lines = rows.strip().splitlines()
hdr = lines[0].split()
for line in lines[1:]:
    p = line.split()
    for b, v in zip(hdr, p[1:]):
        B62[(p[0], b)] = int(v)
AA = "ARNDCQEGHILKMFPSTWYV"


def ungapped_extend(q, s, ic, jc, sub, xdrop):
    """无 gap X-drop 双侧延伸：最优段必含种子。
    返回 (best, bi, bj, ia, ja, ib, jb)  # best 段端点与左右延伸界"""
    n, m = len(q), len(s)
    seed = sub(q[ic], s[jc])
    # 左侧：score(p..ic-1)，Lbest = max(0, max_p)，带 X-drop 终止
    lbest = 0.0; la = ic  # 左侧最优段起点（不含种子时 la=ic）
    sc = 0.0; run_i, run_j = ic - 1, jc - 1
    i, j = ic - 1, jc - 1
    while i >= 0 and j >= 0:
        sc += sub(q[i], s[j])
        if sc > lbest: lbest, la = sc, i
        if lbest - sc > xdrop: break
        i -= 1; j -= 1
    # 右侧
    rbest = 0.0; rb = ic  # 右侧最优段终点
    sc = 0.0
    i, j = ic + 1, jc + 1
    while i < n and j < m:
        sc += sub(q[i], s[j])
        if sc > rbest: rbest, rb = sc, i
        if rbest - sc > xdrop: break
        i += 1; j += 1
    best = seed + lbest + rbest
    return best, la, rb, la, max(0, jc - (ic - la)), ic, jc


def diag_max_containing(q, s, ic, jc, sub):
    """暴力参照：包含种子的对角线最大段得分"""
    n, m = len(q), len(s)
    # 收集对角线（含种子）
    i, j = ic, jc
    while i > 0 and j > 0: i -= 1; j -= 1
    diag = []
    while i < n and j < m:
        diag.append(sub(q[i], s[j]))
        if i == ic: seed_idx = len(diag) - 1
        i += 1; j += 1
    # max segment [l..r] with l <= seed_idx <= r
    best = -10**9
    for l in range(seed_idx + 1):
        acc = 0
        for r in range(l, len(diag)):
            acc += diag[r]
            if l <= seed_idx <= r and acc > best: best = acc
    return best


def gapped_fwd(q, s, si0, sj0, h0, sub, go, ge, xdrop, collect=False):
    """前向 X-drop 仿射 DP。种子格 (si0,sj0) 得分 h0。
    每格存 H/E/F 三状态回溯方向。返回 (best, bu, bv, moves)"""
    n, m = len(q), len(s)
    umax = n - 1 - si0; vmax = m - 1 - sj0
    NEG = -10**9
    go_open = go + ge      # NCBI: 长度 k 的 gap = go + k*ge ⇒ 首个残基扣 go+ge
    best = h0; bu = bv = 0
    prev2H = {}    # t-2: u -> H
    prev1 = {}     # t-1: u -> (H, E, F)
    cur = {}
    traces = {0: {0: (0, -1, -1, -1)}} if collect else None
    prev1[0] = (h0, NEG, NEG)
    t = 1; alive = True; cells = 0
    while alive and t <= umax + vmax + 1:
        alive = False
        cur = {}
        tdir = {} if collect else None
        lo_u = max(0, t - vmax); hi_u = min(umax, t)
        cutoff = best - xdrop
        for u in range(lo_u, hi_u + 1):
            v = t - u
            # E(u,v)：消耗 query（subject 侧 gap），来自 (u-1,v) 的 H 或 E
            e = NEG; e_dir = -1
            if u >= 1:
                p = prev1.get(u - 1)
                if p is not None:
                    hp, ep, _ = p
                    if hp > NEG // 2 and hp - go_open > e: e = hp - go_open; e_dir = 0
                    if ep > NEG // 2 and ep - ge > e: e = ep - ge; e_dir = 1
            # F(u,v)：消耗 subject（query 侧 gap），来自 (u,v-1) 的 H 或 F
            f = NEG; f_dir = -1
            p = prev1.get(u)
            if p is not None:
                hp, _, fp = p
                if hp > NEG // 2 and hp - go_open > f: f = hp - go_open; f_dir = 0
                if fp > NEG // 2 and fp - ge > f: f = fp - ge; f_dir = 2
            # diag
            d = NEG
            ph = prev2H.get(u - 1)
            if ph is not None:
                d = ph + sub(q[si0 + u], s[sj0 + v])
            h = max(d, e, f)
            h_dir = 0 if h == d else (1 if h == e else 2)
            if h > best: best = h; bu = u; bv = v
            # 状态存活性
            h_alive = h >= cutoff
            e_alive = e >= cutoff
            f_alive = f >= cutoff
            if not (h_alive or e_alive or f_alive):
                continue
            alive = True; cells += 1
            if cells > 4_000_000: alive = False; break
            cur[u] = (h if h_alive else NEG,
                      e if e_alive else NEG,
                      f if f_alive else NEG)
            if collect:
                tdir[u] = (h_dir if h_alive else -1,
                           e_dir if e_alive else -1,
                           f_dir if f_alive else -1)
        if collect: traces[t] = tdir
        prev2H = {u: p[0] for u, p in prev1.items() if p[0] > NEG // 2}
        prev1 = cur
        t += 1
    moves = []
    if collect:
        u, v, st = bu, bv, 0
        tt = bu + bv
        while tt > 0 or (u, v) != (0, 0) or st != 0:
            ent = traces.get(tt, {}).get(u)
            if ent is None: break
            dh, de, df = ent
            if st == 0:
                if dh == 0:
                    moves.append('d'); u -= 1; v -= 1; tt -= 2; st = 0
                elif dh == 1:
                    # H 经 E 到达：同格 dirE 决定前驱状态（gap-open→H / 延续→E）
                    moves.append('e'); u -= 1; tt -= 1
                    if de == 0: st = 0
                    elif de == 1: st = 1
                    else: break
                elif dh == 2:
                    moves.append('f'); v -= 1; tt -= 1
                    if df == 0: st = 0
                    elif df == 2: st = 2
                    else: break
                else: break
            elif st == 1:
                moves.append('e'); u -= 1; tt -= 1
                if de == 0: st = 0
                elif de == 1: st = 1
                else: break
            else:
                moves.append('f'); v -= 1; tt -= 1
                if df == 0: st = 0
                elif df == 2: st = 2
                else: break
            if tt < 0: break
        moves.reverse()
    return best, bu, bv, moves


def recompute(qa, sa, sub, go, ge):
    """从比对字符串重算得分。

    NCBI 仿射 gap 约定：长度 k 的 gap 代价 = go + k*ge
    （首个 gap 残基扣 go+ge，其后每个残基扣 ge）。与 SeedExtend.vb 的 DP 一致。
    """
    score = 0; inq = ins = 0
    for a, b in zip(qa, sa):
        if a != '-' and b != '-':
            if inq: score -= go + inq*ge; inq = 0
            if ins: score -= go + ins*ge; ins = 0
            score += sub(a, b)
        elif a != '-':
            if ins: score -= go + ins*ge; ins = 0
            inq += 1
        else:
            if inq: score -= go + inq*ge; inq = 0
            ins += 1
    if inq: score -= go + inq*ge
    if ins: score -= go + ins*ge
    return score


def sw_full(q, s, sub, go, ge):
    n, m = len(q), len(s)
    H = [[0]*(m+1) for _ in range(n+1)]
    E = [[0]*(m+1) for _ in range(n+1)]
    F = [[0]*(m+1) for _ in range(n+1)]
    best = 0
    for i in range(1, n+1):
        for j in range(1, m+1):
            E[i][j] = max(H[i-1][j]-go, E[i-1][j]-ge)
            F[i][j] = max(H[i][j-1]-go, F[i][j-1]-ge)
            H[i][j] = max(0, H[i-1][j-1]+sub(q[i-1], s[j-1]), E[i][j], F[i][j])
            if H[i][j] > best: best = H[i][j]
    return best


def run():
    rng = random.Random(7)
    sub = lambda a, b: B62[(a, b)]
    GO, GE = 11, 1
    fails = 0

    # 测试1: 前向 traceback 重算一致性
    for trial in range(60):
        q = [rng.choice(AA) for _ in range(150)]
        s = [rng.choice(AA) for _ in range(150)]
        for k in range(60):   # 嵌入同源
            if rng.random() >= 0.15: s[60+k] = q[30+k]
        ic, jc = 45, 90
        h0 = sub(q[ic], s[jc])
        xdrop = int(40 * 0.6931 / 0.3176)
        fb, fu, fv, fm = gapped_fwd(q, s, ic, jc, h0, sub, GO, GE, xdrop, collect=True)
        qa = [q[ic]]; sa_ = [s[jc]]
        i, j = ic, jc
        for mv in fm:
            if mv == 'd': i += 1; j += 1; qa.append(q[i]); sa_.append(s[j])
            elif mv == 'e': i += 1; qa.append(q[i]); sa_.append('-')
            else: j += 1; qa.append('-'); sa_.append(s[j])
        rec = recompute(''.join(qa), ''.join(sa_), sub, GO, GE)
        if rec != fb:
            print(f"  [FAIL t{trial}] traceback重算 {rec} != DP {fb}")
            fails += 1
    print(f"测试1(前向traceback重算==DP): {'PASS' if fails == 0 else fails}")

    # 测试2: 双端合并 vs SW（同源区含种子且为显著信号）
    f2 = 0
    for trial in range(60):
        hom = [rng.choice(AA) for _ in range(80)]
        for k in range(80):
            if rng.random() < 0.08: hom[k] = rng.choice(AA)
        q = [rng.choice(AA) for _ in range(20)] + hom + [rng.choice(AA) for _ in range(20)]
        s = [rng.choice(AA) for _ in range(35)] + hom + [rng.choice(AA) for _ in range(25)]
        ic, jc = 20 + 40, 35 + 40
        h0 = sub(q[ic], s[jc])
        xdrop = int(40 * 0.6931 / 0.3176)
        fb, _, _, _ = gapped_fwd(q, s, ic, jc, h0, sub, GO, GE, xdrop, collect=False)
        rq = q[:ic+1][::-1]; rs = s[:jc+1][::-1]
        bb, _, _, _ = gapped_fwd(rq, rs, 0, 0, h0, sub, GO, GE, xdrop, collect=False)
        total = fb + bb - h0
        sw = sw_full(q, s, sub, GO, GE)
        if abs(total - sw) > 2:
            print(f"  [FAIL t{trial}] combined={total} sw={sw}")
            f2 += 1
    print(f"测试2(双端合并==SW): {'PASS' if f2 == 0 else f2}")

    # 测试3: 无 gap 延伸 vs 暴力（无限 X-drop，双侧含种子）
    f3 = 0
    sub_nt = lambda a, b: 2 if a == b else -3
    for trial in range(200):
        q = [rng.choice("ACGT") for _ in range(80)]
        s = [rng.choice("ACGT") for _ in range(80)]
        ic, jc = rng.randint(10, 60), rng.randint(10, 60)
        brute = diag_max_containing(q, s, ic, jc, sub_nt)
        ug, _, _, _, _, _, _ = ungapped_extend(q, s, ic, jc, sub_nt, 10**9)
        if ug != brute:
            print(f"  [FAIL t{trial}] ungapped={ug} brute={brute}")
            f3 += 1
    print(f"测试3(无gap双侧==暴力): {'PASS' if f3 == 0 else f3}")

    # 测试4: 带X-drop的无gap延伸 >= 无限制版本 - 0（drop 只会变小）且单调性
    f4 = 0
    for trial in range(200):
        q = [rng.choice(AA) for _ in range(60)]
        s = [rng.choice(AA) for _ in range(60)]
        ic, jc = 30, 30
        u_inf = ungapped_extend(q, s, ic, jc, sub, 10**9)[0]
        u_20 = ungapped_extend(q, s, ic, jc, sub, 22)[0]
        if u_20 > u_inf + 1e-9:
            print(f"  [FAIL t{trial}] X-drop 版本更大 {u_20} > {u_inf}")
            f4 += 1
    print(f"测试4(X-drop单调性): {'PASS' if f4 == 0 else f4}")

    return fails + f2 + f3 + f4


if __name__ == "__main__":
    f = run()
    print(f"\n总计: {'ALL PASS' if f == 0 else f'{f} FAILS'}")
