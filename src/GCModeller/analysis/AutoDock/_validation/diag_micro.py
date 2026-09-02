import math, random
import validate_physics as V

# ---- 微测试A: 单原子对的力 ----
p = (2.0, 0.5, 0.3)
q = (0.0, 0.0, 0.0)
ti, tj = 'C', 'OA'
Ri, Rj = 1.9, 1.7
e, f = V.score_pair(p, q, ti, tj, Ri, Rj)
h = 1e-7
fx = (V.score_pair((p[0]+h, p[1], p[2]), q, ti, tj, Ri, Rj)[0] -
      V.score_pair((p[0]-h, p[1], p[2]), q, ti, tj, Ri, Rj)[0]) / (2*h)
fy = (V.score_pair((p[0], p[1]+h, p[2]), q, ti, tj, Ri, Rj)[0] -
      V.score_pair((p[0], p[1]-h, p[2]), q, ti, tj, Ri, Rj)[0]) / (2*h)
print(f"A: 解析F=({f[0]:.6f},{f[1]:.6f},{f[2]:.6f})  FD-dE/dx=({-fx:.6f},{-fy:.6f})  "
      f"{'OK' if abs(f[0]+fx)<1e-5 and abs(f[1]+fy)<1e-5 else 'FAIL'}")

# ---- 微测试B: 两原子刚体配体旋转 ----
coords0 = [(0.0,0.0,0.0),(1.5,0.0,0.0)]
lt = ['C','OA']
rec = [(4.0,0.5,0.2),(0.0,3.0,0.5),(-3.0,-1.0,0.3)]
rt = ['OA','C','N']
trans = (0.4,-0.2,0.1)
rotvec = (0.2,-0.1,0.3)
tors = []
tb, tbr = [], []

def f_of(t, rv):
    e,_ = V.energy_grad(coords0, t, rv, tors, rec, rt, lt, tb, tbr)
    return e

_, ga = V.energy_grad(coords0, trans, rotvec, tors, rec, rt, lt, tb, tbr)
# FD 增量语义
for k in range(3):
    dr = [h if i==k else 0.0 for i in range(3)]
    _, r2 = V.apply_increment(trans, rotvec, (0,0,0), dr)
    _, r1 = V.apply_increment(trans, rotvec, (0,0,0), (-dr[0],-dr[1],-dr[2]))
    fd = (f_of(trans, r2) - f_of(trans, r1)) / (2*h)
    print(f"B: 旋转{k}: 解析={ga[3+k]:.6f}  FD={fd:.6f}  {'OK' if abs(ga[3+k]-fd)<1e-5 else 'FAIL'}")

# ---- 微测试C: 有扭转的两原子branch ----
# 配体: 0-1-2-3 链, 扭转键 (1,2), branch={2,3}
coords0 = [(0.0,0.0,0.0),(1.5,0.0,0.0),(3.0,0.0,0.0),(4.5,0.0,0.0)]
lt = ['C','C','C','OA']
allb = [(0,1),(1,2),(2,3)]
tb, tbr = V.TorsionTree.build(allb, 4, [1])
tors = [0.7]
trans = (0.4,-0.2,0.1)
rotvec = (0.2,-0.1,0.3)
_, ga = V.energy_grad(coords0, trans, rotvec, tors, rec, rt, lt, tb, tbr)
fd = (f_of2 := None)
def f3(t, rv, to):
    e,_ = V.energy_grad(coords0, t, rv, to, rec, rt, lt, tb, tbr)
    return e
tp = list(tors); tp[0] += h
tm = list(tors); tm[0] -= h
fd = (f3(trans, rotvec, tp) - f3(trans, rotvec, tm)) / (2*h)
print(f"C: 扭转0: 解析={ga[6]:.6f}  FD={fd:.6f}  {'OK' if abs(ga[6]-fd)<1e-5 else 'FAIL'}")

# 扭转的构成检查：手动旋转 branch 比对 apply_pose
pos0, _ = V.apply_pose(coords0, trans, rotvec, tb, tbr, [0.7])
pos1, _ = V.apply_pose(coords0, trans, rotvec, tb, tbr, [0.7 + h])
# branch 原子 2,3 应绕 pos0[1]-pos0[?] 轴转
print(f"C2: atom2 位移 = {tuple(round(pos1[2][k]-pos0[2][k],6) for k in range(3))}")
print(f"C2: atom3 位移 = {tuple(round(pos1[3][k]-pos0[3][k],6) for k in range(3))}")
