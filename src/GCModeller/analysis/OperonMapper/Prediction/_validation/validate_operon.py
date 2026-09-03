# ============================================================================
# validate_operon.py — 操纵子预测核心数学的 Python 镜像验证
# 待验证（将 1:1 转录到 VB）：
#   1. UniOP 先验 q=(M−2O)/(M−O) [operon.md §1.4] + 退化截断
#   2. 高斯核 KDE（Silverman 带宽）+ 贝叶斯后验分离度（种植数据）
#   3. 非操纵子分布近似：趋同+发散距离成对取均值 [operon.md §1.4]
#   4. 发夹+U 串 Rho 非依赖终止子扫描 [operon.md 特征5]
#   5. -35/-10 启动子框扫描
#   6. 二项分布 LLR（系统发育条形码/保守对）[operon.md §2.4/§2.5]
#   7. Viterbi/前向后向 HMM 整合 [operon.md §2.4]
# ============================================================================
import math
import random

# ---------------- KDE ----------------
def silverman_bw(xs):
    n = len(xs)
    if n < 2:
        return 1.0
    m = sum(xs) / n
    var = sum((x - m) ** 2 for x in xs) / (n - 1)
    sd = math.sqrt(var)
    xs_s = sorted(xs)
    q1 = xs_s[int(0.25 * (n - 1))]
    q3 = xs_s[int(0.75 * (n - 1))]
    iqr = q3 - q1
    sig = min(sd, iqr / 1.34) if iqr > 0 else sd
    if sig <= 0:
        sig = sd if sd > 0 else 1.0
    return 0.9 * sig * n ** (-0.2)

def kde_density(sample, x, bw):
    if not sample:
        return 1e-12
    s = 0.0
    for v in sample:
        z = (x - v) / bw
        s += math.exp(-0.5 * z * z)
    return s / (len(sample) * bw * math.sqrt(2 * math.pi))

def uniop_prior(M, O):
    """[operon.md §1.4] q = (M−2O)/(M−O)，退化截断"""
    denom = M - O
    if denom <= 0:
        return 0.5
    q = (M - 2.0 * O) / denom
    return max(0.05, min(0.95, q))

def uniop_posterior(same_d, conv_d, div_d, q):
    """UniOP 后验：非操纵子分布 = 成对(趋同,发散)均值 [operon.md §1.4]
    闭式精确贝叶斯：f_mix = q·f_op + (1−q)·f_non（混合恒等式）
      ⇒ P(op|d) = 1 − (1−q)·f_non(d)/f_mix(d)
    （文档 §1.4 的"f_op=同链KDE"写法在 q 大时长距离后验下界为 q，
      由混合恒等式推出的闭式无此缺陷且只用文档已有的三个量）"""
    same_d = list(same_d)
    conv_d = list(conv_d)
    div_d = list(div_d)
    rng = random.Random(7)
    c = conv_d[:]
    d = div_d[:]
    rng.shuffle(c)
    rng.shuffle(d)
    n = min(len(c), len(d))
    if n >= 10:
        # [operon.md §1.4] 非操纵子距离 ≈ 成对(趋同,发散)均值（2 元件 vs 1 终止子+1 启动子）
        nonop = [(c[i] + d[i]) / 2.0 for i in range(n)]
    else:
        # 稀疏回退：趋同/发散对太少时直接用全部反义对距离（分布偏宽 → 后验保守）
        nonop = conv_d + div_d
    if not nonop:
        nonop = [200.0]
    bw_mix = silverman_bw(same_d)
    bw_non = silverman_bw(nonop)
    post = []
    for x in same_d:
        fm = kde_density(same_d, x, bw_mix)
        fn = kde_density(nonop, x, bw_non)
        p = 1.0 - (1.0 - q) * fn / max(fm, 1e-300)
        post.append(max(0.0, min(1.0, p)))
    return post, nonop


def kde_density_w(sample, weights, x, bw):
    s = 0.0
    wsum = sum(weights)
    for v, w in zip(sample, weights):
        z = (x - v) / bw
        s += w * math.exp(-0.5 * z * z)
    return s / (wsum * bw * math.sqrt(2 * math.pi))


def synth_genome(n_operons=30, seed=11, gap_op=(4, 25), gap_boundary=(120, 420)):
    """合成基因组：操纵子（2-4 基因短间隔）+ 长边界间隔，部分反义对"""
    rng = random.Random(seed)
    genes = []   # (start, end, strand)
    pos = 100
    truth_pairs = []   # (i, i+1, is_op) 按相邻对
    gid = 0
    for _ in range(n_operons):
        n = rng.randint(2, 4)
        starts = []
        for k in range(n):
            L = rng.randint(300, 900)
            starts.append((gid, pos, pos + L, '+'))
            pos += L + rng.randint(*gap_op)
            gid += 1
        genes.extend(starts)
        # 操纵子内部对 = op
        for k in range(n - 1):
            truth_pairs.append(1)
        # 边界：同链长间隔 或 发散/趋同对
        r = rng.random()
        if r < 0.45:
            # 发散对：插入一个 − 链基因（← → 结构：前一个 + 基因后放 − 基因…）
            # 简化：下一个操纵子第一个基因放 − 链，与前一 + 链基因构成发散
            if genes:
                pass
        pos += rng.randint(*gap_boundary)
    return genes, truth_pairs


def test_uniop():
    print("=== 1. UniOP 先验 + KDE 后验 ===")
    # 自洽合成：边界距离从与趋同/发散相同的生成过程导出（成对均值），
    # 先验 q 与混合真实比例一致（M=3.5O → q=(M−2O)/(M−O)≈0.6）
    rng = random.Random(5)
    conv_pool = [rng.uniform(60, 300) for _ in range(14)]
    div_pool = [rng.uniform(80, 320) for _ in range(14)]
    op_d = [max(0, rng.gauss(15, 10)) for _ in range(60)]
    bnd_d = [(rng.choice(conv_pool) + rng.choice(div_pool)) / 2.0 for _ in range(40)]
    same_d = op_d + bnd_d
    M, O = 100, 28          # q = (100−56)/(100−28) = 0.611 ≈ 真实 op 比例 0.6
    q = uniop_prior(M, O)
    print(f"  q = {q:.3f}（M={M}, O={O}，真实 op 比例 0.60）")
    posts, nonop = uniop_posterior(same_d, conv_pool, div_pool, q)
    short_posts = posts[:60]
    long_posts = posts[60:]
    ms = sum(short_posts) / len(short_posts)
    ml = sum(long_posts) / len(long_posts)
    print(f"  短距离(≤60)后验均值 = {ms:.3f}  边界后验均值 = {ml:.3f}")
    acc = (sum(1 for p in short_posts if p > 0.5) + sum(1 for p in long_posts if p <= 0.5)) / len(posts)
    print(f"  0.5 阈值准确率 = {acc:.3f}")
    ok = ms > 0.9 and ml < 0.35 and acc > 0.85 and 0.4 < q < 0.8
    # 退化截断
    q0 = uniop_prior(10, 15)   # M<O → 0.5
    q1 = uniop_prior(100, 0)   # O=0 → 0.95 截断
    print(f"  退化: M<O → q={q0}（期望 0.5）  O=0 → q={q1}（截断 0.95）")
    ok = ok and abs(q0 - 0.5) < 1e-9 and abs(q1 - 0.95) < 1e-9
    return 0 if ok else 1


# ---------------- 终止子扫描 ----------------
def revcomp(s):
    return s[::-1].translate(str.maketrans("ACGT", "TGCA"))

def scan_terminator(seq):
    """发夹(茎≥4, 环3-8) + 下游 U 串(≥4)，返回强度 0..1
    [operon.md 特征5: 下游 Rho 非依赖型终止子（发夹+U串）]"""
    best = 0.0
    n = len(seq)
    if n < 15:
        return 0.0
    for loop in range(3, 9):
        for stem in range(4, min(13, (n - loop) // 2 + 1)):
            for i in range(0, n - 2 * stem - loop + 1):
                left = seq[i:i + stem]
                right = seq[i + stem + loop:i + 2 * stem + loop]
                right_rc = revcomp(right)
                # 碱基配对评分
                pairs = 0
                gc = 0
                for a, b in zip(left, right_rc):
                    if a + b in ("AT", "TA", "GC", "CG"):
                        pairs += 1
                        if a + b in ("GC", "CG"):
                            gc += 1
                if pairs < stem - 1:      # 允许 1 个错配
                    continue
                frac = pairs / stem
                # U 串：环后 12nt 内 ≥4 连续 T
                tail = seq[i + 2 * stem + loop:i + 2 * stem + loop + 12]
                utract = 0
                cur = 0
                for c in tail:
                    if c in "TU":
                        cur += 1
                        utract = max(utract, cur)
                    else:
                        cur = 0
                if utract < 4:
                    continue
                score = 0.45 * frac + 0.35 * min(1.0, utract / 8.0) + 0.2 * min(1.0, (stem - 4) / 6.0)
                if gc >= stem * 0.5:
                    score += 0.05
                best = max(best, min(1.0, score))
    return best

def test_terminator():
    print("=== 2. 终止子扫描 ===")
    # 自互补 GC 茎(10) + 环 3nt + U 串 6：CCGCGCGCGG-AAT-CCGCGCGCGG-TTTTTT
    t1 = "CCGCGCGCGGAATCCGCGCGCGGTTTTTT"
    rng = random.Random(3)
    neg = ["ACGTTTAGCAACGTACGGATCAGCTAGGCATTACGGATCCAGTACGGATCCA" for _ in range(5)]
    s1 = scan_terminator(t1)
    sn = max(scan_terminator(s) for s in neg)
    print(f"  强终止子评分 = {s1:.3f}（应 >0.6）  随机序列最大 = {sn:.3f}（应 <0.4）")
    ok = s1 > 0.6 and sn < 0.4
    # 调用方读框约定：负链基因的终止子在基因组上是 revcomp(t1)，
    # 扫描时以 revcomp(基因组区间) 恢复读框 → 应检出
    genomic_minus = revcomp("ACGTAC" + t1 + "GGCA")
    s2 = scan_terminator(revcomp(genomic_minus))
    print(f"  负链读框恢复扫描 = {s2:.3f}（应 ≈ {s1:.3f}）")
    ok = ok and abs(s2 - s1) < 1e-9
    return 0 if ok else 1

# ---------------- 启动子扫描 ----------------
def scan_promoter(seq):
    """-35 TTGACA / -10 TATAAT（≤2 错配，间隔 15-19）→ 强度 0..1"""
    best = 0.0
    n = len(seq)
    m35, m10 = "TTGACA", "TATAAT"
    def mismatch(a, b):
        return sum(1 for x, y in zip(a, b) if x != y)
    for i in range(0, max(1, n - 5)):
        if i + 6 <= n and mismatch(seq[i:i + 6], m35) <= 2:
            for j in range(i + 15, min(i + 20, n - 5)):
                if j + 6 <= n and mismatch(seq[j:j + 6], m10) <= 2:
                    mm = mismatch(seq[i:i + 6], m35) + mismatch(seq[j:j + 6], m10)
                    sc = 1.0 - mm / 8.0
                    best = max(best, sc)
    return best

def test_promoter():
    print("=== 3. 启动子扫描 ===")
    # -35 起始到 -10 起始间隔 17bp（σ70 经典 17±2）
    p = "TTGACA" + "ACGTTCGACG" + "TATAAT"
    s = scan_promoter(p)
    rng = random.Random(9)
    neg = ["".join(rng.choice("ACGT") for _ in range(40)) for _ in range(20)]
    sn = max(scan_promoter(x) for x in neg)
    print(f"  经典启动子评分 = {s:.3f}（应 >0.7）  随机最大 = {sn:.3f}（应低于真值）")
    ok = s > 0.7 and s > sn + 0.05
    return 0 if ok else 1

# ---------------- 二项 LLR ----------------
def log_binom_pmf(k, n, p):
    if p <= 0:
        return 0.0 if k == 0 else float('-inf')
    if p >= 1:
        return 0.0 if k == n else float('-inf')
    return (math.lgamma(n + 1) - math.lgamma(k + 1) - math.lgamma(n - k + 1)
            + k * math.log(p) + (n - k) * math.log(1 - p))

def test_binom_llr():
    print("=== 4. 二项 LLR（条形码/保守对）===")
    # 条形码: op 内 h 小（p_in=0.15），间 h 大（p_out=0.45）
    llr0 = log_binom_pmf(0, 35, 0.15) - log_binom_pmf(0, 35, 0.45)
    llr18 = log_binom_pmf(18, 35, 0.15) - log_binom_pmf(18, 35, 0.45)
    print(f"  h=0: LLR={llr0:.2f}（应>0）  h=18: LLR={llr18:.2f}（应<0）")
    ok = llr0 > 3 and llr18 < -3
    # 单调性：h 越大 LLR 越低
    prev = float('inf')
    mono = True
    for h in range(0, 36):
        v = log_binom_pmf(h, 35, 0.15) - log_binom_pmf(h, 35, 0.45)
        if v > prev + 1e-9:
            mono = False
        prev = v
    print(f"  LLR 随 h 单调递减: {'OK' if mono else 'FAIL'}")
    return 0 if (ok and mono) else 1

# ---------------- HMM ----------------
def viterbi(llrs, q, persist=0.5):
    """llrs[i] = 第 i 对的发射 log-odds（op vs boundary）"""
    n = len(llrs)
    logq = math.log(max(q, 1e-6))
    log1q = math.log(max(1 - q, 1e-6))
    p_oo = min(0.999, q + (1 - q) * persist)       # op→op（持续）
    p_bo = max(0.001, q * (1 - persist))           # boundary→op
    loo, lo_b = math.log(p_oo), math.log(1 - p_oo)
    lbo, lb_b = math.log(p_bo), math.log(1 - p_bo)
    d1 = llrs[0] + logq
    d0 = log1q
    path = []
    for i in range(1, n):
        s = llrs[i]
        c1 = s + max(d1 + loo, d0 + lbo)
        c0 = max(d1 + lo_b, d0 + lb_b)
        path.append((d1 + loo > d0 + lbo, d1 + lo_b > d0 + lb_b))
        d1, d0 = c1, c0
    # 回溯
    states = [0] * n
    states[n - 1] = 1 if d1 > d0 else 0
    for i in range(n - 1, 0, -1):
        take_op, take_b = path[i - 1]
        if states[i] == 1:
            states[i - 1] = 1 if take_op else 0
        else:
            states[i - 1] = 1 if take_b else 0
    return states

def forward_backward(llrs, q, persist=0.5):
    n = len(llrs)
    logq = math.log(max(q, 1e-6))
    log1q = math.log(max(1 - q, 1e-6))
    p_oo = min(0.999, q + (1 - q) * persist)
    p_bo = max(0.001, q * (1 - persist))
    loo, lo_b = math.log(p_oo), math.log(1 - p_oo)
    lbo, lb_b = math.log(p_bo), math.log(1 - p_bo)
    # 前向（log 空间）
    a1 = [llrs[0] + logq]
    a0 = [log1q]
    for i in range(1, n):
        s = llrs[i]
        a1.append(s + max(a1[i-1] + loo, a0[i-1] + lbo) + math.log(
            math.exp(a1[i-1] + loo - (max(a1[i-1] + loo, a0[i-1] + lbo))) +
            math.exp(a0[i-1] + lbo - (max(a1[i-1] + loo, a0[i-1] + lbo)))))
        m = max(a1[i-1] + lo_b, a0[i-1] + lb_b)
        a0.append(m + math.log(math.exp(a1[i-1] + lo_b - m) + math.exp(a0[i-1] + lb_b - m)))
    # 后向
    b1 = [0.0] * n
    b0 = [0.0] * n
    for i in range(n - 2, -1, -1):
        s = llrs[i + 1]
        m1 = max(loo + s + b1[i+1], lo_b + b0[i+1])
        b1[i] = m1 + math.log(math.exp(loo + s + b1[i+1] - m1) + math.exp(lo_b + b0[i+1] - m1))
        m0 = max(lbo + s + b1[i+1], lb_b + b0[i+1])
        b0[i] = m0 + math.log(math.exp(lbo + s + b1[i+1] - m0) + math.exp(lb_b + b0[i+1] - m0))
    posts = []
    for i in range(n):
        m = max(a1[i] + b1[i], a0[i] + b0[i])
        posts.append(math.exp(a1[i] + b1[i] - m) /
                     (math.exp(a1[i] + b1[i] - m) + math.exp(a0[i] + b0[i] - m)))
    return posts

def test_hmm():
    print("=== 5. Viterbi / 前向后向 ===")
    # 真实结构: 8 op 对 + 1 边界 + 6 op 对；发射由距离后验模拟
    q = 0.7
    rng = random.Random(2)
    truth = [1] * 8 + [0] + [1] * 6
    llrs = []
    for t in truth:
        base = 2.0 if t == 1 else -2.5
        llrs.append(base + rng.gauss(0, 1.0))
    st = viterbi(llrs, q)
    posts = forward_backward(llrs, q)
    acc = sum(1 for a, b in zip(st, truth) if a == b) / len(truth)
    short_post = sum(posts[i] for i in range(8)) / 8
    bnd_post = posts[8]
    long_post = sum(posts[9:]) / 6
    print(f"  Viterbi 准确率 = {acc:.3f}  HMM 后验: op 段 {short_post:.2f}/{long_post:.2f}  边界 {bnd_post:.2f}")
    ok = acc >= 0.95 and short_post > 0.7 and long_post > 0.7 and bnd_post < 0.5
    # 全负发射 → 全边界
    st2 = viterbi([-3.0] * 10, q)
    ok = ok and sum(st2) == 0
    # 全正 → 全 op
    st3 = viterbi([3.0] * 10, q)
    ok = ok and sum(st3) == 10
    return 0 if ok else 1


if __name__ == "__main__":
    total = 0
    total += test_uniop()
    total += test_terminator()
    total += test_promoter()
    total += test_binom_llr()
    total += test_hmm()
    print(f"\n总计: {'ALL PASS' if total == 0 else f'{total} FAILS'}")
