import math, random
import validate_physics as V

coords0, lt, rec, rt, tb, tbr = V.build_test_system()
rng = random.Random(11)
trans = (rng.uniform(-1,1), rng.uniform(-1,1), rng.uniform(-1,1))
rotvec = (rng.uniform(-0.3,0.3), rng.uniform(-0.3,0.3), rng.uniform(-0.3,0.3))
tors = [rng.uniform(-1,1), rng.uniform(-1,1)]

pos, _ = V.apply_pose(coords0, trans, rotvec, tb, tbr, tors)

# 诊断：接近 cutoff / kink 的原子对
near_cut = 0; near_kink = 0
for i, p in enumerate(pos):
    for j, q in enumerate(rec):
        r = math.sqrt(sum((p[k]-q[k])**2 for k in range(3)))
        if abs(r - 8.0) < 1e-3: near_cut += 1
        d = r - V.RADII[lt[i]] - V.RADII[rt[j]]
        for kk in (-0.7, 0.0, 0.5, 1.5):
            if abs(d - kk) < 1e-3: near_kink += 1
print(f"距 cutoff<1e-3 的对: {near_cut}   距 kink<1e-3 的对: {near_kink}")

_, ga = V.energy_grad(coords0, trans, rotvec, tors, rec, rt, lt, tb, tbr)
print(f"解析旋转梯度: {['%.6f' % v for v in ga[3:6]]}")
print(f"解析扭转梯度: {['%.6f' % v for v in ga[6:]]}")
for h in (1e-3, 1e-4, 1e-5, 1e-6):
    _, gn = V.numeric_grad(coords0, trans, rotvec, tors, rec, rt, lt, tb, tbr, h=h)
    print(f"h={h:.0e}: 数值旋转 {['%.6f' % v for v in gn[3:6]]}  数值扭转 {['%.6f' % v for v in gn[6:]]}")
