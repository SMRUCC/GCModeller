# ============================================================================
# validate_stats.py — Karlin-Altschul 统计参数数值解验证
# ----------------------------------------------------------------------------
# 验证目标（对应 MiniBlast/KarlinAltschul.vb 的算法）：
#   1. λ：二分法解 F(λ) = Σ_v prob(v)·e^{λv} = 1
#      验证：BLOSUM62 + Robinson-Robinson 背景频率 → λ ≈ 0.3176 (文献值)
#            blastn +2/-3 均一背景 → λ ≈ 0.6335 (自洽推导)
#   2. H = λ·Σ v·prob(v)·e^{λv}
#      验证：BLOSUM62 → H ≈ 0.4012 (文献值)
#   3. K：随机游走首达梯子常数 C = E[e^{λS_τ}; τ<∞]（截断格点 DP 迭代）
#      候选公式：K = λ·C 或 K = C，对拍文献 K ≈ 0.134 (BLOSUM62)
# ============================================================================
import math

RR = {
    'A': 0.074, 'R': 0.052, 'N': 0.045, 'D': 0.054, 'C': 0.013,
    'Q': 0.043, 'E': 0.047, 'G': 0.057, 'H': 0.024, 'I': 0.068,
    'L': 0.099, 'K': 0.058, 'M': 0.025, 'F': 0.050, 'P': 0.040,
    'S': 0.069, 'T': 0.059, 'W': 0.013, 'Y': 0.033, 'V': 0.066,
}
AA20 = "ARNDCQEGHILKMFPSTWYV"


def parse_matrix(text):
    lines = [l for l in text.strip().splitlines() if l.strip()]
    header = lines[0].split()
    m = {}
    for line in lines[1:]:
        parts = line.split()
        a = parts[0]
        for b, val in zip(header, parts[1:]):
            m[(a, b)] = int(val)
    return m


BLOSUM62 = """
   A  R  N  D  C  Q  E  G  H  I  L  K  M  F  P  S  T  W  Y  V  B  Z  X  *
A  4 -1 -2 -2  0 -1 -1  0 -2 -1 -1 -1 -1 -2 -1  1  0 -3 -2  0 -2 -1  0 -4
R -1  5  0 -2 -3  1  0 -2  0 -3 -2  2 -1 -3 -2 -1 -1 -3 -2 -3 -1  0 -1 -4
N -2  0  6  1 -3  0  0  0  1 -3 -3  0 -2 -3 -2  1  0 -4 -2 -3  3  0 -1 -4
D -2 -2  1  6 -3  0  2 -1 -1 -3 -4 -1 -3 -3 -1  0 -1 -4 -3 -3  4  1 -1 -4
C  0 -3 -3 -3  9 -3 -4 -3 -3 -1 -1 -3 -1 -2 -3 -1 -1 -2 -2 -1 -3 -3 -2 -4
Q -1  1  0  0 -3  5  2 -2  0 -3 -2  1  0 -3 -1  0 -1 -2 -1 -2  0  3 -1 -4
E -1  0  0  2 -4  2  5 -2  0 -3 -3  1 -2 -3 -1  0 -1 -3 -2 -2  1  4 -1 -4
G  0 -2  0 -1 -3 -2 -2  6 -2 -4 -4 -2 -3 -3 -2  0 -2 -2 -3 -3 -1 -2 -1 -4
H -2  0  1 -1 -3  0  0 -2  8 -3 -3 -1 -2 -1 -2 -1 -2 -2  2 -3  0  0 -1 -4
I -1 -3 -3 -3 -1 -3 -3 -4 -3  4  2 -3  1  0 -3 -2 -1 -3 -1  3 -3 -3 -1 -4
L -1 -2 -3 -4 -1 -2 -3 -4 -3  2  4 -2  2  0 -3 -2 -1 -2 -1  1 -4 -3 -1 -4
K -1  2  0 -1 -3  1  1 -2 -1 -3 -2  5 -1 -3 -1  0 -1 -3 -2 -2  0  1 -1 -4
M -1 -1 -2 -3 -1  0 -2 -3 -2  1  2 -1  5  0 -2 -1 -1 -1 -1  1 -3 -1 -1 -4
F -2 -3 -3 -3 -2 -3 -3 -3 -1  0  0 -3  0  6 -4 -2 -2  1  3 -1 -3 -3 -1 -4
P -1 -2 -2 -1 -3 -1 -1 -2 -2 -3 -3 -1 -2 -4  7 -1 -1 -4 -3 -2 -2 -1 -2 -4
S  1 -1  1  0 -1  0  0  0 -1 -2 -2  0 -1 -2 -1  4  1 -3 -2 -2  0  0  0 -4
T  0 -1  0 -1 -1 -1 -1 -2 -2 -1 -1 -1 -1 -2 -1  1  5 -2 -2  0 -1 -1  0 -4
W -3 -3 -4 -4 -2 -2 -3 -2 -2 -3 -2 -3 -1  1 -4 -3 -2 11  2 -3 -4 -3 -2 -4
Y -2 -2 -2 -3 -2 -1 -2 -3  2 -1 -1 -2 -1  3 -3 -2 -2  2  7 -1 -3 -2 -1 -4
V  0 -3 -3 -3 -1 -2 -2 -3 -3  3  1 -2  1 -1 -2 -2  0 -3 -1  4 -3 -2 -1 -4
B -2 -1  3  4 -3  0  1 -1  0 -3 -4  0 -3 -3 -2  0 -1 -4 -3 -3  4  1 -1 -4
Z -1  0  0  1 -3  3  4 -2  0 -3 -3  1 -1 -3 -1  0 -1 -3 -2 -2  1  4 -1 -4
X  0 -1 -1 -1 -2 -1 -1 -1 -1 -1 -1 -1 -1 -1 -2  0  0 -2 -1 -1 -1 -1 -1 -4
* -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4 -4  1
"""


def score_histogram(score_fn, alphabet, freqs):
    hist = {}
    for a in alphabet:
        for b in alphabet:
            v = score_fn(a, b)
            hist[v] = hist.get(v, 0.0) + freqs[a] * freqs[b]
    return hist


def solve_lambda(hist, tol=1e-14):
    def F(lam):
        return sum(p * math.exp(lam * v) for v, p in hist.items())
    lo, hi = 1e-8, 1.0
    while F(hi) < 1.0:
        hi *= 2.0
        if hi > 1e6:
            raise RuntimeError("lambda diverged")
    for _ in range(200):
        mid = 0.5 * (lo + hi)
        if F(mid) < 1.0:
            lo = mid
        else:
            hi = mid
        if hi - lo < tol:
            break
    return 0.5 * (lo + hi)


def solve_H(hist, lam):
    return lam * sum(v * p * math.exp(lam * v) for v, p in hist.items())


def ladder_C(hist, lam, decay_target=1e-10, max_iter=50000):
    """C = E[e^{λS_τ}; τ<∞] — 首达梯子常数，截断格点 DP 迭代"""
    scores = sorted(hist.keys())
    probs = [hist[v] for v in scores]
    B = int(math.ceil(-math.log(decay_target) / lam)) + max(abs(min(scores)), 1)
    g = [0.0] * (B + 1)
    for it in range(max_iter):
        delta = 0.0
        new = [0.0] * (B + 1)
        for idx in range(B + 1):
            k = idx - B
            acc = 0.0
            for v, p in zip(scores, probs):
                nk = k + v
                if nk > 0:
                    acc += p * math.exp(lam * nk)
                elif nk + B >= 0:
                    acc += p * g[nk + B]
                # nk 深于截断线：贡献 0
            new[idx] = acc
            d = abs(acc - g[idx])
            if d > delta:
                delta = d
        g = new
        if delta < 1e-15:
            break
    return g[B]


def main():
    m = parse_matrix(BLOSUM62)
    hist = score_histogram(lambda a, b: m[(a, b)], AA20, RR)
    lam = solve_lambda(hist)
    H = solve_H(hist, lam)
    C = ladder_C(hist, lam)
    print("BLOSUM62 (RR freqs):")
    print(f"  lambda = {lam:.6f}   (pub 0.3176)  dev {abs(lam-0.3176)/0.3176*100:.2f}%")
    print(f"  H      = {H:.6f}   (pub 0.4012)  dev {abs(H-0.4012)/0.4012*100:.2f}%")
    print(f"  C      = {C:.6f}")
    print(f"  K = l*C = {lam*C:.6f}   (pub 0.134)")
    K = lam * C
    S = 50
    E1 = K * 1e9 * math.exp(-lam * S)
    Sprime = (lam * S - math.log(K)) / math.log(2)
    E2 = 1e9 * 2 ** (-Sprime)
    print(f"  identity: E1={E1:.6g} E2={E2:.6g} ok={abs(E1-E2)<1e-4*E1}")

    hist2 = {}
    for a in "ACGT":
        for b in "ACGT":
            v = 2 if a == b else -3
            hist2[v] = hist2.get(v, 0.0) + 0.0625
    lam2 = solve_lambda(hist2)
    H2 = solve_H(hist2, lam2)
    C2 = ladder_C(hist2, lam2)
    print(f"\nblastn +2/-3: lambda={lam2:.6f} (theory 0.6335)  H={H2:.6f}  K(l*C)={lam2*C2:.6f} (pub~0.41)  K(C)={C2:.6f}")

    hist3 = {}
    for a in "ACGT":
        for b in "ACGT":
            v = 1 if a == b else -2
            hist3[v] = hist3.get(v, 0.0) + 0.0625
    lam3 = solve_lambda(hist3)
    C3 = ladder_C(hist3, lam3)
    print(f"blastn +1/-2: lambda={lam3:.6f}  K(l*C)={lam3*C3:.6f}")


if __name__ == "__main__":
    main()
