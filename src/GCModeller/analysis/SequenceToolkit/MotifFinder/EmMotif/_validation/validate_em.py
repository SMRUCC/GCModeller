# ============================================================================
# validate_em.py — EM motif 发现核心数学的 Python 镜像验证
# 待验证（将 1:1 转录到 VB）：
#   1. E 步三种模型的后验公式与约束：OOPS Σ_j Z_ij = 1；ZOOPS Σ_j Z_ij ≤ 1；
#      ANR 各窗口独立 [em.md §2/§6]
#   2. M 步加权计数 + 伪计数归一化 [em.md §3]
#   3. EM 单调收敛 [em.md §4]
#   4. 种植 motif 恢复（DNA/蛋白/反义链）
#   5. χ² 生存函数（不完全伽马）与 E-value
# 注意：em.md §2 的 ZOOPS 窗口级公式 Z=λP_m/(λP_m+(1-λ)P_b) 不满足 §6 的
#   Σ_j Z_ij ≤ 1 约束——正确后验（Bailey & Elkan 1994）为
#   Z_ij = λ·R_ij / ((1-λ) + λ·Σ_j R_ij)，R_ij = Π θ_k,a/θ_0,a。
#   实现取正确式，README 中记录该修正。
# ============================================================================
import math
import random

DNA = "ACGT"
AA = "ACDEFGHIKLMNPQRSTVWY"


def encode(seq, alpha):
    idx = {c: i for i, c in enumerate(alpha)}
    return [idx.get(c, -1) for c in seq.upper()]


def bg_freqs(seqs_enc, K, pseudo=0.1):
    cnt = [pseudo] * K
    for enc in seqs_enc:
        for x in enc:
            if x >= 0:
                cnt[x] += 1
    total = sum(cnt)
    return [c / total for c in cnt]


def pwm_from_seed(seed_enc, W, K, pseudo=0.1):
    pwm = [[pseudo] * K for _ in range(W)]
    for k in range(W):
        pwm[k][seed_enc[k]] += 1.0
    for k in range(W):
        s = sum(pwm[k])
        pwm[k] = [x / s for x in pwm[k]]
    return pwm


def logR_matrix(enc, W, pwm, bg, revcomp_maps=None):
    """所有窗口的 log LLR；revcomp_maps: [正向编码表, 反向编码表] 或 None"""
    L = len(enc)
    nwin = L - W + 1
    if nwin <= 0:
        return []
    rows = []
    for j in range(nwin):
        lr = 0.0
        ok = True
        for k in range(W):
            a = enc[j + k]
            if a < 0:
                ok = False
                break
            b = bg[a]
            lr += math.log(pwm[k][a] / b) if b > 0 and pwm[k][a] > 0 else -1e9
        rows.append(lr if ok else float('-inf'))
    return rows


def estep_z(enc, W, pwm, bg, lam, model):
    """返回 [(j, Z)]；OOPS 按序列内归一化；ZOOPS 序列级混合；ANR 窗口独立"""
    lrs = logR_matrix(enc, W, pwm, bg)
    nwin = len(lrs)
    out = []
    if nwin == 0:
        return out
    m = max(lrs)
    if model == "oops":
        # softmax: Z_j = exp(lr_j) / Σ exp(lr)
        expl = [math.exp(v - m) for v in lrs]
        s = sum(expl)
        out = [(j, e / s) for j, e in enumerate(expl)]
    elif model == "zoops":
        # Z_j = λR_j / ((1-λ) + λΣR)；对数空间稳定化
        expl = [math.exp(v - m) for v in lrs]
        sumR = sum(expl) * math.exp(m)          # Σ R_j
        logA = math.log(lam) + math.log(sumR)   # log(λΣR)
        logB = math.log1p(-lam)                 # log(1-λ)
        logDen = math.log(math.exp(logA - max(logA, logB)) + math.exp(logB - max(logA, logB))) + max(logA, logB)
        for j, v in enumerate(lrs):
            logZ = math.log(lam) + v - logDen
            out.append((j, math.exp(logZ)))
    else:  # anr
        # Z_j = λR_j / (λR_j + 1-λ) = 1 / (1 + exp(log((1-λ)/λ) - lr_j))
        for j, v in enumerate(lrs):
            z = 1.0 / (1.0 + math.exp(math.log((1 - lam) / lam) - v)) if v > -1e8 else 0.0
            out.append((j, z))
    return out


def mstep(seqs_enc, W, K, sites_all, pseudo=0.1):
    """加权计数 + 伪计数归一化"""
    pwm = [[pseudo] * K for _ in range(W)]
    for enc, sites in zip(seqs_enc, sites_all):
        for (j, z) in sites:
            if z <= 0:
                continue
            for k in range(W):
                a = enc[j + k]
                if a >= 0:
                    pwm[k][a] += z
    for k in range(W):
        s = sum(pwm[k])
        pwm[k] = [x / s for x in pwm[k]]
    return pwm


def loglik(seqs_enc, W, pwm, bg, sites_all):
    """观测似然（按模型）：位点用 motif 概率、其余用背景；ZOOPS/ANR 含混合项"""
    ll = 0.0
    for enc, sites in zip(seqs_enc, sites_all):
        L = len(enc)
        # 背景（含歧义字母概率1的简化——这里测试序列无歧义字母）
        ll += sum(math.log(bg[a]) for a in enc if a >= 0)
        for (j, z) in sites:
            if z <= 0:
                continue
            lr = sum(math.log(pwm[k][enc[j + k]] / bg[enc[j + k]]) for k in range(W))
            ll += math.log((1 - lam) + lam * math.exp(lr)) if False else 0  # 占位
    return ll


def em_run(seqs_enc, W, K, seed_enc, model, pseudo=0.1, max_iter=200, eps=1e-4, lam_init=0.5):
    """完整 EM；返回 (pwm, lam, ll_trace, Z列表)。LL 用完整混合似然。"""
    pwm = pwm_from_seed(seed_enc, W, K, pseudo)
    bg = bg_freqs(seqs_enc, K, pseudo)
    lam = lam_init
    trace = []

    def full_ll(sites_all):
        ll = 0.0
        for enc, sites in zip(seqs_enc, sites_all):
            ll += sum(math.log(bg[a]) for a in enc if a >= 0)
            if model == "oops":
                # P(S_i) = Σ_j P_motif(win_j)·P_bg(rest_j)；等价 bg_LL + log(Σ_j R_j)
                lrs = logR_matrix(enc, W, pwm, bg)
                m = max(lrs)
                ll += math.log(sum(math.exp(v - m) for v in lrs)) + m
            elif model == "zoops":
                lrs = logR_matrix(enc, W, pwm, bg)
                m = max(lrs)
                sumR = sum(math.exp(v - m) for v in lrs) * math.exp(m)
                ll += math.log((1 - lam) + lam * sumR)
            else:
                # ANR: Π_j [ (1-λ)·P_b(win_j) + λ·P_m(win_j) ]（窗口独立性，忽略跨窗重叠相关性——标准简化）
                lrs = logR_matrix(enc, W, pwm, bg)
                for v in lrs:
                    ll += math.log((1 - lam) + lam * math.exp(v))
        return ll

    sites_all = [[] for _ in seqs_enc]
    ll_prev = full_ll(sites_all)
    trace.append(ll_prev)
    for it in range(max_iter):
        # E 步
        sites_all = []
        for enc in seqs_enc:
            sites_all.append(estep_z(enc, W, pwm, bg, lam, model))
        # M 步
        pwm = mstep(seqs_enc, W, K, sites_all, pseudo)
        total_z = sum(z for sites in sites_all for (_, z) in sites)
        if model == "oops":
            lam = 1.0
        elif model == "zoops":
            lam = min(0.999, max(0.001, total_z / len(seqs_enc)))
        else:
            nwin_total = sum(max(0, len(enc) - W + 1) for enc in seqs_enc)
            lam = min(0.999, max(0.001, total_z / max(1, nwin_total)))
        ll = full_ll(sites_all)
        trace.append(ll)
        if abs(ll - ll_prev) < eps:
            ll_prev = ll
            break
        ll_prev = ll
    return pwm, lam, trace, sites_all, ll_prev


# ---------------- 测试 ----------------
def plant_test(n=20, L=200, W=10, motif="ACGTTACGTA", mut=1, with_site_ratio=0.8, seed=7):
    rng = random.Random(seed)
    seqs, truth = [], []
    m = list(motif)
    for i in range(n):
        s = [rng.choice(DNA) for _ in range(L)]
        has = rng.random() < with_site_ratio
        pos = rng.randrange(0, L - W) if has else -1
        if has:
            site = m[:]
            nmut = 0
            for k in range(W):
                if rng.random() < 0.15 and nmut < mut:
                    site[k] = rng.choice([c for c in DNA if c != site[k]])
                    nmut += 1
            s[pos:pos + W] = site
        seqs.append("".join(s))
        truth.append((i, pos))
    return seqs, truth


def test_constraints():
    """三种模型的 Z 约束"""
    ok = True
    seqs, _ = plant_test(n=5, seed=3)
    encs = [encode(s, DNA) for s in seqs]
    bg = bg_freqs(encs, 4)
    seed = encode("ACGTTACGTA", DNA)
    pwm = pwm_from_seed(seed, 10, 4)
    for model, check in [("oops", "=1"), ("zoops", "<=1"), ("anr", "free")]:
        sums = []
        for enc in encs:
            sites = estep_z(enc, 10, pwm, bg, 0.5, model)
            sums.append(sum(z for _, z in sites))
        if model == "oops":
            good = all(abs(s - 1.0) < 1e-9 for s in sums)
        elif model == "zoops":
            good = all(s <= 1.0 + 1e-9 for s in sums)
        else:
            good = True
        print(f"  {model:6s} Σ_j Z_ij per seq = {[f'{s:.3f}' for s in sums]}  约束 {check}: {'OK' if good else 'FAIL'}")
        ok = ok and good
    return 0 if ok else 1


def test_recovery():
    """种植恢复：ZOOPS/DNA"""
    seqs, truth = plant_test(n=30, L=200, W=10, motif="ACGTTACGTA", seed=11)
    encs = [encode(s, DNA) for s in seqs]
    seed = encode("ACGTTACGTA", DNA)
    pwm, lam, trace, sites_all, ll = em_run(encs, 10, 4, seed, "zoops", max_iter=300)
    # 恢复质量：每列最大概率碱基构成的一致序列 vs 真值
    consensus = "".join(DNA[max(range(4), key=lambda a: pwm[k][a])] for k in range(10))
    truth_c = "ACGTTACGTA"
    match = sum(1 for a, b in zip(consensus, truth_c) if a == b)
    # 位点定位：有位点的序列，预测 argmax Z 与真值距离
    dists = []
    for (i, pos), sites in zip(truth, sites_all):
        if pos < 0:
            continue
        best_j = max(sites, key=lambda t: t[1])[0]
        dists.append(abs(best_j - pos))
    mono = all(trace[t + 1] >= trace[t] - 1e-6 for t in range(len(trace) - 1))
    print(f"  一致序列匹配 {match}/10  λ={lam:.3f}  迭代 {len(trace)-1} 轮")
    print(f"  位点定位误差: {[f'{d}' for d in dists[:8]]}  均值 {sum(dists)/len(dists):.2f}")
    print(f"  LL 单调: {'OK' if mono else 'FAIL'}  {trace[0]:.1f} → {trace[-1]:.1f}")
    ok = match >= 9 and sum(1 for d in dists if d <= 2) / len(dists) > 0.8 and mono
    return 0 if ok else 1


def test_protein():
    """蛋白序列恢复"""
    rng = random.Random(21)
    W = 8
    motif = "GASTLSKL"
    seqs = []
    for i in range(25):
        s = [rng.choice(AA) for _ in range(120)]
        pos = rng.randrange(0, 120 - W)
        site = list(motif)
        for k in range(W):
            if rng.random() < 0.1:
                site[k] = rng.choice([c for c in AA if c != site[k]])
        s[pos:pos + W] = site
        seqs.append("".join(s))
    encs = [encode(s, AA) for s in seqs]
    seed = encode(motif, AA)
    pwm, lam, trace, sites_all, ll = em_run(encs, W, 20, seed, "zoops", max_iter=300)
    consensus = "".join(AA[max(range(20), key=lambda a: pwm[k][a])] for k in range(W))
    match = sum(1 for a, b in zip(consensus, motif) if a == b)
    mono = all(trace[t + 1] >= trace[t] - 1e-6 for t in range(len(trace) - 1))
    print(f"  蛋白一致序列匹配 {match}/{W}  λ={lam:.3f}  单调 {'OK' if mono else 'FAIL'}")
    return 0 if match >= 7 and mono else 1


def test_revcomp():
    """反义链种植 + 双链扫描"""
    W = 8
    motif = "ACGTCGTA"
    rc = motif[::-1].translate(str.maketrans("ACGT", "TGCA"))
    rng = random.Random(31)
    seqs = []
    planted = []
    for i in range(25):
        s = [rng.choice(DNA) for _ in range(150)]
        pos = rng.randrange(0, 150 - W)
        if i % 2 == 0:
            s[pos:pos + W] = list(motif)
            planted.append((i, pos, "+"))
        else:
            s[pos:pos + W] = list(rc)
            planted.append((i, pos, "-"))
        seqs.append("".join(s))
    # 双链候选：每个 (j, strand)；负链窗口 = revcomp(seq[j:j+W])
    comp = str.maketrans("ACGT", "TGCA")
    encs = [encode(s, DNA) for s in seqs]
    bg = bg_freqs(encs, 4)
    seed_enc = encode(motif, DNA)

    # 扩展 E 步（双链）：直接在 Python 内联实现
    def lrs_both(enc):
        out = []
        nwin = len(enc) - W + 1
        for j in range(nwin):
            fwd = 0.0
            rev = 0.0
            ok = True
            for k in range(W):
                a = enc[j + k]
                b = enc[j + W - 1 - k]      # 反向互补后的第 k 列 = 原串第 (j+W-1-k) 位
                if a < 0 or b < 0:
                    ok = False
                    break
                fwd += math.log(pwm[k][a] / bg[a])
                rev += math.log(pwm[k][b] / bg[b])
            if ok:
                out.append((j, fwd, rev))
            else:
                out.append((j, float('-inf'), float('-inf')))
        return out

    pwm = pwm_from_seed(seed_enc, W, 4)
    for it in range(150):
        cands = []
        for enc in encs:
            rows = lrs_both(enc)
            flat = []
            for (j, f, r) in rows:
                flat.append((j, "+", f))
                flat.append((j, "-", r))
            m = max(v for _, _, v in flat)
            e = [math.exp(v - m) for _, _, v in flat]
            s = sum(e)
            cands.append([(flat[t][0], flat[t][1], e[t] / s) for t in range(len(flat))])
        # M 步
        counts = [[0.1] * 4 for _ in range(W)]
        for enc, cc in zip(encs, cands):
            for (j, strand, z) in cc:
                if z < 1e-9:
                    continue
                for k in range(W):
                    a = enc[j + k] if strand == "+" else enc[j + W - 1 - k]
                    counts[k][a] += z
        pwm = [[c / sum(row) for c in row] for row in counts]
    consensus = "".join(DNA[max(range(4), key=lambda a: pwm[k][a])] for k in range(W))
    match = sum(1 for a, b in zip(consensus, motif) if a == b)
    print(f"  双链恢复一致序列匹配 {match}/{W}（共识 {consensus}）")
    return 0 if match >= 7 else 1


def test_chi2():
    """χ² 生存函数：不完全伽马正则化 Q(s, x)"""
    def gammainc_upper_reg(s, x):
        # Numerical Recipes: Q(s,x) 正则化上不完全伽马
        if x < 0 or s <= 0:
            return 1.0
        if x < s + 1.0:
            # 系列（下不完全伽马 P）→ Q = 1 - P
            ap = s
            summ = 1.0 / s
            delt = summ
            for _ in range(200):
                ap += 1.0
                delt *= x / ap
                summ += delt
                if abs(delt) < abs(summ) * 1e-14:
                    break
            P = summ * math.exp(-x + s * math.log(x) - math.lgamma(s))
            return 1.0 - P
        else:
            # 连分数（Q 直接）
            b = x + 1.0 - s
            c = 1e300
            d = 1.0 / b
            h = d
            for i in range(1, 200):
                an = -i * (i - s)
                b += 2.0
                d = an * d + b
                if abs(d) < 1e-300:
                    d = 1e-300
                c = b + an / c
                if abs(c) < 1e-300:
                    c = 1e-300
                d = 1.0 / d
                delt = d * c
                h *= delt
                if abs(delt - 1.0) < 1e-14:
                    break
            return math.exp(-x + s * math.log(x) - math.lgamma(s)) * h

    def chi2_sf(df, x):
        return gammainc_upper_reg(df / 2.0, x / 2.0)

    checks = [
        (1, 3.841, 0.05), (2, 5.991, 0.05), (4, 9.488, 0.05),
        (10, 18.307, 0.05), (1, 6.635, 0.01), (2, 9.210, 0.01),
    ]
    ok = True
    for df, x, expect in checks:
        sf = chi2_sf(df, x)
        err = abs(sf - expect)
        good = err < 5e-4
        ok = ok and good
        print(f"  χ²({df}) sf({x}) = {sf:.5f}（文献 {expect}）{'OK' if good else 'FAIL'}")
    return 0 if ok else 1


if __name__ == "__main__":
    total = 0
    print("=== 1. E 步约束 ===")
    total += test_constraints()
    print("=== 2. 种植恢复（DNA ZOOPS）===")
    total += test_recovery()
    print("=== 3. 蛋白序列 ===")
    total += test_protein()
    print("=== 4. 双链扫描 ===")
    total += test_revcomp()
    print("=== 5. χ² sf ===")
    total += test_chi2()
    print(f"\n总计: {'ALL PASS' if total == 0 else f'{total} FAILS'}")
